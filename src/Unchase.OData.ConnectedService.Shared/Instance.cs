// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Common;
using Unchase.OData.ConnectedService.Models;

namespace Unchase.OData.ConnectedService
{
    internal class Instance : ConnectedServiceInstance
    {
        #region Properties and fields
        public ServiceConfiguration ServiceConfig { get; set; }

        public string MetadataTempFilePath { get; set; }
        #endregion

        #region Constructors
        public Instance()
        {
            InstanceId = Constants.ExtensionCategory;
            Name = Constants.DefaultServiceName;
        }
        #endregion
    }
}
