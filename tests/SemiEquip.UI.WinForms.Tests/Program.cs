using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
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
                Run("FoupMap WaferID 与 SlotData", TestSlotTextData);
                Run("AlarmList 排序和限制显示", TestAlarmOrderAndLimit);
                Run("AlarmList 更新与事件", TestAlarmUpdateAndEvents);
                Run("ScrollingText 隐藏后停止 Timer", TestScrollingTextTimerLifecycle);

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
                control.SetWaferId(1, "WAFER-01");
                control.SetSlotData(1, "SLOT-DATA-01");

                control.SlotTextDisplayMode = FoupSlotTextDisplayMode.WaferId;
                AssertEqual("WAFER-01", control.GetWaferId(1), "WaferID");
                AssertEqual(FoupSlotTextDisplayMode.WaferId, control.SlotTextDisplayMode, "WaferID 显示模式");

                control.SlotTextDisplayMode = FoupSlotTextDisplayMode.SlotData;
                AssertEqual("SLOT-DATA-01", control.GetSlotData(1), "SlotData");
                AssertEqual(FoupSlotTextDisplayMode.SlotData, control.SlotTextDisplayMode, "SlotData 显示模式");
            }
        }

        private static void TestAlarmOrderAndLimit()
        {
            using (AlarmListControl control = new AlarmListControl())
            {
                AlarmInfo first = CreateAlarm("A1");
                AlarmInfo second = CreateAlarm("A2");
                AlarmInfo third = CreateAlarm("A3");
                control.SetAlarms(new[] { first, second, third });

                control.DisplayOrder = AlarmDisplayOrder.Descending;
                AssertSame(third, control.DisplayedAlarms[0], "倒序首条报警");

                control.LimitDisplayCount = true;
                control.MaxDisplayCount = 2;
                AssertEqual(2, control.DisplayedAlarmCount, "限制显示数量");
                AssertSame(third, control.DisplayedAlarms[0], "限制显示顺序");
                AssertSame(second, control.DisplayedAlarms[1], "限制显示第二条");
            }
        }

        private static void TestAlarmUpdateAndEvents()
        {
            using (AlarmListControl control = new AlarmListControl())
            {
                int addedCount = 0;
                int updatedCount = 0;
                int countChangedCount = 0;
                control.AlarmAdded += delegate { addedCount++; };
                control.AlarmUpdated += delegate { updatedCount++; };
                control.AlarmCountChanged += delegate { countChangedCount++; };

                AlarmInfo alarm = CreateAlarm("A1");
                control.AddAlarm(alarm);
                alarm.AlarmDescription = "更新后的描述";

                AssertTrue(control.UpdateAlarm(alarm), "已存在报警应可更新");
                AssertEqual(1, addedCount, "AlarmAdded 次数");
                AssertEqual(1, updatedCount, "AlarmUpdated 次数");
                AssertEqual(1, countChangedCount, "AlarmCountChanged 次数");
            }
        }

        private static void TestScrollingTextTimerLifecycle()
        {
            using (ScrollingTextControl control = new ScrollingTextControl())
            {
                IntPtr handle = control.Handle;
                Timer timer = GetPrivateField<Timer>(control, "_scrollTimer");
                AssertTrue(timer.Enabled, "可见控件的 Timer 应运行");

                control.Visible = false;
                AssertTrue(!timer.Enabled, "隐藏控件的 Timer 应停止");
            }
        }

        private static AlarmInfo CreateAlarm(string id)
        {
            return new AlarmInfo(id, "事件", "描述", AlarmLevel.Info);
        }

        private static T GetPrivateField<T>(object instance, string fieldName)
        {
            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new InvalidOperationException("未找到字段：" + fieldName);
            }

            return (T)field.GetValue(instance);
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

        private static void AssertSame(object expected, object actual, string name)
        {
            if (!ReferenceEquals(expected, actual))
            {
                throw new InvalidOperationException(name + " 引用不符合预期。");
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
