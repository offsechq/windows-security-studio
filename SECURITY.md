# Security Policy

## About This Repository

This repository (**Windows Security Studio**) is a maintained fork of [HotCakeX/Harden-Windows-Security](https://github.com/HotCakeX/Harden-Windows-Security) by **OFFSECHQ**.

All modifications are made transparently, and the full source code is available for audit.

## Upstream Synchronization

- Updates from the upstream repository are monitored weekly via GitHub Actions
- When new changes are detected, a GitHub issue is created for review before merge

## Verifying Releases

All MSIX packages are built directly from this repository's source code using GitHub Actions. You can verify the integrity by:

1. **Checking Workflow Runs**: Every release is built via [GitHub Actions](https://github.com/OFFSECHQ/windows-security-studio/actions) with full build logs available
2. **Signed Packages**: MSIX packages are signed with a self-signed certificate included in each release
3. **Reproducible Builds**: You can build the packages yourself using the same workflow files in `.github/workflows/`

## Reporting a Vulnerability

If you discover a security vulnerability in this project:

1. **For sensitive issues**: Use the [Security tab](https://github.com/OFFSECHQ/windows-security-studio/security/advisories/new) to report privately via GitHub Security Advisories
2. **Email**: security@offsechq.com
3. **For general issues**: Open an [issue](https://github.com/OFFSECHQ/windows-security-studio/issues) if the vulnerability can be disclosed publicly

Please include:

- A description of the vulnerability
- Steps to reproduce
- Potential impact
- Any suggested fixes (optional)

## Upstream Security

For vulnerabilities that affect the **upstream** Harden-Windows-Security repository, please report directly to the original maintainer at [HotCakeX/Harden-Windows-Security](https://github.com/HotCakeX/Harden-Windows-Security/security).
