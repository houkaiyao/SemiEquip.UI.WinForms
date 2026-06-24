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
            BackColor = Color.FromArgb(18, 22, 28);
            ForeColor = Color.FromArgb(230, 235, 242);
            Padding = new Padding(8);

            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 350,
                BackColor = Color.FromArgb(18, 22, 28)
            };

            _foupMap = new FoupMapControl
            {
                Location = new Point(0, 0),
                Size = new Size(300, 800),
                SlotCount = 25,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                ShowSlotNumbers = true,
                SlotTextDisplayMode = FoupSlotTextDisplayMode.WaferId,
                SlotTextColor = Color.White,
                ShowSelectionCheckBoxes = true
            };
            _foupMap.SlotClick += OnFoupSlotClick;
            controlPanel.Controls.Add(_foupMap);

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
                Text = "FOUP Map 控件",
                Font = new Font(Font.FontFamily, 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button resetButton = new Button
            {
                Text = "重置示例",
                Location = new Point(0, 56),
                Size = new Size(160, 30)
            };
            resetButton.Click += delegate { LoadSampleSlots(); };

            ComboBox slotTextDisplaySelector = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(0, 96),
                Size = new Size(180, 24)
            };
            slotTextDisplaySelector.Items.Add("Slot 内不显示文字");
            slotTextDisplaySelector.Items.Add("Slot 内显示 WaferID");
            slotTextDisplaySelector.Items.Add("Slot 内显示 SlotData");
            slotTextDisplaySelector.SelectedIndexChanged += delegate
            {
                _foupMap.SlotTextDisplayMode = (FoupSlotTextDisplayMode)slotTextDisplaySelector.SelectedIndex;
            };
            slotTextDisplaySelector.SelectedIndex = 1;

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
            CheckBox showSlotNumberToolTipCheckBox = new CheckBox
            {
                AutoSize = false,
                Location = new Point(0, 160),
                Size = new Size(190, 24),
                Text = "悬浮显示 Slot 编号",
                Checked = true
            };
            showSlotNumberToolTipCheckBox.CheckedChanged += delegate
            {
                _foupMap.ShowSlotNumberInToolTip = showSlotNumberToolTipCheckBox.Checked;
            };

            CheckBox showWaferIdToolTipCheckBox = new CheckBox
            {
                AutoSize = false,
                Location = new Point(0, 188),
                Size = new Size(190, 24),
                Text = "悬浮显示 WaferID",
                Checked = true
            };
            showWaferIdToolTipCheckBox.CheckedChanged += delegate
            {
                _foupMap.ShowWaferIdInToolTip = showWaferIdToolTipCheckBox.Checked;
            };

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 224),
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
                Location = new Point(0, 336),
                Size = new Size(320, 48),
                Text = "Timer：等待刷新..."
            };

            optionPanel.Controls.Add(_timerLabel);
            optionPanel.Controls.Add(_statusLabel);
            optionPanel.Controls.Add(showWaferIdToolTipCheckBox);
            optionPanel.Controls.Add(showSlotNumberToolTipCheckBox);
            optionPanel.Controls.Add(showSelectionCheckBox);
            optionPanel.Controls.Add(slotTextDisplaySelector);
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
                _foupMap.SetSlotData(slot, string.Format("SLOT-DATA-{0:00}", slot));

                if (slot % 9 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.Abnormal);
                    _foupMap.SetWaferId(slot, string.Format("W{0:000}", slot));
                }
                else if (slot % 4 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.AfterProcess);
                    _foupMap.SetWaferId(slot, string.Format("W{0:000}", slot));
                }
                else if (slot % 3 == 0)
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.BeforeProcess);
                    _foupMap.SetWaferId(slot, string.Format("W{0:000}", slot));
                }
                else
                {
                    _foupMap.SetSlotState(slot, FoupSlotState.Empty);
                    _foupMap.SetWaferId(slot, string.Empty);
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
            _foupMap.SetWaferId(
                _timerSlot,
                nextState == FoupSlotState.Empty ? string.Empty : string.Format("W{0:000}", _timerSlot));
            _timerLabel.Text = string.Format("Timer：Slot {0:00} -> {1}", _timerSlot, nextState);

            _foupMap.SetSlotData(1,"1001");
            _foupMap.SetWaferId(1, "1001");
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
            _statusLabel.Text += string.Format("\r\nWaferID: {0}", _foupMap.GetWaferId(e.SlotNumber));

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
