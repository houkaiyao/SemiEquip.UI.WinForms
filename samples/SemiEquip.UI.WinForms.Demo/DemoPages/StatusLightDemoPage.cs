using System;
using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class StatusLightDemoPage : UserControl
    {
        private readonly StatusLightControl _statusLight;
        private readonly StatusLightControl _statusLight2;
        private readonly Label _statusLabel;

        public StatusLightDemoPage()
        {
            BackColor = Color.FromArgb(18, 22, 28);
            ForeColor = Color.FromArgb(230, 235, 242);
            Padding = new Padding(8);

            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = Color.FromArgb(18, 22, 28)
            };

            _statusLight = new StatusLightControl
            {
                Location = new Point(40, 40),
                Size = new Size(120, 120),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            controlPanel.Controls.Add(_statusLight);

            _statusLight2 = new StatusLightControl
            {
                Location = new Point(40, 200),
                Size = new Size(20, 20),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            controlPanel.Controls.Add(_statusLight2);

            Panel optionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Color.FromArgb(18, 22, 28)
            };

            Label titleLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 36,
                Text = "状态灯控件",
                Font = new Font(Font.FontFamily, 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 56),
                Size = new Size(320, 32),
                Text = "当前颜色：绿色"
            };

            Button redButton = CreateColorButton("红色", Color.FromArgb(226, 64, 64), 0, 104);
            Button greenButton = CreateColorButton("绿色", Color.FromArgb(46, 184, 92), 92, 104);
            Button yellowButton = CreateColorButton("黄色", Color.FromArgb(245, 190, 58), 184, 104);
            Button blueButton = CreateColorButton("蓝色", Color.FromArgb(55, 137, 255), 276, 104);

            optionPanel.Controls.Add(blueButton);
            optionPanel.Controls.Add(yellowButton);
            optionPanel.Controls.Add(greenButton);
            optionPanel.Controls.Add(redButton);
            optionPanel.Controls.Add(_statusLabel);
            optionPanel.Controls.Add(titleLabel);

            Controls.Add(optionPanel);
            Controls.Add(controlPanel);
        }

        private Button CreateColorButton(string text, Color color, int x, int y)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(78, 30),
                BackColor = color,
                ForeColor = ResolveTextColor(color),
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(90, 102, 118);
            button.Click += delegate { SetLightColor(text, color); };

            return button;
        }

        private void SetLightColor(string colorName, Color color)
        {
            _statusLight.LightColor = color;
            _statusLabel.Text = string.Format("当前颜色：{0}", colorName);
        }

        private static Color ResolveTextColor(Color color)
        {
            int brightness = (color.R * 299 + color.G * 587 + color.B * 114) / 1000;
            return brightness >= 150 ? Color.FromArgb(20, 24, 30) : Color.White;
        }
    }
}
