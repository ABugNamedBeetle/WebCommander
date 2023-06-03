Param(
    [Parameter(Mandatory=$true, Position=0)]
    [String]$Folder,
    [Parameter(Mandatory=$true)]
    [String]$OutputName
)

# Check if the application is running

$process = Get-Process -Name $OutputName.Replace('.exe','') -ErrorAction SilentlyContinue

if ($process) {
    # Application is running, kill the process
    $process | ForEach-Object {
        Write-Host "Killing process with ID: $($_.Id) and Name: $($_.Name)"
        $_.Kill()
    }
} else {
    Write-Host "Application '$OutputName' is not running."
}


$files = Get-ChildItem -Path $Folder -Filter "*.cs" -Recurse | Select-Object -ExpandProperty FullName

[System.Collections.ArrayList] $fset = @();
foreach ($file in $files) {
    $relativePath = $file -replace [regex]::Escape($PWD.Path), ""
    $relativePath = $relativePath.TrimStart("\")
    $relativePath = $relativePath -replace '\\', '/'
    $relativePath = '"' + $relativePath + '"'
    $fset.Add($relativePath) > $null;
}


bflat.exe build "Program.cs" $fset --out bin/bflat/$OutputName --verbose;