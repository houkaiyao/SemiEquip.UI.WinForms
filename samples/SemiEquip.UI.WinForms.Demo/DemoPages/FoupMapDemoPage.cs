using System;
using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class FoupMapDemoPage : UserControl
    {
        private readonly FoupMapControl _foupMap;
        private readonly Label _statusLabel;
        private readonly Label _timerLabel;
        private readonly Timer _demoTimer;
        private int _timerSlot;

        public FoupMapDemoPage()
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

            _foupMap = new FoupMapControl
            {
                Location = new Point(48, 72),
                Size = new Size(260, 560),
                SlotCount = 25,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                ShowSlotNumbers = true,
                ShowSlotText = true,
                ShowSlotTip = true,
                SlotTextColor = Color.FromArgb(18, 22, 28),
                ShowSelectionCheckBoxes = true
            };
            _foupMap.SlotClick += OnFoupSlotClick;
            previewPanel.Controls.Add(_foupMap);

            Panel optionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Color.FromArgb(230, 235, 242)
            };

            Label titleLabel = CreateTitleLabel("FOUP Map 控件");
            titleLabel.Dock = DockStyle.Top;
            optionPanel.Controls.Add(titleLabel);

            GroupBox viewGroup = CreateGroupBox("显示属性", 0, 56, 360, 230);
            CheckBox showNumbersCheckBox = CreateCheckBox("ShowSlotNumbers", 16, 32, true);
            showNumbersCheckBox.CheckedChanged += delegate { _foupMap.ShowSlotNumbers = showNumbersCheckBox.Checked; };
            CheckBox showSlotTextCheckBox = CreateCheckBox("ShowSlotText", 16, 66, true);
            showSlotTextCheckBox.CheckedChanged += delegate { _foupMap.ShowSlotText = showSlotTextCheckBox.Checked; };
            CheckBox showSelectionCheckBox = CreateCheckBox("ShowSelectionCheckBoxes", 16, 100, true);
            showSelectionCheckBox.CheckedChanged += delegate { _foupMap.ShowSelectionCheckBoxes = showSelectionCheckBox.Checked; };
            CheckBox showSlotTipCheckBox = CreateCheckBox("ShowSlotTip", 16, 134, true);
            showSlotTipCheckBox.CheckedChanged += delegate { _foupMap.ShowSlotTip = showSlotTipCheckBox.Checked; };
            CheckBox autoTextCheckBox = CreateCheckBox("AutoScaleSlotTextFont", 16, 168, true);
            autoTextCheckBox.CheckedChanged += delegate { _foupMap.AutoScaleSlotTextFont = autoTextCheckBox.Checked; };
            viewGroup.Controls.Add(autoTextCheckBox);
            viewGroup.Controls.Add(showSlotTipCheckBox);
            viewGroup.Controls.Add(showSelectionCheckBox);
            viewGroup.Controls.Add(showSlotTextCheckBox);
            viewGroup.Controls.Add(showNumbersCheckBox);

            GroupBox layoutGroup = CreateGroupBox("布局属性", 390, 56, 390, 230);
            NumericUpDown contentPaddingUpDown = CreateNumericUpDown(150, 34, 0, 30, 5);
            contentPaddingUpDown.ValueChanged += delegate { _foupMap.ContentPadding = (int)contentPaddingUpDown.Value; };
            NumericUpDown textPaddingUpDown = CreateNumericUpDown(150, 72, 0, 20, 4);
            textPaddingUpDown.ValueChanged += delegate { _foupMap.SlotTextPadding = (int)textPaddingUpDown.Value; };
            NumericUpDown widthUpDown = CreateNumericUpDown(150, 110, 120, 420, _foupMap.Width);
            widthUpDown.ValueChanged += delegate { _foupMap.Width = (int)widthUpDown.Value; };
            NumericUpDown heightUpDown = CreateNumericUpDown(150, 148, 260, 720, _foupMap.Height);
            heightUpDown.ValueChanged += delegate { _foupMap.Height = (int)heightUpDown.Value; };
            layoutGroup.Controls.Add(CreateFieldLabel("ContentPadding", 16, 37));
            layoutGroup.Controls.Add(CreateFieldLabel("SlotTextPadding", 16, 75));
            layoutGroup.Controls.Add(CreateFieldLabel("Width", 16, 113));
            layoutGroup.Controls.Add(CreateFieldLabel("Height", 16, 151));
            layoutGroup.Controls.Add(heightUpDown);
            layoutGroup.Controls.Add(widthUpDown);
            layoutGroup.Controls.Add(textPaddingUpDown);
            layoutGroup.Controls.Add(contentPaddingUpDown);

            GroupBox methodGroup = CreateGroupBox("方法与数据", 0, 312, 780, 180);
            Button resetButton = CreateButton("Load Sample", 16, 34);
            resetButton.Click += delegate { LoadSampleSlots(); };
            Button clearSlotsButton = CreateButton("ClearSlots", 146, 34);
            clearSlotsButton.Click += delegate { _foupMap.ClearSlots(); UpdateStatus("ClearSlots 已执行。"); };
            Button clearTextsButton = CreateButton("ClearSlotTexts", 276, 34);
            clearTextsButton.Click += delegate { _foupMap.ClearSlotTexts(); UpdateStatus("ClearSlotTexts 已执行。"); };
            Button clearTipsButton = CreateButton("ClearSlotTipTexts", 406, 34);
            clearTipsButton.Click += delegate { _foupMap.ClearSlotTipTexts(); UpdateStatus("ClearSlotTipTexts 已执行。"); };
            Button selectEndsButton = CreateButton("Choose Ends", 536, 34);
            selectEndsButton.Click += delegate { _foupMap.ChooseMapData = "1000000000000000000000001"; };
            Button clearSelectButton = CreateButton("Clear Selection", 666, 34);
            clearSelectButton.Click += delegate { _foupMap.ClearSlotSelections(); };
            Button customColorButton = CreateButton("Slot 5 Gold", 16, 86);
            customColorButton.Click += delegate
            {
                _foupMap.SetSlotColor(5, Color.Gold);
                _foupMap.SetSlotText(5, "GOLD");
                UpdateStatus("SetSlotColor(5, Color.Gold) 已执行。");
            };
            Button abnormalButton = CreateButton("Slot 9 Abnormal", 146, 86);
            abnormalButton.Click += delegate
            {
                _foupMap.SetSlotState(9, FoupSlotState.Abnormal);
                _foupMap.SetSlotTipText(9, "手动设置为 Abnormal");
                UpdateStatus("SetSlotState(9, Abnormal) 已执行。");
            };
            methodGroup.Controls.Add(abnormalButton);
            methodGroup.Controls.Add(customColorButton);
            methodGroup.Controls.Add(clearSelectButton);
            methodGroup.Controls.Add(selectEndsButton);
            methodGroup.Controls.Add(clearTipsButton);
            methodGroup.Controls.Add(clearTextsButton);
            methodGroup.Controls.Add(clearSlotsButton);
            methodGroup.Controls.Add(resetButton);

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 526),
                Size = new Size(780, 120),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Text = "点击 Slot 或执行方法查看状态。"
            };

            _timerLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 664),
                Size = new Size(780, 48),
                Text = "Timer：等待刷新..."
            };
            _foupMap.ChooseMapDataChanged += delegate
            {
                UpdateStatus(string.Format("ChooseMapData: {0}", _foupMap.ChooseMapData));
            };

            optionPanel.Controls.Add(_timerLabel);
            optionPanel.Controls.Add(_statusLabel);
            optionPanel.Controls.Add(methodGroup);
            optionPanel.Controls.Add(layoutGroup);
            optionPanel.Controls.Add(viewGroup);

            Controls.Add(optionPanel);
            Controls.Add(previewPanel);

            LoadSampleSlots();

            _demoTimer = new Timer
            {
                Interval = 1000
            };
            _demoTimer.Tick += OnDemoTimerTick;
            _demoTimer.Start();
        }

        private void LoadSampleSlots()
        {
            _foupMap.ClearSlots();
            _timerSlot = 0;

            for (int slot = 1; slot <= _foupMap.SlotCount; slot++)
            {
                _foupMap.SetSlotText(slot, string.Format("S{0:00}", slot));
                _foupMap.SetSlotTipText(slot, string.Format("Slot {0:00} 示例提示文本", slot));

                if (slot % 11 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.Abnormal);
                }
                else if (slot % 5 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.AfterProcess);
                }
                else if (slot % 4 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.Processing);
                }
                else if (slot % 3 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.BeforeProcess);
                }
                else
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.Empty);
                }
            }

            UpdateStatus("Load Sample 已执行。");
        }

        private void OnDemoTimerTick(object sender, EventArgs e)
        {
            if (_foupMap.SlotCount <= 0)
            {
                return;
            }

            _timerSlot++;
            if (_timerSlot > _foupMap.SlotCount)
            {
                _timerSlot = 1;
            }

            FoupSlotState nextState = GetNextState(_foupMap.GetSlotState(_timerSlot));
            _foupMap.SetSlotState(_timerSlot, nextState);
            _foupMap.SetSlotText(_timerSlot, nextState == FoupSlotState.Empty ? string.Empty : string.Format("S{0:00}", _timerSlot));
            _foupMap.SetSlotTipText(_timerSlot, string.Format("Slot {0:00} -> {1}", _timerSlot, nextState));
            _timerLabel.Text = string.Format("Timer：Slot {0:00} -> {1}", _timerSlot, nextState);

            _foupMap.SetSlotText(1, "0000 999");
            _foupMap.SetSlotText(2, "0000  00");
            _foupMap.SetSlotTipText(1, "ASDFGHJKL123456789");
        }

        private static FoupSlotState GetNextState(FoupSlotState state)
        {
            switch (state)
            {
                case FoupSlotState.Empty:
                    return FoupSlotState.BeforeProcess;
                case FoupSlotState.BeforeProcess:
                    return FoupSlotState.Processing;
                case FoupSlotState.Processing:
                    return FoupSlotState.AfterProcess;
                case FoupSlotState.AfterProcess:
                    return FoupSlotState.Abnormal;
                case FoupSlotState.Abnormal:
                case FoupSlotState.Custom:
                default:
                    return FoupSlotState.Empty;
            }
        }

        private void OnFoupSlotClick(object sender, FoupSlotClickEventArgs e)
        {
            string message = string.Format(
                "Slot {0:00}\r\n状态: {1}\r\n颜色: {2}\r\nSlotText: {3}\r\nSlotTipText: {4}\r\nBounds: {5}",
                e.SlotNumber,
                _foupMap.GetSlotState(e.SlotNumber),
                _foupMap.GetSlotColor(e.SlotNumber).Name,
                _foupMap.GetSlotText(e.SlotNumber),
                _foupMap.GetSlotTipText(e.SlotNumber),
                _foupMap.GetSlotBounds(e.SlotNumber));
            UpdateStatus(message);

            _foupMap.SetSlotState(e.SlotNumber, PLCStateToSlotState(900));
        }

        private void UpdateStatus(string text)
        {
            _statusLabel.Text = text;
        }

        /// <summary>
        /// 模拟 PLC 状态转换为 Slot 状态的逻辑
        /// </summary>
        private static FoupSlotState PLCStateToSlotState(int state)
        {
            if (state == 900)
            {
                return FoupSlotState.BeforeProcess;
            }

            if (state == 800)
            {
                return FoupSlotState.Processing;
            }

            if (state == 700)
            {
                return FoupSlotState.AfterProcess;
            }

            if (state == 0)
            {
                return FoupSlotState.Empty;
            }

            if (state == 402)
            {
                return FoupSlotState.Abnormal;
            }

            return FoupSlotState.Empty;
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

        private static GroupBox CreateGroupBox(string text, int x, int y, int width, int height)
        {
            return new GroupBox
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height)
            };
        }

        private static CheckBox CreateCheckBox(string text, int x, int y, bool isChecked)
        {
            return new CheckBox
            {
                AutoSize = false,
                Location = new Point(x, y),
                Size = new Size(220, 26),
                Text = text,
                Checked = isChecked
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && _demoTimer != null)
            {
                _demoTimer.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
