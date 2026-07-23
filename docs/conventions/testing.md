# Testing Conventions

本文件是项目测试代码的通用约定来源。

AI 生成或修改测试时，还必须遵守：

- `docs/conventions/ai-testing-workflow.md`

如果测试来源是 `docs/features/**/*.feature`，还必须遵守：

- `docs/prompts/feature-to-unit-tests.md`

---

## Unit Test Project Rules

新增或大幅改写的单元测试必须放入：

src/tests/Hlk.UnitTests/

禁止：

- 创建新的 UnitTest 项目
- 创建 xxx.AppService.Tests
- 创建 xxx.Service.Tests
- 创建 xxx.FeatureTests 测试项目或测试分组

测试目录按业务模块组织：

src/tests/Hlk.UnitTests/
├── Pricing/
├── Cart/
├── Checkout/
└── Membership/

历史测试如果暂未完全符合本约定，可以在后续相关改动中逐步迁移；
不要为了形式一致性单独大规模重命名或搬移旧测试。

---

## Test Naming

测试类和测试方法必须基于业务行为命名，
不能基于技术实现。

测试是业务规则资产，
而不是实现类附属物。

测试文件名和测试类名应体现 Feature / Rule / 业务能力，例如：

- `LowestPricePriorityTests`
- `PromotionConflictTests`
- `FullReductionTests`
- `ManualPriceOverrideTests`
- `QuantityChangeResetTests`

测试方法命名应表达业务结果，例如：

- `Should_Use_Lower_Price_When_MemberPrice_And_PromotionPrice_Both_Matched`
- `Should_Clear_Manual_Item_Price_When_Line_Quantity_Changed`

### 错误示例

- PricingAppServiceFeatureTests
- AmountModelFeatureTests
- PricingServiceTests
- PricingEngineTests
- CalculateLineAmount_ShouldReturnExpected
- BuildExecutionPlan_ShouldOrderRules

禁止测试文件名和测试类名包含：

- AppService
- Service
- Manager
- Engine
- Pipeline
- Handler
- Calculator
- Repository

当测试来源是 `docs/features/**/*.feature` 时，测试文件名和测试类名还禁止使用：

- FeatureTest
- FeatureTests

错误示例：

- `AmountModelFeatureTests.cs`
- `RuleGraphFeatureTests`

正确示例：

- `PricingAmountModelLineAmountTests.cs`
- `PricingRuleGraphMetadataTests`

---

## Test Structure

普通业务测试应围绕业务能力、业务规则或可观察行为组织。

如果测试来源是 `docs/features/**/*.feature`，必须遵守
`docs/prompts/feature-to-unit-tests.md` 的 Feature / Rule / Scenario 映射规则。

推荐：

- 一个业务能力或业务规则对应一组测试
- 一个测试类聚焦一个清晰的业务主题
- 测试方法表达一个可观察业务结果
- 数据驱动测试只合并同一业务规则下的等价样例

禁止：

- 把多个 `.feature` 文件合并到同一个测试类
- 把多个 `.feature` 文件合并到同一个测试文件
- 把同一个 `.feature` 文件中的多个 Rule 合并到同一个测试类
- 以业务主题相近为理由合并 Feature 测试类

以上 `.feature` 相关禁止项只适用于从 `docs/features/**/*.feature`
生成或维护的 Feature-based Unit Tests。

---

## Assertions

测试应验证：

- 最终价格
- 命中规则
- 冲突结果
- 清除规则
- 叠加结果
- 状态变化后的规则重算结果

不要验证：

- 内部方法调用
- 私有实现
- Pipeline 顺序细节
- 具体类是否被调用

---

## Framework

使用项目现有测试框架。

如果项目没有明确测试框架，默认使用：

- MSTest
- FluentAssertions

MSTest 项目中：

- 普通测试使用 `[TestMethod]`
- 数据驱动测试使用 `[TestMethod]` + `[DataRow(...)]`
