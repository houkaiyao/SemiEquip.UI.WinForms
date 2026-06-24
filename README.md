# SemiEquip.UI.WinForms

面向半导体自动化设备软件的 WinForms 自定义控件库。

项目目标为 **C# / .NET Framework 4.0+ / WinForms**，当前仅保留 `FoupMapControl`、`FourColorLightControl` 两个控件。控件说明、属性和使用实例统一维护在本文档中。

## 项目结构

- `src/SemiEquip.UI.WinForms`：可复用自定义控件库。
- `src/SemiEquip.UI.WinForms/Controls/FoupMap`：FOUP Map 控件及其辅助类型。
- `src/SemiEquip.UI.WinForms/Controls/FourColorLight`：四色灯控件。
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
- `SlotCount` 范围为 `1` 到 `25`；控件 Handle 创建后不允许修改。
- 控件宽高可自由设置，内部 Slot 会按可用空间自动布局。
- 扣除 `ContentPadding` 后，横向区域按 Slot 编号、Slot 主体、选片勾选框约 `1:8:1` 分配。
- Slot 高度按可用内容总高度除以 `SlotCount` 计算。
- `Slots` 提供只读槽位快照，槽位数据应通过公开方法修改。
- Slot 内文字统一使用 `SlotText`，提示文字统一使用 `SlotTipText`。
- Slot 内文字字体可通过 `SlotTextFont` 设置；默认 `AutoScaleSlotTextFont = true`，空间不足时自动缩小。
- 默认字体为 `Times New Roman`。
- 默认不填充控件底色，背景跟随父容器。
- 左侧 Slot 编号默认黑色显示。

### Slot 状态

| 枚举值 | 默认颜色 | 含义 |
| --- | --- | --- |
| `FoupSlotState.Empty` | White | 无片 / 空槽 |
| `FoupSlotState.BeforeProcess` | Blue | 制程前 / 待加工 |
| `FoupSlotState.AfterProcess` | Green | 制程后 / 已完成 |
| `FoupSlotState.Abnormal` | Red | 异常 / 需要关注 |
| `FoupSlotState.Custom` | 自定义颜色 | 由 `SetSlotColor` 指定 |

### 主要属性

| 属性 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| `SlotCount` | `int` | `25` | 实际绘制的 Slot 数量，Handle 创建后不可修改。 |
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
| `AbnormalSlotColor` | `Color` | Red | 异常状态颜色。 |
| `BeforeProcessSlotColor` | `Color` | Blue | 制程前状态颜色。 |
| `AfterProcessSlotColor` | `Color` | Green | 制程后状态颜色。 |
| `EmptySlotColor` | `Color` | White | 空槽状态颜色。 |
| `SlotBorderColor` | `Color` | Gray | Slot 边框颜色。 |
| `FrameColor` | `Color` | LightGray | 控件外框颜色。 |
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
foupMap.SetSlotState(3, FoupSlotState.AfterProcess);
foupMap.SetSlotState(4, FoupSlotState.Abnormal);
foupMap.SetSlotColor(5, Color.Gold);
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

当前验证覆盖 `FoupMapControl` 的 `SlotCount` 锁定、只读 Slots、`ChooseMapData`、`SlotText` 与 `SlotTipText` 等核心行为；`FourColorLightControl` 通过解决方案构建覆盖编译验证。
