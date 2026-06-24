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

        public FourColorLightDemoPage()
        {
            Font = new Font("Times New Roman", 9f, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.FromArgb(230, 235, 242);
            ForeColor = Color.FromArgb(32, 38, 46);
            Padding = new Padding(8);

            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = Color.FromArgb(230, 235, 242)
            };

            _fourColorLight = new FourColorLightControl
            {
                Location = new Point(70, 28),
                Size = new Size(92, 368),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            controlPanel.Controls.Add(_fourColorLight);

            Panel optionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Color.FromArgb(230, 235, 242)
            };

            Label titleLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 36,
                Text = "四色灯控件",
                Font = new Font("Times New Roman", 12f, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label hintLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 56),
                Size = new Size(360, 28),
                Text = "勾选为亮色，取消勾选为暗色。"
            };

            _redCheckBox = CreateLightCheckBox("红灯", 0, 104);
            _yellowCheckBox = CreateLightCheckBox("黄灯", 0, 136);
            _greenCheckBox = CreateLightCheckBox("绿灯", 0, 168);
            _blueCheckBox = CreateLightCheckBox("蓝灯", 0, 200);

            _redCheckBox.CheckedChanged += delegate { _fourColorLight.RedLightOn = _redCheckBox.Checked; };
            _yellowCheckBox.CheckedChanged += delegate { _fourColorLight.YellowLightOn = _yellowCheckBox.Checked; };
            _greenCheckBox.CheckedChanged += delegate { _fourColorLight.GreenLightOn = _greenCheckBox.Checked; };
            _blueCheckBox.CheckedChanged += delegate { _fourColorLight.BlueLightOn = _blueCheckBox.Checked; };

            Button runningButton = new Button
            {
                Text = "运行示例",
                Location = new Point(0, 248),
                Size = new Size(120, 30)
            };
            runningButton.Click += delegate { SetDemoStates(false, false, true, false); };

            Button alarmButton = new Button
            {
                Text = "报警示例",
                Location = new Point(136, 248),
                Size = new Size(120, 30)
            };
            alarmButton.Click += delegate { SetDemoStates(true, true, false, false); };

            Button resetButton = new Button
            {
                Text = "全部关闭",
                Location = new Point(272, 248),
                Size = new Size(120, 30)
            };
            resetButton.Click += delegate { SetDemoStates(false, false, false, false); };

            optionPanel.Controls.Add(resetButton);
            optionPanel.Controls.Add(alarmButton);
            optionPanel.Controls.Add(runningButton);
            optionPanel.Controls.Add(_blueCheckBox);
            optionPanel.Controls.Add(_greenCheckBox);
            optionPanel.Controls.Add(_yellowCheckBox);
            optionPanel.Controls.Add(_redCheckBox);
            optionPanel.Controls.Add(hintLabel);
            optionPanel.Controls.Add(titleLabel);

            Controls.Add(optionPanel);
            Controls.Add(controlPanel);
        }

        private CheckBox CreateLightCheckBox(string text, int x, int y)
        {
            return new CheckBox
            {
                AutoSize = false,
                Location = new Point(x, y),
                Size = new Size(160, 24),
                Text = text
            };
        }

        private void SetDemoStates(bool redOn, bool yellowOn, bool greenOn, bool blueOn)
        {
            _redCheckBox.Checked = redOn;
            _yellowCheckBox.Checked = yellowOn;
            _greenCheckBox.Checked = greenOn;
            _blueCheckBox.Checked = blueOn;
            _fourColorLight.SetLightStates(redOn, yellowOn, greenOn, blueOn);
        }
    }
}
