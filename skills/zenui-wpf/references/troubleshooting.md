# 排查与验收

先复现具体症状，再检查对应边界，不因为一个问题替换整套主题或重写控件。

| 症状 | 优先检查 |
| --- | --- |
| 找不到 ZenUI 类型 | 消费者包引用、目标框架、XAML 命名空间、当前包版本是否有该类型 |
| 找不到属性或枚举值 | 对照当前版本 API；Watermark 不是 PlaceholderText，按钮 Danger 与提示 Error 不同 |
| 控件出现但应用 Token 无法解析 | 控件自动默认样式与应用资源查找是两条路径；页面使用 Token 时显式合并 Generic.xaml |
| 复制示例后缺少资源 | GalleryCardStyle 等是示例专用资源；移除示例依赖并使用应用自己的样式 |
| 切换主题后颜色不变 | 硬编码颜色、StaticResource、更近作用域覆盖、本地属性值、是否在 UI 线程更新正确的字典 |
| 输入值未回写 | DataContext、绑定路径、类型、TwoWay、UpdateSourceTrigger、通知及校验错误 |
| 集合变化但空状态不更新 | 绑定集合对象不会自动重跑转换器；使用可通知的 Count 或独立状态属性 |
| 菜单命令不执行 | Popup 边界的数据上下文、PlacementTarget、行命令与页面命令来源、CanExecute |
| 表格过高或滚动异常 | 布局是否给出有限高度、是否套入无限高度 StackPanel/外层 ScrollViewer、是否破坏虚拟化 |
| 加载状态不结束 | 业务异步流程应在适当的 finally 路径恢复 IsBusy；ZenLoading 不管理请求生命周期 |

## 对消费者改动的检查

1. 使用目标项目本来的构建方式；SDK 风格项目可 `dotnet build <项目路径> -c Release`。不复制组件库发布命令到消费者项目。
2. 启动受影响页面，检查输出中的 WPF 绑定错误、资源缺失和转换失败；构建无法发现所有绑定路径拼写错误。
3. 按改动覆盖输入回写、验证、空集合、加载、命令可用性与键盘焦点。
4. 改动主题时检查 Light、Dark、HighContrast，改动密度时检查所用 Density；检查文字对比度和内容裁切。
5. 说明实际执行的检查。只有构建或 XAML 加载通过时，不声称已完成视觉、屏幕阅读器或全部交互验收。

## Skill 的真实任务验收

维护者更新资料后，在独立消费者示例中执行以下任务；这些是评估场景，不要求每次使用 Skill 都跑一遍。

| 请求 | 可观察的合格结果 |
| --- | --- |
| 用 ZenUI 做名称、数量、日期表单 | 使用真实控件；Quantity 为 decimal；日期为 DateTime?；明确 DataContext 和验证责任；可构建 |
| 给现有页面增加深色模式和紧凑密度 | 保留现有资源；正确调用两个管理器；页面资源随主题变化；保留高对比度默认行为 |
| 用按钮 Ghost 外观做次要操作 | 识别当前 ButtonAppearance 不支持 Ghost，说明后使用合适的 Text/Outlined 或已确认定制入口 |
| 给登录页添加双向绑定 Password | 识别无明文 Password DP；采用 SecurePassword 边界，不虚构属性 |
| 表格为空显示提示，新增数据后隐藏 | 使用 EmptyContent 或可通知的 Count 绑定，新增元素后状态确实更新 |
| 仅使用布尔可见性转换 | 只依赖转换器包，使用正确命名空间及 IsInverted/UseCollapsed 参数 |
| 为已有其他 UI 框架项目修改普通页面 | 没有 ZenUI 意图或依赖时不主动迁移到 ZenUI |

比较启用 Skill 前后的非法 API 数、首次构建结果、绑定错误和人工修正量。格式校验只能证明文件结构有效，不能替代这些行为验收。
