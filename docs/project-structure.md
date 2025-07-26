# Project Structure

The AudiSense Solution implements a multi-client architecture supporting both WPF desktop and Blazor WASM web applications for audio and hearing-related functionality.

## Project Dependencies

### Client Applications
- Both WPF and Blazor applications depend on `AudiSense.Client.Shared`
- Platform-specific implementations reference only their respective core libraries
- Feature modules can be shared between platforms where appropriate

### Shared Layer
- Contains platform-agnostic code used by both client and server
- No dependencies on client- or server-specific code
- Implements core domain model and business rules

### Server Layer
- Implements REST API and application services
- Dependencies flow inward following Clean Architecture principles
- No direct dependencies on client-specific code

## Development Guidelines

1. New features should be implemented first in the shared layer when possible
2. Platform-specific code should be minimal and focused on UI concerns
3. Use dependency injection for service resolution
4. Follow feature-based organization for new functionality
5. Maintain clear separation between platform-specific and shared code

## Testing Strategy

1. Unit tests for shared business logic
2. Integration tests for API endpoints
3. Platform-specific UI tests
4. End-to-end tests for critical user journeys

All test projects are located under `src/Server/AudiSense.Tests/` as shown in the directory structure below.

## Directory Structure

```plaintext
src/
+-- Client/
|   +-- AudiSense.Client.Shared/           # Shared client-side components
|   |   +-- Models/                       # View models and DTOs
|   |   +-- Services/                     # Platform-agnostic services
|   |   |   +-- Interfaces/              # Service contracts
|   |   |   +-- Implementation/          # Shared implementations
|   |   +-- Features/                    # Shared feature implementations
|   |   +-- Extensions/                  # Common extension methods
|   |
|   +-- Platforms/
|   |   +-- Blazor/
|   |   |   +-- AudiSense.Blazor.App/     # Blazor WASM host
|   |   |   +-- AudiSense.Blazor.Core/    # Blazor-specific implementations
|   |   |   +-- AudiSense.Blazor.Components/ # Reusable Blazor components
|   |   |
|   |   +-- WPF/
|   |       +-- AudiSense.Wpf.App/        # WPF host application
|   |       +-- AudiSense.Wpf.Core/       # WPF-specific implementations
|   |       +-- AudiSense.Wpf.Controls/   # Reusable WPF controls
|   |
|   +-- Features/                        # Feature modules
|       +-- Authentication/              # Authentication feature
|       +-- HearingMeasurements/        # Hearing measurements feature
|       +-- TaskManagement/             # Task management feature
|       +-- Reporting/                  # Reporting and analytics
|
+-- Shared/
|   +-- AudiSense.Domain/                # Domain models and logic
|   |   +-- Entities/                   # Core business entities
|   |   +-- ValueObjects/               # Value objects
|   |   +-- Events/                     # Domain events
|   |
|   +-- AudiSense.Contracts/            # Service contracts and DTOs
|   |   +-- Requests/                  # API request models
|   |   +-- Responses/                 # API response models
|   |   +-- Interfaces/                # Service interfaces
|   |
|   +-- AudiSense.Common/               # Cross-cutting concerns
|       +-- Constants/                 # Shared constants
|       +-- Extensions/                # Shared extensions
|       +-- Utilities/                 # Common utilities
|
+-- Server/
    +-- AudiSense.Api/                  # REST API host
    |   +-- Controllers/               # API endpoints
    |   +-- Configuration/             # API configuration
    |
    +-- AudiSense.Application/          # Application layer services
    |   +-- Services/                  # Business logic services
    |   +-- Validators/                # Request validators
    |   +-- Mappings/                  # Object mappings
    |
    +-- AudiSense.Infrastructure/       # External concerns
    |   +-- Persistence/              # Data access
    |   +-- Security/                 # Authentication/Authorization
    |   +-- Integration/              # External services
    |
    +-- AudiSense.Tests/               # Test projects
        +-- Unit/                     # Unit tests
        +-- Integration/              # Integration tests
        +-- E2E/                     # End-to-end tests
```