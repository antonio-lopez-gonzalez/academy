/**
 * Generador del esqueleto común de las apps del curso (Vite + React 19).
 *
 * ¿Por qué existe?
 * El curso contiene ~25 proyectos Vite. Todos comparten el mismo
 * boilerplate (package.json, vite.config, index.html, main.jsx...).
 * Este script crea ese esqueleto para que cada proyecto solo tenga
 * que escribir a mano su código específico en src/.
 *
 * Uso:
 *   node scripts/scaffold-vite-app.mjs <ruta> "<título>" [deps extra separadas por coma]
 *
 * Ejemplo:
 *   node scripts/scaffold-vite-app.mjs proyectos/m03-calculadora "Calculadora" react-router-dom@^7
 */

import { mkdir, writeFile } from "node:fs/promises";
import { join, basename } from "node:path";

const [, , targetDir, title = "App", extraDepsArg = ""] = process.argv;

if (!targetDir) {
  console.error("Uso: node scripts/scaffold-vite-app.mjs <ruta> \"<título>\" [deps-extra]");
  process.exit(1);
}

const appName = basename(targetDir);

// Dependencias extra opcionales, formato: "react-router-dom@^7,zustand@^5"
const extraDeps = Object.fromEntries(
  extraDepsArg
    .split(",")
    .filter(Boolean)
    .map((dep) => {
      const at = dep.lastIndexOf("@");
      // lastIndexOf porque el nombre puede llevar scope: @reduxjs/toolkit@^2
      return at > 0 ? [dep.slice(0, at), dep.slice(at + 1)] : [dep, "latest"];
    })
);

const packageJson = {
  name: appName,
  private: true,
  version: "1.0.0",
  type: "module",
  scripts: {
    dev: "vite",
    build: "vite build",
    preview: "vite preview",
  },
  dependencies: {
    react: "^19.1.0",
    "react-dom": "^19.1.0",
    ...extraDeps,
  },
  devDependencies: {
    "@vitejs/plugin-react": "^5.0.0",
    vite: "^7.1.0",
  },
};

const viteConfig = `import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
});
`;

const indexHtml = `<!doctype html>
<html lang="es">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>${title}</title>
  </head>
  <body>
    <!-- Punto de montaje: React renderizará aquí toda la aplicación -->
    <div id="root"></div>
    <script type="module" src="/src/main.jsx"></script>
  </body>
</html>
`;

const mainJsx = `import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.jsx";
import "./index.css";

// createRoot: punto de entrada de React 19 (API de cliente)
createRoot(document.getElementById("root")).render(
  <StrictMode>
    <App />
  </StrictMode>
);
`;

const indexCss = `/* Reset suave y estilos base del proyecto */
*,
*::before,
*::after {
  box-sizing: border-box;
}

body {
  margin: 0;
  font-family: system-ui, -apple-system, "Segoe UI", Roboto, sans-serif;
  line-height: 1.5;
}
`;

const appJsx = `export default function App() {
  return <h1>${title}</h1>;
}
`;

// Creamos la estructura de carpetas y los archivos base
await mkdir(join(targetDir, "src"), { recursive: true });

await Promise.all([
  writeFile(join(targetDir, "package.json"), JSON.stringify(packageJson, null, 2) + "\n"),
  writeFile(join(targetDir, "vite.config.js"), viteConfig),
  writeFile(join(targetDir, "index.html"), indexHtml),
  writeFile(join(targetDir, "src", "main.jsx"), mainJsx),
  writeFile(join(targetDir, "src", "index.css"), indexCss),
  writeFile(join(targetDir, "src", "App.jsx"), appJsx),
]);

console.log(`✔ Esqueleto creado en ${targetDir}`);
