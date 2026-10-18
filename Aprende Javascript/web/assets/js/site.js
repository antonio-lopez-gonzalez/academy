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

  const STORAGE_KEYS = { theme: "js-theme", progress: "js-progress" };

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
        <svg class="topbar__logo" viewBox="0 0 24 24" fill="#F7DF1E" aria-hidden="true"><path d="M0 0h24v24H0V0zm22.034 18.276c-.175-1.095-.888-2.015-3.003-2.873-.736-.345-1.554-.585-1.797-1.14-.091-.33-.105-.51-.046-.705.15-.646.915-.84 1.515-.66.39.12.75.42.976.9 1.034-.676 1.034-.676 1.755-1.125-.27-.42-.404-.601-.586-.78-.63-.705-1.469-1.065-2.834-1.034l-.705.089c-.676.165-1.32.525-1.71 1.005-1.14 1.291-.811 3.541.569 4.471 1.365 1.02 3.361 1.244 3.616 2.205.24 1.17-.87 1.545-1.966 1.41-.811-.18-1.26-.586-1.755-1.336l-1.83 1.051c.21.48.45.689.81 1.109 1.74 1.756 6.09 1.666 6.871-1.004.029-.09.24-.705.074-1.65l.046.067zm-8.983-7.245h-2.248c0 1.938-.009 3.864-.009 5.805 0 1.232.063 2.363-.138 2.711-.33.689-1.18.601-1.566.48-.396-.196-.597-.466-.83-.855-.063-.105-.11-.196-.127-.196l-1.825 1.125c.305.63.75 1.172 1.324 1.517.855.51 2.004.675 3.207.405.783-.226 1.458-.691 1.811-1.411.51-.93.402-2.07.397-3.346.012-2.054 0-4.109 0-6.179l.004-.056z"/></svg>
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
        <div class="stat"><div class="stat__num">25+</div><div class="stat__label">proyectos ejecutables</div></div>`;
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
     PORTADA: la consola REPL (elemento firma de la portada)
     El curso se presenta a sí mismo en su propio lenguaje: una sesión
     de consola que se "escribe" sola, respetando prefers-reduced-motion.
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
        text: "const curso = { es: true, frameworks: 0, nivel: 'profesional' }",
        html:
          '<span class="t-kw">const</span> curso = <span class="t-pun">{</span> ' +
          "es: <span class=\"t-kw\">true</span>, " +
          "frameworks: <span class=\"t-num\">0</span>, " +
          "nivel: <span class=\"t-str\">'profesional'</span> " +
          '<span class="t-pun">}</span>',
      },
      {
        text: "await curso.desdeCero()",
        html:
          '<span class="t-kw">await</span> <span class="t-bt">curso</span>.' +
          '<span class="t-fn">desdeCero</span>()',
      },
    ];
    const outHtml =
      '<span class="repl__line repl__out">' +
      `✓ ${COURSE.modules.length} módulos · ${total} lecciones · ≈${hours} horas · sin frameworks` +
      "</span>" +
      '<span class="repl__line"><span class="repl__prompt">&gt;</span><span class="repl__caret"></span></span>';

    // Sin movimiento: muestra toda la sesión de una vez
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
      el.innerHTML =
        typedLines.map((l) => '<span class="repl__line"><span class="repl__prompt">&gt;</span> ' + l.html + "</span>").join("") +
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
        lineEl.innerHTML =
          '<span class="repl__prompt">&gt;</span> ' + line.html;
        lineEl.appendChild(caret());
        li++;
        ci = 0;
        window.setTimeout(typeStep, 460);
        return;
      }
      // Siguiente carácter
      lineEl.textContent = "";
      const prompt = document.createElement("span");
      prompt.className = "repl__prompt";
      prompt.textContent = ">";
      lineEl.appendChild(prompt);
      lineEl.appendChild(document.createTextNode(" "));
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
