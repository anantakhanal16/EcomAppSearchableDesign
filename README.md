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

Sample Example:

  # Step 1: Install EF Core CLI
C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet tool install --global dotnet-ef 

# Step 2: Add migration
C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign  

 # Step 3: Update database
C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef database update --project Infrastructure --startup-project EcomAppSearchableDesign 

 Using API Endpoints
After migrations are done, you can start using the API endpoints.

4.1 Register Users
You need to register users with roles. There are Admin and User roles.

User Role Example:
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

if u have read this  leave a comment or star.


Key Features
🛒 User Account & Authentication

Secure user registration and login with JWT

Role-based access: Admin and User

View and manage user profile

Logout functionality

📦 Product Management (Admin)

Create, update, and delete products

Upload product images

View all products with pagination

Import product data from Excel files

🛍 Shopping Cart

Add products to cart

Update or remove items in cart

Clear entire cart

View current cart contents

📝 Order Management

Place new orders

View order details

View all user orders with search, filter, and pagination

Update or cancel orders (Admin)

Export orders in Excel or PDF

🔐 Security

JWT authentication for all sensitive endpoints
Tech Stack

Backend: .NET Core Web API

Database: SqlServer

Authentication: JWT

File Uploads: Excel, Images

Export: Excel & PDF
