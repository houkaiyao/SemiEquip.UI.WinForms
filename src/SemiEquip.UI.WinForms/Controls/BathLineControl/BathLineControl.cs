using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BathLineControl
{
    /// <summary>
    /// Displays an inner process bath with optional outer overflow baths.
    /// </summary>
    [DefaultProperty("OuterBathEnabled")]
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public sealed class BathLineControl : Control
    {
        private bool _outerBathEnabled = true;
        private Color _outlineColor = Color.FromArgb(24, 103, 150);
        private float _outlineWidth = 2F;
        private Color _chemicalColor = Color.White;
        private Color _chemicalForeColor = Color.White;
        private string _chemicalText = string.Empty;
        private string _bathText = string.Empty;
        private Color _bathForeColor = Color.Black;
        private Font _bathFont;
        private Font _chemicalFont;
        private int _chemicalWidth = 24;
        private int _chemicalHeight = 24;
        private bool _innerSensorVisible = true;
        private bool _outerSensorVisible = true;
        private bool _innerLLSensor;
        private bool _innerLSensor;
        private bool _innerHSensor;
        private bool _innerHHSensor;
        private bool _outerLLSensor;
        private bool _outerLSensor;
        private bool _outerHSensor;
        private bool _outerHHSensor;

        public BathLineControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);

            BackColor = Color.Transparent;
            MinimumSize = new Size(80, 50);
            Size = new Size(500, 300);
        }

        [Category("Appearance")]
        [Description("Determines whether the outer overflow baths are drawn.")]
        [DefaultValue(true)]
        public bool OuterBathEnabled
        {
            get { return _outerBathEnabled; }
            set
            {
                if (_outerBathEnabled == value)
                {
                    return;
                }

                _outerBathEnabled = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Color used to draw the bath outline.")]
        public Color OutlineColor
        {
            get { return _outlineColor; }
            set
            {
                if (_outlineColor == value)
                {
                    return;
                }

                _outlineColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Width of the bath outline in pixels.")]
        [DefaultValue(2F)]
        public float OutlineWidth
        {
            get { return _outlineWidth; }
            set
            {
                float normalizedValue = value < 1F ? 1F : value;
                if (_outlineWidth == normalizedValue)
                {
                    return;
                }

                _outlineWidth = normalizedValue;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Color of the square chemical indicator at the bottom of the inner bath.")]
        [DefaultValue(typeof(Color), "White")]
        public Color ChemicalColor
        {
            get { return _chemicalColor; }
            set
            {
                if (_chemicalColor == value)
                {
                    return;
                }

                _chemicalColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Description text displayed inside the chemical indicator.")]
        [DefaultValue("")]
        public string ChemicalText
        {
            get { return _chemicalText; }
            set
            {
                string normalizedValue = value ?? string.Empty;
                if (_chemicalText == normalizedValue)
                {
                    return;
                }

                _chemicalText = normalizedValue;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Bath name displayed in the upper middle of the inner bath.")]
        [DefaultValue("")]
        public string BathText
        {
            get { return _bathText; }
            set
            {
                string normalizedValue = value ?? string.Empty;
                if (_bathText == normalizedValue)
                {
                    return;
                }

                _bathText = normalizedValue;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Color of the bath name text.")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BathForeColor
        {
            get { return _bathForeColor; }
            set
            {
                if (_bathForeColor == value)
                {
                    return;
                }

                _bathForeColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Font of the bath name text. Uses the control Font when not set.")]
        public Font BathFont
        {
            get { return _bathFont ?? Font; }
            set
            {
                if (object.Equals(_bathFont, value))
                {
                    return;
                }

                _bathFont = value;
                Invalidate();
            }
        }

        private bool ShouldSerializeBathFont()
        {
            return _bathFont != null;
        }

        private void ResetBathFont()
        {
            BathFont = null;
        }

        [Category("Appearance")]
        [Description("Font of the chemical description text. Uses the control Font when not set.")]
        public Font ChemicalFont
        {
            get { return _chemicalFont ?? Font; }
            set
            {
                if (object.Equals(_chemicalFont, value))
                {
                    return;
                }

                _chemicalFont = value;
                Invalidate();
            }
        }

        private bool ShouldSerializeChemicalFont()
        {
            return _chemicalFont != null;
        }

        private void ResetChemicalFont()
        {
            ChemicalFont = null;
        }

        [Category("Appearance")]
        [Description("Color of the chemical description text.")]
        [DefaultValue(typeof(Color), "White")]
        public Color ChemicalForeColor
        {
            get { return _chemicalForeColor; }
            set
            {
                if (_chemicalForeColor == value)
                {
                    return;
                }

                _chemicalForeColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Width of the chemical indicator in pixels.")]
        [DefaultValue(24)]
        public int ChemicalWidth
        {
            get { return _chemicalWidth; }
            set
            {
                int normalizedValue = value < 1 ? 1 : value;
                if (_chemicalWidth == normalizedValue)
                {
                    return;
                }

                _chemicalWidth = normalizedValue;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Height of the chemical indicator in pixels.")]
        [DefaultValue(24)]
        public int ChemicalHeight
        {
            get { return _chemicalHeight; }
            set
            {
                int normalizedValue = value < 1 ? 1 : value;
                if (_chemicalHeight == normalizedValue)
                {
                    return;
                }

                _chemicalHeight = normalizedValue;
                Invalidate();
            }
        }

        [Category("Level Sensors")]
        [DefaultValue(true)]
        public bool InnerSensorVisible
        {
            get { return _innerSensorVisible; }
            set
            {
                if (_innerSensorVisible == value)
                {
                    return;
                }

                _innerSensorVisible = value;
                Invalidate();
            }
        }

        [Category("Level Sensors")]
        [DefaultValue(true)]
        public bool OuterSensorVisible
        {
            get { return _outerSensorVisible; }
            set
            {
                if (_outerSensorVisible == value)
                {
                    return;
                }

                _outerSensorVisible = value;
                Invalidate();
            }
        }

        [Category("Level Sensors")]
        [DefaultValue(false)]
        public bool InnerLLSensor
        {
            get { return _innerLLSensor; }
            set { SetSensorValue(ref _innerLLSensor, value); }
        }

        [Category("Level Sensors")]
        [DefaultValue(false)]
        public bool InnerLSensor
        {
            get { return _innerLSensor; }
            set { SetSensorValue(ref _innerLSensor, value); }
        }

        [Category("Level Sensors")]
        [DefaultValue(false)]
        public bool InnerHSensor
        {
            get { return _innerHSensor; }
            set { SetSensorValue(ref _innerHSensor, value); }
        }

        [Category("Level Sensors")]
        [DefaultValue(false)]
        public bool InnerHHSensor
        {
            get { return _innerHHSensor; }
            set { SetSensorValue(ref _innerHHSensor, value); }
        }

        [Category("Level Sensors")]
        [DefaultValue(false)]
        public bool OuterLLSensor
        {
            get { return _outerLLSensor; }
            set { SetSensorValue(ref _outerLLSensor, value); }
        }

        [Category("Level Sensors")]
        [DefaultValue(false)]
        public bool OuterLSensor
        {
            get { return _outerLSensor; }
            set { SetSensorValue(ref _outerLSensor, value); }
        }

        [Category("Level Sensors")]
        [DefaultValue(false)]
        public bool OuterHSensor
        {
            get { return _outerHSensor; }
            set { SetSensorValue(ref _outerHSensor, value); }
        }

        [Category("Level Sensors")]
        [DefaultValue(false)]
        public bool OuterHHSensor
        {
            get { return _outerHHSensor; }
            set { SetSensorValue(ref _outerHHSensor, value); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle drawingBounds = GetDrawingBounds();
            if (drawingBounds.Width <= 1 || drawingBounds.Height <= 1)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            float left = drawingBounds.Left;
            float right = drawingBounds.Right;
            float top = drawingBounds.Top;
            float shoulderY = drawingBounds.Top + drawingBounds.Height * 0.22F;
            float bottom = drawingBounds.Bottom;
            float innerLeft = drawingBounds.Left + drawingBounds.Width * 0.15F;
            float innerRight = drawingBounds.Right - drawingBounds.Width * 0.15F;

            using (Pen pen = new Pen(_outlineColor, _outlineWidth))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                DrawInnerBath(e.Graphics, pen, innerLeft, innerRight, top, bottom);

                if (_outerBathEnabled)
                {
                    DrawOuterBaths(e.Graphics, pen, left, right, top, shoulderY, innerLeft, innerRight);
                }
            }

            DrawChemicalIndicator(e.Graphics, innerLeft, innerRight, top, bottom);
            DrawBathText(e.Graphics, innerLeft, innerRight, top, bottom);
            DrawLevelSensors(e.Graphics, drawingBounds);
        }

        private Rectangle GetDrawingBounds()
        {
            Rectangle bounds = ClientRectangle;
            bounds = Rectangle.FromLTRB(
                bounds.Left + Padding.Left,
                bounds.Top + Padding.Top,
                bounds.Right - Padding.Right - GetSensorPanelWidth(bounds.Width),
                bounds.Bottom - Padding.Bottom);

            int inset = (int)(_outlineWidth / 2F) + 1;
            bounds.Inflate(-inset, -inset);
            return bounds;
        }

        private int GetSensorPanelWidth(int availableWidth)
        {
            bool showAnySensor = _innerSensorVisible || (_outerSensorVisible && _outerBathEnabled);
            return availableWidth >= 220 && showAnySensor ? 92 : 0;
        }

        private void SetSensorValue(ref bool field, bool value)
        {
            if (field == value)
            {
                return;
            }

            field = value;
            Invalidate();
        }

        private void DrawLevelSensors(Graphics graphics, Rectangle bathBounds)
        {
            if (GetSensorPanelWidth(ClientRectangle.Width) == 0)
            {
                return;
            }

            float panelLeft = bathBounds.Right + 12F;
            float innerX = panelLeft + 38F;
            float outerX = panelLeft + 68F;
            float rowHeight = System.Math.Max(18F, bathBounds.Height / 4F);
            float top = bathBounds.Top + rowHeight / 2F;

            using (SolidBrush textBrush = new SolidBrush(ForeColor))
            {
                DrawSensorRow(graphics, textBrush, "HH", top, innerX, outerX, _innerHHSensor, _outerHHSensor);
                DrawSensorRow(graphics, textBrush, "H", top + rowHeight, innerX, outerX, _innerHSensor, _outerHSensor);
                DrawSensorRow(graphics, textBrush, "L", top + rowHeight * 2F, innerX, outerX, _innerLSensor, _outerLSensor);
                DrawSensorRow(graphics, textBrush, "LL", top + rowHeight * 3F, innerX, outerX, _innerLLSensor, _outerLLSensor);
            }
        }

        private void DrawSensorRow(Graphics graphics, Brush textBrush, string label, float y, float innerX, float outerX, bool innerValue, bool outerValue)
        {
            graphics.DrawString(label, Font, textBrush, innerX - 38F, y - Font.Height / 2F);
            if (_innerSensorVisible)
            {
                DrawSensorLamp(graphics, innerX, y, innerValue);
            }
            if (_outerSensorVisible && _outerBathEnabled)
            {
                DrawSensorLamp(graphics, outerX, y, outerValue);
            }
        }

        private static void DrawSensorLamp(Graphics graphics, float x, float y, bool value)
        {
            const float diameter = 12F;
            Color color = value ? Color.LimeGreen : Color.DimGray;
            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.FillEllipse(brush, x - diameter / 2F, y - diameter / 2F, diameter, diameter);
            }
        }

        private void DrawBathText(Graphics graphics, float innerLeft, float innerRight, float top, float bottom)
        {
            if (_bathText.Length == 0)
            {
                return;
            }

            float inset = 4F + _outlineWidth / 2F;
            RectangleF bounds = new RectangleF(
                innerLeft + inset,
                top + (bottom - top) * 0.2F,
                innerRight - innerLeft - inset * 2F,
                (bottom - top) * 0.25F);
            if (bounds.Width <= 0F || bounds.Height <= 0F)
            {
                return;
            }

            using (SolidBrush brush = new SolidBrush(_bathForeColor))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                graphics.DrawString(_bathText, BathFont, brush, bounds, format);
            }
        }

        private void DrawChemicalIndicator(Graphics graphics, float innerLeft, float innerRight, float top, float bottom)
        {
            const float gap = 4F;
            float availableWidth = innerRight - innerLeft - gap * 2F;
            float availableHeight = bottom - top - gap * 2F;
            float width = System.Math.Min(_chemicalWidth, availableWidth);
            float height = System.Math.Min(_chemicalHeight, availableHeight);
            if (width <= 0F || height <= 0F)
            {
                return;
            }

            float x = innerLeft + (innerRight - innerLeft - width) / 2F;
            float y = bottom - gap - _outlineWidth / 2F - height;
            using (SolidBrush brush = new SolidBrush(_chemicalColor))
            {
                graphics.FillRectangle(brush, x, y, width, height);
            }

            if (_chemicalText.Length > 0)
            {
                using (SolidBrush textBrush = new SolidBrush(_chemicalForeColor))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    graphics.DrawString(_chemicalText, ChemicalFont, textBrush, new RectangleF(x, y, width, height), format);
                }
            }
        }

        private static void DrawInnerBath(Graphics graphics, Pen pen, float innerLeft, float innerRight, float top, float bottom)
        {
            graphics.DrawLines(
                pen,
                new[]
                {
                    new PointF(innerLeft, top),
                    new PointF(innerLeft, bottom),
                    new PointF(innerRight, bottom),
                    new PointF(innerRight, top)
                });
        }

        private static void DrawOuterBaths(
            Graphics graphics,
            Pen pen,
            float left,
            float right,
            float top,
            float shoulderY,
            float innerLeft,
            float innerRight)
        {
            graphics.DrawLines(
                pen,
                new[]
                {
                    new PointF(left, top),
                    new PointF(left, shoulderY),
                    new PointF(innerLeft, shoulderY)
                });

            graphics.DrawLines(
                pen,
                new[]
                {
                    new PointF(innerRight, shoulderY),
                    new PointF(right, shoulderY),
                    new PointF(right, top)
                });
        }
    }
}
