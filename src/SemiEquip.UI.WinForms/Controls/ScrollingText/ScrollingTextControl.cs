using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("Text")]
    [DefaultEvent("Click")]
    [DesignerCategory("Code")]
    public class ScrollingTextControl : Control
    {
        private readonly Timer _scrollTimer;
        private ScrollingTextDirection _scrollDirection = ScrollingTextDirection.RightToLeft;
        private bool _scrollingEnabled = true;
        private int _scrollStep = 2;
        private int _textPadding = 4;
        private int _textX;
        private int _textWidth;
        private bool _positionInitialized;

        public ScrollingTextControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.FromArgb(24, 30, 38);
            ForeColor = Color.FromArgb(235, 239, 244);
            Font = new Font("Segoe UI", 10f, FontStyle.Bold, GraphicsUnit.Point);
            Size = new Size(360, 42);
            Text = "SemiEquip.UI.WinForms 滚动文字控件";

            _scrollTimer = new Timer
            {
                Interval = 30
            };
            _scrollTimer.Tick += OnScrollTimerTick;
            _scrollTimer.Start();
        }

        [Category("滚动文字")]
        [Description("文字水平滚动方向。")]
        [DefaultValue(ScrollingTextDirection.RightToLeft)]
        public ScrollingTextDirection ScrollDirection
        {
            get { return _scrollDirection; }
            set
            {
                if (_scrollDirection == value)
                {
                    return;
                }

                _scrollDirection = value;
                ResetScrollPosition();
            }
        }

        [Category("滚动文字")]
        [Description("是否启用文字滚动。关闭后文字居中显示。")]
        [DefaultValue(true)]
        public bool ScrollingEnabled
        {
            get { return _scrollingEnabled; }
            set
            {
                if (_scrollingEnabled == value)
                {
                    return;
                }

                _scrollingEnabled = value;
                if (_scrollingEnabled)
                {
                    _scrollTimer.Start();
                    ResetScrollPosition();
                }
                else
                {
                    _scrollTimer.Stop();
                    Invalidate();
                }
            }
        }

        [Category("滚动文字")]
        [Description("每次滚动移动的像素数，数值越大滚动越快。")]
        [DefaultValue(2)]
        public int ScrollStep
        {
            get { return _scrollStep; }
            set
            {
                _scrollStep = Math.Max(1, value);
                Invalidate();
            }
        }

        [Category("滚动文字")]
        [Description("滚动刷新间隔，单位为毫秒，数值越小滚动越快。")]
        [DefaultValue(30)]
        public int ScrollInterval
        {
            get { return _scrollTimer.Interval; }
            set { _scrollTimer.Interval = Math.Max(10, value); }
        }

        [Category("滚动文字")]
        [Description("文字与控件边界之间的间距，单位为像素。")]
        [DefaultValue(4)]
        public int TextPadding
        {
            get { return _textPadding; }
            set
            {
                _textPadding = Math.Max(0, value);
                ResetScrollPosition();
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            ResetScrollPosition();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            ResetScrollPosition();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResetScrollPosition();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            using (SolidBrush backgroundBrush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(backgroundBrush, ClientRectangle);
            }

            string displayText = Text;
            if (string.IsNullOrEmpty(displayText))
            {
                return;
            }

            Size textSize = MeasureDisplayText(displayText);
            _textWidth = textSize.Width;

            if (!_positionInitialized)
            {
                InitializeTextPosition();
            }

            Rectangle textBounds = _scrollingEnabled
                ? new Rectangle(_textX, 0, Math.Max(1, textSize.Width + _textPadding * 2), Height)
                : new Rectangle(_textPadding, 0, Math.Max(1, Width - _textPadding * 2), Height);

            TextFormatFlags flags = TextFormatFlags.VerticalCenter
                | TextFormatFlags.SingleLine
                | TextFormatFlags.NoPadding
                | TextFormatFlags.EndEllipsis;

            if (!_scrollingEnabled)
            {
                flags |= TextFormatFlags.HorizontalCenter;
            }

            TextRenderer.DrawText(graphics, displayText, Font, textBounds, ForeColor, flags);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scrollTimer.Tick -= OnScrollTimerTick;
                _scrollTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnScrollTimerTick(object sender, EventArgs e)
        {
            if (!_scrollingEnabled)
            {
                return;
            }

            if (!_positionInitialized)
            {
                InitializeTextPosition();
            }

            if (_scrollDirection == ScrollingTextDirection.RightToLeft)
            {
                _textX -= _scrollStep;
                if (_textX + _textWidth + _textPadding * 2 < 0)
                {
                    _textX = Width;
                }
            }
            else
            {
                _textX += _scrollStep;
                if (_textX > Width)
                {
                    _textX = -_textWidth - _textPadding * 2;
                }
            }

            Invalidate();
        }

        private void ResetScrollPosition()
        {
            _positionInitialized = false;
            Invalidate();
        }

        private void InitializeTextPosition()
        {
            Size textSize = MeasureDisplayText(Text);
            _textWidth = textSize.Width;
            _textX = _scrollDirection == ScrollingTextDirection.RightToLeft
                ? Width
                : -_textWidth - _textPadding * 2;
            _positionInitialized = true;
        }

        private Size MeasureDisplayText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            return TextRenderer.MeasureText(text, Font, new Size(int.MaxValue, Math.Max(1, Height)), TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
        }
    }
}
