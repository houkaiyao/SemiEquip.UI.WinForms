using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class ScrollingTextDemoPage : UserControl
    {
        private readonly ScrollingTextControl _scrollingText;
        private readonly TextBox _textEditor;
        private readonly ComboBox _directionSelector;
        private readonly NumericUpDown _scrollStepEditor;
        private readonly NumericUpDown _scrollIntervalEditor;

        public ScrollingTextDemoPage()
        {
            BackColor = Color.FromArgb(18, 22, 28);
            ForeColor = Color.FromArgb(230, 235, 242);
            Padding = new Padding(8);

            Panel previewPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 96,
                Padding = new Padding(0, 18, 0, 24),
                BackColor = Color.FromArgb(18, 22, 28)
            };

            _scrollingText = new ScrollingTextControl
            {
                Dock = DockStyle.Fill,
                Text = "设备运行中：Load Port 1 已准备就绪，Robot 正在等待下一步命令。0034",
                BackColor = Color.FromArgb(32, 38, 48),
                ForeColor = Color.FromArgb(236, 242, 248)
            };
            previewPanel.Controls.Add(_scrollingText);

            Panel optionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 8, 0, 0),
                BackColor = Color.FromArgb(18, 22, 28)
            };

            Label titleLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 0),
                Size = new Size(360, 30),
                Text = "滚动文字控件",
                Font = new Font(Font.FontFamily, 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label textLabel = CreateLabel("显示文字", 0, 52);
            _textEditor = new TextBox
            {
                Location = new Point(0, 78),
                Size = new Size(560, 24),
                Text = _scrollingText.Text
            };
            _textEditor.TextChanged += delegate { _scrollingText.Text = _textEditor.Text; };

            Label directionLabel = CreateLabel("滚动方向", 0, 122);
            _directionSelector = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(0, 148),
                Size = new Size(160, 24)
            };
            _directionSelector.Items.Add("从右到左");
            _directionSelector.Items.Add("从左到右");
            _directionSelector.SelectedIndexChanged += delegate
            {
                _scrollingText.ScrollDirection = _directionSelector.SelectedIndex == 1
                    ? ScrollingTextDirection.LeftToRight
                    : ScrollingTextDirection.RightToLeft;
            };
            _directionSelector.SelectedIndex = 0;

            Label stepLabel = CreateLabel("滚动步长", 184, 122);
            _scrollStepEditor = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 20,
                Value = _scrollingText.ScrollStep,
                Location = new Point(184, 148),
                Size = new Size(92, 24)
            };
            _scrollStepEditor.ValueChanged += delegate { _scrollingText.ScrollStep = (int)_scrollStepEditor.Value; };

            Label intervalLabel = CreateLabel("刷新间隔", 300, 122);
            _scrollIntervalEditor = new NumericUpDown
            {
                Minimum = 10,
                Maximum = 500,
                Value = _scrollingText.ScrollInterval,
                Location = new Point(300, 148),
                Size = new Size(92, 24)
            };
            _scrollIntervalEditor.ValueChanged += delegate { _scrollingText.ScrollInterval = (int)_scrollIntervalEditor.Value; };

            Button lightThemeButton = CreateButton("浅色背景", 0, 204);
            lightThemeButton.Click += delegate
            {
                _scrollingText.BackColor = Color.FromArgb(236, 242, 248);
                _scrollingText.ForeColor = Color.FromArgb(28, 36, 46);
            };

            Button darkThemeButton = CreateButton("深色背景", 116, 204);
            darkThemeButton.Click += delegate
            {
                _scrollingText.BackColor = Color.FromArgb(32, 38, 48);
                _scrollingText.ForeColor = Color.FromArgb(236, 242, 248);
            };

            Button warningThemeButton = CreateButton("警告配色", 232, 204);
            warningThemeButton.Click += delegate
            {
                _scrollingText.BackColor = Color.FromArgb(255, 242, 204);
                _scrollingText.ForeColor = Color.FromArgb(90, 58, 12);
            };

            Button fontButton = CreateButton("切换字体", 348, 204);
            fontButton.Click += delegate
            {
                _scrollingText.Font = _scrollingText.Font.Bold
                    ? new Font("Segoe UI", 10f, FontStyle.Regular)
                    : new Font("Segoe UI", 12f, FontStyle.Bold);
            };

            optionPanel.Controls.Add(fontButton);
            optionPanel.Controls.Add(warningThemeButton);
            optionPanel.Controls.Add(darkThemeButton);
            optionPanel.Controls.Add(lightThemeButton);
            optionPanel.Controls.Add(_scrollIntervalEditor);
            optionPanel.Controls.Add(intervalLabel);
            optionPanel.Controls.Add(_scrollStepEditor);
            optionPanel.Controls.Add(stepLabel);
            optionPanel.Controls.Add(_directionSelector);
            optionPanel.Controls.Add(directionLabel);
            optionPanel.Controls.Add(_textEditor);
            optionPanel.Controls.Add(textLabel);
            optionPanel.Controls.Add(titleLabel);

            Controls.Add(optionPanel);
            Controls.Add(previewPanel);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                AutoSize = false,
                Location = new Point(x, y),
                Size = new Size(120, 22),
                Text = text
            };
        }

        private Button CreateButton(string text, int x, int y)
        {
            return new Button
            {
                Location = new Point(x, y),
                Size = new Size(104, 30),
                Text = text
            };
        }
    }
}
