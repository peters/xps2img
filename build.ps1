param(
    [ValidateSet('Rebuild','Build', 'Clean')]
    [string]$build = "Rebuild",
    
    [ValidateSet('Debug', 'Release')]
    [string]$config = "Release",

    [bool] $BumpVersion = $true,
	
	[string] $OutputPath = "bin\$config\$TargetFrameworkVersion"
)

function Write-Diagnostic {
    param([string]$message)

    Write-Host
    Write-Host $message -ForegroundColor Green
    Write-Host
}

$rootFolder = split-path -parent $MyInvocation.MyCommand.Definition

$scriptsFolder = Join-Path $rootFolder "scripts"

$binaries = "$rootFolder\bin\"

if (Test-Path $binaries) { Remove-Item $binaries -Recurse -Force }

Write-Diagnostic  "Bootstrapping environment"

. $scriptsFolder\bootstrap.ps1

if ($BumpVersion) {
    Write-Diagnostic  "Increment version of libraries"
    . $scriptsFolder\Bump-Version.ps1 -Increment Patch
}

Write-Diagnostic "Building xps2im ($config / 4.0)"

. $scriptsFolder\Build-Solution.ps1 -Project "$rootFolder\src\xps2img\xps2img.csproj" `
                                    -Build $build -Config $config -TargetFrameworkVersion v4.0
									
Write-Diagnostic "Building xps2im ($config / 4.5)"

. $scriptsFolder\Build-Solution.ps1 -Project "$rootFolder\src\xps2img\xps2img.csproj" `
                                    -Build $build -Config $config -TargetFrameworkVersion v4.5
									
Write-Diagnostic "Building nuget package"

. $rootFolder\.nuget\NuGet.exe pack $rootFolder\src\xps2img\xps2img.csproj -Tool -OutputDirectory $rootFolder\build\ -Verbosity quiet -NoPackageAnalysis
