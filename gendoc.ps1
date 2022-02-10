mkdir -Force "doc/"

foreach ($project in Get-ChildItem -Path src/ -Recurse -File *.csproj) {
    $dir=$project.DirectoryName
    $name=[System.IO.Path]::GetFileNameWithoutExtension($project.Name)

    # default namespace
    if ((Get-Content $project) -match "<DefaultNamespace>(.*)<DefaultNamespace/>")
    {
        $name=$Matches[0]
    }

    & xmldoc2md "$dir/bin/Debug/net50/$name.dll" "doc/$name"
}
