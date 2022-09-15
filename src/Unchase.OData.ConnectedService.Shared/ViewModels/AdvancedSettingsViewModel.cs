﻿// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Data.Services.Design;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Debugger.Interop;
using Common = Unchase.OData.ConnectedService.Common;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.Views;

namespace Unchase.OData.ConnectedService.ViewModels
{
    internal class AdvancedSettingsViewModel : ConnectedServiceWizardPage
    {
        #region Properties and fields
        public Common.Constants.OperationImportsGenerator[] OperationImportsGenerators =>
            new[] { Common.Constants.OperationImportsGenerator.Inner, Common.Constants.OperationImportsGenerator.SimpleOData};

        #region UseDataServiceCollection
        private bool _useDataServiceCollection;
        public bool UseDataServiceCollection
        {
            get => _useDataServiceCollection;
            set
            {
                _useDataServiceCollection = value;
                UserSettings.UseDataServiceCollection = value;
                OnPropertyChanged(nameof(UseDataServiceCollection));
                OnPropertyChanged(nameof(ShowAsyncDataServiceCollectionOption));
            }
        }
        #endregion

        #region UseAsyncDataServiceCollection
        private bool _useAsyncDataServiceCollection;

        /// <summary>
        /// Change the INotifyPropertyChanged Implementation to support async operations with synchronous event callbacks
        /// </summary>
        /// <remarks>This should only be set to true is the <see cref="UseDataServiceCollection"/> is also true.</remarks>
        public bool UseAsyncDataServiceCollection
        {
            get => _useAsyncDataServiceCollection;
            set
            {
                _useAsyncDataServiceCollection = value;
                UserSettings.UseAsyncDataServiceCollection = value;
                OnPropertyChanged(nameof(UseAsyncDataServiceCollection));
            }
        }
        #endregion

        #region  ShowAsyncDataServiceCollectionOption
        /// <summary>
        /// Show the advanced option for INotifyPropertyChanged notification that generates async and await compatible bindings 
        /// only available in C# and when <see cref="UseDataServiceCollection"/> is enabled.
        /// </summary>
        public bool ShowAsyncDataServiceCollectionOption
        {
            get => this.UserSettings.LanguageOption == LanguageOption.GenerateCSharpCode && UseDataServiceCollection;
        }
        #endregion

        #region UseNamespacePrefix
        private bool _useNamespacePrefix;
        public bool UseNamespacePrefix
        {
            get => _useNamespacePrefix;
            set
            {
                _useNamespacePrefix = value;
                UserSettings.UseNameSpacePrefix = value;
                OnPropertyChanged(nameof(UseNamespacePrefix));
            }
        }
        #endregion

        #region NamespacePrefix
        private string _namespacePrefix;
        public string NamespacePrefix
        {
            get => _namespacePrefix;
            set
            {
                _namespacePrefix = value;
                UserSettings.NamespacePrefix = value;
                OnPropertyChanged(nameof(NamespacePrefix));
            }
        }
        #endregion

        #region EnableNamingAlias
        private bool _enableNamingAlias;
        public bool EnableNamingAlias
        {
            get => _enableNamingAlias;
            set
            {
                _enableNamingAlias = value;
                UserSettings.EnableNamingAlias = value;
                OnPropertyChanged(nameof(EnableNamingAlias));
            }
        }
        #endregion

        #region IgnoreUnexpectedElementsAndAttributes
        private bool _ignoreUnexpectedElementsAndAttributes;
        public bool IgnoreUnexpectedElementsAndAttributes
        {
            get => _ignoreUnexpectedElementsAndAttributes;
            set
            {
                _ignoreUnexpectedElementsAndAttributes = value;
                UserSettings.IgnoreUnexpectedElementsAndAttributes = value;
                OnPropertyChanged(nameof(IgnoreUnexpectedElementsAndAttributes));
            }
        }
        #endregion

        #region GenerateDynamicPropertiesCollection
        private bool _generateDynamicPropertiesCollection;
        public bool GenerateDynamicPropertiesCollection
        {
            get => _generateDynamicPropertiesCollection;
            set
            {
                _generateDynamicPropertiesCollection = value;
                UserSettings.GenerateDynamicPropertiesCollection = value;
                OnPropertyChanged(nameof(GenerateDynamicPropertiesCollection));
            }
        }
        #endregion

        #region DynamicPropertiesCollectionName
        private string _dynamicPropertiesCollectionName;
        public string DynamicPropertiesCollectionName
        {
            get => _dynamicPropertiesCollectionName;
            set
            {
                _dynamicPropertiesCollectionName = value;
                UserSettings.DynamicPropertiesCollectionName = value;
                OnPropertyChanged(nameof(DynamicPropertiesCollectionName));
            }
        }
        #endregion

        #region GeneratedFileNameEnabled
        public bool GeneratedFileNameEnabled { get; set; }
        #endregion

        #region GeneratedFileNamePrefix
        private string _generatedFileNamePrefix;
        public string GeneratedFileNamePrefix
        {
            get => _generatedFileNamePrefix;
            set
            {
                _generatedFileNamePrefix = value;
                UserSettings.GeneratedFileNamePrefix = value;
                OnPropertyChanged(nameof(GeneratedFileNamePrefix));
            }
        }
        #endregion

        #region IncludeT4FileEnabled
        public bool IncludeT4FileEnabled { get; set; }
        #endregion

        #region IncludeT4File
        private bool _includeT4File;
        public bool IncludeT4File
        {
            get => _includeT4File;
            set
            {
                _includeT4File = value;
                UserSettings.IncludeT4File = value;
                OnPropertyChanged(nameof(IncludeT4File));
            }
        }
        #endregion

        #region EmbedEdmxFileEnabled
        public bool EmbedEdmxFileEnabled { get; set; }
        #endregion

        #region EmbedEdmxFile
        private bool _embedEdmxFile;
        public bool EmbedEdmxFile
        {
            get => _embedEdmxFile;
            set
            {
                _embedEdmxFile = value;
                UserSettings.EmbedEdmxFile = value;
                OnPropertyChanged(nameof(EmbedEdmxFile));
            }
        }
        #endregion

        #region MakeTypesInternal
        private bool _makeTypesInternal;
        public bool MakeTypesInternal
        {
            get => _makeTypesInternal;
            set
            {
                _makeTypesInternal = value;
                UserSettings.MakeTypesInternal = value;
                OnPropertyChanged(nameof(MakeTypesInternal));
            }
        }
        #endregion

        #region IncludeExtensionsT4File
        private bool _includeExtensionsT4File;
        public bool IncludeExtensionsT4File
        {
            get => _includeExtensionsT4File;
            set
            {
                _includeExtensionsT4File = value;
                UserSettings.IncludeExtensionsT4File = value;
                OnPropertyChanged(nameof(IncludeExtensionsT4File));
            }
        }
        #endregion

        #region OperationImportsGenerator
        private Common.Constants.OperationImportsGenerator _operationImportsGenerator;
        public Common.Constants.OperationImportsGenerator OperationImportsGenerator
        {
            get => _operationImportsGenerator;
            set
            {
                _operationImportsGenerator = value;
                UserSettings.OperationImportsGenerator = value;
                OnPropertyChanged(nameof(OperationImportsGenerator));
            }
        }
        #endregion

        #region SelectOperationImports
        private bool _selectOperationImports;
        public bool SelectOperationImports
        {
            get => _selectOperationImports;
            set
            {
                _selectOperationImports = value;
                UserSettings.SelectOperationImports = value;
                OnPropertyChanged(nameof(SelectOperationImports));
            }
        }
        #endregion

        #region ExcludedOperationImportsNames
        private string _excludedOperationImportsNames;
        public string ExcludedOperationImportsNames
        {
            get => _excludedOperationImportsNames;
            set
            {
                _excludedOperationImportsNames = value;
                UserSettings.ExcludedOperationImportsNames = value;
                OnPropertyChanged(nameof(ExcludedOperationImportsNames));
            }
        }
        #endregion

        public Visibility IncludeExtensionsT4FileVisibility { get; set; }

        public UserSettings UserSettings { get; }

        public Wizard InternalWizard;
        #endregion

        #region Constructors
        public AdvancedSettingsViewModel(UserSettings userSettings, Wizard wizard) : base()
        {
            this.Title = "Advanced Settings";
            this.Description = "Advanced settings for generating client proxy and extension methods for call service functions";
            this.Legend = "Advanced Settings";
            this.UserSettings = userSettings;
            this.InternalWizard = wizard;
        }
        #endregion

        #region Methods
        public event EventHandler<EventArgs> PageEntering;

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args);

            this.View = new AdvancedSettings(this.InternalWizard);
            this.UseDataServiceCollection = UserSettings.UseDataServiceCollection;
            this.UseAsyncDataServiceCollection = UserSettings.UseAsyncDataServiceCollection;
            this.UseNamespacePrefix = UserSettings.UseNameSpacePrefix;
            this.NamespacePrefix = UserSettings.NamespacePrefix ?? Common.Constants.DefaultNamespacePrefix;
            this.EnableNamingAlias = UserSettings.EnableNamingAlias;
            this.IgnoreUnexpectedElementsAndAttributes = UserSettings.IgnoreUnexpectedElementsAndAttributes;
            this.GenerateDynamicPropertiesCollection = UserSettings.GenerateDynamicPropertiesCollection;
            this.DynamicPropertiesCollectionName = UserSettings.DynamicPropertiesCollectionName;
            this.GeneratedFileNameEnabled = true;
            this.GeneratedFileNamePrefix = UserSettings.GeneratedFileNamePrefix ?? Common.Constants.DefaultReferenceFileName;
            this.IncludeT4FileEnabled = true;
            this.IncludeT4File = UserSettings.IncludeT4File;
            this.EmbedEdmxFileEnabled = true;
            this.EmbedEdmxFile = UserSettings.EmbedEdmxFile;
            this.MakeTypesInternal = UserSettings.MakeTypesInternal;
            this.IncludeExtensionsT4File = UserSettings.IncludeExtensionsT4File;
            this.OperationImportsGenerator = UserSettings.OperationImportsGenerator;
            this.SelectOperationImports = UserSettings.SelectOperationImports;
            this.ExcludedOperationImportsNames = UserSettings.ExcludedOperationImportsNames ?? string.Empty;
            this.View.DataContext = this;

            PageEntering?.Invoke(this, EventArgs.Empty);

            if (this.UserSettings.LanguageOption != LanguageOption.GenerateCSharpCode || !this.UserSettings.SelectOperationImports)
                this.InternalWizard.RemoveOperationImportsSettingsPage();
        }
        #endregion
    }
}
