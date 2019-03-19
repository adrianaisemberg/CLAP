@echo off
set PACKPATH=%~dp0_nuget_pack
if exist %PACKPATH% rd /q /s %PACKPATH%

set TEMP_LIB=%PACKPATH%\_lib\net20
set LIB=%PACKPATH%\lib\net20
md %TEMP_LIB%
md %LIB%
copy %~dp0..\bin\Release20\CLAP.??? %TEMP_LIB%
copy %~dp0..\bin\Release20\Newtonsoft.Json.??? %TEMP_LIB%
ilmerge /keyfile:..\CLAP.snk /out:%LIB%\CLAP.dll /internalize %TEMP_LIB%\CLAP.Dll %TEMP_LIB%\Newtonsoft.Json.dll
copy %TEMP_LIB%\CLAP.xml %LIB%

set TEMP_LIB=%PACKPATH%\_lib\net35
set LIB=%PACKPATH%\lib\net35
md %TEMP_LIB%
md %LIB%
copy %~dp0..\bin\Release\CLAP.??? %TEMP_LIB%
copy %~dp0..\bin\Release\Newtonsoft.Json.??? %TEMP_LIB%
ilmerge /keyfile:..\CLAP.snk /out:%LIB%\CLAP.dll /internalize %TEMP_LIB%\CLAP.Dll %TEMP_LIB%\Newtonsoft.Json.dll
copy %TEMP_LIB%\CLAP.xml %LIB%

rd /q /s %PACKPATH%\_lib

copy %~dp0CLAP.nuspec %PACKPATH%

getver %PACKPATH%\lib\net20\CLAP.dll > _ver 
set /p VERSION= < _ver
del _ver

fart --quiet %PACKPATH%\CLAP.nuspec ${VERSION} %VERSION%

nuget pack %PACKPATH%\CLAP.nuspec