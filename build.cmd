@echo Off
set config=%1
if "%config%" == "" (
   set config=debug
)

md artifacts
md artifacts\bin
md artifacts\packages
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Jabbot.Console\Jabbot.Console.csproj /p:Configuration="%config%";OutputPath=..\artifacts\bin\;BuildPackage=false;PackageOutputDir=..\artifacts\packages /m /v:M /fl /flp:LogFile=artifacts\msbuild.log;Verbosity=Normal /nr:false
::pause