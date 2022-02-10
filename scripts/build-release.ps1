echo "`n========================================================================================================================`n
Build and pack libs
`n========================================================================================================================`n"

Push-Location .\src

foreach ($project in Get-ChildItem -Recurse Rustic*.csproj -File) {
    Push-Location $project.Directory

    echo "`n========================================================================================================================`nBuild $project`n"

    dotnet build -c:Release --no-restore -verbosity:q --version-suffix $Env:LABEL

    dotnet pack -c:Release --no-restore --no-build -verbosity:q --version-suffix $Env:LABEL

    Pop-Location
}
Pop-Location
