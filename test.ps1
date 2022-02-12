dotnet test --no-restore --no-build --test-adapter-path:. --logger:Appveyor /p:AltCover="true"

$commitID = & git rev-parse HEAD
$commitBranch = & git rev-parse --abbrev-ref HEAD

foreach ($coverage in Get-ChildItem -Recurse -File coverage*.xml)
{
    & csmacnz.coveralls --opencover -i $coverage --commitId $commitID --commitBranch $commitBranch
}
