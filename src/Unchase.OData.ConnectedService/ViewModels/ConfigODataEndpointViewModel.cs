// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Data.Services.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.Views;
using Constants = Unchase.OData.ConnectedService.Common.Constants;

namespace Unchase.OData.ConnectedService.ViewModels
{
	internal class ConfigODataEndpointViewModel : ConnectedServiceWizardPage
	{
        #region Properties and fields
        private string _endPoint;
        public string Endpoint
        {
            get => _endPoint;
            set
            {
                _endPoint = value;
                UserSettings.Endpoint = value;
                OnPropertyChanged(nameof(EndPoint));
            }
        }

		private string _serviceName;
        public string ServiceName
        {
            get => _serviceName;
            set
            {
                _serviceName = value;
                UserSettings.ServiceName = value;
                OnPropertyChanged(nameof(ServiceName));
            }
        }

		private Version _edmxVersion;
        public Version EdmxVersion
        {
            get => _edmxVersion;
            set
            {
                _edmxVersion = value;
                OnPropertyChanged(nameof(EdmxVersion));
            }
        }

		private LanguageOption _languageOption;
        public LanguageOption LanguageOption
        {
            get => _languageOption;
            set
            {
                _languageOption = value;
                UserSettings.LanguageOption = value;
                OnPropertyChanged(nameof(LanguageOption));
            }
        }

        public string MetadataTempPath { get; set; }

        public UserSettings UserSettings { get; }

        public Wizard InternalWizard { get; }

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

        public LanguageOption[] LanguageOptions
        {
            get
            {
                return Enum.GetNames(typeof(LanguageOption))
                    .Select(t => (LanguageOption)Enum.Parse(typeof(LanguageOption), t))
                    .ToArray();
            }
        }
        #endregion

        #region Constructors
        public ConfigODataEndpointViewModel(UserSettings userSettings, Wizard wizard) : base()
        {
            this.Title = "Configure metadata endpoint";
            this.Description = "Enter or choose an OData metadata endpoint to begin";
            this.Legend = "Metadata Endpoint";
            this.InternalWizard = wizard;
            this.View = new ConfigODataEndpoint
            {
                UserSettings = userSettings,
                DataContext = this
            };
            this.UserSettings = userSettings;
            this.ServiceName = userSettings.ServiceName ?? Constants.DefaultServiceName;
            this.Endpoint = userSettings.Endpoint;
            this.LanguageOption = userSettings.LanguageOption;
            this.UseNetworkCredentials = false;
            this.UseWebProxy = false;
            this.UseWebProxyCredentials = false;
        }
        #endregion

        #region Methods
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

        #region Private methods
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

            var xmlUrlResolver = new XmlUrlResolver
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
                return ReadMetadata(out edmxVersion, readerSettings, workFile);
            }
            catch (WebException e)
            {
                if (e.InnerException is System.Security.Authentication.AuthenticationException)
                {
                    var save = ServicePointManager.ServerCertificateValidationCallback;
                    ServicePointManager.ServerCertificateValidationCallback += LifeValidationCallback;
                    return ReadMetadata(out edmxVersion, readerSettings, workFile);
                }
                throw new InvalidOperationException($"Cannot access {this.UserSettings.Endpoint}\nException message: \"{e.Message}\".", e);
            }
        }

        /// <summary>
        /// Read the metadata.
        /// </summary>
        /// <param name="edmxVersion"><see cref="Version"/> of edmx.</param>
        /// <param name="readerSettings"><see cref="XmlReaderSettings"/>.</param>
        /// <param name="workFile"></param>
        /// <returns></returns>
        private string ReadMetadata(out Version edmxVersion, XmlReaderSettings readerSettings, string workFile)
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

                    Constants.SupportedEdmxNamespaces.TryGetValue(reader.NamespaceURI, out edmxVersion);
                    writer.WriteNode(reader, false);
                }
            }
            return workFile;
        }

        /// <summary>
        /// Validation of possibly untrusted certificates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"><see cref="X509Certificate"/>.</param>
        /// <param name="chain"><see cref="X509Chain"/>.</param>
        /// <param name="sslPolicyErrors"><see cref="SslPolicyErrors"/>.</param>
        /// <returns></returns>
        private bool LifeValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var sb = new StringBuilder();

            var x509 = certificate as X509Certificate2;
            sb.AppendLine();
            sb.AppendLine($"Content Type: {X509Certificate2.GetCertContentType(x509?.RawData ?? new byte[]{})}");
            sb.AppendLine($"Name: {certificate.Subject}");
            sb.AppendLine($"Certificate Verified?: {x509?.Verify()}");
            sb.AppendLine($"Simple Name: {x509?.GetNameInfo(X509NameType.SimpleName, true)}");
            sb.AppendLine($"Signature Algorithm: {x509?.SignatureAlgorithm.FriendlyName}");
            //sb.AppendLine($"Private Key: {x509?.PrivateKey.ToXmlString(false)}");
            //sb.AppendLine($"Public Key: {x509?.PublicKey.Key.ToXmlString(false)}");
            sb.AppendLine($"Certificate Archived?: {x509?.Archived}");
            sb.AppendLine($"Length of Raw Data: {x509?.RawData.Length}");

            var result = MessageBox.Show($"Would you like to Accept the untrusted Certificate:{Environment.NewLine}{sb}", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return false;

            return true;
        }
        #endregion

        #endregion
    }
}

