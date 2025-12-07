
[Uploading ecom_readme.md…]()
# EcomAppSearchableDesign 🛒

A full-featured e-commerce backend API built with .NET Core, implementing Clean Architecture principles with comprehensive product, order, and user management capabilities.

## ✨ Features

### 🔐 Authentication & Authorization
- JWT-based authentication with secure token management
- Role-based access control (Admin & User roles)
- User registration and login
- Profile management

### 📦 Product Management
- Full CRUD operations for products
- Product image upload support
- Pagination for large product catalogs
- Bulk product import from Excel files
- Advanced search and filtering capabilities

### 🛍️ Shopping Cart
- Add/remove products from cart
- Update item quantities
- Cart persistence across sessions
- Clear cart functionality
- View cart summary

### 📝 Order Management
- Complete order placement workflow
- Order history with detailed information
- Advanced search, filter, and pagination
- Order status tracking
- Admin order management (update/cancel)

### 📊 Data Export
- Export orders to Excel format
- Generate PDF reports
- Customizable export templates

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/anantakhanal16/EcomAppSearchableDesign.git
cd EcomAppSearchableDesign
```

2. **Install EF Core Tools** (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

3. **Update Connection String**
   
   Open `appsettings.json` in the `EcomAppSearchableDesign` project and update the connection string to match your SQL Server instance.

4. **Run Database Migrations**
```bash
# Create migration
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project EcomAppSearchableDesign

# Apply migration to database
dotnet ef database update --project Infrastructure --startup-project EcomAppSearchableDesign
```

5. **Run the Application**
```bash
dotnet run --project EcomAppSearchableDesign
```

The API will be available at `https://localhost:7xxx` or `http://localhost:5xxx`

## 📖 API Usage

### Register Users

**User Registration**
```json
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "User@123",
  "fullName": "John Doe",
  "role": "User"
}
```

**Admin Registration**
```json
POST /api/auth/register
{
  "email": "admin@example.com",
  "password": "Admin@123",
  "fullName": "Admin User",
  "role": "Admin"
}
```

### Authentication

**Login**
```json
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "User@123"
}
```

Response includes JWT token for authenticated requests.

### Role-Based Access

#### Admin-Only Endpoints
- `POST /api/products` - Create products
- `PUT /api/products/{id}` - Update products
- `DELETE /api/products/{id}` - Delete products
- `POST /api/products/import` - Bulk import from Excel
- `PUT /api/orders/{id}` - Update order status
- `DELETE /api/orders/{id}` - Cancel orders

#### User Endpoints
- `GET /api/products` - View products
- `POST /api/cart` - Manage shopping cart
- `POST /api/orders` - Place orders
- `GET /api/orders` - View own orders

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

- **Domain Layer** - Core business entities and logic
- **Application Layer** - Use cases, DTOs, and interfaces
- **Infrastructure Layer** - Data access, external services
- **API Layer** - Controllers, middleware, and configuration

## 🛠️ Tech Stack

- **Framework**: .NET 8.0 / ASP.NET Core Web API
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Architecture**: Clean Architecture / Onion Architecture
- **File Processing**: EPPlus for Excel, iTextSharp/QuestPDF for PDF
- **Documentation**: Swagger/OpenAPI

## 📝 API Documentation

After running the application, access the Swagger UI at:
```
https://localhost:7xxx/swagger
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👨‍💻 Author

**Ananta Khanal**
- GitHub: [@anantakhanal16](https://github.com/anantakhanal16)

## ⭐ Show Your Support

If you find this project useful, please consider giving it a star on GitHub!

---

**Note**: This is a learning/portfolio project demonstrating modern .NET development practices including Clean Architecture, CQRS patterns, and comprehensive API design.
