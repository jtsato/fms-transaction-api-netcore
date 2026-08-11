# Resolve Sonar Issues Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Resolve all 17 findings from the attached Sonar issue export while preserving application behavior and keeping generated mutation reports compliant on future CI runs.

**Architecture:** Treat the four checked-in mutation reports as generated documentation. Add accessibility metadata after Stryker copies reports in CI, and update the current reports so the repository is clean immediately. Apply only compiler/static-analysis-safe C# cleanups: remove redundant null-forgiving operators, use `EqualityComparer<T>` for generic hash-code handling, and match existing attribute metadata conventions.

**Tech Stack:** .NET 10, C#, xUnit, GitHub Actions YAML, Stryker.NET HTML reports.

## Global Constraints

- Do not alter the existing uncommitted changes in `README.md`, `.github/workflows/sonar-analysis.yml`, or `.github/workflows/continuous-integration.yml`.
- Preserve runtime behavior; changes are limited to static-analysis cleanup and generated-report metadata.
- Keep mutation reports valid HTML with `lang="en"` and a meaningful `<title>`.
- Verify with focused source tests, workflow/report assertions, and whitespace/diff checks.

### Task 1: Make mutation-report accessibility fixes durable

**Files:**
- Modify: `.github/workflows/mutation-testing.yml` after the three Stryker copy steps.
- Modify: `docs/mutation-reports/Core/mutation-report.html`.
- Modify: `docs/mutation-reports/EntryPoint.WebApi/mutation-report.html`.
- Modify: `docs/mutation-reports/Infra.PostgreSql/mutation-report.html`.
- Modify: `docs/mutation-reports/Infra.MongoDB/mutation-report.html`.

- [x] **Step 1: Add a CI normalization step**

After all report copy steps, iterate over `./docs/mutation-reports/**/*.html`, add `lang="en"` to `<html>`, and add `<title>Mutation Test Report</title>` when a title is absent. Make the shell logic idempotent so repeated scheduled runs do not duplicate metadata.

- [x] **Step 2: Update the four checked-in reports**

Change each report header from `<html>` to `<html lang="en">` and insert `<title>Mutation Test Report</title>` in `<head>` after the charset metadata.

- [x] **Step 3: Verify report metadata**

Run a PowerShell assertion over all four reports that exactly one `<title>` exists and that the root HTML element contains `lang="en"`. Confirm no report header remains in the old form.

### Task 2: Resolve C# Sonar findings without behavior changes

**Files:**
- Modify: `src/main/Core/Commons/ArgumentValidator.cs` to remove two redundant null-forgiving operators.
- Modify: `src/main/Core/Commons/Models/Range.cs` to use `EqualityComparer<T>.Default.GetHashCode` for null-safe generic hash codes.
- Modify: `src/main/EntryPoint.WebApi/Commons/Filters/GetLanguageActionFilterAttribute.cs` to add the established `AttributeUsage` declaration.
- Modify: `src/main/EntryPoint.WebApi/Commons/Filters/HandleInvalidModelStateActionFilterAttribute.cs` to add the established `AttributeUsage` declaration.
- Modify: `src/main/EntryPoint.WebApi/Commons/OrderByHelper.cs` to remove two redundant null-forgiving operators.
- Modify: `src/main/EntryPoint.WebApi/Program.cs` to remove the redundant null-forgiving operator on `HttpMethod`.

- [x] **Step 1: Apply the minimal source changes**

Use the existing `[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]` convention from neighboring filter attributes. Replace `(Exception) null!` with `(Exception) null`, remove `value!` and `HttpMethod!`, and replace the generic `!= null` hash-code branches with `EqualityComparer<T>.Default.GetHashCode(...)` so reference-type nulls still hash as zero and value types remain supported.

- [x] **Step 2: Run focused tests**

Run:

```powershell
dotnet test src/test/UnitTest.Core/UnitTest.Core.csproj --no-restore
dotnet test src/test/IntegrationTest.EntryPoint.WebApi/IntegrationTest.EntryPoint.WebApi.csproj --no-restore
```

Expected: both projects build and all tests pass.

### Task 3: Final static verification

- [x] **Step 1: Scan for the reported patterns**

Confirm the six reported null-forgiving locations and two missing filter metadata locations no longer match, and all four report files contain the required metadata.

- [x] **Step 2: Check the complete diff**

Run `git diff --check`, inspect the changed-file list, and confirm the pre-existing user changes remain untouched.
