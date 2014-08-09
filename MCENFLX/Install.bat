@echo off
echo.
echo.Media Center Netflix Search Fix > Install.log
echo. >> Install.log

echo Media Center Netflix Search Fix
echo.
echo   - Installing Service...
%WINDIR%\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe MCENFLX.exe 1>> Install.log 2>&1
echo.
if ERRORLEVEL 0 goto StartService
goto Fail
:StartService
    echo   - Starting service...
    net start MceNflx 1>> Install.log 2>&1
    if ERRORLEVEL 0 goto Success
    goto Fail

:Fail
    echo.
    echo Installation FAILED.  See Install.log for details.
    echo.
    goto End

:Success
    echo.
    echo Installation Successful!
    echo.
    goto End
:End
pause