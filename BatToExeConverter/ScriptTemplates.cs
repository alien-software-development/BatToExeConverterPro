using System.Collections.Generic;

namespace BatToExeConverter;

public class ScriptTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public static class ScriptTemplateLibrary
{
    public static List<ScriptTemplate> GetTemplates()
    {
        return new List<ScriptTemplate>
        {
            new()
            {
                Name = "🛡️ UAC Self-Elevation Wrapper",
                Category = "Elevation & System",
                Description = "Automatically prompts for Administrator permissions with UAC elevation fallback.",
                Code = @"@echo off
:: =======================================================
:: Alien Auto UAC Self-Elevation Wrapper
:: =======================================================
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [OK] Running with full Administrator privileges.
) else (
    echo [!] Requesting administrative privileges...
    powershell -NoProfile -ExecutionPolicy Bypass -Command ""Start-Process -FilePath '%~f0' -Verb RunAs""
    exit /b
)

:: Your code begins here:
echo Administrative tasks executing...
pause
"
            },
            new()
            {
                Name = "⚡ Silent Background Worker",
                Category = "Execution & Performance",
                Description = "Executes operations silently in the background without UI interruption.",
                Code = @"@echo off
:: =======================================================
:: Alien Silent Background Worker Template
:: =======================================================
set LOGFILE=%TEMP%\alien_task_log.txt
echo [%date% %time%] Background task started. >> ""%LOGFILE%""

:: Add your background commands here:
timeout /t 2 /nobreak >nul
echo [%date% %time%] Performing silent maintenance... >> ""%LOGFILE%""

echo [%date% %time%] Task finished successfully. >> ""%LOGFILE%""
exit /b 0
"
            },
            new()
            {
                Name = "🎨 Cyber ANSI Color Console",
                Category = "UI & Display",
                Description = "Outputs stylish cyan, green, and amber colored headers and text.",
                Code = @"@echo off
cls
:: =======================================================
:: Alien Cyber ANSI Colored Console
:: =======================================================
echo [96m====================================================[0m
echo [92m       ALIEN SOFTWARE ADVANCED CONSOLE v3.0         [0m
echo [96m====================================================[0m
echo.
echo [93m[+] Initializing modules...[0m
echo [92m[✓] System check passed![0m
echo [94m[i] Ready for operations.[0m
echo.
pause
"
            },
            new()
            {
                Name = "🧹 Windows Deep System Cleaner",
                Category = "Maintenance",
                Description = "Cleans temporary directories, prefetch, and thumbnail caches safely.",
                Code = @"@echo off
title Alien System Deep Cleaner
color 0A
echo [*] Cleaning Windows Temp Folders...
del /s /f /q %temp%\*.* >nul 2>&1
del /s /f /q C:\Windows\Temp\*.* >nul 2>&1

echo [*] Flushing DNS Cache...
ipconfig /flushdns >nul 2>&1

echo [*] Cleaning Prefetch...
del /s /f /q C:\Windows\Prefetch\*.* >nul 2>&1

echo [✓] System Cleanup Completed Successfully!
timeout /t 3
"
            },
            new()
            {
                Name = "🌐 Network & Internet Diagnostics",
                Category = "Network",
                Description = "Tests ping latency, default gateway, and DNS resolution.",
                Code = @"@echo off
title Alien Network Diagnostics
echo ==================================================
echo         ALIEN NETWORK DIAGNOSTIC TOOL
echo ==================================================
echo.
echo Testing connection to Cloudflare DNS (1.1.1.1)...
ping -n 3 1.1.1.1 | findstr /i ""TTL average""
echo.
echo Testing connection to Google DNS (8.8.8.8)...
ping -n 3 8.8.8.8 | findstr /i ""TTL average""
echo.
echo Checking IP Configuration...
ipconfig | findstr /i ""IPv4 Default""
echo.
echo [✓] Diagnostic Complete.
pause
"
            },
            new()
            {
                Name = "🔥 Embedded PowerShell Bridge",
                Category = "Advanced",
                Description = "Executes inline high-performance PowerShell commands with JSON handling.",
                Code = @"@echo off
:: =======================================================
:: Alien Batch to PowerShell Hybrid Bridge
:: =======================================================
powershell -NoProfile -ExecutionPolicy Bypass -Command ""& {"" ^
    ""Write-Host 'Alien PowerShell Engine Active' -ForegroundColor Cyan;"" ^
    ""$os = (Get-CimInstance Win32_OperatingSystem).Caption;"" ^
    ""$cpu = (Get-CimInstance Win32_Processor).Name;"" ^
    ""$ram = [math]::Round((Get-CimInstance Win32_ComputerSystem).TotalPhysicalMemory / 1GB, 2);"" ^
    ""Write-Host 'OS:' $os -ForegroundColor Green;"" ^
    ""Write-Host 'CPU:' $cpu -ForegroundColor Green;"" ^
    ""Write-Host 'RAM:' $ram 'GB' -ForegroundColor Green;"" ^
""}""
pause
"
            },
            new()
            {
                Name = "💀 Self-Deleting Batch Script",
                Category = "Special",
                Description = "Executes commands and securely deletes itself upon termination.",
                Code = @"@echo off
echo Running one-time deployment task...
timeout /t 2 >nul
echo Task completed. Self-destructing script...
(goto) 2>nul & del ""%~f0""
"
            }
        };
    }
}
