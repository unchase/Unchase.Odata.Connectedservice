// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.ConnectedServices;

namespace Unchase.OData.ConnectedService
{
    [ConnectedServiceProviderExport("Microsoft.Samples.Wizard", SupportsUpdate = true)]
    internal class Provider : ConnectedServiceProvider
    {
        public Provider()
        {
            this.Category = "OData";
            this.Name = "Unchase OData V1-V4 Connected Service";
            this.Description = "OData V1-V4 connected service with extension methods for client-side proxy class";
            this.Icon = new BitmapImage(new Uri("pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Resources/ProviderIcon.png"));
            this.CreatedBy = "Unchase";
            this.Version = new Version(0, 1, 0);
            this.MoreInfoUri = new Uri("https://github.com/unchase/Unchase.Odata.Connectedservice");
        }

        public override IEnumerable<Tuple<string, Uri>> GetSupportedTechnologyLinks()
        {
            // A list of supported technolgoies, such as which services it supports
            // If a Provider configured Dynamics CRM with Azure Redis Cache and Azure Auth, 
            // it would include the following
            yield return Tuple.Create("OData Website", new Uri("http://www.odata.org/"));
            yield return Tuple.Create("OData Docs and Samples", new Uri("http://odata.github.io/odata.net/"));
        }

        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderContext context)
        {
            var wizard = new Wizard(context);
            return Task.FromResult<ConnectedServiceConfigurator>(wizard);
        }
    }
}
