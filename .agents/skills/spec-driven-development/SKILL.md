---
name: spec-driven-development
description: Use when writing Learn SDD lesson content or the Lumen sample project. Defines the SDD vocabulary, the specify-plan-implement-verify loop, and the templates for specs, acceptance criteria, ADRs, skills and delta-specs. Lesson content must stay faithful to Learn SDD/spec.md — this skill is the domain companion to it, not a replacement.
---

# Spec-Driven Development (domain skill)

You are writing a course that teaches developers to work **spec-first with coding agents**. The method, not any single vendor tool, is the subject. When the course spec (`Learn SDD/spec.md`) and this skill disagree, the spec wins — flag the conflict instead of guessing.

## 1. Core vocabulary (use these exact meanings in lessons)

- **Spec**: a versioned document stating *what* is wanted and *why*, with observable acceptance criteria. It is the source of truth. It is **not** a prompt, a ticket, a README, or a design doc.
- **Prompt**: a one-off instruction in a chat thread. Evaporates. Always traceable *back* to a spec, never a substitute for one.
- **Plan**: the ordered, risk-aware path from spec to code (steps, files, out of scope). Written *before* code, reviewed by a human.
- **Source of truth**: the artefact that wins arguments. In SDD it is the spec (plus its contracts), not the chat log and not "whatever the code does today".
- **Acceptance criteria**: checkable statements of done ("it can be demonstrated that…"). Every criterion must be *verifiable* without reading the implementer's mind.
- **No-objetivos (non-goals)**: what is explicitly out of scope. The cheapest way to prevent an agent (or human) inventing features.
- **Constraint**: a technical or business restriction (stack, compatibility, perf budget, legal). Separate from goals.
- **Glossary / ubiquitous language**: the team's agreed terms. Ambiguity here becomes bugs there.
- **Contract**: a machine-checkable part of the spec (JSON Schema, OpenAPI). Generate/validate against it.
- **ADR**: one Architecture Decision Record = one decision + context + consequences. Short. Numbered (`adr/0001-*.md`).
- **Skill (`SKILL.md`)**: a reusable *how-to-work* spec for a class of tasks (course строится, API design…). Product spec = *what*; skill = *how*.
- **`AGENTS.md`**: persistent instructions for agents entering a repo (the door), as opposed to the thread of today (the hallway).
- **Delta-spec**: a change spec — current state, desired state, impact, migration. Most real work is deltas, not greenfield.
- **Verify**: checking the implementation *against the spec*, not "it compiles". Definition of done lives in the spec.

## 2. The SDD loop (every lesson about process must show it)

```text
┌──────────┐    ┌──────┐    ┌───────────┐    ┌────────┐
│ SPECIFY  │───▶│ PLAN │───▶│ IMPLEMENT │───▶│ VERIFY │
│ intention│    │ path │    │ code      │    │ vs spec│
└──────────┘    └──────┘    └───────────┘    └────────┘
      ▲                                            │
      └────────── diverge? update the SPEC ─────────┘
```

- **Specify**: human (or human+agent) writes the spec. Agent may draft; human owns.
- **Plan**: agent proposes, human reviews. No code yet.
- **Implement**: agent (or human) writes code following spec + plan.
- **Verify**: human (or agent with a checklist) checks each acceptance criterion. On divergence, **fix the spec first** if the spec was wrong — never silently patch code to match a bad reading.
- Skipping *plan* is the #1 failure mode to warn about.

## 3. Spec template (lessons 01 of module 1 teach this; Lumen uses it)

```markdown
# <Feature / Product>

## Objetivo
<1–2 sentences: what and why. Observable verbs.>

## No-objetivos
- <explicitly out of scope, each a plausible misunderstanding>

## Glosario
- <term>: <agreed meaning>

## Restricciones
- <stack / compat / budget / legal>

## Supuestos
- <what we take for granted, stated so it can be challenged>

## Criterios de aceptación
- [ ] Dado <contexto>, cuando <acción>, entonces <resultado observable>
- [ ] …(each checkable without mind-reading)

## Fuera de alcance del plan
- <what the plan must not touch>
```

Rules for examples: 10–20 lines for the *minimal* example; the Lumen spec is the *real* one. Forbidden in specs: premature implementation design, filler prose, secrets, decisions without an owner, adjectives that cannot be demonstrated ("bonito", "rápido" without numbers).

## 4. Acceptance criteria style

- Prefer `Dado / Cuando / Entonces` phrasing, but do **not** require Gherkin tooling.
- Each criterion: context + trigger + observable outcome. Bad: "el CLI es usable". Good: "ejecutar `node src/cli.js add 'pan' --store data/notes.json` crea una nota con id único y la lista con `list`".
- If a criterion cannot be demonstrated in the Lumen repo with a command or a file, rewrite it.

## 5. ADR template (8–15 lines)

```markdown
# ADR 0001: <decisión>

- Fecha: <yyyy-mm-dd>
- Contexto: <qué nos obliga a decidir>
- Decisión: <qué elegimos>
- Consecuencias: <qué ganamos / qué perdemos / qué queda irreversible>
```

Open an ADR when: storage format, public CLI surface, dependency, or anything hard to reverse. Do not ADR trivia.

## 6. Delta-spec template (module 5)

```markdown
# Cambio: <título>

## Estado actual
<how it behaves today, with file/command references>

## Estado deseado
<how it must behave, with acceptance criteria>

## Impacto y compatibilidad
- <what breaks / migrates / stays compatible>

## Migración
<steps, order, rollback>
```

## 7. Lumen (the course's running example)

- **Lumen**: minimal notes inbox. CLI `lumen` (Node, vanilla JS): `add`, `list`, `close`. Store: `data/notes.json` validated against `schema/note.schema.json`. Optional static web reader.
- Note shape (stable across lessons): `{ id, title, done, createdAt }` (+ `tags` only when the module-5/7 delta introduces them — do not leak future fields into early lessons).
- CLI contract examples lessons reuse: `node src/cli.js add "comprar pan"`, `node src/cli.js list`, `node src/cli.js close <id>`, `node src/cli.js --help`.
- The `proyectos/lumen/spec/lumen.md` shipped with the course must match what the lessons teach — same structure, same criteria style.

## 8. Tooling stance (neutral, current)

- Teach the **pattern** (versioned spec + plan + verify), illustrate with **Spec Kit** (`specify`/`plan`/`tasks`/`implement`), **OpenSpec**-style artefacts, **JSON Schema**, **OpenAPI** (one lesson), **TDD/BDD** as *sibling* executable specs.
- Before asserting any tool's commands/flags/fields, check official docs via the `documentation` skill (Context7). If a tool renamed or died, say so and keep teaching the pattern.
- Never present a single commercial product as *the* SDD. Never require a paid SaaS or an API key to follow the course.

## 9. Anti-patterns to name explicitly (module 6 draws from here)

- **Under-spec**: the agent invents behaviour. Symptom: "yo no pedí eso". Fix: acceptance criteria.
- **Over-spec**: paralysis, 40-page specs nobody reads. Fix: smallest spec that removes ambiguity.
- **Spec theatre**: approved documents with no checkable criteria. Test: "show me the command that proves it".
- **Vibe-coding creep**: promoting a prototype to production without writing the spec it should have had. Rule: prototype freely, *spec before you ship*.
- **Prompt-as-spec**: pasting chat history and calling it a spec. It has no non-goals, no criteria, no owner.
- **Code-is-the-spec**: "lee el código" as an excuse for stale specs. Divergence means update the spec, in Git, with a reason.
