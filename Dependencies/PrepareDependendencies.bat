@ECHO OFF
SET ThisScriptsDirectory=%~dp0
SET PSSctiptName=PrepareDependendencies.ps1
SET PowerShellScriptPath=%ThisScriptsDirectory%%PSSctiptName%
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& \"%PowerShellScriptPath%\"";
pause