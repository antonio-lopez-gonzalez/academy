# Blog profesional (proyecto final del curso Astro)

Aplicación **Astro 5** completa que reúne lo aprendido: routing por ficheros,
**content collections** tipadas con Zod, layouts, SEO y RSS. Es el *proyecto final*
del curso [`Aprende Astro`](../../AGENTS.md).

## Requisitos

- Node.js 22.12+ y **pnpm**.

## Uso

```bash
cd "Aprende Astro/proyectos/blog-astro"
pnpm install
pnpm dev        # http://localhost:4321
pnpm build      # genera dist/ (SSG)
pnpm preview    # sirve el build
pnpm check      # astro check (tipos)
```

## Estructura

```text
blog-astro/
├── astro.config.mjs        # site + integraciones (mdx, sitemap)
├── src/
│   ├── content.config.ts   # colecciones y esquema Zod (Content Layer)
│   ├── content/blog/*.md   # posts (contenido)
│   ├── layouts/BaseLayout.astro
│   ├── components/PostCard.astro
│   └── pages/
│       ├── index.astro     # portada / listado
│       ├── blog/[slug].astro  # detalle (getStaticPaths)
│       └── rss.xml.ts      # feed RSS
└── public/favicon.svg
```

> Los posts de `src/content/blog/` son contenido de ejemplo: edítalos o bórralos.
