param(
    [string]
    $Project
    ,
    [ValidateSet('Rebuild', 'Build', 'Clean')]
    [string]
    $build = "Build"
    ,
    [ValidateSet('Debug', 'Release')]
    [string]
    $config = "Release"
    ,
    [string]
    $MSBuildVerbosity = "quiet"
	,
    [ValidateSet('v4.0', 'v4.5')]
    [string]
    $TargetFrameworkVersion = "v4.5"
	,
	[string]
	$OutputPath = "bin\$config\$TargetFrameworkVersion"
)

& "$(get-content env:windir)\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $Project /t:$build /p:Configuration=$config /p:TargetFrameworkVersion=$TargetFrameworkVersion /p:OutputPath=$OutputPath /verbosity:$MSBuildVerbosity