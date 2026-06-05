using System;
using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Demo.DemoPages
{
    internal sealed class AlarmListDemoPage : UserControl
    {
        private readonly AlarmListControl _alarmList;
        private readonly Label _statusLabel;
        private readonly CheckBox _limitDisplayCheckBox;
        private readonly NumericUpDown _maxDisplayCountEditor;
        private readonly ComboBox _displayOrderSelector;
        private int _alarmIndex = 1;

        public AlarmListDemoPage()
        {
            BackColor = Color.FromArgb(18, 22, 28);
            ForeColor = Color.FromArgb(230, 235, 242);
            Padding = new Padding(8);

            Panel optionPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 160,
                Padding = new Padding(0, 0, 0, 12),
                BackColor = Color.FromArgb(18, 22, 28)
            };

            Label titleLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 0),
                Size = new Size(360, 30),
                Text = "Alarm List 控件",
                Font = new Font(Font.FontFamily, 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button addInfoButton = CreateButton("添加提示", 0, 46);
            addInfoButton.Click += delegate { AddDemoAlarm(AlarmLevel.Info); };

            Button addWarningButton = CreateButton("添加警告", 108, 46);
            addWarningButton.Click += delegate { AddDemoAlarm(AlarmLevel.Warning); };

            Button addAlarmButton = CreateButton("添加报警", 216, 46);
            addAlarmButton.Click += delegate { AddDemoAlarm(AlarmLevel.Alarm); };

            Button addCriticalButton = CreateButton("添加严重", 324, 46);
            addCriticalButton.Click += delegate { AddDemoAlarm(AlarmLevel.Critical); };

            Button resetButton = CreateButton("重置示例", 432, 46);
            resetButton.Click += delegate { LoadSampleAlarms(); };

            Button clearButton = CreateButton("清空", 540, 46);
            clearButton.Click += delegate
            {
                _alarmList.ClearAlarms();
                _statusLabel.Text = "报警列表已清空。";
                _alarmIndex = 0;
            };

            _limitDisplayCheckBox = new CheckBox
            {
                AutoSize = false,
                Location = new Point(0, 86),
                Size = new Size(112, 24),
                Text = "限制显示"
            };
            _limitDisplayCheckBox.CheckedChanged += delegate
            {
                _alarmList.LimitDisplayCount = _limitDisplayCheckBox.Checked;
                UpdateCountStatus();
            };

            _maxDisplayCountEditor = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 100,
                Value = 3,
                Location = new Point(124, 86),
                Size = new Size(72, 24)
            };
            _maxDisplayCountEditor.ValueChanged += delegate
            {
                _alarmList.MaxDisplayCount = (int)_maxDisplayCountEditor.Value;
                UpdateCountStatus();
            };

            _displayOrderSelector = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(216, 86),
                Size = new Size(160, 24)
            };
            _displayOrderSelector.Items.Add("正序显示");
            _displayOrderSelector.Items.Add("倒序显示");
            _displayOrderSelector.SelectedIndexChanged += delegate
            {
                if (_alarmList == null)
                {
                    return;
                }

                _alarmList.DisplayOrder = _displayOrderSelector.SelectedIndex == 1
                    ? AlarmDisplayOrder.Descending
                    : AlarmDisplayOrder.Ascending;
                UpdateCountStatus();
            };
            _displayOrderSelector.SelectedIndex = 0;

            _statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 124),
                Size = new Size(720, 24),
                Text = "选中或双击报警行查看事件反馈。"
            };

            optionPanel.Controls.Add(_statusLabel);
            optionPanel.Controls.Add(_displayOrderSelector);
            optionPanel.Controls.Add(_maxDisplayCountEditor);
            optionPanel.Controls.Add(_limitDisplayCheckBox);
            optionPanel.Controls.Add(clearButton);
            optionPanel.Controls.Add(resetButton);
            optionPanel.Controls.Add(addCriticalButton);
            optionPanel.Controls.Add(addAlarmButton);
            optionPanel.Controls.Add(addWarningButton);
            optionPanel.Controls.Add(addInfoButton);
            optionPanel.Controls.Add(titleLabel);

            _alarmList = new AlarmListControl
            {
                Dock = DockStyle.Fill,
            };
            _alarmList.MaxDisplayCount = (int)_maxDisplayCountEditor.Value;
            _alarmList.AlarmSelected += OnAlarmSelected;
            _alarmList.AlarmDoubleClick += OnAlarmDoubleClick;

            Controls.Add(_alarmList);
            Controls.Add(optionPanel);

            LoadSampleAlarms();
        }

        private Button CreateButton(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(96, 30)
            };
        }

        private void LoadSampleAlarms()
        {
            _alarmIndex = 1;
            _alarmList.ClearAlarms();
            _alarmList.AddAlarm(CreateAlarm(AlarmLevel.Info, "设备提示", "Load Port 已准备就绪。"));
            _alarmList.AddAlarm(CreateAlarm(AlarmLevel.Warning, "真空警告", "Chamber 压力接近警告上限。"));
            _alarmList.AddAlarm(CreateAlarm(AlarmLevel.Alarm, "传输报警", "Robot 取片动作超时，请检查轴状态。"));
            _alarmList.AddAlarm(CreateAlarm(AlarmLevel.Critical, "安全互锁", "门禁互锁断开，设备已停止运行。"));
        }

        private void AddDemoAlarm(AlarmLevel level)
        {
            //获取ALARM数量
            
            string alarmEvent;
            string description;
            int alarmcount = _alarmList.AlarmCount;
            switch (level)
            {
                case AlarmLevel.Warning:
                    alarmEvent = "温度警告";
                    description = "Heater 温度接近工艺限制。";
                    break;
                case AlarmLevel.Alarm:
                    alarmEvent = "动作报警";
                    description = "执行机构未在设定时间内到达目标位置。";
                    break;
                case AlarmLevel.Critical:
                    alarmEvent = "严重报警";
                    description = "关键安全条件不满足，设备需要立即处理。";
                    break;
                case AlarmLevel.Info:
                default:
                    alarmEvent = "系统提示";
                    description = "设备状态发生普通提示事件。";
                    break;
            }

            _alarmList.AddAlarm(CreateAlarm(level, alarmEvent, description));
            UpdateCountStatus();
        }

        private AlarmInfo CreateAlarm(AlarmLevel level, string alarmEvent, string description)
        {
            AlarmInfo alarm = new AlarmInfo(
                string.Format("ALM-{0:0000}", _alarmIndex),
                alarmEvent,
                description,
                level);
            alarm.OccurTime = DateTime.Now;
            _alarmIndex++;

            return alarm;
        }

        private void OnAlarmSelected(object sender, AlarmEventArgs e)
        {
            _statusLabel.Text = string.Format(
                "选中：{0} / {1} / {2}",
                e.Alarm.AlarmId,
                e.Alarm.AlarmEvent,
                e.Alarm.AlarmLevel);
        }

        private void OnAlarmDoubleClick(object sender, AlarmEventArgs e)
        {
            _statusLabel.Text = string.Format(
                "双击：{0} - {1}",
                e.Alarm.AlarmId,
                e.Alarm.AlarmDescription);
        }

        private void UpdateCountStatus()
        {
            _statusLabel.Text = string.Format(
                "完整报警数量：{0}，当前{1}限制显示，最大显示数量：{2}。",
                _alarmList.AlarmCount,
                _alarmList.LimitDisplayCount ? "已" : "未",
                _alarmList.MaxDisplayCount);
        }
    }
}
