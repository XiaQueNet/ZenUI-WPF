function enhanceManagedReferencePage() {
  if (document.body.dataset.yamlMime !== "ManagedReference") {
    return;
  }

  const article = document.querySelector("main article");
  if (!article) {
    return;
  }

  const sectionLabels = {
    constructors: "构造函数",
    fields: "字段",
    properties: "属性",
    methods: "方法",
    events: "事件",
    operators: "运算符",
    eii: "显式接口实现",
  };

  for (const heading of article.querySelectorAll(":scope > h2.section")) {
    const label = sectionLabels[heading.id];
    if (label) {
      heading.dataset.sectionLabel = label;
    }
  }

  const memberHeadings = Array.from(
    article.querySelectorAll(":scope > h3[data-uid]"),
  );

  for (const heading of memberHeadings) {
    if (heading.parentElement !== article) {
      continue;
    }

    const card = document.createElement("section");
    card.className = "zenui-api-member";
    card.setAttribute("aria-labelledby", heading.id);
    article.insertBefore(card, heading);

    let node = heading;
    while (node) {
      const next = node.nextElementSibling;
      card.appendChild(node);

      if (
        !next ||
        next.matches("h2.section, h3[data-uid]") ||
        (next.matches("a[data-uid]") && next.nextElementSibling?.matches("h3[data-uid]"))
      ) {
        break;
      }

      node = next;
    }
  }
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", enhanceManagedReferencePage);
} else {
  enhanceManagedReferencePage();
}
