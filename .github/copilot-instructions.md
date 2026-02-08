# Copilot Instructions

> **Note**: This repository does not accept external pull requests. Only repository owners and authorized bots may contribute.

## Repository Structure

- `App Control Studio/`: Main WinUI 3 app for WDAC policy management
- `System Security Studio/`: Main WinUI 3 app for system hardening
- `App Control Studio/eXclude/`: Shared infrastructure (CommonCore, Rust interop, services) - used by both apps

## Key Guidelines

1. Use the latest .NET and C# features when working with C# or .NET code.
2. Use the latest features of the Rust language when working with Rust code.
3. Use the latest features of the C++ language when working with C++ code.
4. Maintain existing code structure and organization.
5. Comment the new changes properly.
6. Never add extra dependencies to the projects.
7. Always make sure the code you add or modify is compatible with Native AOT compilation if you're working with C# or .NET code.
8. Use Internal types when working with C# or .NET code.
9. Define variable types explicitly instead of using the `var` keyword when working with C# or .NET code.
10. Do not make unnecessary changes to the code.
11. Do not remove any existing comments from the code unless the comments are no longer correct or valid.
12. Define variable types explicitly after `let` keyword and variable name when working with Rust code.
13. Do not use reflection when working with C# or .NET code.
