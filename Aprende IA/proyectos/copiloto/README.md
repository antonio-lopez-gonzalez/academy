# Copiloto

Asistente con **RAG sobre los cursos** de la Academy. Es el **proyecto conductor** del curso
[`Aprende IA`](../../spec.md): crece módulo a módulo hasta ser un agente con tools y evals.

## Requisitos

- **Python 3.12+** y [`uv`](https://docs.astral.sh/uv/).
- **Ollama** en local (local-first) o, como alternativa, una API en la nube.

## Uso

```bash
cd "Aprende IA/proyectos/copiloto"
uv run copiloto --help      # ayuda de la CLI
uv run copiloto doctor      # comprueba el entorno (python, modelo, índice)
```

## Estructura

```text
copiloto/
├── pyproject.toml
├── README.md
├── data/                 # índice y datos generados (no versionados)
├── evals/
│   └── dataset.jsonl     # preguntas de evaluación del Copiloto
└── src/copiloto/
    ├── cli.py            # interfaz de línea de comandos (entry point `copiloto`)
    ├── config.py         # configuración (variables de entorno)
    ├── llm.py            # cliente del modelo (Ollama por defecto)
    ├── index.py          # ingesta + chunking de las lecciones
    ├── rag.py            # recuperación + generación con citas
    ├── tools.py          # tools del agente (buscar, abrir, progreso)
    ├── agent.py          # el bucle agentic
    └── evals.py          # evaluación (dataset + juez)
```

> Este es el esqueleto inicial: cada módulo del curso implementa una capa.
> Local-first: por defecto usa Ollama; **nunca** guardes claves en el repositorio.
