# We need to run the tests again, bc Appveyor doesnt use dotnet cli for testing, and I cant get the AppveyorTestlogger to work.
dotnet test --no-restore --no-build -verbosity:m /p:AltCover="true" /p:AltCoverAssemblyExcludeFilter="NUnit|AltCover"

$commitID = & git rev-parse HEAD
$commitBranch = & git rev-parse --abbrev-ref HEAD

foreach ($coverage in Get-ChildItem -Recurse -File coverage*.xml)
{
    $relative=Resolve-Path -Relative $coverage.FullName
    & csmacnz.coveralls --opencover -i $relative --useRelativePaths --basePath $pwd --commitId $commitID --commitBranch $commitBranch
}
