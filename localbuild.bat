@echo off

:: This batch file is used for troubleshooting stuff locally, without involving github or nuget.

if "%SUFFIX%"=="" (
  set SUFFIX=1
) else (
  set /A SUFFIX=SUFFIX+1
)
set RELEASE_VERSION=1.0.%SUFFIX%
call vsclean
del %USERPROFILE%\Personal\Dev\Dotnet\Main\LocalNugetSource\MikeNakis.Clio*.nupkg

@echo on

dotnet pack --configuration Debug
dotnet nuget push MikeNakis.Clio\bin\Debug\*.nupkg --source %USERPROFILE%\Personal\Dev\Dotnet\Main\LocalNugetSource

dotnet pack --configuration Release
dotnet nuget push MikeNakis.Clio\bin\Release\*.nupkg --source %USERPROFILE%\Personal\Dev\Dotnet\Main\LocalNugetSource
