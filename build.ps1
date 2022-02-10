$Env:LABEL = "CI" + $Env:APPVEYOR_BUILD_NUMBER.PadLeft(5, "0")
$Env:DOTNET_CLI_TELEMETRY_OPTOUT=1
$Env:DOTNET_NOLOGO=1

Write-Output "`n========================================================================================================================`nBuild and pack libs`n========================================================================================================================`n"
Push-Location .\src

Get-ChildItem -Recurse Rustic*.csproj -File | ForEach-Object {
    Push-Location $_.Directory

    dotnet restore -verbosity:q
    dotnet build -c:Release --no-restore -verbosity:q --version-suffix $Env:LABEL
    dotnet pack -c:Release --no-restore -verbosity:q --version-suffix $Env:LABEL

    Pop-Location
}
Pop-Location

Write-Output "`n========================================================================================================================`nRun and cover tests`n========================================================================================================================`n"
Push-Location .\tests
Get-ChildItem -Recurse Rustic*Tests.csproj -File | ForEach-Object {
    Push-Location $_.Directory

    dotnet test -verbosity:q /p:AltCover=true

    Pop-Location
}
Pop-Location
