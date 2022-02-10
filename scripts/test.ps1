
$target_frameworks = [String[]] 'net48', 'netcoreapp3.1', 'net5.0', 'net6.0'

echo "`n========================================================================================================================`n
Run and cover tests
`n========================================================================================================================`n"

Push-Location .\tests

foreach ($project in Get-ChildItem -Recurse Rustic*Tests.csproj -File) {
    Push-Location $project.Directory

    echo "`n========================================================================================================================`nTest $project`n"

    dotnet build --no-restore -verbosity:q

    foreach ($target in $target_frameworks) {
        dotnet test --no-build --no-restore --framework:$target -verbosity:q /p:AltCover=true
    }

    Pop-Location
}
Pop-Location
