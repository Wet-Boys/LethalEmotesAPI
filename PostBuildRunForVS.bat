tasklist /fi "imagename eq devenv.exe" | findstr /B /I /C:"devenv.exe " >NUL
IF ERRORLEVEL 0 Powershell.exe -ExecutionPolicy Bypass -File "%~dp0build.ps1" --target=Deploy