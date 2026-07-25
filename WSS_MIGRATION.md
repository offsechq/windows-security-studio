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

- [ ] Move `Directory.Build.props` under WSS.
- [ ] Move `CommonCore` under WSS and update all imports.
- [ ] Move the 39 linked source files under WSS.
- [ ] Remove cross-product linked-source entries from the main project.
- [ ] Move `ComManager`, `DISMService`, `QuantumRelayHSS`, and Rust interop under
  `Windows Security Studio/Components`.
- [ ] Update solution and helper project references.
- [ ] Update local build script paths and outputs.

### Phase 3: Rename SSS/HSS to WSS

- [ ] Rename the application directory, project, solution, and build script.
- [ ] Rename `HardenSystemSecurity` namespaces and symbols.
- [ ] Rename remaining `AppControlManager` namespaces used by WSS-owned code.
- [ ] Rename `QuantumRelayHSS` source/project/service/executable to
  `QuantumRelayWSS`.
- [ ] Change product display names, descriptions, package metadata, execution
  alias, release notes, update identifiers, and artifact names.
- [ ] Preserve only explicitly documented compatibility identifiers.
- [ ] Update localized resource values without changing unrelated translations.

### Phase 4: Automation and repository metadata

- [ ] Rename and update the WSS build/release workflow.
- [ ] Remove the ACS build/release workflow.
- [ ] Update caches and all helper project paths.
- [ ] Update Dependabot to scan only WSS and its components.
- [ ] Keep .NET SDK auto-merge limited to the surviving SDK manifest.
- [ ] Update README, agent instructions, CODEOWNERS, `.gitattributes`,
  `.gitignore`, installer behavior, and security documentation where needed.

### Phase 5: Remove ACS and obsolete documentation

- [ ] Delete all residual ACS-only application source and assets.
- [ ] Delete ACS manifests, solutions, scripts, download/version files, and
  packaging metadata.
- [ ] Delete ACS-only wiki documentation.
- [ ] Delete general wiki material outside the WSS product documentation scope,
  as requested.
- [ ] Rename the surviving wiki section to `Windows Security Studio`.
- [ ] Rebuild the wiki home/index so it exposes WSS documentation only.
- [ ] Remove tracked build outputs, IDE caches, binaries, and user files that
  should be generated.

### Phase 6: Validation and cleanup

- [ ] XML-parse project, manifest, solution, and resource files.
- [ ] Validate workflow YAML with `actionlint`.
- [ ] Validate PowerShell syntax where a parser is available.
- [ ] Run `dotnet restore` and the strongest build available in the local
  environment.
- [ ] Run Cargo metadata/checks for moved Rust workspaces where supported.
- [ ] Confirm every project/import/content/native-library path exists.
- [ ] Confirm no ACS directory or ACS product automation remains.
- [ ] Audit residual `App Control Studio`, `AppControlManager`,
  `System Security Studio`, `HardenSystemSecurity`, `QuantumRelayHSS`, `HSS`,
  `SSS`, `acs-v`, and `sss-v` references.
- [ ] Classify and document any intentional compatibility residue.
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
