variable "project" {
  type        = string
  description = "Nombre del proyecto (prefijo de los recursos)."
  default     = "copiloto"
}

variable "environment" {
  type        = string
  description = "Entorno (staging/production)."
  validation {
    condition     = contains(["staging", "production"], var.environment)
    error_message = "El entorno debe ser 'staging' o 'production'."
  }
}

variable "location" {
  type        = string
  description = "Región de Azure."
  default     = "westeurope"
}

variable "image" {
  type        = string
  description = "Imagen de la API por digest inmutable (ghcr.io/... o <acr>.azurecr.io/...@sha256:...)."
}

variable "owner" {
  type        = string
  description = "Responsable, para las etiquetas de coste."
  default     = "academy"
}

locals {
  base_name = "${var.project}-${var.environment}"
  tags = {
    project     = var.project
    environment = var.environment
    owner       = var.owner
    "managed-by" = "terraform"
  }
}
