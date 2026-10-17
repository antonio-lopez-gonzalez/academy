/**
 * Servidor estático de CERO dependencias para la web del curso.
 *
 * ¿Por qué existe este archivo?
 * La web del curso (carpeta web/) es 100 % estática: HTML + CSS + JS.
 * Solo necesita un servidor que entregue archivos por HTTP.
 * Lo escribimos con el módulo nativo `node:http` para no instalar nada.
 *
 * Uso:
 *   node scripts/serve-web.mjs          → http://localhost:3000
 *   node scripts/serve-web.mjs 8080     → http://localhost:8080
 */

import { createServer } from "node:http";
import { readFile } from "node:fs/promises";
import { join, extname, normalize } from "node:path";
import { fileURLToPath } from "node:url";

// Carpeta web/ relativa a la ubicación de este script (scripts/ → ../web)
const WEB_ROOT = fileURLToPath(new URL("../web", import.meta.url));

// Puerto: primer argumento de la línea de comandos o 3000
const PORT = Number(process.argv[2]) || 3000;

// Tabla de tipos MIME: sin ella el navegador no interpretaría bien los archivos
const MIME_TYPES = {
  ".html": "text/html; charset=utf-8",
  ".css": "text/css; charset=utf-8",
  ".js": "text/javascript; charset=utf-8",
  ".mjs": "text/javascript; charset=utf-8",
  ".json": "application/json; charset=utf-8",
  ".svg": "image/svg+xml",
  ".png": "image/png",
  ".jpg": "image/jpeg",
  ".jpeg": "image/jpeg",
  ".webp": "image/webp",
  ".ico": "image/x-icon",
  ".woff": "font/woff",
  ".woff2": "font/woff2",
  ".map": "application/json",
  ".txt": "text/plain; charset=utf-8",
};

const server = createServer(async (request, response) => {
  try {
    // 1. Limpiamos la URL: quitamos query strings (?a=b) y decodificamos %20 etc.
    let urlPath = decodeURIComponent(new URL(request.url, "http://localhost").pathname);

    // 2. Si piden "/", servimos el index.html (como hace cualquier servidor web)
    if (urlPath === "/") urlPath = "/index.html";

    // 3. Seguridad básica: normalize + join evitan rutas tipo ../../etc/passwd
    const filePath = join(WEB_ROOT, normalize(urlPath));
    if (!filePath.startsWith(WEB_ROOT)) {
      response.writeHead(403);
      response.end("403 - Acceso prohibido");
      return;
    }

    // 4. Leemos el archivo y lo servimos con su tipo MIME
    const content = await readFile(filePath);
    const contentType = MIME_TYPES[extname(filePath).toLowerCase()] ?? "application/octet-stream";
    response.writeHead(200, { "Content-Type": contentType });
    response.end(content);
  } catch {
    // Cualquier error de lectura → 404 estándar
    response.writeHead(404, { "Content-Type": "text/plain; charset=utf-8" });
    response.end("404 - No encontrado");
  }
});

server.listen(PORT, () => {
  console.log(`\n  Curso React 19 disponible en:  http://localhost:${PORT}\n`);
  console.log(`  Sirviendo la carpeta: ${WEB_ROOT}`);
  console.log(`  Pulsa Ctrl+C para detener el servidor.\n`);
});
