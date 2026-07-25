# Agent Instructions

## Repository Map

- `Windows Security Studio/`: the WinUI 3 desktop application, packaging project, and application-owned shared infrastructure.
- `Windows Security Studio/CommonCore/`: reusable WSS infrastructure imported by the main app and helper projects.
- `Windows Security Studio/Components/`: native and privileged helper projects used by WSS.
- `Wiki posts/Windows Security Studio/`: WSS-only documentation sources.

App Control Studio has been retired. Do not reintroduce source links, build dependencies, paths, branding, or documentation for it.

## Development Principles

1. Keep changes behavior-focused and consistent with the WSS architecture.
2. Do not add dependencies unless explicitly required.
3. Keep .NET code Native AOT friendly; avoid reflection-heavy or dynamic patterns.
4. Prefer explicit, readable code over clever abstractions.
5. Keep background operations non-blocking and avoid dimming or disabling unrelated UI.
6. Follow the existing WinUI 3 patterns and shared styles.
7. Treat `OFFSECHQ.SystemSecurityStudio` as a compatibility-only MSIX identity. Do not use the legacy name for visible branding or new identifiers.

## UI and Localization

1. Use `x:Uid` for user-facing UI text where possible.
2. Add or update corresponding keys in `Strings/en-US/Resources.resw`.
3. Preserve accessibility affordances: keyboard navigation, focus visibility, and tooltips.
4. Keep layouts responsive for narrow and wide window states.

## Validation

Run the complete build for packaging or component changes:

```powershell
cd "Windows Security Studio"
.\Build-WindowsSecurityStudio.ps1
```

For a fast managed-code check:

```powershell
dotnet build "Windows Security Studio/Windows Security Studio.csproj" -c Debug -p:Platform=x64
```

## Workflow Hygiene

1. Keep workflow paths and artifact names aligned with Windows Security Studio.
2. Do not commit generated helper binaries or build-output directories.
3. Update WSS wiki sources when behavior or user-facing workflows change.
