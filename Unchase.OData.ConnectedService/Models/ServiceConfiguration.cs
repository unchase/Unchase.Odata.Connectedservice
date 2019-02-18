﻿// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using Unchase.OData.ConnectedService.Common;

namespace Unchase.OData.ConnectedService.Models
{
    internal class ServiceConfiguration
    {
        public string ServiceName { get; set; }
        public string Endpoint { get; set; }
        public Version EdmxVersion { get; set; }
        public string GeneratedFileNamePrefix { get; set; }
        public bool UseNameSpacePrefix { get; set; }
        public string NamespacePrefix { get; set; }
        public bool UseDataServiceCollection { get; set; }
    }

    internal class ServiceConfigurationV3 : ServiceConfiguration
    {
        public Constants.FunctionImportsGenerator FunctionImportsGenerator { get; set; }
        public bool IncludeExtensionsT4File { get; set; }
        public bool GenerateFunctionImports { get; set; }
    }

    internal class ServiceConfigurationV4 : ServiceConfigurationV3
    {
        public bool EnableNamingAlias { get; set; }
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }
        public bool IncludeT4File { get; set; }
    }
}
