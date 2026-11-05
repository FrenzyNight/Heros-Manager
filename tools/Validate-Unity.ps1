param(
    [ValidateSet('Project', 'Save', 'Play', 'Build', 'Upgrade')]
    [string]$Check = 'Project',
    [string]$UnityPath = 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe'
)

$ErrorActionPreference = 'Stop'
$projectPath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../HerosManager'))
if (-not (Test-Path -LiteralPath $UnityPath -PathType Leaf)) {
    throw "Unity 6000.4.7f1 was not found: $UnityPath"
}
$methods = @{
    Project = 'MigrationValidation.Validate'
    Save = 'SaveStorageValidation.Run'
    Play = 'MigrationPlaySmoke.Run'
    Build = 'MigrationValidation.BuildWindows'
    Upgrade = 'MigrationValidation.UpgradeAssets'
}
$logDirectory = Join-Path $projectPath 'Logs'
New-Item -ItemType Directory -Force -Path $logDirectory | Out-Null
$logPath = Join-Path $logDirectory "validation-$($Check.ToLowerInvariant()).log"
$unityArguments = @('-batchmode', '-projectPath', "`"$projectPath`"", '-executeMethod', $methods[$Check], '-logFile', "`"$logPath`"")
if ($Check -ne 'Play') { $unityArguments += '-quit' }
if ($Check -in @('Project', 'Save')) { $unityArguments += '-nographics' }
$process = Start-Process -FilePath $UnityPath -ArgumentList $unityArguments -WindowStyle Hidden -PassThru -Wait
Write-Output "Unity exit code: $($process.ExitCode). Log: $logPath"
if ($process.ExitCode -ne 0) { throw "$Check validation failed. See $logPath" }
