# Customer Insight Hub

A customer management web app built to demonstrate a clean, testable ASP.NET Core backend,
with a React front end, event-driven background processing on Azure, and AI-assisted duplicate
detection planned on top.

> **Status:** work in progress. The REST API, business rules and unit tests are done. The items
> under [Roadmap](#roadmap) are not built yet.

All data in this project is made up. It does not contain any real customer or employer data.

## What works today

- Customers CRUD over a REST API (create, list, get by id, update, delete)
- Input validation with clear error responses (`400`)
- Unique email rule, case-insensitive (`409 Conflict`)
- SQL Server persistence with EF Core migrations
- Unit tests for the business rules, run without a database

## Tech stack

| Area | Technology |
|---|---|
| Backend | ASP.NET Core 9 Web API, C# |
| Data | SQL Server (LocalDB for development), EF Core 9 |
| Tests | xUnit |
| Architecture | Clean Architecture (Domain, Application, Infrastructure, Api) |

## Architecture

```
Controller  ->  Service (business rules)  ->  Repository interface  ->  EF Core  ->  SQL Server
   Api            Application                 Application             Infrastructure
```

| Project | Responsibility |
|---|---|
| `CustomerInsightHub.Domain` | Entities. No framework or database dependencies |
| `CustomerInsightHub.Application` | Use cases, validation, the `ICustomerRepository` contract |
| `CustomerInsightHub.Infrastructure` | EF Core, migrations, repository implementation |
| `CustomerInsightHub.Api` | Controllers, HTTP status mapping, startup |
| `CustomerInsightHub.Tests` | Unit tests |

Design choices worth knowing:

- **Dependencies point inward.** Application knows nothing about SQL Server, so the database
  can change without touching the business rules.
- **Services return a result, not an HTTP response.** `CustomerService` reports
  `Success / Invalid / NotFound / Conflict` and the controller maps that to `200 / 400 / 404 / 409`.
- **Time is injected** (`TimeProvider`) so timestamp logic is testable.
- **Tests use an in-memory fake repository**, so they run in milliseconds and need no database.

## Getting started

Requirements: [.NET 9 SDK](https://dotnet.microsoft.com/download) and SQL Server LocalDB
(installed with Visual Studio) or any SQL Server instance.

```powershell
git clone https://github.com/saqibsyed96/customer-insight-hub.git
cd customer-insight-hub

# restore the EF Core CLI tool pinned in dotnet-tools.json
dotnet tool restore

# create the database and apply migrations
dotnet tool run dotnet-ef database update --project src/CustomerInsightHub.Infrastructure --startup-project src/CustomerInsightHub.Api

# run the API
dotnet run --project src/CustomerInsightHub.Api
```

The default connection string targets `(localdb)\MSSQLLocalDB` and is in
`src/CustomerInsightHub.Api/appsettings.json`. Ready-made requests are in
`src/CustomerInsightHub.Api/CustomerInsightHub.Api.http` (open it in Visual Studio or VS Code).

Run the tests:

```powershell
dotnet test
```

## API

| Method | Route | Success | Failure |
|---|---|---|---|
| `GET` | `/api/customers` | `200` list | |
| `GET` | `/api/customers/{id}` | `200` | `404` |
| `POST` | `/api/customers` | `201` + `Location` header | `400` invalid, `409` duplicate email |
| `PUT` | `/api/customers/{id}` | `200` | `400`, `404`, `409` |
| `DELETE` | `/api/customers/{id}` | `204` | `404` |

Example request:

```http
POST /api/customers
Content-Type: application/json

{
  "name": "Riverstone Bakery",
  "email": "hello@riverstone-bakery.example",
  "city": "Portland",
  "country": "USA",
  "website": "https://riverstone-bakery.example"
}
```

Validation failure (`400`):

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Name is required."],
    "Email": ["Email is not a valid email address."]
  }
}
```

## Roadmap

- [ ] JWT authentication with Admin and Viewer roles
- [ ] React front end: login, customer list with search, create/edit forms
- [ ] Bulk CSV import processed in the background (Azure Functions + Storage Queue)
- [ ] Duplicate customer detection with an AI embeddings API
- [ ] Deployment to Azure with GitHub Actions CI/CD

## Known limitations

- Two simultaneous requests with the same new email can both pass the uniqueness check; the
  database unique index then rejects the second one with a generic `500` instead of a `409`.
- Validation error keys are PascalCase (`Name`) while the rest of the JSON is camelCase.

## License

MIT
