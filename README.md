# HRM.CareerSite

Starter .NET web app for building a branded external careers site on top of FourVision Advanced HRM.

Repository: https://github.com/FourVision/HRM.CareerSite

Reference: [CareerSite SDK template (FourVision Wiki)](https://portal.fourvision.com/Wiki?Page=%2fRecruitment%2fCareerSite-SDK.md)

## Overview

This project provides a configurable public careers website for displaying vacancies and capturing job applications from a FourVision Advanced HRM environment. It is designed to be branded and hosted for a company-specific careers page while using the upstream HRM API as the source of truth for jobs and application data.

The CareerSite SDK template gives organizations a fast way to build an external-facing recruitment experience without duplicating the recruitment process in another system. The public site keeps vacancy and application management in A-HRM while the website itself remains a branded presentation layer for candidates.

## What the template does

The CareerSite SDK template is a .NET web application that lets you:

- Show published vacancies on a public site.
- Open a vacancy-specific application form.
- Submit candidate applications back to the configured A-HRM environment.
- Use either API key or OAuth authentication for the connection to A-HRM.
- Keep recruitment setup and process logic in A-HRM while presenting a separate public careers experience.

## When to use it

Use this template when you want:

- A public careers site with your own branding.
- Vacancy pages outside the built-in Opportunity Hub.
- A custom portal or website that still uses A-HRM as the system of record for vacancies and applications.
- A consistent recruitment process across internal and external candidates.

## Site entry points

| Route | Purpose |
|---|---|
| `/` | Public vacancy listing and search experience. |
| `/apply/{JobId}` | Candidate application form for a selected vacancy. |

## How the public application flow works

A typical external candidate journey looks like this:

1. A vacancy is configured for external publication.
2. The vacancy appears on the public CareerSite.
3. A candidate opens the vacancy and selects Apply.
4. The candidate completes the required personal and application details.
5. The CareerSite sends the application to A-HRM.
6. A-HRM creates the corresponding applicant and application records.
7. The recruiter continues the recruitment workflow in A-HRM.

This design keeps the public-facing site separate from the core A-HRM experience while still using the same recruitment records, vacancy data, and application flow.

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

## Recommended implementation setup

Before going live, confirm the following:

- The relevant vacancies are published for external access.
- Recruitment default settings, number sequences, and application stages are configured.
- Candidate source mapping is set up for externally submitted applications.
- Any required CV or supporting document settings are configured.
- The integration identity used by the CareerSite has the necessary access to create applicant and application records.

## Security considerations

The public site does not require the candidate to have an A-HRM user account. The site itself should use a dedicated service or integration identity with the minimum permissions required to create the applicant and application records in A-HRM.

Keep the public website identity separate from the recruiter identity and do not grant public users interactive A-HRM roles.

## Notes

- The app uses the upstream HRM API to load vacancies and submit external applications.
- The API key flow sends the configured header name and key to the HRM server.
- The OAuth flow uses the client credentials grant against Microsoft Entra ID.
- The site is intended as a branded customer-facing template for Advanced HRM-backed recruiting scenarios.

## Support and customization

This repository is a starter project for building a careers site. Custom branding, vacancy styling, and workflow changes should be made in line with your customer requirements and the applicable licensing terms for Advanced HRM.

## Legal notice

HRM.CareerSite is copyright © FourVision and remains the property of FourVision. This template site is available for licensed customers of Advanced HRM and is intended for use only within the scope of valid licensing and customer agreements. Any reuse, redistribution, or deployment outside the terms of a valid Advanced HRM license requires explicit written permission from FourVision.

## Related information

- [CareerSite SDK template (FourVision Wiki)](https://portal.fourvision.com/Wiki?Page=%2fRecruitment%2fCareerSite-SDK.md)

## Summary

The CareerSite SDK template gives organizations a fast way to build a public careers experience while keeping vacancy and application management in A-HRM. It is a practical option when you want custom branding and an external-facing site without duplicating the recruitment process in another system.