EcomAppSearchableDesign – Quick Setup & Cheat Sheet

Tech Stack Overview:

Backend: .NET 10 (Clean Architecture)

Database: SQL Server

ORM: Entity Framework Core

Frontend: Can be integrated with any frontend (React/Next.js recommended)

Other: JWT Authentication, Role-based Authorization (Admin/User)

1. Open Project

Open the .sln file in Visual Studio 2022+.

2. Install EF Core CLI

Install EF Core tools globally to run migrations:

dotnet tool install --global dotnet-ef

3. Run Migrations
Step 1: Create Initial Migration
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign

Step 2: Update Database
dotnet ef database update --project Infrastructure --startup-project EcomAppSearchableDesign


✅ This creates all required tables in your SQL Server database.

4. Register Users

Register users with roles before using other endpoints.

User Role Example
{
  "email": "user1@example.com",
  "password": "Test@123",
  "fullName": "someuser",
  "role": "User"
}

Admin Role Example
{
  "email": "Admin@example.com",
  "password": "Admin@123",
  "fullName": "adminUser",
  "role": "Admin"
}


Authentication uses JWT tokens stored in cookies or headers.

5. API Actions by Role
Action	User	Admin
Add Product	❌	✅
Create Order	❌	✅
Export Data (PDF/Excel)	❌	✅
View Orders	✅	✅

Role-based authorization ensures certain features are restricted to Admins only.

6. Folder/Project Structure (Clean Architecture)
EcomAppSearchableDesign
│
├─ Infrastructure      # EF Core, DB context, Migrations
├─ Application         # Business logic, CQRS, MediatR
├─ Domain              # Entities, Value Objects
├─ API (Startup Project)
│   └─ Controllers     # API Endpoints


This cheat sheet is all-in-one: commands, JSON samples, role info, tech stack, and clean architecture structure
