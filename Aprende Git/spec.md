# Curso Completo de Git

## Del modelo interno a recuperarte de cualquier lío

---

# 1. Objetivo

Actúa como un desarrollador senior que lleva años usando Git en equipo y ha tenido que rescatar más de un repositorio. El objetivo es crear **un curso completo de Git**, pensado para quien **ya sabe programar** y usa Git "de memoria" (`add`, `commit`, `push`) pero no entiende el modelo, no sabe deshacer con criterio y tiene miedo a las ramas, al rebase y a la historia compartida.

El curso enseña el **por qué**:

- Git es una base de datos de objetos direccionada por contenido, no una carpeta de copias.
- Cada comando mueve **una** de tres cosas: el directorio de trabajo, el índice o `HEAD`.
- Deshacer tiene varias herramientas porque cada una responde a una pregunta distinta.
- Reescribir historia es seguro **solo** en commits que nadie más ha basado su trabajo.

El hilo conductor es un repositorio de práctica llamado **taller**: notas, conflictos, un bug escondido y una historia que hay que dejar presentable antes de publicarla.

> Comandos canónicos: los modernos (`git switch`, `git restore`) desde Git 2.23. Se menciona `git checkout` como forma antigua, no como la recomendada.

---

# 2. Audiencia y requisitos previos

- **Saber programar** y usar una terminal.
- No se asume experiencia previa seria con Git: `commit` y `push` se explican desde el modelo.
- Git **2.23 o superior** (para `switch` y `restore`). En Windows, Git for Windows (Git Bash).

---

# 3. Hilo conductor: el taller

Un repositorio local que crece con el curso:

1. Primeros commits de un fichero `notas.md`.
2. Un `.gitignore` y finales de línea coherentes.
3. Una rama `feature/busqueda` que choca con `main`.
4. Un bug introducido a propósito, encontrado con `bisect`.
5. Un commit "perdido" que se recupera con `reflog`.
6. Un remoto local (bare) para practicar `fetch`/`push` sin cuenta.
7. El **proyecto final**: auditar, limpiar y etiquetar una release.

El laboratorio ejecutable vive en `proyectos/laboratorio/` y recrea esos escenarios con `node`.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- Git 2.23+ (`switch`, `restore`, `switch -c`).
- Shell canónico: **bash** (Git Bash en Windows). Callout cuando PowerShell cambie comillas o redirecciones.
- Conventional Commits en los ejemplos de mensajes.
- Diagramas ASCII del grafo (`main`, ramas, `HEAD`).

## No utilizar

- GUIs como única explicación (se puede mencionar que existen).
- `git checkout` para cambiar de rama o descartar ficheros, salvo para decir "esto es lo antiguo".
- `git push --force` a `main` como solución. Si hay que reescribir: `--force-with-lease` y solo en ramas propias.
- `git filter-repo` / BFG como receta operativa de borrar secretos ya publicados: se nombra el riesgo y se remite a la documentación oficial, sin procedimiento de reescritura masiva del remoto.

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE las 10 secciones del contrato Academy.

## Reglas de contenido

- Antes de un comando, di **qué puntero mueve** (trabajo, índice, `HEAD`, rama).
- Distingue "aún no publicado" de "ya está en un remoto compartido".
- Un ejemplo mínimo por concepto, con el resultado esperado (`git status`, `git log --oneline --graph`).
- El reto no se resuelve en el HTML.

---

# 6. Multiplataforma

- **Windows:** Git for Windows. Terminal de los ejemplos: Git Bash. Callout si un ejemplo usa `&&` o rutas.
- **macOS / Linux:** Git del sistema o del gestor de paquetes, siempre ≥ 2.23.
- No se asume cuenta de GitHub para seguir el curso. El remoto de práctica es un repositorio **bare local**.

---

# 7. Índice del curso

## Módulo 0 — Fundamentos
- **01-que-es-git.html** — Qué problema resuelve Git y qué no es.
- **02-instalacion-y-configuracion.html** — Instalar, `user.name`, `user.email`, `--show-origin`.
- **03-el-modelo-interno.html** — Blob, tree, commit, hash; `git cat-file`.
- **04-tres-estados.html** — Directorio de trabajo, índice y `HEAD`.

## Módulo 1 — Commits
- **01-primer-repositorio.html** — `init`, `add`, `commit`, `log`, `show`.
- **02-diff-y-el-indice.html** — `diff`, `diff --staged`, `add -p`.
- **03-commits-atomicos.html** — Mensajes, Conventional Commits, `commit --amend` local.
- **04-gitignore-y-gitattributes.html** — Ignorar, `check-ignore`, finales de línea LF.

## Módulo 2 — Deshacer
- **01-restore.html** — Descartar en el trabajo y sacar del índice.
- **02-reset.html** — `--soft`, `--mixed`, `--hard` y cuándo no usar `--hard`.
- **03-revert.html** — Deshacer un commit ya publicado sin reescribir.
- **04-stash.html** — Guardar trabajo a medias, aplicar y conflictos del stash.

## Módulo 3 — Ramas
- **01-ramas-y-head.html** — La rama es un puntero; `switch`, detached `HEAD`.
- **02-merge.html** — Fast-forward y merge commit; historia lineal vs ramificada.
- **03-conflictos.html** — Marcadores, `status`, abortar, resolver.
- **04-flujos-de-equipo.html** — GitHub Flow, Git Flow y trunk-based: cuándo cada uno.

## Módulo 4 — Historia
- **01-rebase.html** — Rejugar commits; rebase vs merge.
- **02-rebase-interactivo.html** — `squash`, `fixup`, `reword`, reordenar.
- **03-cherry-pick.html** — Traer un commit concreto y su coste.
- **04-reescribir-con-cuidado.html** — Regla de oro, `--force-with-lease`, `reflog` como red.

## Módulo 5 — Remotos
- **01-clone-fetch-pull.html** — `origin`, `fetch` vs `pull`, ramas remotas.
- **02-push-y-tracking.html** — `push -u`, rechazo por divergencia, tags anotados.
- **03-pull-requests.html** — Revisar por diff, no por "ya funciona en mi máquina".
- **04-hooks.html** — `commit-msg` local para exigir Conventional Commits.

## Módulo 6 — Investigar
- **01-log-y-busqueda.html** — `log --graph`, `-S`, `--follow`.
- **02-blame-y-bisect.html** — Quién tocó una línea; cazar una regresión.
- **03-reflog-y-recuperar.html** — Commits "perdidos" y `reset --hard` reversible en local.
- **04-worktree-y-config.html** — Dos checkouts, alias e `includeIf`.

## Módulo 7 — Proyecto final
- **01-auditoria.html** — Diagnosticar un repo sucio. *(project)*
- **02-limpiar-historia.html** — Dejar la rama presentable sin reescribir lo publicado. *(project)*
- **03-flujo-de-equipo.html** — Convención de ramas, hook y protección de `main`. *(project)*
- **04-releases.html** — SemVer, tag anotado y changelog. *(project)*

## Recursos
- **cheatsheet.html** — Comandos por intención.
- **glosario.html** — Objeto, índice, `HEAD`, upstream, fast-forward…
- **faq.html** — FAQ del curso.

**Total: 8 módulos (0-7), 32 lecciones + 3 recursos.**

---

# 8. Estructura

```text
Aprende Git/
├── spec.md
├── AGENTS.md
├── package.json / pnpm-workspace.yaml
├── .gitignore / .gitattributes
├── web/
│   ├── index.html
│   ├── assets/
│   ├── modulos/00-fundamentos/ … 07-proyecto-final/
│   ├── recursos/
│   └── descargas/Aprende-Git.zip
├── scripts/serve-web.mjs
└── proyectos/laboratorio/     ← escenarios que se crean y se comprueban con node --test
```

---

# 9. Formato de entrega

Web estática sin build. Claves: `git-theme`, `git-progress`. `SITE_BASE` según el contrato.

Bloques de comandos: `language-bash`. Salidas de Git: `language-text`. Grafos ASCII en `language-text`.

---

# 10. Diseño y marca

- Paleta propia: **naranja Git** (`#F05032`) con acento ámbar de tag (`#e3b341`) sobre fondo oscuro.
- Firma de portada: un `git log --oneline` que se escribe solo.
- Logo del topbar: `GT`.

---

# 11. Convenciones

1. Contenido en español; comandos e identificadores en inglés.
2. Todo comando se explica por el puntero que mueve.
3. No se reescribe historia publicada.
4. Proyecto conductor: `proyectos/laboratorio/`.
5. 10 secciones siempre. El reto, sin resolver.

---

# 12. Documentación

Comandos contrastados con la referencia de Git (`git help <comando>`) para Git 2.23+. No inventar flags.

---

# 13. Portada

Hero, firma (log que se escribe), estadísticas, `#module-list` y botón de descarga del laboratorio.

---

# 14. Checklist

## Fase 0 — Spec y AGENTS
- [x] `spec.md`, `AGENTS.md`.

## Fase 1 — Scaffold
- [ ] `package.json`, workspace, `.gitignore`, `.gitattributes`, `serve-web.mjs`.
- [ ] Assets re-tematizados y `course-data.js`.
- [ ] Portada.

## Fase 2 — Lecciones
- [ ] 00 … 07 (32 lecciones).

## Fase 3 — Recursos
- [ ] cheatsheet, glosario, faq.

## Fase 4 — Laboratorio
- [ ] `proyectos/laboratorio/` con escenarios y tests.

## Fase 5 — Academy
- [ ] Tarjeta, contadores, filas en `specs.md` y `README.md`, ZIP.

---

# 15. Reglas

1. No des por sabido el índice ni `HEAD`.
2. Explica el por qué de cada deshacer.
3. Un ejemplo con salida esperada.
4. Piensa en el compañero que ya hizo `fetch`.
5. La solución más simple que no reescribe lo compartido.
6. Enlaza la lección previa cuando el concepto dependa de ella.
7. No conviertas el curso en un manual de GitHub: el remoto es Git; la forja es un ejemplo.
8. No des un procedimiento para purgar secretos de un remoto ya publicado.
9. Prioriza el modelo frente a memorizar flags.
10. El laboratorio debe poder ejecutarse sin red.
