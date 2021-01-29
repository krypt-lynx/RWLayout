$ProjectDir = $Env:ProjectDir
$TargetDir = $Env:TargetDir

Echo "Project directory: $ProjectDir"
Echo "Target directory: $TargetDir"

if (($ProjectDir -eq $null) -or
	($TargetDir -eq $null))
{
	Echo "Paths not set; exiting"
	exit
}

$version = git describe --tag  --dirty --always 
$version | Out-File -NoNewline "$ProjectDir\git.txt"