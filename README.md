# StsGateway
StsGateway is a OAuth2 STS(Security Token Service) client


| Package |  Version | Downloads |
| ------- | ----- | ----- |
| `STS Gateway` | [![NuGet](https://img.shields.io/nuget/v/StsGateway.svg)](https://nuget.org/packages/StsGateway) | [![Nuget](https://img.shields.io/nuget/dt/StsGateway.svg)](https://nuget.org/packages/StsGateway) |


### Dependencies
.NET Standand 2.1

You can check supported frameworks here:

https://learn.microsoft.com/pt-br/dotnet/api/?view=net-6.0

### Instalation
This package is available through Nuget Packages: https://www.nuget.org/packages/StsGateway


**Nuget**
```
Install-Package StsGateway
```

**.NET CLI**
```
dotnet add package StsGateway
```

## How to use
```csharp

string jsonString = "{ \"name\":\"product a\" }";
STS Gateway.PublishMessage(jsonString);

```
