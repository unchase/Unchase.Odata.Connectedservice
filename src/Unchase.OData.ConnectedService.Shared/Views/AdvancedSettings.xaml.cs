// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0. See License.txt in the project root for license information.

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
        #region Properties and fields
        private readonly Wizard _wizard;

        internal UserSettings UserSettings => ((AdvancedSettingsViewModel)this.DataContext).UserSettings;
        #endregion

        #region Constructors
        public AdvancedSettings(Wizard wizard)
        {
            InitializeComponent();
            _wizard = wizard;
            this.AdvancedSettingsPanel.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Methods
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsPanel.Visibility = Visibility.Hidden;

            this.AdvancedSettingsPanel.Margin = new Thickness(10, -125, 0, 0);
            this.AdvancedSettingsPanel.Visibility = Visibility.Visible;

            this.AdvancedSettingsForv4.Visibility = _wizard.EdmxVersion == Common.Constants.EdmxVersion4
                ? Visibility.Visible : Visibility.Hidden;

            if (this.UserSettings.LanguageOption != LanguageOption.GenerateCSharpCode)
            {
                this.ExtensionMethodsForCSharpStackPanel.Visibility = Visibility.Collapsed;
                this.SelectOperationImports.Visibility = Visibility.Collapsed;
                _wizard.RemoveOperationImportsSettingsPage();
            }
            else
            {
                this.ExtensionMethodsForCSharpStackPanel.Visibility = Visibility.Visible;
                this.SelectOperationImports.Visibility = Visibility.Visible;
            }
        }

        private void SelectOperationImports_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _wizard.RemoveOperationImportsSettingsPage();
            FunctionImportsStackPanel.Visibility = Visibility.Collapsed;
        }

        private void SelectOperationImports_OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.UserSettings.LanguageOption != LanguageOption.GenerateVBCode)
            {
                _wizard.AddOperationImportsSettingsPage();
                FunctionImportsStackPanel.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
