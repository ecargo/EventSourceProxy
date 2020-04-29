param(
    [parameter(mandatory, helpMessage = "NuGet API key")][string]$NuGetAPIKey,
    [parameter(mandatory, helpMessage = "Version number matching existing GitHub release")][string]$Version
)

$ErrorActionPreference = 'Stop'

Push-Location $env:TEMP
try {
    mkdir "NuGet - $([guid]::NewGuid())" | Set-Location

    Invoke-WebRequest "https://github.com/ecargo/EventSourceProxy/archive/$Version.zip" -OutFile source.zip
    Expand-Archive source.zip
    cd source\EventSourceProxy*\EventSourceProxy

    dotnet pack --configuration Release --output NuGetPackage -p:PackageVersion=$Version
    if($LastExitCode -ne 0) {
        Write-Error "Failed to create NuGet package."
    }

    $nuGetPackageFile = Get-ChildItem .\NuGetPackage\ -filter ECargo.EventSourceProxy.NetStandard.*.nupkg
    if(!$nuGetPackageFile){
        Write-Error "Could not find NuGet package to publish"
    }

    $nuGetPackageFullName = $nuGetPackageFile.FullName
    Write-Host "Publishing $nuGetPackageFullName to NuGet..."
    dotnet nuget push $nuGetPackageFullName -k $NuGetAPIKey -s https://api.nuget.org/v3/index.json
    if($lastExitCode -ne 0) {
        Write-Error "Publish to NuGet failed."
    }
}
finally {
    Pop-Location
}
