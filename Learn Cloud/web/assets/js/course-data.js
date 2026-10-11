/**
 * course-data.js — MAPA COMPLETO DEL CURSO CLOUD
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
  title: "Curso Cloud",
  subtitle: "Azure / AWS — despliega en produccion",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de Cloud" },
    { file: "recursos/glosario.html", title: "Glosario de terminos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MODULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparacion",
      description: "Que es la nube, los modelos de servicio, los proveedores y el control de coste desde el minuto uno.",
      lessons: [
        { file: "01-que-es-la-nube.html", title: "Que es la nube", minutes: 30 },
        { file: "02-modelos-de-servicio.html", title: "IaaS, PaaS y SaaS", minutes: 30 },
        { file: "03-proveedores-y-cuenta.html", title: "Proveedores y tu cuenta", minutes: 35 },
        { file: "04-regiones-y-coste.html", title: "Regiones y coste", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 1 */
    {
      id: "01",
      dir: "modulos/01-red-y-computo",
      title: "Red y computo",
      description: "VNet/VPC y subredes, el espectro de computo, registros de imagenes y como elegir computo.",
      lessons: [
        { file: "01-redes-vnet-vpc.html", title: "Redes: VNet y VPC", minutes: 35 },
        { file: "02-computo.html", title: "Computo: VM, contenedor y serverless", minutes: 35 },
        { file: "03-registros-de-imagenes.html", title: "Registros de imagenes", minutes: 30 },
        { file: "04-eligiendo-computo.html", title: "Como elegir computo", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 2 */
    {
      id: "02",
      dir: "modulos/02-identidad-seguridad",
      title: "Identidad y seguridad",
      description: "IAM, secretos con identidad gestionada, red segura y despliegue sin claves con OIDC.",
      lessons: [
        { file: "01-iam.html", title: "IAM: identidades y roles", minutes: 35 },
        { file: "02-secretos.html", title: "Secretos e identidad gestionada", minutes: 35 },
        { file: "03-red-segura.html", title: "Red segura y exposicion minima", minutes: 30 },
        { file: "04-oidc-sin-claves.html", title: "OIDC: desplegar sin claves", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 3 */
    {
      id: "03",
      dir: "modulos/03-almacenamiento-datos",
      title: "Almacenamiento y datos",
      description: "Almacenamiento de objetos, bases gestionadas, cache y colas, y copias de seguridad.",
      lessons: [
        { file: "01-almacenamiento-objeto.html", title: "Almacenamiento de objetos", minutes: 30 },
        { file: "02-bases-de-datos.html", title: "Bases de datos gestionadas", minutes: 35 },
        { file: "03-cache-y-cola.html", title: "Cache y colas", minutes: 30 },
        { file: "04-backups-y-retencion.html", title: "Backups y retencion", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 4 */
    {
      id: "04",
      dir: "modulos/04-contenedores-gestionados",
      title: "Contenedores gestionados",
      description: "Container Apps/App Runner, Kubernetes gestionado (AKS/EKS), escalado y despliegue del Copiloto.",
      lessons: [
        { file: "01-container-apps.html", title: "Contenedores sin gestionar cluster", minutes: 35 },
        { file: "02-kubernetes-gestionado.html", title: "Kubernetes gestionado (AKS/EKS)", minutes: 35 },
        { file: "03-escalado.html", title: "Escalado automatico", minutes: 30 },
        { file: "04-desplegar-el-copiloto.html", title: "Desplegar el Copiloto", minutes: 40 },
      ],
    },
    /* ============================================================ MODULO 5 */
    {
      id: "05",
      dir: "modulos/05-serverless-eventos",
      title: "Serverless y eventos",
      description: "Funciones serverless, eventos y colas, API gateway y cuando (no) usar serverless.",
      lessons: [
        { file: "01-functions.html", title: "Funciones serverless", minutes: 35 },
        { file: "02-eventos-y-colas.html", title: "Eventos y colas", minutes: 30 },
        { file: "03-api-gateway.html", title: "API Gateway", minutes: 30 },
        { file: "04-cuando-serverless.html", title: "Cuando usar serverless", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 6 */
    {
      id: "06",
      dir: "modulos/06-observabilidad-operacion",
      title: "Observabilidad y operacion",
      description: "Logs y metricas del cloud, trazas/APM, coste y presupuesto, y alta disponibilidad/DR.",
      lessons: [
        { file: "01-logs-y-metricas.html", title: "Logs y metricas", minutes: 35 },
        { file: "02-trazas-y-apm.html", title: "Trazas y APM", minutes: 30 },
        { file: "03-coste-y-presupuesto.html", title: "Coste y presupuesto", minutes: 30 },
        { file: "04-alta-disponibilidad-y-dr.html", title: "Alta disponibilidad y DR", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final: Copiloto en la nube",
      description: "Arquitectura en Azure, Terraform, despliegue con OIDC y coste/observabilidad en produccion.",
      lessons: [
        { file: "01-arquitectura-cloud.html", title: "Arquitectura en la nube", minutes: 45, project: true },
        { file: "02-terraform-azure.html", title: "Terraform para Azure", minutes: 45, project: true },
        { file: "03-despliegue-y-cicd.html", title: "Despliegue y CI/CD", minutes: 40, project: true },
        { file: "04-coste-y-observabilidad.html", title: "Coste y observabilidad", minutes: 40, project: true },
      ],
    },
  ],
};
