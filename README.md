# EcomAppSearchableDesign Setup and Usage Guide

## 1. Open Solution
first clone this project

open cmd and git clone https://github.com/anantakhanal16/EcomAppSearchableDesign.git

Open the `EcomAppSearchableDesign.sln` file in Visual Studio.

## 2. Install EF Core Tools

To run migrations, first install the Entity Framework Core CLI tool:


dotnet tool install --global dotnet-ef

3. Run Migrations
Create the initial migration:
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign

Update the database:
dotnet ef database update --project Infrastructure --startup-project EcomAppSearchableDesign


This will create the necessary tables and the database.
Example:

  # Step 1: Install EF Core CLI
C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet tool install --global dotnet-ef 

# Step 2: Add migration
C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign  

 # Step 3: Update database
C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef database update --project Infrastructure --startup-project EcomAppSearchableDesign 

4. Using API Endpoints
After migrations are done, you can start using the API endpoints.

4.1 Register Users
You need to register users with roles. There are Admin and User roles.

User Role Example:

json
Copy code
{
  "email": "user1@example.com",
  "password": "Test@123",
  "fullName": "someuser",
  "role": "User"
}
Admin Role Example:

json
Copy code
{
  "email": "Admin@example.com",
  "password": "Admin@123",
  "fullName": "adminUser",
  "role": "Admin"
}
4.2 Admin Capabilities
After registering as an Admin, you can:

Add products

Create orders

Export data to PDF and Excel

4.3 Role-Based Access
Some API endpoints are restricted to Admin users.

Other endpoints are accessible to User roles.
