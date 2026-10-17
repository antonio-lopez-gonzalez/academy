output "copiloto_url" {
  description = "URL pública de la API."
  value       = "https://${azurerm_container_app.api.ingress[0].fqdn}"
}

output "acr_login_server" {
  description = "Servidor de login del registro (para el pipeline)."
  value       = azurerm_container_registry.acr.login_server
}

output "resource_group" {
  description = "Resource Group creado."
  value       = azurerm_resource_group.main.name
}
