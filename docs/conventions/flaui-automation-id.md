# FlaUI AutomationId 命名规范

本文约定 WPF POS 系统中 `AutomationProperties.AutomationId` 的命名方式。  
所有 AutomationId 必须统一定义在：

`src/Module/Automation/Hlk.Automation.Contracts/AutomationIds.cs`

## 基本原则

- 不在 XAML 或 UI 测试中直接硬编码 AutomationId 字符串。
- XAML 通过 `{x:Static ...}` 引用 `AutomationIds` 常量。
- FlaUI 测试通过 `AutomationIds` 常量查找控件。
- AutomationId 是 UI 自动化测试契约，命名后应保持稳定。

## 设置范围

AutomationId 只应设置在可交互、可定位、可断言、会参与关键流程的元素上，避免给纯布局或装饰元素滥设 ID。

以下元素必须设置 AutomationId：

- 关键业务流程中的输入控件，例如 `TextBox`、`PasswordBox`、`ComboBox`、日期输入、数字输入。
- 可触发业务动作的控件，例如 `Button`、`ToggleButton`、`RadioButton`、`CheckBox`、菜单按钮、工具栏按钮。
- 需要在 FlaUI 测试中读取或断言的状态和结果元素，例如金额、错误提示、订单状态、会员信息、加载状态。
- 可作为测试入口或定位容器的页面、弹窗、面板和列表，例如登录页、支付弹窗、商品列表、购物车列表、结果列表。
- 动态集合中需要定位的列表容器、列表项模板根元素，或列表项内可操作控件。
- 同一业务控件存在多种可见形态时，每个测试会定位的形态都应设置，例如密码框的 `Masked` 和 `PlainText`。

以下元素通常不需要设置 AutomationId：

- 纯布局元素，例如 `Grid`、`StackPanel`、`Border`。
- 纯装饰元素，例如图标、分隔线、背景、阴影、装饰图片。
- 不参与测试定位或断言的静态说明文本。
- 只作为样式模板内部结构、且不会被测试直接查找的子元素。

判断准则：

- 如果 UI 测试需要 `FindByAutomationId` 定位它，就必须设置。
- 如果元素变化会影响关键业务验收，且测试需要断言它，就必须设置。
- 如果只能通过中文显示文本、层级顺序或坐标找到它，应优先补 AutomationId。

## 命名格式

统一使用点号分段：

`模块.视图.控件[.状态或子元素]`

示例：

```csharp
Login.LoginView.UserNameInput
Login.LoginView.PasswordInput.Masked
Login.LoginView.PasswordInput.PlainText
```

## 分段规则

| 分段 | 说明 | 示例 |
| --- | --- | --- |
| 模块 | 业务模块或功能区域 | `Login`、`Cashier`、`Product` |
| 视图 | XAML View 名称 | `LoginView`、`CartView` |
| 控件 | 具体可交互或需断言的 UI 元素 | `UserNameInput`、`SubmitButton` |
| 状态或子元素 | 同一控件的不同形态或内部元素 | `Masked`、`PlainText` |

## C# 定义规则

`AutomationIds.cs` 中的嵌套类结构应与字符串分段保持一致。

```csharp
public static class AutomationIds
{
    public static class Login
    {
        public static class LoginView
        {
            public const string UserNameInput = "Login.LoginView.UserNameInput";

            public static class PasswordInput
            {
                public const string Masked = "Login.LoginView.PasswordInput.Masked";
                public const string PlainText = "Login.LoginView.PasswordInput.PlainText";
            }
        }
    }
}
```

## 控件命名建议

控件名使用 PascalCase，并体现控件用途：

- 输入框：`UserNameInput`、`PasswordInput`
- 按钮：`LoginButton`、`SearchButton`
- 文本：`TotalAmountText`、`ErrorMessageText`
- 列表：`ProductList`、`CartItemList`
- 弹窗：`ConfirmDialog`、`PaymentDialog`

## 使用示例

XAML：

```xml
<UserControl xmlns:automation="clr-namespace:Hlk.Automation.Contracts;assembly=Hlk.Automation.Contracts">
    <TextBox
        AutomationProperties.AutomationId="{x:Static automation:AutomationIds+Login+LoginView.UserNameInput}" />
</UserControl>
```

FlaUI 测试：

```csharp
using Hlk.Automation.Contracts;

var userNameTextBox = app.FindByAutomationIdInTopLevelWindows(
    automation,
    AutomationIds.Login.LoginView.UserNameInput)?.AsTextBox();
```

## 禁止事项

- 禁止使用无业务含义的 ID，例如 `TextBox1`、`Button2`。
- 禁止在测试中硬编码字符串，例如 `"Login.LoginView.UserNameInput"`。
- 禁止复用同一个 AutomationId 给多个同屏控件。
- 禁止为了当前布局命名，例如 `LeftPanelButton`，应按业务含义命名。
