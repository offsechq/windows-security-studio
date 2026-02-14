# Agent Instructions

## Repository Map

- `App Control Studio/`: WinUI 3 desktop app for app control and policy workflows.
- `System Security Studio/`: WinUI 3 desktop app for hardening and security management workflows.
- `App Control Studio/eXclude/`: Shared components, helpers, and interop used by both apps.

## Development Principles

1. Keep changes small, targeted, and behavior-focused.
2. Preserve existing architecture, naming, and folder structure.
3. Do not add dependencies unless explicitly required.
4. Keep .NET code Native AOT friendly (avoid reflection-heavy or dynamic patterns).
5. Prefer explicit, readable code over clever abstractions.
6. Keep background operations non-blocking and avoid dimming/disabling unrelated UI.
7. Follow existing WinUI 3 patterns and shared styles on each page.

## UI and Localization

1. Use `x:Uid` for user-facing UI text where possible.
2. Add/update corresponding keys in `Strings/en-US/Resources.resw`.
3. Preserve accessibility affordances: keyboard navigation, focus visibility, and tooltips/help text.
4. Keep layout responsive for narrow and wide window states.

## Validation

Run builds for affected app(s):

```powershell
dotnet build "App Control Studio/App Control Studio.csproj" -c Debug
dotnet build "System Security Studio/System Security Studio.csproj" -c Debug
```

If only one app is changed, building that app is sufficient.

## Workflow Hygiene

1. Avoid broad refactors unless requested.
2. Do not touch unrelated files.
3. Keep `.github/workflows` changes limited to requested build/release needs.
