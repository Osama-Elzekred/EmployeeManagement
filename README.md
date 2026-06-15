# Employee Management System

A simple yet powerful employee and department management application built with .NET Core and a clean, modern web interface.

## What Does This Do?

This app lets you manage employees and departments for your organization. You can:
- View all employees and departments
- Add new employees or departments
- Edit existing records
- Delete records when you don't need them anymore
- Search and filter by name or department
- See real-time updates on the dashboard

## Quick Start

### Prerequisites
- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download)
- **SQL Server Express** - Make sure it's running on your machine

### Running the Project

It's super simple - just three steps:

1. **Open your terminal** and navigate to the project folder:
   ```bash
   cd "src/EmployeeManagement.API"
   ```

2. **Start the application:**
   ```bash
   dotnet run --project src/EmployeeManagement.API
   ```

3. **Open your browser** and go to:
   ```
   http://localhost:5121
   ```

That's it! The app will automatically set up the database for you on first run.

## What Happens When You Start?

When the application starts, it automatically:
- Creates the database (if it doesn't exist)
- Sets up all the tables
- Adds some default departments (Engineering, HR, Finance, etc.)
- Runs the website locally on port 5121

## Project Structure

```
src/
├── EmployeeManagement.Domain/        # Core data models
├── EmployeeManagement.Application/   # Business logic & validation
├── EmployeeManagement.Infrastructure/# Database & data access
└── EmployeeManagement.API/           # The web server & website
```

## Technology Stack

### Backend
- **Runtime**: .NET 10
- **Framework**: ASP.NET Core
- **Database**: SQL Server with Entity Framework Core (Code-First)
- **Validation**: FluentValidation for data validation
- **Logging**: Serilog for application logging

### Frontend
- **Language**: Vanilla JavaScript (no frameworks)
- **Styling**: Tailwind CSS with Material Design 3
- **Design**: Material Symbols icons
- **Pattern**: Pure HTML/CSS/JS for lightweight performance

### API & Tooling
- **Documentation**: Scalar UI for interactive API exploration
- **Health Checks**: Built-in application health monitoring
- **Middleware**: Custom request logging for performance tracking

## Architecture & Design Patterns

### Clean Architecture
The application is built using **Clean Architecture** principles, separating concerns into distinct layers:

- **Domain Layer**: Contains pure business entities and interfaces (no framework dependencies)
- **Application Layer**: Handles business logic, validation, and DTOs
- **Infrastructure Layer**: Manages database access and external services
- **API Layer**: Exposes REST endpoints and hosts the frontend

### Key Design Patterns Used

**1. Result<T> Pattern**
- Replaces exceptions for predictable error handling
- Provides success/failure states with typed data
- Supports error lists for comprehensive validation feedback

**2. Unit of Work Pattern**
- Centralizes repository management
- Ensures consistent database transactions
- Single source of truth for data access

**3. Generic Repository Pattern**
- Eliminates repetitive data access code
- Provides common CRUD operations out of the box
- Makes testing easier with mock repositories

**4. ApiResponseFilter**
- Automatic response wrapping in consistent envelope format
- Unified error handling across all endpoints
- Converts domain exceptions to API-friendly responses

**5. Dependency Injection**
- All services and repositories injected at startup
- Makes code testable and loosely coupled
- Configured in `Program.cs`

### Advanced Features

- **Soft Delete**: Entities can be marked as deleted without removal from database
- **Audit Trail**: Automatic tracking of `CreatedAt` and `UpdatedAt` timestamps
- **Global Exception Handling**: Catches and formats all unhandled exceptions
- **Validation Errors as Lists**: Instead of concatenated strings, validation errors return structured arrays
- **Async/Await**: Non-blocking database operations throughout

## Useful URLs

- **Main App**: [http://localhost:5121](http://localhost:5121)
- **API Docs**: [http://localhost:5121/scalar/v1](http://localhost:5121/scalar/v1)

## Built With

- **.NET 10** - The backend framework
- **Entity Framework Core** - Database management
- **SQL Server** - Database
- **Vanilla JavaScript** - No heavy frameworks, just clean JavaScript
- **HTML/CSS** - Modern, responsive design with Tailwind CSS
