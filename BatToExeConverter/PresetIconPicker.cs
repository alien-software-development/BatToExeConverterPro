using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BatToExeConverter;

public class PresetIconPicker : Form
{
    public string? SelectedIconPath { get; private set; }

    public PresetIconPicker()
    {
        InitializePicker();
    }

    private void InitializePicker()
    {
        this.Text = "🎨 Select Built-in Pro Preset Icon";
        this.Size = new Size(680, 520);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = UITheme.BgApp;
        this.Font = UITheme.FontBody;
        this.ForeColor = UITheme.TextWhite;

        try
        {
            if (File.Exists(Path.Combine(AppContext.BaseDirectory, "Assets", "app_icon.ico")))
            {
                this.Icon = new Icon(Path.Combine(AppContext.BaseDirectory, "Assets", "app_icon.ico"));
            }
        }
        catch { }

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = UITheme.BgSidebar,
            Padding = new Padding(16, 12, 16, 12)
        };

        var lblTitle = new Label
        {
            Text = "🎨 Pro Preset Icon Library",
            Font = UITheme.FontH1,
            ForeColor = UITheme.BrandAccent,
            Location = new Point(16, 10),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = "Select from 12+ pre-rendered high-definition application icons (256x256 multi-res)",
            Font = UITheme.FontSmall,
            ForeColor = UITheme.TextSecondary,
            Location = new Point(18, 34),
            AutoSize = true
        };

        pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSub });
        this.Controls.Add(pnlHeader);

        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 55,
            BackColor = UITheme.BgSidebar,
            Padding = new Padding(16, 10, 16, 10)
        };

        var btnCancel = new ProButton
        {
            Text = "Cancel",
            Size = new Size(100, 34),
            Location = new Point(pnlBottom.Width - 120, 10),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BgNormal = UITheme.BgCardHover,
            BgHover = UITheme.DangerRed,
            Radius = 6,
            DialogResult = DialogResult.Cancel
        };

        pnlBottom.Controls.Add(btnCancel);
        this.Controls.Add(pnlBottom);

        var flowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(16),
            BackColor = UITheme.BgApp
        };

        string presetsDir = Path.Combine(AppContext.BaseDirectory, "Assets", "PresetIcons");
        if (!Directory.Exists(presetsDir))
        {
            presetsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Assets", "PresetIcons");
        }
        if (!Directory.Exists(presetsDir))
        {
            presetsDir = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "PresetIcons");
        }

        if (Directory.Exists(presetsDir))
        {
            var files = Directory.GetFiles(presetsDir, "*.ico");
            Array.Sort(files);

            foreach (var iconFile in files)
            {
                string name = Path.GetFileNameWithoutExtension(iconFile);
                if (name.Length > 3 && char.IsDigit(name[0]) && name[2] == '_')
                {
                    name = name[3..];
                }

                var card = new ProCard
                {
                    Size = new Size(138, 125),
                    Margin = new Padding(8),
                    BackColor = UITheme.BgCard,
                    CustomBorderColor = UITheme.Border,
                    Cursor = Cursors.Hand,
                    Radius = 8
                };

                Image? img = null;
                try
                {
                    using var stream = File.OpenRead(iconFile);
                    using var ico = new Icon(stream, 64, 64);
                    img = ico.ToBitmap();
                }
                catch { }

                var pic = new PictureBox
                {
                    Size = new Size(50, 50),
                    Location = new Point((138 - 50) / 2, 12),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = img,
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand
                };

                var lbl = new Label
                {
                    Text = name,
                    Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
                    ForeColor = UITheme.TextPrimary,
                    Location = new Point(4, 68),
                    Size = new Size(130, 48),
                    TextAlign = ContentAlignment.TopCenter,
                    Cursor = Cursors.Hand
                };

                void OnSelect(object? s, EventArgs e)
                {
                    SelectedIconPath = iconFile;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

                card.Click += OnSelect;
                pic.Click += OnSelect;
                lbl.Click += OnSelect;

                card.MouseEnter += (s, e) => { card.BackColor = UITheme.BgCardHover; card.CustomBorderColor = UITheme.BrandAccent; card.Invalidate(); };
                card.MouseLeave += (s, e) => { card.BackColor = UITheme.BgCard; card.CustomBorderColor = UITheme.Border; card.Invalidate(); };
                pic.MouseEnter += (s, e) => { card.BackColor = UITheme.BgCardHover; card.CustomBorderColor = UITheme.BrandAccent; card.Invalidate(); };
                lbl.MouseEnter += (s, e) => { card.BackColor = UITheme.BgCardHover; card.CustomBorderColor = UITheme.BrandAccent; card.Invalidate(); };

                card.Controls.AddRange(new Control[] { pic, lbl });
                flowPanel.Controls.Add(card);
            }
        }

        this.Controls.Add(flowPanel);
        flowPanel.BringToFront();
    }
}
