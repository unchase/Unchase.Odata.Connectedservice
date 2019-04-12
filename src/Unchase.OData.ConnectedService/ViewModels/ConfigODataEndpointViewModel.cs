// Copyright (c) Microsoft Corporation.  All rights reserved.
// Updated by Unchase (https://github.com/unchase).
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Common;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.Views;

namespace Unchase.OData.ConnectedService.ViewModels
{
    internal class ConfigODataEndpointViewModel : ConnectedServiceWizardPage
    {
        public string Endpoint { get; set; }

        public string ServiceName { get; set; }

        public Version EdmxVersion { get; set; }

        public string MetadataTempPath { get; set; }

        public UserSettings UserSettings { get; }

        #region Network Credentials
        public bool UseNetworkCredentials { get; set; }
        public string NetworkCredentialsUserName { get; set; }
        public string NetworkCredentialsPassword { get; set; }
        public string NetworkCredentialsDomain { get; set; }
        #endregion

        #region WebProxy
        public bool UseWebProxy { get; set; }
        public bool UseWebProxyCredentials { get; set; }
        public string WebProxyNetworkCredentialsUserName { get; set; }
        public string WebProxyNetworkCredentialsPassword { get; set; }
        public string WebProxyNetworkCredentialsDomain { get; set; }
        public string WebProxyUri { get; set; }
        #endregion

        public ConfigODataEndpointViewModel(UserSettings userSettings) : base()
        {
            this.Title = "Configure metadata endpoint";
            this.Description = "Enter or choose an OData metadata endpoint to begin";
            this.Legend = "Metadata Endpoint";
            this.View = new ConfigODataEndpoint { DataContext = this };
            this.UserSettings = userSettings;
            this.ServiceName = userSettings.ServiceName ?? Constants.DefaultServiceName;
            this.Endpoint = userSettings.Endpoint;
            this.UseNetworkCredentials = false;
            this.UseWebProxy = false;
            this.UseWebProxyCredentials = false;
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            UserSettings.AddToTopOfMruList(((Wizard)this.Wizard).UserSettings.MruEndpoints, this.Endpoint);
            try
            {
                this.MetadataTempPath = GetMetadata(out var version);
                this.EdmxVersion = version;
                return base.OnPageLeavingAsync(args);
            }
            catch (Exception e)
            {
                return Task.FromResult<PageNavigationResult>(
                    new PageNavigationResult()
                    {
                        ErrorMessage = e.Message,
                        IsSuccess = false,
                        ShowMessageBoxOnFailure = true
                    });
            }
        }

        private string GetMetadata(out Version edmxVersion)
        {
            if (string.IsNullOrEmpty(this.UserSettings.Endpoint))
            {
                throw new ArgumentNullException("OData Service Endpoint", "Please input the service endpoint");
            }

            if (this.UserSettings.Endpoint.StartsWith("https:", StringComparison.Ordinal) || this.UserSettings.Endpoint.StartsWith("http", StringComparison.Ordinal))
            {
                if (!this.UserSettings.Endpoint.EndsWith("$metadata", StringComparison.Ordinal))
                    this.UserSettings.Endpoint = this.UserSettings.Endpoint.TrimEnd('/') + "/$metadata";
            }

            var xmlUrlResolver = new XmlUrlResolver()
            {
                Credentials = this.UseNetworkCredentials ? new NetworkCredential(this.NetworkCredentialsUserName, this.NetworkCredentialsPassword, this.NetworkCredentialsDomain) : CredentialCache.DefaultNetworkCredentials
            };
            if (this.UseWebProxy)
            {
                xmlUrlResolver.Proxy = new WebProxy(this.WebProxyUri, true);
                if (this.UseWebProxyCredentials)
                    xmlUrlResolver.Proxy = new WebProxy(this.WebProxyUri, true, new string[0], new NetworkCredential
                    {
                        UserName = WebProxyNetworkCredentialsUserName,
                        Password = WebProxyNetworkCredentialsPassword,
                        Domain = WebProxyNetworkCredentialsDomain
                    });
                else
                    xmlUrlResolver.Proxy = new WebProxy(this.WebProxyUri);
            }

            var readerSettings = new XmlReaderSettings()
            {
                XmlResolver = new XmlSecureResolver(xmlUrlResolver, new PermissionSet(System.Security.Permissions.PermissionState.Unrestricted))
            };

            var workFile = Path.GetTempFileName();

            try
            {
                using (var reader = XmlReader.Create(this.UserSettings.Endpoint, readerSettings))
                {
                    using (var writer = XmlWriter.Create(workFile))
                    {
                        while (reader.NodeType != XmlNodeType.Element)
                        {
                            reader.Read();
                        }

                        if (reader.EOF)
                        {
                            throw new InvalidOperationException("The metadata is an empty file");
                        }

                        Common.Constants.SupportedEdmxNamespaces.TryGetValue(reader.NamespaceURI, out edmxVersion);
                        writer.WriteNode(reader, false);
                    }
                }
                return workFile;
            }
            catch (WebException e)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot access {0}", this.UserSettings.Endpoint), e);
            }
        }
    }
}
