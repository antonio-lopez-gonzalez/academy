# Curso Completo de IA para desarrollo: del LLM al agente en produccion

## Construye agentes y aplicaciones con LLMs — con un copiloto real, de la API al despliegue

---

# 1. Objetivo

Actua como un ingeniero senior de IA aplicada, con anos construyendo producto real con modelos de lenguaje: RAG, agentes, tools, evaluacion y coste. El objetivo es crear **un curso completo de IA orientado a programacion y agentes**, pensado para un **desarrollador que ya programa** (y probablemente ya usa un agente de codigo) pero que quiere entender y construir con LLMs de verdad: de "que es un token" a un **agente en produccion con RAG, tools, evaluacion y control de coste**.

No quiero un recetario de prompts ni un tutorial de un solo framework. Quiero entender:

- **como funciona un LLM** por dentro a nivel practico (tokens, contexto, sampling, coste y latencia);
- **que problema resuelve** cada pieza (embedding, chunking, retrieval, tool calling, MCP, evals);
- **como se disena y evalua** una app con IA que no se rompe en produccion (guardrails, trazas, regresiones);
- **como se construye un agente** que usa herramientas y memoria, y **como se le da extension** (skills, plugins, MCP).

El alumno termina el curso con **un copiloto funcional** (un asistente que responde sobre los cursos de la Academy usando RAG, con tools y con evals) y con criterio para llevar IA a cualquier producto.

---

# 2. Audiencia y requisitos previos

- **Saber programar** en Python (el curso usa Python 3.12+). No se asume experiencia previa con IA/LLMs.
- Ganas de tocar la terminal y Git. **No** se requiere tarjeta ni API key para el nucleo: el curso funciona con un **LLM local (Ollama)** por defecto y muestra como conectar proveedores en la nube (OpenAI/Anthropic) como alternativa.
- VSCode + Python + ruff/mypy.

---

# 3. Hilo conductor: Copiloto

**Copiloto** — un asistente para desarrolladores que responde preguntas sobre los cursos de esta Academy:

1. **Indexa** el contenido HTML de los cursos (`Academy/Learn */web/modulos/**`).
2. **Responde** con RAG + citas a la leccion de origen.
3. **Usa tools** (buscar lecciones, abrir un modulo, contar progreso).
4. **Se evalua** con un dataset de preguntas y un juez automatico.
5. Se expone por **CLI** y por una **API minima** con UI opcional.

Evolucion del aprendizaje:

- Modulos 0-2: llamadas al modelo, prompting y tool calling sobre el Copiloto (todavia sin RAG).
- Modulos 3-4: se anade **busqueda semantica** y **RAG** sobre las lecciones reales.
- Modulo 5: el Copiloto pasa a ser un **agente** con bucle, tools y memoria.
- Modulo 6: se le da **extension** con un **servidor MCP** y una **skill**.
- Modulos 7-8: **evaluacion, guardrails, coste** y proyecto final end-to-end.

---

# 4. Tecnologias

## Utilizar SIEMPRE

- **Python 3.12+** con **uv** (gestor) y `pyproject.toml`.
- **pydantic** para modelos y validacion de structured output.
- SDKs oficiales de proveedores: `openai`, `anthropic` (solo como alternativa).
- **Ollama** como LLM local por defecto (ejemplos sin API key) y embeddings locales.
- Embeddings con `fastembed` o `sentence-transformers`.
- **pgvector** (PostgreSQL) y `sqlite-vec` como bases vectoriales (una principal, otra comparativa).
- **MCP** (Model Context Protocol) con el SDK oficial de Python.
- `pytest` + `ruff` + `mypy` para calidad; `rich`/`typer` para la CLI.
- **OpenTelemetry** o trazas propias para observabilidad de LLM.

## No utilizar

- Un unico framework "mago" (LangChain/LlamaIndex) como unico camino: se ensena el patron y se mencionan como atajo.
- Un LLM concreto como requisito obligatorio (el nucleo corre en local).
- Notebooks como entregable principal: el codigo vive en `src/` ejecutable.
- Claves de API en el repositorio (siempre `.env` / variables de entorno).

---

# 5. Forma de ensenar

Cada leccion sigue EXACTAMENTE esta estructura.

## 1. Introduccion
Que vamos a aprender. Por que existe. Que problema resuelve. Cuando se usa. Errores tipicos.

## 2. Teoria
Con mucho detalle, sin asumir conocimientos previos. Comparaciones del mundo real (un LLM como un motor estadistico; el contexto como una mesa de trabajo; RAG como una biblioteca con indice). Diagramas ASCII cuando ayuden. Explicar el **por que** de cada decision.

## 3. Ejemplo minimo
El ejemplo mas pequeno posible. Explicarlo linea por linea. Que se ejecute.

## 4. Ejemplo real
El mismo concepto aplicado al **Copiloto** o a un escenario profesional. Explicar todas las decisiones.

## 5. Buenas practicas
Que hacer, que evitar, por que. Tabla cuando aplique.

## 6. Errores habituales
Ejemplos incorrectos, por que estan mal, y la version correcta.

## 7. Ejercicio guiado
Paso a paso, no demasiado dificil, resuelto al detalle, con resultado esperado.

## 8. Reto
Reto adicional **sin resolver** (solo pista).

## 9. Resumen
Bullets + conceptos clave.

## 10. Preguntas frecuentes
FAQ (3-5 `<details>`).

## Reglas de contenido

- Diferenciar siempre **inferencia (runtime del modelo)** de **logica determinista (tu codigo)**: la IA no es magia ni un oraculo.
- Mostrar el **coste y la latencia** cuando sea relevante; ensenar a medirlos.
- Ensenar **evaluacion** antes que "trucos": sin medir, no hay ingenieria.
- Provider-neutral en el discurso; concreto en el codigo (local por defecto).
- Nada de secretos ni claves en el repo.

---

# 6. Multiplataforma: Linux / macOS / Windows

Python + uv son multiplataforma. Shell canonico: **bash** (Linux/macOS/WSL2), con callouts de **PowerShell** cuando cambie la sintaxis (activacion de entorno, variables). Ollama funciona en las tres.

---

# 7. Indice del curso

## Modulo 0 — Preparacion
- **01-que-es-un-llm.html** — Que es (y que no es) un LLM. Prediccion de tokens, no "pensamiento".
- **02-tokens-contexto-sampling.html** — Tokens, ventana de contexto, temperatura, top-p, determinismo.
- **03-ecosistema-y-entorno.html** — Panorama (APIs, locales, embeddings, vector DBs). Entorno con uv y Ollama.
- **04-primera-llamada.html** — Primera llamada al modelo. `pyproject.toml`, `.env`, streaming minimo.

## Modulo 1 — Prompting de ingenieria
- **01-prompting-estructurado.html** — Sistema/usuario, plantillas, separadores, variables.
- **02-few-shot-y-roles.html** — Ejemplos few-shot, role prompting, chain-of-thought y sus limites.
- **03-structured-output.html** — Salida estructurada con pydantic/JSON Schema; validar y reintentar.
- **04-seguridad-de-prompt.html** — Inyeccion de prompt, delimitacion, desconfiar de la entrada.

## Modulo 2 — APIs y SDKs
- **01-llamadas-y-streaming.html** — Llamadas, parametros, streaming, cancelacion, timeouts.
- **02-tool-function-calling.html** — Tool/function calling: declarar tools y ejecutar su resultado.
- **03-multiproveedor-y-errores.html** — Local vs nube, reintentos, backoff, manejo de errores y rate limits.
- **04-coste-latencia-tokens.html** — Medir y controlar tokens, coste y latencia; cache.

## Modulo 3 — Embeddings y busqueda semantica
- **01-que-son-los-embeddings.html** — Vectores, significado, distancia; que capturan y que no.
- **02-similitud-y-vectores.html** — Similitud coseno, normalizacion, busqueda por vecinos.
- **03-chunking.html** — Trocear documentos: tamano, solape, estructura HTML, metadatos.
- **04-bases-vectoriales.html** — pgvector y sqlite-vec; indice, filtros por metadato, hibrida (BM25).

## Modulo 4 — RAG
- **01-pipeline-rag.html** — Ingesta → indexado → consulta → contexto → generacion.
- **02-retrieval-y-reranking.html** — Top-k, umbrales, reranking, cuando el retrieval falla.
- **03-citas-y-fidelidad.html** — Citar la fuente, grounding, respuestas con y sin contexto.
- **04-evaluacion-de-rag.html** — Recall, faithfulness, dataset de preguntas del Copiloto.

## Modulo 5 — Agentes y tools
- **01-bucle-agentic.html** — El bucle pensar → actuar → observar; cuando un agente y cuando no.
- **02-tools-y-handlers.html** — Disenar tools, esquemas, errores de tool y resultados.
- **03-planificacion-y-memoria.html** — Descomponer tareas, estado, memoria de conversacion.
- **04-sandboxing-y-coste.html** — Ejecutar codigo con seguridad, limites de iteraciones, coste del bucle.

## Modulo 6 — Extensibilidad: MCP, skills y plugins
- **01-mcp-servidores.html** — Que es MCP; montar un servidor que expone tools del Copiloto.
- **02-tools-resources-prompts.html** — Tools, resources y prompts; transporte stdio.
- **03-skills-skill-md.html** — Skills (`SKILL.md`): ensenar *como* hacer un tipo de trabajo al agente.
- **04-plugins-y-permisos.html** — Plugins, hooks y permisos; que puede y que no puede tocar el agente.

## Modulo 7 — Evals, calidad y produccion
- **01-datasets-y-evals.html** — Dataset de evaluacion, casos, aserciones deterministas vs juez.
- **02-llm-as-judge.html** — Juez automatico: rúbricas, sesgos, calibracion con humanos.
- **03-observabilidad-y-trazas.html** — Trazas de llamadas, tokens, coste; reproducir una mala respuesta.
- **04-guardrails-coste.html** — Moderacion, PII, caché semantica, versionado de prompts, rate limits.

## Modulo 8 — Proyecto final: Copiloto de la Academy
- **01-arquitectura.html** — Arquitectura del Copiloto: capas, contratos, configuracion. *(project)*
- **02-rag-sobre-los-cursos.html** — RAG real sobre `Academy/Learn */web/modulos/**`. *(project)*
- **03-tools-y-agente.html** — El agente completo con tools y memoria. *(project)*
- **04-evals-y-despliegue.html** — Evaluacion, CLI/API y puesta en produccion. *(project)*

## Recursos
- **cheatsheet.html** — Tokens, coste, plantilla de prompt, pipeline RAG, esquema de tool, ciclo del agente.
- **glosario.html** — Token, contexto, embedding, chunk, RAG, tool calling, MCP, skill, eval, guardrail…
- **faq.html** — FAQ general del curso.

**Total: 9 modulos (0-8), 36 lecciones + 3 recursos.**

---

# 8. Estructura de repositorio

```text
Learn IA/
├── spec.md                      ← ★ ESTE DOCUMENTO (fuente de verdad del curso)
├── AGENTS.md                    ← puntero corto a spec.md + reglas para agentes
├── package.json                 ← script serve
├── pnpm-workspace.yaml
├── .gitignore
├── .gitattributes               ← LF
│   (opencode.json y las skills viven en la raiz de la Academy)
├── web/                         ← ★ EL CURSO (web estatica, sin build)
│   ├── index.html               ← portada + indice
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/00-preparacion/ … 08-proyecto-final/
│   └── recursos/                ← cheatsheet, glosario, faq
├── scripts/serve-web.mjs
└── proyectos/copiloto/          ← asistente Python real (RAG + tools + evals)
    ├── pyproject.toml
    ├── README.md
    └── src/copiloto/{__init__,cli,llm,index,rag,tools,agent,evals}.py
```

---

# 9. Formato de entrega

Web estatica (HTML + CSS + JavaScript vanilla, sin Markdown ni build). `web/` contiene el curso navegable; el chrome (cabecera, sidebar, progreso, tema) lo inyecta `web/assets/js/site.js` desde `web/assets/js/course-data.js` (unica fuente de verdad). El proyecto `proyectos/copiloto/` es Python real y ejecutable con `uv`.

## Anatomia de una leccion

Identica a la familia Academy:

```html
<!DOCTYPE html>
<html lang="es" data-theme="dark" data-module="04" data-lesson="01-pipeline-rag">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>… — Modulo 4 · Curso IA</title>
  <script>document.documentElement.dataset.theme = localStorage.getItem('ia-theme') || 'dark';</script>
  <link rel="stylesheet" href="../../assets/css/site.css">
  <link rel="icon" href="../../assets/favicon.svg" type="image/svg+xml">
</head>
<body>
  <main><article class="lesson">
    <header class="lesson-header">…</header>
    <section id="introduccion">…</section> … 10 secciones …
  </article></main>
  <script>window.SITE_BASE = '../../';</script>
  <script src="../../assets/js/course-data.js"></script>
  <script src="../../assets/js/highlight.js"></script>
  <script src="../../assets/js/site.js"></script>
</body>
</html>
```

Notas:
- `SITE_BASE` = `'../../'` en lecciones, `'./'` en portada, `'../'` en `web/recursos/`.
- Claves de localStorage: `ia-theme`, `ia-progress`.
- Bloques: `callout callout--why/--tip/--note/--warning/--analogy`, `challenge`, `faq details`, `table`, `ol.steps`, `pre > code.language-*`, tokens `t-*`.

---

# 10. Diseno y marca

- Tema oscuro por defecto, toggle a claro.
- Paleta propia (identidad "IA"): violeta/cian sobre fondo profundo `#0b0d12`.
- Firma de portada: una **conversacion que se escribe sola** (turno usuario → respuesta con cita de leccion).
- Identificadores en ingles; contenido, comentarios y UI en espanol.

---

# 11. Convenciones del curso

1. Idioma: contenido, comentarios y UI en espanol; identificadores/comandos en ingles.
2. Python 3.12+ y `uv` en todos los comandos.
3. Esquema del proyecto conductor: CLI `copiloto`; paquete `src/copiloto/`.
4. Local primero (Ollama); nube como alternativa explicita.
5. Cada leccion usa SIEMPRE las 10 secciones.
6. Retos sin resolver en el HTML (solo pista).
7. En el Resumen, tabla de "concepto → herramienta" cuando aplique.

---

# 12. Documentacion actualizada

Consultar la documentacion oficial (docs de OpenAI/Anthropic, docs de Ollama, pgvector, SDK de MCP, pydantic) via Context7 antes de afirmar APIs, flags o versiones. Preferir la version estable mas reciente.

---

# 13. Pagina principal (`web/index.html`)

Portada con hero, firma (conversacion que se escribe), estadisticas y `<div id="module-list">` alimentado por `course-data.js`. Misma mecanica que el resto de cursos.

---

# 14. Checklist de implementacion

## Fase 0 — Spec y AGENTS ✅
- [x] `spec.md` (este documento).
- [x] `AGENTS.md` (puntero corto). Skills centralizadas en `Academy/.agents/skills/`.

## Fase 1 — Scaffold ✅
- [x] `package.json` (script `serve`), `pnpm-workspace.yaml`, `.gitignore`, `.gitattributes` (LF).
- [x] `scripts/serve-web.mjs`.
- [x] Assets: `web/assets/{css,js,fonts,favicon.svg}` re-tematizados (violeta/cian), `course-data.js` completo.
- [x] Portada `web/index.html` + firma.

## Fase 2 — Lecciones (web/modulos/) ✅
- [x] 00-preparacion … 08-proyecto-final (36 lecciones).

## Fase 3 — Recursos ✅
- [x] `web/recursos/{cheatsheet,glosario,faq}.html`.

## Fase 4 — Proyecto Copiloto (ejecutable) ✅
- [x] `proyectos/copiloto/` con `uv`, RAG + tools + evals y README.

## Fase 5 — Academy ✅
- [x] Tarjeta en `Academy/index.html` + contador + fila en `specs.md` §3 y `README.md`.

---

# 15. Reglas importantes

1. Nunca des por sentado un concepto: tokens, contexto y "alucinacion" se explican desde cero.
2. Explica siempre el "por que" y separa **modelo** (probabilistico) de **codigo** (determinista).
3. Todo el codigo debe ejecutarse con Python 3.12+ y `uv`; el nucleo, sin API key (Ollama).
4. Mide coste y latencia; ensena a observar antes de optimizar.
5. La evaluacion es parte del curso, no un anexo.
6. Provider-neutral en el discurso; concreto en el codigo.
7. Nada de claves ni secretos en el repositorio.
8. Si un concepto depende de otro, enlaza la leccion correspondiente.
9. Prioriza la comprension profunda frente a la velocidad.
10. No convertir el curso en un tutorial de un unico framework comercial.
