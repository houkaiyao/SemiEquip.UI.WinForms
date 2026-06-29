using System;
using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class WaferDemoPage : UserControl
    {
        private readonly WaferControl _wafer;
        private readonly ComboBox _stateComboBox;
        private readonly NumericUpDown _contentPaddingUpDown;
        private readonly NumericUpDown _borderWidthUpDown;
        private readonly Label _statusLabel;

        public WaferDemoPage()
        {
            Font = new Font("Times New Roman", 9f, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.FromArgb(230, 235, 242);
            ForeColor = Color.FromArgb(32, 38, 46);
            Padding = new Padding(8);

            Panel previewPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 460,
                BackColor = Color.FromArgb(230, 235, 242)
            };

            Label previewTitle = CreateTitleLabel("预览");
            previewTitle.Location = new Point(24, 20);
            previewPanel.Controls.Add(previewTitle);

            _wafer = new WaferControl
            {
                Location = new Point(100, 92),
                Size = new Size(100, 100),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            _wafer.Click += delegate { CycleState(); };
            previewPanel.Controls.Add(_wafer);

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(24, 380),
                Size = new Size(400, 150),
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

            Label titleLabel = CreateTitleLabel("Wafer 控件");
            titleLabel.Dock = DockStyle.Top;
            optionPanel.Controls.Add(titleLabel);

            GroupBox stateGroup = CreateGroupBox("状态", 0, 56, 390, 190);
            _stateComboBox = CreateComboBox(150, 34, 180);
            _stateComboBox.Items.Add(WaferState.Empty);
            _stateComboBox.Items.Add(WaferState.Processing);
            _stateComboBox.Items.Add(WaferState.Completed);
            _stateComboBox.SelectedItem = WaferState.Empty;
            _stateComboBox.SelectedIndexChanged += delegate { ApplySelectedState(); };
            Button emptyButton = CreateButton("Empty", 16, 86);
            emptyButton.Click += delegate { SetState(WaferState.Empty); };
            Button processingButton = CreateButton("Processing", 146, 86);
            processingButton.Click += delegate { SetState(WaferState.Processing); };
            Button completedButton = CreateButton("Completed", 276, 86);
            completedButton.Click += delegate { SetState(WaferState.Completed); };
            stateGroup.Controls.Add(CreateFieldLabel("State", 16, 37));
            stateGroup.Controls.Add(completedButton);
            stateGroup.Controls.Add(processingButton);
            stateGroup.Controls.Add(emptyButton);
            stateGroup.Controls.Add(_stateComboBox);

            GroupBox layoutGroup = CreateGroupBox("布局", 420, 56, 390, 190);
            _contentPaddingUpDown = CreateNumericUpDown(150, 34, 0, 60, 8);
            _contentPaddingUpDown.ValueChanged += delegate
            {
                _wafer.ContentPadding = (int)_contentPaddingUpDown.Value;
                UpdateStatusLabel();
            };
            _borderWidthUpDown = CreateNumericUpDown(150, 72, 1, 12, 2);
            _borderWidthUpDown.ValueChanged += delegate
            {
                _wafer.BorderWidth = (int)_borderWidthUpDown.Value;
                UpdateStatusLabel();
            };
            layoutGroup.Controls.Add(CreateFieldLabel("ContentPadding", 16, 37));
            layoutGroup.Controls.Add(CreateFieldLabel("BorderWidth", 16, 75));
            layoutGroup.Controls.Add(_borderWidthUpDown);
            layoutGroup.Controls.Add(_contentPaddingUpDown);

            GroupBox methodGroup = CreateGroupBox("配色与方法", 0, 276, 810, 140);
            Button paletteButton = CreateButton("项目配色", 16, 34);
            paletteButton.Click += delegate { ApplyDefaultColors(); };
            Button contrastButton = CreateButton("高对比色", 146, 34);
            contrastButton.Click += delegate { ApplyHighContrastColors(); };
            Button cycleButton = CreateButton("切换状态", 276, 34);
            cycleButton.Click += delegate { CycleState(); };
            methodGroup.Controls.Add(cycleButton);
            methodGroup.Controls.Add(contrastButton);
            methodGroup.Controls.Add(paletteButton);

            optionPanel.Controls.Add(methodGroup);
            optionPanel.Controls.Add(layoutGroup);
            optionPanel.Controls.Add(stateGroup);

            Controls.Add(optionPanel);
            Controls.Add(previewPanel);

            ApplyDefaultColors();
            UpdateStatusLabel();
        }

        private void ApplySelectedState()
        {
            if (_stateComboBox.SelectedItem is WaferState)
            {
                _wafer.State = (WaferState)_stateComboBox.SelectedItem;
                UpdateStatusLabel();
            }
        }

        private void SetState(WaferState state)
        {
            _stateComboBox.SelectedItem = state;
            _wafer.State = state;
            UpdateStatusLabel();
        }

        private void CycleState()
        {
            switch (_wafer.State)
            {
                case WaferState.Empty:
                    SetState(WaferState.Processing);
                    break;
                case WaferState.Processing:
                    SetState(WaferState.Completed);
                    break;
                case WaferState.Completed:
                default:
                    SetState(WaferState.Empty);
                    break;
            }
        }

        private void ApplyDefaultColors()
        {
            _wafer.EmptyWaferColor = Color.White;
            _wafer.ProcessingWaferColor = Color.FromArgb(55, 137, 255);
            _wafer.CompletedWaferColor = Color.FromArgb(46, 184, 92);
            _wafer.BorderColor = Color.FromArgb(80, 104, 132);
            UpdateStatusLabel();
        }

        private void ApplyHighContrastColors()
        {
            _wafer.EmptyWaferColor = Color.FromArgb(245, 247, 250);
            _wafer.ProcessingWaferColor = Color.FromArgb(255, 176, 48);
            _wafer.CompletedWaferColor = Color.FromArgb(20, 170, 92);
            _wafer.BorderColor = Color.FromArgb(32, 38, 46);
            UpdateStatusLabel();
        }

        private void UpdateStatusLabel()
        {
            _statusLabel.Text = string.Format(
                "State: {0}\r\nContentPadding: {1}\r\nBorderWidth: {2}\r\nEmptyWaferColor: {3}\r\nProcessingWaferColor: {4}\r\nCompletedWaferColor: {5}",
                _wafer.State,
                _wafer.ContentPadding,
                _wafer.BorderWidth,
                _wafer.EmptyWaferColor,
                _wafer.ProcessingWaferColor,
                _wafer.CompletedWaferColor);
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
                Size = new Size(130, 22),
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
