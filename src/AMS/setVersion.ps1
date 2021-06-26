
function WriteVersion($filename, $version){

Write-Host "Starting versioning of " $fileName " with version " $version 
$xml=[xml](get-content $fileName)
if ($xml.Project.PropertyGroup.Version -eq $null) 
{ 
        $xml.Project.PropertyGroup.AppendChild($xml.ImportNode(([xml]"<Version/>").DocumentElement,$true)) 
}

$xml.SelectNodes("/Project/PropertyGroup/Version")[0].InnerText = $version 

if($xml.Project.PropertyGroup.PackageVersion -ne $null){
        Write-Host " there is a PackageVersion "
        $xml.SelectNodes("/Project/PropertyGroup/PackageVersion")[0].InnerText = $version 
}

$utf8Bom = New-Object System.Text.UTF8Encoding($true)
$sw = New-Object System.IO.StreamWriter($fileName, $false, $utf8Bom )

$xml.Save( $sw )
$sw.Close()



#reading again
$xml=[xml](get-content $fileName)
Write-Host "Version of " $fileName " is " $xml.Project.PropertyGroup.Version
Write-Host "Package Version of " $fileName " is " $xml.Project.PropertyGroup.PackageVersion

}

$version = Get-Date -Format "yyyy.M.d.Hmm"
Get-ChildItem -Path .\ -Filter *.csproj -Recurse -File -Name| ForEach-Object {

        WriteVersion $_  $version

}
