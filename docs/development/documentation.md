# 文档编写规范

本文约定 ZenUI.Wpf 仓库中文档的组织、命名和链接方式。新增、移动或重命名 Markdown 文档时应遵循本文。

## 文档分类

`docs/` 按文档用途分类：

- `design/`：设计原则、控件规范和设计决策。
- `development/`：编码、测试、文档和协作规范。
- `maintainers/`：版本、发布和项目维护流程。

新文档应放入语义最接近的现有分类。只有形成一组职责明确的文档时才新增一级分类，避免为单个文件创建目录。

`docs/README.md` 是文档导航入口。新增、移动或删除文档时，应同步更新该索引；需要从项目首页直接发现的文档，还应更新仓库根目录的 `README.md`。

## 文件与目录命名

仓库级约定文件使用社区通行的大写名称：

- `README.md`
- `CONTRIBUTING.md`
- `CHANGELOG.md`
- `LICENSE`
- `SECURITY.md`
- `CODE_OF_CONDUCT.md`

这些名称用于项目入口、贡献说明、变更记录、许可证和社区健康文件。分类目录中的普通文章不使用全大写文件名。

普通文档文件和目录遵循以下规则：

- 只使用小写英文字母、数字和连字符。
- 使用 `.md` 扩展名。
- 多个单词使用连字符分隔，不使用空格或下划线。
- 名称应简短、明确，并能表达文档主题。
- 文件名使用英文，文档标题和正文使用中文。
- `README.md` 仅用于仓库或目录的导航入口，不作为普通文章名称。

推荐：

```text
docs/design/component-design.md
docs/development/ai-testing-workflow.md
docs/maintainers/releasing.md
```

不推荐：

```text
docs/design/ComponentDesign.md
docs/development/ai_testing_workflow.md
docs/maintainers/RELEASING.md
docs/发布规范.md
```

## 标题与内容

- 每份文档只有一个一级标题，标题应准确说明文档主题。
- 标题层级按顺序递进，不跳过必要层级。
- 规范应描述稳定的原则、契约和判断标准，易变的实现状态应放在代码、测试或变更记录中。
- 同一规则只保留一个权威来源；其他文档使用摘要和链接引用，避免复制后产生不一致。
- 示例应使用稳定、非业务化的数据，不包含密钥、个人信息或机器相关路径。

## Gallery 控件示例

- Gallery 页面使用统一的页面边距、标题层级和分节节奏。优先通过留白、排版和对齐组织内容，不为每个分节重复添加背景、边框、圆角或阴影。
- 示例首先展示组件的默认呈现，再按需展示确有决策价值的状态、变体或业务场景。新增示例必须说明不同于现有内容的能力，不为增加数量重复排列相似状态。
- 页面可以使用真实但简短的业务场景帮助理解组件协作；场景不得掩盖组件默认行为，也不得同时引入多个相互竞争的视觉焦点。
- 表格、输入区域、浮层和状态表面可以保留表达自身边界所必需的容器；纯粹用于页面分组的外层容器应优先使用留白。
- 当前 Gallery 不设置统一的“推荐用法”代码块。需要说明行为差异、限制或安全要求时，使用就近说明文字；未来引入代码示例时，应先定义统一的内容、格式和复制体验。
- `Overview` 用于表达产品定位和典型协作流程，组件详情页用于展示控件能力，`Token` 页面用于解释设计资源；不同页面不得重复承担同一套展示内容。
- 普通控件示例不得设置 `FontSize`，应直接展示组件库的默认排版。
- 只有明确标注的字号定制示例，或使用字体字符表达图标的特殊场景，才可以覆盖控件字号；字体图标能够使用矢量资源表达时，优先使用矢量资源。
- Gallery 的导航、页面标题、字段标签和说明文字属于展示程序自身界面，可以使用语义化 Typography Token 设置字号。
- 不得在页面、卡片或其他公共父级上设置字号并使其意外影响控件示例；确需覆盖时，将作用域限制在对应的专项示例内。

## 链接

- 仓库内文档使用相对链接，使链接在分支和 Fork 中仍然有效。
- 链接文字应说明目标内容，不使用“点击这里”等缺少语义的文字。
- 从当前文档所在目录计算相对路径，不使用本机绝对路径。
- 移动或重命名文档时，必须搜索并更新仓库内全部旧路径。
- 提交前验证所有本地 Markdown 链接均能解析。

示例：

```markdown
[测试规范](testing.md)
[控件设计规范](../design/component-design.md)
[贡献指南](../../CONTRIBUTING.md)
```

## 变更检查清单

新增、移动或重命名文档后，确认：

- [ ] 文件位于正确的分类目录。
- [ ] 普通文档使用小写和连字符命名。
- [ ] `README.md` 等特殊文件仅用于约定用途。
- [ ] `docs/README.md` 和根目录入口已按需更新。
- [ ] 仓库中不存在旧路径引用。
- [ ] 所有本地 Markdown 链接均可解析。
- [ ] Gallery 页面优先使用留白建立层级，没有重复的分组容器。
- [ ] Gallery 新增示例展示了不同于现有内容的能力或场景。
- [ ] Gallery 中不存在未统一规划的“推荐用法”代码块。
- [ ] Gallery 普通控件示例未覆盖组件库默认字号。
- [ ] `git diff --check` 通过。

## 参考

- [GitHub：关于仓库 README 文件](https://docs.github.com/zh/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-readmes)
- [GitHub：设置仓库贡献者指南](https://docs.github.com/zh/communities/setting-up-your-project-for-healthy-contributions/setting-guidelines-for-repository-contributors)
- [Microsoft Learn：.NET 文档文件命名规范](https://learn.microsoft.com/zh-cn/contribute/content/dotnet/dotnet-style-guide#文件名)
