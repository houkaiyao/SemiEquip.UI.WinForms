using System;
using System.Collections.Generic;
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
        private int _contentPadding = 5;
        private bool _showSlotNumbers;
        private bool _showSelectionCheckBoxes;
        private bool _showSlotText;
        private bool _showSlotTip = true;
        private bool _autoScaleSlotNumberFont = true;
        private bool _autoScaleSlotTextFont = true;
        private int _slotTextPadding = 4;
        private readonly ToolTip _slotToolTip;
        private int _hoverSlotNumber;
        private Color _slotBorderColor = Color.FromArgb(92, 104, 118);
        private Color _frameColor = Color.FromArgb(180, 186, 194);
        private Color _slotNumberColor = Color.Black;
        private Color _slotTextColor = Color.FromArgb(18, 22, 28);
        private Color _emptySlotColor = Color.White;
        private Color _beforeProcessSlotColor = Color.FromArgb(55, 137, 255);
        private Color _afterProcessSlotColor = Color.FromArgb(46, 184, 92);
        private Color _abnormalSlotColor = Color.FromArgb(226, 64, 64);
        private Font _slotTextFont;

        public FoupMapControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(235, 239, 244);
            Font = new Font("Times New Roman", 8.25f, FontStyle.Regular, GraphicsUnit.Point);
            _slotTextFont = new Font("Times New Roman", 7f, FontStyle.Regular, GraphicsUnit.Point);
            Size = new Size(DefaultWidth, DefaultWidth * 2);
            MinimumSize = new Size(60, 100);

            _slots = new FoupSlotCollection(this);
            _slotToolTip = new ToolTip();
            ResetSlots();
        }

        public event EventHandler<FoupSlotClickEventArgs> SlotClick;

        public event EventHandler ChooseMapDataChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<FoupSlotInfo> Slots
        {
            get { return new List<FoupSlotInfo>(_slots).AsReadOnly(); }
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
                if (_hoverSlotNumber > _slotCount)
                {
                    ResetSlotToolTip();
                }

                Invalidate();
                OnChooseMapDataChanged(EventArgs.Empty);
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
        [Description("是否在每个 Slot 右侧显示选片勾选框。")]
        [DefaultValue(false)]
        public bool ShowSelectionCheckBoxes
        {
            get { return _showSelectionCheckBoxes; }
            set
            {
                if (_showSelectionCheckBoxes == value)
                {
                    return;
                }

                _showSelectionCheckBoxes = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ChooseMapData
        {
            get
            {
                char[] mapping = new char[_slotCount];
                for (int slotNumber = 1; slotNumber <= _slotCount; slotNumber++)
                {
                    mapping[slotNumber - 1] = GetSlot(slotNumber).IsSelected ? '1' : '0';
                }

                return new string(mapping);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Length != _slotCount)
                {
                    throw new ArgumentException("ChooseMapData 长度必须等于当前 SlotCount。", "value");
                }

                for (int index = 0; index < value.Length; index++)
                {
                    if (value[index] != '0' && value[index] != '1')
                    {
                        throw new ArgumentException("ChooseMapData 只能包含字符 0 和 1。", "value");
                    }
                }

                bool changed = false;
                for (int slotNumber = 1; slotNumber <= _slotCount; slotNumber++)
                {
                    bool selected = value[slotNumber - 1] == '1';
                    FoupSlotInfo slot = GetSlot(slotNumber);
                    if (slot.IsSelected != selected)
                    {
                        slot.IsSelected = selected;
                        changed = true;
                    }
                }

                if (changed)
                {
                    Invalidate();
                    OnChooseMapDataChanged(EventArgs.Empty);
                }
            }
        }

        [Category("FOUP Map")]
        [Description("是否显示 SlotText。")]
        [DefaultValue(false)]
        public bool ShowSlotText
        {
            get { return _showSlotText; }
            set
            {
                if (_showSlotText == value)
                {
                    return;
                }

                _showSlotText = value;
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("鼠标悬浮到 Slot 上时，是否显示 SlotTipText。")]
        [DefaultValue(true)]
        public bool ShowSlotTip
        {
            get { return _showSlotTip; }
            set
            {
                if (_showSlotTip == value)
                {
                    return;
                }

                _showSlotTip = value;
                if (!_showSlotTip)
                {
                    ResetSlotToolTip();
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
        [Description("自动缩小 Slot 内显示文字的字体，避免文字超出 Slot 图形。")]
        [DefaultValue(true)]
        public bool AutoScaleSlotTextFont
        {
            get { return _autoScaleSlotTextFont; }
            set
            {
                if (_autoScaleSlotTextFont == value)
                {
                    return;
                }

                _autoScaleSlotTextFont = value;
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("Slot 内显示文字与 Slot 边缘之间的内边距，单位为像素。")]
        [DefaultValue(4)]
        public int SlotTextPadding
        {
            get { return _slotTextPadding; }
            set
            {
                _slotTextPadding = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("FOUP Map")]
        [Description("Slot 内显示文字使用的字体。")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Font SlotTextFont
        {
            get { return _slotTextFont; }
            set
            {
                Font newFont = value == null
                    ? new Font(Font.FontFamily, 7f, FontStyle.Regular, GraphicsUnit.Point)
                    : (Font)value.Clone();

                if (_slotTextFont != null)
                {
                    _slotTextFont.Dispose();
                }

                _slotTextFont = newFont;
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

        [Category("FOUP Colors")]
        [Description("Slot 内显示文字的颜色。")]
        public Color SlotTextColor
        {
            get { return _slotTextColor; }
            set
            {
                _slotTextColor = value;
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

        public void SetSlotText(int slotNumber, string slotText)
        {
            FoupSlotInfo slot = GetSlot(slotNumber);
            slot.SlotText = slotText ?? string.Empty;
            Invalidate();
        }

        public string GetSlotText(int slotNumber)
        {
            return GetSlot(slotNumber).SlotText;
        }

        public void ClearSlotTexts()
        {
            foreach (FoupSlotInfo slot in _slots)
            {
                slot.SlotText = string.Empty;
            }

            Invalidate();
        }

        public void SetSlotTipText(int slotNumber, string slotTipText)
        {
            FoupSlotInfo slot = GetSlot(slotNumber);
            slot.SlotTipText = slotTipText ?? string.Empty;
            Invalidate();
        }

        public string GetSlotTipText(int slotNumber)
        {
            return GetSlot(slotNumber).SlotTipText;
        }

        public void ClearSlotTipTexts()
        {
            foreach (FoupSlotInfo slot in _slots)
            {
                slot.SlotTipText = string.Empty;
            }

            Invalidate();
        }

        public void SetSlotSelected(int slotNumber, bool selected)
        {
            FoupSlotInfo slot = GetSlot(slotNumber);
            if (slot.IsSelected == selected)
            {
                return;
            }

            slot.IsSelected = selected;
            Invalidate();
            OnChooseMapDataChanged(EventArgs.Empty);
        }

        public bool GetSlotSelected(int slotNumber)
        {
            return GetSlot(slotNumber).IsSelected;
        }

        public void ClearSlotSelections()
        {
            bool changed = false;
            foreach (FoupSlotInfo slot in _slots)
            {
                if (slot.IsSelected)
                {
                    slot.IsSelected = false;
                    changed = true;
                }
            }

            if (changed)
            {
                Invalidate();
                OnChooseMapDataChanged(EventArgs.Empty);
            }
        }

        public Rectangle GetSlotBounds(int slotNumber)
        {
            ValidateSlotNumber(slotNumber);
            return CalculateSlotBounds(slotNumber);
        }

        public void ClearSlots()
        {
            bool selectionChanged = false;
            foreach (FoupSlotInfo slot in _slots)
            {
                slot.State = FoupSlotState.Empty;
                slot.Color = _emptySlotColor;
                slot.SlotText = string.Empty;
                slot.SlotTipText = string.Empty;
                selectionChanged = selectionChanged || slot.IsSelected;
                slot.IsSelected = false;
            }

            Invalidate();
            if (selectionChanged)
            {
                OnChooseMapDataChanged(EventArgs.Empty);
            }
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

            if (e.Button == MouseButtons.Left && _showSelectionCheckBoxes)
            {
                int selectionSlotNumber = HitTestSelectionCheckBox(e.Location);
                if (selectionSlotNumber > 0)
                {
                    SetSlotSelected(selectionSlotNumber, !GetSlotSelected(selectionSlotNumber));
                    return;
                }
            }

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
                if (_slotTextFont != null)
                {
                    _slotTextFont.Dispose();
                }
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

        protected virtual void OnChooseMapDataChanged(EventArgs e)
        {
            EventHandler handler = ChooseMapDataChanged;
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

            using (Pen pen = new Pen(_frameColor))
            {
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

            FoupSlotInfo slot = GetSlot(slotNumber);

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

            using (SolidBrush fillBrush = new SolidBrush(slot.Color))
            using (Pen borderPen = new Pen(_slotBorderColor))
            {
                graphics.FillRectangle(fillBrush, slotBounds);
                int right = Math.Max(slotBounds.Left, slotBounds.Right - 1);
                int bottom = Math.Max(slotBounds.Top, slotBounds.Bottom - 1);
                graphics.DrawLine(borderPen, slotBounds.Left, slotBounds.Top, right, slotBounds.Top);
                graphics.DrawLine(borderPen, slotBounds.Left, slotBounds.Top, slotBounds.Left, bottom);
                graphics.DrawLine(borderPen, right, slotBounds.Top, right, bottom);

                if (slotNumber == 1)
                {
                    graphics.DrawLine(borderPen, slotBounds.Left, bottom, right, bottom);
                }
            }

            string slotText = _showSlotText ? slot.SlotText : string.Empty;
            if (!string.IsNullOrEmpty(slotText))
            {
                DrawSlotText(graphics, slotBounds, slotText);
            }

            if (_showSelectionCheckBoxes)
            {
                DrawSelectionCheckBox(graphics, slotNumber, slot.IsSelected);
            }
        }

        private void DrawSelectionCheckBox(Graphics graphics, int slotNumber, bool isSelected)
        {
            Rectangle checkBoxBounds = CalculateSelectionCheckBoxBounds(slotNumber);
            if (checkBoxBounds.Width <= 2 || checkBoxBounds.Height <= 2)
            {
                if (isSelected)
                {
                    DrawCompactSelectionMark(graphics, slotNumber);
                }

                return;
            }

            using (SolidBrush backBrush = new SolidBrush(Color.White))
            using (Pen borderPen = new Pen(_slotBorderColor))
            {
                graphics.FillRectangle(backBrush, checkBoxBounds);
                graphics.DrawRectangle(
                    borderPen,
                    checkBoxBounds.X,
                    checkBoxBounds.Y,
                    checkBoxBounds.Width - 1,
                    checkBoxBounds.Height - 1);
            }

            if (!isSelected)
            {
                return;
            }

            int left = checkBoxBounds.Left + Math.Max(1, checkBoxBounds.Width / 5);
            int middleX = checkBoxBounds.Left + checkBoxBounds.Width / 2;
            int right = checkBoxBounds.Right - Math.Max(2, checkBoxBounds.Width / 6);
            int middleY = checkBoxBounds.Top + checkBoxBounds.Height / 2;
            int bottom = checkBoxBounds.Bottom - Math.Max(2, checkBoxBounds.Height / 5);
            int top = checkBoxBounds.Top + Math.Max(1, checkBoxBounds.Height / 4);

            using (Pen checkPen = new Pen(_afterProcessSlotColor, Math.Max(1f, checkBoxBounds.Width / 7f)))
            {
                checkPen.StartCap = LineCap.Round;
                checkPen.EndCap = LineCap.Round;
                graphics.DrawLine(checkPen, left, middleY, middleX, bottom);
                graphics.DrawLine(checkPen, middleX, bottom, right, top);
            }
        }

        private void DrawSlotText(Graphics graphics, Rectangle slotBounds, string slotText)
        {
            Rectangle slotTextBounds = Rectangle.Inflate(slotBounds, -_slotTextPadding, 0);
            if (slotTextBounds.Width <= 0 || slotTextBounds.Height <= 0)
            {
                return;
            }

            using (Font displayFont = CreateSlotTextFont(graphics, slotText, slotTextBounds))
            {
                TextRenderer.DrawText(
                    graphics,
                    slotText,
                    displayFont,
                    slotTextBounds,
                    _slotTextColor,
                    TextFormatFlags.HorizontalCenter
                        | TextFormatFlags.VerticalCenter
                        | TextFormatFlags.EndEllipsis
                        | TextFormatFlags.NoPadding
                        | TextFormatFlags.SingleLine);
            }
        }

        private Rectangle CalculateSlotBounds(int slotNumber)
        {
            Rectangle contentBounds = GetContentBounds();
            int leftAreaWidth = CalculateSideAreaWidth(contentBounds.Width);
            int slotAreaWidth = CalculateSlotAreaWidth(contentBounds.Width, leftAreaWidth);
            int slotAreaX = contentBounds.X + leftAreaWidth;

            int slotIndexFromTop = _slotCount - slotNumber;
            int slotTop = contentBounds.Top + slotIndexFromTop * contentBounds.Height / _slotCount;
            int slotBottom = contentBounds.Top + (slotIndexFromTop + 1) * contentBounds.Height / _slotCount;

            return new Rectangle(slotAreaX, slotTop, slotAreaWidth, Math.Max(1, slotBottom - slotTop));
        }

        private Rectangle CalculateSlotNumberBounds(Rectangle slotBounds)
        {
            Rectangle contentBounds = GetContentBounds();
            int leftAreaWidth = CalculateSideAreaWidth(contentBounds.Width);

            return new Rectangle(
                contentBounds.X,
                slotBounds.Top,
                leftAreaWidth,
                slotBounds.Height);
        }

        private Rectangle CalculateSelectionCheckBoxBounds(int slotNumber)
        {
            Rectangle slotBounds = CalculateSlotBounds(slotNumber);
            Rectangle contentBounds = GetContentBounds();
            int rightAreaWidth = CalculateSideAreaWidth(contentBounds.Width);
            int checkBoxSize = Math.Min(slotBounds.Height - 2, rightAreaWidth - 4);

            if (checkBoxSize <= 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                slotBounds.Right + (rightAreaWidth - checkBoxSize) / 2,
                slotBounds.Top + (slotBounds.Height - checkBoxSize) / 2,
                checkBoxSize,
                checkBoxSize);
        }

        private static int CalculateSideAreaWidth(int contentWidth)
        {
            return Math.Max(0, contentWidth / 10);
        }

        private static int CalculateSlotAreaWidth(int contentWidth, int sideAreaWidth)
        {
            return Math.Max(1, contentWidth - sideAreaWidth * 2);
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

        private Font CreateSlotTextFont(Graphics graphics, string slotText, Rectangle textBounds)
        {
            if (!_autoScaleSlotTextFont)
            {
                return (Font)_slotTextFont.Clone();
            }

            float fontSize = _slotTextFont.Size;
            float minimumSize = Math.Min(3.5f, fontSize);
            Size targetSize = new Size(Math.Max(1, textBounds.Width), Math.Max(1, textBounds.Height));
            TextFormatFlags measureFlags = TextFormatFlags.HorizontalCenter
                | TextFormatFlags.VerticalCenter
                | TextFormatFlags.EndEllipsis
                | TextFormatFlags.NoPadding
                | TextFormatFlags.SingleLine;

            while (fontSize > minimumSize)
            {
                using (Font testFont = new Font(_slotTextFont.FontFamily, fontSize, _slotTextFont.Style, _slotTextFont.Unit))
                {
                    Size measuredSize = TextRenderer.MeasureText(
                        graphics,
                        slotText,
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

            return new Font(_slotTextFont.FontFamily, fontSize, _slotTextFont.Style, _slotTextFont.Unit);
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

        private int HitTestSelectionCheckBox(Point location)
        {
            for (int slotNumber = 1; slotNumber <= _slotCount; slotNumber++)
            {
                if (CalculateSelectionCheckBoxBounds(slotNumber).Contains(location))
                {
                    return slotNumber;
                }
            }

            return 0;
        }

        private void UpdateSlotToolTip(Point location)
        {
            if (!_showSlotTip)
            {
                ResetSlotToolTip();
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

            string tipText = CreateSlotToolTipText(slotNumber);
            if (string.IsNullOrEmpty(tipText))
            {
                ResetSlotToolTip();
                return;
            }

            _slotToolTip.Show(
                tipText,
                this,
                location.X + 12,
                location.Y + 12,
                2000);
        }

        private string CreateSlotToolTipText(int slotNumber)
        {
            return GetSlotTipText(slotNumber);
        }

        private void ResetSlotToolTip()
        {
            _hoverSlotNumber = 0;
            _slotToolTip.Hide(this);
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

        private void DrawCompactSelectionMark(Graphics graphics, int slotNumber)
        {
            Rectangle slotBounds = CalculateSlotBounds(slotNumber);
            Rectangle contentBounds = GetContentBounds();
            int rightAreaWidth = CalculateSideAreaWidth(contentBounds.Width);
            int markSize = Math.Max(1, Math.Min(3, slotBounds.Height));
            int x = slotBounds.Right + Math.Max(0, (rightAreaWidth - markSize) / 2);
            int y = slotBounds.Top + Math.Max(0, (slotBounds.Height - markSize) / 2);

            using (SolidBrush brush = new SolidBrush(_afterProcessSlotColor))
            {
                graphics.FillRectangle(brush, x, y, markSize, markSize);
            }
        }

        private bool IsInDesignMode()
        {
            return DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
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

    }
}
