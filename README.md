# EcomAppSearchableDesign

open sln file in visual studio 
need to run migrations first 
install ef  

dotnet tool install --global dotnet-ef

run migrations 
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign

update
dotnet ef database update --project Infrastructure   --startup-project EcomAppSearchableDesign

this will create tables and database . 
 

 example EcomAppSearchableDesign is our project
 
S C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet tool install --global dotnet-ef

 step1:
 this is for running migrations
 S C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign

step 2
 this is for update 
 S C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign

after migrations is done we can use the api endpoints.

first we need to register user we have admin and user role 
{
  "email": "user1@example.com",
  "password": "Test@123",
  "fullName": "someuser",
  "role": "User"
}
this is for user role 

{
  "email": "Admin@example.com",
  "password": "Admin@123",
  "fullName": "adminUser",
  "role": "Admin"
}

after registering we can add product , create order and export data in pdf and excel (these are only  for admin user)
some role are for user and some are for admins . 


