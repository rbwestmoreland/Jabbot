@echo Off
set config=%1
if "%config%" == "" (set config=debug)

md artifacts
md artifacts\console
md artifacts\web

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Jabbot.Console\Jabbot.Console.csproj /p:Configuration="%config%" /p:OutputPath=..\artifacts\console\ /flp:LogFile=artifacts\msbuild.console.log;Verbosity=Normal
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Jabbot.Web\Jabbot.Web.csproj /p:Configuration="%config%" /p:OutDir=..\artifacts\web\ /flp:LogFile=artifacts\msbuild.web.log;Verbosity=Normal
::pause