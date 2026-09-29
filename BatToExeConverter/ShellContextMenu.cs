using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace BatToExeConverter;

public static class ShellContextMenu
{
    private const string MenuKey = @"Software\Classes\batfile\shell\BatToExePro";
    private const string MenuCmdKey = @"Software\Classes\batfile\shell\BatToExePro\command";

    public static bool IsRegistered()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(MenuKey);
            return key != null;
        }
        catch
        {
            return false;
        }
    }

    public static bool Register()
    {
        try
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            {
                return false;
            }

            using (var key = Registry.CurrentUser.CreateSubKey(MenuKey))
            {
                if (key != null)
                {
                    key.SetValue("", "👽 Convert to EXE with BatToExe Pro");
                    key.SetValue("Icon", $"\"{exePath}\"");
                }
            }

            using (var cmdKey = Registry.CurrentUser.CreateSubKey(MenuCmdKey))
            {
                if (cmdKey != null)
                {
                    cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }

            // Also for .cmd files
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\cmdfile\shell\BatToExePro"))
            {
                if (key != null)
                {
                    key.SetValue("", "👽 Convert to EXE with BatToExe Pro");
                    key.SetValue("Icon", $"\"{exePath}\"");
                }
            }

            using (var cmdKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\cmdfile\shell\BatToExePro\command"))
            {
                if (cmdKey != null)
                {
                    cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Unregister()
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\batfile\shell\BatToExePro", false);
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\cmdfile\shell\BatToExePro", false);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
