# AI Testing Workflow

本文件是 AI 生成、修改、修复测试时的流程约束。

测试代码的目录、命名、断言和框架约定以 `docs/conventions/testing.md` 为准。

如果任务是从 `docs/features/**/*.feature` 生成普通单元测试，还必须先阅读并遵守：

- `docs/prompts/feature-to-unit-tests.md`

---

## Core Flow

新增业务规则测试、从 Feature / Spec 生成测试、或修复业务行为缺陷时，
必须遵循：

Feature / Spec -> Failing Tests -> Implementation -> Refactor

也就是：

1. 先从业务规范生成失败测试（Red）。
2. 再实现或修正业务代码让测试通过（Green）。
3. 最后在不改变业务断言的前提下重构（Refactor）。

禁止：

- Spec Drift
- Fake Tests
- Vacuous Assertions
- 实现驱动测试
- 先实现业务代码再反向补测试
- 修改测试语义来适配当前实现

以下维护性任务不强制要求先制造 Red 状态，但仍必须保持业务断言不降级：

- 重命名测试以符合约定
- 拆分或搬移测试文件
- 修复测试工程配置、引用或编译错误
- 改善测试 helper 的可读性
- 为已实现且缺少覆盖的历史行为补充回归测试

如果任务不适用 Red First，输出中应简要说明原因。

---

## Feature-Based Unit Tests

当任务是根据 `docs/features/**/*.feature` 生成普通单元测试时，必须先阅读：

- `docs/prompts/feature-to-unit-tests.md`
- `docs/conventions/testing.md`
- 相关 `docs/specs/*.md`
- 目标 `docs/features/**/*.feature`

具体映射规则、BDD 禁止项、输出要求和 Spec Drift 处理规则以
`docs/prompts/feature-to-unit-tests.md` 为准。

---

## Red First Requirements

生成失败测试时必须验证真实业务结果。
可验证的业务结果类型以 `docs/conventions/testing.md` 的 Assertions 章节为准。

禁止为了让测试通过而：

- 在测试文件中实现完整业务规则
- 写只验证测试 helper 自身逻辑的测试
- 降低断言或移除关键断言
- 使用 `Assert.True(true)` 或等价空断言
- 使用 mock 掩盖真实业务行为

如果生产代码尚未提供可调用 API，测试应保持 Red，并清楚暴露缺失的业务入口，而不是在测试中补一套会通过的实现。

临时测试适配入口的边界以 `docs/prompts/feature-to-unit-tests.md` 的 Red First 章节为准。
