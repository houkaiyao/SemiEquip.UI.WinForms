using System;
using System.Collections.Generic;
using SemiEquip.UI.WinForms.Controls;

namespace SemiEquip.UI.WinForms.Tests
{
    internal static class Program
    {
        private static int _passedCount;

        [STAThread]
        private static int Main()
        {
            try
            {
                Run("FoupMap 创建前可设置 SlotCount", TestInitialSlotCount);
                Run("FoupMap 创建后锁定 SlotCount", TestLockedSlotCount);
                Run("FoupMap Slots 为只读集合", TestReadOnlySlots);
                Run("FoupMap ChooseMapData 映射", TestChooseMapData);
                Run("FoupMap SlotText 与 SlotTipText", TestSlotTextData);

                Console.WriteLine("全部验证通过，共 {0} 项。", _passedCount);
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("验证失败：{0}", exception);
                return 1;
            }
        }

        private static void TestInitialSlotCount()
        {
            using (FoupMapControl control = new FoupMapControl())
            {
                control.SlotCount = 10;
                AssertEqual(10, control.SlotCount, "SlotCount");
                AssertEqual(10, control.Slots.Count, "Slots.Count");
            }
        }

        private static void TestLockedSlotCount()
        {
            using (FoupMapControl control = new FoupMapControl())
            {
                IntPtr handle = control.Handle;
                AssertThrows<InvalidOperationException>(delegate { control.SlotCount = 20; });
                AssertEqual(FoupMapControl.MaxSlotCount, control.SlotCount, "锁定后的 SlotCount");
            }
        }

        private static void TestReadOnlySlots()
        {
            using (FoupMapControl control = new FoupMapControl())
            {
                AssertThrows<NotSupportedException>(delegate { control.Slots.Add(new FoupSlotInfo(1)); });
            }
        }

        private static void TestChooseMapData()
        {
            using (FoupMapControl control = new FoupMapControl())
            {
                control.SlotCount = 5;
                control.ChooseMapData = "10001";

                AssertTrue(control.GetSlotSelected(1), "Slot 1 应被选中");
                AssertTrue(control.GetSlotSelected(5), "Slot 5 应被选中");
                AssertEqual("10001", control.ChooseMapData, "ChooseMapData");
            }
        }

        private static void TestSlotTextData()
        {
            using (FoupMapControl control = new FoupMapControl())
            {
                control.SetSlotText(1, "SLOT-TEXT-01");
                control.SetSlotTipText(1, "SLOT-TIP-01");

                AssertEqual("SLOT-TEXT-01", control.GetSlotText(1), "SlotText");
                AssertEqual("SLOT-TIP-01", control.GetSlotTipText(1), "SlotTipText");

                control.ClearSlotTexts();
                control.ClearSlotTipTexts();

                AssertEqual(string.Empty, control.GetSlotText(1), "清空后的 SlotText");
                AssertEqual(string.Empty, control.GetSlotTipText(1), "清空后的 SlotTipText");
            }
        }

        private static void Run(string name, Action test)
        {
            test();
            _passedCount++;
            Console.WriteLine("[通过] {0}", name);
        }

        private static void AssertTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string name)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new InvalidOperationException(string.Format(
                    "{0} 不符合预期。预期：{1}，实际：{2}",
                    name,
                    expected,
                    actual));
            }
        }

        private static void AssertThrows<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }

            throw new InvalidOperationException("未抛出预期异常：" + typeof(TException).Name);
        }
    }
}
