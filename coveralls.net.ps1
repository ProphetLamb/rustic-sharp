# We need to run the tests again, bc Appveyor doesnt use dotnet cli for testing, and I cant get the AppveyorTestlogger to work.
dotnet test /p:AltCover="true"

$commitID = & git rev-parse HEAD
$commitBranch = & git rev-parse --abbrev-ref HEAD

foreach ($coverage in Get-ChildItem -Recurse -File coverage*.xml)
{
    csmacnz.coveralls.exe --opencover -i $coverage.FullName --useRelativePaths --basePath $pwd --commitId $commitID --commitBranch $commitBranch
}
