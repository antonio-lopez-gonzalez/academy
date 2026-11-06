/**
 * profiles.mjs — rutas de etapas por perfil de workflow.
 *
 * Las rutas son deliberadamente cortas: el objetivo del proyecto es que se
 * pueda leer, probar y explicar en una lección. El motor real de AI-DLC usa
 * 30 etapas después del setup y ajusta profundidad, tests y revisión.
 */

const STAGES = {
  intent: { id: "intent", title: "Intent capture" },
  requirements: { id: "requirements", title: "Requirements" },
  nfr: { id: "nfr", title: "Non-functional requirements" },
  design: { id: "design", title: "Design" },
  security: { id: "security", title: "Security review" },
  plan: { id: "plan", title: "Plan approval" },
  build: { id: "build", title: "Code generation" },
  test: { id: "test", title: "Build and test" },
  deploy: { id: "deploy", title: "Deploy" },
  operate: { id: "operate", title: "Operate and learn" },
};

function route(ids) {
  return ids.map((id) => ({ ...STAGES[id] }));
}

export const PROFILES = {
  classic: {
    title: "Classic",
    description: "Ceremonia clásica: requisitos, diseño, plan, construcción y pruebas.",
    stages: route(["intent", "requirements", "design", "plan", "build", "test"]),
  },
  express: {
    title: "Express",
    description: "Camino mínimo cuando los requisitos ya están claros.",
    stages: route(["intent", "build", "test"]),
  },
  feature: {
    title: "Feature",
    description: "Feature de producción con ciclo completo y despliegue.",
    stages: route(["intent", "requirements", "design", "plan", "build", "test", "deploy"]),
  },
  enterprise: {
    title: "Enterprise",
    description: "Trabajo regulado con NFRs, seguridad y operación.",
    stages: route([
      "intent",
      "requirements",
      "nfr",
      "design",
      "security",
      "plan",
      "build",
      "test",
      "deploy",
      "operate",
    ]),
  },
  mvp: {
    title: "MVP",
    description: "Primer incremento real sin fase de operación.",
    stages: route(["intent", "requirements", "plan", "build", "test"]),
  },
  poc: {
    title: "Proof of concept",
    description: "Probar viabilidad con el camino más corto.",
    stages: route(["intent", "build", "test"]),
  },
  bugfix: {
    title: "Bugfix",
    description: "Defecto conocido con reparación y test de regresión.",
    stages: route(["intent", "build", "test"]),
  },
  refactor: {
    title: "Refactor",
    description: "Mejorar estructura sin cambiar comportamiento.",
    stages: route(["intent", "plan", "build", "test"]),
  },
  infra: {
    title: "Infrastructure",
    description: "IaC, entornos y fundamentos de despliegue.",
    stages: route(["intent", "requirements", "design", "plan", "build", "test"]),
  },
  "security-patch": {
    title: "Security patch",
    description: "Respuesta acotada a una CVE o vulnerabilidad.",
    stages: route(["intent", "build", "test"]),
  },
  workshop: {
    title: "Workshop",
    description: "Sesión de formación con una ruta corta y guiada.",
    stages: route(["intent", "design", "build", "test"]),
  },
};

export function getProfile(name) {
  const profile = PROFILES[name];
  if (!profile) {
    const available = Object.keys(PROFILES).join(", ");
    throw new Error(`perfil desconocido: ${name}. Disponibles: ${available}`);
  }
  return profile;
}

export function listProfiles() {
  return Object.entries(PROFILES).map(([id, profile]) => ({
    id,
    title: profile.title,
    description: profile.description,
    stages: profile.stages.length,
  }));
}
