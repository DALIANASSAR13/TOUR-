# Builds docs/<name>.html from docs/<name>.md (Markdown + Mermaid rendered in the browser).
# Usage: python .build/make_html.py [name]   (default name: class-diagram)
import json, pathlib, sys
root = pathlib.Path(__file__).resolve().parent.parent
name = sys.argv[1] if len(sys.argv) > 1 else "class-diagram"
md = (root / "docs" / f"{name}.md").read_text(encoding="utf-8")
html = """<!doctype html>
<html lang="en"><head><meta charset="utf-8">
<title>Tour Guide Booking</title>
<style>
  @page { size: A4 landscape; margin: 12mm 12mm 14mm 12mm; }
  body { font-family: "Segoe UI", Arial, sans-serif; color: #1f2328; font-size: 10.5pt; line-height: 1.45; margin: 0; }
  main { max-width: 1060px; margin: 0 auto; padding: 8px 4px; }
  h1 { font-size: 22pt; border-bottom: 2px solid #0b5f7a; padding-bottom: 6px; color: #0b5f7a; }
  h2 { font-size: 15pt; color: #0b5f7a; margin-top: 18px; break-before: page; }
  h2:first-of-type { break-before: auto; }
  h3 { font-size: 12.5pt; margin-top: 14px; break-after: avoid; }
  table { border-collapse: collapse; width: 100%; margin: 8px 0 12px; font-size: 9.5pt; break-inside: auto; }
  tr { break-inside: avoid; }
  th, td { border: 1px solid #c9d1d9; padding: 4px 6px; text-align: left; vertical-align: top; }
  th { background: #e8f2f5; }
  code { font-family: Consolas, monospace; font-size: 9pt; background: #f2f4f6; padding: 0 3px; border-radius: 3px; }
  hr { display: none; }
  .mermaid-wrap { text-align: center; margin: 10px 0; break-inside: avoid; }
  .mermaid-wrap svg { max-width: 100% !important; height: auto; max-height: 148mm; }
  #status { display: none; }
</style></head>
<body><main id="content"></main><div id="status">loading</div>
<script src="https://cdn.jsdelivr.net/npm/marked@12.0.2/marked.min.js"></script>
<script type="module">
import mermaid from "https://cdn.jsdelivr.net/npm/mermaid@11.4.1/dist/mermaid.esm.min.mjs";
const md = __MD__;
const renderer = new marked.Renderer();
let n = 0; const blocks = [];
renderer.code = (code, lang) => {
  const text = typeof code === "object" ? code.text : code;
  const l = typeof code === "object" ? code.lang : lang;
  if (l === "mermaid") { blocks.push(text); return `<div class="mermaid-wrap" id="m${n++}"></div>`; }
  return `<pre><code>${text.replace(/&/g,"&amp;").replace(/</g,"&lt;")}</code></pre>`;
};
document.getElementById("content").innerHTML = marked.parse(md, { renderer });
mermaid.initialize({ startOnLoad: false, theme: "default", securityLevel: "loose",
  class: { useMaxWidth: true }, flowchart: { useMaxWidth: true }, state: { useMaxWidth: true } });
const errors = [];
for (let i = 0; i < blocks.length; i++) {
  try { const { svg } = await mermaid.render("svg" + i, blocks[i]); document.getElementById("m" + i).innerHTML = svg; }
  catch (e) { errors.push(i + ": " + e.message); document.getElementById("m" + i).textContent = "Diagram error: " + e.message; }
}
window.__result = { diagrams: blocks.length, errors };
document.getElementById("status").textContent = errors.length ? "errors" : "done";
document.title = errors.length ? "ERRORS" : "Tour Guide Booking";
</script></body></html>
""".replace("__MD__", json.dumps(md))
(root / "docs" / f"{name}.html").write_text(html, encoding="utf-8")
print("ok", md.count("```mermaid"))
