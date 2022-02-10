$commitID = & git rev-parse HEAD
$commitBranch = & git rev-parse --abbrev-ref HEAD

foreach ($coverage in Get-ChildItem -Recurse -File coverage*.xml)
{
    csmacnz.coveralls.exe --opencover -i $coverage.FullName --useRelativePaths --basePath $pwd --commitId $commitID --commitBranch $commitBranch
}
