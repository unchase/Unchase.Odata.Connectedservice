// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.ViewModels;
using Unchase.OData.ConnectedService.Views;

namespace Unchase.OData.ConnectedService
{
    internal class Wizard : ConnectedServiceWizard
    {
        private readonly UserSettings _userSettings;

        private Instance _serviceInstance;

        public ConfigODataEndpointViewModel ConfigODataEndpointViewModel { get; set; }

        public AdvancedSettingsViewModel AdvancedSettingsViewModel { get; set; }

        public ConnectedServiceProviderContext Context { get; set; }

        public Instance ServiceInstance => this._serviceInstance ?? (this._serviceInstance = new Instance());

        public Version EdmxVersion => this.ConfigODataEndpointViewModel.EdmxVersion;

        public UserSettings UserSettings => this._userSettings;

        public Wizard(ConnectedServiceProviderContext context)
        {
            this.Context = context;
            this._userSettings = UserSettings.Load(context.Logger);

            ConfigODataEndpointViewModel = new ConfigODataEndpointViewModel(this._userSettings);
            AdvancedSettingsViewModel = new AdvancedSettingsViewModel();

            if (this.Context.IsUpdating)
            {
                //Since ServiceConfigurationV4 is a derived type of ServiceConfiguration. So we can deserialize a ServiceConfiguration into a ServiceConfigurationV4.
                var serviceConfig = this.Context.GetExtendedDesignerData<ServiceConfigurationV4>();
                ConfigODataEndpointViewModel.Endpoint = serviceConfig.Endpoint;
                ConfigODataEndpointViewModel.EdmxVersion = serviceConfig.EdmxVersion;
                ConfigODataEndpointViewModel.ServiceName = serviceConfig.ServiceName;
                if (ConfigODataEndpointViewModel.View is ConfigODataEndpoint configODataEndpoint)
                    configODataEndpoint.IsEnabled = false;

                //Restore the advanced settings to UI elements.
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

                            if (serviceConfig.EdmxVersion == Common.Constants.EdmxVersion4)
                            {
                                advancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes =
                                    serviceConfig.IgnoreUnexpectedElementsAndAttributes;
                                advancedSettingsViewModel.EnableNamingAlias = serviceConfig.EnableNamingAlias;
                                advancedSettingsViewModel.IncludeT4File = serviceConfig.IncludeT4File;
                                advancedSettings.IncludeT4File.IsEnabled = false;
                            }
                        }
                    }
                };
            }

            AdvancedSettingsViewModel.PageEntering += (sender, args) =>
            {
                if (sender is AdvancedSettingsViewModel advancedSettingsViewModel)
                {
                    advancedSettingsViewModel.IncludeExtensionsT4File = ConfigODataEndpointViewModel.EdmxVersion != Common.Constants.EdmxVersion4;
                    
                    advancedSettingsViewModel.IncludeExtensionsT4FileVisibility = ConfigODataEndpointViewModel.EdmxVersion != Common.Constants.EdmxVersion4 ? Visibility.Visible : Visibility.Collapsed;
                    advancedSettingsViewModel.FunctionImportsGenerator = AdvancedSettingsViewModel.FunctionImportsGenerator;
                    advancedSettingsViewModel.GenerateFunctionImports = AdvancedSettingsViewModel.GenerateFunctionImports;
                }
            };

            this.Pages.Add(ConfigODataEndpointViewModel);
            this.Pages.Add(AdvancedSettingsViewModel);
            this.IsFinishEnabled = true;
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            this._userSettings.Save();

            this.ServiceInstance.InstanceId = AdvancedSettingsViewModel.GeneratedFileName;
            this.ServiceInstance.Name = ConfigODataEndpointViewModel.ServiceName;
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
                    GenerateFunctionImports = AdvancedSettingsViewModel.GenerateFunctionImports
                };
            }

            serviceConfiguration.ServiceName = ConfigODataEndpointViewModel.ServiceName;
            serviceConfiguration.Endpoint = ConfigODataEndpointViewModel.Endpoint;
            serviceConfiguration.EdmxVersion = ConfigODataEndpointViewModel.EdmxVersion;
            serviceConfiguration.UseDataServiceCollection = AdvancedSettingsViewModel.UseDataServiceCollection;
            serviceConfiguration.GeneratedFileNamePrefix = AdvancedSettingsViewModel.GeneratedFileName;
            serviceConfiguration.UseNameSpacePrefix = AdvancedSettingsViewModel.UseNamespacePrefix;
            if (AdvancedSettingsViewModel.UseNamespacePrefix && !string.IsNullOrEmpty(AdvancedSettingsViewModel.NamespacePrefix))
            {
                serviceConfiguration.NamespacePrefix = AdvancedSettingsViewModel.NamespacePrefix;
            }

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
    }
}
