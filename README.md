# PTP Integrated — Account Management System

An ASP.NET Core 8 MVC web application for managing persons, their accounts, and transaction histories. Built as part of the PTP Integrated Developer Technical Competency Assessment.

---

## Features

- **Person Management** — Create, edit, delete, and search persons by ID number, surname, or account number. List is paginated (10 per page).
- **Account Management** — Add accounts to a person, edit account details, view outstanding balance (auto-calculated), and close or reopen accounts.
- **Transaction Management** — Capture debit and credit transactions against accounts. Balances update automatically. Full edit history with capture date tracking.
- **Business Rule Enforcement** — All rules enforced server-side (unique ID numbers, unique account numbers, no future transaction dates, zero-amount protection, closed account restrictions, delete guards, etc.).
- **Authentication** — ASP.NET Core Identity; login required to access person/account/transaction management.
- **SOLID Principles** — Service layer is fully interface-driven (`IPersonService`, `IAccountService`, `ITransactionService`) with constructor-injected dependencies.

---

## Requirements

### Software

| Requirement | Version |
|---|---|
| .NET SDK | 8.0 or later |
| Microsoft SQL Server | 2008 or later (Express is fine) |
| Visual Studio | 2022 (or VS Code with C# extension) |

### NuGet Packages (restored automatically)

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 8.0.x
- `Microsoft.AspNetCore.Identity.UI` 8.0.x
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.x
- `Microsoft.EntityFrameworkCore.Tools` 8.0.x
- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` 8.0.x

---

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ptpi
```

### 2. Configure the Database Connection

Open `PTPI/appsettings.json` and update the `DefaultConnection` string to point to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PTPI;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance name (e.g., `localhost`, `.\SQLEXPRESS`, or a named instance).

### 3. Apply Database Migrations

From the `PTPI` project folder, run:

```bash
dotnet ef database update
```

This will create the `PTPI` database and all required tables:
- `Persons`
- `Accounts`
- `Transactions`
- ASP.NET Core Identity tables (for authentication)

> **Note:** If you have the provided SQL seed script (`InteractiveMVC_Tables_With_Data.sql`), run it against the database after migrations to populate sample data.

### 4. Run the Application

```bash
dotnet run
```

Or press **F5** in Visual Studio. The application will start on `https://localhost:{port}`.

### 5. Register a User Account

Navigate to the app in your browser, click **Register** in the top-right, and create a login account. You must be logged in to access person, account, and transaction management.

---

## Project Structure

```
PTPI/
├── Controllers/
│   ├── HomeController.cs          # Home, About, Contact pages
│   ├── PersonsController.cs       # Person CRUD + list/search/paging
│   ├── AccountsController.cs      # Account CRUD + close/reopen
│   └── TransactionsController.cs  # Transaction CRUD
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext (Identity + app tables)
│   └── Migrations/                # EF Core migration history
├── Models/
│   ├── Person.cs
│   ├── Account.cs
│   ├── Transaction.cs
│   └── ViewModels/
│       └── PersonListViewModel.cs # Paged person list model
├── Services/
│   ├── Interfaces/
│   │   ├── IPersonService.cs
│   │   ├── IAccountService.cs
│   │   └── ITransactionService.cs
│   ├── PersonService.cs
│   ├── AccountService.cs
│   └── TransactionService.cs
├── Views/
│   ├── Home/                      # Index, About, Contact
│   ├── Persons/                   # Index, Create, Edit, Delete
│   ├── Accounts/                  # Create, Edit
│   ├── Transactions/              # Create, Edit
│   └── Shared/                    # _Layout, _LoginPartial, Error
├── appsettings.json
└── Program.cs
```

---

## Navigation Flow

```
Home
└── Persons (list + search)
    └── Person Details (edit)
        └── Account Details (edit + close/reopen)
            └── Transaction Details (create/edit)
```

---

## Business Rules Enforced

### Persons
- ID Number must be unique — duplicate creation is rejected.
- A person may have an unlimited number of accounts.
- A person can only be deleted if they have no accounts, or all accounts are closed.

### Accounts
- Account numbers must be unique across all persons.
- Accounts can only be added to an existing person (not during person creation).
- Outstanding balance is read-only; it is recalculated automatically from transactions.
- An account cannot be closed if its outstanding balance is not zero.
- No transactions may be posted to a closed account.

### Transactions
- Transaction date cannot be set to a future date.
- Transactions can only be added to an existing account (not during account creation).
- The capture date is set automatically on create and on every edit — it cannot be manually changed.
- Users may enter negative values (debit) or positive values (credit).
- The transaction amount cannot be zero.
