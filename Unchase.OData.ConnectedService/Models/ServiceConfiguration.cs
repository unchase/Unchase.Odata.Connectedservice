// Copyright (c) Microsoft Corporation.  All rights reserved.
// Updated by Unchase (https://github.com/unchase).
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

        #region Network Credentials
        public bool UseNetworkCredentials { get; set; }
        public string NetworkCredentialsUserName { get; set; }
        public string NetworkCredentialsPassword { get; set; }
        public string NetworkCredentialsDomain { get; set; }
        #endregion

        #region WebProxy
        public string WebProxyUri { get; set; }
        public bool UseWebProxy { get; set; }
        public bool UseWebProxyCredentials { get; set; }
        public string WebProxyNetworkCredentialsUserName { get; set; }
        public string WebProxyNetworkCredentialsPassword { get; set; }
        public string WebProxyNetworkCredentialsDomain { get; set; }
        #endregion
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
