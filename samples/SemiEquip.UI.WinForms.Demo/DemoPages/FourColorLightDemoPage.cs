using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class FourColorLightDemoPage : UserControl
    {
        private readonly FourColorLightControl _fourColorLight;
        private readonly CheckBox _redCheckBox;
        private readonly CheckBox _yellowCheckBox;
        private readonly CheckBox _greenCheckBox;
        private readonly CheckBox _blueCheckBox;
        private readonly CheckBox _maintainAspectCheckBox;
        private readonly CheckBox _frostedCheckBox;
        private readonly NumericUpDown _gapUpDown;
        private readonly NumericUpDown _lineSpacingUpDown;
        private readonly Label _statusLabel;

        public FourColorLightDemoPage()
        {
            Font = new Font("Times New Roman", 9f, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.FromArgb(230, 235, 242);
            ForeColor = Color.FromArgb(32, 38, 46);
            Padding = new Padding(8);

            Panel previewPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 420,
                BackColor = Color.FromArgb(230, 235, 242)
            };

            Label previewTitle = CreateTitleLabel("预览");
            previewTitle.Location = new Point(24, 20);
            previewPanel.Controls.Add(previewTitle);

            _fourColorLight = new FourColorLightControl
            {
                Location = new Point(136, 72),
                Size = new Size(92, 368),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            previewPanel.Controls.Add(_fourColorLight);

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(24, 480),
                Size = new Size(360, 120),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            previewPanel.Controls.Add(_statusLabel);

            Panel optionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Color.FromArgb(230, 235, 242)
            };

            Label titleLabel = CreateTitleLabel("四色灯控件");
            titleLabel.Dock = DockStyle.Top;
            optionPanel.Controls.Add(titleLabel);

            GroupBox stateGroup = CreateGroupBox("灯段状态", 0, 56, 360, 210);
            _redCheckBox = CreateCheckBox("RedLightOn", 16, 32);
            _yellowCheckBox = CreateCheckBox("YellowLightOn", 16, 66);
            _greenCheckBox = CreateCheckBox("GreenLightOn", 16, 100);
            _blueCheckBox = CreateCheckBox("BlueLightOn", 16, 134);
            _redCheckBox.CheckedChanged += delegate { ApplyLightStatesFromChecks(); };
            _yellowCheckBox.CheckedChanged += delegate { ApplyLightStatesFromChecks(); };
            _greenCheckBox.CheckedChanged += delegate { ApplyLightStatesFromChecks(); };
            _blueCheckBox.CheckedChanged += delegate { ApplyLightStatesFromChecks(); };
            stateGroup.Controls.Add(_blueCheckBox);
            stateGroup.Controls.Add(_greenCheckBox);
            stateGroup.Controls.Add(_yellowCheckBox);
            stateGroup.Controls.Add(_redCheckBox);

            GroupBox layoutGroup = CreateGroupBox("外观属性", 390, 56, 420, 210);
            _maintainAspectCheckBox = CreateCheckBox("MaintainAspectRatio", 16, 32);
            _maintainAspectCheckBox.Checked = true;
            _maintainAspectCheckBox.CheckedChanged += delegate
            {
                _fourColorLight.MaintainAspectRatio = _maintainAspectCheckBox.Checked;
                UpdateStatusLabel();
            };
            _frostedCheckBox = CreateCheckBox("ShowFrostedLines", 16, 66);
            _frostedCheckBox.Checked = true;
            _frostedCheckBox.CheckedChanged += delegate
            {
                _fourColorLight.ShowFrostedLines = _frostedCheckBox.Checked;
                UpdateStatusLabel();
            };
            _gapUpDown = CreateNumericUpDown(160, 102, 0, 20, 0);
            _gapUpDown.ValueChanged += delegate
            {
                _fourColorLight.LightGap = (int)_gapUpDown.Value;
                UpdateStatusLabel();
            };
            _lineSpacingUpDown = CreateNumericUpDown(160, 136, 1, 12, 3);
            _lineSpacingUpDown.ValueChanged += delegate
            {
                _fourColorLight.FrostedLineSpacing = (int)_lineSpacingUpDown.Value;
                UpdateStatusLabel();
            };
            layoutGroup.Controls.Add(CreateFieldLabel("LightGap", 16, 105));
            layoutGroup.Controls.Add(CreateFieldLabel("FrostedLineSpacing", 16, 139));
            layoutGroup.Controls.Add(_lineSpacingUpDown);
            layoutGroup.Controls.Add(_gapUpDown);
            layoutGroup.Controls.Add(_frostedCheckBox);
            layoutGroup.Controls.Add(_maintainAspectCheckBox);

            GroupBox methodGroup = CreateGroupBox("方法示例", 0, 292, 810, 130);
            Button runningButton = CreateButton("运行示例", 16, 34);
            runningButton.Click += delegate { SetDemoStates(false, false, true, false); };
            Button alarmButton = CreateButton("报警示例", 146, 34);
            alarmButton.Click += delegate { SetDemoStates(true, true, false, false); };
            Button manualButton = CreateButton("人工介入", 276, 34);
            manualButton.Click += delegate { SetDemoStates(false, true, false, true); };
            Button resetButton = CreateButton("全部关闭", 406, 34);
            resetButton.Click += delegate { SetDemoStates(false, false, false, false); };
            Button colorButton = CreateButton("高对比色", 536, 34);
            colorButton.Click += delegate { ApplyHighContrastColors(); };
            Button defaultColorButton = CreateButton("默认配色", 666, 34);
            defaultColorButton.Click += delegate { ApplyDefaultColors(); };
            methodGroup.Controls.Add(defaultColorButton);
            methodGroup.Controls.Add(colorButton);
            methodGroup.Controls.Add(resetButton);
            methodGroup.Controls.Add(manualButton);
            methodGroup.Controls.Add(alarmButton);
            methodGroup.Controls.Add(runningButton);

            optionPanel.Controls.Add(methodGroup);
            optionPanel.Controls.Add(layoutGroup);
            optionPanel.Controls.Add(stateGroup);

            Controls.Add(optionPanel);
            Controls.Add(previewPanel);

            SetDemoStates(false, false, true, false);
        }

        private void ApplyLightStatesFromChecks()
        {
            _fourColorLight.SetLightStates(
                _redCheckBox.Checked,
                _yellowCheckBox.Checked,
                _greenCheckBox.Checked,
                _blueCheckBox.Checked);
            UpdateStatusLabel();
        }

        private void SetDemoStates(bool redOn, bool yellowOn, bool greenOn, bool blueOn)
        {
            _redCheckBox.Checked = redOn;
            _yellowCheckBox.Checked = yellowOn;
            _greenCheckBox.Checked = greenOn;
            _blueCheckBox.Checked = blueOn;
            _fourColorLight.SetLightStates(redOn, yellowOn, greenOn, blueOn);
            UpdateStatusLabel();
        }

        private void ApplyHighContrastColors()
        {
            _fourColorLight.RedOnColor = Color.FromArgb(255, 42, 42);
            _fourColorLight.YellowOnColor = Color.FromArgb(255, 210, 48);
            _fourColorLight.GreenOnColor = Color.FromArgb(28, 210, 96);
            _fourColorLight.BlueOnColor = Color.FromArgb(40, 130, 255);
            UpdateStatusLabel();
        }

        private void ApplyDefaultColors()
        {
            _fourColorLight.RedOnColor = Color.FromArgb(226, 64, 64);
            _fourColorLight.YellowOnColor = Color.FromArgb(245, 190, 58);
            _fourColorLight.GreenOnColor = Color.FromArgb(46, 184, 92);
            _fourColorLight.BlueOnColor = Color.FromArgb(55, 137, 255);
            UpdateStatusLabel();
        }

        private void UpdateStatusLabel()
        {
            _statusLabel.Text = string.Format(
                "Red: {0}  Yellow: {1}\r\nGreen: {2}  Blue: {3}\r\nMaintainAspectRatio: {4}\r\nLightGap: {5}  FrostedLineSpacing: {6}\r\nShowFrostedLines: {7}",
                _fourColorLight.RedLightOn,
                _fourColorLight.YellowLightOn,
                _fourColorLight.GreenLightOn,
                _fourColorLight.BlueLightOn,
                _fourColorLight.MaintainAspectRatio,
                _fourColorLight.LightGap,
                _fourColorLight.FrostedLineSpacing,
                _fourColorLight.ShowFrostedLines);
        }

        private static Label CreateTitleLabel(string text)
        {
            return new Label
            {
                AutoSize = false,
                Size = new Size(360, 32),
                Text = text,
                Font = new Font("Times New Roman", 12f, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Label CreateFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                AutoSize = false,
                Location = new Point(x, y),
                Size = new Size(140, 22),
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
                Size = new Size(220, 26),
                Text = text
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
