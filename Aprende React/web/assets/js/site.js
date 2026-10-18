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

  const STORAGE_KEYS = { theme: "rc19-theme", progress: "rc19-progress" };

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
    if (btn) btn.textContent = html.dataset.theme === "light" ? "🌙" : "☀️";
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
        <svg class="topbar__logo" width="22" height="22" viewBox="0 0 24 24" fill="#61DAFB" aria-hidden="true"><path d="M14.23 12.004a2.236 2.236 0 0 1-2.235 2.236 2.236 2.236 0 0 1-2.236-2.236 2.236 2.236 0 0 1 2.235-2.236 2.236 2.236 0 0 1 2.236 2.236zm2.648-10.69c-1.346 0-3.107.96-4.888 2.622-1.78-1.653-3.542-2.602-4.887-2.602-.41 0-.783.093-1.106.278-1.375.793-1.683 3.264-.973 6.365C1.98 8.917 0 10.42 0 12.004c0 1.59 1.99 3.097 5.043 4.03-.704 3.113-.39 5.588.988 6.38.32.187.69.275 1.102.275 1.345 0 3.107-.96 4.888-2.624 1.78 1.654 3.542 2.603 4.887 2.603.41 0 .783-.09 1.106-.275 1.374-.792 1.683-3.263.973-6.365C22.02 15.096 24 13.59 24 12.004c0-1.59-1.99-3.097-5.043-4.032.704-3.11.39-5.587-.988-6.38-.318-.184-.688-.277-1.092-.278zm-.005 1.09v.006c.225 0 .406.044.558.127.666.382.955 1.835.73 3.704-.054.46-.142.945-.25 1.44-.96-.236-2.006-.417-3.107-.534-.66-.905-1.345-1.727-2.035-2.447 1.592-1.48 3.087-2.292 4.105-2.295zm-9.77.02c1.012 0 2.514.808 4.11 2.28-.686.72-1.37 1.537-2.02 2.442-1.107.117-2.154.298-3.113.538-.112-.49-.195-.964-.254-1.42-.23-1.868.054-3.32.714-3.707.19-.09.4-.127.563-.132zm4.882 3.05c.455.468.91.992 1.36 1.564-.44-.02-.89-.034-1.345-.034-.46 0-.915.01-1.36.034.44-.572.895-1.096 1.345-1.565zM12 8.1c.74 0 1.477.034 2.202.093.406.582.802 1.203 1.183 1.86.372.64.71 1.29 1.018 1.946-.308.655-.646 1.31-1.013 1.95-.38.66-.773 1.288-1.18 1.87-.728.063-1.466.098-2.21.098-.74 0-1.477-.035-2.202-.093-.406-.582-.802-1.204-1.183-1.86-.372-.64-.71-1.29-1.018-1.946.303-.657.646-1.313 1.013-1.954.38-.66.773-1.286 1.18-1.868.728-.064 1.466-.098 2.21-.098zm-3.635.254c-.24.377-.48.763-.704 1.16-.225.39-.435.782-.635 1.174-.265-.656-.49-1.31-.676-1.947.64-.15 1.315-.283 2.015-.386zm7.26 0c.695.103 1.365.23 2.006.387-.18.632-.405 1.282-.66 1.933-.2-.39-.41-.783-.64-1.174-.225-.392-.465-.774-.705-1.146zm3.063.675c.484.15.944.317 1.375.498 1.732.74 2.852 1.708 2.852 2.476-.005.768-1.125 1.74-2.857 2.475-.42.18-.88.342-1.355.493-.28-.958-.646-1.956-1.1-2.98.45-1.017.81-2.01 1.085-2.964zm-13.395.004c.278.96.645 1.957 1.1 2.98-.45 1.017-.812 2.01-1.086 2.964-.484-.15-.944-.318-1.37-.5-1.732-.737-2.852-1.706-2.852-2.474 0-.768 1.12-1.742 2.852-2.476.42-.18.88-.342 1.356-.494zm11.678 4.28c.265.657.49 1.312.676 1.948-.64.157-1.316.29-2.016.39.24-.375.48-.762.705-1.158.225-.39.435-.788.636-1.18zm-9.945.02c.2.392.41.783.64 1.175.23.39.465.772.705 1.143-.695-.102-1.365-.23-2.006-.386.18-.63.406-1.282.66-1.933zM17.92 16.32c.112.493.2.968.254 1.423.23 1.868-.054 3.32-.714 3.708-.147.09-.338.128-.563.128-1.012 0-2.514-.807-4.11-2.28.686-.72 1.37-1.536 2.02-2.44 1.107-.118 2.154-.3 3.113-.54zm-11.83.01c.96.234 2.006.415 3.107.532.66.905 1.345 1.727 2.035 2.446-1.595 1.483-3.092 2.295-4.11 2.295-.22-.005-.406-.05-.553-.132-.666-.38-.955-1.834-.73-3.703.054-.46.142-.944.25-1.438zm4.56.64c.44.02.89.034 1.345.034.46 0 .915-.01 1.36-.034-.44.572-.895 1.095-1.345 1.565-.455-.47-.91-.993-1.36-1.565z"/></svg>
        <span>${COURSE.title}</span>
      </a>
      <div class="topbar__search">
        <input type="search" placeholder="Buscar lección…  (ej: useState)" data-search aria-label="Buscar lección">
      </div>
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
    topbar.querySelector("[data-search]").addEventListener("input", onSearch);
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

      const wrapper = document.createElement("div");
      wrapper.className =
        "sidebar__module" + (isCurrentModule ? " sidebar__module--open" : "");
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
    const resList = document.createElement("ul");
    resList.className = "sidebar__lessons";
    resList.style.display = "block";
    COURSE.resources.forEach((r) => {
      const li = document.createElement("li");
      li.dataset.searchItem = r.title.toLowerCase();
      const a = document.createElement("a");
      a.className = "sidebar__lesson";
      a.href = BASE + r.file;
      a.innerHTML = `<span class="sidebar__check" style="border-style:dashed"></span><span>${r.title}</span>`;
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
  if (current) enhanceLesson(current);

  if (typeof window.highlightAll === "function") window.highlightAll();

  // Cierra el sidebar móvil al navegar
  document.querySelectorAll(".sidebar a").forEach((a) =>
    a.addEventListener("click", () => document.body.classList.remove("sidebar-open"))
  );
})();
