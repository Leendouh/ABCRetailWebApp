# Azure Deployment Guide - ABC Retail Web Application

## Prerequisites
- Active Azure account with available credit
- Azure CLI installed or access to Azure Portal
- Visual Studio or Visual Studio Code
- Git for version control

## Azure Resources Required

### 1. Azure Storage Account
Create a general-purpose storage account with the following configurations:
- **Storage account name**: `abcretailstorage` (must be globally unique)
- **Location**: Choose a region close to your users
- **Account kind**: StorageV2
- **Redundancy**: Locally-redundant storage (LRS) for development
- **Performance**: Standard

### 2. Azure Functions App
Create a Functions App to host the Azure Functions:
- **Function App name**: `abcretail-functions` (must be globally unique)
- **Runtime stack**: .NET
- **Version**: 9.0 (Isolated)
- **Hosting plan**: Consumption
- **Region**: Same as storage account

### 3. Azure App Service
Create a Web App to host the ASP.NET Core application:
- **App Service name**: `abcretail-webapp` (must be globally unique)
- **Runtime stack**: .NET
- **Version**: 9.0
- **Operating system**: Windows
- **Region**: Same as storage account
- **Pricing tier**: Free (F1) for development, Standard (S1) for production

## Deployment Steps

### Step 1: Create Azure Resources via Azure Portal

1. **Create Storage Account**:
   - Go to Azure Portal → Create → Storage Account
   - Configure as per specifications above
   - Copy the connection string for later use

2. **Create Functions App**:
   - Go to Azure Portal → Create → Function App
   - Configure as per specifications above
   - Link to the storage account created in step 1

3. **Create App Service**:
   - Go to Azure Portal → Create → Web App
   - Configure as per specifications above
   - Link to the storage account created in step 1

### Step 2: Configure Application Settings

#### For Web App (App Service):
1. Go to your App Service → Configuration → Application Settings
2. Add the following setting:
   - **Name**: `AzureStorage__ConnectionString`
   - **Value**: Your Azure Storage connection string from Step 1

#### For Functions App:
1. Go to your Functions App → Configuration → Application Settings
2. Add the following settings:
   - **Name**: `AzureStorageConnectionString`
   - **Value**: Your Azure Storage connection string from Step 1
   - **Name**: `AzureWebJobsStorage`
   - **Value**: Your Azure Storage connection string from Step 1

### Step 3: Deploy Web Application

#### Option A: Using Visual Studio
1. Right-click on the `ABCRetailWebApp` project
2. Select "Publish"
3. Choose "Azure" → "Azure App Service"
4. Select your created App Service
5. Click "Publish"

#### Option B: Using Azure CLI
```bash
# Build the application
dotnet publish ABCRetailWebApp/ABCRetailWebApp.csproj -c Release -o ./publish

# Deploy to Azure App Service
az webapp up --name abcretail-webapp --resource-group abcretail-rg --location eastus --sku F1
```

#### Option C: Using GitHub Actions
1. Create a `.github/workflows/deploy.yml` file
2. Configure the workflow to build and deploy to Azure
3. Set up Azure credentials in GitHub secrets

### Step 4: Deploy Azure Functions

#### Option A: Using Visual Studio
1. Right-click on the `ABCRetailFunctions` project
2. Select "Publish"
3. Choose "Azure" → "Azure Function App"
4. Select your created Functions App
5. Click "Publish"

#### Option B: Using Azure CLI
```bash
# Build the functions
dotnet publish ABCRetailFunctions/ABCRetailFunctions.csproj -c Release -o ./publish-functions

# Deploy to Azure Functions
az functionapp create --name abcretail-functions --resource-group abcretail-rg --storage-account abcretailstorage --consumption-plan-location eastus --runtime dotnetIsolated --functions-version 4

# Deploy the functions
az functionapp deployment source config-zip --resource-group abcretail-rg --name abcretail-functions --src ./publish-functions.zip
```

### Step 5: Configure CORS (Cross-Origin Resource Sharing)

1. Go to your Functions App → CORS
2. Add your web app URL to allowed origins
3. Go to your Storage Account → CORS
4. Add your web app URL to allowed origins for Blob, Table, Queue, and File services

### Step 6: Test the Deployed Application

1. Navigate to your web app URL: `https://abcretail-webapp.azurewebsites.net`
2. Test customer creation
3. Test product creation with image upload
4. Test order creation
5. Verify data is stored in Azure Storage tables
6. Verify images are stored in Azure Blob Storage
7. Verify order messages are sent to Azure Queue Storage
8. Test Azure Functions by calling their HTTP endpoints

## Post-Deployment Configuration

### Update Connection Strings
Update the `appsettings.json` in your deployed web app:
```json
{
  "AzureStorage": {
    "ConnectionString": "Your_Azure_Storage_Connection_String"
  }
}
```

### Configure Health Checks
Consider adding health checks to monitor the application status:
```csharp
builder.Services.AddHealthChecks()
    .AddAzureTableStorage(connectionString)
    .AddAzureBlobStorage(connectionString);
```

### Set Up Monitoring
1. Enable Application Insights for both Web App and Functions App
2. Configure logging and alerts
3. Set up dashboards for monitoring

## Troubleshooting

### Common Issues

1. **Connection String Errors**:
   - Verify the connection string is correct
   - Ensure the storage account exists and is accessible
   - Check that firewall rules allow access

2. **CORS Errors**:
   - Verify CORS is configured on both Functions App and Storage Account
   - Ensure the correct URLs are added to allowed origins

3. **Deployment Failures**:
   - Check build logs for specific errors
   - Ensure all dependencies are properly referenced
   - Verify runtime versions match between local and Azure

4. **Performance Issues**:
   - Monitor Azure metrics for bottlenecks
   - Consider scaling up to higher pricing tiers
   - Optimize database queries and storage operations

## Security Considerations

1. **Connection Strings**: Store connection strings in Azure Key Vault instead of app settings
2. **Authentication**: Implement Azure AD authentication for the web application
3. **HTTPS**: Ensure HTTPS is enforced on both web app and functions
4. **Firewall**: Configure IP restrictions on your Azure resources
5. **Managed Identities**: Use Azure Managed Identities for secure resource access

## Cost Optimization

1. **Development**: Use Free tier (F1) for App Service and Consumption plan for Functions
2. **Production**: Monitor usage and scale appropriately
3. **Storage**: Use lifecycle policies to move old data to cheaper storage tiers
4. **Functions**: Configure timeout settings to prevent runaway costs

## Next Steps

1. Test all functionality in the deployed environment
2. Set up CI/CD pipeline for automated deployments
3. Configure backup and disaster recovery
4. Set up monitoring and alerting
5. Document the deployment process for future reference