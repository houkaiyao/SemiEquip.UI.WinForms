using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("RedLightOn")]
    [DefaultEvent("Click")]
    [DesignerCategory("Code")]
    public class FourColorLightControl : Control
    {
        private const int DefaultWidth = 80;

        private bool _maintainAspectRatio = true;
        private bool _redLightOn;
        private bool _yellowLightOn;
        private bool _greenLightOn;
        private bool _blueLightOn;
        private int _lightGap;
        private Color _redOnColor = Color.FromArgb(226, 64, 64);
        private Color _redOffColor = Color.FromArgb(72, 28, 28);
        private Color _yellowOnColor = Color.FromArgb(245, 190, 58);
        private Color _yellowOffColor = Color.FromArgb(78, 65, 28);
        private Color _greenOnColor = Color.FromArgb(46, 184, 92);
        private Color _greenOffColor = Color.FromArgb(25, 70, 42);
        private Color _blueOnColor = Color.FromArgb(55, 137, 255);
        private Color _blueOffColor = Color.FromArgb(24, 48, 84);
        private Color _shadowColor = Color.FromArgb(120, 0, 0, 0);
        private Color _highlightColor = Color.FromArgb(80, 255, 255, 255);
        private bool _showFrostedLines = true;
        private int _frostedLineSpacing = 3;

        public FourColorLightControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.FromArgb(18, 22, 28);
            ForeColor = Color.FromArgb(235, 239, 244);
            Size = new Size(DefaultWidth, DefaultWidth * 4);
        }

        [Category("四色灯")]
        [Description("红灯是否点亮。True 为亮色，False 为暗色。")]
        [DefaultValue(false)]
        public bool RedLightOn
        {
            get { return _redLightOn; }
            set { SetLightState(ref _redLightOn, value); }
        }

        [Category("四色灯")]
        [Description("黄灯是否点亮。True 为亮色，False 为暗色。")]
        [DefaultValue(false)]
        public bool YellowLightOn
        {
            get { return _yellowLightOn; }
            set { SetLightState(ref _yellowLightOn, value); }
        }

        [Category("四色灯")]
        [Description("绿灯是否点亮。True 为亮色，False 为暗色。")]
        [DefaultValue(false)]
        public bool GreenLightOn
        {
            get { return _greenLightOn; }
            set { SetLightState(ref _greenLightOn, value); }
        }

        [Category("四色灯")]
        [Description("蓝灯是否点亮。True 为亮色，False 为暗色。")]
        [DefaultValue(false)]
        public bool BlueLightOn
        {
            get { return _blueLightOn; }
            set { SetLightState(ref _blueLightOn, value); }
        }

        [Category("四色灯")]
        [Description("调整控件大小时保持高度:宽度为 4:1，便于四个长方体灯段纵向排列。")]
        [DefaultValue(true)]
        public bool MaintainAspectRatio
        {
            get { return _maintainAspectRatio; }
            set
            {
                if (_maintainAspectRatio == value)
                {
                    return;
                }

                _maintainAspectRatio = value;
                if (_maintainAspectRatio)
                {
                    Height = Math.Max(4, Width * 4);
                }

                Invalidate();
            }
        }

        [Category("四色灯")]
        [Description("相邻灯段之间的间距，单位为像素。默认 0 表示四段相连。")]
        [DefaultValue(0)]
        public int LightGap
        {
            get { return _lightGap; }
            set
            {
                _lightGap = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("四色灯颜色")]
        [Description("红灯点亮时的颜色。")]
        public Color RedOnColor
        {
            get { return _redOnColor; }
            set { SetColor(ref _redOnColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("红灯熄灭时的暗色。")]
        public Color RedOffColor
        {
            get { return _redOffColor; }
            set { SetColor(ref _redOffColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("黄灯点亮时的颜色。")]
        public Color YellowOnColor
        {
            get { return _yellowOnColor; }
            set { SetColor(ref _yellowOnColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("黄灯熄灭时的暗色。")]
        public Color YellowOffColor
        {
            get { return _yellowOffColor; }
            set { SetColor(ref _yellowOffColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("绿灯点亮时的颜色。")]
        public Color GreenOnColor
        {
            get { return _greenOnColor; }
            set { SetColor(ref _greenOnColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("绿灯熄灭时的暗色。")]
        public Color GreenOffColor
        {
            get { return _greenOffColor; }
            set { SetColor(ref _greenOffColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("蓝灯点亮时的颜色。")]
        public Color BlueOnColor
        {
            get { return _blueOnColor; }
            set { SetColor(ref _blueOnColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("蓝灯熄灭时的暗色。")]
        public Color BlueOffColor
        {
            get { return _blueOffColor; }
            set { SetColor(ref _blueOffColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("灯段暗部阴影颜色，用于增强长方体层次感。")]
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set { SetColor(ref _shadowColor, value); }
        }

        [Category("四色灯颜色")]
        [Description("灯段中间柔光颜色，用于增强圆柱灯罩质感。")]
        public Color HighlightColor
        {
            get { return _highlightColor; }
            set { SetColor(ref _highlightColor, value); }
        }

        [Category("四色灯")]
        [Description("是否显示横向细纹，用于模拟磨砂灯罩或棱纹塑料效果。")]
        [DefaultValue(true)]
        public bool ShowFrostedLines
        {
            get { return _showFrostedLines; }
            set
            {
                if (_showFrostedLines == value)
                {
                    return;
                }

                _showFrostedLines = value;
                Invalidate();
            }
        }

        [Category("四色灯")]
        [Description("横向磨砂细纹间距，单位为像素。")]
        [DefaultValue(3)]
        public int FrostedLineSpacing
        {
            get { return _frostedLineSpacing; }
            set
            {
                _frostedLineSpacing = Math.Max(1, value);
                Invalidate();
            }
        }

        public void SetLightStates(bool redOn, bool yellowOn, bool greenOn, bool blueOn)
        {
            _redLightOn = redOn;
            _yellowLightOn = yellowOn;
            _greenLightOn = greenOn;
            _blueLightOn = blueOn;
            Invalidate();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (_maintainAspectRatio && Dock == DockStyle.None)
            {
                if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width
                    && (specified & BoundsSpecified.Height) != BoundsSpecified.Height)
                {
                    height = Math.Max(4, width * 4);
                }
                else if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height
                    && (specified & BoundsSpecified.Width) != BoundsSpecified.Width)
                {
                    width = Math.Max(1, height / 4);
                }
                else if ((specified & BoundsSpecified.Size) == BoundsSpecified.Size)
                {
                    height = Math.Max(4, width * 4);
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBackground(graphics);

            Rectangle towerBounds = GetTowerBounds();
            if (towerBounds.Width <= 0 || towerBounds.Height <= 0)
            {
                return;
            }

            using (GraphicsPath towerPath = CreateRoundRectanglePath(towerBounds, Math.Max(2, towerBounds.Width / 7)))
            {
                Region oldClip = graphics.Clip;
                graphics.SetClip(towerPath, CombineMode.Replace);

                DrawSegment(graphics, 0, _redLightOn ? _redOnColor : _redOffColor);
                DrawSegment(graphics, 1, _yellowLightOn ? _yellowOnColor : _yellowOffColor);
                DrawSegment(graphics, 2, _greenLightOn ? _greenOnColor : _greenOffColor);
                DrawSegment(graphics, 3, _blueLightOn ? _blueOnColor : _blueOffColor);

                graphics.Clip = oldClip;
                oldClip.Dispose();
            }
        }

        private void DrawBackground(Graphics graphics)
        {
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        private void DrawSegment(Graphics graphics, int index, Color color)
        {
            Rectangle segmentBounds = GetSegmentBounds(index);
            if (segmentBounds.Width <= 0 || segmentBounds.Height <= 0)
            {
                return;
            }

            using (LinearGradientBrush fillBrush = new LinearGradientBrush(
                segmentBounds,
                color,
                color,
                LinearGradientMode.Horizontal))
            {
                fillBrush.InterpolationColors = CreateCylinderBlend(color);
                graphics.FillRectangle(fillBrush, segmentBounds);
            }

            DrawFrostedLines(graphics, segmentBounds);
            DrawSegmentJoint(graphics, segmentBounds, index);
        }

        private void DrawFrostedLines(Graphics graphics, Rectangle segmentBounds)
        {
            if (!_showFrostedLines)
            {
                return;
            }

            using (Pen lightPen = new Pen(Color.FromArgb(55, 255, 255, 255)))
            using (Pen shadowPen = new Pen(Color.FromArgb(45, 0, 0, 0)))
            {
                int spacing = Math.Max(1, _frostedLineSpacing);
                int left = segmentBounds.Left + Math.Max(1, segmentBounds.Width / 18);
                int right = segmentBounds.Right - Math.Max(2, segmentBounds.Width / 18);

                for (int y = segmentBounds.Top + spacing; y < segmentBounds.Bottom - 1; y += spacing)
                {
                    graphics.DrawLine(lightPen, left, y, right, y);
                    if (y + 1 < segmentBounds.Bottom)
                    {
                        graphics.DrawLine(shadowPen, left, y + 1, right, y + 1);
                    }
                }
            }
        }

        private void DrawSegmentJoint(Graphics graphics, Rectangle segmentBounds, int index)
        {
            if (index <= 0)
            {
                return;
            }

            using (Pen shadowPen = new Pen(Color.FromArgb(Math.Min(160, (int)_shadowColor.A), _shadowColor)))
            using (Pen highlightPen = new Pen(Color.FromArgb(Math.Min(90, (int)_highlightColor.A), _highlightColor)))
            {
                graphics.DrawLine(shadowPen, segmentBounds.Left, segmentBounds.Top, segmentBounds.Right - 1, segmentBounds.Top);
                graphics.DrawLine(highlightPen, segmentBounds.Left, segmentBounds.Top + 1, segmentBounds.Right - 1, segmentBounds.Top + 1);
            }
        }

        private Rectangle GetSegmentBounds(int index)
        {
            Rectangle towerBounds = GetTowerBounds();
            int gap = Math.Min(_lightGap, Math.Max(0, Height / 12));
            int totalGap = gap * 3;
            int segmentHeight = Math.Max(1, (towerBounds.Height - totalGap) / 4);
            int y = towerBounds.Top + index * (segmentHeight + gap);

            if (index == 3)
            {
                segmentHeight = Math.Max(1, towerBounds.Bottom - y);
            }

            return new Rectangle(towerBounds.Left, y, Math.Max(1, towerBounds.Width), segmentHeight);
        }

        private Rectangle GetTowerBounds()
        {
            return new Rectangle(0, 0, Math.Max(1, Width), Math.Max(1, Height));
        }

        private void SetLightState(ref bool field, bool value)
        {
            if (field == value)
            {
                return;
            }

            field = value;
            Invalidate();
        }

        private void SetColor(ref Color field, Color value)
        {
            if (field == value)
            {
                return;
            }

            field = value;
            Invalidate();
        }

        private ColorBlend CreateCylinderBlend(Color color)
        {
            Color edgeDark = BlendColor(color, Color.Black, 0.42f);
            Color sideDark = BlendColor(color, Color.Black, 0.24f);
            Color centerLight = BlendColor(color, Color.White, _highlightColor.A / 255f * 0.38f);

            ColorBlend blend = new ColorBlend();
            blend.Positions = new[] { 0.0f, 0.12f, 0.46f, 0.58f, 0.88f, 1.0f };
            blend.Colors = new[]
            {
                edgeDark,
                sideDark,
                centerLight,
                centerLight,
                sideDark,
                edgeDark
            };

            return blend;
        }

        private static Color BlendColor(Color source, Color target, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            int r = source.R + (int)((target.R - source.R) * amount);
            int g = source.G + (int)((target.G - source.G) * amount);
            int b = source.B + (int)((target.B - source.B) * amount);

            return Color.FromArgb(source.A, r, g, b);
        }

        private static GraphicsPath CreateRoundRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));

            if (diameter <= 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
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
}
