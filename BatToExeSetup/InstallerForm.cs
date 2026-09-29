using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using BatToExeConverter;
using Microsoft.Win32;

namespace BatToExeSetup;

public class InstallerForm : Form
{
    private TextBox txtInstallDir = null!;
    private CheckBox chkDesktopShortcut = null!;
    private CheckBox chkStartMenuShortcut = null!;
    private CheckBox chkContextMenu = null!;
    private CheckBox chkLaunchOnFinish = null!;
    private ProgressBar progressBar = null!;
    private Label lblStatus = null!;
    private ProButton btnInstall = null!;
    private ProButton btnCancel = null!;
    private ProButton? btnElevate;

    public InstallerForm()
    {
        InitializeInstallerUI();
        ApplyInstallerIcon();
    }

    public static bool IsAdministrator()
    {
        try
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    private void ApplyInstallerIcon()
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream("BatToExeSetup.Payload.Assets.installer_icon.ico")
                              ?? asm.GetManifestResourceStream("BatToExeSetup.Payload.Assets.app_icon.ico");
            if (stream != null)
            {
                this.Icon = new Icon(stream);
            }
        }
        catch { }
    }

    private void InitializeInstallerUI()
    {
        bool isAdmin = IsAdministrator();

        this.Text = "👽 BAT to EXE Converter Pro Studio v3.5 — Setup Wizard";
        this.Size = new Size(690, 590);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = UITheme.BgApp;
        this.Font = UITheme.FontBody;
        this.ForeColor = UITheme.TextWhite;

        // Header Panel
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 96,
            BackColor = UITheme.BgSidebar,
            Padding = new Padding(24, 14, 24, 12)
        };

        var lblLogo = new Label
        {
            Text = "👽 ALIEN SOFTWARE DEVELOPMENT",
            Font = UITheme.FontBrand,
            ForeColor = UITheme.BrandAccent,
            Location = new Point(24, 14),
            AutoSize = true
        };

        var lblTitle = new Label
        {
            Text = "BAT to EXE Converter Pro Studio v3.5 — Offline Setup",
            Font = UITheme.FontH1,
            ForeColor = UITheme.TextWhite,
            Location = new Point(24, 38),
            AutoSize = true
        };

        var lblPrivilegeBadge = new Label
        {
            Text = isAdmin ? "🛡 Administrator Mode (All Users / Program Files)" : "👤 Standard User Mode (Per-User Installation)",
            Font = UITheme.FontSmall,
            ForeColor = isAdmin ? UITheme.NeonGreen : UITheme.WarningAmber,
            Location = new Point(24, 68),
            AutoSize = true
        };

        pnlHeader.Controls.AddRange(new Control[] { lblLogo, lblTitle, lblPrivilegeBadge });
        this.Controls.Add(pnlHeader);

        // Body Panel
        var pnlBody = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24, 16, 24, 16),
            BackColor = UITheme.BgApp
        };

        // Destination Folder Card
        var cardPath = new ProCard
        {
            Height = 115,
            Dock = DockStyle.Top,
            BackColor = UITheme.BgCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var lblPathTitle = new Label
        {
            Text = "📂 Installation Destination Directory:",
            Font = UITheme.FontH3,
            ForeColor = UITheme.TextWhite,
            Location = new Point(14, 12),
            AutoSize = true
        };

        string defaultPath = isAdmin
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Alien Software Development", "BAT to EXE Converter Pro")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Alien Software Development", "BAT to EXE Converter Pro");

        txtInstallDir = new TextBox
        {
            Location = new Point(14, 42),
            Size = new Size(470, 32),
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextWhite,
            Font = UITheme.FontBody,
            BorderStyle = BorderStyle.FixedSingle,
            Text = defaultPath
        };

        var btnBrowse = new ProButton
        {
            Text = "Browse...",
            Location = new Point(495, 40),
            Size = new Size(110, 32),
            BgNormal = UITheme.BrandAccent,
            BgHover = UITheme.BrandAccentHover,
            Font = UITheme.FontSmall,
            Radius = 6
        };
        btnBrowse.Click += (s, e) =>
        {
            using var fbd = new FolderBrowserDialog { SelectedPath = txtInstallDir.Text };
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                txtInstallDir.Text = fbd.SelectedPath;
            }
        };

        var lblSpace = new Label
        {
            Text = "Required Space: ~180 MB | Native Offline Package | Windows 10 & 11 Ready",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextMuted,
            Location = new Point(14, 82),
            AutoSize = true
        };

        cardPath.Controls.AddRange(new Control[] { lblPathTitle, txtInstallDir, btnBrowse, lblSpace });

        var gap1 = new Panel { Height = 12, Dock = DockStyle.Top, BackColor = Color.Transparent };

        // Options Card
        var cardOpts = new ProCard
        {
            Height = 155,
            Dock = DockStyle.Top,
            BackColor = UITheme.BgCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var lblOptsHead = new Label
        {
            Text = "⚙ Additional Tasks & Integration:",
            Font = UITheme.FontH3,
            ForeColor = UITheme.TextWhite,
            Location = new Point(14, 12),
            AutoSize = true
        };

        chkDesktopShortcut = new CheckBox
        {
            Text = "Create Desktop Shortcut (with Alien Cyber Icon)",
            Location = new Point(16, 40),
            AutoSize = true,
            Checked = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        chkStartMenuShortcut = new CheckBox
        {
            Text = "Create Start Menu Program Group & Shortcuts",
            Location = new Point(16, 66),
            AutoSize = true,
            Checked = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        chkContextMenu = new CheckBox
        {
            Text = "Register Windows Explorer Context Menu ('👽 Convert to EXE with BatToExe Pro')",
            Location = new Point(16, 92),
            AutoSize = true,
            Checked = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        chkLaunchOnFinish = new CheckBox
        {
            Text = "Launch BAT to EXE Converter Pro Studio after installation",
            Location = new Point(16, 118),
            AutoSize = true,
            Checked = true,
            ForeColor = UITheme.NeonGreen,
            Font = UITheme.FontBold
        };

        cardOpts.Controls.AddRange(new Control[] { lblOptsHead, chkDesktopShortcut, chkStartMenuShortcut, chkContextMenu, chkLaunchOnFinish });

        var gap2 = new Panel { Height = 12, Dock = DockStyle.Top, BackColor = Color.Transparent };

        // Progress Panel
        var cardProgress = new ProCard
        {
            Height = 72,
            Dock = DockStyle.Top,
            BackColor = UITheme.BgCard,
            Padding = new Padding(16, 10, 16, 10)
        };

        lblStatus = new Label
        {
            Text = "Ready to install standalone offline package.",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextSecondary,
            Location = new Point(14, 10),
            AutoSize = true
        };

        progressBar = new ProgressBar
        {
            Location = new Point(14, 34),
            Size = new Size(590, 20),
            Style = ProgressBarStyle.Blocks,
            Value = 0
        };

        cardProgress.Controls.AddRange(new Control[] { lblStatus, progressBar });

        pnlBody.Controls.AddRange(new Control[] { cardProgress, gap2, cardOpts, gap1, cardPath });
        this.Controls.Add(pnlBody);
        pnlBody.BringToFront();

        // Bottom Actions Bar
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            BackColor = UITheme.BgSidebar,
            Padding = new Padding(24, 12, 24, 12)
        };

        btnInstall = new ProButton
        {
            Text = "⚡ INSTALL NOW",
            Size = new Size(160, 40),
            Location = new Point(pnlBottom.Width - 190, 12),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BgNormal = UITheme.NeonGreen,
            BgHover = UITheme.NeonGreenHover,
            Font = UITheme.FontBold,
            Radius = 6
        };
        btnInstall.Click += async (s, e) => await StartInstallationAsync();

        btnCancel = new ProButton
        {
            Text = "Cancel",
            Size = new Size(100, 40),
            Location = new Point(pnlBottom.Width - 300, 12),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.DangerRed,
            Font = UITheme.FontBody,
            Radius = 6
        };
        btnCancel.Click += (s, e) => this.Close();

        if (!isAdmin)
        {
            btnElevate = new ProButton
            {
                Text = "🛡 Run as Admin",
                Size = new Size(140, 40),
                Location = new Point(24, 12),
                BgNormal = UITheme.BrandAccent,
                BgHover = UITheme.BrandAccentHover,
                Font = UITheme.FontSmall,
                Radius = 6
            };
            btnElevate.Click += (s, e) => RelaunchAsAdministrator();
            pnlBottom.Controls.Add(btnElevate);
        }

        pnlBottom.Controls.AddRange(new Control[] { btnCancel, btnInstall });
        this.Controls.Add(pnlBottom);
    }

    private void RelaunchAsAdministrator()
    {
        try
        {
            string exePath = Environment.ProcessPath ?? Application.ExecutablePath;
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true,
                Verb = "runas"
            };
            Process.Start(psi);
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Could not elevate with Administrator permissions: " + ex.Message, "UAC Elevation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private static bool TestDirectoryWriteAccess(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string testFile = Path.Combine(path, ".test_" + Guid.NewGuid().ToString("N"));
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task StartInstallationAsync()
    {
        string targetDir = txtInstallDir.Text.Trim();
        if (string.IsNullOrWhiteSpace(targetDir))
        {
            MessageBox.Show(this, "Please select an installation folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Test permission to write to targetDir
        if (!TestDirectoryWriteAccess(targetDir))
        {
            if (!IsAdministrator())
            {
                var dr = MessageBox.Show(this,
                    $"Access to '{targetDir}' requires Administrator permissions.\n\nWould you like to restart the setup wizard as Administrator now?",
                    "Administrator Permissions Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    RelaunchAsAdministrator();
                    return;
                }
            }

            MessageBox.Show(this, $"Failed to access installation path:\n{targetDir}\n\nPermission denied. Please select a user folder (like LocalAppData) or run setup as Administrator.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        btnInstall.Enabled = false;
        btnCancel.Enabled = false;
        if (btnElevate != null) btnElevate.Enabled = false;
        txtInstallDir.ReadOnly = true;
        progressBar.Value = 10;
        lblStatus.Text = "Creating installation directories...";
        lblStatus.ForeColor = UITheme.TextSecondary;

        string targetExe = Path.Combine(targetDir, "BatToExeConverter.exe");
        string keygenExe = Path.Combine(targetDir, "AlienKeygen.exe");

        bool success = false;
        string? failureReason = null;

        await Task.Run(() =>
        {
            try
            {
                Directory.CreateDirectory(targetDir);
                string assetsDir = Path.Combine(targetDir, "Assets");
                Directory.CreateDirectory(assetsDir);
                string presetDir = Path.Combine(assetsDir, "PresetIcons");
                Directory.CreateDirectory(presetDir);

                var asm = Assembly.GetExecutingAssembly();
                string[] resourceNames = asm.GetManifestResourceNames();

                int count = 0;
                foreach (var resName in resourceNames)
                {
                    this.Invoke(() =>
                    {
                        lblStatus.Text = $"Extracting payload files ({count + 1}/{resourceNames.Length})...";
                        progressBar.Value = Math.Min(80, 10 + (int)((count / (float)resourceNames.Length) * 70));
                    });

                    if (resName.EndsWith("BatToExeConverter.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        ExtractResource(asm, resName, targetExe);
                    }
                    else if (resName.EndsWith("AlienKeygen.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        ExtractResource(asm, resName, keygenExe);
                    }
                    else if (resName.Contains("PresetIcons", StringComparison.OrdinalIgnoreCase))
                    {
                        string fileName = GetFileNameFromResource(resName);
                        ExtractResource(asm, resName, Path.Combine(presetDir, fileName));
                    }
                    else if (resName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) || resName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        string fileName = GetFileNameFromResource(resName);
                        ExtractResource(asm, resName, Path.Combine(assetsDir, fileName));
                    }

                    count++;
                }

                this.Invoke(() =>
                {
                    lblStatus.Text = "Creating uninstaller & registering Windows components...";
                    progressBar.Value = 85;
                });

                CreateUninstaller(targetDir);
                RegisterUninstallEntry(targetDir, targetExe);

                if (chkContextMenu.Checked)
                {
                    RegisterShellContextMenu(targetExe);
                }

                if (chkDesktopShortcut.Checked)
                {
                    string desktop = IsAdministrator()
                        ? Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
                        : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                    if (string.IsNullOrEmpty(desktop) || !Directory.Exists(desktop))
                    {
                        desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    }

                    CreateShortcut(Path.Combine(desktop, "BAT to EXE Converter Pro.lnk"), targetExe, targetDir, "Alien BAT to EXE Converter Pro Studio");
                }

                if (chkStartMenuShortcut.Checked)
                {
                    string baseStart = IsAdministrator()
                        ? Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms)
                        : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");

                    if (string.IsNullOrEmpty(baseStart) || !Directory.Exists(baseStart))
                    {
                        baseStart = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");
                    }

                    string startMenu = Path.Combine(baseStart, "Alien Software Development");
                    Directory.CreateDirectory(startMenu);
                    CreateShortcut(Path.Combine(startMenu, "BAT to EXE Converter Pro.lnk"), targetExe, targetDir, "Alien BAT to EXE Converter Pro Studio");
                    CreateShortcut(Path.Combine(startMenu, "Alien Master Keygen.lnk"), keygenExe, targetDir, "Alien Master Keygen");
                    CreateShortcut(Path.Combine(startMenu, "Uninstall BAT to EXE Pro.lnk"), Path.Combine(targetDir, "Uninstall.bat"), targetDir, "Uninstall BAT to EXE Converter Pro");
                }

                success = true;
            }
            catch (Exception ex)
            {
                failureReason = ex.Message;
            }
        });

        if (!success)
        {
            btnInstall.Enabled = true;
            btnCancel.Enabled = true;
            if (btnElevate != null) btnElevate.Enabled = true;
            txtInstallDir.ReadOnly = false;
            lblStatus.Text = "Installation failed: " + failureReason;
            lblStatus.ForeColor = UITheme.DangerRed;

            MessageBox.Show(this, "Failed to complete offline installation:\n" + failureReason, "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        progressBar.Value = 100;
        lblStatus.Text = "Installation completed successfully!";
        lblStatus.ForeColor = UITheme.NeonGreen;

        MessageBox.Show(
            this,
            "BAT to EXE Converter Pro Studio v3.5 has been successfully installed on your computer!",
            "Installation Complete",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );

        if (chkLaunchOnFinish.Checked && File.Exists(targetExe))
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = targetExe, UseShellExecute = true });
            }
            catch { }
        }

        this.Close();
    }

    private static void ExtractResource(Assembly asm, string resName, string outPath)
    {
        try
        {
            using var stream = asm.GetManifestResourceStream(resName);
            if (stream != null)
            {
                using var fileStream = File.Create(outPath);
                stream.CopyTo(fileStream);
            }
        }
        catch { }
    }

    private static string GetFileNameFromResource(string resName)
    {
        var parts = resName.Split('.');
        if (parts.Length >= 2)
        {
            return parts[^2] + "." + parts[^1];
        }
        return resName;
    }

    private static void CreateShortcut(string shortcutPath, string targetPath, string workingDir, string description)
    {
        try
        {
            Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType != null)
            {
                dynamic shell = Activator.CreateInstance(shellType)!;
                var shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = workingDir;
                shortcut.IconLocation = $"{targetPath},0";
                shortcut.Description = description;
                shortcut.Save();
            }
        }
        catch { }
    }

    private static void RegisterShellContextMenu(string exePath)
    {
        try
        {
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\batfile\shell\BatToExePro"))
            {
                if (key != null)
                {
                    key.SetValue("", "👽 Convert to EXE with BatToExe Pro");
                    key.SetValue("Icon", $"\"{exePath}\"");
                }
            }

            using (var cmdKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\batfile\shell\BatToExePro\command"))
            {
                if (cmdKey != null)
                {
                    cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }

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

            if (IsAdministrator())
            {
                using var hklmBat = Registry.ClassesRoot.CreateSubKey(@"batfile\shell\BatToExePro");
                if (hklmBat != null)
                {
                    hklmBat.SetValue("", "👽 Convert to EXE with BatToExe Pro");
                    hklmBat.SetValue("Icon", $"\"{exePath}\"");
                    using var sub = hklmBat.CreateSubKey("command");
                    sub?.SetValue("", $"\"{exePath}\" \"%1\"");
                }

                using var hklmCmd = Registry.ClassesRoot.CreateSubKey(@"cmdfile\shell\BatToExePro");
                if (hklmCmd != null)
                {
                    hklmCmd.SetValue("", "👽 Convert to EXE with BatToExe Pro");
                    hklmCmd.SetValue("Icon", $"\"{exePath}\"");
                    using var sub = hklmCmd.CreateSubKey("command");
                    sub?.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }
        }
        catch { }
    }

    private static void CreateUninstaller(string targetDir)
    {
        string uninstBat = Path.Combine(targetDir, "Uninstall.bat");
        string content = @"@echo off
title Alien BAT to EXE Pro Studio - Uninstaller
color 0a
echo ====================================================
echo   Alien BAT to EXE Converter Pro Studio - Uninstaller
echo ====================================================
echo.
set /p confirm=""Are you sure you want to completely uninstall BAT to EXE Converter Pro? (Y/N): ""
if /i not ""%confirm%""==""Y"" exit /b

echo Cleaning Windows Shell context menus...
reg delete ""HKCU\Software\Classes\batfile\shell\BatToExePro"" /f >nul 2>&1
reg delete ""HKCU\Software\Classes\cmdfile\shell\BatToExePro"" /f >nul 2>&1
reg delete ""HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\AlienBatToExePro"" /f >nul 2>&1
reg delete ""HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\AlienBatToExePro"" /f >nul 2>&1
reg delete ""HKCR\batfile\shell\BatToExePro"" /f >nul 2>&1
reg delete ""HKCR\cmdfile\shell\BatToExePro"" /f >nul 2>&1

echo Removing Desktop and Start Menu shortcuts...
del ""%PUBLIC%\Desktop\BAT to EXE Converter Pro.lnk"" >nul 2>&1
del ""%USERPROFILE%\Desktop\BAT to EXE Converter Pro.lnk"" >nul 2>&1
rmdir /s /q ""%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\Alien Software Development"" >nul 2>&1
rmdir /s /q ""%APPDATA%\Microsoft\Windows\Start Menu\Programs\Alien Software Development"" >nul 2>&1

echo Removing installation files...
timeout /t 1 >nul
(goto) 2>nul & rmdir /s /q """ + targetDir + @"""
";
        File.WriteAllText(uninstBat, content);
    }

    private static void RegisterUninstallEntry(string targetDir, string exePath)
    {
        void WriteEntry(RegistryKey? root)
        {
            if (root == null) return;
            try
            {
                using var key = root.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\AlienBatToExePro");
                if (key != null)
                {
                    key.SetValue("DisplayName", "BAT to EXE Converter Pro Studio");
                    key.SetValue("DisplayVersion", "3.5.0");
                    key.SetValue("Publisher", "Alien Software Development");
                    key.SetValue("DisplayIcon", $"\"{exePath}\",0");
                    key.SetValue("UninstallString", $"cmd.exe /c \"{Path.Combine(targetDir, "Uninstall.bat")}\"");
                    key.SetValue("InstallLocation", targetDir);
                    key.SetValue("HelpLink", "https://wa.me/8801710978997");
                    key.SetValue("URLInfoAbout", "https://aliensoftwaredevelopment.com");
                }
            }
            catch { }
        }

        WriteEntry(Registry.CurrentUser);
        if (IsAdministrator())
        {
            WriteEntry(Registry.LocalMachine);
        }
    }
}
