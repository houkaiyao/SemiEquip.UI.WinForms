using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
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
                Run("FoupMap 制程中状态与颜色", TestFoupMapProcessingState);
                Run("FoupMap 小尺寸勾选框绘制", TestFoupMapCompactSelectionCheckBox);
                Run("Wafer 基础属性与绘制", TestWaferControl);
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

        private static void TestFoupMapProcessingState()
        {
            using (FoupMapControl control = new FoupMapControl())
            {
                AssertEqual(0, (int)FoupSlotState.Empty, "Empty 枚举值");
                AssertEqual(1, (int)FoupSlotState.BeforeProcess, "BeforeProcess 枚举值");
                AssertEqual(2, (int)FoupSlotState.Processing, "Processing 枚举值");
                AssertEqual(3, (int)FoupSlotState.AfterProcess, "AfterProcess 枚举值");
                AssertEqual(4, (int)FoupSlotState.Abnormal, "Abnormal 枚举值");
                AssertEqual(5, (int)FoupSlotState.Custom, "Custom 枚举值");

                AssertEqual(Color.FromArgb(40, 112, 210), control.BeforeProcessSlotColor, "默认 BeforeProcessSlotColor");
                AssertEqual(Color.FromArgb(132, 220, 170), control.ProcessingSlotColor, "默认 ProcessingSlotColor");
                AssertEqual(Color.FromArgb(20, 132, 72), control.AfterProcessSlotColor, "默认 AfterProcessSlotColor");
                AssertEqual(Color.FromArgb(190, 40, 40), control.AbnormalSlotColor, "默认 AbnormalSlotColor");

                control.SetSlotState(1, FoupSlotState.Processing);
                AssertEqual(FoupSlotState.Processing, control.GetSlotState(1), "Processing SlotState");
                AssertEqual(control.ProcessingSlotColor, control.GetSlotColor(1), "Processing SlotColor");

                control.ProcessingSlotColor = Color.LightGreen;
                AssertEqual(Color.LightGreen, control.GetSlotColor(1), "更新后的 Processing SlotColor");
            }
        }

        private static void TestFoupMapCompactSelectionCheckBox()
        {
            using (FoupMapControl control = new FoupMapControl())
            using (Bitmap bitmap = new Bitmap(80, 120))
            {
                control.Size = new Size(80, 120);
                control.ContentPadding = 2;
                control.SlotCount = 10;
                Rectangle hiddenSlotBounds = control.GetSlotBounds(1);
                control.ShowSelectionCheckBoxes = true;
                control.SetSlotSelected(1, true);

                Rectangle slotBounds = control.GetSlotBounds(1);
                Rectangle adjacentSlotBounds = control.GetSlotBounds(2);
                Rectangle checkBoxBounds = InvokeSelectionCheckBoxBounds(control, 1);
                Rectangle adjacentCheckBoxBounds = InvokeSelectionCheckBoxBounds(control, 2);
                int expectedCheckBoxSize = Math.Max(1, slotBounds.Height * 8 / 10);
                AssertEqual(hiddenSlotBounds, slotBounds, "启用勾选框前后 Slot 位置和大小应保持不变");
                AssertEqual(slotBounds.Height, adjacentSlotBounds.Height, "相邻 Slot 高度应一致");
                AssertEqual(expectedCheckBoxSize, checkBoxBounds.Width, "勾选框宽度应为 Slot 高度的十分之八");
                AssertEqual(expectedCheckBoxSize, checkBoxBounds.Height, "勾选框高度应为 Slot 高度的十分之八");
                AssertEqual(checkBoxBounds.Size, adjacentCheckBoxBounds.Size, "相邻 Slot 的勾选框大小应一致");
                AssertTrue(checkBoxBounds.Left - slotBounds.Right >= 2, "Slot 与勾选框之间应至少保留 2 像素空隙");

                control.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }

            using (FoupMapControl control = new FoupMapControl())
            {
                control.Size = new Size(200, 300);
                control.ContentPadding = 5;
                control.SlotCount = 25;
                control.ShowSelectionCheckBoxes = true;

                Rectangle slotBounds = control.GetSlotBounds(1);
                Rectangle adjacentSlotBounds = control.GetSlotBounds(2);
                Rectangle checkBoxBounds = InvokeSelectionCheckBoxBounds(control, 1);
                Rectangle adjacentCheckBoxBounds = InvokeSelectionCheckBoxBounds(control, 2);

                AssertEqual(slotBounds.Height, adjacentSlotBounds.Height, "200x300 下相邻 Slot 高度应一致");
                AssertEqual(checkBoxBounds.Size, adjacentCheckBoxBounds.Size, "200x300 下相邻勾选框大小应一致");
            }
        }

        private static void TestWaferControl()
        {
            using (WaferControl control = new WaferControl())
            using (Bitmap bitmap = new Bitmap(160, 160))
            {
                control.Size = new Size(160, 160);

                AssertEqual(WaferState.Empty, control.State, "默认 State");
                AssertEqual(Color.Transparent, control.BackColor, "默认 BackColor");

                control.State = WaferState.Processing;
                AssertEqual(WaferState.Processing, control.State, "Processing State");

                control.State = WaferState.Completed;
                AssertEqual(WaferState.Completed, control.State, "Completed State");

                control.ContentPadding = -10;
                control.BorderWidth = 0;
                AssertEqual(0, control.ContentPadding, "ContentPadding 应被限制到 0");
                AssertEqual(1, control.BorderWidth, "BorderWidth 应被限制到 1");

                control.EmptyWaferColor = Color.GhostWhite;
                control.ProcessingWaferColor = Color.DeepSkyBlue;
                control.CompletedWaferColor = Color.LimeGreen;
                control.BorderColor = Color.DarkSlateGray;
                AssertEqual(Color.GhostWhite, control.EmptyWaferColor, "EmptyWaferColor");
                AssertEqual(Color.DeepSkyBlue, control.ProcessingWaferColor, "ProcessingWaferColor");
                AssertEqual(Color.LimeGreen, control.CompletedWaferColor, "CompletedWaferColor");
                AssertEqual(Color.DarkSlateGray, control.BorderColor, "BorderColor");

                control.State = WaferState.Empty;
                control.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                control.State = WaferState.Processing;
                control.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                control.State = WaferState.Completed;
                control.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
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

        private static Rectangle InvokeSelectionCheckBoxBounds(FoupMapControl control, int slotNumber)
        {
            MethodInfo method = typeof(FoupMapControl).GetMethod(
                "CalculateSelectionCheckBoxBounds",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
            {
                throw new InvalidOperationException("未找到 CalculateSelectionCheckBoxBounds。");
            }

            return (Rectangle)method.Invoke(control, new object[] { slotNumber });
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
