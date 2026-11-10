param(
    [string[]]$Group = @(),
    [string[]]$CanvasGroup = @('cave-layers', 'terrain-props')
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Add-Type -AssemblyName System.Drawing
if (-not ('MiniGameArtAlphaBounds' -as [type])) {
    # Scan managed BGRA rows once instead of calling GetPixel for every pixel.
    Add-Type -TypeDefinition @'
public static class MiniGameArtAlphaBounds
{
    public static int[] Scan(byte[] pixels, int width, int height, int stride)
    {
        int left = width, top = height, right = -1, bottom = -1;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (pixels[y * stride + x * 4 + 3] > 16)
                {
                    if (x < left) left = x;
                    if (y < top) top = y;
                    if (x > right) right = x;
                    if (y > bottom) bottom = y;
                }
        return right < 0 ? new int[] { 0, 0, 0, 0 } :
            new int[] { left, top, right - left + 1, bottom - top + 1 };
    }
}
'@
}

$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$artRoot = Join-Path $repo 'Art/MinigameRefresh'
$assetRoot = Join-Path $repo 'HerosManager/Assets/04.Images/MiniGame'
$reviewRoot = Join-Path $repo 'HerosManager/Logs/art-review'
$originalRoot = Join-Path $reviewRoot 'originals'
$manifestPath = Join-Path $artRoot 'manifest.json'
$reportPath = Join-Path $reviewRoot 'import-report.json'
$utf8 = [Text.UTF8Encoding]::new($false)
$invariant = [Globalization.CultureInfo]::InvariantCulture

function Resolve-ChildPath([string]$Root, [string]$Relative) {
    if ([IO.Path]::IsPathRooted($Relative)) { throw "Expected relative asset path: $Relative" }
    $boundary = [IO.Path]::GetFullPath($Root).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    $path = [IO.Path]::GetFullPath((Join-Path $Root $Relative))
    if (-not $path.StartsWith($boundary, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Asset path escapes its root: $Relative"
    }
    return $path
}

function Get-AlphaBounds([Drawing.Bitmap]$Bitmap) {
    $rect = [Drawing.Rectangle]::new(0, 0, $Bitmap.Width, $Bitmap.Height)
    $data = $Bitmap.LockBits($rect, [Drawing.Imaging.ImageLockMode]::ReadOnly, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    try {
        $stride = [Math]::Abs($data.Stride)
        $bytes = [byte[]]::new($stride * $Bitmap.Height)
        # Row-wise copying also handles a negative native stride without changing orientation.
        for ($y = 0; $y -lt $Bitmap.Height; $y++) {
            [Runtime.InteropServices.Marshal]::Copy([IntPtr]::Add($data.Scan0, $y * $data.Stride), $bytes, $y * $stride, $stride)
        }
        $bounds = [MiniGameArtAlphaBounds]::Scan($bytes, $Bitmap.Width, $Bitmap.Height, $stride)
        return [Drawing.Rectangle]::new($bounds[0], $bounds[1], $bounds[2], $bounds[3])
    }
    finally { $Bitmap.UnlockBits($data) }
}

function Get-MetaGuid([string]$Text) {
    $match = [regex]::Match($Text, '(?m)^guid: ([0-9a-fA-F]{32})\s*$')
    if (-not $match.Success) { throw 'Texture metadata has no valid GUID.' }
    return $match.Groups[1].Value
}

function New-ScaledMeta([string]$Text, [int]$Scale) {
    $ppu = [regex]::Match($Text, '(?m)^  spritePixelsToUnits: ([0-9.]+)\s*$')
    if (-not $ppu.Success) { throw 'Texture metadata has no spritePixelsToUnits.' }
    $scaledPpu = ([double]::Parse($ppu.Groups[1].Value, $invariant) * $Scale).ToString($invariant)
    $result = [regex]::Replace($Text, '(?m)^(  spritePixelsToUnits:) [^\r\n]+', "`$1 $scaledPpu")
    $result = [regex]::Replace($result, '(?m)^(    filterMode:) [^\r\n]+', '$1 1')
    $result = [regex]::Replace($result, '(?m)^(    textureCompression:) [^\r\n]+', '$1 0')
    if ($result -match '(?m)^  spriteMode: 2\s*$') {
        # Only the pixel-coordinate rectangle changes; IDs, names, normalized pivots and borders remain intact.
        $result = [regex]::Replace($result,
            '(?m)(^      rect:\r?\n        serializedVersion: \d+\r?\n)(        x: [^\r\n]+\r?\n        y: [^\r\n]+\r?\n        width: [^\r\n]+\r?\n        height: [^\r\n]+)',
            [Text.RegularExpressions.MatchEvaluator]{ param($rectMatch)
                $rectMatch.Groups[1].Value + [regex]::Replace($rectMatch.Groups[2].Value,
                    '(?m)^(        (?:x|y|width|height):) ([0-9.]+)',
                    [Text.RegularExpressions.MatchEvaluator]{ param($numberMatch)
                        $numberMatch.Groups[1].Value + ' ' +
                            ([double]::Parse($numberMatch.Groups[2].Value, $invariant) * $Scale).ToString($invariant)
                    })
            })
    }
    return $result
}

$allGroups = @(Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json)
if ($allGroups.Count -eq 0) { throw 'The art manifest is empty.' }
$knownNames = @($allGroups | ForEach-Object { $_.name })
foreach ($name in $Group) {
    if ($name -notin $knownNames) { throw "Unknown art group: $name" }
}
$selected = @($allGroups | Where-Object { $Group.Count -eq 0 -or $_.name -in $Group })
$pending = [Collections.Generic.List[object]]::new()
$destinations = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)

# Preflight every selected input before producing or replacing any project asset.
foreach ($sheet in $selected) {
    if ($sheet.name -notmatch '^[a-z0-9-]+$') { throw "Invalid sheet name: $($sheet.name)" }
    if ($sheet.columns -lt 1 -or $sheet.rows -lt 1 -or $sheet.files.Count -gt ($sheet.columns * $sheet.rows)) {
        throw "Invalid atlas grid: $($sheet.name)"
    }
    $atlasPath = Resolve-ChildPath (Join-Path $artRoot 'Generated') ($sheet.name + '.png')
    if (-not (Test-Path -LiteralPath $atlasPath -PathType Leaf)) { throw "Missing generated sheet: $atlasPath" }
    foreach ($entry in $sheet.files) {
        $source = Resolve-ChildPath $originalRoot $entry.path
        $destination = Resolve-ChildPath $assetRoot $entry.path
        if (-not $destinations.Add($destination)) { throw "Duplicate destination: $($entry.path)" }
        if ($entry.width -lt 1 -or $entry.height -lt 1) { throw "Invalid dimensions: $($entry.path)" }
        if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $entry.sha256) {
            throw "Original backup hash differs from the manifest: $($entry.path)"
        }
        $originalMeta = [IO.File]::ReadAllText($source + '.meta')
        $currentMeta = [IO.File]::ReadAllText($destination + '.meta')
        if ((Get-MetaGuid $originalMeta) -ne $entry.guid -or (Get-MetaGuid $currentMeta) -ne $entry.guid) {
            throw "Original or destination GUID differs from the manifest: $($entry.path)"
        }
        if ($originalMeta -match '(?m)^\s+(outline|physicsShape|vertices):\s*\r?\n\s+-') {
            throw "Custom sprite geometry requires explicit coordinate migration: $($entry.path)"
        }
        $pending.Add([pscustomobject]@{ Sheet = $sheet; Entry = $entry; Source = $source; Destination = $destination; Meta = $originalMeta })
    }
}

# A unique review directory avoids treating a partial previous run as a completed import.
$staging = Resolve-ChildPath $reviewRoot ('imports/' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $staging -Force | Out-Null
$results = [Collections.Generic.List[object]]::new()
foreach ($sheet in $selected) {
    # Large backgrounds need more display resolution than individually cropped foreground cells.
    $importScale = if (-not $sheet.transparent -or $sheet.name -in @('cave-layers', 'terrain-props')) { 4 } else { 2 }
    $atlasPath = Resolve-ChildPath (Join-Path $artRoot 'Generated') ($sheet.name + '.png')
    $atlas = [Drawing.Bitmap]::new($atlasPath)
    try {
        if ($atlas.Width -lt $sheet.columns -or $atlas.Height -lt $sheet.rows) { throw "Atlas is smaller than its grid: $atlasPath" }
        $entries = @($pending | Where-Object { $_.Sheet.name -eq $sheet.name })
        for ($index = 0; $index -lt $entries.Count; $index++) {
            $job = $entries[$index]
            $entry = $job.Entry
            $column = $index % $sheet.columns
            $row = [int][Math]::Floor($index / [double]$sheet.columns)
            $left = [int][Math]::Floor($column * $atlas.Width / [double]$sheet.columns)
            $top = [int][Math]::Floor($row * $atlas.Height / [double]$sheet.rows)
            $right = [int][Math]::Floor(($column + 1) * $atlas.Width / [double]$sheet.columns)
            $bottom = [int][Math]::Floor(($row + 1) * $atlas.Height / [double]$sheet.rows)
            $cellRect = [Drawing.Rectangle]::new($left, $top, $right - $left, $bottom - $top)
            $cell = $atlas.Clone($cellRect, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
            $original = [Drawing.Bitmap]::new($job.Source)
            $output = [Drawing.Bitmap]::new([int]($entry.width * $importScale), [int]($entry.height * $importScale), [Drawing.Imaging.PixelFormat]::Format32bppArgb)
            $graphics = [Drawing.Graphics]::FromImage($output)
            try {
                if ($original.Width -ne $entry.width -or $original.Height -ne $entry.height) {
                    throw "Original dimensions differ from the manifest: $($entry.path)"
                }
                $sourceBounds = Get-AlphaBounds $cell
                $originalBounds = Get-AlphaBounds $original
                if ($sourceBounds.IsEmpty -or $sourceBounds.Width -eq 0 -or $originalBounds.Width -eq 0) {
                    throw "Generated cell or original is empty: $($sheet.name) [$index] $($entry.path)"
                }
                $mode = if ($sheet.transparent -and $sheet.name -notin $CanvasGroup) { 'alpha-bounds-fit' } else { 'full-canvas' }
                $graphics.Clear([Drawing.Color]::Transparent)
                $graphics.CompositingMode = [Drawing.Drawing2D.CompositingMode]::SourceCopy
                $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
                $graphics.PixelOffsetMode = [Drawing.Drawing2D.PixelOffsetMode]::HighQuality
                if ($mode -eq 'alpha-bounds-fit') {
                    $targetWidth = $originalBounds.Width * $importScale
                    $targetHeight = $originalBounds.Height * $importScale
                    $fitScale = [Math]::Min($targetWidth / [double]$sourceBounds.Width, $targetHeight / [double]$sourceBounds.Height)
                    $drawWidth = [single]($sourceBounds.Width * $fitScale)
                    $drawHeight = [single]($sourceBounds.Height * $fitScale)
                    $destinationRect = [Drawing.RectangleF]::new(
                        [single]($originalBounds.X * $importScale + ($targetWidth - $drawWidth) / 2),
                        [single]($originalBounds.Y * $importScale + ($targetHeight - $drawHeight) / 2), $drawWidth, $drawHeight)
                    $graphics.DrawImage($cell, $destinationRect, [Drawing.RectangleF]$sourceBounds, [Drawing.GraphicsUnit]::Pixel)
                }
                else {
                    $graphics.DrawImage($cell, [Drawing.Rectangle]::new(0, 0, $output.Width, $output.Height),
                        0, 0, $cell.Width, $cell.Height, [Drawing.GraphicsUnit]::Pixel)
                }
                $graphics.Flush()
                $outputBounds = Get-AlphaBounds $output
                if ($outputBounds.Width -eq 0) { throw "Imported image has no visible alpha: $($entry.path)" }
                $stagedPath = Resolve-ChildPath $staging $entry.path
                New-Item -ItemType Directory -Path (Split-Path $stagedPath) -Force | Out-Null
                $output.Save($stagedPath, [Drawing.Imaging.ImageFormat]::Png)
                $meta = New-ScaledMeta $job.Meta $importScale
                if ((Get-MetaGuid $meta) -ne $entry.guid) { throw "Importer changed GUID: $($entry.path)" }
                [IO.File]::WriteAllText($stagedPath + '.meta', $meta, $utf8)
                $results.Add([pscustomobject][ordered]@{
                    group = $sheet.name; path = $entry.path; mode = $mode; guid = $entry.guid
                    width = $output.Width; height = $output.Height; scale = $importScale
                    alphaBounds = @($outputBounds.X, $outputBounds.Y, $outputBounds.Width, $outputBounds.Height)
                    sha256 = (Get-FileHash -LiteralPath $stagedPath -Algorithm SHA256).Hash
                    metaSha256 = (Get-FileHash -LiteralPath ($stagedPath + '.meta') -Algorithm SHA256).Hash
                    stagedPath = $stagedPath; destination = $job.Destination
                })
            }
            finally { $graphics.Dispose(); $output.Dispose(); $original.Dispose(); $cell.Dispose() }
        }
    }
    finally { $atlas.Dispose() }
}

# Staging is complete and checked. Re-running always derives scale and metadata from originals.
foreach ($result in $results) {
    Copy-Item -LiteralPath $result.stagedPath -Destination $result.destination -Force
    Copy-Item -LiteralPath ($result.stagedPath + '.meta') -Destination ($result.destination + '.meta') -Force
    if ((Get-FileHash -LiteralPath $result.destination -Algorithm SHA256).Hash -ne $result.sha256 -or
        (Get-MetaGuid ([IO.File]::ReadAllText($result.destination + '.meta'))) -ne $result.guid) {
        throw "Final asset verification failed: $($result.path)"
    }
}

# Merge successful selective imports so the report remains useful after several -Group runs.
$merged = [Collections.Generic.List[object]]::new()
if (Test-Path -LiteralPath $reportPath -PathType Leaf) {
    $previous = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
    foreach ($result in $previous.files) {
        if ($result.group -notin @($selected | ForEach-Object { $_.name })) { $merged.Add($result) }
    }
}
foreach ($result in $results) { $merged.Add($result) }
$report = [ordered]@{
    schemaVersion = 1; updatedUtc = [DateTime]::UtcNow.ToString('o')
    appliedGroups = @($selected | ForEach-Object { $_.name }); files = @($merged.ToArray())
}
[IO.File]::WriteAllText($reportPath, ($report | ConvertTo-Json -Depth 8), $utf8)
Write-Output "Imported $($results.Count) sprites from $($selected.Count) generated sheets. Report: $reportPath"
