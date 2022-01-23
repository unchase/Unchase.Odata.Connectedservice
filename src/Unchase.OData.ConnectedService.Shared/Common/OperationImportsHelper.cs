// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.OData.Edm;
using Unchase.OData.ConnectedService.Models;
using IEdmModel = Microsoft.OData.Edm.IEdmModel;

namespace Unchase.OData.ConnectedService.Common
{
    internal static class OperationImportsHelper
    {
        #region Methods
        internal static List<OperationImportModel> GetOperationsImports(IEdmModel model, string proxyClassNamespace, string endpointUri)
        {
            var result = new List<OperationImportModel>();
            if (model == null)
                return result;

            foreach (var modelEntityContainer in model.EntityContainers())
            {
                foreach (var operationImport in modelEntityContainer.OperationImports())
                    result.Add(new OperationImportModel(model, operationImport, endpointUri, proxyClassNamespace));
            }

            return result;
        }
        #endregion
    }
}
