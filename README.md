# Micrososft Azure For .Net Developers

Microsoft Azure is the premiere cloud platform from Microsoft. It is an excellent space for hosting .NET applications and the modern .NET developer must be comfortable navigating the different services and features and using the cloud hosting platform to produce top-notch enterprise applications. In this project, we will get familiar with Microsoft Azure, it's interface, and various services. We will provision and then use Microsoft Azure resources and services and have an appreciation for how everything connects and can contribute to your stable and modern application being developed.

- Azure CLI and Azure PowerShell
- Virtual Machines on Azure
- Azure App Services
- Monitoring web applications for performance and potential errors using Application Insights
- Scale applications and databases based on load
- Setup Deployment Slots
- Application secrets in .NET Applications
- Azure SQL and understand the different hosting models
- Azure Blob Storage
- Azure Cosmos DB
- Azure Service Bus and Queues
- Build and Deploy Azure Functions
- Integrate Advanced .NET Application Security with Azure AD and Azure AD B2C



## Development

### Azure App Service

- create resources
```
az group create --name alexeirg --location francecentral
az webapp up -g alexeirg -p alexeiplan -n AlexeiAzureWebApp --verbose
```

- ASP.NET Core project set up
```
dotnet new webapp -o MyAzureWebApp
```

- resources will be created on the azure portal
<img src="/pictures/webapp.png" title="webapp"  width="900">

- publish the app
<img src="/pictures/webapp1.png" title="webapp"  width="900">

### CI/CD

- Deployment Center
<img src="/pictures/webapp2.png" title="webapp"  width="900">

- bring modifications to the app and push
<img src="/pictures/webapp3.png" title="webapp"  width="900">

### Azure SQL