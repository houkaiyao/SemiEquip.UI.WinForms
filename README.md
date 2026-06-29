# SemiEquip.UI.WinForms

面向半导体自动化设备软件的 WinForms 自定义控件库。

项目目标为 **C# / .NET Framework 4.0+ / WinForms**，当前保留 `FoupMapControl`、`WaferControl`、`FourColorLightControl`、`ActionSensorButtonControl` 四个控件。控件说明、属性和使用实例统一维护在本文档中。

## 项目结构

- `src/SemiEquip.UI.WinForms`：可复用自定义控件库。
- `src/SemiEquip.UI.WinForms/Controls/FoupMap`：FOUP Map 控件及其辅助类型。
- `src/SemiEquip.UI.WinForms/Controls/Wafer`：单片 Wafer 状态控件。
- `src/SemiEquip.UI.WinForms/Controls/FourColorLight`：四色灯控件。
- `src/SemiEquip.UI.WinForms/Controls/ActionSensorButton`：动作传感器按钮控件。
- `samples/SemiEquip.UI.WinForms.Demo`：WinForms Demo 示例程序。
- `tests/SemiEquip.UI.WinForms.Tests`：兼容 `net40` 的自动化验证程序。

## 安装和引用

当前项目以源码方式维护，其他 WinForms 项目可引用构建输出的 DLL，也可在同一解决方案中使用项目引用。

```csharp
using SemiEquip.UI.WinForms.Controls;
```

## FoupMapControl

`FoupMapControl` 用于绘制 FOUP / Cassette 的 Slot Map，通过颜色、文字和选片状态展示每个槽位的 wafer 状态。

### 核心行为

- 最大 Slot 数量为 `25`。
- `SlotCount` 范围为 `1` 到 `25`；运行时可调整，调整后会按新的槽位数量重建 Slot 数据。
- 控件宽高可自由设置，内部 Slot 会按可用空间自动布局。
- 扣除 `ContentPadding` 后，横向区域按 Slot 编号、Slot 主体、选片勾选框约 `1:8:1` 分配。
- Slot 高度按可用内容总高度除以 `SlotCount` 计算。
- `Slots` 提供只读槽位快照，槽位数据应通过公开方法修改。
- Slot 内文字统一使用 `SlotText`，提示文字统一使用 `SlotTipText`。
- Slot 内文字字体可通过 `SlotTextFont` 设置；默认 `AutoScaleSlotTextFont = true`，空间不足时自动缩小。
- 默认字体为 `Times New Roman`。
- 默认不填充控件底色，背景跟随父容器。
- 默认不绘制控件外边框，仅绘制 Slot 自身边框。
- 左侧 Slot 编号默认黑色显示。

### Slot 状态

| 枚举值 | 默认颜色 | 含义 |
| --- | --- | --- |
| `FoupSlotState.Empty` | White | 无片 / 空槽 |
| `FoupSlotState.BeforeProcess` | DeepBlue | 制程前 / 待加工 |
| `FoupSlotState.Processing` | LightGreen | 制程中 |
| `FoupSlotState.AfterProcess` | DarkGreen | 制成后 / 已完成 |
| `FoupSlotState.Abnormal` | DeepRed | 异常 / 需要关注 |
| `FoupSlotState.Custom` | 自定义颜色 | 由 `SetSlotColor` 指定 |

### 主要属性

| 属性 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| `SlotCount` | `int` | `25` | 实际绘制的 Slot 数量，运行时可调整。 |
| `ShowSlotNumbers` | `bool` | `false` | 是否在左侧直接绘制 Slot 编号。 |
| `ShowSelectionCheckBoxes` | `bool` | `false` | 是否在右侧显示选片勾选框。 |
| `ChooseMapData` | `string` | 空字符串 | 按 Slot 编号顺序映射选片结果，Slot 1 对应第 1 位。 |
| `ShowSlotText` | `bool` | `false` | 是否在 Slot 内显示 `SlotText`。 |
| `ShowSlotTip` | `bool` | `true` | 鼠标悬浮到 Slot 上时，是否显示 `SlotTipText`。 |
| `AutoScaleSlotNumberFont` | `bool` | `true` | 左侧 Slot 编号是否自动缩小字号避免裁剪。 |
| `AutoScaleSlotTextFont` | `bool` | `true` | Slot 内文字是否根据空间自动缩小。 |
| `SlotTextPadding` | `int` | `4` | Slot 内文字与 Slot 边缘的间距。 |
| `SlotTextFont` | `Font` | `Times New Roman 7pt` | Slot 内文字基准字体。 |
| `ContentPadding` | `int` | `8` | 控件内容区域边距。 |
| `AbnormalSlotColor` | `Color` | DeepRed | 异常状态颜色。 |
| `BeforeProcessSlotColor` | `Color` | DeepBlue | 制程前状态颜色。 |
| `ProcessingSlotColor` | `Color` | LightGreen | 制程中状态颜色。 |
| `AfterProcessSlotColor` | `Color` | DarkGreen | 制成后状态颜色。 |
| `EmptySlotColor` | `Color` | White | 空槽状态颜色。 |
| `SlotBorderColor` | `Color` | Gray | Slot 边框颜色。 |
| `FrameColor` | `Color` | LightGray | 兼容保留属性；当前默认不绘制控件外框。 |
| `SlotNumberColor` | `Color` | Black | 左侧 Slot 编号文字颜色。 |
| `SlotTextColor` | `Color` | Dark | Slot 内文字颜色。 |

### 方法

| 方法 | 说明 |
| --- | --- |
| `SetSlotColor(int slotNumber, Color color)` | 设置指定 Slot 的自定义颜色，并将状态标记为 `Custom`。 |
| `GetSlotColor(int slotNumber)` | 获取指定 Slot 当前颜色。 |
| `SetSlotState(int slotNumber, FoupSlotState state)` | 设置指定 Slot 状态，并使用对应状态颜色。 |
| `GetSlotState(int slotNumber)` | 获取指定 Slot 当前状态。 |
| `SetSlotText(int slotNumber, string slotText)` | 设置指定 Slot 内显示的通用文本。 |
| `GetSlotText(int slotNumber)` | 获取指定 Slot 的通用显示文本。 |
| `ClearSlotTexts()` | 清空所有 SlotText。 |
| `SetSlotTipText(int slotNumber, string slotTipText)` | 设置指定 Slot 的悬浮提示文本。 |
| `GetSlotTipText(int slotNumber)` | 获取指定 Slot 的悬浮提示文本。 |
| `ClearSlotTipTexts()` | 清空所有 SlotTipText。 |
| `SetSlotSelected(int slotNumber, bool selected)` | 设置指定 Slot 是否被选中。 |
| `GetSlotSelected(int slotNumber)` | 获取指定 Slot 是否被选中。 |
| `ClearSlotSelections()` | 清空所有选片状态。 |
| `GetSlotBounds(int slotNumber)` | 获取指定 Slot 在控件内的绘制矩形。 |
| `ClearSlots()` | 重置全部 Slot 状态、文字和选片状态。 |

### 事件

| 事件 | 参数 | 说明 |
| --- | --- | --- |
| `SlotClick` | `FoupSlotClickEventArgs` | 点击 Slot 时触发，参数包含 `SlotNumber`、`Button`、`Location`。 |
| `ChooseMapDataChanged` | `EventArgs` | 选片状态或 `ChooseMapData` 变化时触发。 |

### 基本示例

```csharp
var foupMap = new FoupMapControl();
foupMap.Location = new Point(32, 32);
foupMap.Size = new Size(180, 360);
foupMap.SlotCount = 25;
foupMap.ShowSlotNumbers = true;
foupMap.ShowSelectionCheckBoxes = true;
foupMap.ShowSlotText = true;
foupMap.ShowSlotTip = true;

foupMap.SetSlotState(1, FoupSlotState.Empty);
foupMap.SetSlotState(2, FoupSlotState.BeforeProcess);
foupMap.SetSlotState(3, FoupSlotState.Processing);
foupMap.SetSlotState(4, FoupSlotState.AfterProcess);
foupMap.SetSlotState(5, FoupSlotState.Abnormal);
foupMap.SetSlotColor(6, Color.Gold);
foupMap.SetSlotText(2, "S02");
foupMap.SetSlotTipText(2, "Slot 02 当前为制程前状态。");
foupMap.SetSlotSelected(25, true);

Controls.Add(foupMap);
```

### Slot 内字体示例

固定字体大小：

```csharp
foupMap.SlotTextFont = new Font("Times New Roman", 9f, FontStyle.Regular);
foupMap.AutoScaleSlotTextFont = false;
```

以 9pt 为基准，空间不足时自动缩小：

```csharp
foupMap.SlotTextFont = new Font("Times New Roman", 9f, FontStyle.Regular);
foupMap.AutoScaleSlotTextFont = true;
```

### 选片数据示例

```csharp
foupMap.ChooseMapData = "1000000000000000000000001";

bool slot1Selected = foupMap.GetSlotSelected(1);
bool slot25Selected = foupMap.GetSlotSelected(25);
string chooseMapData = foupMap.ChooseMapData;
```

### 点击事件示例

```csharp
foupMap.SlotClick += delegate(object sender, FoupSlotClickEventArgs e)
{
    int slotNumber = e.SlotNumber;
    FoupSlotState state = foupMap.GetSlotState(slotNumber);
    string slotText = foupMap.GetSlotText(slotNumber);
    string slotTipText = foupMap.GetSlotTipText(slotNumber);
};
```

## WaferControl

`WaferControl` 用于绘制单片 Wafer，通过颜色表达无料、制程中、制成结束三种状态。控件适合在设备腔体、机械手取放片点位、对位台等界面位置显示单片晶圆状态。

### 核心行为

- 控件绘制一个居中的圆形 Wafer。
- 使用抗锯齿和 45 度线性渐变，保留浅色高光与深色阴影质感。
- 控件宽高可自由设置，Wafer 会按可用区域自动取较小边居中绘制。
- `ContentPadding` 小于 0 时按 0 处理。
- `BorderWidth` 小于 1 时按 1 处理。
- 默认背景透明，Wafer 以外区域显示父容器背景。

### Wafer 状态

| 枚举值 | 默认颜色 | 含义 |
| --- | --- | --- |
| `WaferState.Empty` | White | 无料 |
| `WaferState.Processing` | Blue | 制程中 |
| `WaferState.Completed` | Green | 制成结束 / 已完成 |

### 主要属性

| 属性 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| `State` | `WaferState` | `Empty` | 当前 Wafer 状态。 |
| `ContentPadding` | `int` | `8` | 控件内容区域边距。 |
| `BorderWidth` | `int` | `2` | Wafer 外圈边框宽度。 |
| `EmptyWaferColor` | `Color` | White | 无料状态颜色。 |
| `ProcessingWaferColor` | `Color` | Blue | 制程中状态颜色。 |
| `CompletedWaferColor` | `Color` | Green | 制成结束状态颜色。 |
| `BorderColor` | `Color` | Gray | Wafer 外圈边框颜色。 |

### 基本示例

```csharp
var wafer = new WaferControl();
wafer.Location = new Point(32, 32);
wafer.Size = new Size(120, 120);
wafer.State = WaferState.Processing;
wafer.ContentPadding = 8;
wafer.BorderWidth = 2;

Controls.Add(wafer);
```

## ActionSensorButtonControl

`ActionSensorButtonControl` 是一个左侧带状态灯的 Button 控件，用于同时显示 PLC 动作 BOOL 状态和 0/1/2 个 IO Sensor 状态，适合气缸、夹爪、挡停、门锁、轴定位等设备动作点位。

控件不绑定固定业务颜色。动作状态、传感器状态、边框和阴影均通过属性配置，Demo 中的颜色只作为预览示例。

### 核心行为

- 按钮主体显示 `ButtonText`。
- 按钮底色通过 `CommandState` 和配置颜色表达 PLC 动作 BOOL。
- 左侧按 `SensorMode` 显示 0、1 或 2 个传感器状态灯。
- 状态灯固定上下排列；单灯时垂直居中。
- 状态灯形状、左边距、与文字距离和按钮圆角均可配置。
- 控件继承 `Control` 并复用标准 `Click` 事件，第一版不区分点击区域。
- 控件支持缩放绘制，小尺寸下文字使用省略和自动缩小避免明显重叠。

### 属性

| 属性 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| `ButtonText` | `string` | `Action` | 顶部动作名称。 |
| `CommandState` | `bool` | `false` | PLC 动作 BOOL 状态。 |
| `SensorMode` | `SensorDisplayMode` | `Two` | 传感器显示模式：`None`、`One`、`Two`。 |
| `Sensor1State` / `Sensor2State` | `bool` | `false` | 两个传感器状态。 |
| `CommandOnBackColor` / `CommandOffBackColor` | `Color` | 示例色 | 动作 True / False 的主体背景色。 |
| `HoverBackColor` | `Color` | 示例色 | 鼠标悬浮且动作为 False 时的背景色。 |
| `PressedBackColor` | `Color` | 示例色 | 鼠标按下且动作为 False 时的背景色。 |
| `DefaultBack` | `Color` | 示例色 | `CommandOffBackColor` 的兼容别名，贴近 `AntdUI.Button` 命名。 |
| `BackHover` | `Color` | 示例色 | `HoverBackColor` 的兼容别名，贴近 `AntdUI.Button` 命名。 |
| `BackActive` | `Color` | 示例色 | `PressedBackColor` 的兼容别名，贴近 `AntdUI.Button` 命名。 |
| `CommandOnForeColor` / `CommandOffForeColor` | `Color` | `White` | 动作 True / False 的主体文字色。 |
| `ForeHover` | `Color` | `White` | 鼠标悬浮且动作为 False 时的文字色，贴近 `AntdUI.Button` 命名。 |
| `SensorOnColor` / `SensorOffColor` | `Color` | 示例色 | 传感器 True / False 的指示颜色。 |
| `SensorBorderColor` | `Color` | 示例色 | 传感器边框颜色。 |
| `BorderColor` | `Color` | 示例色 | 控件外框颜色。 |
| `CornerRadius` | `int` | `8` | 控件圆角半径。 |
| `Radius` | `int` | `8` | `CornerRadius` 的兼容别名，贴近 `AntdUI.Button` 命名。 |
| `SensorLeftPadding` | `int` | `10` | 状态灯距离按钮左边界的距离。 |
| `SensorTextSpacing` | `int` | `8` | 状态灯右侧到按钮文字区域的距离。 |
| `SensorShape` | `SensorIndicatorShape` | `Rectangle` | 传感器形状：`Circle`、`Rectangle`、`RoundedRectangle`。 |
| `Shadow` | `int` | `0` | 阴影大小，默认不绘制阴影。 |
| `ShadowColor` | `Color` | `Black` | 阴影颜色。 |
| `ShadowOffsetX` / `ShadowOffsetY` | `int` | `0` / `1` | 阴影偏移。 |
| `ShadowOpacity` | `float` | `0.18` | 阴影透明度，范围为 0 到 1。 |

### 基本示例

```csharp
var actionButton = new ActionSensorButtonControl();
actionButton.Location = new Point(32, 32);
actionButton.Size = new Size(180, 58);
actionButton.ButtonText = "顶升气缸";
actionButton.CommandState = plcLiftCommand;
actionButton.SensorMode = SensorDisplayMode.Two;
actionButton.Sensor1State = ioLiftUpSensor;
actionButton.Sensor2State = ioLiftDownSensor;

actionButton.CommandOnBackColor = Color.FromArgb(226, 64, 64);      // CommandState = true
actionButton.DefaultBack = Color.FromArgb(225, 236, 251);           // 默认底色
actionButton.BackHover = Color.FromArgb(43, 125, 211);              // 鼠标悬浮
actionButton.BackActive = Color.FromArgb(32, 104, 184);             // 鼠标按下 / CommandState = true
actionButton.CommandOnForeColor = Color.White;
actionButton.CommandOffForeColor = Color.Black;
actionButton.ForeHover = Color.White;
actionButton.SensorOnColor = Color.FromArgb(40, 112, 210);
actionButton.SensorOffColor = Color.White;
actionButton.SensorBorderColor = Color.FromArgb(40, 112, 210);
actionButton.Radius = 8;
actionButton.SensorShape = SensorIndicatorShape.Rectangle;
actionButton.SensorLeftPadding = 10;
actionButton.SensorTextSpacing = 8;
actionButton.Shadow = 4;
actionButton.ShadowOffsetY = 1;
actionButton.ShadowOpacity = 0.18f;

actionButton.Click += delegate
{
    // 在业务代码中下发 PLC 动作或打开操作确认。
};

Controls.Add(actionButton);
```

### 单传感器示例

```csharp
var axisButton = new ActionSensorButtonControl();
axisButton.ButtonText = "X 轴定位";
axisButton.SensorMode = SensorDisplayMode.One;
axisButton.SensorShape = SensorIndicatorShape.RoundedRectangle;
```

## FourColorLightControl

`FourColorLightControl` 用于绘制竖向四色灯，从上到下依次为红、黄、绿、蓝四个相连灯段。控件适合设备运行、等待、报警、人工介入等状态提示。

### 核心行为

- 每个灯段通过独立 `bool` 属性控制亮暗。
- `true` 显示亮色，`false` 显示暗色。
- 默认保持高度:宽度为 `4:1`。
- 默认四段相连，`LightGap = 0`。
- 支持横向细纹，模拟磨砂或棱纹塑料效果。
- 支持配置每个灯段的亮色、暗色、阴影色和高光色。

### 属性

| 属性 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| `RedLightOn` | `bool` | `false` | 红灯是否点亮。 |
| `YellowLightOn` | `bool` | `false` | 黄灯是否点亮。 |
| `GreenLightOn` | `bool` | `false` | 绿灯是否点亮。 |
| `BlueLightOn` | `bool` | `false` | 蓝灯是否点亮。 |
| `MaintainAspectRatio` | `bool` | `true` | 调整大小时保持高度:宽度为 `4:1`。 |
| `LightGap` | `int` | `0` | 相邻灯段间距，单位为像素。 |
| `RedOnColor` / `RedOffColor` | `Color` | 红色亮色 / 暗色 | 红灯点亮和熄灭颜色。 |
| `YellowOnColor` / `YellowOffColor` | `Color` | 黄色亮色 / 暗色 | 黄灯点亮和熄灭颜色。 |
| `GreenOnColor` / `GreenOffColor` | `Color` | 绿色亮色 / 暗色 | 绿灯点亮和熄灭颜色。 |
| `BlueOnColor` / `BlueOffColor` | `Color` | 蓝色亮色 / 暗色 | 蓝灯点亮和熄灭颜色。 |
| `ShadowColor` | `Color` | 半透明黑色 | 灯段暗部阴影颜色。 |
| `HighlightColor` | `Color` | 半透明白色 | 灯段中间柔光颜色。 |
| `ShowFrostedLines` | `bool` | `true` | 是否显示横向磨砂细纹。 |
| `FrostedLineSpacing` | `int` | `3` | 横向磨砂细纹间距，单位为像素。 |

### 方法

| 方法 | 说明 |
| --- | --- |
| `SetLightStates(bool redOn, bool yellowOn, bool greenOn, bool blueOn)` | 一次性设置红、黄、绿、蓝四个灯段状态。 |

### 基本示例

```csharp
var fourColorLight = new FourColorLightControl();
fourColorLight.Location = new Point(32, 32);
fourColorLight.Size = new Size(80, 320);
fourColorLight.RedLightOn = false;
fourColorLight.YellowLightOn = false;
fourColorLight.GreenLightOn = true;
fourColorLight.BlueLightOn = false;
fourColorLight.ShowFrostedLines = true;

Controls.Add(fourColorLight);
```

### 状态切换示例

```csharp
// 运行中：绿灯亮
fourColorLight.SetLightStates(false, false, true, false);

// 报警：红灯和黄灯亮
fourColorLight.SetLightStates(true, true, false, false);

// 全部关闭
fourColorLight.SetLightStates(false, false, false, false);
```

### 自定义颜色示例

```csharp
fourColorLight.RedOnColor = Color.FromArgb(255, 70, 70);
fourColorLight.RedOffColor = Color.FromArgb(80, 24, 24);
fourColorLight.LightGap = 2;
fourColorLight.FrostedLineSpacing = 4;
```

## 自动化验证

在 `SemiEquip.UI.WinForms` 目录执行：

```powershell
dotnet build SemiEquip.UI.WinForms.sln -c Release
.\tests\SemiEquip.UI.WinForms.Tests\bin\Release\net40\SemiEquip.UI.WinForms.Tests.exe
```

当前验证覆盖 `FoupMapControl` 的运行时 `SlotCount` 调整、只读 Slots、`ChooseMapData`、`SlotText` 与 `SlotTipText` 等核心行为；覆盖 `WaferControl` 的基础属性、状态颜色和绘制路径；覆盖 `ActionSensorButtonControl` 的基础属性、颜色配置和绘制路径；`FourColorLightControl` 通过解决方案构建覆盖编译验证。
