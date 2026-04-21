# Project Setup Guide

## Development Environment Setup

### 1. Install .NET 10.0 SDK
Download and install from: https://dotnet.microsoft.com/download

### 2. Configure User Secrets (Required)

The application requires a connection string stored in User Secrets for development.

**Initialize User Secrets (one-time):**
```bash
cd BlazorDemo
dotnet user-secrets init
```

**Set the connection string:**
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=BlazorDemo.db"
```

**Verify your secrets:**
```bash
dotnet user-secrets list
```

Expected output:
```
ConnectionStrings:DefaultConnection = Data Source=BlazorDemo.db
```

### 3. Run Database Migrations

```bash
cd BlazorDemo
dotnet ef database update
```

### 4. Run the Application

```bash
cd BlazorDemo
dotnet run
```

The app will be available at:
- http://localhost:5000
- https://localhost:5001 (if HTTPS is configured)

## Production/Containerized Deployment

### Environment Variables

For production or containerized deployments, use environment variables instead of User Secrets:

```bash
# Linux/macOS
export ConnectionStrings__DefaultConnection="Data Source=/app/data/BlazorDemo.db"

# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="Data Source=C:\app\data\BlazorDemo.db"
```

### Docker Example

```dockerfile
# Dockerfile
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/BlazorDemo.db"
```

### Azure / Cloud Deployment

Use Azure Key Vault or your cloud provider's secret management service:
- Azure: Configure as Key Vault secrets
- AWS: Use AWS Secrets Manager
- GCP: Use Secret Manager

## Never Commit Secrets

⚠️ **IMPORTANT:** Never commit the following to git:
- Connection strings with passwords
- API keys
- Private tokens
- Certificates or .pfx files

These should always be stored in:
- User Secrets (development)
- Environment variables (containers)
- Azure Key Vault / AWS Secrets Manager (production)
