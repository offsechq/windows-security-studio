# Windows Security Studio

Windows Security Studio is a modern secure lightweight application that can help you harden, secure and lock down your system. It is designed to be user-friendly and efficient, providing a range of features to enhance the security of your Windows operating system.

It always stays up to date with the latest security patches and provides constant and consistent maintenance and support.

## How To Install or Update The App

### Use The [Microsoft Store](https://apps.microsoft.com/detail/9p7ggfl7dx57)

<a href="https://apps.microsoft.com/detail/9p7ggfl7dx57?referrer=appbadge&mode=direct">
	<img src="https://get.microsoft.com/images/en-us%20dark.svg" width="270"/>
</a>

### Use Winget

You can utilize Winget to automate the installation of the Windows Security Studio.

```powershell
winget install --id 9p7ggfl7dx57 --exact --accept-package-agreements --accept-source-agreements --force --source msstore
```

### Offline Installation

Download the latest **Windows Security Studio Install Kit** from the repository's [Releases](https://github.com/OFFSECHQ/windows-security-studio/releases), extract it, and run `Install.cmd`. The kit includes the MSIX bundle and its signing certificate.

Please open a repository discussion if you have questions about the build, security model, or usage. The [complete source code](https://github.com/OFFSECHQ/windows-security-studio/tree/main/Windows%20Security%20Studio) is available in this repository.

### Supported Operating Systems

- Windows 11 25H2
- Windows 11 24H2
- Windows 11 23H2
- Windows 11 22H2
- Windows Server 2025

## Preview of the App

<p align="center">
</p>

## Technical Details of The App

- Secure and transparent development and build process.
- Built using [WinUI3](https://learn.microsoft.com/windows/apps/winui/winui3/) / [XAML](https://github.com/microsoft/microsoft-ui-xaml) / [C#](https://learn.microsoft.com/dotnet/csharp/).
- Built using the latest [.NET](https://dotnet.microsoft.com) SDK.
- Powered by the [WinAppSDK](https://github.com/microsoft/WindowsAppSDK) (formerly Project Reunion).
- Packaged with the modern [MSIX](https://learn.microsoft.com/windows/msix/overview) format.
- Incorporates the [Mica](https://learn.microsoft.com/windows/apps/design/style/mica) material design for backgrounds.
- Adopts the Windows 11 [Fluent design system](https://fluent2.microsoft.design/components/windows).
- Fast execution and startup time.
- 0 required dependency.
- 0 Third-party library or file used.
- 0 Telemetry or data collection.
- 100% clean uninstallation.
- 100% open-source and free to use.
- Natively supports X64 and ARM64 architectures.
- Full [Trimming](https://learn.microsoft.com/dotnet/core/deploying/trimming/trim-self-contained) and [Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot) support.
- Never uses runtime marshaling.
- 0 dependency on any executable in the OS.

## Features

<p align="center">
</p>

- Everything always stays up-to-date with the newest proactive security measures that are industry standards and scalable.

- No Windows functionality is removed/disabled against Microsoft's recommendations.

- All of the links in the documentations and sources are from official Microsoft websites, straight from the source. No bias, No FUD, No misinformation and definitely No old obsolete methods. That's why there are no links to 3rd party news websites, forums, made up blogs/articles, and such.

- When a security measure is no longer necessary because it's applied by default by Microsoft on new builds of Windows, it will also be removed from the app in order to prevent any problems and because it won't be necessary anymore. **Community feedback will always be taken into account when doing so.**

- Applying the security measures can make your system compliant with Microsoft Security Baselines and Secured-core PC specifications (provided that you use modern hardware that supports the latest Windows security features) - [See what makes a Secured-core PC](https://learn.microsoft.com/windows-hardware/design/device-experiences/oem-highly-secure-11#what-makes-a-secured-core-pc) - <a href="https://github.com/OFFSECHQ/windows-security-studio/wiki/Device-Guard">Check Device Guard category for more info</a>

  > [Secured-core](https://learn.microsoft.com/windows-hardware/design/device-experiences/oem-highly-secure-11) – recommended for the most sensitive systems and industries like financial, healthcare, and government agencies. Builds on the previous layers and leverages advanced processor capabilities to provide protection from firmware attacks.

- Since I originally created this repository for myself and people I care about, I always maintain it to the highest possible standard.

- If you have multiple accounts on your device, you only need to apply the security measures 1 time with Admin privileges, that will make system-wide changes. Then you can **_optionally_** run the app, without Admin privileges, for each standard user to apply the [Non-Admin category](https://github.com/OFFSECHQ/windows-security-studio/wiki/Non-Admin-Measures).

## Comprehensive Logging Capabilities

The Windows Security Studio app includes detailed logging feature that tracks every part of its operations. These logs are helpful for reviewing what actions were taken, making it easier to audit and troubleshoot if necessary. The logs are saved in the following location:

```
C:\Users\USERNAME\AppData\Local\Temp\WindowsSecurityStudioLogs
```

The logs ensure that all actions taken by the Windows Security Studio app are recorded, giving you clear visibility into your security processes. Whether you're conducting security checks, responding to issues, or just keeping an eye on things, these logs can provide valuable information.

## Security

> [!IMPORTANT]
> The Windows Security Studio application is built publicly using a [GitHub Workflow](https://github.com/OFFSECHQ/windows-security-studio/actions/runs/17206622843/workflow) and uploaded to the Microsoft Partner Center for validation and signing. The action uses [SBOM (Software Bill of Materials)](https://github.com/OFFSECHQ/windows-security-studio/network/dependencies) generation to comply with the highest [security standards](https://docs.github.com/en/actions/security-for-github-actions/using-artifact-attestations/using-artifact-attestations-to-establish-provenance-for-builds) such as [SLSA](https://slsa.dev/spec/v1.0/levels) level 3. [GitHub's CodeQL Advanced workflow](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/codeql.yml) with extended security model scans the entire repository. All of the dependencies of any project in this repository are uploaded to GitHub and are available in the [Dependency Graph](https://github.com/OFFSECHQ/windows-security-studio/network/dependencies).

Windows Security Studio is architected with a security-first philosophy from its inception. Every feature is designed and implemented with an offensive security mindset, ensuring that security is never an afterthought—and never will be. When selecting a solution tasked with defending critical systems, the last thing you want is a so‑called security tool that silently broadens your attack surface or neglects foundational safeguards. This application is built to be inherently trustworthy, defensible, and resilient.

### Dependencies

Windows Security Studio explicitly and unequivocally maintains zero third‑party dependencies. It relies solely on the .NET SDK, the Windows App SDK, and a minimal set of small trusted Microsoft platform components for the User Interface. This deliberate constraint sharply reduces the attack surface and virtually eliminates common software supply chain attack vectors. Rather than pulling transient packages to satisfy feature gaps, required capabilities are purpose‑built in-house—implemented correctly, auditable, and securely. While this increases development effort and time, the mission and deployment contexts of this application more than justify the investment.

Leveraging GitHub's native automation (including Dependabot) alongside Microsoft's patch cadence, security and platform updates can be integrated and released rapidly, preserving both stability and assurance.

### Exploit Protection

The application avoids dynamic code generation, enhancing security posture and reducing vulnerability exposure. This design ensures compatibility with advanced OS-level exploit mitigation. The Windows Security Studio supports [process mitigations / Exploit Protections](https://learn.microsoft.com/defender-endpoint/exploit-protection-reference) such as: `Blocking low integrity images`, `Blocking remote images`, `Blocking untrusted fonts`, `Strict Control Flow Guard`, `Disabling extension points`, `Export Address Filtering`, `Hardware enforced stack protection`, `Import Address Filtering`, `Validate handle usage`, `Validate stack integrity`, `Code integrity guard`.

This disciplined approach bolsters resistance against memory corruption, injection, and tampering techniques frequently leveraged by sophisticated adversaries.

### Code Review

The codebase is extensively and thoughtfully documented, enabling reviewers to trace logic, validate control flows, and assess security-relevant decisions with minimal friction. I remain fully available to clarify design rationale, threat assumptions, or implementation details whenever deeper scrutiny is desired.

## Documentation

> [!NOTE]
> Mixing 3rd party security solutions with advanced Microsoft Defender features or other features offered by the Windows Security Studio app is not recommended as it can create conflicts.

### Quick Links

- [Protect](https://github.com/OFFSECHQ/windows-security-studio/wiki/Protect)
- [Microsoft Security Baselines](https://github.com/OFFSECHQ/windows-security-studio/wiki/Microsoft-Security-Baselines)
- [Microsoft 365 Apps Security Baseline](https://github.com/OFFSECHQ/windows-security-studio/wiki/Microsoft-365-Apps-Security-Baseline)
- [Microsoft Defender](https://github.com/OFFSECHQ/windows-security-studio/wiki/Microsoft-Defender)
- [Attack Surface Reduction](https://github.com/OFFSECHQ/windows-security-studio/wiki/Attack-Surface-Reduction)
- [Bitlocker](https://github.com/OFFSECHQ/windows-security-studio/wiki/BitLocker)
- [Device Guard](https://github.com/OFFSECHQ/windows-security-studio/wiki/Device-Guard)
- [TLS Security](https://github.com/OFFSECHQ/windows-security-studio/wiki/TLS-Security)
- [Lock Screen](https://github.com/OFFSECHQ/windows-security-studio/wiki/Lock-Screen)
- [User Account Control](https://github.com/OFFSECHQ/windows-security-studio/wiki/User-Account-Control)
- [Windows Firewall](https://github.com/OFFSECHQ/windows-security-studio/wiki/Windows-Firewall)
- [Optional Windows Features](https://github.com/OFFSECHQ/windows-security-studio/wiki/Optional-Windows-Features)
- [Windows Networking](https://github.com/OFFSECHQ/windows-security-studio/wiki/Windows-Networking)
- [Miscellaneous Configurations](https://github.com/OFFSECHQ/windows-security-studio/wiki/Miscellaneous-Configurations)
- [Windows Update](https://github.com/OFFSECHQ/windows-security-studio/wiki/Windows-Update)
- [Edge Browser](https://github.com/OFFSECHQ/windows-security-studio/wiki/Edge-Browser)
- [Certificate Checking](https://github.com/OFFSECHQ/windows-security-studio/wiki/Certificate-Checking)
- [Country IP Blocking](https://github.com/OFFSECHQ/windows-security-studio/wiki/Country-IP-Blocking)
- [Non Admin Measures](https://github.com/OFFSECHQ/windows-security-studio/wiki/Non-Admin-Measures)
- [Group Policy Editor](https://github.com/OFFSECHQ/windows-security-studio/wiki/Group-Policy-Editor)
- [Manage Installed Apps](https://github.com/OFFSECHQ/windows-security-studio/wiki/Manage-Installed-Apps)
- [File Reputation](https://github.com/OFFSECHQ/windows-security-studio/wiki/File-Reputation)
- [Audit Policies](https://github.com/OFFSECHQ/windows-security-studio/wiki/Audit-Policies)
- [Cryptographic Bill of Materials](https://github.com/OFFSECHQ/windows-security-studio/wiki/Cryptographic-Bill-of-Materials)
- [Intune](https://github.com/OFFSECHQ/windows-security-studio/wiki/Intune)
- [Configuration Service Provider (CSP)](https://github.com/OFFSECHQ/windows-security-studio/wiki/Configuration-Service-Provider)

## Supported Languages

The Windows Security Studio fully supports the following languages:

- English
- Hebrew
- Greek
- Hindi
- Malayalam
- Arabic
- Spanish
- Polish
- German
- French

## Windows Service

The Windows Security Studio app utilizes a Windows Service that is responsible for performing tasks that require SYSTEM privilege such as Intune configurations detection during verification jobs so that even when you applied the security measures via Intune, they will be detected and verifiable by the app. The service is very compact (2MBs only), highly optimized and runs only when needed. It does not consume any resources when idle. The service is designed to automatically shut itself down when idle for 120 seconds.

It can only be used by elevated Administrators and SYSTEM account. It is automatically installed when the Windows Security Studio app is installed and removed when the Windows Security Studio app is uninstalled, not leaving any leftovers on the system. It has 0 dependency other than the .NET SDK itself and its executable is inside the app's package.

The service source code [can be found here](https://github.com/OFFSECHQ/windows-security-studio/tree/main/Windows%20Security%20Studio/Components/QuantumRelayWSS). The service name is `QuantumRelayWSS`. To enable verbose Windows Event Log output, create a system environment variable named `QUANTUMRELAYWSS_DEBUG` with a value of `1` or `true`.

The service supports Arbitrary Code Guard exploit protection as well as many others, all of which can be applied to it in the [Microsoft Defender category](https://github.com/OFFSECHQ/windows-security-studio/wiki/Microsoft-Defender).

## CommandLine Interface (CLI) Support

The Windows Security Studio app can be launched via command line for advanced users and automation scenarios. All CLI arguments are case-insensitive.

When `--cli` is present the app runs headless (no GUI).
If an operation requires elevation, the app relaunches itself elevated and preserves all CLI arguments.
If elevation is denied when required, no changes are performed and the process exits with code 0 (no-op).

---

### Via Execution Alias

#### Open a Group Policy (.POL) file in the Group Policy Editor

```powershell
WSS.exe --file="C:\Path\Policy.pol"
```

---

### Via File Activation (Supported File Types Only)

#### Opens a POL file in the Group Policy Editor (same as double‑clicking in File Explorer)

```powershell
Invoke-Item -Path "C:\Path\Policy.pol"
```

---

### Headless CLI Mode (`--cli`)

Use `--cli` to run without the GUI.

#### Preset-based Operations

Run a full preset across selected categories.

```powershell
WSS.exe --cli --preset=0|1|2 --op=Apply|Remove|Verify
```

Presets:

- 0 = Basic
- 1 = Recommended
- 2 = Complete

Examples:

```powershell
# Apply the Recommended preset
WSS.exe --cli --preset=1 --op=Apply

# Verify the Complete preset
WSS.exe --cli --preset=2 --op=Verify

# Remove the Basic preset
WSS.exe --cli --preset=0 --op=Remove
```

#### Device Usage Intent Operations

Apply protections tailored to a specific [device usage intent](https://github.com/OFFSECHQ/windows-security-studio/wiki/Protect#device-usage-intents).

> [!NOTE]
> Only `Apply` is supported for intents at this time.

```powershell
WSS.exe --cli --intent=<IntentName> --op=Apply
```

Supported intents:

- Development
- Gaming
- School
- Business
- SpecializedAccessWorkstation
- PrivilegedAccessWorkstation

Example (Business intent):

```powershell
WSS.exe --cli --intent=Business --op=Apply
```

---

### System State Report Export

Create a full system state JSON report. Elevation is required.

Syntax:

```powershell
WSS.exe --cli ExportReport --out="C:\Path\WindowsSecurityStudio-Report.json"
```

Requirements:

- `--out` is mandatory.

Example:

```powershell
WSS.exe --cli ExportReport --out="C:\Reports\WSS-SystemState.json"
```

---

### System State Report Import / Restore

Import and apply a previously exported system state report. Elevation is required.

Syntax:

```powershell
WSS.exe --cli ImportReport --in="C:\Path\Report.json" [--mode=full|partial]
```

Requirements:

- `--in` is mandatory and must point to an existing `.json` file.
- `--mode` defaults to `full` if omitted.

Modes:

- `full` → Apply all measures marked applied AND remove all measures marked not applied.
- `partial` → Apply only measures marked applied; skip removals.

Examples:

```powershell
# Full synchronization (default)
WSS.exe --cli ImportReport --in="C:\Reports\WSS-SystemState.json"

# Partial (apply-only) restore
WSS.exe --cli ImportReport --in="C:\Reports\WSS-SystemState.json" --mode=partial
```

---

### Microsoft Store App Update Check

Headless check for app updates (requires elevation):

```powershell
WSS.exe --cli CheckMSStoreAppUpdate
```

---

### Exit Codes

| Code | Meaning                                                                              |
| ---- | ------------------------------------------------------------------------------------ |
| 0    | Success or no-op (including elevation denied before performing any change)           |
| 1    | Unexpected runtime failure (exception during execution)                              |
| 2    | Invalid arguments (missing required flag, unsupported value, invalid path/extension) |

## Repository Structure

The application is self-contained under [`Windows Security Studio/`](https://github.com/OFFSECHQ/windows-security-studio/tree/main/Windows%20Security%20Studio):

- `CommonCore/` contains reusable WSS infrastructure shared by the application and its helper processes.
- `Components/ComManager/` contains the native C++ COM helper.
- `Components/DISMService/` contains the isolated DISM helper.
- `Components/QuantumRelayWSS/` contains the on-demand privileged Windows service.
- `Components/RustInterop/` contains the Rust interoperability library.
- `Resources/` contains hardening policies, security data, and Intune-ready resources.

Generated helper executables and libraries are build outputs and are not stored in source control.

## Build Windows Security Studio Locally

Build on Windows with Visual Studio 2022, the .NET 10 SDK, and Rust nightly with the `x86_64-pc-windows-msvc` target installed:

```powershell
cd "Windows Security Studio"
.\Build-WindowsSecurityStudio.ps1
```

The script builds every required component and then creates the x64 WSS MSIX package. The repository [Windows packaging workflow](https://github.com/OFFSECHQ/windows-security-studio/actions/workflows/Build%20Windows%20Security%20Studio%20MSIX%20Package.yml) performs the same build in a clean GitHub-hosted Windows environment and produces the install kit and provenance artifacts.
