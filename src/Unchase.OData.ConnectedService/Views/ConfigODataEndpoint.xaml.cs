// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0. See License.txt in the project root for license information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace Unchase.OData.ConnectedService.Views
{
    public partial class ConfigODataEndpoint : UserControl
    {
        #region Properties and fields
        private const string ReportABugUrlFormat = "https://github.com/unchase/Unchase.OData.Connectedservice/issues/new?title={0}&labels=bug&body={1}";
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
        #endregion
    }
}
