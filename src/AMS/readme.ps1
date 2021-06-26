$path = "../../README.md"
$path =Resolve-Path $path 
Write-Host $path 

$fileContent = gc $path

Write-Host $fileContent.Length

For($i=0;$i -lt $fileContent.Length ; $i++){
    $line = $fileContent[$i];
    If ($line  -match '^<!--') {
        $state='comment'
        $fileContent[$i] =$null  # because `continue` doesn't work in a foreach-object
      }
      If ($line  -match '-->$') {
        $state='content'
        $fileContent[$i] =$null  
      }
      If ($state -eq 'comment') {
        $fileContent[$i] =$null  
      } 
}

$fileContent |Set-Content $path
