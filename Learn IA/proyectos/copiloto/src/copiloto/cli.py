"""CLI del Copiloto (`copiloto`). Local-first: no requiere red para `--help`/`doctor`."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

from . import __version__
from .config import load_settings


def _academy_root() -> Path:
    """Sube desde este paquete hasta la raíz de la Academy (contiene `Learn IA/`)."""
    here = Path(__file__).resolve()
    for parent in here.parents:
        if (parent / "Learn IA").is_dir():
            return parent
    return here.parents[-1]


def cmd_doctor(_: argparse.Namespace) -> int:
    s = load_settings()
    root = _academy_root()
    print(f"copiloto {__version__}")
    print(f"python   : {sys.version.split()[0]}")
    print(f"modelo   : {s.model} (Ollama en {s.ollama_host})")
    print(f"academy  : {root}")
    print(f"lecciones: {len(list(root.glob('Learn */web/modulos/**/*.html')))}")
    return 0


def cmd_index(_: argparse.Namespace) -> int:
    from .index import iter_lesson_files

    files = iter_lesson_files(_academy_root())
    print(f"Lecciones encontradas: {len(files)}")
    print("(La indexación real se implementa en el módulo 3 del curso.)")
    return 0


def cmd_ask(args: argparse.Namespace) -> int:
    from .llm import default_client
    from .rag import answer

    result = answer(args.question, context=[], llm=default_client())
    print(result.text)
    return 0


def cmd_agent(args: argparse.Namespace) -> int:
    from .agent import run
    from .llm import default_client

    result = run(args.question, default_client())
    print(result.answer)
    return 0


def cmd_eval(args: argparse.Namespace) -> int:
    from .evals import load_dataset

    path = Path(args.dataset)
    if not path.exists():
        print(f"Dataset no encontrado: {path}")
        return 1
    cases = load_dataset(path)
    print(f"Casos de evaluación: {len(cases)}")
    print("(El harness completo se implementa en el módulo 7 del curso.)")
    return 0


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(prog="copiloto", description="Copiloto de la Academy (curso Learn IA)")
    parser.add_argument("--version", action="version", version=f"copiloto {__version__}")
    sub = parser.add_subparsers(dest="command", required=True)

    sub.add_parser("doctor", help="Comprueba el entorno").set_defaults(func=cmd_doctor)
    sub.add_parser("index", help="Indexa las lecciones del curso").set_defaults(func=cmd_index)

    p_ask = sub.add_parser("ask", help="Pregunta puntual al Copiloto")
    p_ask.add_argument("question")
    p_ask.set_defaults(func=cmd_ask)

    p_agent = sub.add_parser("agent", help="Ejecuta el agente con tools")
    p_agent.add_argument("question")
    p_agent.set_defaults(func=cmd_agent)

    p_eval = sub.add_parser("eval", help="Ejecuta la evaluación sobre un dataset JSONL")
    p_eval.add_argument("--dataset", default="evals/dataset.jsonl")
    p_eval.set_defaults(func=cmd_eval)

    return parser


def main(argv: list[str] | None = None) -> int:
    parser = build_parser()
    args = parser.parse_args(argv)
    return args.func(args)


if __name__ == "__main__":
    raise SystemExit(main())
