@echo off

@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
 
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

set nuget=
if "%nuget%" == "" (
	set nuget=nuget
)

mkdir Build

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild src\calmo.framework.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" src\calmo.framework.sln /p:Configuration="Release" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false

mkdir Build
mkdir Build\lib
mkdir Build\lib\net45

%nuget% pack "src\Terrarium.Sdk.nuspec" -NoPackageAnalysis -verbosity detailed -o Build -Version %version% -p Configuration="%config%"