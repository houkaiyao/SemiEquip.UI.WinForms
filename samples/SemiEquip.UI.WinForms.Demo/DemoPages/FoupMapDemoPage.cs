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

            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 350,
                BackColor = Color.FromArgb(230, 235, 242)
            };

            _foupMap = new FoupMapControl
            {
                Location = new Point(0, 0),
                Size = new Size(200, 400),
                SlotCount = 25,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                ShowSlotNumbers = true,
                ShowSlotText = true,
                ShowSlotTip = true,
                SlotTextColor = Color.FromArgb(18, 22, 28),
                ShowSelectionCheckBoxes = true
            };
            _foupMap.SlotClick += OnFoupSlotClick;
            controlPanel.Controls.Add(_foupMap);

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
                Text = "FOUP Map 控件",
                Font = new Font("Times New Roman", 12f, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button resetButton = new Button
            {
                Text = "重置示例",
                Location = new Point(0, 56),
                Size = new Size(160, 30)
            };
            resetButton.Click += delegate { LoadSampleSlots(); };

            CheckBox showSlotTextCheckBox = new CheckBox
            {
                Location = new Point(0, 96),
                Size = new Size(180, 24),
                Text = "显示 SlotText",
                Checked = true
            };
            showSlotTextCheckBox.CheckedChanged += delegate
            {
                _foupMap.ShowSlotText = showSlotTextCheckBox.Checked;
            };

            CheckBox showSelectionCheckBox = new CheckBox
            {
                AutoSize = false,
                Location = new Point(0, 132),
                Size = new Size(180, 24),
                Text = "显示选片框",
                Checked = true
            };
            showSelectionCheckBox.CheckedChanged += delegate
            {
                _foupMap.ShowSelectionCheckBoxes = showSelectionCheckBox.Checked;
            };
            CheckBox showSlotTipCheckBox = new CheckBox
            {
                AutoSize = false,
                Location = new Point(0, 160),
                Size = new Size(190, 24),
                Text = "显示 SlotTipText",
                Checked = true
            };
            showSlotTipCheckBox.CheckedChanged += delegate
            {
                _foupMap.ShowSlotTip = showSlotTipCheckBox.Checked;
            };

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 196),
                Size = new Size(320, 92),
                Text = "点击 Slot 查看槽位信息。"
            };
            _foupMap.ChooseMapDataChanged += delegate
            {
                _statusLabel.Text = string.Format("ChooseMapData: {0}", _foupMap.ChooseMapData);
            };

            _timerLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 308),
                Size = new Size(320, 48),
                Text = "Timer：等待刷新..."
            };

            optionPanel.Controls.Add(_timerLabel);
            optionPanel.Controls.Add(_statusLabel);
            optionPanel.Controls.Add(showSlotTipCheckBox);
            optionPanel.Controls.Add(showSelectionCheckBox);
            optionPanel.Controls.Add(showSlotTextCheckBox);
            optionPanel.Controls.Add(resetButton);
            optionPanel.Controls.Add(titleLabel);

            Controls.Add(optionPanel);
            Controls.Add(controlPanel);

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

                if (slot % 9 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.Abnormal);
                }
                else if (slot % 4 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.AfterProcess);
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
            _statusLabel.Text = string.Format(
                "Slot {0:00}\r\n状态: {1}\r\n颜色: {2}",
                e.SlotNumber,
                _foupMap.GetSlotState(e.SlotNumber),
                _foupMap.GetSlotColor(e.SlotNumber).Name);
            _statusLabel.Text += string.Format("\r\nSlotText: {0}", _foupMap.GetSlotText(e.SlotNumber));
            _statusLabel.Text += string.Format("\r\nSlotTipText: {0}", _foupMap.GetSlotTipText(e.SlotNumber));

            _foupMap.SetSlotState(e.SlotNumber, PLCStateToSlotState(900));
        }
        /// <summary>
        /// 模拟 PLC 状态转换为 Slot 状态的逻辑
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static FoupSlotState PLCStateToSlotState(int state)
        {
            if (state == 900)
            {
                return FoupSlotState.BeforeProcess;
            }

            if (state == 800)
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
