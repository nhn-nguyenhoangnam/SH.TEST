## 🛠️ Technology Stack

### Backend
- **.NET 9.0** - Latest LTS version

### Frontend
- **Blazor WebAssembly** - Client-side SPA framework

### Infrastructure
- **.NET Aspire** - Orchestration and service discovery
- **SQL Server 2022** - Database (containerized)
- **Docker** - Container runtime

## 📋 Prerequisites

Before running the application, ensure you have the following installed:

1. **.NET 9.0 SDK** (version 9.0.307 or later)
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verify: `dotnet --version`

2. **Docker Desktop**
   - Download: https://www.docker.com/products/docker-desktop
   - Required for SQL Server container
   - Verify: `docker --version`

3. **.NET Aspire Workload**
   ```bash
   dotnet workload install aspire
   ```

4. **IDE (Optional but recommended)**
   - Visual Studio 2022 (v17.12+) with ASP.NET workload
   - JetBrains Rider 2024.3+
   - Visual Studio Code with C# Dev Kit

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/nhn-nguyenhoangnam/SH.TEST.git
cd BankingApp
```

### 2. Start Docker Desktop

Ensure Docker Desktop is running, as the SQL Server container will be automatically started by Aspire.

### 3. Run the Application

The easiest way to run the entire application is using the AppHost:

```bash
dotnet run --project BankingApp.AppHost
```

This single command will:
- ✅ Start SQL Server 2022 container (port 9433)
- ✅ Create database and run migrations
- ✅ Start the API project
- ✅ Start the Blazor Web project
- ✅ Open Aspire Dashboard in your browser

### 4. Access the Applications

After running the AppHost, you'll see the Aspire Dashboard open automatically:

- **Aspire Dashboard**: `https://localhost:17016` (default)
  - Monitor all services, logs, and traces
  - View service health and resource usage

From the dashboard, you can access:

- **Blazor Web UI**: Check the dashboard for the assigned port (typically `http://localhost:6001`)
- **API Swagger**: Check the dashboard for the assigned port (typically `http://localhost:5002/docs/index.html`)

### 5. Database Setup

The database is automatically configured:
- **SQL Server**: Runs in Docker container on port `9433`
- **Database Name**: `BankingDb`
- **Password**: `PassWord123456`
- **Migrations**: Applied automatically on startup
- **Seed Data**: 10 sample accounts are pre-loaded

### Sample Accounts

The application comes with 10 pre-seeded accounts:

| Account Number | Account Holder      | Balance (VNĐ) |
|---------------|---------------------|---------------|
| ACC001        | John Smith          | 5,000,000     |
| ACC002        | Mary Johnson        | 10,000,000    |
| ACC003        | Robert Williams     | 7,500,000     |
| ACC004        | Patricia Brown      | 15,000,000    |
| ACC005        | Michael Davis       | 3,000,000     |
| ACC006        | Jennifer Miller     | 20,000,000    |
| ACC007        | William Wilson      | 12,000,000    |
| ACC008        | Linda Moore         | 8,500,000     |
| ACC009        | David Taylor        | 6,000,000     |
| ACC010        | Elizabeth Anderson  | 25,000,000    |

## 🔧 Manual Setup (Alternative)

If you prefer to run services individually:

### 1. Start SQL Server Container

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 9433:1433 --name sqlserver2022 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Update Connection String

Edit `BankingApp.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "AppDb": "Server=localhost,9433;Database=BankingDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

### 3. Run Migrations

```bash
cd BankingApp.Infra
dotnet ef database update --startup-project ../BankingApp.Api
```

### 4. Run API

```bash
cd BankingApp.Api
dotnet run
```

### 5. Run Web UI

```bash
cd BankingApp.Web
dotnet run
```

## 🔄 Development Workflow

### Adding a Migration

```bash
# Navigate to the infrastructure project
cd BankingApp.Infra

# Create new migration
dotnet ef migrations add MigrationName --startup-project ../BankingApp.Api

# Apply migration
dotnet ef database update --startup-project ../BankingApp.Api
```

Or use the provided script:

```bash
sh add-migration.sh MigrationName
```