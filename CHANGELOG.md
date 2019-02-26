# Unchase.OData.ConnectedService сhangelog:

### Version 0.1.0 - 0.1.32:

*Features:*

- Generate client-side C# proxy-classes for OData protocol versions 1.0-4.0;
- Generate client-side class with proxy-class extension methods for OData protocol versions 1.0-3.0 (with generating functions from FunctionImports);
- Add necessary nuget-packages for client-side OData proxy-classes.

### Version 0.1.33:

*Bug fixes:*

- Fixed bug in tt-template for OData V3 extension class: made public a few internal classes.

### Version 0.1.34:

*Code fixes:*

- Added code-fix in tt-template for OData V3 extension class: simplify method "AddGetQueryOptions".

### Version 0.2:

*Features:*

- Added methods in OData V3 extension class for calling service methods returns void value.

### Version 0.3:

*Features:*

- Added methods to generating functions from FunctionImports in OData V3 extension class.

### Version 0.3.2:

*Minor fixes:*

- Added checkbox for enable/disable methods generation from FunctionImports.

### Version 0.3.6:

*Features:*

- Added network credentials for connecting to the endpoint.

### Version 0.3.7:

*Features:*

- Added web-proxy (with network credentials) for connecting to the endpoint.

### Version 0.3.8:

*Fixes:*

- Fixed setting web-proxy credentials.

### Version 0.3.9:

*Fixes:*

- Fixed errors in  OData V4 generator class: fix casting IEdmTypeDefinition to clrTypeName.

### Version 0.3.10:

*Minor fixes:*

- Fixed nuget package source to nuget online repository.


 