using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("SlotCount")]
    [DefaultEvent("SlotClick")]
    [DesignerCategory("Code")]
    public class FoupMapControl : Control
    {
        public const int MaxSlotCount = 25;

        private const int DefaultWidth = 150;

        private readonly FoupSlotCollection _slots;
        private int _slotCount = MaxSlotCount;
        private bool _maintainAspectRatio = true;
        private int _slotCornerRadius = 4;
        private int _slotGap = 3;
        private int _slotNumberWidth;
        private int _contentPadding = 8;
        private bool _showSlotNumbers;
        private bool _showSlotToolTip = true;
        private bool _autoScaleSlotNumberFont = true;
        private float _slotWidthRatio = 0.8f;
        private readonly ToolTip _slotToolTip;
        private int _hoverSlotNumber;
        private Color _slotBorderColor = Color.FromArgb(92, 104, 118);
        private Color _frameColor = Color.FromArgb(54, 65, 78);
        private Color _slotNumberColor = Color.FromArgb(218, 224, 232);
        private Color _emptySlotColor = Color.White;
        private Color _beforeProcessSlotColor = Color.FromArgb(55, 137, 255);
        private Color _afterProcessSlotColor = Color.FromArgb(46, 184, 92);
        private Color _abnormalSlotColor = Color.FromArgb(226, 64, 64);

        public FoupMapControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.FromArgb(24, 29, 36);
            ForeColor = Color.FromArgb(235, 239, 244);
            Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point);
            Size = new Size(DefaultWidth, DefaultWidth * 2);

            _slots = new FoupSlotCollection(this);
            _slotToolTip = new ToolTip();
            ResetSlots();
        }

        public event EventHandler<FoupSlotClickEventArgs> SlotClick;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FoupSlotCollection Slots
        {
            get { return _slots; }
        }

        [Category("FOUP Map")]
        [Description("需要绘制的 FOUP Slot 数量，有效范围为 1 到 25。")]
        [DefaultValue(MaxSlotCount)]
        public int SlotCount
        {
            get { return _slotCount; }
            set
            {
                int normalized = Math.Max(1, Math.Min(MaxSlotCount, value));
                if (_slotCount == normalized)
                {
                    return;
                }

                _slotCount = normalized;
                ResetSlots();
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("调整控件大小时保持高度:宽度为 2:1。")]
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
                    Height = Math.Max(2, Width * 2);
                }

                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("是否在每个 Slot 左侧显示 Slot 编号。")]
        [DefaultValue(false)]
        public bool ShowSlotNumbers
        {
            get { return _showSlotNumbers; }
            set
            {
                if (_showSlotNumbers == value)
                {
                    return;
                }

                _showSlotNumbers = value;
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("鼠标悬浮到 Slot 上时，是否通过 Tooltip 显示 Slot 编号。")]
        [DefaultValue(true)]
        public bool ShowSlotToolTip
        {
            get { return _showSlotToolTip; }
            set
            {
                if (_showSlotToolTip == value)
                {
                    return;
                }

                _showSlotToolTip = value;
                if (!_showSlotToolTip)
                {
                    _slotToolTip.Hide(this);
                    _hoverSlotNumber = 0;
                }
            }
        }

        [Category("FOUP Map")]
        [Description("自动缩小 Slot 编号字体，避免在较薄 Slot 中出现文字裁剪。")]
        [DefaultValue(true)]
        public bool AutoScaleSlotNumberFont
        {
            get { return _autoScaleSlotNumberFont; }
            set
            {
                if (_autoScaleSlotNumberFont == value)
                {
                    return;
                }

                _autoScaleSlotNumberFont = value;
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("控件内部内容区域的外边距，单位为像素。")]
        [DefaultValue(8)]
        public int ContentPadding
        {
            get { return _contentPadding; }
            set
            {
                _contentPadding = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("Slot 之间的基础间距，单位为像素。")]
        [DefaultValue(3)]
        public int SlotGap
        {
            get { return _slotGap; }
            set
            {
                _slotGap = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("Slot 主体宽度占控件宽度的比例，默认 0.8，约为控件宽度的五分之四。")]
        [DefaultValue(0.8f)]
        public float SlotWidthRatio
        {
            get { return _slotWidthRatio; }
            set
            {
                float normalized = Math.Max(0.1f, Math.Min(1.0f, value));
                if (Math.Abs(_slotWidthRatio - normalized) < 0.0001f)
                {
                    return;
                }

                _slotWidthRatio = normalized;
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("左侧 Slot 编号区域的最小宽度。默认 0 表示按左1、中8、右1的比例自动计算。")]
        [DefaultValue(0)]
        public int SlotNumberWidth
        {
            get { return _slotNumberWidth; }
            set
            {
                _slotNumberWidth = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("每个扁长 Slot 图形使用的圆角半径。")]
        [DefaultValue(4)]
        public int SlotCornerRadius
        {
            get { return _slotCornerRadius; }
            set
            {
                _slotCornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("FOUP Colors")]
        [Description("异常片 Slot 使用的颜色。")]
        public Color AbnormalSlotColor
        {
            get { return _abnormalSlotColor; }
            set
            {
                _abnormalSlotColor = value;
                ApplyStateColor(FoupSlotState.Abnormal, value);
            }
        }

        [Category("FOUP Colors")]
        [Description("制程前 Slot 使用的颜色。")]
        public Color BeforeProcessSlotColor
        {
            get { return _beforeProcessSlotColor; }
            set
            {
                _beforeProcessSlotColor = value;
                ApplyStateColor(FoupSlotState.BeforeProcess, value);
            }
        }

        [Category("FOUP Colors")]
        [Description("制程后 Slot 使用的颜色。")]
        public Color AfterProcessSlotColor
        {
            get { return _afterProcessSlotColor; }
            set
            {
                _afterProcessSlotColor = value;
                ApplyStateColor(FoupSlotState.AfterProcess, value);
            }
        }

        [Category("FOUP Colors")]
        [Description("无片 Slot 使用的颜色。")]
        public Color EmptySlotColor
        {
            get { return _emptySlotColor; }
            set
            {
                _emptySlotColor = value;
                ApplyStateColor(FoupSlotState.Empty, value);
            }
        }

        [Category("FOUP Colors")]
        [Description("Slot 边框颜色。")]
        public Color SlotBorderColor
        {
            get { return _slotBorderColor; }
            set
            {
                _slotBorderColor = value;
                Invalidate();
            }
        }

        [Category("FOUP Colors")]
        [Description("控件外框颜色。")]
        public Color FrameColor
        {
            get { return _frameColor; }
            set
            {
                _frameColor = value;
                Invalidate();
            }
        }

        [Category("FOUP Colors")]
        [Description("Slot 编号文字颜色。")]
        public Color SlotNumberColor
        {
            get { return _slotNumberColor; }
            set
            {
                _slotNumberColor = value;
                Invalidate();
            }
        }

        public void SetSlotColor(int slotNumber, Color color)
        {
            FoupSlotInfo slot = GetSlot(slotNumber);
            slot.Color = color;
            slot.State = FoupSlotState.Custom;
            Invalidate();
        }

        public Color GetSlotColor(int slotNumber)
        {
            return GetSlot(slotNumber).Color;
        }

        public void SetSlotState(int slotNumber, FoupSlotState state)
        {
            FoupSlotInfo slot = GetSlot(slotNumber);
            slot.State = state;
            slot.Color = ResolveStateColor(state, slot.Color);
            Invalidate();
        }

        public FoupSlotState GetSlotState(int slotNumber)
        {
            return GetSlot(slotNumber).State;
        }

        public Rectangle GetSlotBounds(int slotNumber)
        {
            ValidateSlotNumber(slotNumber);
            return CalculateSlotBounds(slotNumber);
        }

        public void ClearSlots()
        {
            foreach (FoupSlotInfo slot in _slots)
            {
                slot.State = FoupSlotState.Empty;
                slot.Color = _emptySlotColor;
            }

            Invalidate();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (_maintainAspectRatio && Dock == DockStyle.None)
            {
                if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width
                    && (specified & BoundsSpecified.Height) != BoundsSpecified.Height)
                {
                    height = Math.Max(2, width * 2);
                }
                else if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height
                    && (specified & BoundsSpecified.Width) != BoundsSpecified.Width)
                {
                    width = Math.Max(1, height / 2);
                }
                else if ((specified & BoundsSpecified.Size) == BoundsSpecified.Size)
                {
                    height = Math.Max(2, width * 2);
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawFrame(graphics);

            for (int slotNumber = 1; slotNumber <= _slotCount; slotNumber++)
            {
                DrawSlot(graphics, slotNumber);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int slotNumber = HitTest(e.Location);
            if (slotNumber > 0)
            {
                OnSlotClick(new FoupSlotClickEventArgs(slotNumber, e.Button, e.Location));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateSlotToolTip(e.Location);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverSlotNumber = 0;
            _slotToolTip.Hide(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _slotToolTip.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void OnSlotClick(FoupSlotClickEventArgs e)
        {
            EventHandler<FoupSlotClickEventArgs> handler = SlotClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void DrawFrame(Graphics graphics)
        {
            Rectangle frameBounds = ClientRectangle;
            frameBounds.Width -= 1;
            frameBounds.Height -= 1;

            using (SolidBrush brush = new SolidBrush(BackColor))
            using (Pen pen = new Pen(_frameColor))
            {
                graphics.FillRectangle(brush, ClientRectangle);
                graphics.DrawRectangle(pen, frameBounds);
            }
        }

        private void DrawSlot(Graphics graphics, int slotNumber)
        {
            Rectangle slotBounds = CalculateSlotBounds(slotNumber);
            if (slotBounds.Width <= 0 || slotBounds.Height <= 0)
            {
                return;
            }

            if (_showSlotNumbers)
            {
                Rectangle numberBounds = CalculateSlotNumberBounds(slotBounds);

                using (Font slotNumberFont = CreateSlotNumberFont(graphics, numberBounds))
                {
                    TextRenderer.DrawText(
                        graphics,
                        slotNumber.ToString("00"),
                        slotNumberFont,
                        numberBounds,
                        _slotNumberColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }

            using (GraphicsPath path = CreateRoundRectanglePath(slotBounds, _slotCornerRadius))
            using (SolidBrush fillBrush = new SolidBrush(GetSlot(slotNumber).Color))
            using (Pen borderPen = new Pen(_slotBorderColor))
            {
                graphics.FillPath(fillBrush, path);
                graphics.DrawPath(borderPen, path);
            }
        }

        private Rectangle CalculateSlotBounds(int slotNumber)
        {
            Rectangle contentBounds = GetContentBounds();
            int slotAreaWidth = CalculateSlotAreaWidth(contentBounds.Width);
            int leftAreaWidth = CalculateLeftAreaWidth(contentBounds.Width, slotAreaWidth);
            int slotAreaX = contentBounds.X + leftAreaWidth;
            int slotHeight = CalculateSlotHeightForFullMap(contentBounds.Height);
            int slotGap = CalculateDynamicSlotGap(contentBounds.Height, slotHeight);
            int usedHeight = slotHeight * _slotCount + Math.Max(0, _slotCount - 1) * slotGap;
            int topOffset = Math.Max(0, (contentBounds.Height - usedHeight) / 2);
            int slotIndexFromTop = _slotCount - slotNumber;
            int slotY = contentBounds.Top + topOffset + slotIndexFromTop * (slotHeight + slotGap);

            return new Rectangle(slotAreaX, slotY, slotAreaWidth, slotHeight);
        }

        private Rectangle CalculateSlotNumberBounds(Rectangle slotBounds)
        {
            Rectangle contentBounds = GetContentBounds();
            int slotAreaWidth = CalculateSlotAreaWidth(contentBounds.Width);
            int leftAreaWidth = CalculateLeftAreaWidth(contentBounds.Width, slotAreaWidth);

            return new Rectangle(
                contentBounds.X,
                slotBounds.Top,
                leftAreaWidth,
                slotBounds.Height);
        }

        private int CalculateSlotAreaWidth(int contentWidth)
        {
            return Math.Max(1, (int)Math.Round(contentWidth * _slotWidthRatio));
        }

        private int CalculateLeftAreaWidth(int contentWidth, int slotAreaWidth)
        {
            int ratioWidth = Math.Max(1, (contentWidth - slotAreaWidth) / 2);
            if (!_showSlotNumbers || _slotNumberWidth <= 0)
            {
                return ratioWidth;
            }

            int maximumLeftWidth = Math.Max(1, contentWidth - slotAreaWidth);
            return Math.Min(maximumLeftWidth, Math.Max(ratioWidth, _slotNumberWidth));
        }

        private Rectangle GetContentBounds()
        {
            int padding = _contentPadding;
            return new Rectangle(
                padding,
                padding,
                Math.Max(1, Width - padding * 2),
                Math.Max(1, Height - padding * 2));
        }

        private int CalculateSlotHeightForFullMap(int contentHeight)
        {
            int referenceGap = Math.Max(0, MaxSlotCount - 1) * _slotGap;
            return Math.Max(1, (contentHeight - referenceGap) / MaxSlotCount);
        }

        private int CalculateDynamicSlotGap(int contentHeight, int slotHeight)
        {
            if (_slotCount <= 1)
            {
                return 0;
            }

            int remainingHeight = contentHeight - slotHeight * _slotCount;
            return Math.Max(_slotGap, remainingHeight / (_slotCount - 1));
        }

        private Font CreateSlotNumberFont(Graphics graphics, Rectangle textBounds)
        {
            if (!_autoScaleSlotNumberFont)
            {
                return (Font)Font.Clone();
            }

            float fontSize = Font.Size;
            float minimumSize = Math.Min(3.5f, fontSize);
            Size targetSize = new Size(Math.Max(1, textBounds.Width), Math.Max(1, textBounds.Height));
            TextFormatFlags measureFlags = TextFormatFlags.Right
                | TextFormatFlags.VerticalCenter
                | TextFormatFlags.EndEllipsis
                | TextFormatFlags.NoPadding
                | TextFormatFlags.SingleLine;

            while (fontSize > minimumSize)
            {
                using (Font testFont = new Font(Font.FontFamily, fontSize, Font.Style, Font.Unit))
                {
                    Size measuredSize = TextRenderer.MeasureText(
                        graphics,
                        "25",
                        testFont,
                        targetSize,
                        measureFlags);

                    if (measuredSize.Height <= targetSize.Height
                        && measuredSize.Width <= targetSize.Width)
                    {
                        break;
                    }
                }

                fontSize -= 0.25f;
            }

            return new Font(Font.FontFamily, fontSize, Font.Style, Font.Unit);
        }

        private int HitTest(Point location)
        {
            for (int slotNumber = 1; slotNumber <= _slotCount; slotNumber++)
            {
                if (CalculateSlotBounds(slotNumber).Contains(location))
                {
                    return slotNumber;
                }
            }

            return 0;
        }

        private void UpdateSlotToolTip(Point location)
        {
            if (!_showSlotToolTip)
            {
                return;
            }

            int slotNumber = HitTest(location);
            if (slotNumber == _hoverSlotNumber)
            {
                return;
            }

            _hoverSlotNumber = slotNumber;
            if (slotNumber <= 0)
            {
                _slotToolTip.Hide(this);
                return;
            }

            _slotToolTip.Show(
                string.Format("Slot {0:00}", slotNumber),
                this,
                location.X + 12,
                location.Y + 12,
                2000);
        }

        private FoupSlotInfo GetSlot(int slotNumber)
        {
            ValidateSlotNumber(slotNumber);

            FoupSlotInfo slot = _slots.FirstOrDefault(item => item.SlotNumber == slotNumber);
            if (slot == null)
            {
                slot = new FoupSlotInfo(slotNumber, FoupSlotState.Empty, _emptySlotColor);
                _slots.Add(slot);
            }

            return slot;
        }

        private void ValidateSlotNumber(int slotNumber)
        {
            if (slotNumber < 1 || slotNumber > _slotCount)
            {
                throw new ArgumentOutOfRangeException("slotNumber", "Slot 编号必须在当前 SlotCount 范围内。");
            }
        }

        private void ResetSlots()
        {
            _slots.Clear();

            for (int slotNumber = 1; slotNumber <= _slotCount; slotNumber++)
            {
                _slots.Add(new FoupSlotInfo(slotNumber, FoupSlotState.Empty, _emptySlotColor));
            }
        }

        private void ApplyStateColor(FoupSlotState state, Color color)
        {
            foreach (FoupSlotInfo slot in _slots.Where(item => item.State == state))
            {
                slot.Color = color;
            }

            Invalidate();
        }

        private Color ResolveStateColor(FoupSlotState state, Color customFallback)
        {
            switch (state)
            {
                case FoupSlotState.Abnormal:
                    return _abnormalSlotColor;
                case FoupSlotState.BeforeProcess:
                    return _beforeProcessSlotColor;
                case FoupSlotState.AfterProcess:
                    return _afterProcessSlotColor;
                case FoupSlotState.Empty:
                    return _emptySlotColor;
                case FoupSlotState.Custom:
                default:
                    return customFallback;
            }
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
