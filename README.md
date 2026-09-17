# ABC Retail Web Application

![Azure](https://img.shields.io/badge/Azure-0078D4?style=for-the-badge&logo=microsoft-azure&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-9.0-purple?style=for-the-badge&logo=.net&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=for-the-badge&logo=asp.net&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)

A modern, cloud-based retail management system built with ASP.NET Core MVC and integrated with Microsoft Azure cloud services. This application demonstrates enterprise-grade cloud architecture for managing customers, products, and orders with real-time analytics and automated workflows.

## 📋 Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Azure Services Integration](#azure-services-integration)
- [Screenshots](#screenshots)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Deployment](#deployment)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## 🎯 Project Overview

ABC Retail Web Application is a comprehensive retail management system designed to address the challenges of traditional on-premises infrastructure. The application leverages Microsoft Azure cloud services to provide:

- **Scalable order processing** during peak shopping seasons
- **Reliable message queuing** for transaction processing
- **Efficient file storage** for product images and documents
- **Real-time data analytics** for business insights
- **Automated workflows** for customer and product updates

This project demonstrates the integration of four key Azure storage services:
1. **Azure Table Storage** - Customer, Product, and Order data
2. **Azure Blob Storage** - Product images
3. **Azure Queue Storage** - Order transaction messages
4. **Azure File Storage** - Document management

### Business Problem Solved

The traditional on-premises infrastructure faced several challenges:
- Struggled to handle increasing transaction volumes during peak seasons
- Slow access times for product images stored in network shared drives
- Legacy message queuing system with delivery delays and processing errors
- Inconsistent messaging delivery and difficulty in scaling infrastructure
- Challenges in real-time event processing and data analytics

This cloud-based solution addresses all these issues with a modern, scalable architecture.

## ✨ Features

### Customer Management
- ✅ Create, view, edit, and delete customer profiles
- ✅ Advanced search functionality (name, email, phone)
- ✅ Real-time statistics dashboard
- ✅ Customer profile with avatar system
- ✅ Automatic order updates when customer information changes
- ✅ Responsive design for mobile access

### Product Management
- ✅ Comprehensive product catalog management
- ✅ Product image upload to Azure Blob Storage
- ✅ Category-based organization
- ✅ Stock quantity tracking
- ✅ Price management with automatic order updates
- ✅ Search and filter functionality
- ✅ Inventory value calculations

### Order Management
- ✅ Dropdown-based order creation (no manual GUID entry)
- ✅ Customer and product selection with readable names
- ✅ Live total amount calculation
- ✅ Automatic order ID generation (ORD-yyyyMMdd-XXXX format)
- ✅ Order status tracking (Pending, Processing, Shipped, Delivered, Cancelled)
- ✅ Shipping address auto-fill from customer data
- ✅ Queue message integration for order processing
- ✅ Order editing with status change capability
- ✅ Automatic order updates when product information changes

### File Management
- ✅ Document upload to Azure File Storage
- ✅ File download capability
- ✅ File deletion with confirmation
- ✅ File listing with status indicators
- ✅ Integration with retail-documents share

### Dashboard
- ✅ Real-time statistics (customers, products, orders, revenue)
- ✅ Recent orders display
- ✅ Order status overview with progress indicators
- ✅ Quick action cards
- ✅ System information panel

### Technical Features
- ✅ Automatic data synchronization (customer/product changes update orders)
- ✅ Responsive design (mobile-friendly)
- ✅ Modern UI with gradient styling
- ✅ Sticky navigation header
- ✅ Proper footer positioning
- ✅ Client-side search and filtering
- ✅ Success/error message feedback
- ✅ Anti-forgery token protection
- ✅ Async/await for all operations

## 🛠️ Technology Stack

### Backend
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core MVC** - Web application framework
- **C#** - Programming language
- **Dependency Injection** - Service pattern implementation

### Frontend
- **Razor Views** - Server-side rendering
- **Bootstrap 5.3** - UI framework
- **Font Awesome 6.4** - Icon library
- **Custom CSS** - Modern gradient styling
- **JavaScript** - Client-side interactivity

### Cloud Services
- **Azure Table Storage** - NoSQL data storage
- **Azure Blob Storage** - Binary object storage
- **Azure Queue Storage** - Message queuing
- **Azure File Storage** - File share storage
- **Azure Function App** - Serverless functions (separate project)

### Development Tools
- **Visual Studio 2022** - IDE
- **Git** - Version control
- **GitHub** - Code repository
- **Azure Portal** - Cloud management

## 🏗️ Architecture

### System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     User Interface Layer                     │
│  (ASP.NET Core MVC - Responsive Web Application)            │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ├──────────────────────────────────────┐
                     │         Service Layer                  │
                     │  ┌─────────────────────────────────┐  │
                     │  │ TableStorageService              │  │
                     │  │ BlobStorageService              │  │
                     │  │ QueueStorageService             │  │
                     │  │ FileStorageService              │  │
                     │  └─────────────────────────────────┘  │
                     └──────────────────────────────────────┘
                     │
        ┌────────────┼────────────┬──────────────┬──────────┐
        │            │            │              │          │
        ▼            ▼            ▼              ▼          ▼
┌──────────────┐ ┌────────────┐ ┌──────────┐ ┌────────┐ ┌─────────────┐
│   Azure      │ │   Azure    │ │  Azure   │ │ Azure  │ │   Azure     │
│  Table       │ │   Blob     │ │  Queue   │ │  File  │ │  Function   │
│  Storage     │ │  Storage   │ │ Storage  │ │Storage │ │    App      │
└──────────────┘ └────────────┘ └──────────┘ └────────┘ └─────────────┘
│ Customers    │ │ product-   │ │ order-   │ │ retail-│ │ 4 Functions │
│ Products     │ │ images     │ │ transact. │ │ docs   │ │ (separate)  │
│ Orders       │ │            │ │          │ │        │ │             │
└──────────────┘ └────────────┘ └──────────┘ └────────┘ └─────────────┘
```

### Data Flow

1. **Order Creation Flow**:
   ```
   User → Order Form → OrderController → TableStorageService → Azure Table (Orders)
                                                      ↓
                                             QueueStorageService → Azure Queue (order-transactions)
   ```

2. **Customer Update Flow**:
   ```
   User → Edit Customer → CustomerController → TableStorageService → Azure Table (Customers)
                                                        ↓
                                             UpdateOrdersForCustomerAsync → Azure Table (Orders)
   ```

3. **Product Image Upload Flow**:
   ```
   User → Edit Product → ProductController → BlobStorageService → Azure Blob (product-images)
                                                       ↓
                                             TableStorageService → Azure Table (Products)
   ```

4. **File Upload Flow**:
   ```
   User → File Management → FileController → FileStorageService → Azure File (retail-documents/uploads)
   ```

## ☁️ Azure Services Integration

### 1. Azure Table Storage

**Purpose**: Store structured data for Customers, Products, and Orders

**Tables Created**:
- `Customers` - Customer profiles with contact information
- `Products` - Product catalog with pricing and inventory
- `Orders` - Order records with customer and product details

**Key Features**:
- NoSQL database with schema-less design
- Automatic partitioning for scalability
- ACID transactions within partitions
- OData protocol support

**Data Model**:
```csharp
// Customer Entity
PartitionKey: CustomerId
RowKey: CustomerId
Properties: FirstName, LastName, Email, PhoneNumber, Address, CreatedAt

// Product Entity
PartitionKey: ProductId
RowKey: ProductId
Properties: Name, Description, Price, StockQuantity, Category, ImageUrl, CreatedAt

// Order Entity
PartitionKey: OrderId
RowKey: OrderId
Properties: OrderId, CustomerId, CustomerName, ProductId, ProductName, 
            Quantity, UnitPrice, TotalAmount, OrderDate, Status, ShippingAddress
```

### 2. Azure Blob Storage

**Purpose**: Store product images efficiently

**Container Created**:
- `product-images` - Stores all product images

**Key Features**:
- Optimized for binary data storage
- CDN integration capability
- Tiered storage (Hot, Cool, Archive)
- Automatic redundancy (LRS, GRS, RA-GRS)

**Blob Naming Convention**:
```
{ProductId}_{OriginalFileName}
Example: a1b2c3d4-e5f6-7890-abcd-ef1234567890_laptop.jpg
```

### 3. Azure Queue Storage

**Purpose**: Reliable message queuing for order processing

**Queue Created**:
- `order-transactions` - Order transaction messages

**Key Features**:
- FIFO message delivery
- Message visibility timeout
- At-least-once delivery guarantee
- Poisson distribution for load balancing

**Message Format**:
```json
{
  "OrderId": "ORD-20250115-ABCD",
  "CustomerId": "customer-guid",
  "CustomerName": "John Doe",
  "ProductName": "Laptop",
  "Quantity": 1,
  "TotalAmount": 5000.00,
  "Status": "Pending",
  "Message": "Order created"
}
```

### 4. Azure File Storage

**Purpose**: Store documents and files

**Share Created**:
- `retail-documents` - Document storage share

**Directory Structure**:
```
retail-documents/
└── uploads/
    ├── invoice.pdf
    ├── shipping_manifest.docx
    └── product_manual.pdf
```

**Key Features**:
- SMB protocol support
- File share mount capability
- Directory structure support
- Azure AD integration

### 5. Azure Function App (Separate Project)

**Purpose**: Serverless functions for Azure service demonstrations

**Functions Created**:
1. `TableStorageFunction` - Store data in Azure Tables
2. `BlobStorageFunction` - Write to Blob Storage
3. `QueueStorageFunction` - Send/receive queue messages
4. `FileStorageFunction` - Write to Azure Files

**Configuration**:
- Function App Name: `st10381088`
- Runtime: .NET 9 Isolated
- Region: South Africa North
- Plan: Consumption (Windows)

## 📸 Screenshots

### Application Screenshots

*Add screenshots of your application here*

1. **Dashboard** - Overview with statistics and recent orders
2. **Customer Management** - Customer list with search
3. **Customer Create Form** - New customer creation
4. **Product Management** - Product catalog grid
5. **Product Create Form** - New product with image upload
6. **Order Management** - Order list with status badges
7. **Order Create Form** - Dropdown-based order creation
8. **File Management** - Document upload and listing

### Azure Services Screenshots

*Add screenshots from Azure Portal*

1. **Storage Account Overview** - st10381088 details
2. **Azure Tables** - Customers, Products, Orders tables
3. **Azure Blob Container** - product-images with blobs
4. **Azure Queue** - order-transactions with messages
5. **Azure File Share** - retail-documents with uploads directory
6. **Azure Function App** - st10381088 with 4 functions
7. **Function Code** - Screenshots of each function's code

## 📦 Prerequisites

### Required Software

- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Visual Studio 2022** (17.8 or later) - [Download](https://visualstudio.microsoft.com/downloads/)
- **Git** - [Download](https://git-scm.com/downloads)
- **Azure Account** with available credit
- **Azure Storage Emulator** (for local development) - Optional

### Required Azure Services

- Azure Storage Account (`st10381088`)
- Azure Function App (`st10381088`)
- Access keys and connection strings

### Required NuGet Packages

```xml
<PackageReference Include="Azure.Data.Tables" Version="12.10.0" />
<PackageReference Include="Azure.Storage.Blobs" Version="12.23.0" />
<PackageReference Include="Azure.Storage.Queues" Version="12.21.0" />
<PackageReference Include="Azure.Storage.Files.Shares" Version="12.20.0" />
```

## 🚀 Installation

### Step 1: Clone the Repository

```bash
git clone https://github.com/Leendouh/ABCRetailWebApp.git
cd ABCRetailWebApp
```

### Step 2: Restore NuGet Packages

```bash
cd ABCRetailWebApp
dotnet restore
```

### Step 3: Configure Application Settings

Open `appsettings.json` and update the Azure Storage connection string:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AzureStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=st10381088;AccountKey=YOUR_ACCOUNT_KEY;EndpointSuffix=core.windows.net"
  },
  "AllowedHosts": "*"
}
```

**To get your connection string**:
1. Navigate to Azure Portal → Storage Account `st10381088`
2. Click "Access keys" in the left menu
3. Copy the "Connection string" under key1
4. Replace `YOUR_ACCOUNT_KEY` with the actual connection string

### Step 4: Build the Application

```bash
dotnet build
```

### Step 5: Run the Application

```bash
dotnet run
```

The application will start at `https://localhost:5001` or `http://localhost:5000`

## ⚙️ Configuration

### Azure Storage Configuration

The application uses the following Azure Storage configuration:

| Setting | Value | Description |
|---------|-------|-------------|
| Storage Account | `st10381088` | Name of the storage account |
| Region | South Africa North | Azure region for resources |
| Performance | Standard | Storage performance tier |
| Redundancy | LRS | Locally redundant storage |
| Tables | Customers, Products, Orders | Data tables |
| Blob Container | product-images | Image storage |
| Queue | order-transactions | Message queue |
| File Share | retail-documents | Document storage |

### Application Settings

Key configuration options in `appsettings.json`:

```json
{
  "AzureStorage": {
    "ConnectionString": "Your connection string here"
  }
}
```

### Culture Configuration

The application is configured for South African Rand (ZAR):

```csharp
var cultureInfo = new CultureInfo("en-ZA");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
```

## 📖 Usage

### Creating a Customer

1. Navigate to **Customers** from the navigation menu
2. Click **Add New Customer**
3. Fill in the customer details:
   - First Name
   - Last Name
   - Email
   - Phone Number
   - Address
4. Click **Create Customer**
5. The customer is saved to Azure Table Storage

### Creating a Product

1. Navigate to **Products** from the navigation menu
2. Click **Add New Product**
3. Fill in the product details:
   - Name
   - Description
   - Price (R)
   - Stock Quantity
   - Category
   - Product Image (optional)
4. Click **Create Product**
5. The product is saved to Azure Table Storage
6. If an image is uploaded, it's stored in Azure Blob Storage

### Creating an Order

1. Navigate to **Orders** from the navigation menu
2. Click **Create New Order**
3. Select a customer from the dropdown (shows "FirstName LastName — Email")
4. Select a product from the dropdown (shows "Product Name — R Price")
5. Enter the quantity
6. The total amount is calculated automatically
7. The shipping address is auto-filled from the selected customer
8. Click **Create Order**
9. The order is saved to Azure Table Storage
10. A message is sent to the Azure Queue (order-transactions)

### Managing Files

1. Navigate to **Files** from the navigation menu
2. Click **Choose File** and select a document
3. Click **Upload File**
4. The file is uploaded to Azure File Storage (retail-documents/uploads)
5. You can download or delete files from the list

### Automatic Data Synchronization

When you update a customer's name or product information, all related orders are automatically updated:

- **Customer Name Change**: Updates `CustomerName` in all orders for that customer
- **Product Name Change**: Updates `ProductName` in all orders for that product
- **Product Price Change**: Updates `UnitPrice` and recalculates `TotalAmount` in all orders

## 🔌 API Endpoints

### Customer Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Customer` | List all customers |
| GET | `/Customer/Details/{id}` | Get customer details |
| GET | `/Customer/Create` | Create customer form |
| POST | `/Customer/Create` | Create new customer |
| GET | `/Customer/Edit/{id}` | Edit customer form |
| POST | `/Customer/Edit/{id}` | Update customer |
| GET | `/Customer/Delete/{id}` | Delete customer confirmation |
| POST | `/Customer/Delete/{id}` | Delete customer |

### Product Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Product` | List all products |
| GET | `/Product/Details/{id}` | Get product details |
| GET | `/Product/Create` | Create product form |
| POST | `/Product/Create` | Create new product |
| GET | `/Product/Edit/{id}` | Edit product form |
| POST | `/Product/Edit/{id}` | Update product |
| GET | `/Product/Delete/{id}` | Delete product confirmation |
| POST | `/Product/Delete/{id}` | Delete product |

### Order Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Order` | List all orders |
| GET | `/Order/Details/{id}` | Get order details |
| GET | `/Order/Create` | Create order form |
| POST | `/Order/Create` | Create new order |
| GET | `/Order/Edit/{id}` | Edit order form |
| POST | `/Order/Edit/{id}` | Update order |
| GET | `/Order/Delete/{id}` | Delete order confirmation |
| POST | `/Order/Delete/{id}` | Delete order |

### File Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/File` | List all files |
| POST | `/File/Upload` | Upload file |
| GET | `/File/Download/{fileName}` | Download file |
| POST | `/File/Delete/{fileName}` | Delete file |

## 🌐 Deployment

### Deploy to Azure App Service

#### Using Visual Studio

1. Open the project in Visual Studio
2. Right-click the project → **Publish**
3. Choose **Azure** → **Azure App Service (Windows)**
4. Sign in to your Azure account
5. Click **Create new** or select existing App Service
6. Configure:
   - Resource Group: `ABCRetailRG`
   - App Service Plan: Create new (South Africa North)
   - Runtime Stack: .NET 9
   - Region: South Africa North
7. Click **Create** and wait for deployment
8. Note the URL: `http://student_number.azurewebsites.net`

#### Using Azure CLI

```bash
# Create resource group
az group create --name ABCRetailRG --location southafricanorth

# Create App Service plan
az appservice plan create --name ABCRetailPlan --resource-group ABCRetailRG --location southafricanorth --sku B1

# Create web app
az webapp create --name your-app-name --resource-group ABCRetailRG --plan ABCRetailPlan --runtime "DOTNETCORE|9.0"

# Deploy
az webapp up --name your-app-name --resource-group ABCRetailRG --location southafricanorth
```

#### Using GitHub Actions

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure Web App

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2

    - name: Set up .NET Core
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '9.0.x'

    - name: Build with dotnet
      run: dotnet build --configuration Release

    - name: Publish with dotnet
      run: dotnet publish -c Release -o ./publish

    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'your-app-name'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

### Deploy Azure Functions

The Azure Functions are in a separate project. To deploy:

```bash
cd ABCRetailFunctions
func azure functionapp publish st10381088
```

Or use Visual Studio:
1. Open the Functions project
2. Right-click → **Publish**
3. Select **Azure Function App (Windows)**
4. Choose existing Function App: `st10381088`
5. Click **Publish**

## 🧪 Testing

### Manual Testing Checklist

#### Customer Management
- [ ] Create a new customer
- [ ] Search for a customer
- [ ] View customer details
- [ ] Edit customer information
- [ ] Delete a customer
- [ ] Verify order updates when customer name changes

#### Product Management
- [ ] Create a new product
- [ ] Upload product image
- [ ] Search for a product
- [ ] View product details
- [ ] Edit product information
- [ ] Update product price
- [ ] Delete a product
- [ ] Verify order updates when product changes

#### Order Management
- [ ] Create an order with dropdowns
- [ ] Verify total calculation
- [ ] Verify shipping address auto-fill
- [ ] Check queue message in Azure Portal
- [ ] Edit order status
- [ ] Edit order customer/product
- [ ] View order details
- [ ] Delete an order

#### File Management
- [ ] Upload a document
- [ ] Download a document
- [ ] Delete a document
- [ ] Verify file in Azure Portal

### Automated Testing

Run the built application tests:

```bash
dotnet test
```

### Azure Service Verification

1. **Azure Table Storage**:
   - Navigate to Azure Portal → Storage Account → Tables
   - Verify data in Customers, Products, Orders tables

2. **Azure Blob Storage**:
   - Navigate to Azure Portal → Storage Account → Containers
   - Verify images in product-images container

3. **Azure Queue Storage**:
   - Navigate to Azure Portal → Storage Account → Queues
   - Verify messages in order-transactions queue

4. **Azure File Storage**:
   - Navigate to Azure Portal → Storage Account → File shares
   - Verify files in retail-documents/uploads

5. **Azure Functions**:
   - Navigate to Azure Portal → Function App → st10381088
   - Verify all 4 functions are deployed and running

## 🤝 Contributing

This is an academic project. For contributions:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is created for academic purposes for the module PROG121 at your institution.

## 👨‍💻 Author

**Student Name**: [Your Name]
**Student Number**: [Your Student Number]
**Module**: PROG121
**Project**: Project 2 - Integrating Azure Services into a Web Application

## 📞 Contact

- **GitHub**: [Leendouh](https://github.com/Leendouh)
- **Repository**: [ABCRetailWebApp](https://github.com/Leendouh/ABCRetailWebApp.git)
- **Deployed Application**: [URL after deployment]

## 🙏 Acknowledgments

- Microsoft Azure for cloud services
- ASP.NET Core team for the framework
- Bootstrap team for the UI framework
- Font Awesome for the icon library
- Module lecturer for guidance and support

## 📚 Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Azure Storage Documentation](https://docs.microsoft.com/azure/storage)
- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.3/getting-started/introduction/)
- [Font Awesome Documentation](https://fontawesome.com/docs)

## 🗺️ Roadmap

### Completed Features
- ✅ Customer CRUD operations
- ✅ Product CRUD operations
- ✅ Order CRUD operations
- ✅ File management
- ✅ Azure Table Storage integration
- ✅ Azure Blob Storage integration
- ✅ Azure Queue Storage integration
- ✅ Azure File Storage integration
- ✅ Automatic data synchronization
- ✅ Modern responsive UI
- ✅ Dashboard with statistics
- ✅ Azure Functions for service demonstrations

### Future Enhancements
- ⏳ User authentication and authorization
- ⏳ Role-based access control
- ⏳ Payment gateway integration
- ⏳ Email notifications
- ⏳ Advanced reporting and analytics
- ⏳ Real-time updates with SignalR
- ⏳ Mobile application
- ⏳ Product recommendation system
- ⏳ Customer loyalty program
- ⏳ Multi-tenant support

---

**Last Updated**: January 2025  
**Version**: 2.0  
**.NET Version**: 9.0  
**Azure SDK Version**: Latest
