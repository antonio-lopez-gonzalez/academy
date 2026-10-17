output "namespace" {
  value = kubernetes_namespace.copiloto.metadata[0].name
}
