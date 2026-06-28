using System;
using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class ActionSensorButtonDemoPage : UserControl
    {
        private readonly ActionSensorButtonControl _twoSensorButton;
        private readonly ActionSensorButtonControl _oneSensorButton;
        private readonly ActionSensorButtonControl _noSensorButton;
        private readonly ActionSensorButtonControl[] _buttons;
        private readonly CheckBox _commandCheckBox;
        private readonly CheckBox _sensor1CheckBox;
        private readonly CheckBox _sensor2CheckBox;
        private readonly ComboBox _sensorModeComboBox;
        private readonly ComboBox _sensorShapeComboBox;
        private readonly NumericUpDown _leftPaddingUpDown;
        private readonly NumericUpDown _textSpacingUpDown;
        private readonly NumericUpDown _radiusUpDown;
        private readonly NumericUpDown _shadowUpDown;
        private readonly NumericUpDown _shadowOffsetXUpDown;
        private readonly NumericUpDown _shadowOffsetYUpDown;
        private readonly NumericUpDown _shadowOpacityUpDown;
        private readonly Label _statusLabel;
        private int _clickCount;

        public ActionSensorButtonDemoPage()
        {
            Font = new Font("Times New Roman", 9f, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.FromArgb(230, 235, 242);
            ForeColor = Color.FromArgb(32, 38, 46);
            Padding = new Padding(8);

            Panel previewPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 520,
                BackColor = Color.FromArgb(230, 235, 242)
            };

            Label previewTitleLabel = CreateTitleLabel("预览");
            previewTitleLabel.Location = new Point(24, 20);
            previewPanel.Controls.Add(previewTitleLabel);

            _twoSensorButton = CreatePreviewButton("Ch1Arm1 Lift", SensorDisplayMode.Two, new Point(28, 76), new Size(180, 44));
            _oneSensorButton = CreatePreviewButton("X Axis Home", SensorDisplayMode.One, new Point(28, 148), new Size(180, 44));
            _noSensorButton = CreatePreviewButton("Refresh Setting", SensorDisplayMode.None, new Point(28, 220), new Size(180, 44));
            _buttons = new[] { _twoSensorButton, _oneSensorButton, _noSensorButton };

            Label twoLabel = CreateNoteLabel("Two sensors", 236, 84);
            Label oneLabel = CreateNoteLabel("One sensor", 236, 156);
            Label noneLabel = CreateNoteLabel("No sensor", 236, 228);

            previewPanel.Controls.Add(noneLabel);
            previewPanel.Controls.Add(oneLabel);
            previewPanel.Controls.Add(twoLabel);
            previewPanel.Controls.Add(_noSensorButton);
            previewPanel.Controls.Add(_oneSensorButton);
            previewPanel.Controls.Add(_twoSensorButton);

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(24, 312),
                Size = new Size(460, 180),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                TextAlign = ContentAlignment.TopLeft
            };
            previewPanel.Controls.Add(_statusLabel);

            Panel optionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Color.FromArgb(230, 235, 242)
            };

            Label titleLabel = CreateTitleLabel("动作传感器按钮控件");
            titleLabel.Dock = DockStyle.Top;
            optionPanel.Controls.Add(titleLabel);

            GroupBox stateGroup = CreateGroupBox("状态", 0, 56, 360, 160);
            _commandCheckBox = CreateCheckBox("CommandState / PLC 控制状态", 16, 30);
            _sensor1CheckBox = CreateCheckBox("Sensor1State", 16, 64);
            _sensor2CheckBox = CreateCheckBox("Sensor2State", 16, 98);
            _commandCheckBox.CheckedChanged += delegate { ApplyStates(); };
            _sensor1CheckBox.CheckedChanged += delegate { ApplyStates(); };
            _sensor2CheckBox.CheckedChanged += delegate { ApplyStates(); };
            stateGroup.Controls.Add(_sensor2CheckBox);
            stateGroup.Controls.Add(_sensor1CheckBox);
            stateGroup.Controls.Add(_commandCheckBox);

            GroupBox modeGroup = CreateGroupBox("模式与布局", 390, 56, 430, 220);
            _sensorModeComboBox = CreateComboBox(140, 30, 160);
            _sensorModeComboBox.Items.Add(SensorDisplayMode.None);
            _sensorModeComboBox.Items.Add(SensorDisplayMode.One);
            _sensorModeComboBox.Items.Add(SensorDisplayMode.Two);
            _sensorModeComboBox.SelectedItem = SensorDisplayMode.Two;
            _sensorModeComboBox.SelectedIndexChanged += delegate { ApplyModeAndLayout(); };

            _sensorShapeComboBox = CreateComboBox(140, 64, 160);
            _sensorShapeComboBox.Items.Add(SensorIndicatorShape.Rectangle);
            _sensorShapeComboBox.Items.Add(SensorIndicatorShape.Circle);
            _sensorShapeComboBox.Items.Add(SensorIndicatorShape.RoundedRectangle);
            _sensorShapeComboBox.SelectedItem = SensorIndicatorShape.Rectangle;
            _sensorShapeComboBox.SelectedIndexChanged += delegate { ApplyModeAndLayout(); };

            _leftPaddingUpDown = CreateNumericUpDown(140, 98, 0, 40, 10);
            _leftPaddingUpDown.ValueChanged += delegate { ApplyModeAndLayout(); };
            _textSpacingUpDown = CreateNumericUpDown(140, 132, 0, 40, 8);
            _textSpacingUpDown.ValueChanged += delegate { ApplyModeAndLayout(); };
            _radiusUpDown = CreateNumericUpDown(140, 166, 0, 18, 8);
            _radiusUpDown.ValueChanged += delegate { ApplyModeAndLayout(); };

            modeGroup.Controls.Add(CreateFieldLabel("SensorMode", 16, 33));
            modeGroup.Controls.Add(CreateFieldLabel("SensorShape", 16, 67));
            modeGroup.Controls.Add(CreateFieldLabel("SensorLeftPadding", 16, 101));
            modeGroup.Controls.Add(CreateFieldLabel("SensorTextSpacing", 16, 135));
            modeGroup.Controls.Add(CreateFieldLabel("Radius", 16, 169));
            modeGroup.Controls.Add(_radiusUpDown);
            modeGroup.Controls.Add(_textSpacingUpDown);
            modeGroup.Controls.Add(_leftPaddingUpDown);
            modeGroup.Controls.Add(_sensorShapeComboBox);
            modeGroup.Controls.Add(_sensorModeComboBox);

            GroupBox shadowGroup = CreateGroupBox("阴影", 0, 238, 360, 190);
            _shadowUpDown = CreateNumericUpDown(150, 30, 0, 12, 4);
            _shadowOffsetXUpDown = CreateNumericUpDown(150, 64, -8, 8, 0);
            _shadowOffsetYUpDown = CreateNumericUpDown(150, 98, -8, 8, 1);
            _shadowOpacityUpDown = CreateNumericUpDown(150, 132, 0, 100, 18);
            _shadowUpDown.ValueChanged += delegate { ApplyShadow(); };
            _shadowOffsetXUpDown.ValueChanged += delegate { ApplyShadow(); };
            _shadowOffsetYUpDown.ValueChanged += delegate { ApplyShadow(); };
            _shadowOpacityUpDown.ValueChanged += delegate { ApplyShadow(); };
            shadowGroup.Controls.Add(CreateFieldLabel("Shadow", 16, 33));
            shadowGroup.Controls.Add(CreateFieldLabel("OffsetX", 16, 67));
            shadowGroup.Controls.Add(CreateFieldLabel("OffsetY", 16, 101));
            shadowGroup.Controls.Add(CreateFieldLabel("Opacity %", 16, 135));
            shadowGroup.Controls.Add(_shadowOpacityUpDown);
            shadowGroup.Controls.Add(_shadowOffsetYUpDown);
            shadowGroup.Controls.Add(_shadowOffsetXUpDown);
            shadowGroup.Controls.Add(_shadowUpDown);

            GroupBox colorGroup = CreateGroupBox("配色与方法", 390, 298, 430, 150);
            Button blueButton = CreateButton("项目配色", 16, 32);
            blueButton.Click += delegate { ApplyBluePalette(); };
            Button amberButton = CreateButton("琥珀配色", 146, 32);
            amberButton.Click += delegate { ApplyAmberPalette(); };
            Button darkButton = CreateButton("深色配色", 276, 32);
            darkButton.Click += delegate { ApplyDarkPalette(); };
            Button resetButton = CreateButton("Reset Demo", 16, 82);
            resetButton.Click += delegate { ResetDemo(); };
            Button noneButton = CreateButton("No Sensor", 146, 82);
            noneButton.Click += delegate
            {
                _sensorModeComboBox.SelectedItem = SensorDisplayMode.None;
                ApplyModeAndLayout();
            };
            colorGroup.Controls.Add(noneButton);
            colorGroup.Controls.Add(resetButton);
            colorGroup.Controls.Add(darkButton);
            colorGroup.Controls.Add(amberButton);
            colorGroup.Controls.Add(blueButton);

            optionPanel.Controls.Add(colorGroup);
            optionPanel.Controls.Add(shadowGroup);
            optionPanel.Controls.Add(modeGroup);
            optionPanel.Controls.Add(stateGroup);

            Controls.Add(optionPanel);
            Controls.Add(previewPanel);

            ResetDemo();
        }

        private ActionSensorButtonControl CreatePreviewButton(string text, SensorDisplayMode mode, Point location, Size size)
        {
            ActionSensorButtonControl button = new ActionSensorButtonControl
            {
                Location = location,
                Size = size,
                ButtonText = text,
                SensorMode = mode,
                SensorShape = SensorIndicatorShape.Rectangle,
                Shadow = 4,
                ShadowOffsetY = 1
            };
            button.Click += OnPreviewButtonClick;
            return button;
        }

        private void OnPreviewButtonClick(object sender, System.EventArgs e)
        {
            _clickCount++;
            UpdateStatusLabel();
        }

        private void ResetDemo()
        {
            _commandCheckBox.Checked = false;
            _sensor1CheckBox.Checked = false;
            _sensor2CheckBox.Checked = true;
            _sensorModeComboBox.SelectedItem = SensorDisplayMode.Two;
            _sensorShapeComboBox.SelectedItem = SensorIndicatorShape.Rectangle;
            _leftPaddingUpDown.Value = 10;
            _textSpacingUpDown.Value = 8;
            _radiusUpDown.Value = 8;
            _shadowUpDown.Value = 4;
            _shadowOffsetXUpDown.Value = 0;
            _shadowOffsetYUpDown.Value = 1;
            _shadowOpacityUpDown.Value = 18;
            ApplyBluePalette();
            ApplyStates();
            ApplyModeAndLayout();
            ApplyShadow();
        }

        private void ApplyStates()
        {
            for (int index = 0; index < _buttons.Length; index++)
            {
                _buttons[index].CommandState = _commandCheckBox.Checked;
                _buttons[index].Sensor1State = _sensor1CheckBox.Checked;
                _buttons[index].Sensor2State = _sensor2CheckBox.Checked;
            }

            _oneSensorButton.Sensor2State = false;
            UpdateStatusLabel();
        }

        private void ApplyModeAndLayout()
        {
            SensorDisplayMode selectedMode = _sensorModeComboBox.SelectedItem is SensorDisplayMode
                ? (SensorDisplayMode)_sensorModeComboBox.SelectedItem
                : SensorDisplayMode.Two;
            SensorIndicatorShape selectedShape = _sensorShapeComboBox.SelectedItem is SensorIndicatorShape
                ? (SensorIndicatorShape)_sensorShapeComboBox.SelectedItem
                : SensorIndicatorShape.Rectangle;

            _twoSensorButton.SensorMode = selectedMode;
            _oneSensorButton.SensorMode = SensorDisplayMode.One;
            _noSensorButton.SensorMode = SensorDisplayMode.None;

            for (int index = 0; index < _buttons.Length; index++)
            {
                _buttons[index].SensorShape = selectedShape;
                _buttons[index].SensorLeftPadding = (int)_leftPaddingUpDown.Value;
                _buttons[index].SensorTextSpacing = (int)_textSpacingUpDown.Value;
                _buttons[index].Radius = (int)_radiusUpDown.Value;
            }

            UpdateStatusLabel();
        }

        private void ApplyShadow()
        {
            for (int index = 0; index < _buttons.Length; index++)
            {
                _buttons[index].Shadow = (int)_shadowUpDown.Value;
                _buttons[index].ShadowOffsetX = (int)_shadowOffsetXUpDown.Value;
                _buttons[index].ShadowOffsetY = (int)_shadowOffsetYUpDown.Value;
                _buttons[index].ShadowOpacity = (float)_shadowOpacityUpDown.Value / 100f;
            }

            UpdateStatusLabel();
        }

        private void ApplyBluePalette()
        {
            ApplyPalette(Color.FromArgb(226, 64, 64), Color.FromArgb(225, 236, 251), Color.FromArgb(40, 112, 210), Color.White, Color.FromArgb(40, 112, 210), Color.White, Color.Black);
        }

        private void ApplyAmberPalette()
        {
            ApplyPalette(Color.FromArgb(218, 142, 40), Color.FromArgb(128, 118, 102), Color.FromArgb(245, 190, 58), Color.FromArgb(90, 92, 96), Color.FromArgb(96, 82, 62), Color.White, Color.White);
        }

        private void ApplyDarkPalette()
        {
            ApplyPalette(Color.FromArgb(58, 173, 182), Color.FromArgb(55, 62, 70), Color.FromArgb(132, 220, 170), Color.FromArgb(70, 78, 88), Color.FromArgb(28, 34, 42), Color.White, Color.White);
        }

        private void ApplyPalette(Color commandOn, Color commandOff, Color sensorOn, Color sensorOff, Color border, Color commandOnFore, Color commandOffFore)
        {
            for (int index = 0; index < _buttons.Length; index++)
            {
                ActionSensorButtonControl control = _buttons[index];
                control.CommandOnBackColor = commandOn;
                control.CommandOffBackColor = commandOff;
                control.BackHover = Color.FromArgb(43, 125, 211);
                control.BackActive = Color.FromArgb(32, 104, 184);
                control.CommandOnForeColor = commandOnFore;
                control.CommandOffForeColor = commandOffFore;
                control.ForeHover = Color.White;
                control.SensorOnColor = sensorOn;
                control.SensorOffColor = sensorOff;
                control.SensorBorderColor = border;
                control.BorderColor = border;
            }

            UpdateStatusLabel();
        }

        private void UpdateStatusLabel()
        {
            _statusLabel.Text = string.Format(
                "ButtonText: {0}\r\nCommandState: {1}\r\nSensor1State: {2}  Sensor2State: {3}\r\nSensorMode: {4}\r\nSensorShape: {5}\r\nSensorLeftPadding: {6}  SensorTextSpacing: {7}\r\nRadius: {8}  Shadow: {9}  Offset: {10},{11}  Opacity: {12}%\r\nClick Count: {13}",
                _twoSensorButton.ButtonText,
                _twoSensorButton.CommandState,
                _twoSensorButton.Sensor1State,
                _twoSensorButton.Sensor2State,
                _twoSensorButton.SensorMode,
                _twoSensorButton.SensorShape,
                _twoSensorButton.SensorLeftPadding,
                _twoSensorButton.SensorTextSpacing,
                _twoSensorButton.Radius,
                _twoSensorButton.Shadow,
                _twoSensorButton.ShadowOffsetX,
                _twoSensorButton.ShadowOffsetY,
                (int)(_twoSensorButton.ShadowOpacity * 100f),
                _clickCount);
        }

        private static Label CreateTitleLabel(string text)
        {
            return new Label
            {
                AutoSize = false,
                Size = new Size(420, 32),
                Text = text,
                Font = new Font("Times New Roman", 12f, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Label CreateNoteLabel(string text, int x, int y)
        {
            return new Label
            {
                AutoSize = false,
                Location = new Point(x, y),
                Size = new Size(200, 24),
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Label CreateFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                AutoSize = false,
                Location = new Point(x, y),
                Size = new Size(120, 22),
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static GroupBox CreateGroupBox(string text, int x, int y, int width, int height)
        {
            return new GroupBox
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height)
            };
        }

        private static CheckBox CreateCheckBox(string text, int x, int y)
        {
            return new CheckBox
            {
                AutoSize = false,
                Location = new Point(x, y),
                Size = new Size(260, 26),
                Text = text
            };
        }

        private static ComboBox CreateComboBox(int x, int y, int width)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(x, y),
                Size = new Size(width, 24)
            };
        }

        private static NumericUpDown CreateNumericUpDown(int x, int y, int min, int max, int value)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Size = new Size(90, 24),
                Minimum = min,
                Maximum = max,
                Value = value
            };
        }

        private static Button CreateButton(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(116, 30)
            };
        }
    }
}
