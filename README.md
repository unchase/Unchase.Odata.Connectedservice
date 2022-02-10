<p align="center">
  <a href="https://marketplace.visualstudio.com/items?itemName=Unchase.unchaseodataconnectedservice">
    <img src="img/Unchase-OData-Connected-Service-Logo.png" alt="Unchase OData (Swagger) Connected Service Logo">
  </a>
</p>

[Unchase OData Connected Service](https://marketplace.visualstudio.com/items?itemName=Unchase.unchaseODataConnectedService) is a Visual Studio extension to generate `C#` or `VB` client code for `OData` web service.


> Starting from Visual Studio Community 2019 v16.1.3 extensions based on `Microsoft Connected Services` now work fine.

## Getting Started

#### **[Read How-To on medium.com](https://medium.com/@unchase/how-to-generate-c-or-visual-basic-client-code-for-odata-protocol-versions-1-0-4-0-a3a4f9402ea1)**

Install from `Tools -> Extensions and Updates` menu inside [Visual Studio](https://visualstudio.microsoft.com/vs/) (including 2019) or [download](http://vsixgallery.com/extensions/Unchase.OData.ConnectedService.afc46f39-8c64-4e14-85d0-af6c7c4291f3/extension.vsix)  as `VSIX` package from VSGallery or [download](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseODataConnectedService)  as `VSIX` package from Visual Studio Marketplace: 

![Adding Unchase OData Connected Service in Visual Studio](img/Unchase-OData-Connected-Service.gif)

> The project is developed and maintained by [Nikolay Chebotov (**Unchase**)](https://github.com/unchase).

## Builds status

|Status|Value|
|:----|:---:|
|Build|[![Build status](https://ci.appveyor.com/api/projects/status/kk4auowp28dy7qme)](https://ci.appveyor.com/project/unchase/unchase.odata.connectedservice)
|Build History|![Build history](https://buildstats.info/appveyor/chart/unchase/unchase-odata-connectedservice)
|GitHub Release|[![GitHub release](https://img.shields.io/github/release/unchase/Unchase.Odata.Connectedservice.svg)](https://github.com/unchase/Unchase.Odata.Connectedservice/releases/latest)
|GitHub Release Date|[![GitHub Release Date](https://img.shields.io/github/release-date/unchase/Unchase.Odata.Connectedservice.svg)](https://github.com/unchase/Unchase.Odata.Connectedservice/releases/latest)
|GitHub Release Downloads|[![Github Releases](https://img.shields.io/github/downloads/unchase/Unchase.Odata.Connectedservice/total.svg)](https://github.com/unchase/Unchase.Odata.Connectedservice/releases/latest)
|VS Marketplace|[![VS Marketplace](http://vsmarketplacebadge.apphb.com/version-short/unchase.UnchaseODataConnectedService.svg)](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseODataConnectedService)
|VS Marketplace Downloads|[![VS Marketplace Downloads](http://vsmarketplacebadge.apphb.com/downloads-short/unchase.UnchaseODataConnectedService.svg)](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseODataConnectedService)
|VS Marketplace Installs|[![VS Marketplace Installs](http://vsmarketplacebadge.apphb.com/installs-short/unchase.UnchaseODataConnectedService.svg)](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseODataConnectedService)

## Features

- Generate `C#` and `VB` clients/proxies (client code) from OData specifications for OData protocol versions 1.0-4.0
- Support for emitting dynamic properties on open types for OData protocol version 4.0
- Support selection `OperationImports` (`Actions` and `FunctionImports`) that will be generated for OData protocol versions 1.0-4.0
- Support loading client code generation parameters from json files (including `Connected Service .json` from OData Connected Service by Microsoft)
- Generate `C#` proxy-class extension methods for OData protocol versions 1.0-3.0
- Generate functions to call service methods from OData protocol versions 1.0-3.0 `FunctionImports` or from OData protocol version 4.0 `OperationImports`
  - You can select the necessary methods that will be added after generation for OData protocol versions 1.0-4.0
- Add required dependencies for the `C#` client (before generating):
	- Libraries for `OData v3` Client:
		1. Microsoft.Data.Services.Client ([NuGet](https://www.nuget.org/packages/Microsoft.Data.Services.Client))
		2. Microsoft.Data.OData ([NuGet](https://www.nuget.org/packages/Microsoft.Data.OData))
		3. Microsoft.Data.Edm ([NuGet](https://www.nuget.org/packages/Microsoft.Data.Edm))
		4. System.Spatial ([NuGet](https://www.nuget.org/packages/System.Spatial))
		5. *In additional*: System.ValueTuple ([NuGet](https://www.nuget.org/packages/System.ValueTuple))
	- Libraries for `OData v4` Client:
		1. Microsoft.OData.Client ([NuGet](https://www.nuget.org/packages/Microsoft.OData.Client))
		2. Microsoft.OData.Core ([NuGet](https://www.nuget.org/packages/Microsoft.OData.Core))
		3. Microsoft.OData.Edm ([NuGet](https://www.nuget.org/packages/Microsoft.OData.Edm))
		4. Microsoft.Spatial ([NuGet](https://www.nuget.org/packages/Microsoft.Spatial))
- Storage of the last 10 endpoints (json-specification path)

## Features are supported since v1.0.0

### Generation `VB` client code for OData protocol versions 1.0-4.0

![Unchase OData Connected Service VisualBasic](img/Unchase-OData-Connected-Service-VisualBasic.png)

## Features are supported since v1.2.0

### Selection `FunctionImports` that will be generated for OData protocol versions 1.0-4.0

![Unchase OData Connected Service FunctionImports](img/Unchase-OData-Connected-Service-FunctionImports.png)

## Features are supported since v1.3.0

### Loading client code generation parameters from json files (including `Connected Service .json` from OData Connected Service by Microsoft)

![Unchase OData Connected Service Load Parameters](img/Unchase-OData-Connected-Service-LoadJson.png)

## HowTos

- [ ] Add HowTos in a future
- [ ] ... [request for HowTo you need](https://github.com/unchase/Unchase.OData.Connectedservice/issues/new?title=DOC)

## Troubleshooting

### Installation completes but I can't see the Service in the list of connected services (Visual Studio 2019)

- Relevant [bug report](https://developercommunity.visualstudio.com/content/problem/468751/vs2019-preview-cannot-install-connected-service-ex.html). `Connected Services` restored in the v16.1.3 update to [Visual Studio](https://visualstudio.microsoft.com/vs/) 2019.

## Roadmap

See the [changelog](CHANGELOG.md) for the further development plans and version history.

## Feedback

Please feel free to add your [review](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseODataConnectedService&ssr=false#review-details), [request a feature](https://github.com/unchase/Unchase.OData.Connectedservice/issues/new?title=FEATURE), [ask a question](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseODataConnectedService&ssr=false#qna) or [report a bug](https://github.com/unchase/Unchase.OData.Connectedservice/issues/new?title=BUG) including in connected service:

![Unchase OData Connected Service Report a Bug](img/Unchase-OData-Connected-Service-ReportBug.png)

Thank you in advance!

## Thank me!

If you like what I am doing and you would like to thank me, please consider:

[![Buy me a coffe!](img/buymeacoffe.png)](https://www.buymeacoffee.com/nikolaychebotov)

Thank you for your support!

----------

Copyright &copy; 2019 [Nikolay Chebotov (**Unchase**)](https://github.com/unchase) - Provided under the [Apache License 2.0](LICENSE.md).
