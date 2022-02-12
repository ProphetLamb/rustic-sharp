dotnet test --no-restore --no-build -verbosity:m  --test-adapter-path:. --logger:Appveyor /p:AltCover="true" /p:AltCoverAssemblyExcludeFilter="NUnit|AltCover" /p:AltCoverAssemblyFilter="Rustic\."

$commitID = & git rev-parse HEAD
$commitBranch = & git rev-parse --abbrev-ref HEAD

foreach ($coverage in Get-ChildItem -Recurse -File coverage*.xml)
{
    $relative=Resolve-Path -Relative $coverage.FullName
    & csmacnz.coveralls --opencover -i $relative --useRelativePaths --basePath $pwd --commitId $commitID --commitBranch $commitBranch
}
