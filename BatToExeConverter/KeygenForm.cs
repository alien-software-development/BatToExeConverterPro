using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BatToExeConverter;

public class KeygenForm : Form
{
    private TextBox txtMachineId = null!;
    private TextBox txtCustomerName = null!;
    private TextBox txtGeneratedKey = null!;
    private ModernButton btnGenerate = null!;
    private ModernButton btnCopy = null!;
    private ModernButton btnApplyDirectly = null!;

    public KeygenForm(string? defaultMachineId = null)
    {
        InitializeCustomKeygen();
        if (!string.IsNullOrEmpty(defaultMachineId))
        {
            txtMachineId.Text = defaultMachineId;
        }
        else
        {
            txtMachineId.Text = LicenseManager.GetMachineFingerprint();
        }
        txtCustomerName.Text = "Alien Software VIP";
    }

    private void InitializeCustomKeygen()
    {
        this.Text = "👽 Alien Software Development — Official Key Generator (Keygen)";
        this.Size = new Size(620, 480);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = UITheme.BgDark;
        this.Font = UITheme.FontRegular;

        try
        {
            string ico = Path.Combine(AppContext.BaseDirectory, "Assets", "keygen_icon.ico");
            if (!File.Exists(ico)) ico = Path.Combine(AppContext.BaseDirectory, "Assets", "app_icon.ico");
            if (File.Exists(ico)) this.Icon = new Icon(ico);
        }
        catch { }

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = UITheme.BgCard
        };

        var lblTitle = new Label
        {
            Text = "🔑 ALIEN MASTER KEYGEN v2.0",
            Font = UITheme.FontHeading,
            ForeColor = UITheme.PurpleAccent,
            Location = new Point(20, 14),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = "Cryptographic Paid License Generator for Alien Software Products",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextSecondary,
            Location = new Point(22, 38),
            AutoSize = true
        };

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblSub);
        this.Controls.Add(pnlHeader);

        var pnlBody = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(25, 20, 25, 20)
        };

        var lblMid = new Label { Text = "Target Hardware Machine ID:", ForeColor = UITheme.TextSecondary, Location = new Point(25, 15), AutoSize = true, Font = UITheme.FontSmall };
        txtMachineId = new TextBox
        {
            Location = new Point(25, 38),
            Size = new Size(550, 26),
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.PrimaryAccent,
            Font = UITheme.FontCode
        };

        var lblCust = new Label { Text = "Customer / Registered Name:", ForeColor = UITheme.TextSecondary, Location = new Point(25, 78), AutoSize = true, Font = UITheme.FontSmall };
        txtCustomerName = new TextBox
        {
            Location = new Point(25, 101),
            Size = new Size(550, 26),
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextPrimary,
            Font = UITheme.FontRegular
        };

        btnGenerate = new ModernButton
        {
            Text = "⚡ Generate Paid Lifetime License Key",
            Location = new Point(25, 145),
            Size = new Size(550, 38),
            NormalColor = UITheme.PurpleAccent,
            HoverColor = Color.FromArgb(147, 51, 234),
            Font = UITheme.FontBold
        };
        btnGenerate.Click += BtnGenerate_Click;

        var lblOut = new Label { Text = "Generated Lifetime Activation Key:", ForeColor = UITheme.TextSecondary, Location = new Point(25, 200), AutoSize = true, Font = UITheme.FontSmall };
        txtGeneratedKey = new TextBox
        {
            Location = new Point(25, 223),
            Size = new Size(550, 28),
            ReadOnly = true,
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.SuccessColor,
            Font = new Font("Consolas", 11f, FontStyle.Bold)
        };

        btnCopy = new ModernButton
        {
            Text = "📋 Copy Key",
            Location = new Point(25, 265),
            Size = new Size(265, 36),
            NormalColor = UITheme.BgCardHover,
            HoverColor = UITheme.PrimaryAccent,
            Font = UITheme.FontBold
        };
        btnCopy.Click += (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(txtGeneratedKey.Text))
            {
                Clipboard.SetText(txtGeneratedKey.Text);
                MessageBox.Show(this, "License key copied to clipboard!", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };

        btnApplyDirectly = new ModernButton
        {
            Text = "✔ Activate This Machine Directly",
            Location = new Point(305, 265),
            Size = new Size(270, 36),
            NormalColor = UITheme.SuccessColor,
            HoverColor = Color.FromArgb(5, 150, 105),
            Font = UITheme.FontBold
        };
        btnApplyDirectly.Click += BtnApplyDirectly_Click;

        var lblDevInfo = new Label
        {
            Text = "Alien Software Development | Support: +8801710978997 | aliensoftwaredevelopment.com",
            ForeColor = UITheme.TextMuted,
            Location = new Point(25, 330),
            AutoSize = true,
            Font = UITheme.FontSmall
        };

        pnlBody.Controls.AddRange(new Control[]
        {
            lblMid, txtMachineId,
            lblCust, txtCustomerName,
            btnGenerate,
            lblOut, txtGeneratedKey,
            btnCopy, btnApplyDirectly,
            lblDevInfo
        });

        this.Controls.Add(pnlBody);
    }

    private void BtnGenerate_Click(object? sender, EventArgs e)
    {
        string mid = txtMachineId.Text.Trim();
        string user = txtCustomerName.Text.Trim();

        if (string.IsNullOrEmpty(mid) || string.IsNullOrEmpty(user))
        {
            MessageBox.Show(this, "Please enter both Machine ID and Customer Name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string key = LicenseManager.GenerateLicenseKey(mid, user);
        txtGeneratedKey.Text = key;
    }

    private void BtnApplyDirectly_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtGeneratedKey.Text))
        {
            BtnGenerate_Click(sender, e);
        }

        string mid = txtMachineId.Text.Trim();
        string user = txtCustomerName.Text.Trim();
        string key = txtGeneratedKey.Text.Trim();

        if (mid == LicenseManager.GetMachineFingerprint())
        {
            if (LicenseManager.ActivatePaidLicense(key, user))
            {
                MessageBox.Show(this, "Successfully activated this PC to Paid Lifetime License!", "Activated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "Key generation or validation error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            MessageBox.Show(this, "The Machine ID entered in the keygen belongs to another computer.\nCopy the generated key and enter it on that target computer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
