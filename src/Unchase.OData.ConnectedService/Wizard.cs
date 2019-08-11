// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
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

            ConfigODataEndpointViewModel = new ConfigODataEndpointViewModel(this.UserSettings);
            AdvancedSettingsViewModel = new AdvancedSettingsViewModel(this.UserSettings, this);
            FunctionImportsViewModel = new FunctionImportsViewModel(this.UserSettings);


            if (this.Context.IsUpdating)
            {
                // Since ServiceConfigurationV4 is a derived type of ServiceConfiguration. So we can deserialize a ServiceConfiguration into a ServiceConfigurationV4.
                var serviceConfig = this.Context.GetExtendedDesignerData<ServiceConfigurationV4>();
                ConfigODataEndpointViewModel.Endpoint = serviceConfig.Endpoint;
                ConfigODataEndpointViewModel.EdmxVersion = serviceConfig.EdmxVersion;
                ConfigODataEndpointViewModel.ServiceName = serviceConfig.ServiceName;
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
                AdvancedSettingsViewModel.GeneratedFileName = serviceConfig.GeneratedFileNamePrefix;
                AdvancedSettingsViewModel.UseNamespacePrefix = serviceConfig.UseNameSpacePrefix;
                AdvancedSettingsViewModel.NamespacePrefix = serviceConfig.NamespacePrefix;
                AdvancedSettingsViewModel.UseDataServiceCollection = serviceConfig.UseDataServiceCollection;
                AdvancedSettingsViewModel.FunctionImportsGenerator = serviceConfig.FunctionImportsGenerator;
                AdvancedSettingsViewModel.GenerateFunctionImports = serviceConfig.GenerateFunctionImports;

                if (serviceConfig.EdmxVersion == Common.Constants.EdmxVersion4)
                {
                    AdvancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes =
                        serviceConfig.IgnoreUnexpectedElementsAndAttributes;
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
                            advancedSettingsViewModel.GeneratedFileName = serviceConfig.GeneratedFileNamePrefix;
                            advancedSettingsViewModel.UseNamespacePrefix = serviceConfig.UseNameSpacePrefix;
                            advancedSettingsViewModel.NamespacePrefix = serviceConfig.NamespacePrefix;
                            advancedSettingsViewModel.UseDataServiceCollection = serviceConfig.UseDataServiceCollection;
                            advancedSettingsViewModel.FunctionImportsGenerator = serviceConfig.FunctionImportsGenerator;
                            advancedSettingsViewModel.GenerateFunctionImports = serviceConfig.GenerateFunctionImports;

                            if (serviceConfig.EdmxVersion == Common.Constants.EdmxVersion4)
                            {
                                advancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes =
                                    serviceConfig.IgnoreUnexpectedElementsAndAttributes;
                                advancedSettingsViewModel.EnableNamingAlias = serviceConfig.EnableNamingAlias;
                                advancedSettingsViewModel.IncludeT4File = serviceConfig.IncludeT4File;
                                advancedSettingsViewModel.IncludeT4FileEnabled = true;
                                advancedSettings.IncludeT4File.IsEnabled = true;
                            }

                            if (!advancedSettingsViewModel.GenerateFunctionImports)
                                RemoveFunctionImportsSettingsPage();
                            else
                                AddFunctionImportsSettingsPage();
                        }
                    }
                };

                FunctionImportsViewModel.FunctionImports = serviceConfig.FunctionImports;
            }

            AdvancedSettingsViewModel.PageEntering += (sender, args) =>
            {
                if (sender is AdvancedSettingsViewModel advancedSettingsViewModel)
                {
                    advancedSettingsViewModel.IncludeExtensionsT4File = ConfigODataEndpointViewModel.EdmxVersion != Common.Constants.EdmxVersion4;
                    advancedSettingsViewModel.IncludeExtensionsT4FileVisibility = ConfigODataEndpointViewModel.EdmxVersion != Common.Constants.EdmxVersion4 ? Visibility.Visible : Visibility.Collapsed;
                    advancedSettingsViewModel.FunctionImportsGenerator = AdvancedSettingsViewModel.FunctionImportsGenerator;
                    advancedSettingsViewModel.GenerateFunctionImports = AdvancedSettingsViewModel.GenerateFunctionImports;
                    advancedSettingsViewModel.IncludeT4FileEnabled = true;
                    if (!advancedSettingsViewModel.GenerateFunctionImports)
                        RemoveFunctionImportsSettingsPage();
                    else
                        AddFunctionImportsSettingsPage();
                }
            };

            FunctionImportsViewModel.PageEntering += (sender, args) =>
            {
                if (sender is FunctionImportsViewModel functionImportsViewModel)
                {
                    if (FunctionImportsViewModel.FunctionImports == null || !FunctionImportsViewModel.FunctionImports.Any() || FunctionImportsViewModel.FunctionImports.Any(fi => !ConfigODataEndpointViewModel.Endpoint.Contains(fi.EndpointUri)))
                        FunctionImportsViewModel.FunctionImports = FunctionImportsHelper.GetFunctionImports(
                            this.CreateServiceConfiguration().GetEdmModel(),
                            AdvancedSettingsViewModel.UseNamespacePrefix ? AdvancedSettingsViewModel.NamespacePrefix : null,
                            ConfigODataEndpointViewModel.Endpoint.Replace("$metadata", string.Empty));
                    functionImportsViewModel.FunctionImports = FunctionImportsViewModel.FunctionImports;
                }
            };

            this.Pages.Add(ConfigODataEndpointViewModel);
            this.Pages.Add(AdvancedSettingsViewModel);
            this.IsFinishEnabled = true;
        }
        #endregion

        #region Methods
        public void AddFunctionImportsSettingsPage()
        {
            if (!this.Pages.Contains(FunctionImportsViewModel))
                this.Pages.Add(FunctionImportsViewModel);
        }

        public void RemoveFunctionImportsSettingsPage()
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
                        AdvancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes,
                    EnableNamingAlias = AdvancedSettingsViewModel.EnableNamingAlias,
                    IncludeT4File = AdvancedSettingsViewModel.IncludeT4File
                };
            }
            else
            {
                serviceConfiguration = new ServiceConfigurationV3
                {
                    IncludeExtensionsT4File = AdvancedSettingsViewModel.IncludeExtensionsT4File,
                    FunctionImportsGenerator = AdvancedSettingsViewModel.FunctionImportsGenerator,
                    GenerateFunctionImports = AdvancedSettingsViewModel.GenerateFunctionImports,
                    FunctionImports = FunctionImportsViewModel.FunctionImports
                };
            }

            serviceConfiguration.LanguageOption = ConfigODataEndpointViewModel.UserSettings.LanguageOption;
            serviceConfiguration.ServiceName = ConfigODataEndpointViewModel.UserSettings.ServiceName;
            serviceConfiguration.Endpoint = ConfigODataEndpointViewModel.UserSettings.Endpoint;
            serviceConfiguration.EdmxVersion = ConfigODataEndpointViewModel.EdmxVersion;
            serviceConfiguration.UseDataServiceCollection = AdvancedSettingsViewModel.UseDataServiceCollection;
            serviceConfiguration.GeneratedFileNamePrefix = AdvancedSettingsViewModel.GeneratedFileName;
            serviceConfiguration.UseNameSpacePrefix = AdvancedSettingsViewModel.UseNamespacePrefix;
            serviceConfiguration.OpenGeneratedFilesOnComplete = ConfigODataEndpointViewModel.UserSettings.OpenGeneratedFilesOnComplete;
            if (AdvancedSettingsViewModel.UseNamespacePrefix && !string.IsNullOrEmpty(AdvancedSettingsViewModel.NamespacePrefix))
            {
                serviceConfiguration.NamespacePrefix = AdvancedSettingsViewModel.NamespacePrefix;
            }

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
