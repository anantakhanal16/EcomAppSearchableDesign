EcomAppSearchableDesign – Step-by-Step Setup Guide

Tech Stack: .NET 10, Clean Architecture, SQL Server, EF Core

Step 1: Open the Solution

Open your solution in Visual Studio:

File → Open → Project/Solution → EcomAppSearchableDesign.sln

Step 2: Install EF Core CLI Tool

Install the Entity Framework Core global tool to run migrations:

C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet tool install --global dotnet-ef


This allows you to run dotnet ef commands from anywhere.

Step 3: Create Initial Migration

Run this command to generate the initial migration for the database tables:

C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign


--project Infrastructure → EF Core context is in the Infrastructure project

--startup-project EcomAppSearchableDesign → Startup project with DI and configurations

Step 4: Update Database

Apply the migration to create the database and tables:

C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef database update --project Infrastructure --startup-project EcomAppSearchableDesign


✅ After this, your database and tables are ready.

Step 5: Register Users

Use your API endpoints to register users. Examples:

User Role:

POST /api/auth/register
{
  "email": "user1@example.com",
  "password": "Test@123",
  "fullName": "someuser",
  "role": "User"
}


Admin Role:

POST /api/auth/register
{
  "email": "Admin@example.com",
  "password": "Admin@123",
  "fullName": "adminUser",
  "role": "Admin"
}

Step 6: Role-Based Actions

Admin User can:

Add products

Create orders

Export data to PDF and Excel

Regular User can:

Access only user-level endpoints

Step 7: Start Using API

After registration, you can now:

Log in with JWT authentication

Perform CRUD operations for products and orders

Access export features (PDF/Excel) as admin

✅ This workflow follows Clean Architecture:

Infrastructure → Database & EF Core

Application → Business logic & CQRS

API → Controllers & endpoints
