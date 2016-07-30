@ECHO OFF

:: Use Release build configuration unless specified
IF "%1"=="" (
CALL build.bat Release
SET build=%errorlevel%
EXIT /b %build%
)

IF EXIST .\build RMDIR .\build /S /Q

:: Download NuGet if its not already there
IF NOT EXIST .\packages\nuget\NuGet.exe (
IF NOT EXIST .\packages\nuget\ MKDIR .\packages\nuget
ECHO ^(New-Object System.Net.WebClient^).DownloadFile('https://dist.nuget.org/win-x86-commandline/v3.4.4/NuGet.exe', '.\\packages\\nuget\\NuGet.exe'^) > .\nuget.ps1
PowerShell.exe -ExecutionPolicy Bypass -File .\nuget.ps1
)

:: Clean up
IF EXIST .\nuget.ps1 DEL .\nuget.ps1
IF ERRORLEVEL 1 GOTO ERROR

:: Restore all packages
.\packages\nuget\nuget.exe restore
IF ERRORLEVEL 1 GOTO ERROR

:: Build Xml Transformer
"C:\Program Files (x86)\MSBuild\14.0\Bin\Msbuild.exe" /t:Rebuild .\Svenkle.XmlTransformer\Svenkle.XmlTransformer.csproj /p:Configuration=%1;OutputPath=.\..\build\
IF ERRORLEVEL 1 GOTO ERROR

EXIT /b 0

:ERROR
PAUSE
EXIT /b -1