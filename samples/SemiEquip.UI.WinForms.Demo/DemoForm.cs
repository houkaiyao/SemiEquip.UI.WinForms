using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SemiEquip.UI.WinForms.Demo.Common;
using SemiEquip.UI.WinForms.Demo.DemoPages;

namespace SemiEquip.UI.WinForms.Demo
{
    public sealed class DemoForm : Form
    {
        private readonly ComboBox _demoSelector;
        private readonly Panel _pageHost;
        private readonly List<DemoPageInfo> _demoPages;
        private UserControl _currentPage;

        public DemoForm()
        {
            Text = "SemiEquip.UI.WinForms Demo";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 1000);
            BackColor = Color.FromArgb(18, 22, 28);
            ForeColor = Color.FromArgb(230, 235, 242);

            _demoPages = new List<DemoPageInfo>();
            RegisterDemoPages();

            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                Padding = new Padding(24, 18, 24, 12),
                BackColor = Color.FromArgb(24, 30, 38)
            };

            Label selectorLabel = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Left,
                Text = "示例控件",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 5, 12, 0)
            };

            _demoSelector = new ComboBox
            {
                Dock = DockStyle.Left,
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _demoSelector.SelectedIndexChanged += OnDemoSelectorSelectedIndexChanged;

            _pageHost = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Color.FromArgb(18, 22, 28)
            };

            headerPanel.Controls.Add(_demoSelector);
            headerPanel.Controls.Add(selectorLabel);
            Controls.Add(_pageHost);
            Controls.Add(headerPanel);

            LoadDemoSelectorItems();
        }

        private void RegisterDemoPages()
        {
            _demoPages.Add(new DemoPageInfo("FOUP Map 控件", delegate { return new FoupMapDemoPage(); }));
            _demoPages.Add(new DemoPageInfo("状态灯控件", delegate { return new StatusLightDemoPage(); }));
            _demoPages.Add(new DemoPageInfo("四色灯控件", delegate { return new FourColorLightDemoPage(); }));
            _demoPages.Add(new DemoPageInfo("Alarm List 控件", delegate { return new AlarmListDemoPage(); }));
            _demoPages.Add(new DemoPageInfo("滚动文字控件", delegate { return new ScrollingTextDemoPage(); }));
        }

        private void LoadDemoSelectorItems()
        {
            _demoSelector.Items.Clear();

            for (int index = 0; index < _demoPages.Count; index++)
            {
                _demoSelector.Items.Add(_demoPages[index]);
            }

            if (_demoSelector.Items.Count > 0)
            {
                _demoSelector.SelectedIndex = 0;
            }
        }

        private void OnDemoSelectorSelectedIndexChanged(object sender, EventArgs e)
        {
            DemoPageInfo pageInfo = _demoSelector.SelectedItem as DemoPageInfo;
            if (pageInfo == null)
            {
                return;
            }

            ShowDemoPage(pageInfo);
        }

        private void ShowDemoPage(DemoPageInfo pageInfo)
        {
            if (_currentPage != null)
            {
                _pageHost.Controls.Remove(_currentPage);
                _currentPage.Dispose();
                _currentPage = null;
            }

            _currentPage = pageInfo.CreatePage();
            _currentPage.Dock = DockStyle.Fill;
            _pageHost.Controls.Add(_currentPage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _currentPage != null)
            {
                _currentPage.Dispose();
                _currentPage = null;
            }

            base.Dispose(disposing);
        }
    }
}
