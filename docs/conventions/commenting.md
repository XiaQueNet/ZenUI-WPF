# C# 注释规范

本文件是项目 C# 注释的通用约定来源。模块、规格或契约文档可以在此基础上补充更严格规则。

---

## 总原则

注释用于解释代码无法直接表达的信息，包括：

- 业务规则
- 设计意图
- 边界条件
- 兼容性原因
- 临时方案原因
- 第三方系统限制
- 性能或并发约束

注释不应重复代码本身。

不推荐：

```csharp
// 获取商品名称
var name = product.Name;
```

推荐：

```csharp
// 第三方接口要求商品名称不能为空，否则会返回 400。
var name = string.IsNullOrWhiteSpace(product.Name) ? "-" : product.Name;
```

---

## 注释分层原则

项目中的注释按职责分为三类：

| 类型 | 作用 | 使用位置 |
| --- | --- | --- |
| XML 文档注释 | 描述公开契约 | public 接口、public 类型、public 成员 |
| 普通代码注释 | 解释特殊实现原因 | 方法内部、字段、局部逻辑 |
| 标记注释 | 标记待办、缺陷、兼容、临时方案 | TODO、FIXME、HACK、COMPAT 等 |

---

## XML 文档注释规范

### public 接口必须写完整注释

接口代表对外契约，必须描述清楚能力、参数、返回值和异常。

```csharp
/// <summary>
/// 提供收银秤读数采集能力。
/// </summary>
public interface IScaleReader
{
    /// <summary>
    /// 启动收银秤读数采集。
    /// </summary>
    void Start();

    /// <summary>
    /// 停止收银秤读数采集。
    /// </summary>
    void Stop();

    /// <summary>
    /// 获取当前最新称重读数。
    /// </summary>
    ScaleReading CurrentReading { get; }

    /// <summary>
    /// 当收银秤读数发生变化时触发。
    /// </summary>
    event EventHandler<ScaleReadingChangedEventArgs> ReadingChanged;
}
```

接口注释应描述能力和契约，不要描述具体实现。

不推荐：

```csharp
/// <summary>
/// 使用串口读取收银秤数据。
/// </summary>
public interface IScaleReader
{
}
```

推荐：

```csharp
/// <summary>
/// 提供收银秤读数采集能力。
/// </summary>
public interface IScaleReader
{
}
```

---

## 实现类注释规范

### 接口已有注释时，实现类 public 成员默认使用 `<inheritdoc />`

实现类不要重复复制接口注释，避免接口注释和实现注释不一致。

```csharp
public sealed class SerialPortScaleReader : IScaleReader
{
    /// <inheritdoc />
    public void Start()
    {
    }

    /// <inheritdoc />
    public void Stop()
    {
    }

    /// <inheritdoc />
    public ScaleReading CurrentReading { get; }

    /// <inheritdoc />
    public event EventHandler<ScaleReadingChangedEventArgs> ReadingChanged;
}
```

### 实现类有特殊实现时，在类上补充说明

如果实现类有特殊约束、兼容性、降级方案或性能原因，应在类注释中说明。

```csharp
/// <summary>
/// 基于串口轮询的收银秤读数服务。
/// </summary>
/// <remarks>
/// 部分厂商收银秤 SDK 不提供重量变化回调，因此该实现使用固定周期轮询读取重量。
/// </remarks>
public sealed class SerialPortScaleReader : IScaleReader
{
}
```

### 实现类方法有额外行为时，可在 `<inheritdoc />` 后补充 `<remarks>`

```csharp
/// <inheritdoc />
/// <remarks>
/// 该实现会在启动失败时尝试重新打开串口一次。
/// </remarks>
public void Start()
{
}
```

不要为了凑注释而重复接口说明。

### 实现类额外 public 成员必须单独注释

如果实现类中存在不属于接口的 public 成员，必须写 XML 注释。

```csharp
public sealed class SerialPortScaleReader : IScaleReader
{
    /// <inheritdoc />
    public void Start()
    {
    }

    /// <summary>
    /// 清空当前读数缓存。
    /// </summary>
    public void ResetBuffer()
    {
    }
}
```

实现类出现额外 public 成员时，应检查它是否真的需要公开。很多时候它应该是 `private`、`internal`，或者应该被抽象进接口。

---

## 类、接口、方法、属性、事件的注释要求

### 类注释

类注释应说明职责边界。

```csharp
/// <summary>
/// 默认价格计算服务，负责根据购物车快照计算商品优惠、整单优惠和最终应付金额。
/// </summary>
public sealed class PricingCalculationService : IPricingCalculationService
{
}
```

不推荐：

```csharp
/// <summary>
/// 价格计算类。
/// </summary>
public sealed class PricingCalculationService
{
}
```

### 方法注释

方法注释应说明“做什么”，不要逐行解释“怎么做”。

```csharp
/// <summary>
/// 根据购物车快照计算价格结果。
/// </summary>
/// <param name="cart">购物车快照。</param>
/// <returns>价格计算结果。</returns>
PriceSummary Calculate(CartSnapshot cart);
```

### 构造函数注释

构造函数默认不写 XML 注释，避免为只做字段赋值或依赖注入的构造函数补充重复说明。

以下情况需要写 XML 注释：

- 构造参数存在业务含义、单位、范围或默认约定。
- 构造函数会产生副作用，例如订阅事件、启动任务、打开设备连接。
- 构造函数有异常抛出规则。
- 类型是公共 API、SDK、基础库暴露给其他模块使用。
- 生命周期或释放规则不明显，例如需要 `Dispose` 取消订阅。

模块、规格或契约文档可以定义更严格规则；出现更严格规则时，以对应文档为准。例如 Pricing Contract 可要求特定 DTO 构造函数必须写 XML 注释。

### 属性注释

属性注释应说明业务含义。涉及数量、金额、重量、时间时，必须说明单位。

```csharp
/// <summary>
/// 当前稳定重量，单位为克。
/// </summary>
public decimal StableWeight { get; init; }
```

布尔属性应说明 `true` 的含义。

```csharp
/// <summary>
/// 获取一个值，表示当前读数是否已经达到稳定状态。
/// </summary>
public bool IsStable { get; init; }
```

### 事件注释

事件注释应说明触发时机。

```csharp
/// <summary>
/// 当收银秤读数发生变化时触发。
/// </summary>
event EventHandler<ScaleReadingChangedEventArgs> ReadingChanged;
```

如果事件有特殊条件，应写清楚。

```csharp
/// <summary>
/// 当稳定重量从无商品状态变化为有商品状态时触发。
/// </summary>
event EventHandler<ItemPlacedEventArgs> ItemPlaced;
```

---

## XML 标签使用规范

### `<summary>`

用于描述类型或成员职责。

要求：

- 使用完整句子
- 描述业务含义
- 不写实现细节
- 不写空泛词

推荐：

```csharp
/// <summary>
/// 处理收银称重读数变化。
/// </summary>
```

不推荐：

```csharp
/// <summary>
/// 处理方法。
/// </summary>
```

### `<param>`

用于说明参数含义和限制。

```csharp
/// <param name="amount">支付金额，必须大于 0。</param>
```

### `<returns>`

用于说明返回值含义。

```csharp
/// <returns>商品匹配成功时返回识别结果，否则返回 null。</returns>
```

### `<exception>`

方法主动抛出明确异常时，应说明。

```csharp
/// <exception cref="ArgumentNullException">
/// 当 <paramref name="cart"/> 为 null 时抛出。
/// </exception>
```

### `<remarks>`

用于说明补充规则、设计原因、兼容性说明。

```csharp
/// <remarks>
/// 该服务只负责计算价格，不修改购物车状态。
/// 购物车状态变更应由应用层用例负责。
/// </remarks>
```

### `<inheritdoc />`

用于接口实现、抽象类实现、重写方法。

```csharp
/// <inheritdoc />
public PriceSummary Calculate(CartSnapshot cart)
{
}
```

推荐规则：

- 接口成员写完整注释
- 实现成员默认写 `<inheritdoc />`
- 实现有特殊行为时，加 `<remarks>`
- 不复制接口注释

---

## 普通代码注释规范

### 方法内部注释只解释特殊原因

推荐：

```csharp
// 厂商秤在空载时可能返回 1g ~ 3g 抖动，因此不能直接用 0 判断无商品。
var isEmpty = weight < emptyThreshold;
```

不推荐：

```csharp
// 判断重量是否小于阈值
var isEmpty = weight < emptyThreshold;
```

### 字段通常不需要注释

字段名应尽量自解释。只有涉及缓存、并发、性能、兼容性时才写注释。

```csharp
// SQLite 在 Win7 低配机器上写入较慢，使用单写队列避免多线程写冲突。
private readonly Channel<WriteCommand> _writeQueue;
```

### 业务规则注释应写在规则所在位置

不要把业务规则注释散落在 UI 事件中。

推荐：

```csharp
// 单品改价后，不再参与单品促销，但仍可参与整单满减。
private bool CanApplyItemPromotion(CartItem item)
{
}
```

更推荐：

```csharp
public sealed class ManualPriceChangeExcludesItemPromotionRule
{
}
```

规则复杂时，应优先通过命名对象、Spec、BDD 测试表达，而不是堆大段注释。

---

## 标记注释规范

### 允许的标记

| 标记 | 含义 |
| --- | --- |
| TODO | 后续需要完成 |
| FIXME | 已知缺陷，需要修复 |
| HACK | 临时方案，需要替换 |
| NOTE | 重要说明 |
| PERF | 性能相关说明 |
| COMPAT | 兼容性说明 |
| SECURITY | 安全相关说明 |

### TODO 必须说明原因和后续动作

推荐：

```csharp
// TODO(Jiang): 等后端补充会员等级字段后，改为按会员等级计算折扣。
```

不推荐：

```csharp
// TODO: 优化
```

### HACK 必须说明为什么是临时方案

```csharp
// HACK: 当前后端暂未提供促销活动范围字段，临时按门店维度过滤。
// 接口补齐后应改为按活动范围计算。
```

### COMPAT 必须说明兼容对象

```csharp
// COMPAT: Win7 SP1 环境下部分厂商 SDK 不支持异步回调，
// 因此这里使用 100ms 轮询读取称重数据。
```

---

## 异常处理注释规范

普通异常处理不需要注释。只有以下情况必须说明：

- 吞异常
- 降级处理
- 重试
- 忽略特定错误
- 保持兼容行为

推荐：

```csharp
try
{
    _voice.Speak(text);
}
catch (SapiException ex)
{
    // COMPAT: 部分 Win7 精简系统可能缺失 SAPI 组件。
    // 这里降级为预录音频播报，避免收银流程中断。
    _logger.Warn(ex, "SAPI voice failed. Fallback to audio files.");
    _fallbackVoice.Speak(text);
}
```

不推荐：

```csharp
try
{
}
catch (Exception ex)
{
    // 捕获异常
}
```

---

## 测试代码注释规范

测试代码优先通过测试名表达业务规则。

推荐：

```csharp
[TestMethod]
public void Calculate_Should_Use_Lowest_Item_Price_When_Member_And_Promotion_Both_Available()
{
}
```

复杂测试可以使用 Given / When / Then 注释。

```csharp
// Given: 商品同时存在会员价、促销价、特价
// When: 计算购物车价格
// Then: 应选择最低的单品价
```

不要在测试中解释普通代码语法。

---

## 注释语言规范

团队内部业务系统统一使用中文注释。

以下情况可以保留英文：

- 技术名词
- 第三方接口字段
- 协议字段
- 业务系统固定英文名称
- 开源项目或 SDK 代码

同一文件内尽量不要中英文混杂。新增或修改注释时，必须同步检查附近注释是否过期；如果编辑附近英文注释，应按项目约定翻译为中文。

---

## 禁止的注释

禁止以下类型的注释：

### 重复代码

```csharp
// 判断是否为空
if (item == null)
{
}
```

### 空泛注释

```csharp
// 处理逻辑
Process();
```

### 过期注释

```csharp
// 计算会员价
private decimal CalculatePromotionPrice()
{
}
```

### 情绪化注释

```csharp
// 这里写得很烂，之后再说
```

### 无责任人的 TODO

```csharp
// TODO: 以后优化
```

---

## 项目规则

本项目统一采用以下规则：

1. `public interface` 必须写完整 XML 注释。
2. `public class` 建议写 XML 注释；领域类、服务类、基础设施实现类必须写。
3. 接口实现类的 public 成员默认使用 `<inheritdoc />`。
4. 实现类有特殊约束时，在类上使用 `<remarks>` 说明。
5. 实现类额外暴露的 public 成员必须单独写 XML 注释；构造函数按构造函数专门规则判断。
6. 构造函数默认不写 XML 注释，命中特殊规则或更严格模块规范时才写。
7. `private` 方法默认不写 XML 注释，优先通过命名表达意图。
8. 方法内部注释只解释原因，不解释语法。
9. TODO、FIXME、HACK、COMPAT 必须说明原因和后续动作。
10. 修改代码时必须同步检查注释是否过期。
11. 注释和代码冲突时，以代码为准，并立即修正注释。

---

## 推荐模板

### 接口模板

```csharp
/// <summary>
/// 提供 XXX 能力。
/// </summary>
public interface IXxxService
{
    /// <summary>
    /// 执行 XXX。
    /// </summary>
    /// <param name="request">XXX 请求。</param>
    /// <returns>XXX 结果。</returns>
    XxxResult Execute(XxxRequest request);
}
```

### 实现类模板

```csharp
/// <summary>
/// 默认 XXX 服务。
/// </summary>
/// <remarks>
/// 该实现用于 XXX 场景，不负责 XXX。
/// </remarks>
public sealed class XxxService : IXxxService
{
    /// <inheritdoc />
    public XxxResult Execute(XxxRequest request)
    {
    }
}
```

### 兼容性模板

```csharp
// COMPAT: 由于 XXX 环境存在 XXX 限制，这里采用 XXX 方案。
```

### 临时方案模板

```csharp
// HACK: 当前 XXX 尚未支持 XXX，临时使用 XXX。
// 后续 XXX 完成后应替换为 XXX。
```

### 待办模板

```csharp
// TODO(负责人): 等 XXX 完成后，改为 XXX。
```

---

## 提交前检查清单

提交代码前检查：

- public 接口和成员是否有完整 XML 注释？
- 实现类成员是否使用 `<inheritdoc />`，而不是复制接口注释？
- 实现类是否有特殊实现需要说明？
- 构造函数是否命中特殊情况，需要 XML 注释？
- 注释是否解释了原因，而不是重复代码？
- TODO / FIXME / HACK / COMPAT 是否说明了原因和后续动作？
- 注释是否和当前代码一致？
- 是否存在可以通过更好命名删除的注释？
- 单位、边界、异常、兼容性是否说明清楚？
- 测试名是否已经表达业务规则？

---

## 一句话总结

好的注释回答：

- 为什么这样做？
- 这个规则从哪里来？
- 这里有什么坑？
- 修改这里要注意什么？

坏注释只是把代码翻译成中文。
