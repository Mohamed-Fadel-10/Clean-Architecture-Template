# 🏛️ Fadel.CleanArchTemplate

<div align="center">

[![NuGet Version](https://img.shields.io/nuget/v/Fadel.CleanArchTemplate?color=blue&label=nuget)](https://www.nuget.org/packages/Fadel.CleanArchTemplate)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Fadel.CleanArchTemplate?color=green)](https://www.nuget.org/packages/Fadel.CleanArchTemplate)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com)

A production-ready **.NET Clean Architecture Template** with everything you need to build scalable APIs from day one.

</div>

---

## 📦 Installation

```bash
dotnet new install Fadel.CleanArchTemplate
```

## 🚀 Usage

```bash
dotnet new cleanarch -n YourProjectName
```

---

## ✅ What's Included

| Feature | Description |
|---|---|
| 🏗️ **Clean Architecture** | Domain, Application, Persistence, API, Shared layers |
| 🔐 **ASP.NET Core Identity** | Built-in user management & authentication |
| 🔑 **JWT + Refresh Tokens** | Secure token-based authentication |
| 📨 **MediatR + CQRS** | Clean command/query separation |
| ✔️ **FluentValidation** | Elegant input validation |
| 🗄️ **EF Core + SQL Server** | Database access with Entity Framework Core |
| 🍕 **Vertical Slice Architecture** | Feature-focused code organization |

---

## 📁 Project Structure

After running `dotnet new cleanarch -n MyApp`, you'll get:

```
MyApp/
├── MyApp.API/              # Controllers, Middleware, Program.cs
├── MyApp.Application/      # CQRS Handlers, DTOs, Interfaces
├── MyApp.Domain/           # Entities, Value Objects, Domain Events
├── MyApp.Persistence/      # EF Core, Migrations, Repositories
├── MyApp.Shared/           # Common utilities & Extensions
└── MyApp.sln
```

---

## 📋 Changelog

All notable changes to this project are documented here.  
The format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

### [1.0.0] - 2026-03-07 🎉

#### Added
- Initial release of Fadel.CleanArchTemplate
- Clean Architecture with 5 layers (API, Application, Domain, Persistence, Shared)
- ASP.NET Core Identity integration
- JWT Authentication with Refresh Token support
- MediatR + CQRS pattern setup
- FluentValidation pipeline behavior
- EF Core with SQL Server configuration
- Vertical Slice Architecture structure
- `.template.config` with full rename support via `sourceName`

---

## 🔢 Versioning Guide

This project uses [Semantic Versioning](https://semver.org/):

```
MAJOR.MINOR.PATCH
  │     │     └── Bug fixes, small tweaks
  │     └──────── New features (backwards compatible)
  └────────────── Breaking changes
```

| Version | When to bump |
|---|---|
| `PATCH` (1.0.**x**) | Fixed a bug, updated a NuGet dependency |
| `MINOR` (1.**x**.0) | Added a new Layer, Feature, or Option |
| `MAJOR` (**x**.0.0) | Changed project structure, breaking existing usage |

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "feat: add your feature"`
4. Push to the branch: `git push origin feature/your-feature`
5. Open a Pull Request

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](./LICENSE) file for details.

---

<div align="center">
  Made with ❤️ by <a href="https://github.com/Mohamed-Fadel-10">Fadel</a>
</div>
