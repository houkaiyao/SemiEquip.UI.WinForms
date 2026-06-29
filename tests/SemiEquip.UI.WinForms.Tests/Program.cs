using System;
using System.Collections.Generic;
using System.Drawing;
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
                Run("FoupMap 创建后可设置 SlotCount", TestRuntimeSlotCount);
                Run("FoupMap Slots 为只读集合", TestReadOnlySlots);
                Run("FoupMap ChooseMapData 映射", TestChooseMapData);
                Run("FoupMap SlotText 与 SlotTipText", TestSlotTextData);
                Run("ActionSensorButton 基础属性与绘制", TestActionSensorButton);

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

        private static void TestRuntimeSlotCount()
        {
            using (FoupMapControl control = new FoupMapControl())
            {
                IntPtr handle = control.Handle;
                control.SlotCount = 20;

                AssertEqual(20, control.SlotCount, "Handle 创建后的 SlotCount");
                AssertEqual(20, control.Slots.Count, "Handle 创建后的 Slots.Count");
                AssertEqual("00000000000000000000", control.ChooseMapData, "Handle 创建后的 ChooseMapData");
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

        private static void TestActionSensorButton()
        {
            using (ActionSensorButtonControl control = new ActionSensorButtonControl())
            using (Bitmap bitmap = new Bitmap(180, 96))
            {
                control.Size = new Size(180, 96);
                control.ButtonText = "顶升气缸";
                control.CommandState = true;
                control.SensorMode = SensorDisplayMode.Two;
                control.Sensor1State = true;
                control.Sensor2State = false;
                AssertEqual(SensorIndicatorShape.Rectangle, control.SensorShape, "默认 SensorShape");
                control.CommandOnBackColor = Color.Purple;
                control.CommandOffBackColor = Color.Gray;
                control.DefaultBack = Color.AliceBlue;
                control.BackHover = Color.SkyBlue;
                control.BackActive = Color.Navy;
                control.ForeHover = Color.White;
                control.SensorShape = SensorIndicatorShape.RoundedRectangle;
                control.SensorLeftPadding = 12;
                control.SensorTextSpacing = 9;
                control.CornerRadius = -1;
                control.Radius = 8;
                control.Shadow = 4;
                control.ShadowOpacity = 2f;

                AssertEqual("顶升气缸", control.ButtonText, "ButtonText");
                AssertTrue(control.CommandState, "CommandState 应为 True");
                AssertEqual(SensorDisplayMode.Two, control.SensorMode, "SensorMode");
                AssertEqual(Color.Purple, control.CommandOnBackColor, "CommandOnBackColor");
                AssertEqual(Color.AliceBlue, control.CommandOffBackColor, "DefaultBack 应映射 CommandOffBackColor");
                AssertEqual(Color.SkyBlue, control.HoverBackColor, "BackHover 应映射 HoverBackColor");
                AssertEqual(Color.Navy, control.PressedBackColor, "BackActive 应映射 PressedBackColor");
                AssertEqual(Color.White, control.ForeHover, "ForeHover");
                AssertEqual(SensorIndicatorShape.RoundedRectangle, control.SensorShape, "SensorShape");
                AssertEqual(12, control.SensorLeftPadding, "SensorLeftPadding");
                AssertEqual(9, control.SensorTextSpacing, "SensorTextSpacing");
                AssertEqual(8, control.CornerRadius, "Radius 应映射 CornerRadius");
                AssertEqual(4, control.Shadow, "Shadow");
                AssertEqual(1f, control.ShadowOpacity, "ShadowOpacity 应被限制到 1");

                control.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
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
