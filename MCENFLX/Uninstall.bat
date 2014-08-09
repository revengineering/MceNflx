@echo off
pushd "%CD%"
CD /D "%~dp0"
echo.
echo.Media Center Netflix Search Fix Service... > Uninstall.log
echo. >> Uninstall.log

echo Media Center Netflix Search Fix Uninstaller
echo.
echo   - Stopping service...
net stop MceNflx 1>> Uninstall.log 2>&1
echo.
if ERRORLEVEL 0 goto UninstallService
goto Fail
:UninstallService
    echo   - Uninstalling Service...
    %WINDIR%\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe /u MCENFLX.exe 1>> Uninstall.log 2>&1
    if ERRORLEVEL 0 goto Success
    goto Fail

:Fail
    echo.
    echo Uninstallation FAILED.  See Uninstall.log for details.
    echo.
    goto End

:Success
    echo.
    echo Uninstallation Successful!
    echo.
    goto End
:End
popd
pause