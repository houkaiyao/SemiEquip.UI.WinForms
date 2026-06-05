# SemiEquip.UI.WinForms

面向半导体自动化设备软件的 WinForms 自定义控件库。

项目目标为 **C# / .NET Framework 4.0+ / WinForms**，当前已包含 `FoupMapControl`、`StatusLightControl`、`FourColorLightControl`、`AlarmListControl`、`ScrollingTextControl`。后续可在同一个控件库中继续增加更多设备软件常用控件。

## 项目结构

- `src/SemiEquip.UI.WinForms`：可复用自定义控件库。
- `src/SemiEquip.UI.WinForms/Controls/FoupMap`：FOUP Map 控件及其辅助类型。
- `src/SemiEquip.UI.WinForms/Controls/StatusLight`：状态灯控件。
- `src/SemiEquip.UI.WinForms/Controls/FourColorLight`：四色灯控件。
- `src/SemiEquip.UI.WinForms/Controls/AlarmList`：报警列表控件及其数据类型。
- `src/SemiEquip.UI.WinForms/Controls/ScrollingText`：滚动文字控件。
- `samples/SemiEquip.UI.WinForms.Demo`：WinForms Demo 示例程序。

## FoupMapControl

`FoupMapControl` 用于绘制 FOUP / Cassette 的 Slot Map。

- 支持 `SlotCount`，范围为 1 到 25。
- 默认鼠标悬浮 Slot 时通过 Tooltip 显示 Slot 号。
- 可选择在左侧直接显示 Slot 号。
- 每个 Slot 绘制为扁长形状。
- 默认保持控件高度:宽度为 2:1。
- Slot 主体默认约占控件宽度的 80%。
- 当 `SlotCount` 减少时，不拉伸单个 Slot 高度，而是增大 Slot 间距。
- 可在 WinForms 设计器中拖拽调整大小。
- 支持通过代码设置每个 Slot 的颜色。
- 内置状态：
  - `Abnormal`：红色，异常。
  - `BeforeProcess`：蓝色，制程前。
  - `AfterProcess`：绿色，制程后。
  - `Empty`：白色，无片。

```csharp
var foupMap = new FoupMapControl();
foupMap.SlotCount = 25;
foupMap.SetSlotState(1, FoupSlotState.Empty);
foupMap.SetSlotState(2, FoupSlotState.BeforeProcess);
foupMap.SetSlotState(3, FoupSlotState.AfterProcess);
foupMap.SetSlotState(4, FoupSlotState.Abnormal);
foupMap.SetSlotColor(5, Color.Gold);
```

## StatusLightControl

`StatusLightControl` 用于绘制设备软件中常见的状态灯。

- 整体外形为正方形。
- 内部绘制圆形灯。
- 默认保持控件宽度和高度为 1:1。
- 圆形灯默认不保留外边距，贴近控件边界显示。
- 默认不显示正方形外框线和圆灯边框线。
- 内置高光和暗部阴影，使灯泡显示更有层次。
- 可通过 `LightColor` 在运行时动态修改灯色。
- 可配置圆灯内边距、圆灯边框颜色、正方形外框颜色、高光颜色和阴影颜色。

```csharp
var statusLight = new StatusLightControl();
statusLight.LightColor = Color.Red;
statusLight.LightColor = Color.LimeGreen;
```

## FourColorLightControl

`FourColorLightControl` 用于绘制竖向四色灯，从上到下依次为红、黄、绿、蓝四个相连灯段。控件按真实塔灯的圆柱体灯罩效果绘制，正视图为高大于宽的柱状结构。

- 每个灯通过独立 `bool` 属性控制亮暗。
- `True` 显示亮色，`False` 显示暗色。
- 默认保持高度:宽度为 4:1。
- 默认四段相连，不保留间距。
- 两侧暗、中间柔亮，模拟圆柱体灯罩。
- 支持横向细纹，模拟磨砂或棱纹塑料效果。
- 支持配置每个颜色的亮色和暗色。

```csharp
var fourColorLight = new FourColorLightControl();
fourColorLight.RedLightOn = true;
fourColorLight.YellowLightOn = false;
fourColorLight.GreenLightOn = true;
fourColorLight.BlueLightOn = false;
fourColorLight.ShowFrostedLines = true;
```

## AlarmListControl

`AlarmListControl` 用于显示设备报警列表。

- 默认四列：报警ID、报警事件、报警描述、报警等级。
- 通过 `AlarmLevel` 自动修改整行颜色。
- 支持提示、警告、报警、严重四种等级。
- 支持添加、设置、清空报警。
- 可通过 `AlarmCount` 获取当前报警数量。
- 可通过 `DisplayOrder` 设置正序或倒序显示，默认正序。
- 可通过 `LimitDisplayCount` 和 `MaxDisplayCount` 按当前显示顺序只显示前 N 条报警。
- 支持选中和双击事件。

```csharp
var alarmList = new AlarmListControl();
alarmList.AddAlarm(new AlarmInfo(
    "ALM-0001",
    "传输报警",
    "Robot 取片动作超时，请检查轴状态。",
    AlarmLevel.Alarm));
int alarmCount = alarmList.AlarmCount;
alarmList.DisplayOrder = AlarmDisplayOrder.Descending;
alarmList.LimitDisplayCount = true;
alarmList.MaxDisplayCount = 5;
```

## ScrollingTextControl

`ScrollingTextControl` 用于显示水平滚动文字，适合设备状态提示、生产线消息、报警摘要等区域。

- 支持从右到左、从左到右两种滚动方向。
- 支持通过 `Text` 设置显示文字。
- 支持通过 `Font` 设置字体类型和字号。
- 支持通过 `ForeColor` 设置字体颜色。
- 支持通过 `BackColor` 设置背景颜色。
- 支持设置滚动步长和刷新间隔。

```csharp
var scrollingText = new ScrollingTextControl();
scrollingText.Text = "设备运行中：Robot 正在等待下一步命令。";
scrollingText.ScrollDirection = ScrollingTextDirection.RightToLeft;
scrollingText.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
scrollingText.ForeColor = Color.White;
scrollingText.BackColor = Color.FromArgb(32, 38, 48);
```
