# Prerequisites

Before running the application, ensure the following are installed:

- **Docker**: [Docker Desktop](https://www.docker.com/products/docker-desktop/) for containerized deployment.
- **.NET SDK**: Version 8.0 or later for ASP.NET Core and .NET Aspire. Download from [Microsoft](https://dotnet.microsoft.com/download).
- **Git**: To clone the repository.
- **Code Editor**: (Optional) [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/) for development.

## Getting Started

### Running with Docker

The `TendersAPI` project includes a `Dockerfile` in the `TendersAPI/TendersAPI` directory for containerized deployment.

1. **Ensure Docker is running**:
   - Verify that Docker Desktop is running on your machine.

2. **Build the Docker image**:
   - Navigate to the `TendersAPI` directory:

     ```powershell
     Set-Location TendersAPI
     ```

   - Build the Docker image:

     ```powershell
     docker build -t tendersapi:latest .
     ```

     The `Dockerfile` uses the `mcr.microsoft.com/dotnet/aspnet:8.0` base image, restores dependencies, builds the project, and exposes port 8080.

3. **Run the Docker container**:
   - Start the container, mapping port 8080:

     ```powershell
     docker run -d -p 8080:8080 --name tendersapi tendersapi:latest
     ```

   - Access the API at `http://localhost:8080`. For HTTPS, check `appsettings.json` or logs for the port (e.g., 8081).

4. **Environment Variables**:
   - Pass required configurations (e.g., database connection strings) via environment variables:

     ```powershell
     docker run -d -p 8080:8080 -e "ConnectionStrings__DefaultConnection=your_connection_string" --name tendersapi tendersapi:latest
     ```

   - Review `TendersAPI/appsettings.json` for required settings.

5. **Stop the container**:
   - Stop the running container:

     ```powershell
     docker stop tendersapi
     ```

### Running with .NET Aspire

The `TendersAPI.AppHost` project in `TendersAPI/TendersAPI.AppHost` uses .NET Aspire to orchestrate the `TendersAPI` application and its dependencies (e.g., databases, caching services).

1. **Install .NET Aspire**:
   - Ensure the .NET 8.0 SDK is installed.
   - Install the .NET Aspire workload:

     ```powershell
     dotnet workload install aspire
     ```

2. **Navigate to the AppHost project**:
   - Move to the `TendersAPI.AppHost` directory:

     ```powershell
     Set-Location TendersAPI.AppHost
     ```

3. **Restore dependencies**:
   - Restore NuGet packages:

     ```powershell
     dotnet restore
     ```

4. **Run the Aspire orchestrator**:
   - Start the orchestrator to launch the API and dependencies:

     ```powershell
     dotnet run
     ```

   - Access the Aspire dashboard at `http://localhost:18888` (port may vary; check console output).
   - The API is typically available at `http://localhost:8080` or `https://localhost:8081` (verify in `TendersAPI.AppHost/Program.cs`).

5. **Configuration**:
   - Update `TendersAPI/appsettings.json` or environment variables for required settings, e.g., database connection:

     ```json
     "ApiSettings": {
      "BaseUrl": "https://tenders.guru/api/pl/tenders",
      "ConcurrencyLimit":  10
      }
     ```

   - Check `TendersAPI.AppHost/Program.cs` for additional services (e.g., Redis, SQL Server) and their configurations.

6. **Stop the application**:
   - Press `Ctrl+C` in the terminal to stop the orchestrator.
