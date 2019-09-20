// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Common;
using Unchase.OData.ConnectedService.Properties;

namespace Unchase.OData.ConnectedService
{
    [ConnectedServiceProviderExport("Unchase.OData.ConnectedService", SupportsUpdate = true)]
    internal class Provider : ConnectedServiceProvider
    {
        #region Constructors
        public Provider()
        {
            Category = Constants.ExtensionCategory;
            Name = Constants.ExtensionName;
            Description = Constants.ExtensionDescription;
            Icon = Imaging.CreateBitmapSourceFromHBitmap(
                Resources.icon_32x32.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(32, 32)
            );
            CreatedBy = Constants.Author;
            Version = new Version(1, 3, 0, 0);
            Version = typeof(Provider).Assembly.GetName().Version;
            MoreInfoUri = new Uri(Constants.Website);
        }
        #endregion

        #region Methods
        public override IEnumerable<Tuple<string, Uri>> GetSupportedTechnologyLinks()
        {
            yield return Tuple.Create("OData Website", new Uri("http://www.odata.org/"));
            yield return Tuple.Create("OData Docs and Samples", new Uri("http://odata.github.io/odata.net/"));
        }

        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderContext context)
        {
            var wizard = new Wizard(context);
            return Task.FromResult<ConnectedServiceConfigurator>(wizard);
        }
        #endregion
    }
}
