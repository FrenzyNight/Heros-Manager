param(
    [ValidateSet('Project', 'Rules', 'Save', 'Isolation', 'IsolationPlay', 'Play', 'Meadow', 'Guide', 'Lighting', 'Build', 'Upgrade')]
    [string]$Check = 'Project',
    [string]$UnityPath = 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe'
)

$ErrorActionPreference = 'Stop'
$projectPath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../HerosManager'))
if (-not (Test-Path -LiteralPath $UnityPath -PathType Leaf)) {
    throw "Unity 6000.4.7f1 was not found: $UnityPath"
}
$openEditor = Get-CimInstance Win32_Process -Filter "name = 'Unity.exe'" | Where-Object {
    $_.CommandLine -match '-projectPath\s+(?:"([^"]+)"|(\S+))' -and
    [IO.Path]::GetFullPath(($Matches[1], $Matches[2] | Where-Object { $_ } | Select-Object -First 1)).TrimEnd('\', '/') -ieq $projectPath.TrimEnd('\', '/')
}
if ($openEditor) { throw "Close the Unity editor for this project before validation (PID $($openEditor.ProcessId))." }
$methods = @{
    Project = 'MigrationValidation.Validate'
    Rules = 'GameplayRulesValidation.Run'
    Save = 'SaveStorageValidation.Run'
    Isolation = 'SaveIsolationValidation.Run'
    IsolationPlay = 'SaveIsolationPlayValidation.Run'
    Play = 'MigrationPlaySmoke.Run'
    Meadow = 'MeadowPlayValidation.Run'
    Guide = 'GuidePlayValidation.Run'
    Lighting = 'LightingPlayValidation.Run'
    Build = 'MigrationValidation.BuildWindows'
    Upgrade = 'MigrationValidation.UpgradeAssets'
}
$logDirectory = Join-Path $projectPath 'Logs'
New-Item -ItemType Directory -Force -Path $logDirectory | Out-Null
$logPath = Join-Path $logDirectory "validation-$($Check.ToLowerInvariant()).log"
$unityArguments = @('-batchmode', '-projectPath', "`"$projectPath`"", '-executeMethod', $methods[$Check], '-logFile', "`"$logPath`"")
if ($Check -notin @('Play', 'Meadow', 'Guide', 'Lighting', 'IsolationPlay')) { $unityArguments += '-quit' }
if ($Check -in @('Project', 'Rules', 'Save', 'Isolation')) { $unityArguments += '-nographics' }
$process = Start-Process -FilePath $UnityPath -ArgumentList $unityArguments -WindowStyle Hidden -PassThru
if (-not $process.WaitForExit(1200000)) {
    $process.Kill()
    throw "$Check exceeded 20 minutes; the owned Unity process was stopped. See $logPath"
}
Write-Output "Unity exit code: $($process.ExitCode). Log: $logPath"
if ($process.ExitCode -ne 0) { throw "$Check validation failed. See $logPath" }
