using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using BathLineControl;
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
                Run("BathLineControl basic API and drawing", TestBathLineControl);
                Run("RobotTransfer basic API and drawing", TestRobotTransferControl);
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

                AssertEqual(Color.FromArgb(55, 137, 255), control.BeforeProcessWaferColor, "Default BeforeProcessWaferColor");
                AssertEqual(Color.FromArgb(132, 220, 170), control.ProcessingWaferColor, "Default ProcessingWaferColor");
                AssertEqual(1, (int)WaferState.BeforeProcess, "BeforeProcess enum value");
                AssertEqual(2, (int)WaferState.Processing, "Processing enum value");
                AssertEqual(3, (int)WaferState.Completed, "Completed enum value");

                control.State = WaferState.BeforeProcess;
                AssertEqual(WaferState.BeforeProcess, control.State, "BeforeProcess State");

                control.State = WaferState.Processing;
                AssertEqual(WaferState.Processing, control.State, "Processing State");

                control.State = WaferState.Completed;
                AssertEqual(WaferState.Completed, control.State, "Completed State");

                control.ContentPadding = -10;
                control.BorderWidth = 0;
                AssertEqual(0, control.ContentPadding, "ContentPadding 应被限制到 0");
                AssertEqual(1, control.BorderWidth, "BorderWidth 应被限制到 1");

                control.EmptyWaferColor = Color.GhostWhite;
                control.BeforeProcessWaferColor = Color.RoyalBlue;
                control.ProcessingWaferColor = Color.DeepSkyBlue;
                control.CompletedWaferColor = Color.LimeGreen;
                control.BorderColor = Color.DarkSlateGray;
                AssertEqual(Color.GhostWhite, control.EmptyWaferColor, "EmptyWaferColor");
                AssertEqual(Color.RoyalBlue, control.BeforeProcessWaferColor, "BeforeProcessWaferColor");
                AssertEqual(Color.DeepSkyBlue, control.ProcessingWaferColor, "ProcessingWaferColor");
                AssertEqual(Color.LimeGreen, control.CompletedWaferColor, "CompletedWaferColor");
                AssertEqual(Color.DarkSlateGray, control.BorderColor, "BorderColor");

                control.State = WaferState.Empty;
                control.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                control.State = WaferState.BeforeProcess;
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

        private static void TestRobotTransferControl()
        {
            using (RobotTransferControl control = new RobotTransferControl())
            using (Bitmap bitmap = new Bitmap(420, 420))
            {
                control.Size = new Size(420, 420);

                AssertTrue(control.IsReady, "RobotTransfer should be ready by default");
                AssertTrue(!control.IsBusy, "RobotTransfer should not be busy by default");
                AssertEqual(0.0, control.BaseAngle, "Default BaseAngle");
                AssertEqual(0.0, control.TargetBaseAngle, "Default TargetBaseAngle");
                AssertEqual(138.0, control.PushDistance, "Default PushDistance");
                AssertEqual(0.82, control.ForkMoveSpeed, "Default ForkMoveSpeed");
                AssertEqual(120.0, control.BaseRotateSpeed, "Default BaseRotateSpeed");

                control.PushDistance = -1.0;
                control.ForkMoveSpeed = -1.0;
                control.BaseRotateSpeed = -1.0;
                AssertEqual(0.0, control.PushDistance, "PushDistance should clamp to zero");
                AssertEqual(0.01, control.ForkMoveSpeed, "ForkMoveSpeed should clamp to minimum");
                AssertEqual(1.0, control.BaseRotateSpeed, "BaseRotateSpeed should clamp to minimum");

                control.PushDistance = 138.0;
                control.ForkMoveSpeed = 0.82;
                control.BaseRotateSpeed = 120.0;
                AssertTrue(control.Start(RobotFork.Fork1, 0.0, RobotTransferAction.None), "Start None should be accepted while ready");
                AssertEqual(0.0, control.TargetBaseAngle, "TargetBaseAngle after Start None");
                AssertTrue(control.Extend(RobotFork.Fork1), "Extend should be accepted when base is settled");
                AssertTrue(!control.Start(RobotFork.Fork2, 90.0, RobotTransferAction.Get), "Start should be rejected while fork target is not settled");
                AssertTrue(control.Retract(RobotFork.Fork1), "Retract should be accepted for idle fork runtime");

                control.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }
        }

        private static void TestBathLineControl()
        {
            using (BathLineControl.BathLineControl control = new BathLineControl.BathLineControl())
            using (Bitmap bitmap = new Bitmap(500, 300))
            {
                control.Size = bitmap.Size;
                AssertTrue(control.OuterBathEnabled, "Outer bath should be enabled by default");
                AssertEqual(Color.FromArgb(24, 103, 150), control.OutlineColor, "Default OutlineColor");
                AssertEqual(2F, control.OutlineWidth, "Default OutlineWidth");
                AssertEqual(Color.White, control.ChemicalColor, "Default ChemicalColor");
                AssertEqual(Color.White, control.ChemicalForeColor, "Default ChemicalForeColor");
                AssertEqual(string.Empty, control.ChemicalText, "Default ChemicalText");
                AssertEqual(24, control.ChemicalWidth, "Default ChemicalWidth");
                AssertEqual(24, control.ChemicalHeight, "Default ChemicalHeight");
                AssertTrue(control.InnerSensorVisible, "InnerSensorVisible should be true by default");
                AssertTrue(control.OuterSensorVisible, "OuterSensorVisible should be true by default");
                AssertTrue(!control.InnerLLSensor, "InnerLLSensor should be false by default");
                AssertTrue(!control.InnerLSensor, "InnerLSensor should be false by default");
                AssertTrue(!control.InnerHSensor, "InnerHSensor should be false by default");
                AssertTrue(!control.InnerHHSensor, "InnerHHSensor should be false by default");
                AssertTrue(!control.OuterLLSensor, "OuterLLSensor should be false by default");
                AssertTrue(!control.OuterLSensor, "OuterLSensor should be false by default");
                AssertTrue(!control.OuterHSensor, "OuterHSensor should be false by default");
                AssertTrue(!control.OuterHHSensor, "OuterHHSensor should be false by default");

                control.OutlineWidth = 0F;
                AssertEqual(1F, control.OutlineWidth, "OutlineWidth should clamp to one");
                control.ChemicalColor = Color.Orange;
                control.ChemicalForeColor = Color.Navy;
                control.ChemicalText = "H2SO4";
                control.ChemicalWidth = 0;
                control.ChemicalHeight = 0;
                AssertEqual(Color.Orange, control.ChemicalColor, "ChemicalColor");
                AssertEqual(Color.Navy, control.ChemicalForeColor, "ChemicalForeColor");
                AssertEqual("H2SO4", control.ChemicalText, "ChemicalText");
                AssertEqual(1, control.ChemicalWidth, "ChemicalWidth should clamp to one");
                AssertEqual(1, control.ChemicalHeight, "ChemicalHeight should clamp to one");
                control.InnerLLSensor = true;
                control.InnerLSensor = true;
                control.InnerHSensor = true;
                control.InnerHHSensor = true;
                control.OuterLLSensor = true;
                control.OuterLSensor = true;
                control.OuterHSensor = true;
                control.OuterHHSensor = true;
                AssertTrue(control.InnerHHSensor, "InnerHHSensor");
                AssertTrue(control.OuterHHSensor, "OuterHHSensor");
                control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));

                control.OuterBathEnabled = false;
                AssertTrue(control.OuterSensorVisible, "OuterSensorVisible setting should be preserved when outer bath is hidden");
                AssertTrue(control.OuterLLSensor, "OuterLLSensor value should be preserved when outer bath is hidden");
                AssertTrue(control.OuterHHSensor, "OuterHHSensor value should be preserved when outer bath is hidden");
                control.InnerSensorVisible = false;
                AssertTrue(control.InnerLLSensor, "InnerLLSensor value should be preserved when inner sensors are hidden");
                AssertTrue(control.InnerHHSensor, "InnerHHSensor value should be preserved when inner sensors are hidden");
                control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
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
