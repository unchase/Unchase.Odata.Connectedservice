// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Models;

namespace Unchase.OData.ConnectedService
{
    internal class Instance : ConnectedServiceInstance
    {
        public ServiceConfiguration ServiceConfig { get; set; }
        public string MetadataTempFilePath { get; set; }
    }
}
