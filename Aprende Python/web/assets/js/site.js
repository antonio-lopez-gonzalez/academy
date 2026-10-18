/**
 * site.js — Motor de la interfaz del curso.
 *
 * Cada página del curso es un HTML completo y legible por sí mismo.
 * Este script añade el "chrome" común:
 *   topbar (buscador, tema, progreso) · sidebar (árbol del curso)
 *   navegación anterior/siguiente · botón "marcar como completada"
 *
 * Contrato con las páginas:
 *   <html data-module="04" data-lesson="01-usestate">   → lección
 *   <body data-page="home">                             → portada
 *   <html data-lesson="recursos/glosario">              → recurso
 *   window.SITE_BASE = './' | '../' | '../../'          → ruta a la raíz de web/
 *
 * El progreso se guarda en localStorage (sin servidor, sin cuentas).
 */
(function () {
  "use strict";

  const COURSE = window.COURSE;
  const BASE = window.SITE_BASE || "./";
  const html = document.documentElement;

  const STORAGE_KEYS = { theme: "py-theme", progress: "py-progress" };

  /* ==================================================================
     TEMA (claro/oscuro). La preferencia inicial la aplica un script
     inline en el <head> de cada página para evitar parpadeos.
     ================================================================== */
  function toggleTheme() {
    const next = html.dataset.theme === "light" ? "dark" : "light";
    html.dataset.theme = next;
    localStorage.setItem(STORAGE_KEYS.theme, next);
    updateThemeIcon();
  }
  function updateThemeIcon() {
    const btn = document.querySelector("[data-theme-toggle]");
    if (btn) btn.textContent = html.dataset.theme === "light" ? "☀" : "☾";
  }

  /* ==================================================================
     PROGRESO (lecciones completadas)
     ================================================================== */
  function getProgress() {
    try {
      return JSON.parse(localStorage.getItem(STORAGE_KEYS.progress)) || {};
    } catch {
      return {};
    }
  }
  function saveProgress(progress) {
    localStorage.setItem(STORAGE_KEYS.progress, JSON.stringify(progress));
  }
  function lessonPath(mod, lesson) {
    return mod.dir + "/" + lesson.file;
  }
  function isDone(path) {
    return !!getProgress()[path];
  }
  function toggleDone(path) {
    const progress = getProgress();
    if (progress[path]) delete progress[path];
    else progress[path] = true;
    saveProgress(progress);
  }
  function totalLessons() {
    return COURSE.modules.reduce((n, m) => n + m.lessons.length, 0);
  }
  function doneLessons() {
    return Object.keys(getProgress()).length;
  }

  // Lista plana de lecciones para calcular anterior/siguiente
  function flatLessons() {
    const flat = [];
    COURSE.modules.forEach((mod) =>
      mod.lessons.forEach((lesson) => flat.push({ mod, lesson }))
    );
    return flat;
  }

  /* ==================================================================
     TOPBAR
     ================================================================== */
  function buildTopbar() {
    const topbar = document.createElement("header");
    topbar.className = "topbar";
    topbar.innerHTML = `
      <a class="topbar__btn" href="${BASE}../../index.html" title="Volver a la biblioteca" aria-label="Volver a la biblioteca"><svg viewBox="0 0 24 24" width="15" height="15" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M3 10.5 12 3l9 7.5"/><path d="M5 9.5V21h14V9.5"/><path d="M9 21v-6h6v6"/></svg></a>
      <button class="topbar__btn hamburger" aria-label="Abrir índice" data-hamburger>☰</button>
      <a class="topbar__brand" href="${BASE}index.html">
        <svg class="topbar__logo" viewBox="0 0 24 24" fill="#4B8BBE" aria-hidden="true"><path d="M14.25.18l.9.2.73.26.59.3.45.32.34.34.25.34.16.33.1.3.04.26.02.2-.01.13V8.5l-.05.63-.13.55-.21.46-.26.38-.3.31-.33.25-.35.19-.35.14-.33.1-.3.07-.26.04-.21.02H8.77l-.69.05-.59.14-.5.22-.41.27-.33.32-.27.35-.2.36-.15.37-.1.35-.07.32-.04.27-.02.21v3.06H3.17l-.21-.03-.28-.07-.32-.12-.35-.18-.36-.26-.36-.36-.35-.46-.32-.59-.28-.73-.21-.88-.14-1.05-.05-1.23.06-1.22.16-1.04.24-.87.32-.71.36-.57.4-.44.42-.33.42-.24.4-.16.36-.1.32-.05.24-.01h.16l.06.01h8.16v-.83H6.18l-.01-2.75-.02-.37.05-.34.11-.31.17-.28.25-.26.31-.23.38-.2.44-.18.51-.15.58-.12.64-.1.71-.06.77-.04.84-.02 1.27.05zm-6.3 1.98l-.23.33-.08.41.08.41.23.34.33.22.41.09.41-.09.33-.22.23-.34.08-.41-.08-.41-.23-.33-.33-.22-.41-.09-.41.09zm13.09 3.95l.28.06.32.12.35.18.36.27.36.35.35.47.32.59.28.73.21.88.14 1.04.05 1.23-.06 1.23-.16 1.04-.24.86-.32.71-.36.57-.4.45-.42.33-.42.24-.4.16-.36.09-.32.05-.24.02-.16-.01h-8.22v.82h5.84l.01 2.76.02.36-.05.34-.11.31-.17.29-.25.25-.31.24-.38.2-.44.17-.51.15-.58.13-.64.09-.71.07-.77.04-.84.01-1.27-.04-1.07-.14-.9-.2-.73-.25-.59-.3-.45-.33-.34-.34-.25-.34-.16-.33-.1-.3-.04-.25-.02-.2.01-.13v-5.34l.05-.64.13-.54.21-.46.26-.38.3-.32.33-.24.35-.2.35-.14.33-.1.3-.06.26-.04.21-.02.13-.01h5.84l.69-.05.59-.14.5-.21.41-.28.33-.32.27-.35.2-.36.15-.36.1-.35.07-.32.04-.28.02-.21V6.07h2.09l.14.01zm-6.47 14.25l-.23.33-.08.41.08.41.23.33.33.23.41.08.41-.08.33-.23.23-.33.08-.41-.08-.41-.23-.33-.33-.23-.41-.08-.41.08z"/></svg>
        <span>${COURSE.title}</span>
      </a>
      <div class="topbar__progress" title="Lecciones completadas">
        <div class="topbar__progress-bar"><div class="topbar__progress-fill" data-progress-fill></div></div>
        <span data-progress-label></span>
      </div>
      <button class="topbar__btn" data-theme-toggle title="Cambiar tema" aria-label="Cambiar tema"></button>
    `;
    document.body.prepend(topbar);

    topbar.querySelector("[data-theme-toggle]").addEventListener("click", toggleTheme);
    topbar.querySelector("[data-hamburger]").addEventListener("click", () => {
      document.body.classList.toggle("sidebar-open");
    });
    updateThemeIcon();
    renderProgress();
  }

  function renderProgress() {
    const done = doneLessons();
    const total = totalLessons();
    const pct = total ? Math.round((done / total) * 100) : 0;
    const fill = document.querySelector("[data-progress-fill]");
    const label = document.querySelector("[data-progress-label]");
    if (fill) fill.style.width = pct + "%";
    if (label) label.textContent = `${done}/${total} · ${pct}%`;
  }

  /* ==================================================================
     SIDEBAR
     ================================================================== */
  function buildSidebar(current) {
    const aside = document.createElement("aside");
    aside.className = "sidebar";

    COURSE.modules.forEach((mod) => {
      const isCurrentModule = current && current.mod.id === mod.id;
      const done = mod.lessons.filter((l) => isDone(lessonPath(mod, l))).length;

      // Estado de progreso del módulo: acabado (verde) o en curso (ámbar)
      let modState = "";
      if (done === mod.lessons.length) modState = " sidebar__module--done";
      else if (done > 0) modState = " sidebar__module--partial";

      const wrapper = document.createElement("div");
      wrapper.className =
        "sidebar__module" +
        (isCurrentModule ? " sidebar__module--open" : "") +
        modState;
      wrapper.dataset.searchBlock = "";

      const headerBtn = document.createElement("button");
      headerBtn.className = "sidebar__module-header";
      headerBtn.innerHTML = `
        <span class="sidebar__module-num">${mod.id}</span>
        <span>${mod.title}</span>
        <span class="sidebar__chevron">▶</span>`;
      headerBtn.title = `${mod.description} (${done}/${mod.lessons.length})`;
      headerBtn.addEventListener("click", () =>
        wrapper.classList.toggle("sidebar__module--open")
      );
      wrapper.appendChild(headerBtn);

      const list = document.createElement("ul");
      list.className = "sidebar__lessons";
      mod.lessons.forEach((lesson) => {
        const path = lessonPath(mod, lesson);
        const li = document.createElement("li");
        li.dataset.searchItem = lesson.title.toLowerCase();

        const link = document.createElement("a");
        link.href = BASE + path;
        link.className =
          "sidebar__lesson" +
          (current && current.lesson.file === lesson.file && isCurrentModule
            ? " sidebar__lesson--active"
            : "") +
          (lesson.project ? " sidebar__lesson--project" : "");

        const check = document.createElement("span");
        check.className = "sidebar__check" + (isDone(path) ? " sidebar__check--done" : "");
        check.textContent = isDone(path) ? "✓" : "";
        check.title = "Marcar como completada";
        check.addEventListener("click", (e) => {
          e.preventDefault();
          toggleDone(path);
          buildSidebarRefresh();
        });

        const text = document.createElement("span");
        text.textContent = lesson.title;

        link.append(check, text);
        li.appendChild(link);
        list.appendChild(li);
      });
      wrapper.appendChild(list);
      aside.appendChild(wrapper);
    });

    // Bloque de recursos al final
    const res = document.createElement("div");
    res.className = "sidebar__module sidebar__module--open";
    res.innerHTML = `<button class="sidebar__module-header">
        <span class="sidebar__module-num">★</span><span>Recursos</span>
        <span class="sidebar__chevron">▶</span></button>`;
    const resHeader = res.querySelector(".sidebar__module-header");
    resHeader.addEventListener("click", () =>
      res.classList.toggle("sidebar__module--open")
    );
    const resList = document.createElement("ul");
    resList.className = "sidebar__lessons";
    COURSE.resources.forEach((r) => {
      const li = document.createElement("li");
      li.dataset.searchItem = (r.title + " " + r.file).toLowerCase();
      const a = document.createElement("a");
      a.className = "sidebar__lesson";
      a.href = BASE + r.file;
      a.title = r.title;
      const fileName = r.file.split("/").pop();
      a.innerHTML = `<span class="sidebar__check" style="border-style:dashed"></span><span>${fileName}</span>`;
      li.appendChild(a);
      resList.appendChild(li);
    });
    res.appendChild(resList);
    aside.appendChild(res);

    return aside;
  }

  // Reconstruye el sidebar (tras marcar completar) y refresca el progreso
  function buildSidebarRefresh() {
    const current = getCurrent();
    const old = document.querySelector(".sidebar");
    if (old) old.replaceWith(buildSidebar(current));
    renderProgress();
    syncCompleteButton();
  }

  /* ==================================================================
     BÚSQUEDA: filtra las lecciones del sidebar por título
     ================================================================== */
  function onSearch(event) {
    const query = event.target.value.trim().toLowerCase();
    const sidebar = document.querySelector(".sidebar");
    if (!sidebar) return;
    let anyVisible = false;

    sidebar.querySelectorAll("[data-search-item]").forEach((li) => {
      const match = !query || li.dataset.searchItem.includes(query);
      li.style.display = match ? "" : "none";
      if (match) anyVisible = true;
    });

    // Con búsqueda activa, abrimos todos los módulos para ver resultados
    sidebar.querySelectorAll(".sidebar__module").forEach((mod) => {
      if (query) mod.classList.add("sidebar__module--open");
      const visible = [...mod.querySelectorAll("[data-search-item]")].some(
        (li) => li.style.display !== "none"
      );
      mod.style.display = visible ? "" : "none";
    });

    let empty = sidebar.querySelector(".sidebar__empty");
    if (!anyVisible && query) {
      if (!empty) {
        empty = document.createElement("p");
        empty.className = "sidebar__empty";
        empty.textContent = "Sin resultados para esta búsqueda.";
        sidebar.appendChild(empty);
      }
    } else if (empty) {
      empty.remove();
    }
  }

  /* ==================================================================
     PÁGINA ACTUAL (a partir de los data-attributes del <html>)
     ================================================================== */
  function getCurrent() {
    const moduleId = html.dataset.module;
    const lessonFile = html.dataset.lesson;
    if (!moduleId || !lessonFile) return null;
    for (const mod of COURSE.modules) {
      if (mod.id !== moduleId) continue;
      for (const lesson of mod.lessons) {
        if (lesson.file === lessonFile + ".html") return { mod, lesson };
      }
    }
    return null;
  }

  /* ==================================================================
     LECCIONES: breadcrumb, botón completar, anterior/siguiente
     ================================================================== */
  function enhanceLesson(current) {
    const lessonEl = document.querySelector(".lesson");
    if (!lessonEl) return;

    // Breadcrumb arriba del todo
    const index = current.mod.lessons.indexOf(current.lesson);
    const crumbs = document.createElement("div");
    crumbs.className = "lesson-header__crumbs";
    crumbs.innerHTML = `
      <a href="${BASE}index.html">Inicio</a> <span>/</span>
      <span>Módulo ${current.mod.id} · ${current.mod.title}</span> <span>/</span>
      <span>Lección ${index + 1} de ${current.mod.lessons.length}</span>`;
    lessonEl.prepend(crumbs);

    // Botón "marcar como completada"
    const path = lessonPath(current.mod, current.lesson);
    const btn = document.createElement("button");
    btn.className = "complete-btn";
    btn.dataset.completeBtn = path;
    btn.addEventListener("click", () => {
      toggleDone(path);
      buildSidebarRefresh();
    });
    lessonEl.appendChild(btn);
    syncCompleteButton();

    // Navegación anterior / siguiente
    const flat = flatLessons();
    const pos = flat.findIndex(
      (x) => x.mod.id === current.mod.id && x.lesson.file === current.lesson.file
    );
    const prev = flat[pos - 1];
    const next = flat[pos + 1];
    const nav = document.createElement("nav");
    nav.className = "lesson-nav";
    nav.setAttribute("aria-label", "Navegación entre lecciones");
    if (prev) {
      nav.innerHTML += `
        <a href="${BASE}${lessonPath(prev.mod, prev.lesson)}">
          <span class="lesson-nav__dir">← Anterior · Módulo ${prev.mod.id}</span>
          ${prev.lesson.title}
        </a>`;
    }
    if (next) {
      nav.innerHTML += `
        <a class="lesson-nav__next" href="${BASE}${lessonPath(next.mod, next.lesson)}">
          <span class="lesson-nav__dir">Siguiente · Módulo ${next.mod.id} →</span>
          ${next.lesson.title}
        </a>`;
    }
    lessonEl.appendChild(nav);
  }

  function syncCompleteButton() {
    const btn = document.querySelector("[data-complete-btn]");
    if (!btn) return;
    const done = isDone(btn.dataset.completeBtn);
    btn.classList.toggle("complete-btn--done", done);
    btn.innerHTML = done
      ? "✓ Lección completada"
      : "Marcar como completada";
  }

  /* ==================================================================
     PORTADA: tarjetas de módulo + estadísticas + "continuar donde ibas"
     ================================================================== */
  function buildHome() {
    const listEl = document.querySelector("#module-list");
    const statsEl = document.querySelector("#home-stats");
    if (!listEl) return;

    const total = totalLessons();
    const minutes = COURSE.modules.reduce(
      (sum, m) => sum + m.lessons.reduce((s, l) => s + (l.minutes || 30), 0),
      0
    );

    if (statsEl) {
      statsEl.innerHTML = `
        <div class="stat"><div class="stat__num">${COURSE.modules.length}</div><div class="stat__label">módulos</div></div>
        <div class="stat"><div class="stat__num">${total}</div><div class="stat__label">lecciones</div></div>
        <div class="stat"><div class="stat__num">${Math.round(minutes / 60)}</div><div class="stat__label">horas de curso</div></div>
        <div class="stat"><div class="stat__num">1</div><div class="stat__label">proyecto final ejecutable</div></div>`;
    }

    const progress = getProgress();
    COURSE.modules.forEach((mod) => {
      const done = mod.lessons.filter((l) => progress[lessonPath(mod, l)]).length;
      const card = document.createElement("article");
      card.className = "module-card";
      card.innerHTML = `
        <div class="module-card__header">
          <div class="module-card__num">${mod.id}</div>
          <div class="module-card__title">
            <h2>Módulo ${mod.id} — ${mod.title}</h2>
            <p>${mod.description}</p>
          </div>
          <div class="module-card__progress">${done}/${mod.lessons.length}</div>
        </div>
        <ol class="module-card__lessons">
          ${mod.lessons
            .map((lesson) => {
              const path = lessonPath(mod, lesson);
              const tick = progress[path] ? '<span class="done">✓</span>' : "";
              return `<li>${tick}<a href="${BASE}${path}">${lesson.title}</a> <span style="color:var(--text-faint);font-size:.8em">· ${lesson.minutes} min</span></li>`;
            })
            .join("")}
        </ol>`;
      listEl.appendChild(card);
    });

    // Botón "continuar donde lo dejaste": primera lección sin completar
    const continueBtn = document.querySelector("[data-continue]");
    if (continueBtn) {
      const pending = flatLessons().find(
        (x) => !progress[lessonPath(x.mod, x.lesson)]
      );
      if (pending) {
        continueBtn.href = BASE + lessonPath(pending.mod, pending.lesson);
        continueBtn.textContent =
          doneLessons() === 0
            ? "▶ Empezar el curso"
            : `▶ Continuar: ${pending.lesson.title}`;
      } else {
        continueBtn.href = BASE + "index.html";
        continueBtn.textContent = "✓ ¡Curso completado! Repasar";
      }
    }
  }

  /* ==================================================================
     PORTADA: la REPL (elemento firma de la portada)
     El curso se presenta a sí mismo en su propio lenguaje: una sesión
     de REPL de Python que se "escribe" sola, respetando prefers-reduced-motion.
     ================================================================== */
  function buildRepl() {
    const el = document.querySelector("[data-repl]");
    if (!el) return;

    const total = totalLessons();
    const minutes = COURSE.modules.reduce(
      (sum, m) => sum + m.lessons.reduce((s, l) => s + (l.minutes || 30), 0),
      0
    );
    const hours = Math.round(minutes / 60);

    // Líneas que se teclean: texto plano + su versión coloreada final
    const typedLines = [
      {
        text: ">>> import curso",
        html:
          '<span class="repl__prompt">&gt;&gt;&gt;</span> ' +
          '<span class="t-kw">import</span> <span class="t-bt">curso</span>',
      },
      {
        text: ">>> curso.temario()",
        html:
          '<span class="repl__prompt">&gt;&gt;&gt;</span> ' +
          '<span class="t-bt">curso</span>.<span class="t-fn">temario</span>()',
      },
      {
        text: `✓ ${COURSE.modules.length} módulos · ${total} lecciones · Python 3.12+ · con proyecto real`,
        html:
          '<span class="t-kw">✓</span> ' +
          '<span class="t-num">' + COURSE.modules.length + '</span> módulos · ' +
          '<span class="t-num">' + total + '</span> lecciones · ' +
          '<span class="t-str">Python 3.12+</span> · con proyecto real',
      },
      {
        text: ">>> from biblioteca import cli",
        html:
          '<span class="repl__prompt">&gt;&gt;&gt;</span> ' +
          '<span class="t-kw">from</span> <span class="t-bt">biblioteca</span> ' +
          '<span class="t-kw">import</span> <span class="t-fn">cli</span>',
      },
      {
        text: ">>> cli.main(['list'])",
        html:
          '<span class="repl__prompt">&gt;&gt;&gt;</span> ' +
          '<span class="t-bt">cli</span>.<span class="t-fn">main</span>([<span class="t-str">\'list\'</span>])',
      },
      {
        text: "1984 — George Orwell (prestado)",
        html: '<span class="t-num">1984</span> — George Orwell <span class="t-kw">(prestado)</span>',
      },
      {
        text: "Dune — Frank Herbert",
        html: '<span class="t-num">Dune</span> — Frank Herbert',
      },
      {
        text: "Blade Runner — Ridley Scott",
        html: '<span class="t-num">Blade Runner</span> — Ridley Scott',
      },
    ];
    const outHtml =
      '<span class="repl__line repl__out">' +
      `✓ de un script suelto a un paquete instalable con tests` +
      "</span>" +
      '<span class="repl__line"><span class="repl__prompt">&gt;&gt;&gt;</span><span class="repl__caret"></span></span>';

    // Sin movimiento: muestra toda la sesión de una vez
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
      el.innerHTML =
        typedLines.map((l) => '<span class="repl__line">' + l.html + "</span>").join("") +
        outHtml;
      return;
    }

    let li = 0;
    let ci = 0;

    function caret() {
      const c = document.createElement("span");
      c.className = "repl__caret";
      return c;
    }

    function typeStep() {
      if (li >= typedLines.length) {
        el.innerHTML += outHtml;
        return;
      }
      const line = typedLines[li];
      let lineEl = el.children[li];
      if (!lineEl) {
        lineEl = document.createElement("span");
        lineEl.className = "repl__line";
        el.appendChild(lineEl);
      }
      ci++;
      if (ci > line.text.length) {
        // Línea completa: muestra la versión coloreada + el cursor
        lineEl.innerHTML = line.html;
        lineEl.appendChild(caret());
        li++;
        ci = 0;
        window.setTimeout(typeStep, 460);
        return;
      }
      // Siguiente carácter (REPL: el prompt lo arrastra el HTML final de la línea)
      lineEl.textContent = "";
      const text = document.createElement("span");
      text.textContent = line.text.slice(0, ci);
      lineEl.appendChild(text);
      lineEl.appendChild(caret());
      window.setTimeout(typeStep, 42);
    }

    window.setTimeout(typeStep, 500);
  }

  /* ==================================================================
     CÓDIGO: botón "copiar al portapapeles" sobre cada bloque <pre>
     ================================================================== */
  function copyText(text) {
    // navigator.clipboard solo existe en contextos seguros (https o localhost)
    if (navigator.clipboard && window.isSecureContext) {
      return navigator.clipboard.writeText(text);
    }
    // Fallback para abrir la web con file:// sin servidor
    const ta = document.createElement("textarea");
    ta.value = text;
    ta.setAttribute("readonly", "");
    ta.style.position = "fixed";
    ta.style.opacity = "0";
    document.body.appendChild(ta);
    ta.select();
    document.execCommand("copy");
    ta.remove();
    return Promise.resolve();
  }

  function addCopyButtons() {
    // Iconos SVG (trazo hereda de currentColor), igual estilo que el logo
    const iconCopy =
      '<svg viewBox="0 0 24 24" width="14" height="14" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">' +
      '<rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect>' +
      '<path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path></svg>';
    const iconCheck =
      '<svg viewBox="0 0 24 24" width="14" height="14" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">' +
      '<polyline points="20 6 9 17 4 12"></polyline></svg>';

    document.querySelectorAll('pre code[class*="language-"]').forEach((code) => {
      const pre = code.closest("pre");
      if (!pre || pre.querySelector(".copy-btn")) return;

      const btn = document.createElement("button");
      btn.type = "button";
      btn.className = "copy-btn";
      btn.innerHTML = iconCopy;
      btn.title = "Copiar al portapapeles";
      btn.setAttribute("aria-label", "Copiar este código al portapapeles");
      btn.addEventListener("click", () => {
        // textContent del <code> ya es el código fuente sin resaltado
        copyText(code.textContent).then(() => {
          btn.classList.add("copy-btn--done");
          btn.innerHTML = iconCheck;
          window.setTimeout(() => {
            btn.classList.remove("copy-btn--done");
            btn.innerHTML = iconCopy;
          }, 1600);
        });
      });
      pre.appendChild(btn);
    });
  }

  /* ==================================================================
     ARRANQUE
     ================================================================== */
  buildTopbar();

  const isHome = document.body.dataset.page === "home";
  const current = getCurrent();
  const main = document.querySelector("main");

  if (main) {
    main.classList.add("content");
    const page = document.createElement("div");
    page.className = "page";
    main.parentNode.insertBefore(page, main);
    page.appendChild(buildSidebar(current));
    page.appendChild(main);
  }

  if (isHome) buildHome();
  if (isHome) buildRepl();
  if (current) enhanceLesson(current);

  if (typeof window.highlightAll === "function") window.highlightAll();
  addCopyButtons();

  // Cierra el sidebar móvil al navegar
  document.querySelectorAll(".sidebar a").forEach((a) =>
    a.addEventListener("click", () => document.body.classList.remove("sidebar-open"))
  );
})();
