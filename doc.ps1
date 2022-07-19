mkdir -Force "doc/"

foreach ($project in Get-ChildItem -Path src/ -Recurse -File *.csproj) {
    $dir = $project.DirectoryName

    $lines = Get-Content $project

    # assembly name
    $name = [System.IO.Path]::GetFileNameWithoutExtension($project.Name)
    $lines | Select-String -Pattern '<Product\>(.*)</Product\>' | ForEach-Object {
        $name = $_.Matches.Groups[1].Value
    }

    # target framework
    $target = 'net60'
    $lines | Select-String -Pattern '<TargetFrameworks\>([^;]+).*</TargetFrameworks\>' | ForEach-Object {
        $target = $_.Matches.Groups[1].Value # use the first target framework
    }
    $lines | Select-String -Pattern '<TargetFramework\>(.*)</TargetFramework\>' | ForEach-Object {
        $target = "" # no sub directory created for single target framework
    }

    & xmldoc2md "$dir/bin/Debug/$target/$name.dll" "doc/$name"
}
