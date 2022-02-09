dotnet restore

$Env:LABEL = "CI" + $Env:APPVEYOR_BUILD_NUMBER.PadLeft(5, "0")
$Env:DOTNET_CLI_TELEMETRY_OPTOUT=1
$Env:DOTNET_NOLOGO=1

dotnet build -c:Release --no-restore --version-suffix $Env:LABEL
dotnet pack -c:Release --no-restore --version-suffix $Env:LABEL

dotnet build -c:Debug --no-restore --version-suffix $Env:LABEL

Get-ChildItem -Path .\tests -Recurse *Tests.csproj -File | ForEach-Object {
    dotnet test --no-restore --no-build --test-adapter-path:. --logger:Appveyor "${_.FullName}"
}
