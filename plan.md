# Production-Grade Blazor Project Roadmap

> Path from demo/learning project to senior-level, production-ready application.

---

## Phase 1: Code Quality & Architecture (Priority: High) ✅ DONE

### 1.1 Dependency Injection & Service Layer ✅
- [x] Extract business logic from `@code` blocks into dedicated services
- [x] Create `Services/` folder with interfaces (e.g., `IFileUploadService`, `ICustomerService`)
- [x] Register services in `Program.cs` with appropriate lifetimes (`Scoped` vs `Singleton`)
- [x] Inject services into components instead of inline logic

**Why:** Separates concerns, makes code testable, follows SOLID principles.

### 1.2 Configuration Management ✅
- [x] Move hardcoded values (file size limits, allowed extensions) to `appsettings.json`
- [x] Create strongly-typed configuration classes (e.g., `UploadOptions`)
- [x] Use `IOptions<T>` pattern for accessing config in services
- [x] Add environment-specific configs (Development vs Production)

**Example:**
```csharp
// appsettings.json
"UploadOptions": {
  "MaxFileSize": 3145728,
  "MaxFileCount": 3,
  "AllowedExtensions": [".jpg", ".png"]
}
```

### 1.3 Error Handling Strategy ✅
- [x] Replace generic `try/catch` with structured exception handling
- [x] Create custom exception types (`FileUploadException`, `FileValidationException`)
- [x] Implement global error handler middleware
- [x] Add user-friendly error messages vs technical logs
- [x] Set up structured logging with `ILogger<T>` throughout

---

## Phase 2: Testing Excellence (Priority: High) ✅ DONE

### 2.1 Unit Test Coverage ✅
- [x] Aim for 70%+ code coverage
- [x] Add tests for all service classes
- [x] Mock external dependencies (file system, HTTP calls)
- [x] Test edge cases: empty files, invalid extensions, network failures

### 2.2 Component Tests ✅
- [x] Add bUnit tests for Blazor components
- [x] Test component rendering and event handling
- [x] Mock `IJSRuntime` for JavaScript interop tests

### 2.3 Integration Test Expansion ✅
- [x] Add authentication/authorization flow tests
- [x] Test file upload end-to-end with test files
- [x] Add database integration tests (if adding persistence)

### 2.4 Test Organization ✅
```
BlazorDemo.Tests/
├── UnitTests/
│   ├── Services/          ← FileValidation, FileUpload, FilePreview service tests
│   └── Models/            ← CustomerModel, UploadOptions, Exception, validation tests
├── ComponentTests/        ← bUnit tests (Counter, Weather, Home, NavMenu)
├── IntegrationTests/      ← HTTP endpoints, DI, config, E2E upload pipeline
└── TestHelpers/           ← FakeBrowserFile and shared test doubles
```

---

## Phase 3: Security Hardening (Priority: Critical)

### 3.1 File Upload Security ✅
- [x] **Server-side MIME type validation** (don't trust client `ContentType`)
- [x] **Magic number/file signature validation** (check actual file bytes)
- [x] **Scan uploads with ClamAV** or similar (for production)
- [x] **Store files outside wwwroot** (e.g., `App_Data/uploads/`)
- [x] Serve files via controller/handler with authorization checks
- [x] **Rate limiting** on upload endpoints

### 3.2 Authentication & Authorization ✅
- [x] **Add ASP.NET Core Identity** with SQLite database
- [x] **Implement `[Authorize]`** attributes on Home page
- [x] **Add claims-based authorization** ("CanUploadFiles" policy)
- [x] **Create Login/Register pages** with form validation
- [x] **Add logout functionality** with form POST
- [ ] Secure SignalR hubs with authorization (Phase 3.3)

### 3.3 Data Protection
- [ ] Use `IDataProtector` for sensitive temporary data
- [ ] Encrypt file names/metadata if they contain PII
- [ ] Implement anti-forgery token validation properly
- [ ] Add CORS configuration if API is consumed externally

### 3.4 Secrets Management
- [ ] Move secrets to `UserSecrets` (dev) or Azure Key Vault (prod)
- [ ] Never commit connection strings, API keys, or passwords
- [ ] Use environment variables for containerized deployments

---

## Phase 4: Database & Persistence (Priority: High)

### 4.1 Entity Framework Core Setup
- [ ] Add EF Core with SQL Server/PostgreSQL/SQLite
- [ ] Create `DbContext` with `Customer`, `FileUpload` entities
- [ ] Add migrations: `dotnet ef migrations add InitialCreate`
- [ ] Implement repository pattern (optional but recommended)

### 4.2 Data Model Expansion
```csharp
public class FileUpload
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = "";
    public string StoredFileName { get; set; } = "";
    public long FileSize { get; set; }
    public string ContentType { get; set; } = "";
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = "";
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}
```

### 4.3 Audit Trail
- [ ] Add `CreatedAt`, `ModifiedAt`, `CreatedBy`, `ModifiedBy` to all entities
- [ ] Use EF Core interceptors for automatic audit tracking

---

## Phase 5: API & Backend Expansion (Priority: Medium)

### 5.1 Minimal API Controllers
- [ ] Add API controllers for REST endpoints (`/api/customers`, `/api/uploads`)
- [ ] Implement proper HTTP status codes (201 Created, 400 Bad Request, etc.)
- [ ] Add request/response DTOs (Data Transfer Objects)
- [ ] Use `ProducesResponseType` attributes for Swagger documentation

### 5.2 Swagger/OpenAPI
- [ ] Add `Swashbuckle.AspNetCore` package
- [ ] Configure Swagger with XML comments
- [ ] Add authentication to Swagger UI for testing protected endpoints

### 5.3 Background Jobs (Optional)
- [ ] Add Hangfire or Quartz.NET for async processing
- [ ] Move file scanning/thumbnail generation to background jobs
- [ ] Add email notifications for upload completion

---

## Phase 6: DevOps & CI/CD (Priority: Medium)

### 6.1 GitHub Actions Pipeline
Create `.github/workflows/dotnet.yml`:
```yaml
name: .NET CI/CD
on: [push, pull_request]
jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore --configuration Release
      - name: Test
        run: dotnet test --no-build --verbosity normal
      - name: Publish
        if: github.ref == 'refs/heads/main'
        run: dotnet publish -c Release -o ./publish
```

### 6.2 Containerization
- [ ] Create `Dockerfile` for the application
- [ ] Create `docker-compose.yml` for local development (app + database)
- [ ] Use multi-stage builds for smaller images
- [ ] Add `.dockerignore` file

### 6.3 Deployment Targets
- [ ] Azure App Service (easiest for Blazor)
- [ ] Azure Container Apps (if using Docker)
- [ ] AWS ECS/EKS or GCP Cloud Run
- [ ] Self-hosted with Nginx reverse proxy

### 6.4 Infrastructure as Code
- [ ] Add Terraform or Bicep scripts for Azure resources
- [ ] Automate database provisioning

---

## Phase 7: Frontend Polish (Priority: Medium)

### 7.1 Component Library
- [ ] Replace inline styles with CSS variables/design tokens
- [ ] Create reusable components: `FileCard`, `UploadProgress`, `ErrorAlert`
- [ ] Add loading skeletons for better UX

### 7.2 Real-time Features
- [ ] Use SignalR for upload progress notifications
- [ ] Add toast notifications for success/error
- [ ] Implement optimistic UI updates

### 7.3 Accessibility (a11y)
- [ ] Add ARIA labels to all interactive elements
- [ ] Ensure keyboard navigation works
- [ ] Test with screen readers (NVDA, VoiceOver)
- [ ] Follow WCAG 2.1 AA guidelines

### 7.4 Responsive Design
- [ ] Test on mobile devices (file upload UX is tricky on mobile)
- [ ] Use Bootstrap breakpoints consistently
- [ ] Optimize image previews for smaller screens

---

## Phase 8: Observability (Priority: Medium)

### 8.1 Logging
- [ ] Structured logging with Serilog
- [ ] Log to console (dev), file (staging), and cloud (production)
- [ ] Add correlation IDs for request tracing

### 8.2 Metrics & Monitoring
- [ ] Add Application Insights or OpenTelemetry
- [ ] Track: upload count, failure rate, file size averages
- [ ] Set up alerts for error rate thresholds

### 8.3 Health Checks
- [ ] Add `/health` endpoint
- [ ] Check database connectivity
- [ ] Check file system write access
- [ ] Use for load balancer health probes

---

## Phase 9: Performance (Priority: Low-Medium)

### 9.1 File Upload Optimization
- [ ] Implement chunked upload for large files
- [ ] Add client-side compression before upload (optional)
- [ ] Use streaming to avoid loading entire file into memory

### 9.2 Caching
- [ ] Add in-memory caching for frequently accessed data
- [ ] Use Redis for distributed caching (multi-instance deployments)
- [ ] Cache file metadata, not the files themselves

### 9.3 Database Optimization
- [ ] Add database indexes on foreign keys and search fields
- [ ] Implement pagination for file lists
- [ ] Use async/await consistently for all I/O operations

---

## Phase 10: Documentation (Priority: Low, but ongoing)

### 10.1 Code Documentation
- [ ] Add XML documentation comments to public APIs
- [ ] Document complex business logic with comments
- [ ] Maintain CHANGELOG.md for releases

### 10.2 Architecture Decision Records (ADRs)
Create `docs/adr/` folder with decisions like:
- Why Interactive Server vs WebAssembly
- Why EF Core vs Dapper
- File storage strategy (local vs cloud)

### 10.3 Runbooks
- [ ] Deployment procedures
- [ ] Rollback procedures
- [ ] Incident response playbooks

---

## Quick Wins (Do These First)

1. **Add EF Core + database** — currently data is lost on restart
2. **Move file storage out of wwwroot** — security risk
3. **Add authentication** — know who uploaded what
4. **Implement service layer** — separate UI from business logic
5. **Add GitHub Actions** — catch issues before merging

---

## Recommended Learning Path

| Week | Focus | Outcome |
|------|-------|---------|
| 1 | EF Core + database | Persistent data storage |
| 2 | Service layer + DI | Clean architecture |
| 3 | Authentication | Secure login system |
| 4 | API controllers + Swagger | Professional API surface |
| 5 | Docker + CI/CD | Automated deployment |
| 6 | Security hardening | Production-ready app |

---

## Senior Developer Checklist

Before calling it "production-ready":

- [ ] Code coverage > 70%
- [ ] No secrets in source control
- [ ] Database migrations automated
- [ ] Health checks passing
- [ ] Logging configured
- [ ] Error handling tested
- [ ] Security scan passed (OWASP ZAP or similar)
- [ ] Load testing completed
- [ ] Documentation complete
- [ ] Runbook written
- [ ] Team can deploy without you

---

*Last updated: 2026-04-20*
*Next review: After Phase 1 completion*
