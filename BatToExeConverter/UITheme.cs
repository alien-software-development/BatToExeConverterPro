using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BatToExeConverter;

public static class UITheme
{
    // Premium Obsidian & Cyber Deep Palette
    public static readonly Color BgApp = Color.FromArgb(11, 15, 25);           // #0B0F19 - Main Background
    public static readonly Color BgSidebar = Color.FromArgb(15, 23, 42);       // #0F172A - Sidebar / Header Dark Slate
    public static readonly Color BgCard = Color.FromArgb(20, 30, 55);          // #141E37 - Card Surface
    public static readonly Color BgCardHover = Color.FromArgb(30, 41, 69);     // #1E2945 - Card Hover
    public static readonly Color BgInput = Color.FromArgb(13, 20, 36);         // #0D1424 - Input Field
    public static readonly Color BgTerminal = Color.FromArgb(8, 12, 20);        // #080C14 - Terminal Console
    public static readonly Color Border = Color.FromArgb(37, 52, 84);          // #253454 - Subtle Outline
    public static readonly Color BorderHover = Color.FromArgb(59, 82, 128);    // #3B5280 - Hover Outline
    public static readonly Color BorderActive = Color.FromArgb(14, 165, 233);  // #0EA5E9 - Active Glow

    // Text Hierarchy
    public static readonly Color TextWhite = Color.FromArgb(255, 255, 255);
    public static readonly Color TextPrimary = Color.FromArgb(241, 245, 249);  // #F1F5F9 - Slate 100
    public static readonly Color TextSecondary = Color.FromArgb(148, 163, 184);// #94A3B8 - Slate 400
    public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);    // #64748B - Slate 500

    // Accents & Brand
    public static readonly Color BrandAccent = Color.FromArgb(14, 165, 233);   // #0EA5E9 - Sky Blue
    public static readonly Color BrandAccentHover = Color.FromArgb(2, 132, 199);// #0284C7
    public static readonly Color NeonGreen = Color.FromArgb(16, 185, 129);     // #10B981 - Emerald Green
    public static readonly Color NeonGreenHover = Color.FromArgb(5, 150, 105); // #059669
    public static readonly Color WarningAmber = Color.FromArgb(245, 158, 11);  // #F59E0B - Amber
    public static readonly Color DangerRed = Color.FromArgb(239, 68, 68);      // #EF4444 - Red
    public static readonly Color Purple = Color.FromArgb(168, 85, 247);         // #A855F7 - Purple

    // Backward Compatibility Aliases
    public static readonly Color BgDark = BgApp;
    public static readonly Color PrimaryAccent = BrandAccent;
    public static readonly Color PrimaryHover = BrandAccentHover;
    public static readonly Color SuccessColor = NeonGreen;
    public static readonly Color SuccessHover = NeonGreenHover;
    public static readonly Color WarningColor = WarningAmber;
    public static readonly Color DangerColor = DangerRed;
    public static readonly Color BorderColor = Border;
    public static readonly Color PurpleAccent = Purple;

    // High-DPI Crisp Typography
    public static readonly Font FontBrand = new("Segoe UI", 12f, FontStyle.Bold);
    public static readonly Font FontH1 = new("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font FontH2 = new("Segoe UI", 11.5f, FontStyle.Bold);
    public static readonly Font FontH3 = new("Segoe UI", 9.75f, FontStyle.Bold);
    public static readonly Font FontBody = new("Segoe UI", 9.25f, FontStyle.Regular);
    public static readonly Font FontBold = new("Segoe UI", 9.25f, FontStyle.Bold);
    public static readonly Font FontSmall = new("Segoe UI", 8.25f, FontStyle.Regular);
    public static readonly Font FontCode = new("Consolas", 9.25f, FontStyle.Regular);
    public static readonly Font FontRegular = FontBody;
    public static readonly Font FontHeading = FontH1;

    public static GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
    {
        int diameter = Math.Max(2, radius * 2);
        Size size = new(diameter, diameter);
        Rectangle arc = new(bounds.Location, size);
        GraphicsPath path = new();

        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}

public class ProCard : Panel
{
    public int Radius { get; set; } = 10;
    public Color CustomBorderColor { get; set; } = UITheme.Border;
    public Color BorderColor { get => CustomBorderColor; set => CustomBorderColor = value; }
    public int BorderWidth { get; set; } = 1;

    public ProCard()
    {
        this.DoubleBuffered = true;
        this.BackColor = UITheme.BgCard;
        this.Padding = new Padding(16);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
        using var path = UITheme.GetRoundedRectangle(rect, Radius);

        using (var brush = new SolidBrush(this.BackColor))
        {
            e.Graphics.FillPath(brush, path);
        }

        if (BorderWidth > 0)
        {
            using var pen = new Pen(CustomBorderColor, BorderWidth);
            e.Graphics.DrawPath(pen, path);
        }
    }
}

public class ProButton : Button
{
    public int Radius { get; set; } = 8;
    public Color BgNormal { get; set; } = UITheme.BrandAccent;
    public Color BgHover { get; set; } = UITheme.BrandAccentHover;
    public Color NormalColor { get => BgNormal; set => BgNormal = value; }
    public Color HoverColor { get => BgHover; set => BgHover = value; }
    public Color TextColor { get; set; } = Color.White;
    public bool IsPrimaryGlow { get; set; } = false;

    private bool isHover = false;
    private bool isDown = false;

    public ProButton()
    {
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;
        this.Cursor = Cursors.Hand;
        this.Font = UITheme.FontBold;
        this.DoubleBuffered = true;
        this.Height = 38;
        this.MouseEnter += (s, e) => { isHover = true; Invalidate(); };
        this.MouseLeave += (s, e) => { isHover = false; isDown = false; Invalidate(); };
        this.MouseDown += (s, e) => { isDown = true; Invalidate(); };
        this.MouseUp += (s, e) => { isDown = false; Invalidate(); };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
        using var path = UITheme.GetRoundedRectangle(rect, Radius);

        Color currentBg;
        if (!this.Enabled)
        {
            currentBg = Color.FromArgb(30, 40, 60);
        }
        else if (isDown)
        {
            currentBg = Color.FromArgb(Math.Max(0, BgHover.R - 20), Math.Max(0, BgHover.G - 20), Math.Max(0, BgHover.B - 20));
        }
        else if (isHover)
        {
            currentBg = BgHover;
        }
        else
        {
            currentBg = BgNormal;
        }

        using (var brush = new SolidBrush(currentBg))
        {
            e.Graphics.FillPath(brush, path);
        }

        Color txt = !this.Enabled ? UITheme.TextMuted : TextColor;
        TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;
        
        if (this.TextAlign == ContentAlignment.MiddleLeft)
            flags |= TextFormatFlags.Left;
        else if (this.TextAlign == ContentAlignment.MiddleRight)
            flags |= TextFormatFlags.Right;
        else
            flags |= TextFormatFlags.HorizontalCenter;

        var textRect = new Rectangle(12, 0, this.Width - 24, this.Height);
        TextRenderer.DrawText(e.Graphics, this.Text, this.Font, textRect, txt, flags);
    }
}

public class SidebarNavButton : Control
{
    private bool isHover = false;
    private bool isSelected = false;
    private string icon = "⚡";
    private string title = "Nav Item";

    public bool IsSelected
    {
        get => isSelected;
        set { isSelected = value; Invalidate(); }
    }

    public string IconChar
    {
        get => icon;
        set { icon = value; Invalidate(); }
    }

    public string Title
    {
        get => title;
        set { title = value; Invalidate(); }
    }

    public SidebarNavButton()
    {
        this.DoubleBuffered = true;
        this.Cursor = Cursors.Hand;
        this.Height = 44;
        this.Font = UITheme.FontBody;
        this.MouseEnter += (s, e) => { isHover = true; Invalidate(); };
        this.MouseLeave += (s, e) => { isHover = false; Invalidate(); };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        var rect = new Rectangle(0, 0, this.Width, this.Height);

        if (isSelected)
        {
            using var fillBrush = new SolidBrush(Color.FromArgb(28, 42, 74));
            e.Graphics.FillRectangle(fillBrush, rect);

            using var indBrush = new SolidBrush(UITheme.BrandAccent);
            e.Graphics.FillRectangle(indBrush, new Rectangle(0, 0, 4, this.Height));
        }
        else if (isHover)
        {
            using var hoverBrush = new SolidBrush(Color.FromArgb(22, 33, 56));
            e.Graphics.FillRectangle(hoverBrush, rect);
        }

        Color textColor = isSelected ? UITheme.TextWhite : (isHover ? UITheme.TextPrimary : UITheme.TextSecondary);
        Font fontToUse = isSelected ? UITheme.FontBold : UITheme.FontBody;

        using (var iconFont = new Font("Segoe UI Emoji", 10.5f))
        {
            var iconRect = new Rectangle(16, 0, 24, this.Height);
            TextRenderer.DrawText(e.Graphics, icon, iconFont, iconRect, isSelected ? UITheme.BrandAccent : UITheme.TextSecondary,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        var textRect = new Rectangle(46, 0, this.Width - 52, this.Height);
        TextRenderer.DrawText(e.Graphics, title, fontToUse, textRect, textColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.SingleLine);
    }
}

public class ModernInputBox : Panel
{
    private readonly TextBox innerBox;
    private bool isFocused = false;

    public string TextValue
    {
        get => innerBox.Text;
        set => innerBox.Text = value;
    }

    public TextBox InnerTextBox => innerBox;
    public event EventHandler? ValueChanged;

    public ModernInputBox()
    {
        this.Height = 38;
        this.BackColor = UITheme.BgInput;
        this.Padding = new Padding(12, 9, 12, 9);
        this.DoubleBuffered = true;
        this.Cursor = Cursors.IBeam;

        innerBox = new TextBox
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.BgInput,
            ForeColor = UITheme.TextWhite,
            BorderStyle = BorderStyle.None,
            Font = UITheme.FontBody
        };

        innerBox.GotFocus += (s, e) => { isFocused = true; Invalidate(); };
        innerBox.LostFocus += (s, e) => { isFocused = false; Invalidate(); };
        innerBox.TextChanged += (s, e) => ValueChanged?.Invoke(this, e);

        this.Click += (s, e) => innerBox.Focus();
        this.Controls.Add(innerBox);
    }

    public void SetReadOnly(bool readOnly)
    {
        innerBox.ReadOnly = readOnly;
    }

    public void SetPasswordChar(bool isPassword)
    {
        innerBox.UseSystemPasswordChar = isPassword;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
        using var path = UITheme.GetRoundedRectangle(rect, 6);

        using (var bgBrush = new SolidBrush(UITheme.BgInput))
        {
            e.Graphics.FillPath(bgBrush, path);
        }

        using (var pen = new Pen(isFocused ? UITheme.BorderActive : UITheme.Border, 1))
        {
            e.Graphics.DrawPath(pen, path);
        }
    }
}

public class FlatCard : ProCard { }
public class RoundedPanel : ProCard { }
public class FlatButton : ProButton { }
public class ModernButton : ProButton { }
public class StyledTextBox : TextBox
{
    public StyledTextBox()
    {
        this.BackColor = UITheme.BgInput;
        this.ForeColor = UITheme.TextWhite;
        this.BorderStyle = BorderStyle.FixedSingle;
        this.Font = UITheme.FontBody;
    }
}
