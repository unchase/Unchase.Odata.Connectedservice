// Copyright (c) Microsoft Corporation.  All rights reserved.
// Updated by Unchase (https://github.com/unchase).
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
