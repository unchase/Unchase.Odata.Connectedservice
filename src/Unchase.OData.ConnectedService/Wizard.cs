// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Services.Design;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Common;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.ViewModels;
using Unchase.OData.ConnectedService.Views;

namespace Unchase.OData.ConnectedService
{
    public class Wizard : ConnectedServiceWizard
    {
        #region Properties and fields
        private Instance _serviceInstance;

        internal ConfigODataEndpointViewModel ConfigODataEndpointViewModel { get; set; }

        internal AdvancedSettingsViewModel AdvancedSettingsViewModel { get; set; }

        internal FunctionImportsViewModel FunctionImportsViewModel { get; set; }

        public ConnectedServiceProviderContext Context { get; set; }

        internal Instance ServiceInstance => this._serviceInstance ?? (this._serviceInstance = new Instance());

        public Version EdmxVersion => this.ConfigODataEndpointViewModel.EdmxVersion;

        internal UserSettings UserSettings { get; }
        #endregion

        #region Constructors
        public Wizard(ConnectedServiceProviderContext context)
        {
            this.Context = context;
            this.UserSettings = UserSettings.Load(context.Logger);

            ConfigODataEndpointViewModel = new ConfigODataEndpointViewModel(this.UserSettings, this);
            AdvancedSettingsViewModel = new AdvancedSettingsViewModel(this.UserSettings, this);
            FunctionImportsViewModel = new FunctionImportsViewModel(this.UserSettings, this);

            if (this.Context.IsUpdating)
            {
                // Since ServiceConfigurationV4 is a derived type of ServiceConfiguration. So we can deserialize a ServiceConfiguration into a ServiceConfigurationV4.
                var serviceConfig = this.Context.GetExtendedDesignerData<ServiceConfigurationV4>();
                ConfigODataEndpointViewModel.Endpoint = serviceConfig.Endpoint;
                ConfigODataEndpointViewModel.EdmxVersion = serviceConfig.EdmxVersion;
                ConfigODataEndpointViewModel.ServiceName = serviceConfig.ServiceName;
                ConfigODataEndpointViewModel.AcceptAllUntrustedCertificates = serviceConfig.AcceptAllUntrustedCertificates;
                ConfigODataEndpointViewModel.UseWebProxy = serviceConfig.UseWebProxy;
                ConfigODataEndpointViewModel.NetworkCredentialsDomain = serviceConfig.NetworkCredentialsDomain;
                ConfigODataEndpointViewModel.NetworkCredentialsUserName = serviceConfig.NetworkCredentialsUserName;
                ConfigODataEndpointViewModel.NetworkCredentialsPassword = serviceConfig.NetworkCredentialsPassword;
                ConfigODataEndpointViewModel.WebProxyNetworkCredentialsDomain = serviceConfig.WebProxyNetworkCredentialsDomain;
                ConfigODataEndpointViewModel.WebProxyNetworkCredentialsUserName = serviceConfig.WebProxyNetworkCredentialsUserName;
                ConfigODataEndpointViewModel.WebProxyNetworkCredentialsPassword = serviceConfig.WebProxyNetworkCredentialsPassword;
                ConfigODataEndpointViewModel.UseNetworkCredentials = serviceConfig.UseNetworkCredentials;
                ConfigODataEndpointViewModel.WebProxyUri = serviceConfig.WebProxyUri;
                if (ConfigODataEndpointViewModel.View is ConfigODataEndpoint configODataEndpoint)
                    configODataEndpoint.IsEnabled = false;

                // The Viewmodel should always be filled otherwise if the wizard is completed without visiting this page  the generated code becomes wrong
                AdvancedSettingsViewModel.GeneratedFileNameEnabled = false; // advancedSettings.ReferenceFileName.IsEnabled = false;
                AdvancedSettingsViewModel.GeneratedFileNamePrefix = serviceConfig.GeneratedFileNamePrefix;
                AdvancedSettingsViewModel.UseNamespacePrefix = serviceConfig.UseNameSpacePrefix;
                AdvancedSettingsViewModel.NamespacePrefix = serviceConfig.NamespacePrefix;
                AdvancedSettingsViewModel.UseDataServiceCollection = serviceConfig.UseDataServiceCollection;
                AdvancedSettingsViewModel.OperationImportsGenerator = serviceConfig.OperationImportsGenerator;
                AdvancedSettingsViewModel.SelectOperationImports = serviceConfig.GenerateOperationImports;
                AdvancedSettingsViewModel.ExcludedOperationImportsNames = serviceConfig.ExcludedOperationImportsNames;

                if (serviceConfig.EdmxVersion == Common.Constants.EdmxVersion4)
                {
                    AdvancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes =
                        serviceConfig.IgnoreUnexpectedElementsAndAttributes;
                    AdvancedSettingsViewModel.GenerateDynamicPropertiesCollection = serviceConfig.GenerateDynamicPropertiesCollection;
                    AdvancedSettingsViewModel.DynamicPropertiesCollectionName = serviceConfig.DynamicPropertiesCollectionName;
                    AdvancedSettingsViewModel.EnableNamingAlias = serviceConfig.EnableNamingAlias;
                    AdvancedSettingsViewModel.IncludeT4File = serviceConfig.IncludeT4File;
                    AdvancedSettingsViewModel.IncludeT4FileEnabled = true;
                }

                // Restore the advanced settings to UI elements.
                AdvancedSettingsViewModel.PageEntering += (sender, args) =>
                {
                    if (sender is AdvancedSettingsViewModel advancedSettingsViewModel)
                    {
                        if (advancedSettingsViewModel.View is AdvancedSettings advancedSettings)
                        {
                            advancedSettings.ReferenceFileName.IsEnabled = false;
                            advancedSettingsViewModel.GeneratedFileNamePrefix = serviceConfig.GeneratedFileNamePrefix;
                            advancedSettingsViewModel.UseNamespacePrefix = serviceConfig.UseNameSpacePrefix;
                            advancedSettingsViewModel.NamespacePrefix = serviceConfig.NamespacePrefix;
                            advancedSettingsViewModel.UseDataServiceCollection = serviceConfig.UseDataServiceCollection;
                            advancedSettingsViewModel.OperationImportsGenerator = serviceConfig.OperationImportsGenerator;
                            advancedSettingsViewModel.SelectOperationImports = serviceConfig.GenerateOperationImports;
                            advancedSettingsViewModel.ExcludedOperationImportsNames = serviceConfig.ExcludedOperationImportsNames;

                            if (serviceConfig.EdmxVersion == Common.Constants.EdmxVersion4)
                            {
                                advancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes =
                                    serviceConfig.IgnoreUnexpectedElementsAndAttributes;
                                advancedSettingsViewModel.GenerateDynamicPropertiesCollection = serviceConfig.GenerateDynamicPropertiesCollection;
                                advancedSettingsViewModel.DynamicPropertiesCollectionName = serviceConfig.DynamicPropertiesCollectionName;
                                advancedSettingsViewModel.EnableNamingAlias = serviceConfig.EnableNamingAlias;
                                advancedSettingsViewModel.IncludeT4File = serviceConfig.IncludeT4File;
                                advancedSettingsViewModel.IncludeT4FileEnabled = true;
                                advancedSettings.IncludeT4File.IsEnabled = true;
                            }

                            if (!advancedSettingsViewModel.SelectOperationImports || UserSettings.LanguageOption != LanguageOption.GenerateCSharpCode)
                                RemoveOperationImportsSettingsPage();
                            else
                                AddOperationImportsSettingsPage();
                        }
                    }
                };

                FunctionImportsViewModel.FunctionImports = serviceConfig.FunctionImports ?? new List<FunctionImportModel>();
                FunctionImportsViewModel.OperationImports = serviceConfig.OperationImports ?? new List<OperationImportModel>();
                FunctionImportsViewModel.FunctionImportsCount = serviceConfig.FunctionImports?.Count ?? 0;
                FunctionImportsViewModel.OperationImportsCount = serviceConfig.OperationImports?.Count ?? 0;
            }

            AdvancedSettingsViewModel.PageEntering += (sender, args) =>
            {
                if (sender is AdvancedSettingsViewModel advancedSettingsViewModel)
                {
                    advancedSettingsViewModel.IncludeExtensionsT4File = AdvancedSettingsViewModel.IncludeExtensionsT4File;
                    advancedSettingsViewModel.IncludeExtensionsT4FileVisibility = ConfigODataEndpointViewModel.EdmxVersion != Common.Constants.EdmxVersion4 ? Visibility.Visible : Visibility.Collapsed;
                    advancedSettingsViewModel.OperationImportsGenerator = AdvancedSettingsViewModel.OperationImportsGenerator;
                    advancedSettingsViewModel.SelectOperationImports = AdvancedSettingsViewModel.SelectOperationImports;
                    advancedSettingsViewModel.IncludeT4FileEnabled = true;
                    if (!advancedSettingsViewModel.SelectOperationImports || UserSettings.LanguageOption != LanguageOption.GenerateCSharpCode)
                        RemoveOperationImportsSettingsPage();
                    else
                        AddOperationImportsSettingsPage();
                }
            };

            FunctionImportsViewModel.PageEntering += (sender, args) =>
            {
                if (sender is FunctionImportsViewModel functionImportsViewModel)
                {
                    if (ConfigODataEndpointViewModel.EdmxVersion == Common.Constants.EdmxVersion4 && (FunctionImportsViewModel.OperationImports == null ||
                        !FunctionImportsViewModel.OperationImports.Any() ||
                        (AdvancedSettingsViewModel.UseNamespacePrefix && FunctionImportsViewModel.OperationImports.Any(fi => fi.Namespace != AdvancedSettingsViewModel.NamespacePrefix)) ||
                        FunctionImportsViewModel.OperationImports.Any(fi =>
                            !this.UserSettings.Endpoint.Contains(fi.EndpointUri))))
                    {
                        FunctionImportsViewModel.OperationImports = OperationImportsHelper.GetOperationsImports(
                            (this.CreateServiceConfiguration() as ServiceConfigurationV4).GetODataEdmModel(),
                            AdvancedSettingsViewModel.UseNamespacePrefix
                                ? AdvancedSettingsViewModel.NamespacePrefix
                                : null,
                            File.Exists(ConfigODataEndpointViewModel.Endpoint) ? ConfigODataEndpointViewModel.Endpoint : ConfigODataEndpointViewModel.Endpoint.Replace("$metadata", string.Empty));
                        FunctionImportsViewModel.OperationImportsCount = FunctionImportsViewModel.OperationImports?.Count ?? 0;
                    }

                    if (ConfigODataEndpointViewModel.EdmxVersion != Common.Constants.EdmxVersion4 && (FunctionImportsViewModel.FunctionImports == null ||
                        !FunctionImportsViewModel.FunctionImports.Any() ||
                        (AdvancedSettingsViewModel.UseNamespacePrefix && FunctionImportsViewModel.FunctionImports.Any(fi => fi.Namespace != AdvancedSettingsViewModel.NamespacePrefix)) ||
                        FunctionImportsViewModel.FunctionImports.Any(fi =>
                            !this.UserSettings.Endpoint.Contains(fi.EndpointUri))))
                    {
                        FunctionImportsViewModel.FunctionImports = FunctionImportsHelper.GetFunctionImports(
                            this.CreateServiceConfiguration().GetDataEdmModel(),
                            AdvancedSettingsViewModel.UseNamespacePrefix
                                ? AdvancedSettingsViewModel.NamespacePrefix
                                : null,
                            File.Exists(ConfigODataEndpointViewModel.Endpoint) ? ConfigODataEndpointViewModel.Endpoint : ConfigODataEndpointViewModel.Endpoint.Replace("$metadata", string.Empty));
                        FunctionImportsViewModel.FunctionImportsCount = FunctionImportsViewModel.FunctionImports?.Count ?? 0;
                    }

                    functionImportsViewModel.FunctionImports = FunctionImportsViewModel.FunctionImports;
                    functionImportsViewModel.FunctionImportsCount = functionImportsViewModel.FunctionImports?.Count ?? 0;
                    functionImportsViewModel.OperationImports = FunctionImportsViewModel.OperationImports;
                    functionImportsViewModel.OperationImportsCount = functionImportsViewModel.OperationImports?.Count ?? 0;
                }
            };

            this.Pages.Add(ConfigODataEndpointViewModel);
            this.Pages.Add(AdvancedSettingsViewModel);
            this.IsFinishEnabled = true;
        }
        #endregion

        #region Methods
        public void AddOperationImportsSettingsPage()
        {
            if (!this.Pages.Contains(FunctionImportsViewModel))
                this.Pages.Add(FunctionImportsViewModel);
        }

        public void RemoveOperationImportsSettingsPage()
        {
            if (this.Pages.Contains(FunctionImportsViewModel))
                this.Pages.Remove(FunctionImportsViewModel);
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            this.UserSettings.Save();

            this.ServiceInstance.Name = ConfigODataEndpointViewModel.UserSettings.ServiceName;
            this.ServiceInstance.MetadataTempFilePath = ConfigODataEndpointViewModel.MetadataTempPath;
            this.ServiceInstance.ServiceConfig = this.CreateServiceConfiguration();
            return Task.FromResult<ConnectedServiceInstance>(this.ServiceInstance);
        }

        /// <summary>
        /// Create the service configuration according to the edmx version.
        /// </summary>
        /// <returns>If the edm version is less than 4.0, returns a ServiceConfiguration, else, returns ServiceConfigurationV4</returns>
        private ServiceConfiguration CreateServiceConfiguration()
        {
            ServiceConfiguration serviceConfiguration;
            if (ConfigODataEndpointViewModel.EdmxVersion == Common.Constants.EdmxVersion4)
            {
                serviceConfiguration = new ServiceConfigurationV4
                {
                    IgnoreUnexpectedElementsAndAttributes =
                        AdvancedSettingsViewModel.UserSettings.IgnoreUnexpectedElementsAndAttributes,
                    GenerateDynamicPropertiesCollection = AdvancedSettingsViewModel.UserSettings.GenerateDynamicPropertiesCollection,
                    DynamicPropertiesCollectionName = AdvancedSettingsViewModel.UserSettings.DynamicPropertiesCollectionName,
                    EnableNamingAlias = AdvancedSettingsViewModel.UserSettings.EnableNamingAlias,
                    IncludeT4File = AdvancedSettingsViewModel.UserSettings.IncludeT4File
                };
            }
            else
            {
                serviceConfiguration = new ServiceConfigurationV3
                {
                    IncludeExtensionsT4File = AdvancedSettingsViewModel.UserSettings.IncludeExtensionsT4File,
                    OperationImportsGenerator = AdvancedSettingsViewModel.UserSettings.OperationImportsGenerator,
                    GenerateOperationImports = AdvancedSettingsViewModel.UserSettings.SelectOperationImports,
                    FunctionImports = FunctionImportsViewModel.FunctionImports
                };
            }

            serviceConfiguration.LanguageOption = ConfigODataEndpointViewModel.UserSettings.LanguageOption;
            serviceConfiguration.ServiceName = ConfigODataEndpointViewModel.UserSettings.ServiceName;
            serviceConfiguration.AcceptAllUntrustedCertificates = ConfigODataEndpointViewModel.UserSettings.AcceptAllUntrustedCertificates;
            serviceConfiguration.Endpoint = ConfigODataEndpointViewModel.UserSettings.Endpoint;
            serviceConfiguration.EdmxVersion = ConfigODataEndpointViewModel.EdmxVersion;
            serviceConfiguration.UseDataServiceCollection = AdvancedSettingsViewModel.UserSettings.UseDataServiceCollection;
            serviceConfiguration.GeneratedFileNamePrefix = AdvancedSettingsViewModel.UserSettings.GeneratedFileNamePrefix;
            serviceConfiguration.UseNameSpacePrefix = AdvancedSettingsViewModel.UserSettings.UseNameSpacePrefix;
            serviceConfiguration.OpenGeneratedFilesOnComplete = ConfigODataEndpointViewModel.UserSettings.OpenGeneratedFilesOnComplete;
            if (AdvancedSettingsViewModel.UserSettings.UseNameSpacePrefix && !string.IsNullOrEmpty(AdvancedSettingsViewModel.UserSettings.NamespacePrefix))
                serviceConfiguration.NamespacePrefix = AdvancedSettingsViewModel.UserSettings.NamespacePrefix;

            serviceConfiguration.ExcludedOperationImportsNames = AdvancedSettingsViewModel.UserSettings.ExcludedOperationImportsNames;
            serviceConfiguration.OperationImports = FunctionImportsViewModel.OperationImports;

            #region Network Credentials
            serviceConfiguration.UseNetworkCredentials =
                ConfigODataEndpointViewModel.UseNetworkCredentials;
            serviceConfiguration.NetworkCredentialsUserName =
                ConfigODataEndpointViewModel.NetworkCredentialsUserName;
            serviceConfiguration.NetworkCredentialsPassword =
                ConfigODataEndpointViewModel.NetworkCredentialsPassword;
            serviceConfiguration.NetworkCredentialsDomain =
                ConfigODataEndpointViewModel.NetworkCredentialsDomain;
            #endregion

            #region Web-proxy
            serviceConfiguration.UseWebProxy =
                ConfigODataEndpointViewModel.UseWebProxy;
            serviceConfiguration.UseWebProxyCredentials =
                ConfigODataEndpointViewModel.UseWebProxyCredentials;
            serviceConfiguration.WebProxyNetworkCredentialsUserName =
                ConfigODataEndpointViewModel.WebProxyNetworkCredentialsUserName;
            serviceConfiguration.WebProxyNetworkCredentialsPassword =
                ConfigODataEndpointViewModel.WebProxyNetworkCredentialsPassword;
            serviceConfiguration.WebProxyNetworkCredentialsDomain =
                ConfigODataEndpointViewModel.WebProxyNetworkCredentialsDomain;
            serviceConfiguration.WebProxyUri =
                ConfigODataEndpointViewModel.WebProxyUri;
            #endregion

            return serviceConfiguration;
        }

        /// <summary>
        /// Cleanup object references
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (this.AdvancedSettingsViewModel != null)
                    {
                        this.AdvancedSettingsViewModel.Dispose();
                        this.AdvancedSettingsViewModel = null;
                    }

                    if (this.ConfigODataEndpointViewModel != null)
                    {
                        this.ConfigODataEndpointViewModel.Dispose();
                        this.ConfigODataEndpointViewModel = null;
                    }

                    if (this.FunctionImportsViewModel != null)
                    {
                        this.FunctionImportsViewModel.Dispose();
                        this.FunctionImportsViewModel = null;
                    }

                    if (this._serviceInstance != null)
                    {
                        this._serviceInstance.Dispose();
                        this._serviceInstance = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
