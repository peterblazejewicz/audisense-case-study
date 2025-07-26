# AudiSense - Audio & Hearing Management System

AudiSense is a study case application for hearing tests built with WPF and .NET 9.0. This application serves hearing tests to users, allowing them to conduct hearing tests which are saved to the system and can be reviewed and updated later. The solution implements a multi-client architecture with a REST API backend and WPF desktop client.

## Project Structure

```
src/
├── AudiSense.sln                           # Solution file
├── Client/
│   ├── AudiSense.Client.Shared/             # Shared client-side components
│   │   ├── Models/                         # View models and DTOs
│   │   ├── Services/                       # Platform-agnostic services
│   │   │   ├── Interfaces/                 # Service contracts
│   │   │   └── Implementation/             # Shared implementations
│   │   ├── Features/                       # Shared feature implementations
│   │   └── Extensions/                     # Common extension methods
│   │
│   └── Platforms/
│       └── WPF/
│           ├── AudiSense.Wpf.App/           # WPF host application
│           ├── AudiSense.Wpf.Core/          # WPF-specific implementations
│           └── AudiSense.Wpf.Controls/      # Reusable WPF controls
│
├── Shared/                                 # (Future) Shared domain layer
└── Server/                                 # (Future) API and backend services
```

## Features

### Current Implementation

- **Modern WPF UI**: Clean, modern interface with Material Design color scheme
- **REST API Backend**: Full CRUD operations for hearing tests with Entity Framework Core
- **Dependency Injection**: Full DI container setup with Microsoft.Extensions
- **Configuration**: JSON-based configuration system
- **Modular Architecture**: Clean separation between client, server, and shared layers
- **Custom Controls**: Hearing test forms and management interfaces

### Navigation Modules

- **Dashboard**: Main overview and statistics
- **Hearing Tests**: Audio testing functionality
- **Audio Analysis**: Audio data analysis tools
- **Reports**: Generate and view reports
- **Patients**: Patient management system

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Windows 10/11 (for WPF application)
- Visual Studio 2022 or Visual Studio Code (optional)

### Building the Application

1. Clone the repository
2. Navigate to the `src` directory
3. Restore dependencies and build:

```bash
dotnet restore AudiSense.sln
dotnet build AudiSense.sln
```

### Running the Application

**Important:** The API service must be started before running the WPF application.

1. **Start the API service first:**

```bash
dotnet run --project Server/AudiSense.Api
```

The API will be available at:
- HTTPS: https://localhost:51703
- HTTP: http://localhost:51704
- Swagger UI: https://localhost:51703/swagger

2. **Then run the WPF application:**

```bash
dotnet run --project Client/Platforms/WPF/AudiSense.Wpf.App
```

## Architecture

### Dependency Injection

The application uses Microsoft.Extensions.DependencyInjection with the following services:

- **MainWindow**: Singleton window instance
- **IDataService**: Abstraction for data access (WPF-specific implementation)
- **Logging**: Serilog integration for structured logging
- **Configuration**: JSON configuration with environment overrides

### Configuration

Application settings are stored in `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:51703/api",
    "Timeout": 30
  },
  "ApplicationSettings": {
    "Theme": "Light",
    "AutoSave": true,
    "DataPath": "./Data"
  }
}
```

### Custom Controls

The following custom WPF controls are currently implemented:

#### HearingTestForm

A user control for conducting and editing hearing tests with the following features:

- Input fields for tester name, date, and test results
- Form validation and data binding
- Save/Cancel operations with API integration

#### HearingTestsList

A user control for displaying and managing hearing tests with the following features:

- List view of all hearing tests
- Edit and delete operations
- Integration with the API for CRUD operations

## Development Guidelines

1. **Shared First**: Implement new features in the shared layer when possible
2. **Platform Specific**: Keep platform-specific code minimal and focused on UI concerns
3. **Dependency Injection**: Use DI for service resolution
4. **Feature Organization**: Organize code by features rather than technical layers
5. **Clean Architecture**: Maintain clear separation between layers

## Testing Strategy

- **Unit Tests**: Shared business logic
- **Integration Tests**: API endpoints (when implemented)
- **Platform Tests**: UI-specific functionality
- **End-to-End Tests**: Critical user journeys

## Future Enhancements

- **Blazor WASM Client**: Web-based version of the application
- **Real Audio Processing**: Integration with audio libraries for actual hearing tests
- **Persistent Database**: Replace in-memory database with SQL Server/SQLite
- **Patient Management**: Complete patient records and history tracking
- **Multi-language Support**: Localization framework
- **Themes**: Dark/light theme switching
- **Advanced Reporting**: Charts, graphs, and detailed analysis reports

## Contributing

1. Follow the existing project structure
2. Use consistent naming conventions
3. Add appropriate logging and error handling
4. Update documentation for new features
5. Ensure compatibility with the multi-platform architecture

## License

Copyright © AudiSense 2025
