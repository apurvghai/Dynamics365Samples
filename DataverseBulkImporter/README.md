---
ArtifactType: azfunc
Language: csharp, powershell, markdown
Platform: windows
Tags: dataverse performance, dataverse hyperscale, dataverse data import sample
---

# Dataverse - Bulk Data Import Sample

This sample allows you to import data to a Dataverse instance using Elastic and SQL (Standard) tables. The sample is designed to run on the Azure Function Premium plan with prewarmed instances (around 20) to achieve maximum throughput. It includes a PowerShell script to simulate an Azure function following Azure Function throttle limits for invoking the web request.

To learn more about [Elastic Tables](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/create-edit-elastic-tables)


## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 

### Prerequisites

- Powershell 7
- Net 6.0
- Visual Studio 2022


### Setup

- The organization instance is referenced via an environment variable (DataverseInstance).
- Create an application using App Registration.
- Create an application user (using PPAC) - an S2S user with a security role that can access the entity you want to import.
- Generate a client secret from Step #2 and store it in the Key Vault.
- When the app runs, the secret is automatically fetched from the Key Vault.

## Authors

- Apurv Ghai - Sr. Embedded Escalation Engineer (Microsoft Customer Support & Service)
- Nikhil Aggarwal - Group Engineering Manager (Microsoft Dataverse)


