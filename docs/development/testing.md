# 测试规范

本文约定 ZenUI.Wpf 组件与转换器的自动化测试写法。测试应保护公开行为、WPF 契约和跨目标框架兼容性，避免绑定到无关的实现细节。

## 工程与框架

- 控件和主题测试放在 `tests/ZenUI.Wpf.Tests/`，目录与被测能力对应，例如 `Controls/`、`Theming/`。
- 转换器测试放在 `tests/ZenUI.Wpf.Converters.Tests/`，保证转换器包不通过测试项目间接依赖控件包。
- .NET 5、6、7 使用 `tests/ZenUI.Wpf.ModernCompatibilityTests/` 在对应运行时验证主题资源、控件元数据、Live Region 与转换器；这些已停止维护的运行时只提供兼容性保障。
- 使用项目现有的 MSTest；除独立 NuGet 包边界外，不为单个控件或功能新建测试工程。
- 普通测试使用 `[TestClass]` 和 `[TestMethod]`。
- 创建或操作 WPF 控件、窗口、模板、Dispatcher 或 UI Automation Peer 的测试使用 `[STATestClass]`。
- 测试必须兼容 CI 矩阵中的 .NET Framework 4.6.2～4.8.1，以及 `.NET 5～10 for Windows`；不要无条件使用仅在部分目标可用的测试 API。正式包构建 `net462`、`net471`、`net472`、`net5.0-windows` 与 `net8.0-windows` 资产，中间目标用于逐版本验证兼容性。

## 命名与组织

- 测试类按被测类型或一组紧密相关的公开能力命名，例如 `NumberBoxTests`。
- 测试方法用英文描述可观察行为，例如 `InvalidStepIsRejected`。
- 一个测试聚焦一个行为；只有建立同一场景所需的相关断言可以放在一起。
- 数据驱动测试只合并执行路径和断言语义一致的输入组合。
- 测试优先采用 Arrange、Act、Assert 的自然顺序；仅在较长测试中用空行或简短注释分隔阶段。

## 断言边界

优先验证使用者可观察到的结果：

- 依赖属性的默认值、校验、强制和变更行为；
- 控件事件、命令、键盘与鼠标交互；
- 控件模板契约、资源 Key 和主题切换；
- 默认、悬停、按下、焦点、禁用、只读、验证错误及高对比度状态；
- UI Automation 的控件类型、Pattern、名称和状态；
- 转换器的输入、输出、反向转换及边界值；
- 多目标框架下应保持一致的行为。

避免验证：

- 与公开契约无关的私有方法调用；
- 可自由调整的内部执行顺序；
- 仅为让测试通过而复制到测试中的生产逻辑；
- 恒真断言或只验证测试辅助方法本身的断言。

模板测试可以定位 `PART_` 部件，因为它们属于 WPF 模板契约。非契约的视觉树内部元素只有在其结构本身就是回归风险时才应直接断言。

## WPF 测试要求

- 需要模板实例化时，将控件放入临时 `Window`，调用 `Show`、`ApplyTemplate` 或 `UpdateLayout` 后再断言。
- 使用 `try/finally` 关闭测试创建的窗口，避免污染后续测试。
- 等待布局或 Dispatcher 时使用确定性的同步方式，不依赖任意时长的休眠。
- 测试不得依赖本机主题、DPI、区域设置、时区或执行顺序；确需依赖时应在测试中显式设置并恢复。
- UI Automation 测试验证语义契约，不依赖显示文本、屏幕坐标或易变的视觉树层级定位控件。

## 视觉回归

- 快照覆盖 Light、Dark、HighContrast 主题、Compact、Standard、Comfortable 密度以及仓库约定的 DPI 比例。
- 输出只用于审查时，测试仍需包含能自动发现明显退化的断言。
- 快照内容使用稳定、非业务化的示例数据，避免时间、随机数和机器相关信息。
- 更新快照或阈值时说明视觉变化的原因，不以放宽断言掩盖回归。

## Popup 与多显示器检查

自动化测试应至少覆盖弹层在主工作区底边和右边附近的翻转与边界约束。发布前还需在真实多显示器环境人工检查：

- 主、副显示器分别设置 100%、125%、150% 或 200% 缩放，覆盖混合 DPI。
- 将主显示器放在虚拟桌面的中间，并覆盖副显示器位于左侧或上方的负坐标布局。
- 在每台显示器的上、下、左、右边缘打开 ComboBox 与 DatePicker。
- 确认弹层停留在控件所在显示器的可用工作区内，不被任务栏或屏幕边缘裁切。
- 打开弹层后使用方向键、Enter、Escape 和 Tab，确认焦点、选择和关闭行为正常。
- 在弹层保持打开时移动窗口跨越显示器，确认布局不会停留在旧显示器或出现不可交互区域。

## 回归与验证

修复缺陷时先添加能复现问题的回归测试；纯重构、测试基础设施修复或为已有行为补覆盖时，不要求刻意制造失败。

测试验证分为以下三个等级，默认使用能够覆盖传统 WPF 兼容性的 `net472` 作为日常测试框架：

### 组件测试

组件新增、修改、删除或缺陷修复后，至少运行该组件及受影响能力的测试项目、测试类或测试方法。共享主题、Token、基类或公共测试辅助代码的改动必须包含所有受影响组件，不能只验证直接修改的文件。

例如，仅运行 Button 相关测试：

```powershell
dotnet test --project tests/ZenUI.Wpf.Tests/ZenUI.Wpf.Tests.csproj -c Release -f net472 --filter "FullyQualifiedName~ButtonTests"
```

### 单框架全量测试

改动影响多个组件、共享主题或测试基础设施，进行较大范围重构，或者准备提交 Pull Request 时，在 `net472` 上运行全部控件、主题和转换器测试：

```powershell
dotnet test --project tests/ZenUI.Wpf.Tests/ZenUI.Wpf.Tests.csproj -c Release -f net472 --max-parallel-test-modules 1
dotnet test --project tests/ZenUI.Wpf.Converters.Tests/ZenUI.Wpf.Converters.Tests.csproj -c Release -f net472 --max-parallel-test-modules 1
```

### 全框架矩阵测试

全框架矩阵在 .NET Framework 4.6.2～4.8.1 与 `.NET 8/9/10 for Windows` 上运行全部控件、主题和转换器测试，并在对应运行时上对 `.NET 5/6/7 for Windows` 运行兼容性冒烟测试。它用于目标框架或跨框架兼容层变更、构建与打包基础设施变更、发布验证，或明确要求全框架验证的场景；日常组件改动不要求在本地运行。

远程 CI 在隔离 Runner 中执行完整矩阵。本地确需运行时，完整测试模块必须串行执行，避免共享的 WPF、主题、Dispatcher、视觉快照或其他进程级状态互相干扰：

```powershell
dotnet test --project tests/ZenUI.Wpf.Tests/ZenUI.Wpf.Tests.csproj -c Release --max-parallel-test-modules 1
dotnet test --project tests/ZenUI.Wpf.Converters.Tests/ZenUI.Wpf.Converters.Tests.csproj -c Release --max-parallel-test-modules 1
foreach ($framework in @('net5.0-windows', 'net6.0-windows', 'net7.0-windows')) {
    dotnet run --project tests/ZenUI.Wpf.ModernCompatibilityTests/ZenUI.Wpf.ModernCompatibilityTests.csproj -c Release -f $framework
}
```

影响打包、公共 API 或多目标框架配置时，还应按 `CONTRIBUTING.md` 运行完整构建与打包检查。
