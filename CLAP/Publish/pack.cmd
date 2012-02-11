@echo off
set PACKPATH=%~dp0_nuget_pack
rd /q /s %PACKPATH%
md %PACKPATH%\lib\net20
copy %~dp0merged_Release\CLAP.??? %PACKPATH%\lib\net20
copy %~dp0CLAP.nuspec %PACKPATH%

getver %PACKPATH%\lib\net20\CLAP.dll > _ver 
set /p VERSION= < _ver
del _ver

fart --quiet %PACKPATH%\CLAP.nuspec ${VERSION} %VERSION%

nuget pack %PACKPATH%\CLAP.nuspec