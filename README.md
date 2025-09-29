# DroneComply

DroneComply is a WinUI 3 application that helps Part 107 operators stay compliant by centralising pre-flight planning, flight logging, maintenance tracking, waiver management, and weather intelligence.

## Projects

- src/DroneComply.App - WinUI 3 shell, MVVM view models, navigation, and dependency injection entry point.
- src/DroneComply.Core - Domain models, enums, interfaces, and business services.
- src/DroneComply.Data - Entity Framework Core context, configurations, and repository implementations for encrypted SQLite storage.
- src/DroneComply.External - HTTP abstractions for FAA weather and LAANC integrations.
- tests/DroneComply.Tests - xUnit test project (placeholder).

## Getting Started

1. Install the prerequisites:
   - .NET 8 SDK (winget install Microsoft.DotNet.SDK.8)
  
   - Windows App SDK 1.5 (winget install Microsoft.WindowsAppSDK)
   - WinUI 3 tooling (dotnet workload install maui-windows or install VS 2022 with the "Windows App SDK C# Templates" workload)
   - Entity Framework Core tools (dotnet tool install --global dotnet-ef)

2. Restore and build the solution:

   ```powershell
   dotnet restore
   dotnet build
   ```

3. Configure secrets via environment variables (they are read at runtime):
   - `DRONECOMPLY_DB_PASSWORD` for the encrypted SQLite database (optional; omit to run without encryption).
   - `DRONECOMPLY_ALOFT_API_KEY` for live LAANC advisories.
   - `DRONECOMPLY_GOOGLE_MAPS_API_KEY` for mapping features.

   You can add environment-specific overrides in `appsettings.Development.json` or `appsettings.Production.json`, but avoid storing secrets in source-controlled JSON files.

4. Apply migrations when they are added (placeholder):

   ```powershell
   dotnet ef migrations add InitialCreate --project src/DroneComply.Data --startup-project src/DroneComply.App
   dotnet ef database update --project src/DroneComply.Data --startup-project src/DroneComply.App
   ```

5. Run the app:

   ```powershell
   dotnet run --project src/DroneComply.App
   ```

## Next Steps

- Flesh out DTOs in DroneComply.Data.Entities (currently everything maps to the domain models directly).
- Add migrations and seed data for common FAA waivers, mission templates, and maintenance schedules.
- Hook up real FAA weather and LAANC providers.
- Expand the test suite with unit tests for the services and repositories.
- Add MSIX packaging and GitHub Actions workflows (folders are ready but empty).

## Repository Conventions

- MVVM with CommunityToolkit.Mvvm
- Dependency injection via Host.CreateDefaultBuilder
- Logging through Serilog (JSON configuration)
- Repositories expose async operations returning immutable collections
- External services keep network logic isolated from the core domain

