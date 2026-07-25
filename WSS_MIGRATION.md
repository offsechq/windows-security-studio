# Windows Security Studio Migration

## Purpose

This file is the durable implementation plan, progress log, decision record, and
validation checklist for consolidating this repository into one Windows-only
application:

- Remove App Control Studio (ACS) as an application and product.
- Preserve every component required by the surviving security application.
- Rename System Security Studio (SSS) to Windows Security Studio (WSS).
- Make WSS structurally independent: no source, build, packaging, or release
  path may depend on an `App Control Studio` directory.
- Remove ACS-only source, packaging, release automation, assets, and wiki
  documentation.
- Clean obsolete generated artifacts and stale product terminology.

## Baseline Inventory

Inventory captured from commit `01c73a6498639b4707333bbf3289c4794129900c`.

| Area | Baseline |
|---|---:|
| Tracked ACS files | 677 |
| Tracked SSS files | 423 |
| ACS-owned `CommonCore` source files compiled by SSS | 117 |
| Additional ACS-owned C# files linked directly by SSS | 39 |
| Authored SSS files containing `AppControlManager` types/namespaces | 84 |
| ACS-owned conditional compilation sites for SSS | 34 |
| Required helper/native projects stored below ACS | 4 |

The required helper/native projects are:

1. `ComManager` (C++)
2. `DISMService` (C#)
3. `QuantumRelayHSS` (C# Windows service)
4. `Rust Interop Library` (Rust static/dynamic library)

### Current dependency edges

The surviving application currently:

- Imports `App Control Studio/Directory.Build.props`.
- Imports `App Control Studio/eXclude/CommonCore/CommonCore.projitems`.
- Links 39 C# files from ACS for shared WinUI controls, application/window
  behavior, navigation, settings, updates, logs, Intune, and collections.
- Builds helper projects from `App Control Studio/eXclude`.
- Links the Rust static library from the ACS build tree.
- Uses ACS paths in the local PowerShell build and Windows GitHub Actions build.

This is source/layout coupling, not a dependency on an installed ACS executable
or compiled ACS application assembly.

## Target Layout

```text
windows-security-studio/
├── Windows Security Studio/
│   ├── Windows Security Studio.csproj
│   ├── Windows Security Studio.slnx
│   ├── Directory.Build.props
│   ├── CommonCore/
│   ├── Components/
│   │   ├── ComManager/
│   │   ├── DISMService/
│   │   ├── QuantumRelayWSS/
│   │   └── RustInterop/
│   ├── CustomUIElements/
│   ├── Pages/
│   ├── ViewModels/
│   └── ...
├── Wiki posts/
│   └── Windows Security Studio/
├── .github/
├── Install.cmd
└── README.md
```

Because WSS will be the only application, shared code is owned by WSS instead of
being kept in a misleading top-level shared product hierarchy.

## Naming Decisions

| Concept | Old | New |
|---|---|---|
| Product | System Security Studio | Windows Security Studio |
| Abbreviation | SSS / HSS | WSS |
| Main namespace | `HardenSystemSecurity` | `WindowsSecurityStudio` |
| Shared UI namespace | `AppControlManager.*` | `WindowsSecurityStudio.*` |
| Assembly/executable | `HardenSystemSecurity` | `WindowsSecurityStudio` |
| Command alias | `HSS.exe` | `WSS.exe` |
| Privileged service | `QuantumRelayHSS` | `QuantumRelayWSS` |
| Release tag prefix | `sss-v` | `wss-v` |
| Artifact name | `System Security Studio_*` | `Windows Security Studio_*` |

### MSIX upgrade compatibility exception

Keep the existing package identity name
`OFFSECHQ.SystemSecurityStudio` unless a Windows packaging test proves that it
can be changed without preventing in-place upgrades. The identity is not
user-visible branding, and preserving it avoids creating a second package
family that would install beside the existing application.

Publisher identity and signing certificate identifiers must also remain stable.
The manifest display name, description, executable, alias, assets, release
artifacts, and documentation will use WSS branding.

## Migration Phases

### Phase 1: Inventory and tracker

- [x] Capture repository baseline and dependency counts.
- [x] Identify source-level ACS dependencies.
- [x] Identify helper/native dependencies.
- [x] Identify build, workflow, Dependabot, release, installer, and wiki scope.
- [x] Record target layout and naming decisions.

### Phase 2: Relocate required ACS-owned code

- [x] Move `Directory.Build.props` under WSS.
- [x] Move `CommonCore` under WSS and update all imports.
- [x] Move the 39 linked source files under WSS.
- [x] Remove cross-product linked-source entries from the main project.
- [x] Move `ComManager`, `DISMService`, `QuantumRelayHSS`, and Rust interop under
  `Windows Security Studio/Components`.
- [x] Update solution and helper project references.
- [x] Update local build script paths and outputs.

### Phase 3: Rename SSS/HSS to WSS

- [x] Rename the application directory, project, solution, and build script.
- [x] Rename `HardenSystemSecurity` namespaces and symbols.
- [x] Rename remaining `AppControlManager` namespaces used by WSS-owned code.
- [x] Rename `QuantumRelayHSS` source/project/service/executable to
  `QuantumRelayWSS`.
- [x] Change product display names, descriptions, package metadata, execution
  alias, release notes, update identifiers, and artifact names.
- [x] Preserve only explicitly documented compatibility identifiers.
- [x] Update localized resource values without changing unrelated translations.

### Phase 4: Automation and repository metadata

- [x] Rename and update the WSS build/release workflow.
- [x] Remove the ACS build/release workflow.
- [x] Update caches and all helper project paths.
- [x] Update Dependabot to scan only WSS and its components.
- [x] Keep .NET SDK auto-merge limited to the surviving SDK manifest.
- [x] Update README, agent instructions, CODEOWNERS, `.gitattributes`,
  `.gitignore`, installer behavior, and security documentation where needed.

### Phase 5: Remove ACS and obsolete documentation

- [x] Delete all residual ACS-only application source and assets.
- [x] Delete ACS manifests, solutions, scripts, download/version files, and
  packaging metadata.
- [x] Delete ACS-only wiki documentation.
- [x] Delete general wiki material outside the WSS product documentation scope,
  as requested.
- [x] Rename the surviving wiki section to `Windows Security Studio`.
- [x] Rebuild the wiki home/index so it exposes WSS documentation only.
- [x] Remove tracked build outputs, IDE caches, binaries, and user files that
  should be generated.

### Phase 6: Validation and cleanup

- [x] XML-parse project, manifest, solution, and resource files.
- [x] Validate workflow YAML with `actionlint`.
- [x] Validate PowerShell syntax where a parser is available.
- [x] Run `dotnet restore` and the strongest build available in the local
  environment.
- [x] Run Cargo metadata/checks for moved Rust workspaces where supported.
- [x] Confirm every project/import/content/native-library path exists.
- [x] Confirm no ACS directory or ACS product automation remains.
- [x] Audit residual `App Control Studio`, `AppControlManager`,
  `System Security Studio`, `HardenSystemSecurity`, `QuantumRelayHSS`, `HSS`,
  `SSS`, `acs-v`, and `sss-v` references.
- [x] Classify and document any intentional compatibility residue.
- [ ] Build/package WSS on the `windows-2025` GitHub runner.
- [ ] Verify the produced bundle, symbols, SBOM, release tag/name, download URL,
  and installer inputs.

## Runtime Acceptance Matrix

The Windows release build and smoke test must cover:

- Application launch, activation, navigation, settings, theme, and shutdown
- Update discovery and package download
- Logging and error-dialog paths
- Microsoft Graph/Intune authentication and policy operations
- Defender and Attack Surface Reduction
- BitLocker
- Device Guard, VBS, and HVCI
- Firewall, networking, TLS, and country IP blocking
- Optional Features through DISM
- Audit policy and Group Policy editor
- Elevated helper execution and packaged service lifecycle
- Rust interop calls in Debug and Native AOT Release
- File reputation, certificate inspection, and installed-app management
- MSIX installation over an existing SSS package

## Progress Log

### 2026-07-25

- Confirmed clean `main` at `01c73a64`.
- Completed the read-only architecture and dependency assessment.
- Chose a self-contained WSS layout rather than retaining a misleading ACS or
  generic shared-product directory.
- Chose to preserve the current MSIX package identity for upgrade compatibility
  while replacing visible and code-level SSS/HSS branding.
- Created this tracker before moving or deleting application files.
- Moved all WSS-required shared sources and native/privileged components into
  the self-contained `Windows Security Studio` tree.
- Removed the ACS application, its packaging workflow, its generated binaries,
  and all non-WSS wiki sections.
- Renamed visible product branding, namespaces, executable/alias, relay service,
  artifact names, release tags, paths, automation, and documentation to WSS.
- Preserved `OFFSECHQ.SystemSecurityStudio` only as the internal MSIX package
  identity required for in-place upgrades.
- Replaced stale linked-source and staged-binary packaging with direct local
  component output references.
- Updated the Native AOT runtime packages to `10.0.9`, resolving a restore
  downgrade detected by the pinned .NET 10.0.301 SDK.
- Validated 41 XML files, every `CommonCore` compile entry, PowerShell syntax,
  both workflow files with `actionlint`, every referenced action tag, all
  retained internal wiki links, and all JSON resources (allowing UTF-8 BOMs).
- Restored all three .NET projects. DISMService and QuantumRelayWSS compile with
  zero warnings; the main app reaches the Windows-only WinUI XAML compiler,
  which cannot execute on Linux and is deferred to the Windows runner.
- Ran Cargo metadata and cross-target `cargo check` successfully for
  `x86_64-pc-windows-msvc` with the nightly toolchain and configured mitigation
  flags.
- Confirmed the legacy-term audit is empty outside this tracker, historical
  screenshot URLs, and the documented MSIX compatibility identity.

## Completion Definition

The migration is complete only when:

1. WSS builds and packages successfully on Windows.
2. The packaged application contains and can launch all required helper
   executables.
3. There is no `App Control Studio` application or source/build dependency.
4. There is no SSS/HSS branding except an explicitly documented non-user-visible
   compatibility identifier.
5. Repository automation manages only WSS.
6. The wiki contains only current WSS documentation.
7. The working tree contains no accidental build output or migration debris.
