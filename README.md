# Warehouse App

A simple console application for managing warehouse boxes and pallets, built with .NET 9, EF Core (SQLite) and Spectre.Console.

---

## Features

- **Domain model**  
  - `StorageItem` – base class  
  - `Box` – dimensions, weight, manufacture/expiration dates  
  - `Pallet` – holds multiple boxes, computed volume/expiration  

- **Persistence & infrastructure**  
  - EF Core `DbContext` + SQLite (`warehouse.db`)  
  - Automatic migrations on startup  
  - Generic repositories (`IBoxRepository`, `IPalletRepository`)  

- **Application services**  
  - `IWarehouseService` for business logic  
  - Group pallets by expiration, get top-3 by box expiry, create/read operations  

- **Console UI**  
  - Interactive menus via Spectre.Console  
  - Commands pattern (`IMenuCommand`)  
  - Data entry for boxes & pallets  

- **Testing**  
  - **Integration tests** with EF Core In-Memory provider  
  - **Unit tests** mocking repositories with Moq  

- **CI/CD**  
  - GitHub Actions (`.github/workflows/dotnet-ci.yml`)  
  - Restores, builds, analyzes, scripts migrations, runs tests  

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Optional: a terminal that supports UTF-8 (for Spectre.Console)

### Clone & Build

```bash
git clone https://github.com/GeoRuby07/warehouse-app.git
cd warehouse-app
dotnet restore
dotnet build --configuration Release