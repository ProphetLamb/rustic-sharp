$Env:DOTNET_CLI_TELEMETRY_OPTOUT=1
$Env:DOTNET_NOLOGO=1

$target_frameworks = [String[]] 'netstandard2.1', 'netcoreapp3.1', 'net5.0', 'net6.0'

echo "`n========================================================================================================================`n
Run and cover tests
`n========================================================================================================================`n"
Push-Location .\tests

foreach ($project in Get-ChildItem -Recurse Rustic*Tests.csproj -File) {
    Push-Location $project.Directory

    echo "`n========================================================================================================================`nTest $project`n"
    foreach ($target in $target_frameworks) {
        dotnet test --framework:$target -verbosity:q /p:AltCover=true
    }

    Pop-Location
}
Pop-Location
