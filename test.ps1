$dir = (Get-Item $pwd).FullName + "\src"
$moduleExclude = '^(?!Rustic\..*).* | .*Tests$'
dotnet test --no-restore --no-build --test-adapter-path:$dir --logger:Appveyor /p:AltCover='true' /p:AltCoverAssemblyExcludeFilter=$moduleExclude

$commitID = & git rev-parse HEAD
$commitBranch = & git rev-parse --abbrev-ref HEAD

foreach ($coverage in Get-ChildItem -Recurse -File coverage*.xml) {
    # postprocess the coverage files
    $coverage_xml = [xml](Get-Content $coverage.FullName)
    $coverage_xml.CoverageSession.Modules.Module | % {
        if ($_.ModuleName -match $moduleExclude) {
            # remove excluded modules
            $_.ParentNode.RemoveChild($_)
        }
        if ($_.skippedDueTo -eq 'Filter') {
            # remove filtered modules
            $_.ParentNode.RemoveChild($_)
        }

        $_.Files.File | % {
            if ($_.fullPath -match '^[\\/]_[\\/]') {
                # trim /_/ prefix for project directory
                $_.fullPath = $_.fullPath -replace '^[\\/]_[\\/]', ''
            }
            else {
                # remove files outside of the project directory
                $_.ParentNode.RemoveChild($_)
            }
        }
        # make absolute paths relative
        $_.ModulePath = $_.ModulePath -replace [regex]::escape($dir), ''
    }
    # wirte the changes back to the file
    $coverage_xml.Save($coverage.FullName)

    & csmacnz.coveralls --opencover -i $coverage --commitId $commitID --commitBranch $commitBranch
}
