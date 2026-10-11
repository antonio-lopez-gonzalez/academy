/**
 * course-data.js — MAPA COMPLETO DEL CURSO DEVOPS
 *
 * Unica fuente de verdad para:
 *  - el sidebar (arbol de modulos y lecciones)
 *  - la navegacion anterior / siguiente
 *  - el indice de la portada
 *  - el calculo de progreso
 *
 * Cada leccion apunta a un archivo HTML real dentro de web/modulos/.
 * Si anades o renombras una leccion, edita SOLO este archivo.
 */
window.COURSE = {
  title: "Curso DevOps",
  subtitle: "CI/CD — del commit al despliegue",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de CI/CD" },
    { file: "recursos/glosario.html", title: "Glosario de terminos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MODULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparacion",
      description: "Que es CI/CD y la cultura DevOps, el proyecto conductor y la anatomia de un workflow de GitHub Actions.",
      lessons: [
        { file: "01-cicd-y-cultura.html", title: "CI/CD y cultura DevOps", minutes: 30 },
        { file: "02-proyecto-conductor.html", title: "Publicar el Copiloto", minutes: 30 },
        { file: "03-entorno-gh-act.html", title: "Entorno: gh, act y Docker", minutes: 35 },
        { file: "04-anatomia-workflow.html", title: "Anatomia de un workflow", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 1 */
    {
      id: "01",
      dir: "modulos/01-git-ramas-versiones",
      title: "Git, ramas y versiones",
      description: "Estrategias de ramas, pull requests, versionado semantico y Conventional Commits con changelog.",
      lessons: [
        { file: "01-estrategias-de-ramas.html", title: "Estrategias de ramas", minutes: 30 },
        { file: "02-pull-requests-y-review.html", title: "Pull requests y revision", minutes: 30 },
        { file: "03-versionado-semantico.html", title: "Versionado semantico", minutes: 30 },
        { file: "04-conventional-commits-changelog.html", title: "Conventional Commits y changelog", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 2 */
    {
      id: "02",
      dir: "modulos/02-integracion-continua",
      title: "Integracion continua",
      description: "El pipeline de CI: lint, tests y build; matrices, cache, artefactos y cobertura.",
      lessons: [
        { file: "01-pipeline-ci.html", title: "El pipeline de CI", minutes: 35 },
        { file: "02-lint-test-build.html", title: "Lint, tests y build", minutes: 35 },
        { file: "03-matrices-y-cache.html", title: "Matrices y cache", minutes: 35 },
        { file: "04-artefactos-y-cobertura.html", title: "Artefactos y cobertura", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 3 */
    {
      id: "03",
      dir: "modulos/03-build-artefactos",
      title: "Build y artefactos",
      description: "Docker en CI, registries y tagging inmutable, SBOM/procedencia y cache de builds.",
      lessons: [
        { file: "01-docker-en-ci.html", title: "Docker en CI", minutes: 35 },
        { file: "02-registries-y-tagging.html", title: "Registries y tagging", minutes: 35 },
        { file: "03-sbom-y-provenance.html", title: "SBOM y procedencia", minutes: 30 },
        { file: "04-cache-de-builds.html", title: "Cache de builds", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 4 */
    {
      id: "04",
      dir: "modulos/04-entrega-continua",
      title: "Entrega continua",
      description: "Entornos y configuracion, secretos y OIDC, puertas de aprobacion y migraciones en CD.",
      lessons: [
        { file: "01-entornos-y-config.html", title: "Entornos y configuracion", minutes: 35 },
        { file: "02-secretos-y-variables.html", title: "Secretos, variables y OIDC", minutes: 35 },
        { file: "03-gates-y-aprobaciones.html", title: "Puertas de aprobacion", minutes: 30 },
        { file: "04-migraciones-en-cd.html", title: "Migraciones en CD", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 5 */
    {
      id: "05",
      dir: "modulos/05-estrategias-despliegue",
      title: "Estrategias de despliegue",
      description: "Rolling, blue/green, canary con feature flags y rollback seguro.",
      lessons: [
        { file: "01-rolling.html", title: "Despliegue rolling", minutes: 30 },
        { file: "02-blue-green.html", title: "Blue/green", minutes: 35 },
        { file: "03-canary-y-feature-flags.html", title: "Canary y feature flags", minutes: 35 },
        { file: "04-rollback.html", title: "Rollback", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 6 */
    {
      id: "06",
      dir: "modulos/06-iac",
      title: "Infraestructura como codigo",
      description: "Que es IaC, Terraform/OpenTofu, estado remoto y modulos, y entornos definidos como codigo.",
      lessons: [
        { file: "01-que-es-iac.html", title: "Que es IaC", minutes: 30 },
        { file: "02-terraform-basico.html", title: "Terraform basico", minutes: 40 },
        { file: "03-estado-y-modulos.html", title: "Estado y modulos", minutes: 35 },
        { file: "04-entornos-con-iac.html", title: "Entornos con IaC", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final: el pipeline del Copiloto",
      description: "CI, build y CD end-to-end; entornos con aprobacion; observabilidad y seguridad de la cadena de suministro.",
      lessons: [
        { file: "01-pipeline-completo.html", title: "Pipeline completo", minutes: 45, project: true },
        { file: "02-cd-y-entornos.html", title: "CD y entornos", minutes: 45, project: true },
        { file: "03-observabilidad-pipeline.html", title: "Observabilidad del pipeline", minutes: 40, project: true },
        { file: "04-seguridad-supply-chain.html", title: "Seguridad de la cadena de suministro", minutes: 40, project: true },
      ],
    },
  ],
};
