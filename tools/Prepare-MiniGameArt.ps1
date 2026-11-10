param([string]$SourceRevision = 'b55f8231c2effd41a50eb70ce289f74c9488fffe')
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$root = Join-Path $repo 'HerosManager/Assets/04.Images/MiniGame'
$work = Join-Path $repo 'HerosManager/Logs/art-review'
$output = Join-Path $repo 'Art/MinigameRefresh'
New-Item -ItemType Directory -Force "$work/originals", "$output/References", "$output/Generated" | Out-Null
$groups = [Collections.Generic.List[object]]::new()
function Export-Original($relative, $destination) {
    $info = [Diagnostics.ProcessStartInfo]::new('git')
    $info.WorkingDirectory = $repo
    $info.UseShellExecute = $false
    $info.CreateNoWindow = $true
    $info.RedirectStandardOutput = $true
    $info.RedirectStandardError = $true
    $info.ArgumentList.Add('show')
    $info.ArgumentList.Add("${SourceRevision}:HerosManager/Assets/04.Images/MiniGame/$relative")
    $process = [Diagnostics.Process]::Start($info)
    $stream = [IO.File]::Create($destination + '.pending')
    try { $process.StandardOutput.BaseStream.CopyTo($stream) }
    finally { $stream.Dispose() }
    $errorText = $process.StandardError.ReadToEnd()
    $process.WaitForExit()
    if ($process.ExitCode -ne 0) { throw "Cannot read original asset from Git: $relative. $errorText" }
    Move-Item -LiteralPath ($destination + '.pending') -Destination $destination -Force
    $process.Dispose()
}
function Add-Group($name, $columns, $transparent, $files) {
    $files = @($files)
    $rows = [int][Math]::Ceiling($files.Count / [double]$columns)
    $bitmap = [Drawing.Bitmap]::new(($columns * 256), ($rows * 256))
    $graphics = [Drawing.Graphics]::FromImage($bitmap)
    $graphics.Clear([Drawing.Color]::Transparent)
    $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
    $entries = @()
    for ($i = 0; $i -lt $files.Count; $i++) {
        $relative = $files[$i]
        $backup = Join-Path "$work/originals" $relative
        New-Item -ItemType Directory -Force (Split-Path $backup) | Out-Null
        if (-not (Test-Path -LiteralPath $backup)) { Export-Original $relative $backup }
        if (-not (Test-Path -LiteralPath "$backup.meta")) { Export-Original "$relative.meta" "$backup.meta" }
        $image = [Drawing.Image]::FromFile($backup)
        # Every cell retains the original full canvas; bounds registration happens on import.
        $graphics.DrawImage($image, [Drawing.Rectangle]::new((($i % $columns)*256), ([int][Math]::Floor($i/$columns)*256), 256, 256))
        $meta = Get-Content -LiteralPath "$backup.meta" -Raw
        $entries += [ordered]@{ path=$relative; width=$image.Width; height=$image.Height; guid=([regex]::Match($meta,'(?m)^guid: (.+)').Groups[1].Value.Trim()); sha256=(Get-FileHash -LiteralPath $backup).Hash }
        $image.Dispose()
    }
    $bitmap.Save("$output/References/$name.png")
    $graphics.Dispose(); $bitmap.Dispose()
    $groups.Add([ordered]@{ name=$name; columns=$columns; rows=$rows; transparent=$transparent; files=$entries })
}
function Frames($folder, $pattern) { Get-ChildItem -LiteralPath (Join-Path $root $folder) -Filter $pattern | Sort-Object { [regex]::Replace($_.Name,'\d+', { param($m) $m.Value.PadLeft(4,'0') }) } | ForEach-Object { "$folder/$($_.Name)" } }
Add-Group 'forest-grounds' 2 $false @('1-1 Wood/1-1Background.png','1-3 herbs/1-3 grasslandmaybe.png','1-4 Hunt/1-4 hunt.png','1-5 riverside/riverside.png')
Add-Group 'desert-grounds' 2 $false @('2-1 tree/treedesertbackground.png','2-2 maze/desert maze.png','2-3 deserthunt/hunt.png','2-4 desertherb/hunt.png')
Add-Group 'water-grounds' 2 $false @('2-5 fish/desertfish_background 1.png','2-5 fish/desertfish_background.png','2-6 well section/wellsectionfinal.png','2-7 oasis/desertplanetbackground.png')
Add-Group 'river-frames' 2 $false (Frames '1-6 river' 'river(*).png')
Add-Group 'cave-layers' 2 $true @('1-2 Cave/cavebackgroundlayer_1.png','1-2 Cave/cavebackgroundlayer_2.png','1-2 Cave/cavebackgroundlayer_3.png','1-2 Cave/ceiling.png')
Add-Group 'terrain-props' 3 $true @('1-1 Wood/1-1Backgroundclouds.png','2-5 fish/desertfish_clouds.png','2-5 fish/desertfish_clouds 1.png','1-2 Cave/cavebottomgroundtile.png','2-2 maze/mazetilefin.png','2-7 oasis/desert planet.png','1-2 Cave/cave plank long.png','1-5 riverside/log1tile.png','2-6 well section/ropelong.png')
Add-Group 'forest-animals' 4 $true @((Frames '1-4 Hunt' 'bearre*.png'); (Frames '1-4 Hunt' 'foxre*.png'); (Frames '1-4 Hunt' 'idlefoxre*.png'))
Add-Group 'desert-animals' 5 $true @((Frames '2-3 deserthunt' 'desertfox_idle*.png'); (Frames '2-3 deserthunt' 'desertfox_move*.png'); (Frames '2-3 deserthunt' 'desertfoxgold_idle*.png'); (Frames '2-3 deserthunt' 'desertfoxgold_move*.png'); (Frames '2-3 deserthunt' 'weasle_idle*.png'); (Frames '2-3 deserthunt' 'weasle_move*.png'))
Add-Group 'forest-flowers' 4 $true @((Frames '1-3 herbs' 'flowerre*.png'); (Frames '1-3 herbs' 'goldflower*.png'))
Add-Group 'desert-flowers' 4 $true @((Frames '2-4 desertherb' 'desertflower(*).png'); (Frames '2-4 desertherb' 'desertflower_gold*.png'); '2-4 desertherb/cactustopview.png'; '2-3 deserthunt/cactus.png')
Add-Group 'river-fish' 4 $true @((Frames '1-5 riverside' 'fishswimmingre*.png'); (Frames '1-5 riverside' 'fishswimming_gold*.png'))
Add-Group 'lake-fish' 5 $true @((Frames '2-5 fish' 'fishsideswimming(*).png'); (Frames '2-5 fish' 'fishsideswimming_gold*.png'))
Add-Group 'hero-side' 4 $true @((Frames 'Char/Run' '*.png'); (Frames 'Char/Walk' '*.png'))
Add-Group 'hero-top' 5 $true @((Frames 'Char/Bow' '*.png'); (Frames 'Char/Top' '*.png'); (Frames 'Char/Tree' '*.png'))
Add-Group 'hero-actions' 4 $true @((Frames 'Char/FishFront' '*.png'); (Frames 'Char/FishSide' '*.png'); (Frames '1-2 Cave' 'fall*.png'); (Frames '1-2 Cave' 'slide*.png'); '1-1 Wood/2.png'; '1-1 Wood/3.png'; '1-1 Wood/대지 1.png'; '1-6 river/pail.png')
Add-Group 'gems' 5 $true (Frames '1-2 Cave' 'RubyAnimation*.png')
Add-Group 'wood-stone-props' 4 $true @('1-1 Wood/1-1treechoped.png','1-1 Wood/1-1treechoped_gold.png','2-1 tree/normallog.png','2-1 tree/goldlog.png','2-1 tree/holelog_2.png','1-2 Cave/caverock.png','1-2 Cave/cavestalactite.png','1-2 Cave/cavestalactite_2.png','2-6 well section/stone.png','2-2 maze/map.png','2-7 oasis/cartoonmeat.png','2-7 oasis/treelogs.png','2-7 oasis/waterjug.png','2-7 oasis/fishingnet.png','1-4 Hunt/arrow.png','2-5 fish/fishhook.png')
Add-Group 'water-props' 4 $true @((Frames '1-6 river' 'bucket*.png'); (Frames '2-6 well section' 'bucket*.png'); '1-6 river/boots.png'; '1-6 river/deadfish.png'; '1-6 river/eaten apple.png')
$groups | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath "$output/manifest.json" -Encoding utf8
Write-Output "$($groups.Count) sheets, $(($groups.files | Measure-Object).Count) source images prepared."
