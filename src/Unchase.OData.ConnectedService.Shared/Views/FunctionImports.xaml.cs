// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0. See License.txt in the project root for license information.

using System.Windows;
using System.Windows.Controls;

namespace Unchase.OData.ConnectedService.Views
{
    /// <summary>
    /// Логика взаимодействия для FunctionImports.xaml
    /// </summary>
    public partial class FunctionImports : UserControl
    {
        #region Properties and fields
        private readonly Wizard _wizard;
        #endregion

        #region Constructors
        public FunctionImports(Wizard wizard)
        {
            InitializeComponent();
            _wizard = wizard;

            if (_wizard.ConfigODataEndpointViewModel.EdmxVersion == Common.Constants.EdmxVersion4)
            {
                FunctionImportsListBox.Visibility = Visibility.Collapsed;
                FunctionImportsCountLabel.Visibility = Visibility.Collapsed;
                OperationImportsListBox.Visibility = Visibility.Visible;
                OperationImportsCountLabel.Visibility = Visibility.Visible;
            }
            else
            {
                FunctionImportsListBox.Visibility = Visibility.Visible;
                FunctionImportsCountLabel.Visibility = Visibility.Visible;
                OperationImportsListBox.Visibility = Visibility.Collapsed;
                OperationImportsCountLabel.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}
