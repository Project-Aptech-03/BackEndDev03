# eProject — Shradha General Book Stores (ASP.NET Core)

---

## 👥 Team Members

| Student ID   | Full Name        | Role                  |
|--------------|-----------------|-----------------------|
| Student1559700 | Nguyễn Minh Cường | Backend Developer     |
| Student1477469 | Nguyễn Tiến Đạt   | Full-stack Developer  |
| Student1559709 | Nguyễn Đức Sinh   | Database Administrator|
| Student1470125 | Lê Trương Tín     | Frontend Developer    |
| Student1545742 | Hoàng Nhật Phúc   | Project Manager       |

---

## 1. 📖 Introduction

In today's digital era, **Shradha General Book Stores** have become essential platforms for readers to conveniently access diverse publications.  

This eProject focuses on building a **comprehensive Shradha General Book Stores** using **ASP.NET Core**, combining **e-commerce functionality** with **content management**.  

Through this project, students will:  
- 🖥️ Develop a **full-stack web application** using ASP.NET Core  
- 🛒 Implement **e-commerce features**: inventory, cart, payment  
- 🗄️ Practice **database design & optimization** with SQL Server  
- 🔗 Integrate multiple modules into a **unified application**  
- 🔐 Gain experience in **authentication, authorization, and security**  
- 🤝 Work collaboratively with **Git** and **Agile methodology**  

---

## 2. 🎯 Objectives

- Design and implement a **complete Online Book Store system** using ASP.NET Core MVC  
- Build a **responsive UI** across devices  
- Implement **ASP.NET Core Identity** for authentication & authorization  
- Create an **Admin Dashboard** for products, orders, and users  
- Integrate **payment methods** and delivery options  
- Add **search, filtering, categorization** features  
- Develop **coupon/discount system**  
- Ensure **data integrity and security**  
- Practice the **full SDLC**: requirements → deployment  

---

## 3. ⚙️ Features & Functionalities

### 🔑 Authentication & Authorization
- User registration & login with ASP.NET Core Identity  
- Role-based access control: `Customer`, `Admin`, `Content Manager`  
- Social media login  
- Password recovery & email confirmation  

### 📦 Product Catalog
- Multi-level categories (Books, Magazines, CDs, DVDs, Utilities)  
- Advanced search & filtering  
- Product details with images, description, specifications  
- Inventory management with stock tracking  

### 🛍️ Shopping Experience
- Shopping cart (persistent)  
- Wishlist  
- Customer addresses with delivery calculation  
- Real-time stock validation  

### 📦 Order Management
- Multi-step checkout  
- Payment options: **Cash on Delivery, Online Payment**  
- Order tracking (Pending → Confirmed → Shipped → Delivered)  
- Email notifications  

### 🎟️ Promotion System
- Coupon codes (percentage, fixed amount, free shipping)  
- Automatic bulk discounts  
- Coupon validation & expiration  

### 🙋 Customer Support
- FAQ management  
- Customer queries & admin replies  
- Return & refund processing  

### 🛠️ Administration
- Dashboard with orders, users, products overview  
- Inventory & stock movement tracking  
- Sales reporting & analytics  
- System configuration  

---

## 4. 🏗️ Technical Architecture

### 🗄️ Database Design
- **24 normalized tables**  
- ASP.NET Core Identity integration  
- Optimized indexes  
- Audit trails for critical operations  

### ⚡ Backend
- ASP.NET Core 7.0 MVC  
- Entity Framework Core (code-first)  
- Repository pattern + Dependency Injection  
- AutoMapper for DTO mapping  
- Serilog for logging  

### 🎨 Frontend
- Razor Pages + **Bootstrap 5**  
- JavaScript / jQuery  
- AJAX for partial updates  
- jQuery Validation for client-side  

### 🔐 Security
- HTTPS enforcement  
- SQL Injection prevention  
- XSS protection  
- CSRF tokens  
- Password hashing  

---

## 5. 🗃️ Database Schema Overview

The system uses **24 tables**, including:  

- **Authentication**: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, ...  
- **Products**: `categories`, `manufacturers`, `publishers`, `products`, `product_photos`  
- **Orders**: `orders`, `order_items`, `shopping_cart`, `customer_addresses`  
- **Payments & Returns**: `payments`, `product_returns`  
- **Inventory**: `stock_movements`  
- **Promotions**: `coupons`  
- **Support**: `customer_queries`, `admin_replies`, `faq`  
- **System Config**: `system_settings`  

---

## 6. 📦 Deliverables

- **Bi-weekly Status Reports** (every 10 days)  
  - Progress & completed features  
  - Code review documentation  
  - Issues & solutions  
  - Updated timeline  

- **Final Submission**  
  - Source code + documentation  
  - Database schema + migrations  
  - Technical docs: architecture, API, deployment guide  
  - User manuals  
  - Test cases & results  
  - Presentation + demo video  

---

## 7. 💻 Requirements

### Development
- CPU: Intel Core i5+  
- RAM: 8GB+  
- Storage: 20GB SSD  
- OS: Windows 10/11 / macOS Big Sur+  

**Tools:**  
- IDE: Visual Studio 2022 (v17.4+)  
- SDK: .NET 7.0  
- DB: SQL Server 2019+  
- Version Control: Git + GitHub/GitLab  
- UI Design: Figma / Adobe XD  

### Production
- Web Server: IIS / Azure App Service  
- DB: Azure SQL / SQL Server 2019  
- Cache: Redis (optional)  
- Media Storage: Azure Blob Storage  

---

## 8. 🗓️ Project Timeline

| Week | Milestone |
|------|-----------|
| 1-2  | Requirement Analysis & Database Design |
| 3-4  | Authentication & Authorization |
| 5-6  | Product Catalog & Shopping Cart |
| 7-8  | Order Processing & Payment Integration |
| 9-10 | Admin Dashboard & Reporting |
| 11   | Testing & Bug Fixing |
| 12   | Documentation & Final Presentation |

---

## 9. 📝 Notes

- Use **Git Feature Branch Workflow** with clear commit messages  
- Follow **clean architecture principles**  
- Ensure **responsive UI** for mobile & desktop  
- Conduct **peer code reviews** before merging  
- Write **unit tests** for critical logic  
- Perform **security testing** (OWASP Top 10)  
- Prepare **CI/CD pipeline** for deployment  
- Record **demo video** for final submission  

---
✨ *Project: Online Book Store (ASP.NET Core, EF Core, SQL Server)*  
