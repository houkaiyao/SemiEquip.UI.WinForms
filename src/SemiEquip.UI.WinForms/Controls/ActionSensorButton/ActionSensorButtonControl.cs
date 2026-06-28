using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("ButtonText")]
    [DefaultEvent("Click")]
    [DesignerCategory("Code")]
    public class ActionSensorButtonControl : Control
    {
        private const int DefaultWidth = 132;
        private const int DefaultHeight = 36;

        private bool _commandState;
        private SensorDisplayMode _sensorMode = SensorDisplayMode.Two;
        private bool _sensor1State;
        private bool _sensor2State;
        private Color _commandOnBackColor = Color.FromArgb(226, 64, 64);
        private Color _commandOffBackColor = Color.FromArgb(225, 236, 251);
        private Color _hoverBackColor = Color.FromArgb(43, 125, 211);
        private Color _pressedBackColor = Color.FromArgb(32, 104, 184);
        private Color _commandOnForeColor = Color.White;
        private Color _commandOffForeColor = Color.Black;
        private Color _hoverForeColor = Color.White;
        private Color _sensorOnColor = Color.FromArgb(40, 112, 210);
        private Color _sensorOffColor = Color.White;
        private Color _sensorBorderColor = Color.FromArgb(40, 112, 210);
        private Color _borderColor = Color.FromArgb(40, 112, 210);
        private int _cornerRadius = 8;
        private int _sensorLeftPadding = 10;
        private int _sensorTextSpacing = 8;
        private SensorIndicatorShape _sensorShape = SensorIndicatorShape.Rectangle;
        private int _shadow;
        private Color _shadowColor = Color.FromArgb(0, 0, 0);
        private int _shadowOffsetX;
        private int _shadowOffsetY = 1;
        private float _shadowOpacity = 0.18f;
        private bool _mouseHover;
        private bool _mousePressed;

        public ActionSensorButtonControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.Selectable
                | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            ForeColor = Color.Black;
            Font = new Font("Times New Roman", 10f, FontStyle.Regular, GraphicsUnit.Point);
            Size = new Size(DefaultWidth, DefaultHeight);
            MinimumSize = new Size(80, 32);
            Cursor = Cursors.Hand;
            Text = "Action";
        }

        [Category("动作传感器按钮")]
        [Description("按钮主标题文本，通常用于显示动作名称。")]
        [DefaultValue("Action")]
        public string ButtonText
        {
            get { return Text; }
            set { Text = value == null ? string.Empty : value; }
        }

        [Category("动作传感器按钮")]
        [Description("PLC 动作 BOOL 状态。True 和 False 的显示颜色由调用方配置。")]
        [DefaultValue(false)]
        public bool CommandState
        {
            get { return _commandState; }
            set { SetBoolean(ref _commandState, value); }
        }

        [Category("动作传感器按钮")]
        [Description("传感器显示模式，可选择不显示、显示一个或显示两个传感器。")]
        [DefaultValue(SensorDisplayMode.Two)]
        public SensorDisplayMode SensorMode
        {
            get { return _sensorMode; }
            set
            {
                if (_sensorMode == value)
                {
                    return;
                }

                _sensorMode = value;
                Invalidate();
            }
        }

        [Category("动作传感器按钮")]
        [Description("第一个 IO 传感器状态。")]
        [DefaultValue(false)]
        public bool Sensor1State
        {
            get { return _sensor1State; }
            set { SetBoolean(ref _sensor1State, value); }
        }

        [Category("动作传感器按钮")]
        [Description("第二个 IO 传感器状态。")]
        [DefaultValue(false)]
        public bool Sensor2State
        {
            get { return _sensor2State; }
            set { SetBoolean(ref _sensor2State, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("动作状态为 True 时按钮主体背景色。")]
        public Color CommandOnBackColor
        {
            get { return _commandOnBackColor; }
            set { SetColor(ref _commandOnBackColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("动作状态为 False 时按钮主体背景色。")]
        public Color CommandOffBackColor
        {
            get { return _commandOffBackColor; }
            set { SetColor(ref _commandOffBackColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("默认按钮背景色。用于兼容 AntdUI.Button 的 DefaultBack 命名，等同于 CommandOffBackColor。")]
        public Color DefaultBack
        {
            get { return _commandOffBackColor; }
            set { SetColor(ref _commandOffBackColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("鼠标悬浮且动作状态为 False 时按钮主体背景色。")]
        public Color HoverBackColor
        {
            get { return _hoverBackColor; }
            set { SetColor(ref _hoverBackColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("鼠标悬浮背景色。用于兼容 AntdUI.Button 的 BackHover 命名，等同于 HoverBackColor。")]
        public Color BackHover
        {
            get { return _hoverBackColor; }
            set { SetColor(ref _hoverBackColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("鼠标按下且动作状态为 False 时按钮主体背景色。")]
        public Color PressedBackColor
        {
            get { return _pressedBackColor; }
            set { SetColor(ref _pressedBackColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("鼠标按下背景色。用于兼容 AntdUI.Button 的 BackActive 命名，等同于 PressedBackColor。")]
        public Color BackActive
        {
            get { return _pressedBackColor; }
            set { SetColor(ref _pressedBackColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("动作状态为 True 时按钮主体文字色。")]
        public Color CommandOnForeColor
        {
            get { return _commandOnForeColor; }
            set { SetColor(ref _commandOnForeColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("动作状态为 False 时按钮主体文字色。")]
        public Color CommandOffForeColor
        {
            get { return _commandOffForeColor; }
            set { SetColor(ref _commandOffForeColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("鼠标悬浮且动作状态为 False 时按钮文字色。")]
        public Color ForeHover
        {
            get { return _hoverForeColor; }
            set { SetColor(ref _hoverForeColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("传感器状态为 True 时的指示颜色。")]
        public Color SensorOnColor
        {
            get { return _sensorOnColor; }
            set { SetColor(ref _sensorOnColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("传感器状态为 False 时的指示颜色。")]
        public Color SensorOffColor
        {
            get { return _sensorOffColor; }
            set { SetColor(ref _sensorOffColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("传感器指示边框颜色。")]
        public Color SensorBorderColor
        {
            get { return _sensorBorderColor; }
            set { SetColor(ref _sensorBorderColor, value); }
        }

        [Category("动作传感器按钮颜色")]
        [Description("控件外框颜色。")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { SetColor(ref _borderColor, value); }
        }

        [Category("动作传感器按钮外观")]
        [Description("控件圆角半径，单位为像素。")]
        [DefaultValue(8)]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                int normalized = Math.Max(0, value);
                if (_cornerRadius == normalized)
                {
                    return;
                }

                _cornerRadius = normalized;
                Invalidate();
            }
        }

        [Category("动作传感器按钮外观")]
        [Description("控件圆角半径，单位为像素。用于兼容 AntdUI.Button 的 Radius 命名，等同于 CornerRadius。")]
        [DefaultValue(8)]
        public int Radius
        {
            get { return CornerRadius; }
            set { CornerRadius = value; }
        }

        [Category("动作传感器按钮布局")]
        [Description("状态灯距离按钮左边界的距离，单位为像素。")]
        [DefaultValue(10)]
        public int SensorLeftPadding
        {
            get { return _sensorLeftPadding; }
            set
            {
                int normalized = Math.Max(0, value);
                if (_sensorLeftPadding == normalized)
                {
                    return;
                }

                _sensorLeftPadding = normalized;
                Invalidate();
            }
        }

        [Category("动作传感器按钮布局")]
        [Description("状态灯右侧到按钮文字区域的距离，单位为像素。")]
        [DefaultValue(8)]
        public int SensorTextSpacing
        {
            get { return _sensorTextSpacing; }
            set
            {
                int normalized = Math.Max(0, value);
                if (_sensorTextSpacing == normalized)
                {
                    return;
                }

                _sensorTextSpacing = normalized;
                Invalidate();
            }
        }

        [Category("阴影")]
        [Description("阴影大小，单位为像素。默认 0 表示不绘制阴影。")]
        [DefaultValue(0)]
        public int Shadow
        {
            get { return _shadow; }
            set
            {
                int normalized = Math.Max(0, value);
                if (_shadow == normalized)
                {
                    return;
                }

                _shadow = normalized;
                Invalidate();
            }
        }

        [Category("阴影")]
        [Description("阴影颜色。")]
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set { SetColor(ref _shadowColor, value); }
        }

        [Category("阴影")]
        [Description("阴影横向偏移，单位为像素。")]
        [DefaultValue(0)]
        public int ShadowOffsetX
        {
            get { return _shadowOffsetX; }
            set
            {
                if (_shadowOffsetX == value)
                {
                    return;
                }

                _shadowOffsetX = value;
                Invalidate();
            }
        }

        [Category("阴影")]
        [Description("阴影纵向偏移，单位为像素。")]
        [DefaultValue(1)]
        public int ShadowOffsetY
        {
            get { return _shadowOffsetY; }
            set
            {
                if (_shadowOffsetY == value)
                {
                    return;
                }

                _shadowOffsetY = value;
                Invalidate();
            }
        }

        [Category("阴影")]
        [Description("阴影透明度，有效范围为 0 到 1。")]
        [DefaultValue(0.18f)]
        public float ShadowOpacity
        {
            get { return _shadowOpacity; }
            set
            {
                float normalized = Math.Max(0f, Math.Min(1f, value));
                if (Math.Abs(_shadowOpacity - normalized) < 0.001f)
                {
                    return;
                }

                _shadowOpacity = normalized;
                Invalidate();
            }
        }

        [Category("动作传感器按钮外观")]
        [Description("传感器指示灯形状。")]
        [DefaultValue(SensorIndicatorShape.Rectangle)]
        public SensorIndicatorShape SensorShape
        {
            get { return _sensorShape; }
            set
            {
                if (_sensorShape == value)
                {
                    return;
                }

                _sensorShape = value;
                Invalidate();
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                _mousePressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_mousePressed)
            {
                _mousePressed = false;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _mouseHover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _mouseHover = false;
            if (_mousePressed)
            {
                _mousePressed = false;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle bounds = new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1));
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            DrawBackground(graphics);

            DrawSimpleButton(graphics, bounds);
        }

        private void DrawSimpleButton(Graphics graphics, Rectangle bounds)
        {
            Rectangle outerBounds = GetButtonBodyBounds(bounds);
            Color commandBackColor = GetCurrentCommandBackColor();

            DrawButtonShadow(graphics, outerBounds);

            using (GraphicsPath outerPath = CreateRoundRectanglePath(outerBounds, GetActualCornerRadius(outerBounds)))
            using (SolidBrush bodyBrush = new SolidBrush(commandBackColor))
            using (Pen borderPen = new Pen(_commandState ? BlendColor(_pressedBackColor, Color.Black, 0.20f) : _borderColor))
            {
                graphics.FillPath(bodyBrush, outerPath);
                graphics.DrawPath(borderPen, outerPath);
            }

            int sensorCount = GetVisibleSensorCount();
            int padding = Math.Max(4, Math.Min(7, Math.Min(Width, Height) / 9));
            int lampAreaWidth = sensorCount > 0 ? GetLampSize(outerBounds.Height, sensorCount) : 0;
            Rectangle lampAreaBounds = sensorCount > 0
                ? new Rectangle(outerBounds.Left + _sensorLeftPadding, outerBounds.Top, lampAreaWidth, outerBounds.Height)
                : Rectangle.Empty;

            if (sensorCount > 0)
            {
                DrawSimpleLampArea(graphics, lampAreaBounds, sensorCount, outerBounds);
            }

            int textLeft = sensorCount > 0 ? lampAreaBounds.Right + _sensorTextSpacing : outerBounds.Left + padding;
            Rectangle textBounds = new Rectangle(
                textLeft,
                outerBounds.Top + padding,
                Math.Max(1, outerBounds.Right - textLeft - padding),
                Math.Max(1, outerBounds.Height - padding * 2));
            DrawSimpleButtonText(graphics, textBounds);
        }

        private Rectangle GetButtonBodyBounds(Rectangle bounds)
        {
            int shadow = Math.Max(0, _shadow);
            if (shadow <= 0)
            {
                return new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
            }

            int leftInset = shadow + Math.Max(0, -_shadowOffsetX);
            int rightInset = shadow + Math.Max(0, _shadowOffsetX);
            int topInset = shadow + Math.Max(0, -_shadowOffsetY);
            int bottomInset = shadow + Math.Max(0, _shadowOffsetY);

            return new Rectangle(
                bounds.Left + leftInset,
                bounds.Top + topInset,
                Math.Max(1, bounds.Width - leftInset - rightInset),
                Math.Max(1, bounds.Height - topInset - bottomInset));
        }

        private void DrawButtonShadow(Graphics graphics, Rectangle bodyBounds)
        {
            int shadow = Math.Max(0, _shadow);
            if (shadow <= 0 || _shadowOpacity <= 0f)
            {
                return;
            }

            for (int layer = shadow; layer >= 1; layer--)
            {
                int alpha = (int)(255f * _shadowOpacity * layer / shadow / 2f);
                if (alpha <= 0)
                {
                    continue;
                }

                Rectangle shadowBounds = new Rectangle(
                    bodyBounds.Left + _shadowOffsetX - layer / 2,
                    bodyBounds.Top + _shadowOffsetY - layer / 2,
                    bodyBounds.Width + layer,
                    bodyBounds.Height + layer);

                using (GraphicsPath path = CreateRoundRectanglePath(shadowBounds, GetActualCornerRadius(bodyBounds) + layer / 2))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, _shadowColor)))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private Color GetCurrentCommandBackColor()
        {
            if (_commandState)
            {
                return _pressedBackColor;
            }

            if (_mousePressed)
            {
                return _pressedBackColor;
            }

            if (_mouseHover)
            {
                return _hoverBackColor;
            }

            return _commandOffBackColor;
        }

        private void DrawSimpleLampArea(Graphics graphics, Rectangle lampAreaBounds, int sensorCount, Rectangle buttonBounds)
        {
            if (lampAreaBounds.Width <= 0 || lampAreaBounds.Height <= 0)
            {
                return;
            }

            DrawLampBackground(graphics, lampAreaBounds, buttonBounds);

            Rectangle firstBounds;
            Rectangle secondBounds;
            GetSimpleLampBounds(lampAreaBounds, sensorCount, out firstBounds, out secondBounds);
            DrawSensorIndicator(graphics, firstBounds, _sensor1State);
            if (sensorCount == 2)
            {
                DrawSensorIndicator(graphics, secondBounds, _sensor2State);
            }
        }

        private void DrawLampBackground(Graphics graphics, Rectangle lampAreaBounds, Rectangle buttonBounds)
        {
            int horizontalPadding = Math.Max(4, _sensorLeftPadding);
            int backgroundLeft = Math.Max(buttonBounds.Left, lampAreaBounds.Left - horizontalPadding);
            int backgroundRight = Math.Min(
                buttonBounds.Right,
                lampAreaBounds.Right + Math.Max(4, _sensorTextSpacing / 2));
            Rectangle backgroundBounds = new Rectangle(
                backgroundLeft,
                buttonBounds.Top,
                Math.Max(1, backgroundRight - backgroundLeft),
                buttonBounds.Height);

            using (GraphicsPath path = CreateLeftRoundRectanglePath(backgroundBounds, GetActualCornerRadius(buttonBounds)))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(245, Color.White)))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void GetSimpleLampBounds(Rectangle lampAreaBounds, int sensorCount, out Rectangle firstBounds, out Rectangle secondBounds)
        {
            int lampSize = GetLampSize(lampAreaBounds.Height, sensorCount);
            int x = lampAreaBounds.Left + (lampAreaBounds.Width - lampSize) / 2;

            if (sensorCount == 1)
            {
                firstBounds = new Rectangle(x, lampAreaBounds.Top + (lampAreaBounds.Height - lampSize) / 2, lampSize, lampSize);
                secondBounds = Rectangle.Empty;
                return;
            }

            int totalHeight = lampSize * 2 + 5;
            int firstY = lampAreaBounds.Top + (lampAreaBounds.Height - totalHeight) / 2;
            firstBounds = new Rectangle(x, firstY, lampSize, lampSize);
            secondBounds = new Rectangle(x, firstY + lampSize + 5, lampSize, lampSize);
        }

        private static int GetLampSize(int availableHeight, int sensorCount)
        {
            int size = sensorCount == 2 ? (availableHeight - 5) / 2 : availableHeight - 10;
            return Math.Max(6, Math.Min(12, size));
        }

        private void DrawSimpleButtonText(Graphics graphics, Rectangle textBounds)
        {
            string text = ButtonText;
            Color textColor = GetCurrentCommandForeColor();
            Font textFont = GetScaledFont(graphics, Font, text, textBounds, FontStyle.Regular, 1.0f);

            try
            {
                TextRenderer.DrawText(
                    graphics,
                    text,
                    textFont,
                    textBounds,
                    textColor,
                    TextFormatFlags.HorizontalCenter
                        | TextFormatFlags.VerticalCenter
                        | TextFormatFlags.SingleLine
                        | TextFormatFlags.EndEllipsis);
            }
            finally
            {
                if (!object.ReferenceEquals(textFont, Font))
                {
                    textFont.Dispose();
                }
            }
        }

        private Color GetCurrentCommandForeColor()
        {
            if (_commandState)
            {
                return _commandOnForeColor;
            }

            if (_mouseHover || _mousePressed)
            {
                return _hoverForeColor;
            }

            return _commandOffForeColor;
        }

        private void DrawBackground(Graphics graphics)
        {
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        private void DrawSensorIndicator(Graphics graphics, Rectangle indicatorBounds, bool state)
        {
            Color fillColor = state ? _sensorOnColor : _sensorOffColor;
            Color borderColor = state ? BlendColor(_sensorOnColor, Color.Black, 0.12f) : _sensorBorderColor;

            using (GraphicsPath path = CreateSensorPath(indicatorBounds))
            using (SolidBrush brush = new SolidBrush(fillColor))
            using (Pen borderPen = new Pen(borderColor))
            {
                graphics.FillPath(brush, path);
                graphics.DrawPath(borderPen, path);
            }
        }

        private int GetVisibleSensorCount()
        {
            switch (_sensorMode)
            {
                case SensorDisplayMode.One:
                    return 1;
                case SensorDisplayMode.Two:
                    return 2;
                case SensorDisplayMode.None:
                default:
                    return 0;
            }
        }

        private int GetActualCornerRadius(Rectangle bounds)
        {
            return Math.Min(_cornerRadius, Math.Min(bounds.Width, bounds.Height) / 2);
        }

        private GraphicsPath CreateSensorPath(Rectangle bounds)
        {
            switch (_sensorShape)
            {
                case SensorIndicatorShape.Rectangle:
                    GraphicsPath rectanglePath = new GraphicsPath();
                    rectanglePath.AddRectangle(bounds);
                    rectanglePath.CloseFigure();
                    return rectanglePath;
                case SensorIndicatorShape.RoundedRectangle:
                    return CreateRoundRectanglePath(bounds, Math.Max(2, Math.Min(bounds.Width, bounds.Height) / 4));
                case SensorIndicatorShape.Circle:
                default:
                    GraphicsPath ellipsePath = new GraphicsPath();
                    ellipsePath.AddEllipse(bounds);
                    ellipsePath.CloseFigure();
                    return ellipsePath;
            }
        }

        private Font GetScaledFont(Graphics graphics, Font baseFont, string text, Rectangle bounds, FontStyle style, float scale)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 2 || bounds.Height <= 2)
            {
                return baseFont;
            }

            float targetSize = Math.Max(6f, Math.Min(16f, baseFont.Size * scale));
            Font font = new Font(baseFont.FontFamily, targetSize, style, baseFont.Unit);

            while (targetSize > 6f)
            {
                SizeF textSize = graphics.MeasureString(text, font);
                if (textSize.Width <= bounds.Width - 2 && textSize.Height <= bounds.Height + 2)
                {
                    return font;
                }

                font.Dispose();
                targetSize -= 0.5f;
                font = new Font(baseFont.FontFamily, targetSize, style, baseFont.Unit);
            }

            return font;
        }

        private void SetBoolean(ref bool field, bool value)
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

        private static GraphicsPath CreateLeftRoundRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));

            if (diameter <= 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            Rectangle topArc = new Rectangle(bounds.Left, bounds.Top, diameter, diameter);
            Rectangle bottomArc = new Rectangle(bounds.Left, bounds.Bottom - diameter, diameter, diameter);

            path.StartFigure();
            path.AddLine(bounds.Left + radius, bounds.Top, bounds.Right, bounds.Top);
            path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
            path.AddLine(bounds.Right, bounds.Bottom, bounds.Left + radius, bounds.Bottom);
            path.AddArc(bottomArc, 90, 90);
            path.AddLine(bounds.Left, bounds.Bottom - radius, bounds.Left, bounds.Top + radius);
            path.AddArc(topArc, 180, 90);
            path.CloseFigure();

            return path;
        }
    }
}
