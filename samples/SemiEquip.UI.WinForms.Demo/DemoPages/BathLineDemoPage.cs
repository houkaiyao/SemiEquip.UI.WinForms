using System;
using System.Drawing;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class BathLineDemoPage : UserControl
    {
        private readonly BathLineControl.BathLineControl _bath;
        private readonly CheckBox _outerBathEnabled;
        private readonly CheckBox _innerSensorVisible;
        private readonly CheckBox _outerSensorVisible;
        private readonly CheckBox _innerLL;
        private readonly CheckBox _innerL;
        private readonly CheckBox _innerH;
        private readonly CheckBox _innerHH;
        private readonly CheckBox _outerLL;
        private readonly CheckBox _outerL;
        private readonly CheckBox _outerH;
        private readonly CheckBox _outerHH;

        public BathLineDemoPage()
        {
            BackColor = Color.FromArgb(230, 235, 242);
            Padding = new Padding(16);

            Panel previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(230, 235, 242)
            };

            _bath = new BathLineControl.BathLineControl
            {
                Location = new Point(24, 80),
                Size = new Size(560, 340),
                Padding = new Padding(28),
                OutlineWidth = 3F,
                ChemicalColor = Color.Yellow,
                ChemicalText = "H2SO4",
                ChemicalWidth = 150,
                ChemicalHeight = 34,
                OuterBathEnabled = true,
                InnerLLSensor = true,
                InnerLSensor = true,
                OuterLLSensor = true,
                BathText = "槽体",
            };
            previewPanel.Controls.Add(_bath);

            Panel propertyPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 420,
                AutoScroll = true,
                Padding = new Padding(12),
                BackColor = Color.White
            };

            GroupBox bathGroup = CreateGroup("槽体", 0, 0, 370, 190);
            _outerBathEnabled = CreateCheckBox("OuterBathEnabled", 16, 28, _bath.OuterBathEnabled);
            NumericUpDown outlineWidth = CreateNumber(190, 26, 1, 20, (decimal)_bath.OutlineWidth);
            outlineWidth.ValueChanged += delegate { _bath.OutlineWidth = (float)outlineWidth.Value; };
            bathGroup.Controls.Add(_outerBathEnabled);
            bathGroup.Controls.Add(CreateLabel("OutlineWidth", 190, 10));
            bathGroup.Controls.Add(outlineWidth);
            TextBox bathText = new TextBox
            {
                Location = new Point(120, 65),
                Size = new Size(220, 24),
                Text = _bath.BathText
            };
            bathText.TextChanged += delegate { _bath.BathText = bathText.Text; };
            bathGroup.Controls.Add(CreateLabel("BathText", 16, 69));
            bathGroup.Controls.Add(bathText);
            bathGroup.Controls.Add(CreateFontButton("BathFont 字体 / 字号", 104,
                delegate { return _bath.BathFont; }, delegate(Font font) { _bath.BathFont = font; }));
            bathGroup.Controls.Add(CreateTextColorButton("BathForeColor", 144,
                delegate { return _bath.BathForeColor; }, delegate(Color color) { _bath.BathForeColor = color; }));

            GroupBox chemicalGroup = CreateGroup("Chemical", 0, 200, 370, 230);
            TextBox chemicalText = new TextBox
            {
                Location = new Point(120, 26),
                Size = new Size(220, 24),
                Text = _bath.ChemicalText
            };
            chemicalText.TextChanged += delegate { _bath.ChemicalText = chemicalText.Text; };
            NumericUpDown chemicalWidth = CreateNumber(120, 62, 1, 300, _bath.ChemicalWidth);
            NumericUpDown chemicalHeight = CreateNumber(250, 62, 1, 150, _bath.ChemicalHeight);
            chemicalWidth.ValueChanged += delegate { _bath.ChemicalWidth = (int)chemicalWidth.Value; };
            chemicalHeight.ValueChanged += delegate { _bath.ChemicalHeight = (int)chemicalHeight.Value; };
            Button chemicalColor = new Button
            {
                Location = new Point(120, 104),
                Size = new Size(220, 32),
                Text = "选择 ChemicalColor",
                BackColor = _bath.ChemicalColor
            };
            chemicalColor.Click += delegate
            {
                using (ColorDialog dialog = new ColorDialog())
                {
                    dialog.Color = _bath.ChemicalColor;
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        _bath.ChemicalColor = dialog.Color;
                        chemicalColor.BackColor = dialog.Color;
                    }
                }
            };
            chemicalGroup.Controls.Add(CreateLabel("ChemicalText", 16, 30));
            chemicalGroup.Controls.Add(chemicalText);
            chemicalGroup.Controls.Add(CreateLabel("宽", 16, 66));
            chemicalGroup.Controls.Add(chemicalWidth);
            chemicalGroup.Controls.Add(CreateLabel("高", 210, 66));
            chemicalGroup.Controls.Add(chemicalHeight);
            chemicalGroup.Controls.Add(chemicalColor);
            chemicalGroup.Controls.Add(CreateFontButton("ChemicalFont 字体 / 字号", 144,
                delegate { return _bath.ChemicalFont; }, delegate(Font font) { _bath.ChemicalFont = font; }));
            chemicalGroup.Controls.Add(CreateTextColorButton("ChemicalForeColor", 184,
                delegate { return _bath.ChemicalForeColor; }, delegate(Color color) { _bath.ChemicalForeColor = color; }));

            GroupBox sensorGroup = CreateGroup("Level Sensors", 0, 440, 370, 260);
            _innerSensorVisible = CreateCheckBox("InnerSensorVisible", 16, 28, true);
            _outerSensorVisible = CreateCheckBox("OuterSensorVisible", 190, 28, true);
            sensorGroup.Controls.Add(_innerSensorVisible);
            sensorGroup.Controls.Add(_outerSensorVisible);
            sensorGroup.Controls.Add(CreateLabel("Inner", 120, 66));
            sensorGroup.Controls.Add(CreateLabel("Outer", 245, 66));

            _innerHH = CreateSensorCheckBox("HH", 120, 92, _bath.InnerHHSensor);
            _outerHH = CreateSensorCheckBox("HH", 245, 92, _bath.OuterHHSensor);
            _innerH = CreateSensorCheckBox("H", 120, 126, _bath.InnerHSensor);
            _outerH = CreateSensorCheckBox("H", 245, 126, _bath.OuterHSensor);
            _innerL = CreateSensorCheckBox("L", 120, 160, _bath.InnerLSensor);
            _outerL = CreateSensorCheckBox("L", 245, 160, _bath.OuterLSensor);
            _innerLL = CreateSensorCheckBox("LL", 120, 194, _bath.InnerLLSensor);
            _outerLL = CreateSensorCheckBox("LL", 245, 194, _bath.OuterLLSensor);
            sensorGroup.Controls.AddRange(new Control[]
            {
                _innerHH, _outerHH, _innerH, _outerH,
                _innerL, _outerL, _innerLL, _outerLL
            });

            _outerBathEnabled.CheckedChanged += delegate
            {
                _bath.OuterBathEnabled = _outerBathEnabled.Checked;
                SyncSensorControls();
            };
            _innerSensorVisible.CheckedChanged += delegate
            {
                _bath.InnerSensorVisible = _innerSensorVisible.Checked;
                SyncSensorControls();
            };
            _outerSensorVisible.CheckedChanged += delegate
            {
                _bath.OuterSensorVisible = _outerSensorVisible.Checked;
                SyncSensorControls();
            };

            BindSensor(_innerLL, delegate(bool value) { _bath.InnerLLSensor = value; });
            BindSensor(_innerL, delegate(bool value) { _bath.InnerLSensor = value; });
            BindSensor(_innerH, delegate(bool value) { _bath.InnerHSensor = value; });
            BindSensor(_innerHH, delegate(bool value) { _bath.InnerHHSensor = value; });
            BindSensor(_outerLL, delegate(bool value) { _bath.OuterLLSensor = value; });
            BindSensor(_outerL, delegate(bool value) { _bath.OuterLSensor = value; });
            BindSensor(_outerH, delegate(bool value) { _bath.OuterHSensor = value; });
            BindSensor(_outerHH, delegate(bool value) { _bath.OuterHHSensor = value; });

            propertyPanel.Controls.Add(sensorGroup);
            propertyPanel.Controls.Add(chemicalGroup);
            propertyPanel.Controls.Add(bathGroup);
            Controls.Add(previewPanel);
            Controls.Add(propertyPanel);
            SyncSensorControls();
        }

        private Button CreateFontButton(string text, int y, Func<Font> getter, Action<Font> setter)
        {
            Button button = new Button { Text = text, Location = new Point(120, y), Size = new Size(220, 32) };
            button.Click += delegate
            {
                using (FontDialog dialog = new FontDialog())
                {
                    dialog.Font = getter();
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        setter(dialog.Font);
                    }
                }
            };
            return button;
        }

        private Button CreateTextColorButton(string text, int y, Func<Color> getter, Action<Color> setter)
        {
            Button button = new Button { Text = text, Location = new Point(120, y), Size = new Size(220, 32) };
            button.Click += delegate
            {
                using (ColorDialog dialog = new ColorDialog())
                {
                    dialog.Color = getter();
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        setter(dialog.Color);
                    }
                }
            };
            return button;
        }

        private void BindSensor(CheckBox checkBox, Action<bool> setter)
        {
            checkBox.CheckedChanged += delegate { setter(checkBox.Checked); };
        }

        private void SyncSensorControls()
        {
            _outerBathEnabled.Checked = _bath.OuterBathEnabled;
            _innerSensorVisible.Checked = _bath.InnerSensorVisible;
            _outerSensorVisible.Checked = _bath.OuterSensorVisible;

            _innerHH.Checked = _bath.InnerHHSensor;
            _innerH.Checked = _bath.InnerHSensor;
            _innerL.Checked = _bath.InnerLSensor;
            _innerLL.Checked = _bath.InnerLLSensor;
            _outerHH.Checked = _bath.OuterHHSensor;
            _outerH.Checked = _bath.OuterHSensor;
            _outerL.Checked = _bath.OuterLSensor;
            _outerLL.Checked = _bath.OuterLLSensor;
        }

        private static GroupBox CreateGroup(string text, int x, int y, int width, int height)
        {
            return new GroupBox { Text = text, Location = new Point(x, y), Size = new Size(width, height) };
        }

        private static Label CreateLabel(string text, int x, int y)
        {
            return new Label { AutoSize = true, Text = text, Location = new Point(x, y) };
        }

        private static CheckBox CreateCheckBox(string text, int x, int y, bool value)
        {
            return new CheckBox { AutoSize = true, Text = text, Location = new Point(x, y), Checked = value };
        }

        private static CheckBox CreateSensorCheckBox(string text, int x, int y, bool value)
        {
            return CreateCheckBox(text, x, y, value);
        }

        private static NumericUpDown CreateNumber(int x, int y, int minimum, int maximum, decimal value)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Size = new Size(90, 24),
                Minimum = minimum,
                Maximum = maximum,
                Value = value
            };
        }
    }
}
