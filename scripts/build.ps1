echo "`n========================================================================================================================`n
Build libs
`n========================================================================================================================`n"

Push-Location .\src

foreach ($project in Get-ChildItem -Recurse Rustic*.csproj -File) {
    Push-Location $project.Directory

    echo "`n========================================================================================================================`nBuild $project`n"

    dotnet build -c:Debug --no-restore -verbosity:q --version-suffix $Env:LABEL

    Pop-Location
}
Pop-Location
