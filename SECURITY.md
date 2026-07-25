# Security Policy

## Reporting a Vulnerability

**Do not open a public issue.**

Report privately via [GitHub Security Advisories](https://github.com/OFFSECHQ/windows-security-studio/security/advisories/new) or email security@offsechq.com. Include a description, reproduction steps, and impact. Expect an initial response within 72 hours.

For vulnerabilities in the upstream [Harden-Windows-Security](https://github.com/HotCakeX/Harden-Windows-Security) code, report to the [upstream repository](https://github.com/HotCakeX/Harden-Windows-Security/security) directly.

## Verifying Releases

All packages are built from source via [GitHub Actions](https://github.com/OFFSECHQ/windows-security-studio/actions) with full build logs. MSIX bundles are signed with the certificate included in each installation kit. You can reproduce any release by cloning the repository and running the build scripts.
