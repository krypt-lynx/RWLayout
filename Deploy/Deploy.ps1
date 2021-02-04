# Configurable variables


$config = ".\Deploy.xml"

[string]$packageId = (Select-Xml -Path $config -XPath '/config/about/packageId' | Select-Object -ExpandProperty Node).innerText
[string]$packageName = (Select-Xml -Path $config -XPath '/config/about/packageName' | Select-Object -ExpandProperty Node).innerText

[string]$solution = (Select-Xml -Path $config -XPath '/config/build/solution' | Select-Object -ExpandProperty Node).innerText
[string]$target = (Select-Xml -Path $config -XPath '/config/build/target' | Select-Object -ExpandProperty Node).innerText
[string]$platform = (Select-Xml -Path $config -XPath '/config/build/platform' | Select-Object -ExpandProperty Node).innerText


[string[]]$configurations = Select-Xml -Path $config -XPath '/config/build/configurations/configuration/text()' | Foreach-Object {
	($_ | Select-Object -ExpandProperty Node).Value
}

[string[]]$prebuildCleanup = Select-Xml -Path $config -XPath '/config/archive/prebuild/remove/path/text()' | Foreach-Object {
	($_ | Select-Object -ExpandProperty Node).Value
}

[string]$repo = (Select-Xml -Path $config -XPath '/config/archive/repository' | Select-Object -ExpandProperty Node).innerText
[string]$packing = (Select-Xml -Path $config -XPath '/config/archive/temp' | Select-Object -ExpandProperty Node).innerText
[string]$outputFormat = (Select-Xml -Path $config -XPath '/config/archive/outputTemplate' | Select-Object -ExpandProperty Node).innerText
[string]$internalPath = (Select-Xml -Path $config -XPath '/config/archive/modDirectory' | Select-Object -ExpandProperty Node).innerText

[string[]]$pathsToRemove = Select-Xml -Path $config -XPath '/config/archive/postbuild/exclude/path/text()' | Foreach-Object {
	($_ | Select-Object -ExpandProperty Node).Value
}


[Console]::ResetColor()

# Progress Bar Variables
$Activity             = "Deploying"
$Id                   = 1

# Complex Progress Bar
$Step                 = 0
$TotalSteps           = 9 
$StatusText           = '"Step $($Step.ToString().PadLeft($TotalSteps.ToString().Length)) of $TotalSteps | $Task"' # Single quotes need to be on the outside
$StatusBlock          = [ScriptBlock]::Create($StatusText) # This script block allows the string above to use the current values of embedded values each time it's run

# Read environvemt
$Task = "Collecting info..."
$Step++
##Write-Progress -Id $Id -Activity $Activity -Status (& $StatusBlock) -CurrentOperation " " -PercentComplete ($Step / $TotalSteps * 100)

$startupPath = Get-Location
$vswhere = "$Env:programfiles (x86)\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | select-object -first 1
$7z = (GET-ItemProperty 'HKLM:\SOFTWARE\7-Zip').Path + '7z.exe'
$rw = (GET-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 294100').InstallLocation

$packingMod = $packing + "\" + $internalPath


Push-Location -Path $repo

try {
	[string]$version = git describe --tags --always --dirty
} catch {
	[string]$version = ""
}

if ($version -eq "") {
	$manifest = "./About/Manifest.xml"
	[string]$version = (Select-Xml -Path $manifest -XPath '/Manifest/version' | Select-Object -ExpandProperty Node).innerText
}

$output = $outputFormat -f $version
$mod = $rw + '\Mods\' + $internalPath

Pop-Location

# Cleanup
$Task = "Cleanup..."
$Step++
Write-Progress -Id $Id -Activity $Activity -Status (& $StatusBlock) -CurrentOperation " " -PercentComplete ($Step / $TotalSteps * 100)

if (Test-Path $packing) { Remove-Item -Recurse -Force $packing }
if (Test-Path $output) { Remove-Item $output }
if (Test-Path $mod) { Remove-Item -Recurse -Force $mod }

Push-Location -Path $repo

foreach ($path in $prebuildCleanup) {
	if (Test-Path $path) { Remove-Item -Recurse -Force $path }
}

Pop-Location

$Task = "Building..."
$Step++
Write-Progress -Id $Id -Activity $Activity -Status (& $StatusBlock) -CurrentOperation " " -PercentComplete ($Step / $TotalSteps * 100)
foreach ($configuration in $configurations) {
	Echo "Building $configuration..."
	& $msbuild $solution /p:Configuration=$configuration /p:BuildProjectReferences=true
}

# Prepating data
$Task = "Copying..."
$Step++
Write-Progress -Id $Id -Activity $Activity -Status (& $StatusBlock) -CurrentOperation " " -PercentComplete ($Step / $TotalSteps * 100)

$items = Get-ChildItem -Force -Path $repo
$items | Foreach-Object -Begin { 
	$i = 0
} -Process {
    $i++
	
	if (-Not ($pathsToRemove -contains $_.Name)) {
		Copy-Item -Recurse $_.FullName -Destination ($packingMod + "\" + $_.Name)
	}
	
	$p = $i * 100 / $items.Count
	Write-Progress -Id ($Id+1) -ParentId $Id -Activity 'Copying' -Status ('{0:0}% complete' -f $p) -PercentComplete $p
}
Write-Progress -Id ($Id+1) -ParentId $Id -Activity "Copying" -Status "Ready" -Completed

$Task = "Patching..."
$Step++

$about = $packingMod + "/About/About.xml"
(Select-Xml -Path $about -XPath '/ModMetaData/packageId' | Select-Object -ExpandProperty Node).innerText = $packageId 

$xml = [xml](Get-Content $about)
$xml.SelectNodes('/ModMetaData/packageId') | % { 
    $_."#text" = $packageId
    }
$xml.SelectNodes('/ModMetaData/name') | % { 
    $_."#text" = $packageName
    }

$xml.Save($about)


# removing files from subfolders
$Task = "Excluding..."
$Step++
Write-Progress -Id $Id -Activity $Activity -Status (& $StatusBlock) -CurrentOperation " " -PercentComplete ($Step / $TotalSteps * 100)

foreach ($path in $pathsToRemove) {
	$p = $packingMod + '\' + $path
		if (Test-Path $p) { Remove-Item -Recurse -Force $p }
}

# archiving
$Task = "Archiving..."
$Step++
Write-Progress -Id $Id -Activity $Activity -Status (& $StatusBlock) -CurrentOperation " " -PercentComplete ($Step / $TotalSteps * 100)

Push-Location -Path  $packing
& $7z a -r -tzip ($startupPath.Path+'\'+$output) + $internalPath | Foreach-Object -Begin { 
	Write-Progress -Id ($Id+1) -ParentId $Id -Activity 'Archiving' -Status "Starting..." -PercentComplete 0
} -Process {
	$line = $_.Trim()
	if ($line -ne "") {
		[int]$p = 0
		[bool]$result = [int]::TryParse($line.Split('%')[0], [ref]$p)
		if ($result) {
			Write-Progress -Id ($Id+1) -ParentId $Id -Activity 'Archiving' -Status $line -PercentComplete $p
		}
	}
}

Write-Progress -Id ($Id+1)  -Activity "Archiving" -Status "Ready" -Completed
Pop-Location


# cleanup
$Task = "Cleanup..."
$Step++
Write-Progress -Id $Id -Activity $Activity -Status (& $StatusBlock) -CurrentOperation " " -PercentComplete ($Step / $TotalSteps * 100)

if (Test-Path $mod) { Remove-Item -Recurse -Force $mod }
Move-Item -Path $packingMod -Destination $mod
if (Test-Path $packing) { Remove-Item -Recurse -Force $packing }

Write-Progress -Id $Id -Activity $Activity -Status (& $StatusBlock) -Completed

# If running in the console, wait for input before closing.
if ($Host.Name -eq "ConsoleHost")
{
	Write-Host "Done"
#    Write-Host "Press any key to continue..."
#    $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp") > $null
}