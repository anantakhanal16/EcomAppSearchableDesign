# EcomAppSearchableDesign

open sln file in visual studio 
need to run migrations first 
install ef  

dotnet tool install --global dotnet-ef

run migrations 
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign

update
 dotnet ef database update --project Infrastructure   --startup-project EcomAppSearchableDesign


 example

S C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet tool install --global dotnet-ef
 
 this is for running migrations
 S C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign

 
 this is for update 
 S C:\Users\User\source\repos\EcomAppSearchableDesign> dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign
