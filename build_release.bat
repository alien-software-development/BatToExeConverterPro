@echo off
setlocal
title ALIEN BAT TO EXE CONVERTER PRO STUDIO v3.5 - BUILD
color 0a
echo ========================================================================
echo    ALIEN BAT TO EXE CONVERTER PRO STUDIO v3.5 - AUTOMATED BUILD
echo ========================================================================
echo.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0build_release.ps1"
if %errorlevel% neq 0 (
    color 0c
    echo.
    echo [ERROR] Build pipeline failed! Check errors above.
    pause
    exit /b %errorlevel%
)
echo.
echo Build finished successfully.
pause
