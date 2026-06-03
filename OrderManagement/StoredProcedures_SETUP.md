# Stored Procedures Setup & Connection Guide

This document explains how to create the database, run the SQL scripts in this repository, and connect the application for local development. It covers Docker (quick start), local SQL Server (SSMS/Azure Data Studio), and Azure SQL.

> Location of SQL files in this repo: `OrderManagement/StoredProcedures/`
```bash
  - Alternatively: Project Properties → Debug → Environment variables, add `ConnectionStrings__DefaultConnection` and `JwtSettings__Key` etc. (double-underscore for nested keys).
  - Ensure the project starts in Development environment (launch settings) so `dotnet user-secrets` or `secrets.json` are used.
Troubleshooting in SSMS and Visual Studio
- If scripts fail with permission errors, run SSMS as Administrator or ensure the SQL login has `db_owner` on `OrdersDb` while setting up.
- If a type or procedure is missing, double-check you executed `OrderItemTableType.sql` before the stored procedures and that the target database is `OrdersDb`.
- Local Docker / SQL Server:
dotnet ef database update --project OrderManagement/Orders.csproj
# Stored Procedures Setup — Visual Studio Community & SSMS

This document contains a concise, focused guide for setting up the database and running the stored procedures using Visual Studio Community and SQL Server Management Studio (SSMS).

Location of SQL files: `OrderManagement/StoredProcedures/`

## Prerequisites
- Visual Studio Community
- SQL Server instance (local or remote)
- SQL Server Management Studio (SSMS)

## Steps (SSMS + Visual Studio)

1. Open Visual Studio Community and load `OrdersManagement.slnx`.
2. Open SSMS and connect to your SQL Server instance.
3. Create a database named `OrdersDb` (right-click `Databases` → `New Database...`).
4. In SSMS, open and run the SQL files from `OrderManagement/StoredProcedures/` in this order:
  - `OrderItemTableType.sql` (create TVP types)
  - `sp_CreateOrder.sql`
  - `sp_InsertOrder.sql`
  - `sp_UpdateOrderStatus.sql`
  Ensure the target database is `OrdersDb` (run `USE OrdersDb;` if needed).
5. Verify objects in SSMS:
  - `Databases` → `OrdersDb` → `Programmability` → `Types` (table types)
  - `Databases` → `OrdersDb` → `Programmability` → `Stored Procedures`
6. Configure the app connection in Visual Studio:
  - Right-click `OrderManagement` project → `Manage User Secrets` and add `ConnectionStrings:DefaultConnection` and `JwtSettings` entries, or
  - Project Properties → Debug → Environment variables (`ConnectionStrings__DefaultConnection`, etc.).
7. Run the API from Visual Studio (F5). Open Swagger at `/swagger` to test endpoints.

## Quick checks
- Confirm stored procedures exist: `SELECT name FROM sys.objects WHERE type = 'P';`
- Confirm table types: `SELECT * FROM sys.types WHERE is_table_type = 1;`

## Troubleshooting
- If a procedure/type is missing, re-run `OrderItemTableType.sql` first, then the procs.
- If you get login or permission errors, ensure the SQL login has appropriate permissions (use `db_owner` during setup).

---

If you want, I can add a short PowerShell script that opens the SQL files in SSMS or a small `scripts/setup-db.ps1` to run the scripts against a local instance. Tell me if you'd like that.
```sql
SELECT * FROM sys.types WHERE is_table_type = 1;
```

---

## Example: calling a stored procedure manually

```sql
EXEC sp_CreateOrder @CustomerId = '00000000-0000-0000-0000-000000000000', @RestaurantId = '11111111-1111-1111-1111-111111111111';
```

For procedures that accept table-valued parameters, follow the proc signature and create a table variable matching the type before calling.

---

## Repository integration checks
- Make sure the parameter names used in ADO.NET calls match the SQL script parameter names (e.g., `@Items`, `@OrderId`, `@Remarks`).
- Ensure the `TypeName` used in `SqlParameter` (e.g., `OrderItemTableType`) matches the type name in `OrderItemTableType.sql`.

---

## Troubleshooting
- **Login failed**: verify username/password and firewall rules (Azure).
- **Type or procedure not found**: ensure scripts ran in the correct database and in the correct order (table types first).
- **Permission denied**: use a DB owner for initial setup, then lock down permissions later.
- If using Docker, view container logs:

```bash
docker logs orders-sql
```

---

## Automation (optional)
I can add a small shell script `scripts/setup-db.sh` that runs Docker + sqlcmd commands to create the DB and run the SQL files. Tell me if you want that and whether you prefer Docker or native SQL Server instructions.

---

## Quick checklist for a teammate
- Start SQL Server (Docker or local)
- Create DB `OrdersDb`
- Run `OrderItemTableType.sql` then `sp_CreateOrder.sql`, `sp_InsertOrder.sql`, `sp_UpdateOrderStatus.sql`
- Set `ConnectionStrings:DefaultConnection` via user-secrets or environment variable
- Start the API: `dotnet run --project OrderManagement/Orders.csproj`
- Test with Postman/Swagger
