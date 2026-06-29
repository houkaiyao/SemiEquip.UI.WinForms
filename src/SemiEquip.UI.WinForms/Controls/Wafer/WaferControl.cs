using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("State")]
    [DefaultEvent("Click")]
    [DesignerCategory("Code")]
    public class WaferControl : Control
    {
        private const int DefaultControlSize = 120;

        private WaferState _state;
        private int _contentPadding = 8;
        private int _borderWidth = 2;
        private Color _emptyWaferColor = Color.White;
        private Color _processingWaferColor = Color.FromArgb(55, 137, 255);
        private Color _completedWaferColor = Color.FromArgb(46, 184, 92);
        private Color _borderColor = Color.FromArgb(80, 104, 132);

        public WaferControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Size = new Size(DefaultControlSize, DefaultControlSize);
            MinimumSize = new Size(20, 20);
        }

        [Category("Wafer")]
        [Description("Wafer 当前状态。Empty 为无料，Processing 为制程中，Completed 为制成结束。")]
        [DefaultValue(WaferState.Empty)]
        public WaferState State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
                Invalidate();
            }
        }

        [Category("Wafer")]
        [Description("控件内容区域与边界之间的间距，单位为像素。")]
        [DefaultValue(8)]
        public int ContentPadding
        {
            get { return _contentPadding; }
            set
            {
                int normalized = Math.Max(0, value);
                if (_contentPadding == normalized)
                {
                    return;
                }

                _contentPadding = normalized;
                Invalidate();
            }
        }

        [Category("Wafer")]
        [Description("Wafer 外圈边框宽度，单位为像素。")]
        [DefaultValue(2)]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                int normalized = Math.Max(1, value);
                if (_borderWidth == normalized)
                {
                    return;
                }

                _borderWidth = normalized;
                Invalidate();
            }
        }

        [Category("Wafer Colors")]
        [Description("无料状态下的 Wafer 面颜色。")]
        public Color EmptyWaferColor
        {
            get { return _emptyWaferColor; }
            set { SetColor(ref _emptyWaferColor, value); }
        }

        [Category("Wafer Colors")]
        [Description("制程中状态下的 Wafer 面颜色。")]
        public Color ProcessingWaferColor
        {
            get { return _processingWaferColor; }
            set { SetColor(ref _processingWaferColor, value); }
        }

        [Category("Wafer Colors")]
        [Description("制成结束状态下的 Wafer 面颜色。")]
        public Color CompletedWaferColor
        {
            get { return _completedWaferColor; }
            set { SetColor(ref _completedWaferColor, value); }
        }

        [Category("Wafer Colors")]
        [Description("Wafer 外圈边框颜色。")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { SetColor(ref _borderColor, value); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBackground(graphics);
            DrawWafer(graphics);
        }

        private void DrawBackground(Graphics graphics)
        {
            if (BackColor == Color.Transparent)
            {
                return;
            }

            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        private void DrawWafer(Graphics graphics)
        {
            Rectangle contentBounds = GetContentBounds();
            int size = Math.Min(contentBounds.Width, contentBounds.Height);
            if (size <= 0)
            {
                return;
            }

            Rectangle waferRect = new Rectangle(
                contentBounds.Left + (contentBounds.Width - size) / 2,
                contentBounds.Top + (contentBounds.Height - size) / 2,
                size,
                size);

            if (waferRect.Width <= 0 || waferRect.Height <= 0)
            {
                return;
            }

            Color baseColor = ResolveStateColor();
            Color lightColor = BlendColor(baseColor, Color.White, _state == WaferState.Empty ? 0.16f : 0.42f);
            Color darkColor = BlendColor(baseColor, Color.FromArgb(80, 104, 132), _state == WaferState.Empty ? 0.18f : 0.30f);

            using (LinearGradientBrush waferBrush = new LinearGradientBrush(
                waferRect,
                lightColor,
                darkColor,
                45F))
            using (Pen outerPen = new Pen(_borderColor, _borderWidth))
            {
                graphics.FillEllipse(waferBrush, waferRect);
                graphics.DrawEllipse(outerPen, waferRect);
            }
        }

        private Rectangle GetContentBounds()
        {
            int padding = _contentPadding;
            return new Rectangle(
                padding,
                padding,
                Math.Max(0, Width - padding * 2),
                Math.Max(0, Height - padding * 2));
        }

        private Color ResolveStateColor()
        {
            switch (_state)
            {
                case WaferState.Processing:
                    return _processingWaferColor;
                case WaferState.Completed:
                    return _completedWaferColor;
                case WaferState.Empty:
                default:
                    return _emptyWaferColor;
            }
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
            int a = source.A + (int)((target.A - source.A) * amount);
            int r = source.R + (int)((target.R - source.R) * amount);
            int g = source.G + (int)((target.G - source.G) * amount);
            int b = source.B + (int)((target.B - source.B) * amount);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
