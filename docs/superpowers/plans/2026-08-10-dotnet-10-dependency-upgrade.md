# .NET 10 and Dependency Upgrade Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Move the solution from .NET 8 to .NET 10 and update every direct NuGet dependency to the latest stable version available on 2026-08-10 while preserving the existing application and test behavior.

**Architecture:** Keep the existing six-project Clean Architecture solution and explicit per-project PackageReference layout. Align runtime, SDK, Docker, GitHub Actions, and documentation references with net10.0, then validate the complete solution through restore, build, unit tests, and integration tests.

**Tech Stack:** .NET 10 SDK/runtime, ASP.NET Core 10, C#, NuGet PackageReference, Docker, GitHub Actions, xUnit v3, Coverlet.

## Global Constraints

- Target every project at `net10.0`.
- Use stable package versions only; do not select prerelease versions.
- Keep package changes limited to direct PackageReferences already present in the solution, except replacing deprecated `xunit` v2 with stable `xunit.v3`.
- Preserve existing test commands and Docker/PostgreSQL integration behavior.

---

### Task 1: Upgrade project targets and direct dependencies

**Files:**
- Modify: `global.json`
- Modify: `src/main/Core/Core.csproj`
- Modify: `src/main/EntryPoint.WebApi/EntryPoint.WebApi.csproj`
- Modify: `src/main/Infra.PostgreSql/Infra.PostgreSql.csproj`
- Modify: `src/test/UnitTest.Core/UnitTest.Core.csproj`
- Modify: `src/test/IntegrationTest.EntryPoint.WebApi/IntegrationTest.EntryPoint.WebApi.csproj`
- Modify: `src/test/IntegrationTest.Infra.PostgreSql/IntegrationTest.Infra.PostgreSql.csproj`

**Interfaces:**
- Consumes: Existing project references, test source, and PackageReference metadata.
- Produces: A solution targeting `net10.0` with stable versions: FluentValidation 12.1.1, Newtonsoft.Json.Schema 4.0.1, Microsoft.AspNetCore.OpenApi 10.0.10, Swashbuckle.AspNetCore and Annotations 10.2.3, System.Text.Json 10.0.10, Dapper 2.1.79, Dapper.SqlBuilder 2.1.66, Npgsql and Npgsql.DependencyInjection 10.0.3, Coverlet packages 10.0.1, Microsoft.NET.Test.Sdk 18.8.1, Moq 4.20.72, xunit.v3 3.2.2, xunit.runner.visualstudio 3.1.5, Ductus.FluentDocker 2.85.0, and Microsoft.Extensions.Configuration.Json 10.0.10.

- [ ] Update all `<TargetFramework>` values to `net10.0` and set `global.json` to a valid .NET 10 SDK feature-band version with prerelease SDKs disabled.
- [ ] Update direct PackageReference versions and replace `xunit` with `xunit.v3` in all test projects.
- [ ] Keep existing PrivateAssets and IncludeAssets settings for test-only packages.

### Task 2: Align containers, CI, and documentation

**Files:**
- Modify: `Dockerfile`
- Modify: `.github/workflows/continuous-integration.yml`
- Modify: `.github/workflows/mutation-testing.yml`
- Modify: `.github/workflows/sonar-analysis.yml`
- Modify: `README.md`

**Interfaces:**
- Consumes: The `net10.0` project targets from Task 1.
- Produces: Docker builds using .NET 10 images, CI setup/hack paths using .NET 10, and documentation that advertises .NET 10 commands and prerequisites.

- [ ] Change ASP.NET and SDK Docker image tags from `8.0` to `10.0`.
- [ ] Change GitHub Actions setup labels, SDK versions, and generated `bin` paths from 8.0/net8.0 to 10.0/net10.0.
- [ ] Update README references from .NET Core 8/.NET 8 and `net8.0` output paths to .NET 10/net10.0.

### Task 3: Restore and verify compatibility

**Files:**
- Modify: Any source or test file required by compiler/test failures caused by the .NET 10 or xUnit v3 upgrade.

**Interfaces:**
- Consumes: The upgraded projects and infrastructure from Tasks 1–2.
- Produces: A restored, compiled, and tested solution with no avoidable compatibility regressions.

- [ ] Run `dotnet restore --force --no-cache`.
- [ ] Run `dotnet build transactions-api-netcore.sln --configuration Debug --no-restore`.
- [ ] Run each existing test project with `dotnet test --no-build --nologo -v n`; start the PostgreSQL test container when integration tests require it.
- [ ] If a failure is caused by the dependency migration, add or adjust the smallest required compatibility change and rerun the affected verification before the full suite.

### Task 4: Final review

**Files:**
- Review: `git diff --check`, all changed project/configuration files, and generated build outputs excluded from version control.

- [ ] Confirm no net8.0, .NET 8, SDK 8.0.x, or stable outdated direct PackageReference remains in tracked project/configuration files.
- [ ] Run `git diff --check` and inspect `git status --short`.
- [ ] Report exact changed files, package versions, and fresh verification results.
