# Despliegue del Copiloto (ejemplo neutro sobre Kubernetes).
provider "kubernetes" {
  config_path = var.kubeconfig
}

resource "kubernetes_namespace" "copiloto" {
  metadata {
    name = var.environment
  }
}

resource "kubernetes_deployment" "api" {
  metadata {
    name      = "copiloto-api"
    namespace = kubernetes_namespace.copiloto.metadata[0].name
  }

  spec {
    replicas = var.replicas

    selector {
      match_labels = { app = "copiloto-api" }
    }

    template {
      metadata {
        labels = { app = "copiloto-api" }
      }
      spec {
        container {
          name  = "api"
          image = var.image   # por digest (inmutable)
          port {
            container_port = 8000
          }
          env {
            name  = "COPILOTO_API_ENV"
            value = var.environment
          }
        }
      }
    }
  }
}
