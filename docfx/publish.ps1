$source = ".\\_site"
$destination = "..\\docs"

docfx build docfx.json

Get-ChildItem -Path $source -Recurse | Copy-Item -Destination { Join-Path $destination $_.FullName.Substring($source.length) } -Force