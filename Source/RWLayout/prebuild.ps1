param([string]$ProjectDir, [string]$TargetDir);
Echo "parameters:"
Echo "ProjectDir: $ProjectDir"
Echo "TargetDir: $TargetDir"

$version = git describe --tag  --dirty --always 
$version | Out-File -NoNewline "$ProjectDir\git.txt"