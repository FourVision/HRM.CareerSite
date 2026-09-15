# HRM.CareerSite

Starter .NET web app for building a branded external careers site on top of FourVision Advanced HRM.

Repository: https://github.com/FourVision/HRM.CareerSite

## Overview

This project provides a configurable external careers website for displaying vacancies and capturing job applications from a FourVision Advanced HRM environment. It is designed to be branded and hosted for a company-specific careers page while using the upstream HRM API as the source of truth for jobs and application data.

## Legal notice

HRM.CareerSite is copyright © FourVision and remains the property of FourVision. This template site is available for licensed customers of Advanced HRM and is intended for use only within the scope of valid licensing and customer agreements. Any reuse, redistribution, or deployment outside the terms of a valid Advanced HRM license requires explicit written permission from FourVision.

## Requirements

- .NET 10 SDK
- Access to an Advanced HRM environment
- Either an API key or OAuth client credentials for the upstream HRM server

## Configuration

The app reads configuration from the `HrmServer` section. Configure these values as environment variables, App Service settings, or local user secrets:

- `HrmServer__Host`
- `HrmServer__Authentication__Mode` (`None`, `ApiKey`, or `OAuth`)
- `HrmServer__Authentication__ApiKeyHeaderName`
- `HrmServer__Authentication__ApiKey`
- `HrmServer__Authentication__OAuth__TenantId`
- `HrmServer__Authentication__OAuth__ClientId`
- `HrmServer__Authentication__OAuth__ClientSecret`

A typical local setup with PowerShell looks like this:

```powershell
$env:HrmServer__Host = "https://your-hrm-environment.example"
$env:HrmServer__Authentication__Mode = "ApiKey"
$env:HrmServer__Authentication__ApiKeyHeaderName = "x-api-key"
$env:HrmServer__Authentication__ApiKey = "<your-api-key>"
```

If `HrmServer__Authentication__Mode` is set to `None`, the site opens in setup mode and lets you enter the connection details in the UI. For production deployments, store the final values as environment variables so they survive restarts and do not need to be re-entered.

## Local setup

1. Clone the repository.
2. Set `HrmServer__Host` to your Advanced HRM environment URL.
3. Choose `ApiKey` or `OAuth` and provide the matching credentials.
4. Run the app:

```powershell
dotnet run --project CareerSite\Hrm.CareerSite.csproj
```

The default route loads the vacancy listing and search experience. Application forms are available from the job detail route.

## Deployment

Publish the site with the .NET SDK:

```powershell
dotnet publish CareerSite\Hrm.CareerSite.csproj -c Release -o .\publish
```

Deploy the `publish` folder to Azure App Service or another ASP.NET-compatible host.

The repository also includes deployment pipelines for Azure:

- `.github/workflows/azure-web-app-careersite.yml`
- `azure-web-app-pipelines.yml`
- `azure-web-app-careersite.bicep`

## Site routes

- `/` - vacancy listing and search
- `/apply/{JobId}` - application form for a selected vacancy

## Notes

- The app uses the upstream HRM API to load vacancies and submit external applications.
- The API key flow sends the configured header name and key to the HRM server.
- The OAuth flow uses the client credentials grant against Microsoft Entra ID.
- The site is intended as a branded customer-facing template for Advanced HRM-backed recruiting scenarios.

## Support and customization

This repository is a starter project for building a careers site. Custom branding, vacancy styling, and workflow changes should be made in line with your customer requirements and the applicable licensing terms for Advanced HRM.