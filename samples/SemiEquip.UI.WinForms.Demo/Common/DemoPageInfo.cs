using System;
using System.Windows.Forms;

namespace SemiEquip.UI.WinForms.Demo.Common
{
    internal sealed class DemoPageInfo
    {
        public DemoPageInfo(string name, Func<UserControl> createPage)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Demo 页面名称不能为空。", "name");
            }

            if (createPage == null)
            {
                throw new ArgumentNullException("createPage");
            }

            Name = name;
            CreatePage = createPage;
        }

        public string Name { get; private set; }

        public Func<UserControl> CreatePage { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
