mkdir "doc/"

foreach ($project in Get-ChildItem -Path src/ -Recurse -File *.csproj) {
    $dir=$project.DirectoryName
    $name=[System.IO.Path]::GetFileNameWithoutExtension($project.Name)
    xmldoc2md "$dir/bin/Debug/net5.0/$name.dll" "doc/$name"
}
