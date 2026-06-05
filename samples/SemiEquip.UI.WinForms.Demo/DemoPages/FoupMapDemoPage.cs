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
        private readonly NumericUpDown _slotCountEditor;
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
                Width = 260,
                BackColor = Color.FromArgb(18, 22, 28)
            };
            
            _foupMap = new FoupMapControl
            {
                Location = new Point(24, 24),
                Size = new Size(180, 360),
                SlotCount = 25,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                ShowSlotNumbers = true
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

            Label slotCountLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 56),
                Size = new Size(160, 22),
                Text = "Slot 数量"
            };

            _slotCountEditor = new NumericUpDown
            {
                Minimum = 1,
                Maximum = FoupMapControl.MaxSlotCount,
                Value = 25,
                Location = new Point(0, 82),
                Size = new Size(160, 24)
            };
            _slotCountEditor.ValueChanged += delegate
            {
                _foupMap.SlotCount = (int)_slotCountEditor.Value;
                LoadSampleSlots();
            };

            Button resetButton = new Button
            {
                Text = "重置示例",
                Location = new Point(0, 124),
                Size = new Size(160, 30)
            };
            resetButton.Click += delegate { LoadSampleSlots(); };

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 184),
                Size = new Size(320, 92),
                Text = "点击 Slot 查看槽位信息。"
            };

            _timerLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 296),
                Size = new Size(320, 48),
                Text = "Timer：等待刷新..."
            };

            optionPanel.Controls.Add(_timerLabel);
            optionPanel.Controls.Add(_statusLabel);
            optionPanel.Controls.Add(resetButton);
            optionPanel.Controls.Add(_slotCountEditor);
            optionPanel.Controls.Add(slotCountLabel);
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
            _timerLabel.Text = string.Format("Timer：Slot {0:00} -> {1}", _timerSlot, nextState);
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
