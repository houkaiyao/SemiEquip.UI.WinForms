$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$workspaceRoot = Split-Path -Parent $projectRoot
$documentName = "SemiEquip.UI.WinForms_FoupMapControl_Manual.docx"
$documentPath = Join-Path $workspaceRoot $documentName
$tempRoot = Join-Path $projectRoot "tools\.docx-build"
$zipPath = Join-Path $projectRoot "tools\.docx-build.zip"

function Assert-UnderWorkspaceRoot {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path
    )

    $fullRoot = [System.IO.Path]::GetFullPath($workspaceRoot)
    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if (-not $fullPath.StartsWith($fullRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Path is outside workspace root: $fullPath"
    }
}

function XmlEscape {
    param(
        [AllowEmptyString()]
        [string]$Text
    )

    return [System.Security.SecurityElement]::Escape($Text)
}

function Add-Paragraph {
    param(
        [Parameter(Mandatory=$true)]
        [System.Text.StringBuilder]$Builder,

        [Parameter(Mandatory=$true)]
        [string]$Text,

        [string]$Style = "Normal"
    )

    $escapedText = XmlEscape $Text
    [void]$Builder.AppendLine("<w:p><w:pPr><w:pStyle w:val=`"$Style`"/></w:pPr><w:r><w:t xml:space=`"preserve`">$escapedText</w:t></w:r></w:p>")
}

function Add-Heading {
    param(
        [Parameter(Mandatory=$true)]
        [System.Text.StringBuilder]$Builder,

        [Parameter(Mandatory=$true)]
        [string]$Text,

        [int]$Level = 1
    )

    $style = "Heading$Level"
    Add-Paragraph -Builder $Builder -Text $Text -Style $style
}

function Add-Code {
    param(
        [Parameter(Mandatory=$true)]
        [System.Text.StringBuilder]$Builder,

        [AllowEmptyString()]
        [string[]]$Lines
    )

    foreach ($line in $Lines) {
        $escapedLine = XmlEscape $line
        [void]$Builder.AppendLine("<w:p><w:pPr><w:pStyle w:val=`"Code`"/></w:pPr><w:r><w:t xml:space=`"preserve`">$escapedLine</w:t></w:r></w:p>")
    }
}

function Add-Table {
    param(
        [Parameter(Mandatory=$true)]
        [System.Text.StringBuilder]$Builder,

        [Parameter(Mandatory=$true)]
        [string[]]$Headers,

        [Parameter(Mandatory=$true)]
        [object[]]$Rows
    )

    [void]$Builder.AppendLine("<w:tbl><w:tblPr><w:tblStyle w:val=`"TableGrid`"/><w:tblW w:w=`"0`" w:type=`"auto`"/><w:tblBorders><w:top w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"B8C0CC`"/><w:left w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"B8C0CC`"/><w:bottom w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"B8C0CC`"/><w:right w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"B8C0CC`"/><w:insideH w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"B8C0CC`"/><w:insideV w:val=`"single`" w:sz=`"4`" w:space=`"0`" w:color=`"B8C0CC`"/></w:tblBorders></w:tblPr>")

    [void]$Builder.AppendLine("<w:tr>")
    foreach ($header in $Headers) {
        $escapedHeader = XmlEscape $header
        [void]$Builder.AppendLine("<w:tc><w:tcPr><w:shd w:fill=`"D9EAF7`"/></w:tcPr><w:p><w:pPr><w:pStyle w:val=`"TableText`"/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>$escapedHeader</w:t></w:r></w:p></w:tc>")
    }
    [void]$Builder.AppendLine("</w:tr>")

    foreach ($row in $Rows) {
        [void]$Builder.AppendLine("<w:tr>")
        foreach ($cell in $row) {
            $escapedCell = XmlEscape ([string]$cell)
            [void]$Builder.AppendLine("<w:tc><w:p><w:pPr><w:pStyle w:val=`"TableText`"/></w:pPr><w:r><w:t xml:space=`"preserve`">$escapedCell</w:t></w:r></w:p></w:tc>")
        }
        [void]$Builder.AppendLine("</w:tr>")
    }

    [void]$Builder.AppendLine("</w:tbl>")
}

Assert-UnderWorkspaceRoot -Path $tempRoot
Assert-UnderWorkspaceRoot -Path $zipPath
Assert-UnderWorkspaceRoot -Path $documentPath

if (Test-Path -LiteralPath $tempRoot) {
    Remove-Item -LiteralPath $tempRoot -Recurse -Force
}

if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

if (Test-Path -LiteralPath $documentPath) {
    Remove-Item -LiteralPath $documentPath -Force
}

New-Item -ItemType Directory -Force -Path (Join-Path $tempRoot "_rels") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $tempRoot "word") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $tempRoot "docProps") | Out-Null

$body = New-Object System.Text.StringBuilder

Add-Paragraph -Builder $body -Text "SemiEquip.UI.WinForms 控件开发手册" -Style "Title"
Add-Paragraph -Builder $body -Text "控件：FoupMapControl / FOUP Map 显示控件，StatusLightControl / 状态灯控件，FourColorLightControl / 四色灯控件，AlarmListControl / 报警列表控件，ScrollingTextControl / 滚动文字控件" -Style "Subtitle"
Add-Paragraph -Builder $body -Text "适用技术栈：C#、WinForms、.NET Framework 4.0 及以上。"
Add-Paragraph -Builder $body -Text "当前文档版本：0.1.0。建议后续每新增一个控件，都按本文档结构补充独立章节。"

Add-Heading -Builder $body -Text "1. 文档目的" -Level 1
Add-Paragraph -Builder $body -Text "本文档用于说明 SemiEquip.UI.WinForms 控件库中控件的设计目标、调用方式、属性、方法、事件、渲染规则和后续扩展预留内容。"
Add-Paragraph -Builder $body -Text "该控件面向半导体自动化设备软件，用于显示 FOUP、Cassette 或类似载具中的 wafer slot 状态。"

Add-Heading -Builder $body -Text "2. 项目结构" -Level 1
Add-Table -Builder $body -Headers @("路径", "说明") -Rows @(
    @("src/SemiEquip.UI.WinForms", "控件库项目，输出可被 WinForms 项目引用的 DLL。"),
    @("src/SemiEquip.UI.WinForms/Controls", "控件源码根目录，按控件模块建立子目录。"),
    @("src/SemiEquip.UI.WinForms/Controls/FoupMap", "FOUP Map 控件及其 Slot 状态、事件参数、集合等辅助类型。"),
    @("src/SemiEquip.UI.WinForms/Controls/StatusLight", "状态灯控件源码目录。"),
    @("src/SemiEquip.UI.WinForms/Controls/FourColorLight", "四色灯控件源码目录。"),
    @("src/SemiEquip.UI.WinForms/Controls/AlarmList", "报警列表控件及其数据类型源码目录。"),
    @("src/SemiEquip.UI.WinForms/Controls/ScrollingText", "滚动文字控件源码目录。"),
    @("samples/SemiEquip.UI.WinForms.Demo", "Demo WinForms 程序，用于验证控件显示和交互。"),
    @("tests/SemiEquip.UI.WinForms.Tests", "不依赖第三方测试框架的 net40 自动化验证程序。"),
    @("tools", "文档和辅助脚本目录，当前包含本手册生成脚本。")
)

Add-Heading -Builder $body -Text "3. 控件概览" -Level 1
Add-Table -Builder $body -Headers @("项目", "内容") -Rows @(
    @("控件名称", "FoupMapControl"),
    @("命名空间", "SemiEquip.UI.WinForms.Controls"),
    @("继承类型", "System.Windows.Forms.Control"),
    @("目标框架", ".NET Framework 4.0+"),
    @("主要用途", "绘制 FOUP/Cassette slot map，并通过颜色表示每个 slot 的 wafer 状态。"),
    @("最大 slot 数", "25"),
    @("控件尺寸", "宽高比例不受限制，调用方可自由设置。"),
    @("横向布局", "扣除 ContentPadding 后，Slot 编号、Slot 主体、选片勾选框约按 1:8:1 分配。"),
    @("默认 slot 号显示", "鼠标悬浮 tooltip 显示，例如 Slot 01。")
)

Add-Heading -Builder $body -Text "4. 快速开始" -Level 1
Add-Paragraph -Builder $body -Text "在 WinForms 窗体中引用控件库 DLL 后，可以直接创建控件实例。"
Add-Code -Builder $body -Lines @(
    "using SemiEquip.UI.WinForms.Controls;",
    "",
    "var foupMap = new FoupMapControl",
    "{",
    "    Location = new Point(32, 32),",
    "    Size = new Size(150, 300),",
    "    SlotCount = 25,",
    "    Anchor = AnchorStyles.Left | AnchorStyles.Top",
    "};",
    "",
    "foupMap.SetSlotState(1, FoupSlotState.Empty);",
    "foupMap.SetSlotState(2, FoupSlotState.BeforeProcess);",
    "foupMap.SetSlotState(3, FoupSlotState.AfterProcess);",
    "foupMap.SetSlotState(4, FoupSlotState.Abnormal);",
    "foupMap.SetSlotColor(5, Color.Gold);",
    "foupMap.SlotTextDisplayMode = FoupSlotTextDisplayMode.WaferId;",
    "foupMap.SetWaferId(2, `"WAFER-0002`");",
    "foupMap.SetSlotData(2, `"SLOT-DATA-02`");",
    "foupMap.ShowSlotNumberInToolTip = true;",
    "foupMap.ShowWaferIdInToolTip = true;",
    "foupMap.ShowSelectionCheckBoxes = true;",
    "foupMap.SetSlotSelected(25, true);",
    "string chooseMapData = foupMap.ChooseMapData;",
    "",
    "Controls.Add(foupMap);"
)

Add-Heading -Builder $body -Text "5. Slot 状态定义" -Level 1
Add-Table -Builder $body -Headers @("枚举值", "默认颜色", "建议含义") -Rows @(
    @("FoupSlotState.Empty", "White", "无片 / 空槽"),
    @("FoupSlotState.BeforeProcess", "Blue", "制程前 / 待加工"),
    @("FoupSlotState.AfterProcess", "Green", "制程后 / 已完成"),
    @("FoupSlotState.Abnormal", "Red", "异常 / 需要关注"),
    @("FoupSlotState.Custom", "由 SetSlotColor 指定", "自定义状态或项目特定状态")
)

Add-Heading -Builder $body -Text "6. 属性说明" -Level 1
Add-Table -Builder $body -Headers @("属性", "类型", "默认值", "说明") -Rows @(
    @("SlotCount", "int", "25", "实际绘制的 slot 数量，范围 1 到 25；运行时控件 Handle 创建后不允许修改。"),
    @("ContentPadding", "int", "8", "控件内容区域边距。"),
    @("ShowSlotToolTip", "bool", "true", "是否启用鼠标悬浮 Tooltip。"),
    @("ShowSlotNumberInToolTip", "bool", "true", "Tooltip 中是否显示 Slot 编号。"),
    @("ShowWaferIdInToolTip", "bool", "true", "Tooltip 中是否显示 WaferID。"),
    @("ShowSlotNumbers", "bool", "false", "是否在左侧直接绘制 slot 编号。"),
    @("ShowSelectionCheckBoxes", "bool", "false", "是否在每个 slot 右侧显示选片勾选框。"),
    @("AutoScaleSlotNumberFont", "bool", "true", "左侧编号开启时，自动缩小字号避免裁剪。"),
    @("SlotTextDisplayMode", "FoupSlotTextDisplayMode", "None", "选择 Slot 图形内部不显示文字、显示 WaferID 或显示 SlotData。"),
    @("AutoScaleSlotTextFont", "bool", "true", "自动缩小 Slot 内显示文字，避免超出 slot 图形。"),
    @("SlotTextPadding", "int", "4", "Slot 内显示文字与 slot 边缘之间的内边距。"),
    @("SlotTextFont", "Font", "Segoe UI 7pt", "Slot 内显示文字使用的字体。"),
    @("EmptySlotColor", "Color", "White", "无片状态颜色。"),
    @("BeforeProcessSlotColor", "Color", "Blue", "制程前状态颜色。"),
    @("AfterProcessSlotColor", "Color", "Green", "制程后状态颜色。"),
    @("AbnormalSlotColor", "Color", "Red", "异常状态颜色。"),
    @("SlotBorderColor", "Color", "Gray", "slot 边框颜色。"),
    @("FrameColor", "Color", "Dark gray", "控件外框颜色。"),
    @("SlotNumberColor", "Color", "Light gray", "左侧编号文字颜色。"),
    @("SlotTextColor", "Color", "Dark", "Slot 内显示文字的颜色。")
)

Add-Heading -Builder $body -Text "7. 方法说明" -Level 1
Add-Table -Builder $body -Headers @("方法", "说明") -Rows @(
    @("SetSlotState(int slotNumber, FoupSlotState state)", "设置指定 slot 的业务状态，并使用对应默认颜色。"),
    @("GetSlotState(int slotNumber)", "读取指定 slot 的当前状态。"),
    @("SetSlotColor(int slotNumber, Color color)", "设置指定 slot 的自定义颜色，并将状态标记为 Custom。"),
    @("GetSlotColor(int slotNumber)", "读取指定 slot 的当前颜色。"),
    @("SetWaferId(int slotNumber, string waferId)", "设置指定 slot 的 WaferID。"),
    @("GetWaferId(int slotNumber)", "读取指定 slot 的 WaferID。"),
    @("ClearWaferIds()", "清空所有 slot 的 WaferID。"),
    @("SetSlotData(int slotNumber, string slotData)", "设置指定 slot 的 SlotData。"),
    @("GetSlotData(int slotNumber)", "读取指定 slot 的 SlotData。"),
    @("ClearSlotData()", "清空所有 slot 的 SlotData。"),
    @("ChooseMapData", "按 Slot 编号顺序获取或设置选片字符串。Slot 1 对应第 1 位，Slot 25 对应第 25 位。"),
    @("SetSlotSelected(int slotNumber, bool selected)", "设置指定 slot 是否被选中。"),
    @("GetSlotSelected(int slotNumber)", "读取指定 slot 是否被选中。"),
    @("ClearSlotSelections()", "清空所有 slot 的选片状态。"),
    @("GetSlotBounds(int slotNumber)", "获取指定 slot 在控件内的绘制矩形，可用于定位菜单或扩展交互。"),
    @("ClearSlots()", "将所有 slot 重置为 Empty 状态，并清空 WaferID、SlotData 和选片状态。")
)

Add-Heading -Builder $body -Text "8. 事件说明" -Level 1
Add-Table -Builder $body -Headers @("事件", "事件参数", "说明") -Rows @(
    @("SlotClick", "FoupSlotClickEventArgs", "鼠标点击 slot 时触发。事件参数包含 SlotNumber、Button、Location。"),
    @("ChooseMapDataChanged", "EventArgs", "选片状态或 ChooseMapData 发生变化时触发。")
)
Add-Code -Builder $body -Lines @(
    "foupMap.SlotClick += OnFoupSlotClick;",
    "",
    "private void OnFoupSlotClick(object sender, FoupSlotClickEventArgs e)",
    "{",
    "    int slotNumber = e.SlotNumber;",
    "    MouseButtons button = e.Button;",
    "    Point location = e.Location;",
    "}"
)

Add-Heading -Builder $body -Text "9. 渲染规则" -Level 1
Add-Paragraph -Builder $body -Text "控件宽高比例不受限制，调用方可以通过设计器或代码自由设置尺寸。内部 slot 会根据 ContentPadding、SlotCount 和当前控件大小自动重新布局。"
Add-Paragraph -Builder $body -Text "扣除 ContentPadding 后，横向区域固定按 Slot 编号、Slot 主体、选片勾选框约 1:8:1 分配。左右区域始终保留，显示开关只控制内容是否绘制。"
Add-Paragraph -Builder $body -Text "slot 使用连续矩形网格绘制，并在 ContentPadding 保留的内容区域内，按照可用总高度除以 SlotCount 分配每个 slot 的高度。"
Add-Paragraph -Builder $body -Text "slot 编号默认通过鼠标悬浮 tooltip 显示，避免小尺寸下文字被裁剪。若必须直接显示编号，可开启 ShowSlotNumbers。"
Add-Paragraph -Builder $body -Text "Slots 属性提供只读槽位快照，业务代码应通过 SetSlotState、SetWaferId、SetSlotData 和 SetSlotSelected 等方法修改槽位。小尺寸无法完整显示勾选框时，已选槽位会显示简化标记。"

Add-Heading -Builder $body -Text "10. Demo 说明" -Level 1
Add-Paragraph -Builder $body -Text "DemoForm 中包含一个固定 SlotCount 的 FoupMapControl、Reset Sample 按钮、slot 点击信息显示，以及一个 Timer 模拟实时刷新。"
Add-Paragraph -Builder $body -Text "Timer 每秒推进一个 slot 状态，顺序为 Empty -> BeforeProcess -> AfterProcess -> Abnormal -> Empty，用于验证控件动态刷新。"

Add-Heading -Builder $body -Text "11. 设计器使用建议" -Level 1
Add-Paragraph -Builder $body -Text "控件已标记 ToolboxItem(true)，可被 WinForms Designer 识别。建议在设计器中先拖放控件，然后设置 SlotCount、颜色属性、显示开关和是否显示 tooltip。"
Add-Paragraph -Builder $body -Text "调整控件尺寸后，应检查较小尺寸下 Slot 编号、WaferID 和选片勾选框是否仍具备足够的显示空间。"

Add-Heading -Builder $body -Text "12. 后续控件扩展预留" -Level 1
Add-Paragraph -Builder $body -Text "后续新增控件时，建议每个控件都补充以下内容：控件目标、应用场景、属性表、方法表、事件表、状态颜色表、Demo 示例、注意事项、版本变更记录。"
Add-Table -Builder $body -Headers @("计划控件", "建议用途", "状态") -Rows @(
    @("StatusLamp", "设备状态灯：Idle、Running、Warning、Alarm、Offline。", "待开发"),
    @("ParameterBox", "带单位、上下限、报警色的参数输入控件。", "待开发"),
    @("AlarmBanner", "顶部报警条，显示当前最高等级报警。", "待开发"),
    @("AlarmGrid", "报警历史表格，支持 Ack、Clear、等级颜色。", "待开发"),
    @("RecipeStepGrid", "工艺步骤表，显示 Step、时间、温度、压力、气体等。", "待开发"),
    @("TrendChartControl", "实时趋势曲线控件，显示温度、压力、流量等。", "待开发")
)

Add-Heading -Builder $body -Text "13. StatusLightControl 状态灯控件" -Level 1
Add-Paragraph -Builder $body -Text "StatusLightControl 用于绘制设备软件中常见的状态灯，整体外形为正方形，内部绘制圆形灯。圆形灯默认不保留外边距，贴近控件边界显示；默认不显示正方形外框线和圆灯边框线，并通过高光和暗部阴影增强层次感。可通过 LightColor 在运行时动态设置红、绿、黄、蓝等颜色。"
Add-Table -Builder $body -Headers @("属性", "类型", "默认值", "说明") -Rows @(
    @("LightColor", "Color", "Green", "圆形灯当前显示的颜色，可在运行时动态修改。"),
    @("MaintainAspectRatio", "bool", "true", "调整控件大小时保持整体外形为正方形。"),
    @("LightPadding", "int", "0", "圆形灯与正方形外框之间的间距，默认贴近控件边界。"),
    @("LightBorderColor", "Color", "Transparent", "圆形灯边框颜色，默认透明即不显示边框线。"),
    @("FrameColor", "Color", "Transparent", "正方形外框颜色，默认透明即不显示外框线。"),
    @("ShadowColor", "Color", "Transparent black", "圆形灯暗部阴影颜色。"),
    @("HighlightColor", "Color", "Transparent white", "圆形灯高光颜色。")
)
Add-Code -Builder $body -Lines @(
    "using SemiEquip.UI.WinForms.Controls;",
    "",
    "var statusLight = new StatusLightControl();",
    "statusLight.LightColor = Color.Red;",
    "statusLight.LightColor = Color.LimeGreen;"
)

Add-Heading -Builder $body -Text "14. FourColorLightControl 四色灯控件" -Level 1
Add-Paragraph -Builder $body -Text "FourColorLightControl 用于绘制竖向四色灯，从上到下依次为红、黄、绿、蓝四个相连灯段。控件按真实塔灯的圆柱体灯罩效果绘制，正视图为高大于宽的柱状结构；两侧暗、中间柔亮，并通过横向细纹模拟磨砂或棱纹塑料效果。每个灯通过独立 bool 属性控制亮暗，True 显示亮色，False 显示暗色。"
Add-Table -Builder $body -Headers @("属性", "类型", "默认值", "说明") -Rows @(
    @("RedLightOn", "bool", "false", "红灯是否点亮。"),
    @("YellowLightOn", "bool", "false", "黄灯是否点亮。"),
    @("GreenLightOn", "bool", "false", "绿灯是否点亮。"),
    @("BlueLightOn", "bool", "false", "蓝灯是否点亮。"),
    @("MaintainAspectRatio", "bool", "true", "调整控件大小时保持高度:宽度为 4:1。"),
    @("LightGap", "int", "0", "相邻灯段之间的间距，默认 0 表示四段相连。"),
    @("ShowFrostedLines", "bool", "true", "是否显示横向细纹，用于模拟磨砂灯罩或棱纹塑料效果。"),
    @("FrostedLineSpacing", "int", "3", "横向磨砂细纹间距。"),
    @("RedOnColor / RedOffColor", "Color", "Red / Dark red", "红灯亮色和暗色。"),
    @("YellowOnColor / YellowOffColor", "Color", "Yellow / Dark yellow", "黄灯亮色和暗色。"),
    @("GreenOnColor / GreenOffColor", "Color", "Green / Dark green", "绿灯亮色和暗色。"),
    @("BlueOnColor / BlueOffColor", "Color", "Blue / Dark blue", "蓝灯亮色和暗色。")
)
Add-Code -Builder $body -Lines @(
    "using SemiEquip.UI.WinForms.Controls;",
    "",
    "var fourColorLight = new FourColorLightControl();",
    "fourColorLight.RedLightOn = true;",
    "fourColorLight.YellowLightOn = false;",
    "fourColorLight.GreenLightOn = true;",
    "fourColorLight.BlueLightOn = false;",
    "fourColorLight.ShowFrostedLines = true;"
)

Add-Heading -Builder $body -Text "15. AlarmListControl 报警列表控件" -Level 1
Add-Paragraph -Builder $body -Text "AlarmListControl 用于显示设备报警列表，默认包含报警ID、报警事件、报警描述、报警等级四列。控件通过 AlarmLevel 自动修改整行颜色，便于操作员快速识别报警严重程度。控件内部完整保存已添加报警，表格显示顺序由 DisplayOrder 控制，默认正序；开启 LimitDisplayCount 后，会按当前显示顺序只显示前 N 条报警，适合主页面摘要区域使用。"
Add-Table -Builder $body -Headers @("类型或成员", "说明") -Rows @(
    @("AlarmInfo", "报警数据对象，包含 AlarmId、AlarmEvent、AlarmDescription、AlarmLevel、OccurTime。"),
    @("AlarmLevel", "报警等级枚举，包含 Info、Warning、Alarm、Critical。"),
    @("AddAlarm(AlarmInfo alarm)", "添加一条报警并刷新行颜色。"),
    @("SetAlarms(IEnumerable<AlarmInfo> alarms)", "批量设置报警列表。"),
    @("ClearAlarms()", "清空报警列表。"),
    @("AlarmCount", "获取当前已添加的报警数量。"),
    @("DisplayedAlarmCount / DisplayedAlarms", "获取当前表格实际显示的报警数量和只读快照。"),
    @("UpdateAlarm(AlarmInfo alarm)", "修改已存在的 AlarmInfo 后刷新对应报警显示。"),
    @("RefreshAlarms()", "重新刷新当前报警表格。"),
    @("DisplayOrder", "报警显示顺序，Ascending 为添加顺序，Descending 为最新加入优先。"),
    @("LimitDisplayCount", "是否限制表格当前显示的报警数量。完整报警仍保存在 Alarms 中。"),
    @("MaxDisplayCount", "限制显示时最多显示的报警数量。开启限制后按 DisplayOrder 排序结果取前 N 条报警。"),
    @("AlarmSelected", "选中报警行时触发。"),
    @("AlarmDoubleClick", "双击报警行时触发。")
)
Add-Table -Builder $body -Headers @("等级", "默认含义", "默认行色") -Rows @(
    @("Info", "提示", "浅蓝色"),
    @("Warning", "警告", "浅黄色"),
    @("Alarm", "报警", "浅橙色"),
    @("Critical", "严重", "浅红色")
)
Add-Code -Builder $body -Lines @(
    "using SemiEquip.UI.WinForms.Controls;",
    "",
    "var alarmList = new AlarmListControl();",
    "alarmList.AddAlarm(new AlarmInfo(",
    "    `"ALM-0001`",",
    "    `"传输报警`",",
    "    `"Robot 取片动作超时，请检查轴状态。`",",
    "    AlarmLevel.Alarm));",
    "int alarmCount = alarmList.AlarmCount;",
    "alarmList.DisplayOrder = AlarmDisplayOrder.Descending;",
    "alarmList.LimitDisplayCount = true;",
    "alarmList.MaxDisplayCount = 5;"
)

Add-Heading -Builder $body -Text "16. ScrollingTextControl 滚动文字控件" -Level 1
Add-Paragraph -Builder $body -Text "ScrollingTextControl 用于显示水平滚动文字，适合设备状态提示、生产线消息、报警摘要等区域。控件支持从左到右和从右到左两种方向，并复用 WinForms 标准 Text、Font、ForeColor、BackColor 属性设置文字内容、字体、字体颜色和背景颜色。控件隐藏、Handle 销毁或处于设计器中时会自动停止 Timer。"
Add-Table -Builder $body -Headers @("属性", "说明") -Rows @(
    @("Text", "需要滚动显示的文字内容。"),
    @("ScrollDirection", "滚动方向，RightToLeft 为从右到左，LeftToRight 为从左到右。"),
    @("ScrollingEnabled", "是否启用滚动；关闭后文字居中显示。"),
    @("ScrollStep", "每次滚动移动的像素数。"),
    @("ScrollInterval", "滚动刷新间隔，单位为毫秒。"),
    @("TextPadding", "文字与控件边界之间的间距。"),
    @("Font", "文字字体类型、字号和样式。"),
    @("ForeColor", "文字颜色。"),
    @("BackColor", "背景颜色。")
)
Add-Code -Builder $body -Lines @(
    "using SemiEquip.UI.WinForms.Controls;",
    "",
    "var scrollingText = new ScrollingTextControl();",
    "scrollingText.Text = `"设备运行中：Robot 正在等待下一步命令。`";",
    "scrollingText.ScrollDirection = ScrollingTextDirection.RightToLeft;",
    "scrollingText.Font = new Font(`"Segoe UI`", 12f, FontStyle.Bold);",
    "scrollingText.ForeColor = Color.White;",
    "scrollingText.BackColor = Color.FromArgb(32, 38, 48);"
)

Add-Heading -Builder $body -Text "17. 文档维护记录" -Level 1
Add-Table -Builder $body -Headers @("版本", "日期", "内容", "维护人") -Rows @(
    @("0.1.0", "2026-06-04", "创建 FoupMapControl 第一个控件开发手册。", "Codex"),
    @("0.2.0", "2026-06-05", "新增 StatusLightControl 状态灯控件说明。", "Codex"),
    @("0.3.0", "2026-06-05", "新增 FourColorLightControl 四色灯控件说明。", "Codex"),
    @("0.4.0", "2026-06-05", "新增 AlarmListControl 报警列表控件说明。", "Codex"),
    @("0.5.0", "2026-06-05", "新增 ScrollingTextControl 滚动文字控件说明。", "Codex"),
    @("0.6.0", "2026-06-09", "补充 SlotCount 锁定、只读 Slots、报警更新、Timer 生命周期和自动化验证说明。", "Codex")
)

$documentXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:document xmlns:wpc="http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:o="urn:schemas-microsoft-com:office:office"
    xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
    xmlns:m="http://schemas.openxmlformats.org/officeDocument/2006/math"
    xmlns:v="urn:schemas-microsoft-com:vml"
    xmlns:wp14="http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing"
    xmlns:wp="http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"
    xmlns:w10="urn:schemas-microsoft-com:office:word"
    xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"
    xmlns:w14="http://schemas.microsoft.com/office/word/2010/wordml"
    xmlns:wpg="http://schemas.microsoft.com/office/word/2010/wordprocessingGroup"
    xmlns:wpi="http://schemas.microsoft.com/office/word/2010/wordprocessingInk"
    xmlns:wne="http://schemas.microsoft.com/office/word/2006/wordml"
    xmlns:wps="http://schemas.microsoft.com/office/word/2010/wordprocessingShape"
    mc:Ignorable="w14 wp14">
  <w:body>
$($body.ToString())
    <w:sectPr>
      <w:pgSz w:w="11906" w:h="16838"/>
      <w:pgMar w:top="1134" w:right="1134" w:bottom="1134" w:left="1134" w:header="708" w:footer="708" w:gutter="0"/>
    </w:sectPr>
  </w:body>
</w:document>
"@

$stylesXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:styles xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
  <w:style w:type="paragraph" w:default="1" w:styleId="Normal">
    <w:name w:val="Normal"/>
    <w:qFormat/>
    <w:pPr><w:spacing w:after="120" w:line="276" w:lineRule="auto"/></w:pPr>
    <w:rPr><w:rFonts w:ascii="Microsoft YaHei" w:eastAsia="Microsoft YaHei" w:hAnsi="Microsoft YaHei"/><w:sz w:val="21"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Title">
    <w:name w:val="Title"/>
    <w:qFormat/>
    <w:pPr><w:spacing w:after="200"/></w:pPr>
    <w:rPr><w:rFonts w:ascii="Microsoft YaHei" w:eastAsia="Microsoft YaHei" w:hAnsi="Microsoft YaHei"/><w:b/><w:color w:val="1F4E79"/><w:sz w:val="36"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Subtitle">
    <w:name w:val="Subtitle"/>
    <w:qFormat/>
    <w:pPr><w:spacing w:after="240"/></w:pPr>
    <w:rPr><w:rFonts w:ascii="Microsoft YaHei" w:eastAsia="Microsoft YaHei" w:hAnsi="Microsoft YaHei"/><w:color w:val="5B677A"/><w:sz w:val="24"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading1">
    <w:name w:val="heading 1"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:qFormat/>
    <w:pPr><w:spacing w:before="280" w:after="120"/><w:outlineLvl w:val="0"/></w:pPr>
    <w:rPr><w:rFonts w:ascii="Microsoft YaHei" w:eastAsia="Microsoft YaHei" w:hAnsi="Microsoft YaHei"/><w:b/><w:color w:val="1F4E79"/><w:sz w:val="28"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading2">
    <w:name w:val="heading 2"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:qFormat/>
    <w:pPr><w:spacing w:before="200" w:after="100"/><w:outlineLvl w:val="1"/></w:pPr>
    <w:rPr><w:rFonts w:ascii="Microsoft YaHei" w:eastAsia="Microsoft YaHei" w:hAnsi="Microsoft YaHei"/><w:b/><w:color w:val="2F5597"/><w:sz w:val="24"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="TableText">
    <w:name w:val="TableText"/>
    <w:basedOn w:val="Normal"/>
    <w:pPr><w:spacing w:after="0" w:line="240" w:lineRule="auto"/></w:pPr>
    <w:rPr><w:rFonts w:ascii="Microsoft YaHei" w:eastAsia="Microsoft YaHei" w:hAnsi="Microsoft YaHei"/><w:sz w:val="18"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Code">
    <w:name w:val="Code"/>
    <w:basedOn w:val="Normal"/>
    <w:pPr><w:spacing w:after="0" w:line="220" w:lineRule="auto"/><w:shd w:fill="F2F4F7"/></w:pPr>
    <w:rPr><w:rFonts w:ascii="Consolas" w:eastAsia="Microsoft YaHei" w:hAnsi="Consolas"/><w:sz w:val="18"/></w:rPr>
  </w:style>
  <w:style w:type="table" w:styleId="TableGrid">
    <w:name w:val="Table Grid"/>
    <w:tblPr>
      <w:tblBorders>
        <w:top w:val="single" w:sz="4" w:space="0" w:color="B8C0CC"/>
        <w:left w:val="single" w:sz="4" w:space="0" w:color="B8C0CC"/>
        <w:bottom w:val="single" w:sz="4" w:space="0" w:color="B8C0CC"/>
        <w:right w:val="single" w:sz="4" w:space="0" w:color="B8C0CC"/>
        <w:insideH w:val="single" w:sz="4" w:space="0" w:color="B8C0CC"/>
        <w:insideV w:val="single" w:sz="4" w:space="0" w:color="B8C0CC"/>
      </w:tblBorders>
    </w:tblPr>
  </w:style>
</w:styles>
"@

$contentTypesXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
  <Override PartName="/word/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"/>
  <Override PartName="/docProps/core.xml" ContentType="application/vnd.openxmlformats-package.core-properties+xml"/>
  <Override PartName="/docProps/app.xml" ContentType="application/vnd.openxmlformats-officedocument.extended-properties+xml"/>
</Types>
"@

$relsXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties" Target="docProps/core.xml"/>
  <Relationship Id="rId3" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties" Target="docProps/app.xml"/>
</Relationships>
"@

$documentRelsXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
</Relationships>
"@

$coreXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<cp:coreProperties xmlns:cp="http://schemas.openxmlformats.org/package/2006/metadata/core-properties" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:dcterms="http://purl.org/dc/terms/" xmlns:dcmitype="http://purl.org/dc/dcmitype/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <dc:title>SemiEquip.UI.WinForms FoupMapControl Manual</dc:title>
  <dc:subject>WinForms semiconductor equipment custom control manual</dc:subject>
  <dc:creator>Codex</dc:creator>
  <cp:lastModifiedBy>Codex</cp:lastModifiedBy>
  <dcterms:created xsi:type="dcterms:W3CDTF">2026-06-04T00:00:00Z</dcterms:created>
  <dcterms:modified xsi:type="dcterms:W3CDTF">2026-06-04T00:00:00Z</dcterms:modified>
</cp:coreProperties>
"@

$appXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Properties xmlns="http://schemas.openxmlformats.org/officeDocument/2006/extended-properties" xmlns:vt="http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes">
  <Application>Codex</Application>
  <DocSecurity>0</DocSecurity>
  <ScaleCrop>false</ScaleCrop>
  <Company></Company>
  <LinksUpToDate>false</LinksUpToDate>
  <SharedDoc>false</SharedDoc>
  <HyperlinksChanged>false</HyperlinksChanged>
  <AppVersion>16.0000</AppVersion>
</Properties>
"@

Set-Content -LiteralPath (Join-Path $tempRoot "[Content_Types].xml") -Value $contentTypesXml -Encoding UTF8
Set-Content -LiteralPath (Join-Path $tempRoot "_rels\.rels") -Value $relsXml -Encoding UTF8
Set-Content -LiteralPath (Join-Path $tempRoot "word\document.xml") -Value $documentXml -Encoding UTF8
Set-Content -LiteralPath (Join-Path $tempRoot "word\styles.xml") -Value $stylesXml -Encoding UTF8
New-Item -ItemType Directory -Force -Path (Join-Path $tempRoot "word\_rels") | Out-Null
Set-Content -LiteralPath (Join-Path $tempRoot "word\_rels\document.xml.rels") -Value $documentRelsXml -Encoding UTF8
Set-Content -LiteralPath (Join-Path $tempRoot "docProps\core.xml") -Value $coreXml -Encoding UTF8
Set-Content -LiteralPath (Join-Path $tempRoot "docProps\app.xml") -Value $appXml -Encoding UTF8

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::Open($zipPath, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($file in Get-ChildItem -LiteralPath $tempRoot -File -Recurse) {
        $entryName = $file.FullName.Substring($tempRoot.Length).TrimStart("\").Replace("\", "/")
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
            $archive,
            $file.FullName,
            $entryName,
            [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
}
finally {
    $archive.Dispose()
}

Move-Item -LiteralPath $zipPath -Destination $documentPath -Force

if (Test-Path -LiteralPath $tempRoot) {
    Remove-Item -LiteralPath $tempRoot -Recurse -Force
}

Write-Host "Created $documentPath"
