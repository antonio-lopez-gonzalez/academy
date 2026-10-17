/**
 * highlight.js — Resaltador de sintaxis propio y sin dependencias.
 *
 * ¿Por qué lo escribimos a mano en lugar de usar Prism o highlight.js?
 *  1. La web del curso debe funcionar offline, sin CDNs.
 *  2. Es un ejercicio didáctico: entender cómo se tokeniza código.
 *
 * Cómo funciona (técnica de "placeholders"):
 *  1. Extraemos comentarios y strings y los sustituimos por marcas
 *     invisibles (\u0001N\u0002) para que las reglas siguientes no los toquen.
 *  2. Aplicamos expresiones regulares sobre el resto (keywords, números…).
 *  3. Restauramos las marcas envolviéndolas en sus <span> de color.
 *
 * Lenguajes soportados: js, jsx, html, css, bash, json, csharp, sql, text.
 */
(function () {
  "use strict";

  /* ---------- utilidades ---------- */

  // Escapa el texto para poder insertarlo como HTML sin que se rompa
  function escapeHtml(text) {
    return text
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
  }

  // Envuelve un fragmento en un span con la clase de token dada
  function span(type, text) {
    return '<span class="tok-' + type + '">' + text + "</span>";
  }

  /**
   * Extrae comentarios y strings del código ya escapado.
   * patterns: array de regex (sobre texto escapado) con su tipo de token.
   * Devuelve { code, restore } donde restore(html) devuelve el HTML final.
   */
  function extract(code, patterns) {
    const saved = [];
    let out = code;
    for (const { regex, type } of patterns) {
      out = out.replace(regex, (match) => {
        const index = saved.length;
        saved.push(span(type, match));
        // Usamos UN SOLO carácter del área privada de Unicode como índice
        // No hay dígitos, así que las regex no pueden capturarlos
        return String.fromCharCode(0xE000 + index);
      });
    }
    return {
      code: out,
      restore(html) {
        return html.replace(/[\uE000-\uF8FF]/g, (char) => {
          const index = char.charCodeAt(0) - 0xE000;
          return saved[index] || char;
        });
      },
    };
  }

  // Construye una regex de keywords con límites de palabra
  function kw(words) {
    return new RegExp("\\b(" + words.join("|") + ")\\b", "g");
  }

  /* ---------- definiciones por lenguaje ---------- */

  const JS_KEYWORDS = [
    "const", "let", "var", "function", "return", "if", "else", "for", "while",
    "do", "switch", "case", "default", "break", "continue", "new", "delete",
    "typeof", "instanceof", "in", "of", "class", "extends", "super", "this",
    "import", "export", "from", "as", "try", "catch", "finally", "throw",
    "async", "await", "yield", "static", "get", "set", "null", "undefined",
    "true", "false", "void",
  ];
  const JS_BUILTINS = [
    "console", "document", "window", "fetch", "Promise", "Array", "Object",
    "JSON", "Math", "Date", "Map", "Set", "Error", "localStorage", "setTimeout",
    "setInterval", "clearTimeout", "clearInterval", "AbortController", "URL",
    "URLSearchParams", "FormData", "requestAnimationFrame", "navigator",
    "alert", "confirm", "Number", "String", "Boolean", "Symbol", "RegExp",
    "parseInt", "parseFloat", "isNaN", "encodeURIComponent", "structuredClone",
  ];
  const CSHARP_KEYWORDS = [
    "using", "namespace", "public", "private", "protected", "internal", "class",
    "interface", "record", "struct", "enum", "void", "int", "string", "bool",
    "double", "float", "decimal", "long", "var", "new", "return", "if", "else",
    "for", "foreach", "while", "do", "switch", "case", "default", "break",
    "continue", "try", "catch", "finally", "throw", "async", "await", "static",
    "readonly", "const", "this", "base", "null", "true", "false", "is", "as",
    "in", "out", "ref", "override", "virtual", "abstract", "sealed", "partial",
    "get", "set", "init", "required", "nameof", "typeof", "Task", "when",
  ];
  const SQL_KEYWORDS = [
    "SELECT", "FROM", "WHERE", "INSERT", "INTO", "VALUES", "UPDATE", "SET",
    "DELETE", "CREATE", "TABLE", "ALTER", "DROP", "PRIMARY", "KEY", "FOREIGN",
    "REFERENCES", "NOT", "NULL", "DEFAULT", "JOIN", "LEFT", "RIGHT", "INNER",
    "OUTER", "ON", "GROUP", "BY", "ORDER", "LIMIT", "OFFSET", "AS", "AND",
    "OR", "IN", "EXISTS", "COUNT", "SUM", "AVG", "MAX", "MIN", "SERIAL",
    "VARCHAR", "INTEGER", "TEXT", "BOOLEAN", "TIMESTAMP", "INDEX", "UNIQUE",
    "select", "from", "where", "insert", "into", "values", "update", "set",
    "delete", "create", "table", "join", "on", "group", "by", "order", "as",
    "and", "or", "limit",
  ];

  // Patrones de comentarios y strings reutilizables (sobre texto escapado)
  const STRING_PATTERNS = [
    // Template literals con backticks (pueden ocupar varias líneas)
    { regex: /`[^`]*`/gs, type: "string" },
    // Strings dobles y simples (sin salto de línea)
    { regex: /&quot;.*?&quot;|"[^"\n]*"|'[^'\n]*'/g, type: "string" },
  ];
  const JS_COMMENTS = [
    { regex: /\/\*[\s\S]*?\*\//g, type: "comment" },
    { regex: /\/\/[^\n]*/g, type: "comment" },
  ];

  const languages = {
    /* ----- JavaScript / JSX ----- */
    js(code) {
      const { code: rest, restore } = languages._extractJs(code);
      let html = rest;
      // keywords y builtins ANTES del tag regex: evita que "class" dentro de
      // <span class="tok-tag"> sea capturada como keyword y rompa el HTML.
      html = html.replace(kw(JS_KEYWORDS), (_, w) => span("keyword", w));
      html = html.replace(kw(JS_BUILTINS), (_, w) => span("built-in", w));
      html = html.replace(
        /(&lt;\/?)([A-Za-z][A-Za-z0-9._]*)/g,
        (_, open, name) => open + span("tag", name)
      );
      html = html.replace(
        /\b([A-Z][A-Za-z0-9_]*)\b/g,
        (_, w) => span("tag", w) // Componentes y clases: PascalCase
      );
      html = html.replace(
        /\b([a-zA-Z_$][\w$]*)(?=\s*\()/g,
        (_, w) => span("function", w) // llamadas a función
      );
      html = html.replace(/\b(\d[\d_]*(?:\.\d+)?)\b/g, (_, w) => span("number", w));
      return restore(html);
    },

    _extractJs(code) {
      return extract(code, [...JS_COMMENTS, ...STRING_PATTERNS]);
    },

    /* ----- HTML / XML ----- */
    html(code) {
      const { code: rest, restore } = extract(code, [
        { regex: /&lt;!--[\s\S]*?--&gt;/g, type: "comment" },
        { regex: /&quot;.*?&quot;|"[^"\n]*"|'[^'\n]*'/g, type: "string" },
      ]);
      let html = rest;
      // atributos y puntuación ANTES del tag regex: mismo motivo que en js(),
      // evita que "class" dentro de los <span> generados coincida como attr.
      html = html.replace(
        /([a-zA-Z-]+)(?==&quot;|=")/g,
        (_, name) => span("attr", name)
      );
      html = html.replace(/(\/?&gt;)/g, span("punctuation", "$1"));
      html = html.replace(
        /(&lt;\/?)([a-zA-Z][\w-]*)/g,
        (_, open, name) => open + span("tag", name)
      );
      return restore(html);
    },

    /* ----- CSS ----- */
    css(code) {
      const { code: rest, restore } = extract(code, [
        { regex: /\/\*[\s\S]*?\*\//g, type: "comment" },
        { regex: /&quot;.*?&quot;|"[^"\n]*"|'[^'\n]*'/g, type: "string" },
      ]);
      let html = rest;
      // Propiedades: palabra seguida de ":" dentro de un bloque
      html = html.replace(
        /([a-z-]+)(\s*:)(?![^<{]*\{)/g,
        (_, prop, colon) => span("attr", prop) + colon
      );
      // Selectores: lo que precede a "{"
      html = html.replace(
        /^([^{}\n]+)(?=\s*\{)/gm,
        (_, sel) => span("tag", sel)
      );
      html = html.replace(
        /\b(\d+(?:\.\d+)?)(px|rem|em|%|vh|vw|s|ms|fr|deg)?\b/g,
        (m, num, unit) => span("number", num) + (unit ? span("keyword", unit) : "")
      );
      return restore(html);
    },

    /* ----- Bash / terminal ----- */
    bash(code) {
      const { code: rest, restore } = extract(code, [
        { regex: /#[^\n]*/g, type: "comment" },
        ...STRING_PATTERNS,
      ]);
      let html = rest;
      html = html.replace(
        /^(\s*)(pnpm|npm|node|npx|git|cd|mkdir|docker|dotnet|code|corepack|curl|pnpm-?dlx|serve|New-Item|Get-ChildItem|Remove-Item|Set-Location|cp|mv|ls|echo)\b/gm,
        (_, space, cmd) => space + span("keyword", cmd)
      );
      html = html.replace(/(\s)(-{1,2}[\w-]+)/g, (_, s, flag) => s + span("attr", flag));
      return restore(html);
    },

    /* ----- JSON ----- */
    json(code) {
      let html = code;
      html = html.replace(
        /("(?:[^"\\]|\\.)*")(\s*:)?/g,
        (m, str, colon) => (colon ? span("attr", str) + colon : span("string", str))
      );
      html = html.replace(/\b(-?\d+(?:\.\d+)?)\b/g, (_, n) => span("number", n));
      html = html.replace(/\b(true|false|null)\b/g, (_, b) => span("keyword", b));
      return html;
    },

    /* ----- C# ----- */
    csharp(code) {
      const { code: rest, restore } = extract(code, [...JS_COMMENTS, ...STRING_PATTERNS]);
      let html = rest;
      html = html.replace(kw(CSHARP_KEYWORDS), (_, w) => span("keyword", w));
      html = html.replace(/\b([A-Z][A-Za-z0-9_]*)\b/g, (_, w) => span("tag", w));
      html = html.replace(/\b([a-zA-Z_]\w*)(?=\s*\()/g, (_, w) => span("function", w));
      html = html.replace(/\b(\d[\d_]*(?:\.\d+)?)\b/g, (_, w) => span("number", w));
      return restore(html);
    },

    /* ----- SQL ----- */
    sql(code) {
      const { code: rest, restore } = extract(code, [
        { regex: /--[^\n]*/g, type: "comment" },
        { regex: /'[^'\n]*'/g, type: "string" },
      ]);
      let html = rest;
      html = html.replace(kw(SQL_KEYWORDS), (_, w) => span("keyword", w));
      html = html.replace(/\b(\d+(?:\.\d+)?)\b/g, (_, n) => span("number", n));
      return restore(html);
    },

    /* ----- Texto plano ----- */
    text(code) {
      return code;
    },
  };

  // Alias de lenguajes (class="language-js", "language-shell", ...)
  languages.jsx = languages.js;
  languages.javascript = languages.js;
  languages.xml = languages.html;
  languages.shell = languages.bash;
  languages.sh = languages.bash;
  languages.powershell = languages.bash;
  languages.cs = languages.csharp;
  languages.plain = languages.text;
  languages.none = languages.text;

  /* ---------- punto de entrada público ---------- */

  window.highlightAll = function highlightAll(root = document) {
    root.querySelectorAll('pre code[class*="language-"]').forEach((block) => {
      const lang = (block.className.match(/language-(\w+)/) || [])[1] || "text";
      const highlighter = languages[lang] || languages.text;
      // textContent decodifica las entidades (&lt; → <), así que re-escapamos
      const raw = block.textContent;
      block.innerHTML = highlighter(escapeHtml(raw));
    });
  };
})();
