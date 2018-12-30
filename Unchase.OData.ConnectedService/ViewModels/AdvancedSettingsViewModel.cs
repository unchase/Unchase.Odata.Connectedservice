// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Views;

namespace Unchase.OData.ConnectedService.ViewModels
{
    internal class AdvancedSettingsViewModel : ConnectedServiceWizardPage
    {
        public bool UseDataServiceCollection { get; set; }
        public bool UseNamespacePrefix { get; set; }
        public string NamespacePrefix { get; set; }
        public bool EnableNamingAlias { get; set; }
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }
        public string GeneratedFileName { get; set; }
        public bool IncludeT4File { get; set; }
        public bool IncludeExtensionsT4File { get; set; }
        public Visibility IncludeExtensionsT4FileVisibility { get; set; }

        public AdvancedSettingsViewModel() : base()
        {
            this.Title = "Settings";
            this.Description = "Advanced settings for generating client proxy and extension methods for call service functions";
            this.Legend = "Settings";
            this.IncludeExtensionsT4File = false;
            this.IncludeExtensionsT4FileVisibility = Visibility.Collapsed;
            this.ResetDataContext();
        }

        public event EventHandler<EventArgs> PageEntering;

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args);

            this.View = new AdvancedSettings();
            this.ResetDataContext();
            this.View.DataContext = this;
            PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return base.OnPageLeavingAsync(args);
        }

        private void ResetDataContext()
        {
            this.UseNamespacePrefix = false;
            this.NamespacePrefix = Common.Constants.DefaultNamespacePrefix;
            this.UseDataServiceCollection = true;
            this.IgnoreUnexpectedElementsAndAttributes = false;
            this.EnableNamingAlias = false;
            this.GeneratedFileName = Common.Constants.DefaultReferenceFileName;
            this.IncludeT4File = false;
            this.IncludeExtensionsT4File = false;
            this.IncludeExtensionsT4FileVisibility = Visibility.Collapsed;
        }
    }
}
