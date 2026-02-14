# Agent Instructions

This repository is developed independently. Do not add or restore upstream-sync automation.

## Projects

- `App Control Studio/`: WinUI 3 app for application control and policy management.
- `System Security Studio/`: WinUI 3 app for system hardening and protection flows.
- `App Control Studio/eXclude/`: Shared services, helpers, and Rust interop used across apps.

## Engineering Rules

1. Keep changes minimal and scoped to the requested behavior.
2. Preserve existing architecture, naming, and folder structure.
3. Do not add new dependencies unless explicitly requested.
4. Keep .NET changes Native AOT friendly (avoid reflection-heavy patterns and dynamic code gen).
5. Favor explicit, readable code over clever abstractions.
6. Update `Strings/en-US/Resources.resw` for user-facing text changes.
7. Keep UI responsive; avoid blocking or dimming patterns for background retrieval work.
8. Preserve WinUI 3 control/style consistency when editing pages and tabs.
9. Use x64 as the target runtime for validation.

## Build Validation

Run relevant builds after edits:

```powershell
dotnet build "App Control Studio/App Control Studio.csproj" -c Debug -r win-x64
dotnet build "System Security Studio/System Security Studio.csproj" -c Debug -r win-x64
```

If the task only touches one app, build that app at minimum.

## GitHub Folder Policy

- Keep release/build workflows that are currently used.
- Do not reintroduce `.github/workflows/sync-upstream.yml`.
- Do not reintroduce upstream-tracking metadata such as `.github/LAST_SYNCED_UPSTREAM_SHA` or sync guide files.
