using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("Alarms")]
    [DefaultEvent("AlarmSelected")]
    [DesignerCategory("Code")]
    public class AlarmListControl : UserControl
    {
        private readonly DataGridView _grid;
        private readonly List<AlarmInfo> _alarms;
        private Color _infoBackColor = Color.FromArgb(221, 235, 247);
        private Color _warningBackColor = Color.FromArgb(255, 242, 204);
        private Color _alarmBackColor = Color.FromArgb(252, 228, 214);
        private Color _criticalBackColor = Color.FromArgb(244, 204, 204);
        private Color _rowForeColor = Color.FromArgb(24, 28, 35);
        private Color _selectedBackColor = Color.FromArgb(62, 110, 166);
        private Color _selectedForeColor = Color.White;
        private bool _autoScrollToLastAlarm = true;
        private bool _limitDisplayCount;
        private int _maxDisplayCount = 5;
        private AlarmDisplayOrder _displayOrder = AlarmDisplayOrder.Ascending;

        public AlarmListControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint, true);

            _alarms = new List<AlarmInfo>();

            BackColor = Color.FromArgb(18, 22, 28);
            ForeColor = Color.FromArgb(230, 235, 242);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BackgroundColor = Color.FromArgb(18, 22, 28),
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 32,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                EnableHeadersVisualStyles = false,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                RowTemplate = { Height = 30 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            ConfigureGridStyle();
            CreateColumns();

            _grid.SelectionChanged += OnGridSelectionChanged;
            _grid.CellDoubleClick += OnGridCellDoubleClick;

            Controls.Add(_grid);
            Size = new Size(720, 320);
        }

        public event EventHandler<AlarmEventArgs> AlarmSelected;

        public event EventHandler<AlarmEventArgs> AlarmDoubleClick;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<AlarmInfo> Alarms
        {
            get { return _alarms.AsReadOnly(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int AlarmCount
        {
            get { return _alarms.Count; }
        }

        [Category("Alarm List")]
        [Description("添加报警后是否自动滚动到最后一条报警。")]
        [DefaultValue(true)]
        public bool AutoScrollToLastAlarm
        {
            get { return _autoScrollToLastAlarm; }
            set { _autoScrollToLastAlarm = value; }
        }

        [Category("Alarm List")]
        [Description("是否限制表格当前显示的报警数量。完整报警仍保存在 Alarms 中。")]
        [DefaultValue(false)]
        public bool LimitDisplayCount
        {
            get { return _limitDisplayCount; }
            set
            {
                if (_limitDisplayCount == value)
                {
                    return;
                }

                _limitDisplayCount = value;
                RefreshGridRows();
            }
        }

        [Category("Alarm List")]
        [Description("限制显示时最多显示的报警数量。按当前 DisplayOrder 排序后取前 N 条。")]
        [DefaultValue(5)]
        public int MaxDisplayCount
        {
            get { return _maxDisplayCount; }
            set
            {
                int normalized = Math.Max(1, value);
                if (_maxDisplayCount == normalized)
                {
                    return;
                }

                _maxDisplayCount = normalized;
                RefreshGridRows();
            }
        }

        [Category("Alarm List")]
        [Description("报警显示顺序。Ascending 按添加顺序显示，Descending 按最新加入优先显示。")]
        [DefaultValue(AlarmDisplayOrder.Ascending)]
        public AlarmDisplayOrder DisplayOrder
        {
            get { return _displayOrder; }
            set
            {
                if (_displayOrder == value)
                {
                    return;
                }

                _displayOrder = value;
                RefreshGridRows();
            }
        }

        [Category("Alarm Colors")]
        [Description("提示等级报警行背景色。")]
        public Color InfoBackColor
        {
            get { return _infoBackColor; }
            set { SetColor(ref _infoBackColor, value); }
        }

        [Category("Alarm Colors")]
        [Description("警告等级报警行背景色。")]
        public Color WarningBackColor
        {
            get { return _warningBackColor; }
            set { SetColor(ref _warningBackColor, value); }
        }

        [Category("Alarm Colors")]
        [Description("报警等级报警行背景色。")]
        public Color AlarmBackColor
        {
            get { return _alarmBackColor; }
            set { SetColor(ref _alarmBackColor, value); }
        }

        [Category("Alarm Colors")]
        [Description("严重等级报警行背景色。")]
        public Color CriticalBackColor
        {
            get { return _criticalBackColor; }
            set { SetColor(ref _criticalBackColor, value); }
        }

        [Category("Alarm Colors")]
        [Description("普通报警行文字颜色。")]
        public Color RowForeColor
        {
            get { return _rowForeColor; }
            set { SetColor(ref _rowForeColor, value); }
        }

        [Category("Alarm Colors")]
        [Description("选中报警行背景色。")]
        public Color SelectedBackColor
        {
            get { return _selectedBackColor; }
            set { SetColor(ref _selectedBackColor, value); }
        }

        [Category("Alarm Colors")]
        [Description("选中报警行文字颜色。")]
        public Color SelectedForeColor
        {
            get { return _selectedForeColor; }
            set { SetColor(ref _selectedForeColor, value); }
        }

        public void AddAlarm(AlarmInfo alarm)
        {
            if (alarm == null)
            {
                throw new ArgumentNullException("alarm");
            }

            if (alarm.OccurTime == DateTime.MinValue)
            {
                alarm.OccurTime = DateTime.Now;
            }

            _alarms.Add(alarm);
            RefreshGridRows();

            if (_autoScrollToLastAlarm && _grid.Rows.Count > 0)
            {
                SelectAndScrollToRow(GetLatestAlarmDisplayRowIndex());
            }
        }

        public void SetAlarms(IEnumerable<AlarmInfo> alarms)
        {
            _alarms.Clear();

            if (alarms != null)
            {
                foreach (AlarmInfo alarm in alarms)
                {
                    if (alarm == null)
                    {
                        continue;
                    }

                    if (alarm.OccurTime == DateTime.MinValue)
                    {
                        alarm.OccurTime = DateTime.Now;
                    }

                    _alarms.Add(alarm);
                }
            }

            RefreshGridRows();
            if (_autoScrollToLastAlarm && _grid.Rows.Count > 0)
            {
                SelectAndScrollToRow(GetLatestAlarmDisplayRowIndex());
            }
        }

        public void ClearAlarms()
        {
            _alarms.Clear();
            _grid.Rows.Clear();
        }

        protected virtual void OnAlarmSelected(AlarmEventArgs e)
        {
            EventHandler<AlarmEventArgs> handler = AlarmSelected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAlarmDoubleClick(AlarmEventArgs e)
        {
            EventHandler<AlarmEventArgs> handler = AlarmDoubleClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ConfigureGridStyle()
        {
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 49, 60);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(236, 240, 246);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font(Font.FontFamily, 9f, FontStyle.Bold);
            _grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            _grid.GridColor = Color.FromArgb(118, 128, 140);
            _grid.DefaultCellStyle.SelectionBackColor = _selectedBackColor;
            _grid.DefaultCellStyle.SelectionForeColor = _selectedForeColor;
            _grid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        private void CreateColumns()
        {
            _grid.Columns.Add(CreateTextColumn("AlarmId", "报警ID", 110, DataGridViewContentAlignment.MiddleCenter));
            _grid.Columns.Add(CreateTextColumn("AlarmEvent", "报警事件", 180, DataGridViewContentAlignment.MiddleLeft));

            DataGridViewTextBoxColumn descriptionColumn = CreateTextColumn(
                "AlarmDescription",
                "报警描述",
                320,
                DataGridViewContentAlignment.MiddleLeft);
            descriptionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _grid.Columns.Add(descriptionColumn);

            _grid.Columns.Add(CreateTextColumn("AlarmLevel", "报警等级", 100, DataGridViewContentAlignment.MiddleCenter));
        }

        private static DataGridViewTextBoxColumn CreateTextColumn(
            string name,
            string headerText,
            int width,
            DataGridViewContentAlignment alignment)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                Width = width,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            column.DefaultCellStyle.Alignment = alignment;
            return column;
        }

        private void AddGridRow(AlarmInfo alarm)
        {
            int rowIndex = _grid.Rows.Add(
                alarm.AlarmId,
                alarm.AlarmEvent,
                alarm.AlarmDescription,
                GetAlarmLevelText(alarm.AlarmLevel));

            DataGridViewRow row = _grid.Rows[rowIndex];
            row.Tag = alarm;
            ApplyRowStyle(row, alarm.AlarmLevel);
        }

        private void RefreshGridRows()
        {
            _grid.Rows.Clear();

            int displayedCount = 0;
            if (_displayOrder == AlarmDisplayOrder.Descending)
            {
                for (int index = _alarms.Count - 1; index >= 0; index--)
                {
                    if (!TryAddDisplayedAlarm(_alarms[index], ref displayedCount))
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int index = 0; index < _alarms.Count; index++)
                {
                    if (!TryAddDisplayedAlarm(_alarms[index], ref displayedCount))
                    {
                        break;
                    }
                }
            }
        }

        private bool TryAddDisplayedAlarm(AlarmInfo alarm, ref int displayedCount)
        {
            if (_limitDisplayCount && displayedCount >= _maxDisplayCount)
            {
                return false;
            }

            AddGridRow(alarm);
            displayedCount++;
            return true;
        }

        private int GetLatestAlarmDisplayRowIndex()
        {
            if (_grid.Rows.Count <= 0)
            {
                return -1;
            }

            if (_displayOrder == AlarmDisplayOrder.Descending)
            {
                return 0;
            }

            return _grid.Rows.Count - 1;
        }

        private void SelectAndScrollToRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _grid.Rows.Count)
            {
                return;
            }

            DataGridViewRow row = _grid.Rows[rowIndex];
            if (!row.Visible)
            {
                return;
            }

            _grid.ClearSelection();
            row.Selected = true;
            _grid.CurrentCell = row.Cells[0];

            if (!CanScrollRows())
            {
                return;
            }

            try
            {
                _grid.FirstDisplayedScrollingRowIndex = rowIndex;
            }
            catch (InvalidOperationException)
            {
                // DataGridView 初始化布局前可能没有可显示行空间，下一次添加或用户滚动时会恢复正常。
            }
        }

        private bool CanScrollRows()
        {
            if (!_grid.IsHandleCreated || !_grid.Visible)
            {
                return false;
            }

            int availableHeight = _grid.ClientSize.Height - _grid.ColumnHeadersHeight;
            return availableHeight >= _grid.RowTemplate.Height;
        }

        private void ApplyRowStyle(DataGridViewRow row, AlarmLevel level)
        {
            row.DefaultCellStyle.BackColor = GetBackColor(level);
            row.DefaultCellStyle.ForeColor = _rowForeColor;
            row.DefaultCellStyle.SelectionBackColor = _selectedBackColor;
            row.DefaultCellStyle.SelectionForeColor = _selectedForeColor;
        }

        private void RefreshRowStyles()
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                AlarmInfo alarm = row.Tag as AlarmInfo;
                if (alarm != null)
                {
                    ApplyRowStyle(row, alarm.AlarmLevel);
                }
            }
        }

        private Color GetBackColor(AlarmLevel level)
        {
            switch (level)
            {
                case AlarmLevel.Warning:
                    return _warningBackColor;
                case AlarmLevel.Alarm:
                    return _alarmBackColor;
                case AlarmLevel.Critical:
                    return _criticalBackColor;
                case AlarmLevel.Info:
                default:
                    return _infoBackColor;
            }
        }

        private static string GetAlarmLevelText(AlarmLevel level)
        {
            switch (level)
            {
                case AlarmLevel.Warning:
                    return "警告";
                case AlarmLevel.Alarm:
                    return "报警";
                case AlarmLevel.Critical:
                    return "严重";
                case AlarmLevel.Info:
                default:
                    return "提示";
            }
        }

        private AlarmInfo GetCurrentAlarm()
        {
            if (_grid.CurrentRow == null)
            {
                return null;
            }

            return _grid.CurrentRow.Tag as AlarmInfo;
        }

        private void OnGridSelectionChanged(object sender, EventArgs e)
        {
            AlarmInfo alarm = GetCurrentAlarm();
            if (alarm != null)
            {
                OnAlarmSelected(new AlarmEventArgs(alarm));
            }
        }

        private void OnGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            AlarmInfo alarm = _grid.Rows[e.RowIndex].Tag as AlarmInfo;
            if (alarm != null)
            {
                OnAlarmDoubleClick(new AlarmEventArgs(alarm));
            }
        }

        private void SetColor(ref Color field, Color value)
        {
            if (field == value)
            {
                return;
            }

            field = value;
            RefreshRowStyles();
            Invalidate();
        }
    }
}
