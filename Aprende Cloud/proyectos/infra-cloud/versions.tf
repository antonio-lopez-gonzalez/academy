terraform {
  required_version = ">= 1.6"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.110"
    }
  }
  # Backend remoto (recomendado en equipos): estado en Blob Storage con locking.
  # Descomenta y rellena (o usa `terraform init -backend-config=...`):
  # backend "azurerm" {
  #   resource_group_name  = "tfstate-rg"
  #   storage_account_name = "tfstateacct"
  #   container_name       = "tfstate"
  #   key                  = "copiloto.tfstate"
  # }
}

provider "azurerm" {
  features {}
}
