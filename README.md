# 🛍️ E-Commerce Store - ASP.NET Core MVC
![.NET Core](https://img.shields.io/badge/.NET-9.0-blue)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-green)
![EF Core](https://img.shields.io/badge/EF%20Core-6.0-orange)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.0-purple)

A full-featured e-commerce platform built with **ASP.NET Core MVC**, following clean architecture principles and modern web development practices.

## ✨ Features

### 🛒 Core E-Commerce Functionality
- **Product Catalog** with categories, filtering, and search
- **Shopping Cart** system with session management
- **Order Processing** workflow (Cart → Checkout → Payment → Confirmation)
- **User Reviews & Ratings** for products

### 🔐 Authentication & Authorization
- User registration & login with **ASP.NET Core Identity**
- Role-based access control (Customer, Admin)
- Password reset functionality

### 🏪 Admin Dashboard
- **CRUD operations** for products, categories, and inventory
- Order management system
- Sales analytics and reporting

### 🛠️ Technical Implementation
- **Repository Pattern** with Entity Framework Core
- **Dependency Injection** for loose coupling
- **Responsive Design** with Bootstrap 5
- **Client-side validation** with jQuery

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 (or VS Code)
- SQL Server (or SQL Express)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Bishoy-Adel-Faheem/E-CommerceStore.git
   ```

2. **Navigate to project directory:**
   ```bash
   cd E-CommerceStore
   ```

3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

4. **Configure database:**
   - Update connection string in `appsettings.json`
   - Run migrations:
   ```bash
   dotnet ef database update
   ```

5. **Run the application:**
   ```bash
   dotnet run
   ```

## 📂 Project Structure

```
E-CommerceStore/
├── Controllers/        # MVC Controllers
├── Models/             # Domain models and ViewModels
├── Views/              # Razor views
├── Services/           # Business logic layer
├── Data/               # Data access layer (DbContext, Repositories)
├── Migrations/         # Entity Framework migrations
├── wwwroot/            # Static files (CSS, JS, images)
└── appsettings.json    # Configuration file
```

## 🧑‍💻 Development Notes

### Seeding Initial Data
Run the following in Package Manager Console:
```powershell
Update-Database
```

### Admin Access
Default admin credentials:
- **Email:** `admin@store.com`
- **Password:** `Admin@123`

## 🛠️ Built With

- ASP.NET Core MVC
- Entity Framework Core
- Bootstrap 5
- jQuery

## 📝 License

This project is licensed under the MIT License

## 📬 Contact

**Bishoy Adel**
- 📧 Email: bish0yadelfaheeem@gmail.com
- 🔗 [LinkedIn](https://linkedin.com/in/bishoy-adel-faheem)
- 🐱 [GitHub](https://github.com/Bishoy-Adel-Faheem)

---

*This README includes clear project documentation with feature highlights, installation instructions, project structure overview, development notes, technology stack, and contact information. Feel free to customize any section further based on your specific implementation details.*
