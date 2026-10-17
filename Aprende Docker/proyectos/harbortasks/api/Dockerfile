# api/Dockerfile — Dockerfile multi-stage de la API de HarborTasks
#
# Etapa 1 (deps): instala dependencias de producción en una imagen node.
# Etapa 2 (build): copia el código fuente.
# Etapa 3 (runtime): imagen final mínima con solo deps + código y usuario no-root.
# Resultado: imagen pequeña y segura.
#
# Contexto de build: carpeta api/  →  docker build -f api/Dockerfile ./api

# ---- Etapa 1: dependencias (muy cacheable) ----
FROM node:20-alpine AS deps
WORKDIR /app
# Copiamos solo package*.json: si no cambian, Docker reutiliza esta capa
COPY package*.json ./
RUN npm ci --omit=dev

# ---- Etapa 2: código ----
FROM node:20-alpine AS build
WORKDIR /app
# Reutilizamos las dependencias de la etapa anterior (caché de capas)
COPY --from=deps /app/node_modules ./node_modules
COPY src ./src

# ---- Etapa 3: imagen final de runtime ----
FROM node:20-alpine AS runtime
ENV NODE_ENV=production
WORKDIR /app
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/src ./src
# Usuario sin privilegios (mejores prácticas de seguridad)
RUN addgroup -S app && adduser -S app -G app
USER app
EXPOSE 3000
# CMD en exec form: node es PID 1 y recibe SIGTERM correctamente
CMD ["node", "src/index.js"]