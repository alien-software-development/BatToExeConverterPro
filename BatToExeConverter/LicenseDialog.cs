using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BatToExeConverter;

public class LicenseDialog : Form
{
    private Label lblStatus = null!;
    private TextBox txtMachineId = null!;
    private TextBox txtLicensee = null!;
    private TextBox txtLicenseKey = null!;
    private ModernButton btnActivate = null!;
    private ModernButton btnDeactivate = null!;
    private Label lblTrialInfo = null!;

    public LicenseDialog()
    {
        InitializeDialog();
        RefreshStatus();
    }

    private void InitializeDialog()
    {
        this.Text = "Alien Software — License & Activation";
        this.Size = new Size(580, 560);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = UITheme.BgDark;
        this.Font = UITheme.FontRegular;

        try
        {
            string ico = Path.Combine(AppContext.BaseDirectory, "Assets", "app_icon.ico");
            if (File.Exists(ico)) this.Icon = new Icon(ico);
        }
        catch { }

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = UITheme.BgSidebar,
            Padding = new Padding(20, 12, 20, 10)
        };

        var lblTitle = new Label
        {
            Text = "👽 PRODUCT ACTIVATION & SECURITY",
            Font = UITheme.FontHeading,
            ForeColor = UITheme.PrimaryAccent,
            Location = new Point(20, 12),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = "BAT to EXE Converter Pro — Official License Center",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextSecondary,
            Location = new Point(22, 36),
            AutoSize = true
        };

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblSub);
        this.Controls.Add(pnlHeader);

        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };

        var pnlStatus = new RoundedPanel
        {
            Location = new Point(20, 12),
            Size = new Size(524, 75),
            BackColor = UITheme.BgCard,
            BorderColor = UITheme.BorderColor
        };

        lblStatus = new Label
        {
            Text = "LICENSE STATUS: CHECKING...",
            Font = UITheme.FontBold,
            ForeColor = UITheme.TextPrimary,
            Location = new Point(14, 12),
            AutoSize = true
        };

        lblTrialInfo = new Label
        {
            Text = "Free trial grants 7 days of full feature access (Max 3/day).",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextSecondary,
            Location = new Point(14, 38),
            AutoSize = true
        };

        pnlStatus.Controls.Add(lblStatus);
        pnlStatus.Controls.Add(lblTrialInfo);
        pnlContent.Controls.Add(pnlStatus);

        var lblMid = new Label { Text = "Hardware Machine ID (Provide this to Alien Support for activation):", ForeColor = UITheme.TextSecondary, Location = new Point(20, 98), AutoSize = true, Font = UITheme.FontSmall };
        txtMachineId = new TextBox
        {
            Location = new Point(20, 120),
            Size = new Size(405, 26),
            ReadOnly = true,
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.PrimaryAccent,
            Font = UITheme.FontCode
        };
        var btnCopyMid = new ModernButton
        {
            Text = "📋 Copy ID",
            Location = new Point(435, 118),
            Size = new Size(109, 30),
            NormalColor = UITheme.BgCardHover,
            HoverColor = UITheme.PrimaryAccent,
            Font = UITheme.FontSmall
        };
        btnCopyMid.Click += (s, e) =>
        {
            Clipboard.SetText(txtMachineId.Text);
            MessageBox.Show(this, "Hardware Machine ID copied to clipboard!", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        pnlContent.Controls.AddRange(new Control[] { lblMid, txtMachineId, btnCopyMid });

        var lblName = new Label { Text = "Registered Name / Customer Name:", ForeColor = UITheme.TextSecondary, Location = new Point(20, 158), AutoSize = true, Font = UITheme.FontSmall };
        txtLicensee = new TextBox
        {
            Location = new Point(20, 180),
            Size = new Size(524, 26),
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontRegular
        };
        pnlContent.Controls.AddRange(new Control[] { lblName, txtLicensee });

        var lblKey = new Label { Text = "Paid Lifetime License Key (ALIEN-XXXXX-XXXXX-XXXXX-XXXXX):", ForeColor = UITheme.TextSecondary, Location = new Point(20, 218), AutoSize = true, Font = UITheme.FontSmall };
        txtLicenseKey = new TextBox
        {
            Location = new Point(20, 240),
            Size = new Size(524, 26),
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.SuccessColor,
            Font = UITheme.FontCode
        };
        pnlContent.Controls.AddRange(new Control[] { lblKey, txtLicenseKey });

        btnActivate = new ModernButton
        {
            Text = "⚡ Activate Paid Lifetime License",
            Location = new Point(20, 280),
            Size = new Size(524, 38),
            NormalColor = UITheme.SuccessColor,
            HoverColor = Color.FromArgb(5, 150, 105),
            Font = UITheme.FontBold
        };
        btnActivate.Click += BtnActivate_Click;

        btnDeactivate = new ModernButton
        {
            Text = "🗑 Remove / Deactivate License Key",
            Location = new Point(20, 324),
            Size = new Size(524, 34),
            NormalColor = UITheme.BgCardHover,
            HoverColor = UITheme.DangerColor,
            Font = UITheme.FontSmall,
            Visible = false
        };
        btnDeactivate.Click += BtnDeactivate_Click;

        pnlContent.Controls.Add(btnActivate);
        pnlContent.Controls.Add(btnDeactivate);

        var pnlSupport = new RoundedPanel
        {
            Location = new Point(20, 368),
            Size = new Size(524, 85),
            BackColor = UITheme.BgCard,
            BorderColor = UITheme.BorderColor
        };

        var lblDev = new Label
        {
            Text = "🏢 Alien Software Development | Official Licensing Support",
            Font = UITheme.FontBold,
            ForeColor = UITheme.TextPrimary,
            Location = new Point(14, 10),
            AutoSize = true
        };

        var lnkWhatsApp = new LinkLabel
        {
            Text = "💬 WhatsApp Support: +8801710978997",
            LinkColor = UITheme.SuccessColor,
            ActiveLinkColor = UITheme.PrimaryAccent,
            Font = UITheme.FontSmall,
            Location = new Point(14, 34),
            AutoSize = true
        };
        lnkWhatsApp.Click += (s, e) => OpenUrl("https://wa.me/8801710978997");

        var lnkWeb = new LinkLabel
        {
            Text = "🌐 Official Website: aliensoftwaredevelopment.com",
            LinkColor = UITheme.PrimaryAccent,
            ActiveLinkColor = UITheme.SuccessColor,
            Font = UITheme.FontSmall,
            Location = new Point(14, 56),
            AutoSize = true
        };
        lnkWeb.Click += (s, e) => OpenUrl("https://aliensoftwaredevelopment.com");

        pnlSupport.Controls.Add(lblDev);
        pnlSupport.Controls.Add(lnkWhatsApp);
        pnlSupport.Controls.Add(lnkWeb);
        pnlContent.Controls.Add(pnlSupport);

        this.Controls.Add(pnlContent);
    }

    private void RefreshStatus()
    {
        var status = LicenseManager.CheckLicense();
        txtMachineId.Text = status.MachineId;

        if (status.Type == LicenseType.PaidLifetime)
        {
            lblStatus.Text = $"✔ ACTIVATED — {status.StatusText}";
            lblStatus.ForeColor = UITheme.SuccessColor;
            lblTrialInfo.Text = $"Licensed to: {status.RegisteredName} (Unlimited Lifetime Access)";
            lblTrialInfo.ForeColor = UITheme.TextSecondary;
            txtLicensee.Text = status.RegisteredName;
            txtLicenseKey.Text = "**********************************";
            txtLicenseKey.Enabled = false;
            txtLicensee.Enabled = false;
            btnActivate.Text = "✔ Lifetime License Active";
            btnActivate.Enabled = false;
            btnDeactivate.Visible = true;
        }
        else
        {
            btnDeactivate.Visible = false;
            txtLicenseKey.Enabled = true;
            txtLicensee.Enabled = true;
            txtLicenseKey.Text = string.Empty;
            txtLicensee.Text = string.Empty;
            btnActivate.Text = "⚡ Activate Paid Lifetime License";
            btnActivate.Enabled = true;

            if (status.Type == LicenseType.Trial)
            {
                lblStatus.Text = $"⏱ {status.StatusText}";
                lblStatus.ForeColor = UITheme.WarningColor;
                lblTrialInfo.Text = $"7-Day Free Trial (Max 3/day: {status.DailyConversionsRemaining} conversions left today). Expires {status.ExpiryDate:yyyy-MM-dd}.";
                lblTrialInfo.ForeColor = UITheme.TextSecondary;
            }
            else
            {
                lblStatus.Text = $"✖ {status.StatusText}";
                lblStatus.ForeColor = UITheme.DangerRed;
                lblTrialInfo.Text = "Your 7-day free trial has expired. Please enter an official paid license key.";
                lblTrialInfo.ForeColor = UITheme.DangerColor;
            }
        }
    }

    private void BtnActivate_Click(object? sender, EventArgs e)
    {
        string name = txtLicensee.Text.Trim();
        string key = txtLicenseKey.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            MessageBox.Show(this, "Please enter your Registered / Customer Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrEmpty(key))
        {
            MessageBox.Show(this, "Please enter your License Key.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (LicenseManager.ActivatePaidLicense(key, name))
        {
            MessageBox.Show(this, "Congratulations! Your Paid Lifetime License has been successfully activated.", "Activation Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshStatus();
            this.DialogResult = DialogResult.OK;
        }
        else
        {
            MessageBox.Show(this, "Invalid License Key or Name for this Machine ID.\n\nPlease contact Alien Software Development on WhatsApp (+8801710978997) to obtain an official license key.", "Activation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDeactivate_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            this,
            "Are you sure you want to remove and deactivate the paid license key from this machine?",
            "Confirm License Removal",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (confirm == DialogResult.Yes)
        {
            LicenseManager.RemovePaidLicense();
            MessageBox.Show(this, "License key has been successfully removed from this machine.", "Deactivated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshStatus();
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
