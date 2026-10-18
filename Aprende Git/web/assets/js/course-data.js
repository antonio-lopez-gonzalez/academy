/**
 * course-data.js — MAPA COMPLETO DEL CURSO GIT
 *
 * Única fuente de verdad para:
 *  - el sidebar (árbol de módulos y lecciones)
 *  - la navegación anterior / siguiente
 *  - el índice de la portada
 *  - el cálculo de progreso
 *
 * Cada lección apunta a un archivo HTML real dentro de web/modulos/.
 * Si añades o renombras una lección, edita SOLO este archivo.
 */
window.COURSE = {
  title: "Aprende Git",
  subtitle: "Del modelo interno a recuperarte de cualquier lío",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de Git" },
    { file: "recursos/glosario.html", title: "Glosario de términos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MÓDULO 0 */
    {
      id: "00",
      dir: "modulos/00-fundamentos",
      title: "Fundamentos",
      description: "Qué problema resuelve Git, cómo configurarlo, su modelo de objetos y los tres estados.",
      lessons: [
        { file: "01-que-es-git.html", title: "Qué es Git", minutes: 30 },
        { file: "02-instalacion-y-configuracion.html", title: "Instalación y configuración", minutes: 35 },
        { file: "03-el-modelo-interno.html", title: "El modelo interno", minutes: 35 },
        { file: "04-tres-estados.html", title: "Los tres estados", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 1 */
    {
      id: "01",
      dir: "modulos/01-commits",
      title: "Commits",
      description: "Crear un repositorio, leer diffs, escribir commits atómicos e ignorar lo que no debe entrar.",
      lessons: [
        { file: "01-primer-repositorio.html", title: "El primer repositorio", minutes: 30 },
        { file: "02-diff-y-el-indice.html", title: "diff y el índice", minutes: 35 },
        { file: "03-commits-atomicos.html", title: "Commits atómicos", minutes: 35 },
        { file: "04-gitignore-y-gitattributes.html", title: ".gitignore y .gitattributes", minutes: 30 },
      ],
    },
    /* ============================================================ MÓDULO 2 */
    {
      id: "02",
      dir: "modulos/02-deshacer",
      title: "Deshacer",
      description: "restore, reset, revert y stash: cuatro formas de volver atrás con criterio.",
      lessons: [
        { file: "01-restore.html", title: "restore: descartar cambios", minutes: 30 },
        { file: "02-reset.html", title: "reset: mover HEAD", minutes: 35 },
        { file: "03-revert.html", title: "revert: deshacer lo publicado", minutes: 30 },
        { file: "04-stash.html", title: "stash: trabajo a medias", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 3 */
    {
      id: "03",
      dir: "modulos/03-ramas",
      title: "Ramas",
      description: "Ramas y HEAD, merge, resolución de conflictos y flujos de trabajo en equipo.",
      lessons: [
        { file: "01-ramas-y-head.html", title: "Ramas y HEAD", minutes: 30 },
        { file: "02-merge.html", title: "merge: unir ramas", minutes: 35 },
        { file: "03-conflictos.html", title: "Resolver conflictos", minutes: 40 },
        { file: "04-flujos-de-equipo.html", title: "Flujos de equipo", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 4 */
    {
      id: "04",
      dir: "modulos/04-historia",
      title: "Historia",
      description: "Rebase, rebase interactivo, cherry-pick y cuándo reescribir sin romper a nadie.",
      lessons: [
        { file: "01-rebase.html", title: "rebase", minutes: 35 },
        { file: "02-rebase-interactivo.html", title: "rebase interactivo", minutes: 40 },
        { file: "03-cherry-pick.html", title: "cherry-pick", minutes: 35 },
        { file: "04-reescribir-con-cuidado.html", title: "Reescribir con cuidado", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 5 */
    {
      id: "05",
      dir: "modulos/05-remotos",
      title: "Remotos",
      description: "Clone, fetch/pull, push, tags, pull requests y hooks de validación.",
      lessons: [
        { file: "01-clone-fetch-pull.html", title: "clone, fetch y pull", minutes: 35 },
        { file: "02-push-y-tracking.html", title: "push y ramas de seguimiento", minutes: 35 },
        { file: "03-pull-requests.html", title: "Pull requests", minutes: 35 },
        { file: "04-hooks.html", title: "Hooks", minutes: 40 },
      ],
    },
    /* ============================================================ MÓDULO 6 */
    {
      id: "06",
      dir: "modulos/06-investigar",
      title: "Investigar",
      description: "log, blame, bisect, reflog y worktree: depurar la historia como un forense.",
      lessons: [
        { file: "01-log-y-busqueda.html", title: "log y búsqueda", minutes: 35 },
        { file: "02-blame-y-bisect.html", title: "blame y bisect", minutes: 40 },
        { file: "03-reflog-y-recuperar.html", title: "reflog: recuperar", minutes: 35 },
        { file: "04-worktree-y-config.html", title: "worktree y config", minutes: 35 },
      ],
    },
    /* ============================================================ MÓDULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final",
      description: "Auditar, limpiar, etiquetar y documentar un repositorio sucio.",
      lessons: [
        { file: "01-auditoria.html", title: "Auditoría de un repositorio", minutes: 40, project: true },
        { file: "02-limpiar-historia.html", title: "Limpiar la historia", minutes: 45, project: true },
        { file: "03-flujo-de-equipo.html", title: "Flujo de equipo", minutes: 40, project: true },
        { file: "04-releases.html", title: "Releases y changelog", minutes: 40, project: true },
      ],
    },
  ],
};
