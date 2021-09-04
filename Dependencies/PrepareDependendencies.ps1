# 2009463077 brrainz.harmony                      Harmony

$appId = 294100
$modIds = @(2009463077)
$gameVersions = @("1.1", "1.2", "1.3")

#yes, the only purpose is to download Harmony
#yes, it is possible to install nuget package instead

[string]$startupPath = Get-Location
$7z = (GET-ItemProperty 'HKLM:\SOFTWARE\7-Zip').Path + '7z.exe'

$modsLocal = $rw + '\Mods\' + $internalPath
$cache = $startupPath + "\cache"


$steamcmd_location = $cache + "\steamcmd"
$steamcmd = $steamcmd_location +"\steamcmd.exe"

New-Item -Force -Path $cache -ItemType Directory | Out-Null

if (-Not (Test-Path $steamcmd)) {
	# download SteamCMD
	Echo "Downloading steam console client..."
	
	$steamcmd_url = "http://media.steampowered.com/installer/steamcmd.zip"
	$steamcmd_archPath = $cache + "\steamcmd.zip"
	$progressPreference = 'Continue'
	Invoke-WebRequest -Uri $steamcmd_url -OutFile $steamcmd_archPath
	#& $7z e $steamcmd_archPath "-o$steamcmd_location" 
	Expand-Archive  $steamcmd_archPath -DestinationPath $steamcmd_location
	Remove-Item -Recurse -Force $steamcmd_archPath
}


Echo "Downloading mods..."

$steamCmdArgs = @("+login anonymous", "+force_install_dir $cache")
$steamCmdArgs += foreach ($modId in $modIds) {
	"+workshop_download_item $appId $modId"
}
$steamCmdArgs += "+quit"
& $steamcmd $steamCmdArgs 


$modsDownloads = $cache + "\steamapps\workshop\content\" + $appId
Echo $modsDownloads

$assemplyPathResolver = $startupPath + "\ResolveModLoadPaths.exe"

Echo "Coping assemblies..."


foreach ($gameVersion in $gameVersions) {
	Echo "$gameVersion..."
	
	$dependencies = $startupPath + "\" + $gameVersion

	
	if (Test-Path $dependencies) {
		Remove-Item -Recurse -Force $dependencies | Out-Null
	}
	New-Item -Force -Path $dependencies -ItemType Directory | Out-Null
	
	foreach ($modId in $modIds) {
		& $assemplyPathResolver --path "$modsDownloads\$modId" --version "$gameVersion" | Foreach-Object -Process {
			Copy-Item -Recurse $_ -Destination $dependencies
		}
	}
}


Echo "Done!"