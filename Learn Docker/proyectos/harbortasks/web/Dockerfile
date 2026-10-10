# web/Dockerfile — Construye la web de HarborTasks (nginx)
# La web es un SPA estático; este Dockerfile copia el contenido a nginx.
# Contexto de build: carpeta web/  →  docker build -f web/Dockerfile ./web

FROM nginx:1.27-alpine AS runtime
# Copia el contenido estático del SPA (html, css, js)
COPY index.html /usr/share/nginx/html/index.html
# Configuración: proxy de /api hacia la api y SPA en /
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
# nginx en primer plano (PID 1)
CMD ["nginx", "-g", "daemon off;"]