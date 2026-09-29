using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace BatToExeConverter;

public class MainForm : Form
{
    // Master Layout Panels
    private Panel pnlSidebar = null!;
    private Panel pnlRightArea = null!;
    private Panel pnlTopBar = null!;
    private Panel pnlContentContainer = null!;
    private Panel pnlBottomBar = null!;

    // Sidebar Navigation Buttons
    private SidebarNavButton btnNavStudio = null!;
    private SidebarNavButton btnNavEditor = null!;
    private SidebarNavButton btnNavOptions = null!;
    private SidebarNavButton btnNavSecurity = null!;
    private SidebarNavButton btnNavVersion = null!;
    private SidebarNavButton btnNavAssets = null!;
    private SidebarNavButton btnNavTools = null!;
    private SidebarNavButton btnNavLicense = null!;

    // Header Controls
    private Label lblHeaderTitle = null!;
    private Label lblHeaderSubtitle = null!;
    private ProButton btnLicensePill = null!;

    // Content Views
    private Panel pnlViewStudio = null!;
    private Panel pnlViewEditor = null!;
    private Panel pnlViewOptions = null!;
    private Panel pnlViewSecurity = null!;
    private Panel pnlViewVersion = null!;
    private Panel pnlViewAssets = null!;
    private Panel pnlViewTools = null!;

    // Studio Inputs
    private ModernInputBox txtBatPath = null!;
    private ModernInputBox txtExePath = null!;
    private ModernInputBox txtIconPath = null!;
    private PictureBox picIcon = null!;

    // Script Editor Inputs
    private TextBox txtScriptEditor = null!;
    private ComboBox cmbTemplates = null!;
    private Label lblEditorStats = null!;

    // Options Inputs
    private ComboBox cmbVisibility = null!;
    private ComboBox cmbArchitecture = null!;
    private ComboBox cmbPriority = null!;
    private CheckBox chkRequireAdmin = null!;
    private CheckBox chkSingleInstance = null!;
    private RadioButton radDirCurrent = null!;
    private RadioButton radDirTemp = null!;
    private CheckBox chkDeleteOnExit = null!;
    private ModernInputBox txtConsoleTitle = null!;
    private CheckBox chkLogErrors = null!;

    // Security Inputs
    private CheckBox chkEncrypt = null!;
    private ComboBox cmbObfuscation = null!;
    private ModernInputBox txtPassword = null!;
    private CheckBox chkShowPassword = null!;
    private CheckBox chkAntiAnalysis = null!;
    private CheckBox chkAutoStartup = null!;
    private CheckBox chkIntegrity = null!;
    private ModernInputBox txtExecutionTimeout = null!;
    private ModernInputBox txtFakeError = null!;
    private ModernInputBox txtSplashMsg = null!;
    private ModernInputBox txtDefaultArgs = null!;

    // Version Inputs
    private ModernInputBox txtAppName = null!;
    private ModernInputBox txtDescription = null!;
    private ModernInputBox txtCompany = null!;
    private ModernInputBox txtProduct = null!;
    private ModernInputBox txtCopyright = null!;
    private ModernInputBox txtFileVersion = null!;
    private ModernInputBox txtProductVersion = null!;
    private ModernInputBox txtComments = null!;

    // Assets Inputs
    private ListBox listAssets = null!;

    // Tools Inputs
    private Label lblContextStatus = null!;
    private ProButton btnToggleContextMenu = null!;

    // Bottom Action & Console Controls
    private Label lblStatusDot = null!;
    private Label lblStatusText = null!;
    private ProButton btnCompile = null!;
    private ProButton btnOpenFolder = null!;
    private ProButton btnRunExe = null!;
    private TextBox txtLogs = null!;

    private string? lastGeneratedExe = null;

    public MainForm(string[]? startupArgs = null)
    {
        InitializeProStudio();
        ApplyApplicationIcon();
        RefreshLicenseUI();

        if (startupArgs != null && startupArgs.Length > 0 && File.Exists(startupArgs[0]))
        {
            LoadBatchFile(startupArgs[0]);
        }

        this.KeyPreview = true;
        this.KeyDown += (s, e) =>
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.K)
            {
                using var kg = new KeygenForm();
                if (kg.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshLicenseUI();
                }
            }
        };
    }

    private void ApplyApplicationIcon()
    {
        try
        {
            string[] iconPaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "Assets", "app_icon.ico"),
                Path.Combine(AppContext.BaseDirectory, "app_icon.ico"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Assets", "app_icon.ico")
            };

            foreach (var p in iconPaths)
            {
                if (File.Exists(p))
                {
                    this.Icon = new Icon(p);
                    break;
                }
            }
        }
        catch { }
    }

    private void InitializeProStudio()
    {
        this.Text = "BAT to EXE Converter Pro — Alien Software Development Studio";
        this.Size = new Size(1180, 820);
        this.MinimumSize = new Size(1060, 720);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = UITheme.BgApp;
        this.Font = UITheme.FontBody;
        this.ForeColor = UITheme.TextPrimary;
        this.DoubleBuffered = true;

        BuildSidebar();

        pnlRightArea = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.BgApp
        };

        BuildTopBar();
        BuildBottomBar();

        pnlContentContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.BgApp,
            Padding = new Padding(8, 6, 8, 6)
        };
        pnlRightArea.Controls.Add(pnlContentContainer);

        this.Controls.Add(pnlRightArea);
        this.Controls.Add(pnlSidebar);
        pnlSidebar.BringToFront();

        BuildViewStudio();
        BuildViewEditor();
        BuildViewOptions();
        BuildViewSecurity();
        BuildViewVersion();
        BuildViewAssets();
        BuildViewTools();

        SwitchView(pnlViewStudio, btnNavStudio, "👽 Converter Studio", "Convert batch scripts into protected, standalone Native PE executables");
    }

    private void BuildSidebar()
    {
        pnlSidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 240,
            BackColor = UITheme.BgSidebar
        };

        var pnlBrand = new Panel
        {
            Dock = DockStyle.Top,
            Height = 76,
            BackColor = UITheme.BgSidebar,
            Padding = new Padding(18, 16, 18, 12)
        };

        var lblLogo = new Label
        {
            Text = "👽 ALIEN SOFTWARE",
            Font = UITheme.FontBrand,
            ForeColor = UITheme.BrandAccent,
            Location = new Point(16, 14),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = "BAT to EXE Pro Studio v3.5",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextSecondary,
            Location = new Point(18, 38),
            AutoSize = true
        };

        pnlBrand.Controls.AddRange(new Control[] { lblLogo, lblSub });
        pnlSidebar.Controls.Add(pnlBrand);

        var pnlNav = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(0, 8, 0, 8)
        };

        btnNavStudio = new SidebarNavButton { Width = 230, IconChar = "⚡", Title = "Converter Studio" };
        btnNavEditor = new SidebarNavButton { Width = 230, IconChar = "📜", Title = "Script Editor & Tester" };
        btnNavOptions = new SidebarNavButton { Width = 230, IconChar = "⚙", Title = "Execution Options" };
        btnNavSecurity = new SidebarNavButton { Width = 230, IconChar = "🛡", Title = "Security & Armor" };
        btnNavVersion = new SidebarNavButton { Width = 230, IconChar = "🏷", Title = "Version & Branding" };
        btnNavAssets = new SidebarNavButton { Width = 230, IconChar = "📦", Title = "Embedded Assets" };
        btnNavTools = new SidebarNavButton { Width = 230, IconChar = "🛠", Title = "Tools & Integration" };
        btnNavLicense = new SidebarNavButton { Width = 230, IconChar = "🔑", Title = "License & Activation" };

        btnNavStudio.Click += (s, e) => SwitchView(pnlViewStudio, btnNavStudio, "👽 Converter Studio", "Convert batch scripts into protected, standalone Native PE executables");
        btnNavEditor.Click += (s, e) => SwitchView(pnlViewEditor, btnNavEditor, "📜 Batch Script Editor & Live Tester", "Create, edit, debug, and test batch scripts with built-in templates");
        btnNavOptions.Click += (s, e) => SwitchView(pnlViewOptions, btnNavOptions, "⚙ Execution Settings", "Configure visibility modes, CPU architecture, priority, and runtime parameters");
        btnNavSecurity.Click += (s, e) => SwitchView(pnlViewSecurity, btnNavSecurity, "🛡 Security, Encryption & Obfuscation", "Dynamic AES-256 cipher, code obfuscation armor, password protection & anti-analysis guard");
        btnNavVersion.Click += (s, e) => SwitchView(pnlViewVersion, btnNavVersion, "🏷 Version & Assembly Branding", "Embed Windows PE metadata, company name, copyright, and versions");
        btnNavAssets.Click += (s, e) => SwitchView(pnlViewAssets, btnNavAssets, "📦 Embedded Auxiliary Assets", "Bundle PowerShell scripts (.ps1), DLLs, configs, and assets directly inside the .exe");
        btnNavTools.Click += (s, e) => SwitchView(pnlViewTools, btnNavTools, "🛠 Tools & Shell Integration", "Windows Explorer context menu integration, preset icon gallery, and keygen utilities");
        btnNavLicense.Click += (s, e) => OpenLicenseDialog();

        pnlNav.Controls.AddRange(new Control[]
        {
            btnNavStudio, btnNavEditor, btnNavOptions, btnNavSecurity,
            btnNavVersion, btnNavAssets, btnNavTools, btnNavLicense
        });
        pnlSidebar.Controls.Add(pnlNav);

        var pnlFooter = new ProCard
        {
            Dock = DockStyle.Bottom,
            Height = 135,
            BackColor = UITheme.BgApp,
            CustomBorderColor = UITheme.Border,
            Margin = new Padding(8),
            Padding = new Padding(10, 10, 12, 12)
        };

        var lblDevHead = new Label
        {
            Text = "Alien Software Development",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = UITheme.TextWhite,
            Location = new Point(10, 8),
            AutoSize = true
        };

        var lnkSupport = new LinkLabel
        {
            Text = "💬 WhatsApp: +8801710978997",
            LinkColor = UITheme.NeonGreen,
            ActiveLinkColor = UITheme.BrandAccent,
            Font = UITheme.FontSmall,
            Location = new Point(10, 32),
            AutoSize = true
        };
        lnkSupport.Click += (s, e) => OpenUrl("https://wa.me/8801710978997");

        var lnkWeb = new LinkLabel
        {
            Text = "🌐 aliensoftwaredevelopment.com",
            LinkColor = UITheme.BrandAccent,
            ActiveLinkColor = UITheme.NeonGreen,
            Font = UITheme.FontSmall,
            Location = new Point(10, 56),
            AutoSize = true
        };
        lnkWeb.Click += (s, e) => OpenUrl("https://aliensoftwaredevelopment.com");

        var lblSec = new Label
        {
            Text = "🔒 Official Cryptographic License",
            Font = new Font("Segoe UI", 7.5f, FontStyle.Italic),
            ForeColor = UITheme.TextMuted,
            Location = new Point(10, 82),
            AutoSize = true
        };

        pnlFooter.Controls.AddRange(new Control[] { lblDevHead, lnkSupport, lnkWeb, lblSec });
        pnlSidebar.Controls.Add(pnlFooter);
    }

    private void BuildTopBar()
    {
        pnlTopBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = UITheme.BgSidebar,
            Padding = new Padding(24, 12, 24, 12)
        };

        lblHeaderTitle = new Label
        {
            Text = "👽 Converter Studio",
            Font = UITheme.FontH1,
            ForeColor = UITheme.TextWhite,
            Location = new Point(20, 12),
            AutoSize = true
        };

        lblHeaderSubtitle = new Label
        {
            Text = "Convert batch scripts into protected, standalone PE executables",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextSecondary,
            Location = new Point(22, 38),
            AutoSize = true
        };

        btnLicensePill = new ProButton
        {
            Text = "Checking License...",
            Size = new Size(200, 34),
            Location = new Point(pnlTopBar.Width - 224, 15),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.BrandAccent,
            Font = UITheme.FontSmall,
            Radius = 17
        };
        btnLicensePill.Click += (s, e) => OpenLicenseDialog();

        pnlTopBar.Controls.AddRange(new Control[] { lblHeaderTitle, lblHeaderSubtitle, btnLicensePill });
        pnlRightArea.Controls.Add(pnlTopBar);
    }

    private void BuildBottomBar()
    {
        pnlBottomBar = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 160,
            BackColor = UITheme.BgSidebar,
            Padding = new Padding(20, 8, 20, 10)
        };

        var pnlActions = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44
        };

        lblStatusDot = new Label
        {
            Text = "●",
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = UITheme.NeonGreen,
            Location = new Point(0, 10),
            Size = new Size(20, 24)
        };

        lblStatusText = new Label
        {
            Text = "Ready to compile batch script.",
            Font = UITheme.FontBold,
            ForeColor = UITheme.TextPrimary,
            Location = new Point(22, 12),
            AutoSize = true
        };

        btnCompile = new ProButton
        {
            Text = "⚡ CONVERT TO .EXE",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            BgNormal = UITheme.NeonGreen,
            BgHover = UITheme.NeonGreenHover,
            Size = new Size(190, 38),
            Location = new Point(pnlActions.Width - 190, 3),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Radius = 6
        };
        btnCompile.Click += BtnCompile_Click;

        btnOpenFolder = new ProButton
        {
            Text = "📁 Open Folder",
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.BrandAccent,
            Size = new Size(120, 38),
            Location = new Point(pnlActions.Width - 318, 3),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Enabled = false,
            Radius = 6
        };
        btnOpenFolder.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(lastGeneratedExe) && File.Exists(lastGeneratedExe))
            {
                Process.Start("explorer.exe", $"/select,\"{lastGeneratedExe}\"");
            }
        };

        btnRunExe = new ProButton
        {
            Text = "▶ Run Output",
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.BrandAccent,
            Size = new Size(115, 38),
            Location = new Point(pnlActions.Width - 440, 3),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Enabled = false,
            Radius = 6
        };
        btnRunExe.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(lastGeneratedExe) && File.Exists(lastGeneratedExe))
            {
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = lastGeneratedExe, UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Failed to run executable: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        };

        pnlActions.Controls.AddRange(new Control[] { lblStatusDot, lblStatusText, btnRunExe, btnOpenFolder, btnCompile });

        var pnlTerminalCard = new ProCard
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.BgTerminal,
            CustomBorderColor = UITheme.Border,
            Padding = new Padding(10, 6, 10, 6),
            Radius = 6
        };

        txtLogs = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = UITheme.BgTerminal,
            ForeColor = Color.FromArgb(203, 213, 225),
            Font = UITheme.FontCode,
            BorderStyle = BorderStyle.None,
            Text = "[Alien Studio Pro v3.5] Ready. Select a .bat file and click 'CONVERT TO .EXE' to build standalone binary." + Environment.NewLine
        };

        pnlTerminalCard.Controls.Add(txtLogs);
        pnlBottomBar.Controls.AddRange(new Control[] { pnlTerminalCard, pnlActions });
        pnlRightArea.Controls.Add(pnlBottomBar);
    }

    private void BuildViewStudio()
    {
        pnlViewStudio = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20, 16, 20, 16)
        };

        var cardBat = new ProCard
        {
            Height = 96,
            Dock = DockStyle.Top,
            BackColor = UITheme.BgCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var lblBatTitle = new Label
        {
            Text = "📜 Step 1: Select Source Batch Script (.bat / .cmd) *",
            Font = UITheme.FontH3,
            ForeColor = UITheme.TextWhite,
            Location = new Point(14, 10),
            AutoSize = true
        };

        txtBatPath = new ModernInputBox
        {
            Location = new Point(14, 38),
            Size = new Size(520, 38)
        };
        txtBatPath.InnerTextBox.Click += (s, e) => OpenBatFileDialog();
        txtBatPath.ValueChanged += (s, e) =>
        {
            if (File.Exists(txtBatPath.TextValue))
            {
                if (string.IsNullOrWhiteSpace(txtExePath.TextValue))
                    txtExePath.TextValue = Path.ChangeExtension(txtBatPath.TextValue, ".exe");
                txtAppName.TextValue = Path.GetFileNameWithoutExtension(txtBatPath.TextValue);
            }
        };

        var btnBrowseBat = new ProButton
        {
            Text = "📁 Browse Script...",
            Location = new Point(542, 38),
            Size = new Size(150, 38),
            BgNormal = UITheme.BrandAccent,
            BgHover = UITheme.BrandAccentHover,
            Radius = 6
        };
        btnBrowseBat.Click += (s, e) => OpenBatFileDialog();

        var btnClearBat = new ProButton
        {
            Text = "Clear",
            Location = new Point(700, 38),
            Size = new Size(70, 38),
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.DangerRed,
            Radius = 6
        };
        btnClearBat.Click += (s, e) => txtBatPath.TextValue = string.Empty;

        cardBat.Controls.AddRange(new Control[] { lblBatTitle, txtBatPath, btnBrowseBat, btnClearBat });

        var gap1 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardExe = new ProCard
        {
            Height = 96,
            Dock = DockStyle.Top,
            BackColor = UITheme.BgCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var lblExeTitle = new Label
        {
            Text = "🎯 Step 2: Target Executable Output (.exe) *",
            Font = UITheme.FontH3,
            ForeColor = UITheme.TextWhite,
            Location = new Point(14, 10),
            AutoSize = true
        };

        txtExePath = new ModernInputBox
        {
            Location = new Point(14, 38),
            Size = new Size(520, 38)
        };
        txtExePath.InnerTextBox.Click += (s, e) => OpenExeSaveDialog();

        var btnBrowseExe = new ProButton
        {
            Text = "💾 Save As...",
            Location = new Point(542, 38),
            Size = new Size(150, 38),
            BgNormal = UITheme.BrandAccent,
            BgHover = UITheme.BrandAccentHover,
            Radius = 6
        };
        btnBrowseExe.Click += (s, e) => OpenExeSaveDialog();

        var btnClearExe = new ProButton
        {
            Text = "Clear",
            Location = new Point(700, 38),
            Size = new Size(70, 38),
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.DangerRed,
            Radius = 6
        };
        btnClearExe.Click += (s, e) => txtExePath.TextValue = string.Empty;

        cardExe.Controls.AddRange(new Control[] { lblExeTitle, txtExePath, btnBrowseExe, btnClearExe });

        var gap2 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardIco = new ProCard
        {
            Height = 96,
            Dock = DockStyle.Top,
            BackColor = UITheme.BgCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var lblIcoTitle = new Label
        {
            Text = "🎨 Step 3: Application Icon (.ico) [Custom or Built-in Pro Preset]",
            Font = UITheme.FontH3,
            ForeColor = UITheme.TextWhite,
            Location = new Point(14, 10),
            AutoSize = true
        };

        picIcon = new PictureBox
        {
            Location = new Point(14, 38),
            Size = new Size(38, 38),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = UITheme.BgInput,
            BorderStyle = BorderStyle.FixedSingle
        };

        txtIconPath = new ModernInputBox
        {
            Location = new Point(60, 38),
            Size = new Size(420, 38)
        };
        txtIconPath.InnerTextBox.Click += (s, e) => OpenIconFileDialog();
        txtIconPath.ValueChanged += (s, e) => LoadIconPreview(txtIconPath.TextValue);

        var btnBrowseIco = new ProButton
        {
            Text = "📁 Browse...",
            Location = new Point(488, 38),
            Size = new Size(110, 38),
            BgNormal = UITheme.BrandAccent,
            BgHover = UITheme.BrandAccentHover,
            Radius = 6
        };
        btnBrowseIco.Click += (s, e) => OpenIconFileDialog();

        var btnPresetIcons = new ProButton
        {
            Text = "🎨 Pro Presets...",
            Location = new Point(606, 38),
            Size = new Size(130, 38),
            BgNormal = UITheme.Purple,
            BgHover = Color.FromArgb(147, 51, 234),
            Radius = 6
        };
        btnPresetIcons.Click += (s, e) =>
        {
            using var picker = new PresetIconPicker();
            if (picker.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(picker.SelectedIconPath))
            {
                txtIconPath.TextValue = picker.SelectedIconPath;
                LoadIconPreview(picker.SelectedIconPath);
            }
        };

        var btnClearIco = new ProButton
        {
            Text = "Clear",
            Location = new Point(744, 38),
            Size = new Size(60, 38),
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.DangerRed,
            Radius = 6
        };
        btnClearIco.Click += (s, e) =>
        {
            txtIconPath.TextValue = string.Empty;
            picIcon.Image = null;
        };

        cardIco.Controls.AddRange(new Control[] { lblIcoTitle, picIcon, txtIconPath, btnBrowseIco, btnPresetIcons, btnClearIco });

        var gap3 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardTips = new ProCard
        {
            Height = 120,
            Dock = DockStyle.Top,
            BackColor = UITheme.BgSidebar,
            CustomBorderColor = UITheme.Border,
            Padding = new Padding(16, 12, 16, 12)
        };

        var lblTip1 = new Label
        {
            Text = "💡 Pro Compilation Tips & Highlights:",
            Font = UITheme.FontBold,
            ForeColor = UITheme.BrandAccent,
            Location = new Point(14, 10),
            AutoSize = true
        };

        var lblTip2 = new Label
        {
            Text = "• Drag & drop any .bat script directly into the window to auto-populate.\n• Switch to '📜 Script Editor' tab to edit or test batch scripts with built-in code templates.\n• Supports x64 / x86 / AnyCPU architectures with dynamic AES-256 cipher encryption & obfuscation armor.",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextSecondary,
            Location = new Point(14, 34),
            AutoSize = true
        };

        cardTips.Controls.AddRange(new Control[] { lblTip1, lblTip2 });

        pnlViewStudio.Controls.Add(cardTips);
        pnlViewStudio.Controls.Add(gap3);
        pnlViewStudio.Controls.Add(cardIco);
        pnlViewStudio.Controls.Add(gap2);
        pnlViewStudio.Controls.Add(cardExe);
        pnlViewStudio.Controls.Add(gap1);
        pnlViewStudio.Controls.Add(cardBat);

        this.AllowDrop = true;
        this.DragEnter += (s, e) =>
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        };
        this.DragDrop += (s, e) =>
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
                if (files.Length > 0)
                {
                    string f = files[0];
                    string ext = Path.GetExtension(f).ToLowerInvariant();
                    if (ext == ".bat" || ext == ".cmd")
                    {
                        LoadBatchFile(f);
                    }
                    else if (ext == ".ico")
                    {
                        txtIconPath.TextValue = f;
                        LoadIconPreview(f);
                    }
                }
            }
        };

        pnlContentContainer.Controls.Add(pnlViewStudio);
    }

    private void BuildViewEditor()
    {
        pnlViewEditor = new Panel
        {
            Dock = DockStyle.Fill,
            Visible = false,
            Padding = new Padding(20, 16, 20, 16)
        };

        var pnlEditorTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44,
            BackColor = UITheme.BgCard,
            Padding = new Padding(12, 5, 12, 5)
        };

        var lblTpl = new Label
        {
            Text = "Insert Template:",
            Location = new Point(12, 12),
            AutoSize = true,
            ForeColor = UITheme.TextSecondary,
            Font = UITheme.FontSmall
        };

        cmbTemplates = new ComboBox
        {
            Location = new Point(120, 8),
            Size = new Size(260, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextWhite,
            FlatStyle = FlatStyle.Flat,
            Font = UITheme.FontSmall
        };

        var templates = ScriptTemplateLibrary.GetTemplates();
        foreach (var t in templates)
        {
            cmbTemplates.Items.Add(t.Name);
        }
        if (cmbTemplates.Items.Count > 0) cmbTemplates.SelectedIndex = 0;

        var btnInsertTpl = new ProButton
        {
            Text = "➕ Insert",
            Location = new Point(390, 6),
            Size = new Size(80, 32),
            BgNormal = UITheme.BrandAccent,
            BgHover = UITheme.BrandAccentHover,
            Font = UITheme.FontSmall,
            Radius = 4
        };
        btnInsertTpl.Click += (s, e) =>
        {
            if (cmbTemplates.SelectedIndex >= 0 && cmbTemplates.SelectedIndex < templates.Count)
            {
                var t = templates[cmbTemplates.SelectedIndex];
                txtScriptEditor.Text = t.Code;
                txtLogs.AppendText($"[Editor] Loaded template: {t.Name}" + Environment.NewLine);
            }
        };

        var btnTestRun = new ProButton
        {
            Text = "▶ Test Run Script",
            Location = new Point(480, 6),
            Size = new Size(130, 32),
            BgNormal = UITheme.NeonGreen,
            BgHover = UITheme.NeonGreenHover,
            Font = UITheme.FontSmall,
            Radius = 4
        };
        btnTestRun.Click += (s, e) => TestRunCurrentScript();

        var btnSaveScript = new ProButton
        {
            Text = "💾 Save Script...",
            Location = new Point(620, 6),
            Size = new Size(120, 32),
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.BrandAccent,
            Font = UITheme.FontSmall,
            Radius = 4
        };
        btnSaveScript.Click += (s, e) => SaveScriptToFile();

        var btnClearEditor = new ProButton
        {
            Text = "🗑 Clear",
            Location = new Point(750, 6),
            Size = new Size(80, 32),
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.DangerRed,
            Font = UITheme.FontSmall,
            Radius = 4
        };
        btnClearEditor.Click += (s, e) => txtScriptEditor.Clear();

        pnlEditorTop.Controls.AddRange(new Control[]
        {
            lblTpl, cmbTemplates, btnInsertTpl, btnTestRun, btnSaveScript, btnClearEditor
        });

        lblEditorStats = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 26,
            BackColor = UITheme.BgSidebar,
            ForeColor = UITheme.TextSecondary,
            Font = UITheme.FontSmall,
            Padding = new Padding(12, 4, 12, 4),
            Text = "Lines: 0 | Characters: 0 | Encoding: UTF-8"
        };

        var cardEditor = new ProCard
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.BgTerminal,
            CustomBorderColor = UITheme.Border,
            Padding = new Padding(8),
            Radius = 6
        };

        txtScriptEditor = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            BackColor = UITheme.BgTerminal,
            ForeColor = Color.FromArgb(226, 232, 240),
            Font = new Font("Consolas", 10.5f, FontStyle.Regular),
            BorderStyle = BorderStyle.None,
            AcceptsTab = true
        };
        txtScriptEditor.TextChanged += (s, e) =>
        {
            int lines = txtScriptEditor.Lines.Length;
            int chars = txtScriptEditor.Text.Length;
            lblEditorStats.Text = $"Lines: {lines} | Characters: {chars} | Encoding: UTF-8 / Windows ANSI";
        };

        cardEditor.Controls.Add(txtScriptEditor);

        pnlViewEditor.Controls.Add(cardEditor);
        pnlViewEditor.Controls.Add(lblEditorStats);
        pnlViewEditor.Controls.Add(pnlEditorTop);
        pnlContentContainer.Controls.Add(pnlViewEditor);
    }

    private void TestRunCurrentScript()
    {
        string script = txtScriptEditor.Text;
        if (string.IsNullOrWhiteSpace(script))
        {
            MessageBox.Show(this, "Script is empty. Type batch code or select a template first.", "Empty Script", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            string tempBat = Path.Combine(Path.GetTempPath(), "AlienTestRun_" + Guid.NewGuid().ToString("N") + ".bat");
            File.WriteAllText(tempBat, script, Encoding.Default);

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/k \"{tempBat}\"",
                UseShellExecute = true
            });
            txtLogs.AppendText("[Test Runner] Launched script in test shell." + Environment.NewLine);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Failed to test script: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveScriptToFile()
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "Batch Files (*.bat)|*.bat|Command Scripts (*.cmd)|*.cmd",
            Title = "Save Batch Script As"
        };
        if (sfd.ShowDialog(this) == DialogResult.OK)
        {
            File.WriteAllText(sfd.FileName, txtScriptEditor.Text, Encoding.Default);
            txtBatPath.TextValue = sfd.FileName;
            txtExePath.TextValue = Path.ChangeExtension(sfd.FileName, ".exe");
            txtLogs.AppendText($"[Editor] Saved script to {sfd.FileName}" + Environment.NewLine);
        }
    }

    private void LoadBatchFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                txtBatPath.TextValue = filePath;
                txtExePath.TextValue = Path.ChangeExtension(filePath, ".exe");
                txtAppName.TextValue = Path.GetFileNameWithoutExtension(filePath);
                txtScriptEditor.Text = File.ReadAllText(filePath, Encoding.Default);
                txtLogs.AppendText($"[Alien Studio] Loaded script: {filePath}" + Environment.NewLine);
            }
        }
        catch { }
    }

    private void BuildViewOptions()
    {
        pnlViewOptions = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Visible = false,
            Padding = new Padding(20, 16, 20, 16)
        };

        var cardVis = new ProCard { Height = 95, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblVis = new Label { Text = "Window Visibility Mode:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };
        
        cmbVisibility = new ComboBox
        {
            Location = new Point(16, 42),
            Size = new Size(540, 30),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextWhite,
            FlatStyle = FlatStyle.Flat,
            Font = UITheme.FontBody
        };
        cmbVisibility.Items.AddRange(new object[]
        {
            "Visible Console (Standard Command Prompt Window)",
            "Hidden (Silent Background Execution - Zero Console Flashing)",
            "Minimized (Starts Minimized to Windows Taskbar)",
            "Maximized (Starts Fullscreen Terminal Window)"
        });
        cmbVisibility.SelectedIndex = 0;
        cardVis.Controls.AddRange(new Control[] { lblVis, cmbVisibility });

        var gap1 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardArch = new ProCard { Height = 95, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblArch = new Label { Text = "Target Architecture & Process Priority Class:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };
        
        var lblArchSub = new Label { Text = "CPU Architecture:", Location = new Point(16, 46), AutoSize = true, ForeColor = UITheme.TextSecondary };
        cmbArchitecture = new ComboBox
        {
            Location = new Point(140, 42),
            Size = new Size(180, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextWhite,
            FlatStyle = FlatStyle.Flat,
            Font = UITheme.FontBody
        };
        cmbArchitecture.Items.AddRange(new object[] { "AnyCPU (Universal)", "x64 (64-bit Native)", "x86 (32-bit Legacy)" });
        cmbArchitecture.SelectedIndex = 0;

        var lblPri = new Label { Text = "Process Priority:", Location = new Point(350, 46), AutoSize = true, ForeColor = UITheme.TextSecondary };
        cmbPriority = new ComboBox
        {
            Location = new Point(460, 42),
            Size = new Size(180, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextWhite,
            FlatStyle = FlatStyle.Flat,
            Font = UITheme.FontBody
        };
        cmbPriority.Items.AddRange(new object[] { "Normal", "Above Normal", "High Priority", "RealTime", "Below Normal", "Idle" });
        cmbPriority.SelectedIndex = 0;

        cardArch.Controls.AddRange(new Control[] { lblArch, lblArchSub, cmbArchitecture, lblPri, cmbPriority });

        var gap2 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardElev = new ProCard { Height = 105, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblElev = new Label { Text = "Privileges & Execution Security:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };
        
        chkRequireAdmin = new CheckBox
        {
            Text = "Require Administrator Privileges (Embed UAC elevation manifest shield icon)",
            Location = new Point(16, 42),
            AutoSize = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        chkSingleInstance = new CheckBox
        {
            Text = "Single Instance Only (Prevent duplicate concurrent instances using system Mutex)",
            Location = new Point(16, 70),
            AutoSize = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };
        cardElev.Controls.AddRange(new Control[] { lblElev, chkRequireAdmin, chkSingleInstance });

        var gap3 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardDir = new ProCard { Height = 125, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblDir = new Label { Text = "Working Directory & Cleanup:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };
        
        radDirCurrent = new RadioButton
        {
            Text = "Current Directory (Execute in directory where .exe is launched)",
            Location = new Point(16, 40),
            AutoSize = true,
            Checked = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        radDirTemp = new RadioButton
        {
            Text = "Temporary Directory (Execute in isolated temporary extracted folder)",
            Location = new Point(16, 66),
            AutoSize = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        chkDeleteOnExit = new CheckBox
        {
            Text = "Clean and delete temporary extracted files upon exit",
            Location = new Point(16, 92),
            AutoSize = true,
            Checked = true,
            ForeColor = UITheme.TextSecondary,
            Font = UITheme.FontSmall
        };
        cardDir.Controls.AddRange(new Control[] { lblDir, radDirCurrent, radDirTemp, chkDeleteOnExit });

        var gap4 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardProParams = new ProCard { Height = 160, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblParamsHead = new Label { Text = "💎 Pro Execution Parameters & Custom Console Settings:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.BrandAccent };
        
        var lblArgs = new Label { Text = "Default Arguments:", Location = new Point(16, 42), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtDefaultArgs = new ModernInputBox { Location = new Point(150, 36), Size = new Size(300, 36) };
        
        var lblTimeout = new Label { Text = "Timeout (Sec, 0=None):", Location = new Point(470, 42), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtExecutionTimeout = new ModernInputBox { Location = new Point(620, 36), Size = new Size(80, 36) };
        txtExecutionTimeout.TextValue = "0";

        var lblConTitle = new Label { Text = "Custom Console Title:", Location = new Point(16, 84), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtConsoleTitle = new ModernInputBox { Location = new Point(150, 78), Size = new Size(300, 36) };

        chkLogErrors = new CheckBox
        {
            Text = "Auto-log process exit crashes to error.log",
            Location = new Point(470, 84),
            AutoSize = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontSmall
        };

        cardProParams.Controls.AddRange(new Control[] { lblParamsHead, lblArgs, txtDefaultArgs, lblTimeout, txtExecutionTimeout, lblConTitle, txtConsoleTitle, chkLogErrors });

        pnlViewOptions.Controls.AddRange(new Control[] { cardProParams, gap4, cardDir, gap3, cardElev, gap2, cardArch, gap1, cardVis });
        pnlContentContainer.Controls.Add(pnlViewOptions);
    }

    private void BuildViewSecurity()
    {
        pnlViewSecurity = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Visible = false,
            Padding = new Padding(20, 16, 20, 16)
        };

        var cardEnc = new ProCard { Height = 100, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblEncHead = new Label { Text = "Dynamic AES-256 Payload Encryption & Obfuscation Armor:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };
        
        chkEncrypt = new CheckBox
        {
            Text = "AES-256 Payload Cipher (Encrypts batch buffer with randomized symmetric keys)",
            Location = new Point(16, 42),
            AutoSize = true,
            Checked = true,
            ForeColor = UITheme.NeonGreen,
            Font = UITheme.FontBold
        };

        var lblObf = new Label { Text = "Obfuscation Armor:", Location = new Point(16, 70), AutoSize = true, ForeColor = UITheme.TextSecondary };
        cmbObfuscation = new ComboBox
        {
            Location = new Point(150, 66),
            Size = new Size(320, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextWhite,
            FlatStyle = FlatStyle.Flat,
            Font = UITheme.FontSmall
        };
        cmbObfuscation.Items.AddRange(new object[]
        {
            "None (Clean Script)",
            "Level 1: Basic (Strip comments & whitespace)",
            "Level 2: Advanced (Variable fragmentation & slicing)",
            "Level 3: Ultra Armor (Dynamic Base64 tokenized execution)",
            "Level 4: Quantum Armor (Dynamic XOR Polymorphic Tokenization)"
        });
        cmbObfuscation.SelectedIndex = 0;

        cardEnc.Controls.AddRange(new Control[] { lblEncHead, chkEncrypt, lblObf, cmbObfuscation });

        var gap1 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardPwd = new ProCard { Height = 145, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblPwdHead = new Label { Text = "Executable Password Protection: [Pro Lifetime Feature]", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };
        var lblPwdDesc = new Label { Text = "Set a secret password required to execute the compiled application (leave blank for none):", Location = new Point(16, 38), AutoSize = true, Font = UITheme.FontSmall, ForeColor = UITheme.TextSecondary };

        txtPassword = new ModernInputBox { Location = new Point(16, 64), Size = new Size(380, 38) };
        txtPassword.SetPasswordChar(true);

        chkShowPassword = new CheckBox
        {
            Text = "Show Password",
            Location = new Point(410, 72),
            AutoSize = true,
            ForeColor = UITheme.TextSecondary,
            Font = UITheme.FontSmall
        };
        chkShowPassword.CheckedChanged += (s, e) => txtPassword.SetPasswordChar(!chkShowPassword.Checked);

        var lblNote = new Label
        {
            Text = "When set, the generated executable prompts users for this password before unpacking and running the script.",
            Location = new Point(16, 110),
            AutoSize = true,
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextMuted
        };

        cardPwd.Controls.AddRange(new Control[] { lblPwdHead, lblPwdDesc, txtPassword, chkShowPassword, lblNote });

        var gap2 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        var cardDefense = new ProCard { Height = 220, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblDefenseHead = new Label { Text = "🛡️ Advanced Anti-Tamper & Stealth Defense [Pro Features]:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.BrandAccent };
        
        chkAntiAnalysis = new CheckBox
        {
            Text = "Anti-Analysis & Debugger Guard (Detects x64dbg, IDA, Wireshark, ProcessHacker & terminates)",
            Location = new Point(16, 42),
            AutoSize = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        chkAutoStartup = new CheckBox
        {
            Text = "Register Persistent Windows Startup (Adds executable automatically to HKCU Run key)",
            Location = new Point(16, 70),
            AutoSize = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        chkIntegrity = new CheckBox
        {
            Text = "SHA-256 Binary Integrity Self-Check (Prevents binary modification & tampering)",
            Location = new Point(16, 98),
            AutoSize = true,
            Checked = true,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontBody
        };

        var lblSplash = new Label { Text = "Decoy Splash Message / Notice:", Location = new Point(16, 132), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtSplashMsg = new ModernInputBox { Location = new Point(220, 126), Size = new Size(420, 36) };

        var lblFakeErr = new Label { Text = "Fake Error Dialog (Decoy):", Location = new Point(16, 174), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtFakeError = new ModernInputBox { Location = new Point(220, 168), Size = new Size(420, 36) };

        cardDefense.Controls.AddRange(new Control[] { lblDefenseHead, chkAntiAnalysis, chkAutoStartup, chkIntegrity, lblSplash, txtSplashMsg, lblFakeErr, txtFakeError });

        pnlViewSecurity.Controls.AddRange(new Control[] { cardDefense, gap2, cardPwd, gap1, cardEnc });
        pnlContentContainer.Controls.Add(pnlViewSecurity);
    }

    private void BuildViewVersion()
    {
        pnlViewVersion = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Visible = false,
            Padding = new Padding(20, 16, 20, 16)
        };

        var cardMeta = new ProCard { Height = 440, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblHead = new Label { Text = "Binary Metadata & Assembly Information:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };

        int y = 46;
        int rowHeight = 46;

        var lbl1 = new Label { Text = "Application Name:", Location = new Point(16, y + 8), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtAppName = new ModernInputBox { Location = new Point(160, y), Size = new Size(480, 36) };
        txtAppName.TextValue = "My Batch App";

        y += rowHeight;
        var lbl2 = new Label { Text = "File Description:", Location = new Point(16, y + 8), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtDescription = new ModernInputBox { Location = new Point(160, y), Size = new Size(480, 36) };
        txtDescription.TextValue = "Compiled Batch Application";

        y += rowHeight;
        var lbl3 = new Label { Text = "Company Name:", Location = new Point(16, y + 8), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtCompany = new ModernInputBox { Location = new Point(160, y), Size = new Size(480, 36) };
        txtCompany.TextValue = "Alien Software Development";

        y += rowHeight;
        var lbl4 = new Label { Text = "Product Name:", Location = new Point(16, y + 8), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtProduct = new ModernInputBox { Location = new Point(160, y), Size = new Size(480, 36) };
        txtProduct.TextValue = "Alien Batch Suite";

        y += rowHeight;
        var lbl5 = new Label { Text = "Legal Copyright:", Location = new Point(16, y + 8), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtCopyright = new ModernInputBox { Location = new Point(160, y), Size = new Size(480, 36) };
        txtCopyright.TextValue = $"Copyright © {DateTime.Now.Year} Alien Software Development";

        y += rowHeight;
        var lbl6 = new Label { Text = "File Version:", Location = new Point(16, y + 8), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtFileVersion = new ModernInputBox { Location = new Point(160, y), Size = new Size(220, 36) };
        txtFileVersion.TextValue = "1.0.0.0";

        y += rowHeight;
        var lbl7 = new Label { Text = "Product Version:", Location = new Point(16, y + 8), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtProductVersion = new ModernInputBox { Location = new Point(160, y), Size = new Size(220, 36) };
        txtProductVersion.TextValue = "1.0.0.0";

        y += rowHeight;
        var lbl8 = new Label { Text = "Build Comments:", Location = new Point(16, y + 8), AutoSize = true, ForeColor = UITheme.TextSecondary };
        txtComments = new ModernInputBox { Location = new Point(160, y), Size = new Size(480, 36) };
        txtComments.TextValue = "Built with Alien BAT to EXE Studio Pro";

        cardMeta.Controls.AddRange(new Control[]
        {
            lblHead,
            lbl1, txtAppName,
            lbl2, txtDescription,
            lbl3, txtCompany,
            lbl4, txtProduct,
            lbl5, txtCopyright,
            lbl6, txtFileVersion,
            lbl7, txtProductVersion,
            lbl8, txtComments
        });

        pnlViewVersion.Controls.Add(cardMeta);
        pnlContentContainer.Controls.Add(pnlViewVersion);
    }

    private void BuildViewAssets()
    {
        pnlViewAssets = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Visible = false,
            Padding = new Padding(20, 16, 20, 16)
        };

        var cardAssets = new ProCard { Height = 340, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblAssetHead = new Label { Text = "Embedded Auxiliary Files & Dependencies:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };
        var lblAssetSub = new Label { Text = "Bundle PowerShell scripts (.ps1), DLLs, configs, and icons directly inside the executable:", Location = new Point(16, 36), AutoSize = true, Font = UITheme.FontSmall, ForeColor = UITheme.TextSecondary };

        listAssets = new ListBox
        {
            Location = new Point(16, 64),
            Size = new Size(cardAssets.Width - 210, 250),
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextWhite,
            BorderStyle = BorderStyle.FixedSingle,
            Font = UITheme.FontCode,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var btnAdd = new ProButton
        {
            Text = "➕ Add Files...",
            Location = new Point(cardAssets.Width - 180, 64),
            Size = new Size(160, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BgNormal = UITheme.BrandAccent,
            BgHover = UITheme.BrandAccentHover,
            Radius = 6
        };
        btnAdd.Click += (s, e) =>
        {
            using var ofd = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select Extra Files to Embed"
            };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    if (!listAssets.Items.Contains(file))
                        listAssets.Items.Add(file);
                }
            }
        };

        var btnRemove = new ProButton
        {
            Text = "➖ Remove",
            Location = new Point(cardAssets.Width - 180, 110),
            Size = new Size(160, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.DangerRed,
            Radius = 6
        };
        btnRemove.Click += (s, e) =>
        {
            if (listAssets.SelectedIndex >= 0)
                listAssets.Items.RemoveAt(listAssets.SelectedIndex);
        };

        var btnClear = new ProButton
        {
            Text = "🗑 Clear All",
            Location = new Point(cardAssets.Width - 180, 156),
            Size = new Size(160, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.DangerRed,
            Radius = 6
        };
        btnClear.Click += (s, e) => listAssets.Items.Clear();

        cardAssets.Controls.AddRange(new Control[] { lblAssetHead, lblAssetSub, listAssets, btnAdd, btnRemove, btnClear });
        pnlViewAssets.Controls.Add(cardAssets);
        pnlContentContainer.Controls.Add(pnlViewAssets);
    }

    private void BuildViewTools()
    {
        pnlViewTools = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Visible = false,
            Padding = new Padding(20, 16, 20, 16)
        };

        // Context Menu Shell Card
        var cardContext = new ProCard { Height = 140, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblContextHead = new Label { Text = "🗂 Windows Explorer Shell Integration:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.TextWhite };
        var lblContextDesc = new Label { Text = "Add a convenient right-click context menu to all .bat and .cmd files for instant 1-click conversion:", Location = new Point(16, 38), AutoSize = true, Font = UITheme.FontSmall, ForeColor = UITheme.TextSecondary };

        lblContextStatus = new Label
        {
            Text = ShellContextMenu.IsRegistered() ? "Status: [INSTALLED] Context menu active." : "Status: [NOT INSTALLED] Context menu not registered.",
            Location = new Point(16, 68),
            AutoSize = true,
            Font = UITheme.FontBold,
            ForeColor = ShellContextMenu.IsRegistered() ? UITheme.NeonGreen : UITheme.WarningAmber
        };

        btnToggleContextMenu = new ProButton
        {
            Text = ShellContextMenu.IsRegistered() ? "🗑 Remove Context Menu" : "➕ Register Context Menu",
            Location = new Point(16, 92),
            Size = new Size(220, 36),
            BgNormal = ShellContextMenu.IsRegistered() ? UITheme.BgCardHover : UITheme.BrandAccent,
            BgHover = ShellContextMenu.IsRegistered() ? UITheme.DangerRed : UITheme.BrandAccentHover,
            Radius = 6
        };
        btnToggleContextMenu.Click += (s, e) =>
        {
            if (ShellContextMenu.IsRegistered())
            {
                ShellContextMenu.Unregister();
            }
            else
            {
                ShellContextMenu.Register();
            }
            RefreshContextStatus();
        };

        cardContext.Controls.AddRange(new Control[] { lblContextHead, lblContextDesc, lblContextStatus, btnToggleContextMenu });

        var gap1 = new Panel { Height = 14, Dock = DockStyle.Top, BackColor = Color.Transparent };

        // Quick Tools Card
        var cardQuick = new ProCard { Height = 160, Dock = DockStyle.Top, BackColor = UITheme.BgCard };
        var lblQuickHead = new Label { Text = "⚡ Pro Developer Utilities:", Location = new Point(16, 12), AutoSize = true, Font = UITheme.FontH3, ForeColor = UITheme.BrandAccent };

        var btnOpenKeygen = new ProButton
        {
            Text = "🔑 Open Alien Keygen...",
            Location = new Point(16, 46),
            Size = new Size(200, 38),
            BgNormal = UITheme.Purple,
            BgHover = Color.FromArgb(147, 51, 234),
            Radius = 6
        };
        btnOpenKeygen.Click += (s, e) =>
        {
            using var kg = new KeygenForm();
            if (kg.ShowDialog(this) == DialogResult.OK)
            {
                RefreshLicenseUI();
            }
        };

        var btnPresetBrowser = new ProButton
        {
            Text = "🎨 Pro Preset Icons...",
            Location = new Point(230, 46),
            Size = new Size(200, 38),
            BgNormal = UITheme.BrandAccent,
            BgHover = UITheme.BrandAccentHover,
            Radius = 6
        };
        btnPresetBrowser.Click += (s, e) =>
        {
            using var picker = new PresetIconPicker();
            if (picker.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(picker.SelectedIconPath))
            {
                txtIconPath.TextValue = picker.SelectedIconPath;
                LoadIconPreview(picker.SelectedIconPath);
                SwitchView(pnlViewStudio, btnNavStudio, "👽 Converter Studio", "Convert batch scripts into protected, standalone Native PE executables");
            }
        };

        var btnResetLic = new ProButton
        {
            Text = "🔄 Reset License Trial",
            Location = new Point(444, 46),
            Size = new Size(180, 38),
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.WarningAmber,
            Radius = 6
        };
        btnResetLic.Click += (s, e) =>
        {
            if (MessageBox.Show(this, "Are you sure you want to reset license activation and local evaluation storage?", "Confirm Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LicenseManager.ResetAllLicenseData();
                RefreshLicenseUI();
                MessageBox.Show(this, "License evaluation data reset successfully.", "Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };

        cardQuick.Controls.AddRange(new Control[] { lblQuickHead, btnOpenKeygen, btnPresetBrowser, btnResetLic });

        pnlViewTools.Controls.AddRange(new Control[] { cardQuick, gap1, cardContext });
        pnlContentContainer.Controls.Add(pnlViewTools);
    }

    private void RefreshContextStatus()
    {
        bool isReg = ShellContextMenu.IsRegistered();
        lblContextStatus.Text = isReg ? "Status: [INSTALLED] Context menu active." : "Status: [NOT INSTALLED] Context menu not registered.";
        lblContextStatus.ForeColor = isReg ? UITheme.NeonGreen : UITheme.WarningAmber;
        btnToggleContextMenu.Text = isReg ? "🗑 Remove Context Menu" : "➕ Register Context Menu";
        btnToggleContextMenu.BgNormal = isReg ? UITheme.BgCardHover : UITheme.BrandAccent;
        btnToggleContextMenu.BgHover = isReg ? UITheme.DangerRed : UITheme.BrandAccentHover;
    }

    private void SwitchView(Panel targetView, SidebarNavButton activeNav, string title, string subtitle)
    {
        pnlViewStudio.Visible = (targetView == pnlViewStudio);
        pnlViewEditor.Visible = (targetView == pnlViewEditor);
        pnlViewOptions.Visible = (targetView == pnlViewOptions);
        pnlViewSecurity.Visible = (targetView == pnlViewSecurity);
        pnlViewVersion.Visible = (targetView == pnlViewVersion);
        pnlViewAssets.Visible = (targetView == pnlViewAssets);
        pnlViewTools.Visible = (targetView == pnlViewTools);

        lblHeaderTitle.Text = title;
        lblHeaderSubtitle.Text = subtitle;

        SidebarNavButton[] allNavs = { btnNavStudio, btnNavEditor, btnNavOptions, btnNavSecurity, btnNavVersion, btnNavAssets, btnNavTools, btnNavLicense };
        foreach (var nav in allNavs)
        {
            nav.IsSelected = (nav == activeNav);
        }
    }

    private void RefreshLicenseUI()
    {
        var status = LicenseManager.CheckLicense();
        bool isPro = (status.Type == LicenseType.PaidLifetime);

        if (isPro)
        {
            btnLicensePill.Text = "✔ PRO LIFETIME";
            btnLicensePill.BgNormal = UITheme.NeonGreen;
            btnLicensePill.BgHover = UITheme.NeonGreenHover;

            txtCompany.SetReadOnly(false);
            txtCopyright.SetReadOnly(false);
            txtPassword.SetReadOnly(false);
            chkAntiAnalysis.Enabled = true;
            chkAutoStartup.Enabled = true;
            txtFakeError.SetReadOnly(false);
            txtSplashMsg.SetReadOnly(false);
            txtDefaultArgs.SetReadOnly(false);
            txtExecutionTimeout.SetReadOnly(false);
            txtComments.SetReadOnly(false);
        }
        else
        {
            if (status.Type == LicenseType.Trial)
            {
                btnLicensePill.Text = $"⏱ TRIAL ({status.DaysRemaining}d • {status.DailyConversionsRemaining}/{LicenseStatus.MaxDailyTrialConversions} Left)";
                btnLicensePill.BgNormal = status.DailyConversionsRemaining > 0 ? UITheme.WarningAmber : UITheme.DangerRed;
                btnLicensePill.BgHover = Color.FromArgb(217, 119, 6);
            }
            else
            {
                btnLicensePill.Text = "✖ TRIAL EXPIRED";
                btnLicensePill.BgNormal = UITheme.DangerRed;
                btnLicensePill.BgHover = Color.FromArgb(220, 38, 38);
            }

            // In Trial: Lock Company Name and Legal Copyright
            txtCompany.TextValue = "Alien Software Development";
            txtCompany.SetReadOnly(true);

            txtCopyright.TextValue = $"Copyright © {DateTime.Now.Year} Alien Software Development";
            txtCopyright.SetReadOnly(true);

            txtPassword.TextValue = string.Empty;
            txtPassword.SetReadOnly(true);

            txtComments.TextValue = "Built with Alien BAT to EXE Studio (Trial Evaluation)";
            txtComments.SetReadOnly(true);
        }
        btnLicensePill.Invalidate();
    }

    private void OpenLicenseDialog()
    {
        using var dlg = new LicenseDialog();
        dlg.ShowDialog(this);
        RefreshLicenseUI();
    }

    private void OpenBatFileDialog()
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "Batch Files (*.bat;*.cmd)|*.bat;*.cmd|All Files (*.*)|*.*",
            Title = "Select Source Batch Script (.bat / .cmd)"
        };
        if (ofd.ShowDialog(this) == DialogResult.OK)
        {
            LoadBatchFile(ofd.FileName);
        }
    }

    private void OpenExeSaveDialog()
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "Executable Files (*.exe)|*.exe",
            Title = "Select Output Executable Location (.exe)"
        };
        if (sfd.ShowDialog(this) == DialogResult.OK)
        {
            txtExePath.TextValue = sfd.FileName;
        }
    }

    private void OpenIconFileDialog()
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "Icon Files (*.ico)|*.ico",
            Title = "Select Application Icon (.ico)"
        };
        if (ofd.ShowDialog(this) == DialogResult.OK)
        {
            txtIconPath.TextValue = ofd.FileName;
            LoadIconPreview(ofd.FileName);
        }
    }

    private void LoadIconPreview(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                using var stream = File.OpenRead(path);
                using var icon = new Icon(stream, 64, 64);
                picIcon.Image = icon.ToBitmap();
            }
            else
            {
                picIcon.Image = null;
            }
        }
        catch
        {
            picIcon.Image = null;
        }
    }

    private async void BtnCompile_Click(object? sender, EventArgs e)
    {
        var licenseStatus = LicenseManager.CheckLicense();
        if (!licenseStatus.IsActive)
        {
            var res = MessageBox.Show(
                this,
                "Your 7-day free trial has expired!\n\nWould you like to activate a Paid Lifetime License or contact Alien Software Development?",
                "License Activation Required",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );
            if (res == DialogResult.Yes)
            {
                OpenLicenseDialog();
            }
            return;
        }

        if (!LicenseManager.CanConvertToday(out int remainingToday, out int usedToday))
        {
            var res = MessageBox.Show(
                this,
                $"Daily Free Trial Limit Reached ({usedToday} of {LicenseStatus.MaxDailyTrialConversions} conversions used today)!\n\n" +
                "Free Trial grants up to 3 .exe creations per day for 7 days.\n\n" +
                "To create unlimited executables without daily limits, please activate a Paid Lifetime License.\n\n" +
                "Would you like to enter a Paid License Key now?",
                "Daily Free Trial Limit Reached",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );

            if (res == DialogResult.Yes)
            {
                OpenLicenseDialog();
            }
            return;
        }

        bool isPro = (licenseStatus.Type == LicenseType.PaidLifetime);

        if (!isPro)
        {
            txtCompany.TextValue = "Alien Software Development";
            txtCopyright.TextValue = $"Copyright © {DateTime.Now.Year} Alien Software Development";
            txtPassword.TextValue = string.Empty;
        }

        string bat = txtBatPath.TextValue.Trim();
        string exe = txtExePath.TextValue.Trim();
        string inlineCode = txtScriptEditor.Text;

        if ((string.IsNullOrWhiteSpace(bat) || !File.Exists(bat)) && string.IsNullOrWhiteSpace(inlineCode))
        {
            MessageBox.Show(this, "Please select a valid Batch (.bat / .cmd) script or enter code in the Script Editor.", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SwitchView(pnlViewStudio, btnNavStudio, "👽 Converter Studio", "Convert batch scripts into protected, standalone Native PE executables");
            return;
        }

        if (string.IsNullOrWhiteSpace(exe))
        {
            if (!string.IsNullOrEmpty(bat))
            {
                exe = Path.ChangeExtension(bat, ".exe");
                txtExePath.TextValue = exe;
            }
            else
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "Executable Files (*.exe)|*.exe",
                    Title = "Select Output Executable Location (.exe)"
                };
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    exe = sfd.FileName;
                    txtExePath.TextValue = exe;
                }
                else
                {
                    return;
                }
            }
        }

        btnCompile.Enabled = false;
        btnOpenFolder.Enabled = false;
        btnRunExe.Enabled = false;
        lblStatusDot.ForeColor = UITheme.BrandAccent;
        lblStatusText.Text = "Compiling standalone executable...";
        txtLogs.Clear();

        int timeoutSec = 0;
        int.TryParse(txtExecutionTimeout.TextValue.Trim(), out timeoutSec);

        var options = new CompilerOptions
        {
            BatFilePath = bat,
            InlineScriptContent = !string.IsNullOrWhiteSpace(inlineCode) ? inlineCode : null,
            OutputExePath = exe,
            IconPath = string.IsNullOrWhiteSpace(txtIconPath.TextValue) ? null : txtIconPath.TextValue.Trim(),
            RequireAdministrator = chkRequireAdmin.Checked,
            SingleInstanceOnly = chkSingleInstance.Checked,
            WorkingDirectory = radDirCurrent.Checked ? WorkingDirectoryMode.CurrentDirectory : WorkingDirectoryMode.TempDirectory,
            DeleteTempFilesOnExit = chkDeleteOnExit.Checked,
            EncryptScript = chkEncrypt.Checked,
            Obfuscation = (ObfuscationLevel)cmbObfuscation.SelectedIndex,
            Architecture = (TargetArchitecture)cmbArchitecture.SelectedIndex,
            Priority = (ProcessPriorityMode)cmbPriority.SelectedIndex,
            CustomConsoleTitle = string.IsNullOrWhiteSpace(txtConsoleTitle.TextValue) ? null : txtConsoleTitle.TextValue.Trim(),
            LogErrorsToFile = chkLogErrors.Checked,
            Password = isPro && !string.IsNullOrWhiteSpace(txtPassword.TextValue) ? txtPassword.TextValue : null,
            Visibility = (VisibilityMode)cmbVisibility.SelectedIndex,
            AntiAnalysis = isPro && chkAntiAnalysis.Checked,
            AutoRunOnStartup = isPro && chkAutoStartup.Checked,
            EnableIntegrityCheck = chkIntegrity.Checked,
            ExecutionTimeoutSeconds = isPro ? Math.Max(0, timeoutSec) : 0,
            SplashMessage = isPro && !string.IsNullOrWhiteSpace(txtSplashMsg.TextValue) ? txtSplashMsg.TextValue.Trim() : null,
            FakeErrorMessage = isPro && !string.IsNullOrWhiteSpace(txtFakeError.TextValue) ? txtFakeError.TextValue.Trim() : null,
            DefaultCommandLineArgs = isPro && !string.IsNullOrWhiteSpace(txtDefaultArgs.TextValue) ? txtDefaultArgs.TextValue.Trim() : null,
            VersionInfo = new VersionInformation
            {
                Title = string.IsNullOrWhiteSpace(txtAppName.TextValue) ? "Batch Executable" : txtAppName.TextValue,
                Description = txtDescription.TextValue,
                Company = isPro ? txtCompany.TextValue : "Alien Software Development",
                Product = txtProduct.TextValue,
                Copyright = isPro ? txtCopyright.TextValue : $"Copyright © {DateTime.Now.Year} Alien Software Development",
                FileVersion = txtFileVersion.TextValue,
                ProductVersion = txtProductVersion.TextValue,
                Comments = isPro ? txtComments.TextValue : "Built with Alien BAT to EXE Studio (Trial Evaluation)"
            }
        };

        foreach (var item in listAssets.Items)
        {
            if (item is string fp && File.Exists(fp))
            {
                options.AdditionalFiles.Add(fp);
            }
        }

        var result = await BatCompilerEngine.CompileAsync(options, line =>
        {
            this.Invoke(() =>
            {
                txtLogs.AppendText(line + Environment.NewLine);
            });
        });

        btnCompile.Enabled = true;

        if (result.Success)
        {
            LicenseManager.RecordConversion();
            RefreshLicenseUI();

            lastGeneratedExe = result.OutputPath;
            lblStatusDot.ForeColor = UITheme.NeonGreen;
            lblStatusText.Text = "Executable Created Successfully!";
            btnOpenFolder.Enabled = true;
            btnRunExe.Enabled = true;

            string limitNote = licenseStatus.Type == LicenseType.PaidLifetime
                ? "License: Paid Lifetime (Unlimited Conversions)"
                : $"Free Trial: {Math.Max(0, remainingToday - 1)} of {LicenseStatus.MaxDailyTrialConversions} conversions remaining today.";

            MessageBox.Show(
                this,
                $"Binary successfully compiled to:\n{result.OutputPath}\n\nArchitecture: {options.Architecture}\nSecurity: AES-256 Encrypted & Standalone Native PE\n{limitNote}",
                "Alien Compilation Succeeded",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        else
        {
            lblStatusDot.ForeColor = UITheme.DangerRed;
            lblStatusText.Text = "Compilation Failed!";
            MessageBox.Show(this, $"Compilation failed:\n{result.ErrorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch { }
    }
}
