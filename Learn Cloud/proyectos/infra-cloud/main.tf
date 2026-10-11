resource "azurerm_resource_group" "main" {
  name     = "rg-${local.base_name}"
  location = var.location
  tags     = local.tags
}

# --- Registro de imágenes ---
resource "azurerm_container_registry" "acr" {
  name                = replace("acr${local.base_name}", "-", "")
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"
  admin_enabled       = false # sin usuario admin: acceso por identidad gestionada
  tags                = local.tags
}

# --- Observabilidad ---
resource "azurerm_log_analytics_workspace" "logs" {
  name                = "log-${local.base_name}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = local.tags
}

# --- Secretos (sin secretos en el repo) ---
resource "azurerm_key_vault" "kv" {
  name                       = "kv-${local.base_name}"
  resource_group_name        = azurerm_resource_group.main.name
  location                   = azurerm_resource_group.main.location
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  purge_protection_enabled   = true
  rbac_authorization_enabled = true
  tags                       = local.tags
}

data "azurerm_client_config" "current" {}

# --- Base de datos PostgreSQL gestionada ---
resource "azurerm_postgresql_flexible_server" "db" {
  name                   = "psql-${local.base_name}"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  version                = "16"
  administrator_login    = "copiloto"
  administrator_password = var.db_password # TF_VAR_db_password (nunca en el repo)
  sku_name               = "B_Standard_B1ms"
  storage_mb             = 32768
  zone                   = "1"
  tags                   = local.tags
}

# --- Cómputo gestionado: Container Apps ---
resource "azurerm_container_app_environment" "env" {
  name                       = "cae-${local.base_name}"
  resource_group_name        = azurerm_resource_group.main.name
  location                   = azurerm_resource_group.main.location
  log_analytics_workspace_id = azurerm_log_analytics_workspace.logs.id
  tags                        = local.tags
}

resource "azurerm_container_app" "api" {
  name                         = "ca-${local.base_name}"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.env.id
  revision_mode                = "Single"
  tags                         = local.tags

  template {
    min_replicas = 0
    max_replicas = 3

    container {
      name   = "api"
      image  = var.image
      cpu    = 0.5
      memory = "1Gi"

      env {
        name  = "COPILOTO_API_ENV"
        value = var.environment
      }
    }
  }

  ingress {
    external_enabled = true
    target_port      = 8000
    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }
}
