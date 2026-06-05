using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("LightColor")]
    [DefaultEvent("Click")]
    [DesignerCategory("Code")]
    public class StatusLightControl : Control
    {
        private const int DefaultSizeValue = 80;

        private bool _maintainAspectRatio = true;
        private int _lightPadding;
        private Color _lightColor = Color.FromArgb(46, 184, 92);
        private Color _lightBorderColor = Color.Transparent;
        private Color _frameColor = Color.Transparent;
        private Color _shadowColor = Color.FromArgb(120, 0, 0, 0);
        private Color _highlightColor = Color.FromArgb(150, 255, 255, 255);

        public StatusLightControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.FromArgb(24, 29, 36);
            ForeColor = Color.FromArgb(235, 239, 244);
            Size = new Size(DefaultSizeValue, DefaultSizeValue);
        }

        [Category("状态灯")]
        [Description("圆形灯当前显示的颜色，可在运行时动态修改。")]
        public Color LightColor
        {
            get { return _lightColor; }
            set
            {
                if (_lightColor == value)
                {
                    return;
                }

                _lightColor = value;
                Invalidate();
            }
        }

        [Category("状态灯")]
        [Description("调整控件大小时保持整体外形为正方形。")]
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
                    int size = Math.Max(1, Width);
                    Size = new Size(size, size);
                }

                Invalidate();
            }
        }

        [Category("状态灯")]
        [Description("圆形灯与正方形外框之间的间距，单位为像素。")]
        [DefaultValue(0)]
        public int LightPadding
        {
            get { return _lightPadding; }
            set
            {
                _lightPadding = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("状态灯颜色")]
        [Description("圆形灯边框颜色。")]
        public Color LightBorderColor
        {
            get { return _lightBorderColor; }
            set
            {
                _lightBorderColor = value;
                Invalidate();
            }
        }

        [Category("状态灯颜色")]
        [Description("正方形外框颜色。")]
        public Color FrameColor
        {
            get { return _frameColor; }
            set
            {
                _frameColor = value;
                Invalidate();
            }
        }

        [Category("状态灯颜色")]
        [Description("圆形灯暗部阴影颜色，用于增强层次感。")]
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set
            {
                _shadowColor = value;
                Invalidate();
            }
        }

        [Category("状态灯颜色")]
        [Description("圆形灯高光颜色，用于增强灯泡质感。")]
        public Color HighlightColor
        {
            get { return _highlightColor; }
            set
            {
                _highlightColor = value;
                Invalidate();
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (_maintainAspectRatio && Dock == DockStyle.None)
            {
                if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width
                    && (specified & BoundsSpecified.Height) != BoundsSpecified.Height)
                {
                    height = Math.Max(1, width);
                }
                else if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height
                    && (specified & BoundsSpecified.Width) != BoundsSpecified.Width)
                {
                    width = Math.Max(1, height);
                }
                else if ((specified & BoundsSpecified.Size) == BoundsSpecified.Size)
                {
                    height = Math.Max(1, width);
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawFrame(graphics);
            DrawLight(graphics);
        }

        private void DrawFrame(Graphics graphics)
        {
            Rectangle frameBounds = ClientRectangle;
            frameBounds.Width -= 1;
            frameBounds.Height -= 1;

            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(brush, ClientRectangle);

                if (_frameColor.A > 0)
                {
                    using (Pen pen = new Pen(_frameColor))
                    {
                        graphics.DrawRectangle(pen, frameBounds);
                    }
                }
            }
        }

        private void DrawLight(Graphics graphics)
        {
            Rectangle lightBounds = GetLightBounds();
            if (lightBounds.Width <= 0 || lightBounds.Height <= 0)
            {
                return;
            }

            Rectangle shadowBounds = lightBounds;
            shadowBounds.Inflate(-Math.Max(1, lightBounds.Width / 18), -Math.Max(1, lightBounds.Height / 18));
            shadowBounds.Offset(Math.Max(1, lightBounds.Width / 18), Math.Max(1, lightBounds.Height / 18));

            Rectangle highlightBounds = lightBounds;
            highlightBounds.Width = Math.Max(1, lightBounds.Width / 2);
            highlightBounds.Height = Math.Max(1, lightBounds.Height / 2);
            highlightBounds.Offset(Math.Max(1, lightBounds.Width / 8), Math.Max(1, lightBounds.Height / 8));

            using (GraphicsPath path = new GraphicsPath())
            using (GraphicsPath shadowPath = new GraphicsPath())
            using (GraphicsPath highlightPath = new GraphicsPath())
            using (SolidBrush fillBrush = new SolidBrush(_lightColor))
            using (PathGradientBrush shadowBrush = CreateEllipseGradientBrush(shadowBounds, _shadowColor, Color.Transparent))
            using (PathGradientBrush highlightBrush = CreateEllipseGradientBrush(highlightBounds, _highlightColor, Color.Transparent))
            {
                path.AddEllipse(lightBounds);
                graphics.FillPath(fillBrush, path);

                shadowPath.AddEllipse(shadowBounds);
                graphics.FillPath(shadowBrush, shadowPath);

                highlightPath.AddEllipse(highlightBounds);
                graphics.FillPath(highlightBrush, highlightPath);

                if (_lightBorderColor.A > 0)
                {
                    using (Pen borderPen = new Pen(_lightBorderColor))
                    {
                        graphics.DrawPath(borderPen, path);
                    }
                }
            }
        }

        private Rectangle GetLightBounds()
        {
            int sideLength = Math.Min(Width, Height);
            int padding = Math.Min(_lightPadding, Math.Max(0, (sideLength - 1) / 2));
            int lightSize = Math.Max(1, sideLength - padding * 2);
            int x = (Width - lightSize) / 2;
            int y = (Height - lightSize) / 2;

            return new Rectangle(x, y, lightSize - 1, lightSize - 1);
        }

        private static PathGradientBrush CreateEllipseGradientBrush(Rectangle bounds, Color centerColor, Color surroundColor)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(bounds);

            PathGradientBrush brush = new PathGradientBrush(path);
            brush.CenterColor = centerColor;
            brush.SurroundColors = new[] { surroundColor };
            path.Dispose();

            return brush;
        }
    }
}
