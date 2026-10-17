"""CLI del Copiloto (`copiloto`). Local-first: funciona sin red con el fallback."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

from . import __version__
from .config import Settings, load_settings


def academy_root() -> Path:
    """Sube desde este paquete hasta la raíz de la Academy (contiene `Aprende IA/`)."""
    for parent in Path(__file__).resolve().parents:
        if (parent / "Aprende IA").is_dir():
            return parent
    return Path(__file__).resolve().parents[-1]


def _embedder(settings: Settings):
    from .embeddings import get_embedder

    return get_embedder(settings)


def _llm(settings: Settings):
    from .llm import get_llm

    return get_llm(settings)


def cmd_doctor(_: argparse.Namespace) -> int:
    from .index import iter_lesson_files
    from .store import Store

    settings = load_settings()
    embedder = _embedder(settings)
    print(f"copiloto {__version__}")
    print(f"python    : {sys.version.split()[0]}")
    print(f"modelo    : {settings.model}  ·  embeddings: {embedder.name}")
    print(f"ollama    : {settings.ollama_host}")
    print(f"academy   : {academy_root()}")
    print(f"lecciones : {len(iter_lesson_files(academy_root()))}")
    if settings.db_path.exists():
        store = Store(settings.db_path)
        print(f"índice    : {store.count()} chunks en {settings.db_path}")
        store.close()
    else:
        print(f"índice    : (sin construir) → ejecuta `copiloto index`")
    return 0


def cmd_index(args: argparse.Namespace) -> int:
    from .index import build_index

    settings = load_settings()
    embedder = _embedder(settings)
    print(f"Indexando con {embedder.name} …")
    store = build_index(academy_root(), settings, embedder, course=args.course)
    print(f"OK: {store.count()} chunks en {settings.db_path}")
    store.close()
    return 0


def _retriever(settings: Settings, course: str | None = None):
    from .rag import retrieve
    from .store import Store

    embedder = _embedder(settings)
    store = Store(settings.db_path)
    return lambda q: retrieve(q, store, embedder, k=settings.top_k, course=course)


def cmd_search(args: argparse.Namespace) -> int:
    settings = load_settings()
    results = _retriever(settings, args.course)(args.query)
    for i, (score, chunk) in enumerate(results):
        print(f"[{i + 1}] {score:.3f}  {chunk.source}")
        print(f"    {chunk.text[:160]}…")
    return 0


def cmd_ask(args: argparse.Namespace) -> int:
    from .rag import answer

    settings = load_settings()
    results = _retriever(settings, args.course)(args.question)
    context = [chunk for score, chunk in results if score >= settings.min_score]
    result = answer(args.question, context, _llm(settings))
    print(result.text)
    if result.sources:
        print("\nFuentes:")
        for src in result.sources:
            print(f"  · {src}")
    return 0


def cmd_agent(args: argparse.Namespace) -> int:
    from .agent import run
    from .tools import build_tools

    settings = load_settings()
    tools = build_tools(_retriever(settings, args.course), academy_root())
    result = run(args.question, _llm(settings), tools)
    print(result.answer)
    if result.steps:
        print("\nPasos:")
        for step in result.steps:
            print(f"  · {step.tool}({step.arguments})")
    return 0


def cmd_eval(args: argparse.Namespace) -> int:
    from .evals import load_dataset, run_evals, check_abstention, check_citation

    settings = load_settings()
    path = Path(args.dataset)
    if not path.exists():
        print(f"Dataset no encontrado: {path}")
        return 1
    cases = load_dataset(path)
    retriever = _retriever(settings)
    llm = _llm(settings)

    def answer_fn(question: str):
        from .rag import Answer, answer

        context = [chunk for score, chunk in retriever(question) if score >= settings.min_score]
        return answer(question, context, llm)

    results = run_evals(cases, answer_fn)
    passed = sum(1 for r in results if r.passed)
    for r in results:
        mark = "✓" if r.passed else "✗"
        print(f"  {mark} {r.case.question}  ({r.detail})")
    print(f"\n{passed}/{len(results)} casos OK")
    return 0 if passed == len(results) else 1


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(prog="copiloto", description="Copiloto de la Academy (curso Aprende IA)")
    parser.add_argument("--version", action="version", version=f"copiloto {__version__}")
    sub = parser.add_subparsers(dest="command", required=True)

    sub.add_parser("doctor", help="Comprueba el entorno y el índice").set_defaults(func=cmd_doctor)

    p_index = sub.add_parser("index", help="Indexa las lecciones de los cursos")
    p_index.add_argument("--course", default=None, help="Limita a un curso, p. ej. 'Aprende IA'")
    p_index.set_defaults(func=cmd_index)

    p_search = sub.add_parser("search", help="Busca fragmentos relevantes")
    p_search.add_argument("query")
    p_search.add_argument("--course", default=None)
    p_search.set_defaults(func=cmd_search)

    p_ask = sub.add_parser("ask", help="Pregunta al Copiloto (RAG con citas)")
    p_ask.add_argument("question")
    p_ask.add_argument("--course", default=None)
    p_ask.set_defaults(func=cmd_ask)

    p_agent = sub.add_parser("agent", help="Ejecuta el agente con tools")
    p_agent.add_argument("question")
    p_agent.add_argument("--course", default=None)
    p_agent.set_defaults(func=cmd_agent)

    p_eval = sub.add_parser("eval", help="Evalúa el Copiloto sobre un dataset JSONL")
    p_eval.add_argument("--dataset", default="evals/dataset.jsonl")
    p_eval.set_defaults(func=cmd_eval)

    return parser


def main(argv: list[str] | None = None) -> int:
    """Punto de entrada de la CLI."""
    # Consolas Windows (cp1252): fuerza UTF-8 para poder imprimir ✓, → y tildes.
    for stream in (sys.stdout, sys.stderr):
        try:
            stream.reconfigure(encoding="utf-8")  # type: ignore[union-attr]
        except (AttributeError, ValueError):
            pass
    args = build_parser().parse_args(argv)
    return args.func(args)


if __name__ == "__main__":
    raise SystemExit(main())
