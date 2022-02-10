$Env:LABEL = if ($Env:APPVEYOR) { "CI" + $Env:APPVEYOR_BUILD_NUMBER.PadLeft(5, "0") } else { "local" }
$Env:DOTNET_CLI_TELEMETRY_OPTOUT=1
$Env:DOTNET_NOLOGO=1

$target_frameworks = [String[]] 'netstandard2.1', 'netcoreapp3.1', 'net5.0', 'net6.0'

dotnet restore -verbosity:q

echo "`n========================================================================================================================`n
Build and pack libs
`n========================================================================================================================`n"
Push-Location .\src

foreach ($project in Get-ChildItem -Recurse Rustic*.csproj -File) {
    Push-Location $project.Directory

    echo "`n========================================================================================================================`nBuild $project"
    dotnet build -c:Release --no-restore -verbosity:q --version-suffix $Env:LABEL
    dotnet pack -c:Release --no-restore --no-build -verbosity:q --version-suffix $Env:LABEL

    Pop-Location
}
Pop-Location

echo "`n========================================================================================================================`n
Run and cover tests
`n========================================================================================================================`n"
Push-Location .\tests

foreach ($project in Get-ChildItem -Recurse Rustic*Tests.csproj -File) {
    Push-Location $project.Directory

    echo "`n========================================================================================================================`nTest $project"
    foreach ($target in $target_frameworks) {
        dotnet test --framework:$target -verbosity:q /p:AltCover=true
    }

    Pop-Location
}
Pop-Location
