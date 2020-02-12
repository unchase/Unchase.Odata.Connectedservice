// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Services.Design;
using Unchase.OData.ConnectedService.Common;

namespace Unchase.OData.ConnectedService.Models
{
    internal class ServiceConfiguration
    {
        #region Properties and fields
        public string ServiceName { get; set; }

        public bool AcceptAllUntrustedCertificates { get; set; }

        public string Endpoint { get; set; }

        public Version EdmxVersion { get; set; }

        public string GeneratedFileNamePrefix { get; set; }

        public bool UseNameSpacePrefix { get; set; }

        public string NamespacePrefix { get; set; }

        public bool UseDataServiceCollection { get; set; }

        /// <summary>
        /// Change the INotifyPropertyChanged Implementation to support async operations with synchronous event callbacks
        /// </summary>
        /// <remarks>This should only be set to true if the <see cref="UseDataServiceCollection"/> is also true.</remarks>
        public bool UseAsyncDataServiceCollection { get; set; }

        public bool OpenGeneratedFilesOnComplete { get; set; }

        public LanguageOption LanguageOption { get; set; }

        public List<FunctionImportModel> FunctionImports { get; set; }

        public List<OperationImportModel> OperationImports { get; set; }

        public string ExcludedOperationImportsNames { get; set; }

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

        #endregion
    }

    #region Child classes
    internal class ServiceConfigurationV3 : ServiceConfiguration
    {
        public Constants.OperationImportsGenerator OperationImportsGenerator { get; set; }

        public bool IncludeExtensionsT4File { get; set; }

        public bool GenerateOperationImports { get; set; }
    }

    internal class ServiceConfigurationV4 : ServiceConfigurationV3
    {
        public bool EnableNamingAlias { get; set; }

        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }

        public bool GenerateDynamicPropertiesCollection { get; set; }

        public string DynamicPropertiesCollectionName { get; set; }

        public bool IncludeT4File { get; set; }
    }
    #endregion
}