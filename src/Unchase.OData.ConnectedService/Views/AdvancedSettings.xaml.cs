// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Data.Services.Design;
using System.Windows;
using System.Windows.Controls;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.ViewModels;

namespace Unchase.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for AdvancedSettings.xaml
    /// </summary>
    public partial class AdvancedSettings : UserControl
    {
        public AdvancedSettings()
        {
            InitializeComponent();
            this.AdvancedSettingsPanel.Visibility = Visibility.Hidden;
        }

        internal Wizard ServiceWizard => ((AdvancedSettingsViewModel)this.DataContext).Wizard as Wizard;

        internal UserSettings UserSettings => ((AdvancedSettingsViewModel) this.DataContext).UserSettings;

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsPanel.Visibility = Visibility.Hidden;

            this.AdvancedSettingsPanel.Margin = new Thickness(10, -125, 0, 0);
            this.AdvancedSettingsPanel.Visibility = Visibility.Visible;

            this.AdvancedSettingsForv4.Visibility = this.ServiceWizard.EdmxVersion == Common.Constants.EdmxVersion4
                ? Visibility.Visible : Visibility.Hidden;

            this.ExtensionMethodsForCSharpStackPanel.Visibility = this.UserSettings.LanguageOption != LanguageOption.GenerateCSharpCode
                ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
