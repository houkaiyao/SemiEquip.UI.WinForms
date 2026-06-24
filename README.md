# SemiEquip.UI.WinForms

面向半导体自动化设备软件的 WinForms 自定义控件库。

项目目标为 **C# / .NET Framework 4.0+ / WinForms**，当前仅保留 `FoupMapControl`、`FourColorLightControl` 两个控件。后续可在同一个控件库中继续增加更多设备软件常用控件。

## 项目结构

- `src/SemiEquip.UI.WinForms`：可复用自定义控件库。
- `src/SemiEquip.UI.WinForms/Controls/FoupMap`：FOUP Map 控件及其辅助类型。
- `src/SemiEquip.UI.WinForms/Controls/FourColorLight`：四色灯控件。
- `samples/SemiEquip.UI.WinForms.Demo`：WinForms Demo 示例程序。

## FoupMapControl

`FoupMapControl` 用于绘制 FOUP / Cassette 的 Slot Map。

- 支持 `SlotCount`，范围为 1 到 25；运行时控件 Handle 创建后不允许再修改。
- `Slots` 提供只读槽位快照，槽位数据统一通过控件方法修改，避免重复编号和未重绘问题。
- 默认鼠标悬浮 Slot 时通过 Tooltip 显示 Slot 编号和 WaferID，可分别控制是否显示。
- 可选择在左侧直接显示 Slot 号。
- 每个 Slot 绘制为连续矩形网格。
- 控件宽高可由调用方任意设置，内部 Slot 会根据可用空间自动渲染。
- 扣除 `ContentPadding` 后，横向区域固定按 Slot 编号、Slot 主体、选片勾选框约 `1:8:1` 分配。
- Slot 高度按照可用内容总高度除以 `SlotCount` 计算。
- 可在 WinForms 设计器中拖拽调整大小。
- 支持通过代码设置每个 Slot 的颜色。
- 支持为每个 Slot 设置 `WaferID` 和 `SlotData`，并可选择 Slot 内显示哪一种文字。
- 使用 `SlotTextColor`、`SlotTextFont`、`SlotTextPadding` 设置 Slot 内文字外观。
- 旧版 `WaferIdColor`、`WaferIdFont`、`WaferIdTextPadding` 仍可使用，但已标记为过时兼容属性。
- 支持在 Slot 右侧显示选片勾选框，并通过 `ChooseMapData` 获取或设置选片结果。
- 小尺寸下无法完整绘制勾选框时，已选 Slot 会使用简化标记显示。
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
foupMap.SlotTextDisplayMode = FoupSlotTextDisplayMode.WaferId;
foupMap.SlotTextColor = Color.White;
foupMap.SetWaferId(2, "WAFER-0002");
foupMap.SetSlotData(2, "SLOT-DATA-02");
foupMap.ShowSlotNumberInToolTip = true;
foupMap.ShowWaferIdInToolTip = true;
foupMap.ShowSelectionCheckBoxes = true;
foupMap.SetSlotSelected(25, true);
string chooseMapData = foupMap.ChooseMapData; // 0000000000000000000000001
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

## 自动化验证

`tests/SemiEquip.UI.WinForms.Tests` 是兼容 `net40` 的自动化验证程序，不依赖第三方测试框架。

```powershell
dotnet build SemiEquip.UI.WinForms.sln -c Release
.\tests\SemiEquip.UI.WinForms.Tests\bin\Release\net40\SemiEquip.UI.WinForms.Tests.exe
```

当前验证覆盖 `FoupMapControl` 的 `SlotCount` 锁定、只读 Slots、`ChooseMapData`、WaferID 与 SlotData 等核心行为；`FourColorLightControl` 通过解决方案构建覆盖编译验证。
