// Copyright (c) Microsoft Corporation.  All rights reserved.
// Updated by Unchase (https://github.com/unchase).
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Unchase.OData.ConnectedService.Common
{
    internal static class Constants
    {
        public const string Author = "Nikolay Chebotov (Unchase)";
        public const string ExtensionCategory = "OData";
        public const string ExtensionName = "Unchase OData Connected Service";
        public const string ExtensionDescription = "OData V1-V4 connected service with extension methods for client-side proxy class.";
        public const string ProviderId = "Unchase.OData.ConnectedService";
        public const string Website = "https://github.com/unchase/Unchase.OData.Connectedservice/";
        public const string Copyright = "Copyright © 2019";

        public const string NuGetOnlineRepository = "https://www.nuget.org/api/v2/";
        public const string DefaultReferenceFileName = "Reference";
        public const string DefaultNamespacePrefix = "Reference";
        public const string DefaultServiceName = "OData Service";
        public const string DefaultDynamicPropertiesCollectionName = "DynamicProperties";

        public static Version EdmxVersion1 = new Version(1, 0, 0, 0);
        public static Version EdmxVersion2 = new Version(2, 0, 0, 0);
        public static Version EdmxVersion3 = new Version(3, 0, 0, 0);
        public static Version EdmxVersion4 = new Version(4, 0, 0, 0);

        public const string V3ClientNuGetPackage = "Microsoft.Data.Services.Client";
        public const string V3ODataNuGetPackage = "Microsoft.Data.OData";
        public const string V3EdmNuGetPackage = "Microsoft.Data.Edm";
        public const string V3SpatialNuGetPackage = "System.Spatial";

        public const string ValueTupleNuGetPackage = "System.ValueTuple";
        public const string V3ExtensionsT4FileName = "ODataV3ExtensionsT4CodeGenerator.tt";
        public const string SimpleODataClientNuGetPackage = "Simple.OData.Client";
        public const string SystemComponentModelAnnotationsNuGetPackage = "System.ComponentModel.Annotations";

        public const string V4ClientNuGetPackage = "Microsoft.OData.Client";
        public const string V4ODataNuGetPackage = "Microsoft.OData.Core";
        public const string V4EdmNuGetPackage = "Microsoft.OData.Edm";
        public const string V4SpatialNuGetPackage = "Microsoft.Spatial";

        public const string MSEmbeddedResourcePackage = "Microsoft.Extensions.FileProviders.Embedded";

        public const string EdmxVersion1Namespace = "http://schemas.microsoft.com/ado/2007/06/edmx";
        public const string EdmxVersion2Namespace = "http://schemas.microsoft.com/ado/2008/10/edmx";
        public const string EdmxVersion3Namespace = "http://schemas.microsoft.com/ado/2009/11/edmx";
        public const string EdmxVersion4Namespace = "http://docs.oasis-open.org/odata/ns/edmx";

        public const string V3DocUri = "https://msdn.microsoft.com/en-us/library/cc668772(v=vs.110).aspx";
        public const string V4DocUri = "http://odata.github.io/odata.net/";

        public static string[] V3NuGetPackages = new string[]
        {
            V3ClientNuGetPackage,
            V3ODataNuGetPackage,
            V3EdmNuGetPackage,
            V3SpatialNuGetPackage,

        };

        public static string[] V4NuGetPackages = new string[]
        {
            V4ClientNuGetPackage,
            V4ODataNuGetPackage,
            V4EdmNuGetPackage,
            V4SpatialNuGetPackage
        };

        public static Dictionary<string, Version> SupportedEdmxNamespaces { get; } = new Dictionary<string, Version>
        {
            { EdmxVersion1Namespace, EdmxVersion1},
            { EdmxVersion2Namespace, EdmxVersion2},
            { EdmxVersion3Namespace, EdmxVersion3},
            { EdmxVersion4Namespace, EdmxVersion4}
        };

        public enum OperationImportsGenerator
        {
            Inner = 0,
            SimpleOData = 1,
            Vipr = 2
        }
    }
}
