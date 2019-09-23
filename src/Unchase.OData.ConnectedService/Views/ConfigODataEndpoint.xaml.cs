// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unchase.OData.ConnectedService.Common;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.ViewModels;

namespace Unchase.OData.ConnectedService.Views
{
    public partial class ConfigODataEndpoint : UserControl
    {
        #region Properties and fields
        private const string ReportABugUrlFormat = "https://github.com/unchase/Unchase.OData.Connectedservice/issues/new?title={0}&labels=bug&body={1}";

        internal UserSettings UserSettings { get; set; }
        #endregion

        #region Constructors
        public ConfigODataEndpoint()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void ReportABugButton_Click(object sender, RoutedEventArgs e)
        {
            var title = Uri.EscapeUriString("<BUG title>");
            var body = Uri.EscapeUriString("<Please describe what bug you found when using the service.>");
            var url = string.Format(ReportABugUrlFormat, title, body);
            System.Diagnostics.Process.Start(url);
        }

        private void OpenConnectedServiceJsonFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "OData Connected Service json-files (.json)|*.json",
                Title = "Open OData Connected Service json-file"
            };

            var result = openFileDialog.ShowDialog();
            if (result == false)
                return;

            if (!File.Exists(openFileDialog.FileName))
            {
                MessageBox.Show($"File \"{openFileDialog.FileName}\" does not exists.", "Open OData Connected Service json-file", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var jsonFileText = File.ReadAllText(openFileDialog.FileName);
            if (string.IsNullOrWhiteSpace(jsonFileText))
            {
                MessageBox.Show("File have not content.", "Open OData Connected Service json-file", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (JObject.Parse(jsonFileText) == null)
            {
                MessageBox.Show("Can't convert file content to JObject.", "Open OData Connected Service json-file", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var microsoftConnectedServiceData = JsonConvert.DeserializeObject<ConnectedServiceJsonFileData>(jsonFileText);
            if (microsoftConnectedServiceData != null && this.UserSettings != null)
            {
                this.UserSettings.IncludeExtensionsT4File = microsoftConnectedServiceData.ExtendedData?.IncludeExtensionsT4File ?? this.UserSettings.IncludeExtensionsT4File;
                this.UserSettings.EnableNamingAlias = microsoftConnectedServiceData.ExtendedData?.EnableNamingAlias ?? this.UserSettings.EnableNamingAlias;
                this.UserSettings.IgnoreUnexpectedElementsAndAttributes = microsoftConnectedServiceData.ExtendedData?.IgnoreUnexpectedElementsAndAttributes ?? this.UserSettings.IgnoreUnexpectedElementsAndAttributes;
                this.UserSettings.GenerateDynamicPropertiesCollection = microsoftConnectedServiceData.ExtendedData?.GenerateDynamicPropertiesCollection ?? this.UserSettings.GenerateDynamicPropertiesCollection;
                this.UserSettings.DynamicPropertiesCollectionName = microsoftConnectedServiceData.ExtendedData?.DynamicPropertiesCollectionName ?? this.UserSettings.DynamicPropertiesCollectionName;
                this.UserSettings.IncludeT4File = microsoftConnectedServiceData.ExtendedData?.IncludeT4File ?? this.UserSettings.IncludeT4File;
                this.UserSettings.LanguageOption = microsoftConnectedServiceData.ExtendedData?.LanguageOption ?? this.UserSettings.LanguageOption;
                this.UserSettings.OperationImportsGenerator = microsoftConnectedServiceData.ExtendedData?.OperationImportsGenerator ?? this.UserSettings.OperationImportsGenerator;
                this.UserSettings.SelectOperationImports = microsoftConnectedServiceData.ExtendedData?.SelectOperationImports ?? this.UserSettings.SelectOperationImports;
                this.UserSettings.Endpoint = microsoftConnectedServiceData.ExtendedData?.Endpoint ?? this.UserSettings.Endpoint;
                this.UserSettings.ExcludedOperationImportsNames = microsoftConnectedServiceData.ExtendedData?.ExcludedOperationImportsNames ?? this.UserSettings.ExcludedOperationImportsNames;
                this.UserSettings.UseNameSpacePrefix = microsoftConnectedServiceData.ExtendedData?.UseNameSpacePrefix ?? this.UserSettings.UseNameSpacePrefix;
                this.UserSettings.UseDataServiceCollection = microsoftConnectedServiceData.ExtendedData?.UseDataServiceCollection ?? this.UserSettings.UseDataServiceCollection;
                this.UserSettings.ServiceName = microsoftConnectedServiceData.ExtendedData?.ServiceName ?? this.UserSettings.ServiceName;
                this.UserSettings.AcceptAllUntrustedCertificates = microsoftConnectedServiceData.ExtendedData?.AcceptAllUntrustedCertificates ?? this.UserSettings.AcceptAllUntrustedCertificates;
                this.UserSettings.NamespacePrefix = microsoftConnectedServiceData.ExtendedData?.NamespacePrefix ?? this.UserSettings.NamespacePrefix;
                this.UserSettings.OpenGeneratedFilesOnComplete = microsoftConnectedServiceData.ExtendedData?.OpenGeneratedFilesOnComplete ?? this.UserSettings.OpenGeneratedFilesOnComplete;
                this.UserSettings.GeneratedFileNamePrefix = microsoftConnectedServiceData.ExtendedData?.GeneratedFileNamePrefix ?? this.UserSettings.GeneratedFileNamePrefix;

                if (this.UserSettings.LanguageOption == System.Data.Services.Design.LanguageOption.GenerateVBCode || !this.UserSettings.SelectOperationImports)
                    ((ConfigODataEndpointViewModel)this.DataContext).InternalWizard.RemoveOperationImportsSettingsPage();

                this.Endpoint.Text = microsoftConnectedServiceData.ExtendedData?.Endpoint ?? this.Endpoint.Text;
                this.ServiceName.Text = microsoftConnectedServiceData.ExtendedData?.ServiceName ?? this.ServiceName.Text;
                this.AcceptAllUntrustedCertificates.IsChecked = microsoftConnectedServiceData.ExtendedData?.AcceptAllUntrustedCertificates ?? this.AcceptAllUntrustedCertificates.IsChecked;
                this.LanguageOption.SelectedItem = microsoftConnectedServiceData.ExtendedData?.LanguageOption ?? System.Data.Services.Design.LanguageOption.GenerateCSharpCode;
            }
        }

        private void OpenEndpointFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "Endpoint metadata Files (.xml)|*.xml",
                Title = "Open endpoint metadata file"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                Endpoint.Text = openFileDialog.FileName;
            }
        }

        private void LanguageOption_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UserSettings.LanguageOption == System.Data.Services.Design.LanguageOption.GenerateVBCode || !this.UserSettings.SelectOperationImports)
                ((ConfigODataEndpointViewModel)this.DataContext).InternalWizard.RemoveOperationImportsSettingsPage();
        }
        #endregion
    }
}
