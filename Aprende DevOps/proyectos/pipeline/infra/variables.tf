variable "kubeconfig" {
  type    = string
  default = "~/.kube/config"
}

variable "environment" {
  type        = string
  description = "Nombre del entorno (staging/production)."
  validation {
    condition     = contains(["staging", "production"], var.environment)
    error_message = "El entorno debe ser 'staging' o 'production'."
  }
}

variable "image" {
  type        = string
  description = "Imagen por digest inmutable, p. ej. ghcr.io/org/repo@sha256:..."
}

variable "replicas" {
  type    = number
  default = 2
}
