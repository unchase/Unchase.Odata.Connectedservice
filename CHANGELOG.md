# Road map

- [ ] Fix open [issues](https://github.com/unchase/Unchase.OData.Connectedservice/issues/)
- [ ] Gather [feedback](https://github.com/unchase/Unchase.OData.Connectedservice/issues/new) on [`Unchase OData Connected Service`](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseODataConnectedService) extension for [`Visual Studio`](https://visualstudio.microsoft.com/vs/) and plan further features to implement

# Change log

These are the changes to each version that has been released on the official [Visual Studio extension gallery](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseODataConnectedService).

## v0.4.3 `(2019-04-14)`

- [x] Changed [`LICENSE`](LICENSE.md): [MIT License](https://mit-license.org) to [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0).

## v0.4.0 `(2019-04-13)`

- [x] The code has been refactored!
- [x] Added: in the first page of wizard added checkbox for open generated files on complete
- [x] Added ability to hide/show settings in the first page of service settings wizard
- [x] Storage last OData metadata *Endpoint* and *Service name* 

## v0.3.14 `(2019-04-12)`

- [x] Added bug fix for [issue #3](https://github.com/unchase/Unchase.Odata.Connectedservice/issues/3)

## v0.3.12 `(2019-02-26)`

- [x] Fixed error in method `GetModelRegion`

## v0.3.11 `(2019-02-26)`

- [x] Fixed error in method `GetFunctionMethodRegionWithSimpleOdataClient`

## v0.3.10 `(2019-02-26)`

- [x] Fixed nuget package source to nuget online repository

## v0.3.9 `(2019-02-22)`

- [x] Fixed errors in  OData V4 generator class: fix casting `IEdmTypeDefinition` to `clrTypeName`

## v0.3.8 `(2019-02-20)`

- [x] Fixed setting web-proxy credentials

## v0.3.7 `(2019-02-20)`

- [x] Added web-proxy (with network credentials) for connecting to the endpoint

## v0.3.6 `(2019-02-20)`

- [x] Added network credentials for connecting to the endpoint

## v0.3.2 `(2019-02-18)`

- [x] Added checkbox for enable/disable methods generation from `FunctionImports`

## v0.3 `(2019-02-17)`

- [x] Added methods to generating functions from `FunctionImports` in OData V3 extension class

## v0.2 `(2019-01-18 - 2019-01-27)`

- [x] Added methods in OData V3 extension class for calling service methods returns void value

## v0.1.34 `(2019-01-11)`

- [x] Added code-fix in tt-template for OData V3 extension class: simplify method `AddGetQueryOptions`

## v0.1.33 `(2019-01-10)`

- [x] Fixed bug: made public a few internal classes

## v0.1.0 - 0.1.32 `(2018-12-30 - 2018-12-31)`

- [x] Add necessary nuget-packages for client-side OData proxy-classes
- [x] Generate client-side class with proxy-class extension methods for OData protocol versions 1.0-3.0 (with generating functions from `FunctionImports`)
- [x] Generate client-side C# proxy-classes for OData protocol versions 1.0-4.0