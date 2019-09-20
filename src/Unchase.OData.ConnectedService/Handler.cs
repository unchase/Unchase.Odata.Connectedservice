// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.CodeGeneration;
using Unchase.OData.ConnectedService.Models;

namespace Unchase.OData.ConnectedService
{
    [ConnectedServiceHandlerExport("Unchase.OData.ConnectedService", AppliesTo = "VB | CSharp | Web")]
    internal class Handler : ConnectedServiceHandler
    {
        #region Methods
        public override async Task<AddServiceInstanceResult> AddServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding service instance...");

            var codeGenInstance = (Instance)context.ServiceInstance;

            var codeGenDescriptor = await GenerateCodeAsync(codeGenInstance.ServiceConfig.EdmxVersion, context);

            codeGenInstance.ServiceConfig.FunctionImports = null;
            codeGenInstance.ServiceConfig.OperationImports = null;

            context.SetExtendedDesignerData<ServiceConfiguration>(codeGenInstance.ServiceConfig);

            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding service instance complete!");

            return new AddServiceInstanceResult(context.ServiceInstance.Name, new Uri(codeGenDescriptor.ClientDocUri));
        }

        public override async Task<UpdateServiceInstanceResult> UpdateServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Updating service instance...");

            var codeGenInstance = (Instance)context.ServiceInstance;

            var codeGenDescriptor = await GenerateCodeAsync(codeGenInstance.ServiceConfig.EdmxVersion, context);

            codeGenInstance.ServiceConfig.FunctionImports = null;
            codeGenInstance.ServiceConfig.OperationImports = null;

            context.SetExtendedDesignerData<ServiceConfiguration>(codeGenInstance.ServiceConfig);

            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Updating service instance complete.");

            return new UpdateServiceInstanceResult { GettingStartedDocument = new GettingStartedDocument(new Uri(codeGenDescriptor.ClientDocUri)) };
        }

        private static async Task<BaseCodeGenDescriptor> GenerateCodeAsync(Version edmxVersion, ConnectedServiceHandlerContext context)
        {
            var instance = (Instance)context.ServiceInstance;
            BaseCodeGenDescriptor codeGenDescriptor;

            if (edmxVersion == Common.Constants.EdmxVersion1
                || edmxVersion == Common.Constants.EdmxVersion2
                || edmxVersion == Common.Constants.EdmxVersion3)
            {
                codeGenDescriptor = new V3CodeGenDescriptor(context, instance);
            }
            else if (edmxVersion == Common.Constants.EdmxVersion4)
            {
                codeGenDescriptor = new V4CodeGenDescriptor(context, instance);
            }
            else
            {
                throw new Exception($"Not supported Edmx Version{(edmxVersion != null ? $" {edmxVersion}." : ". Try with Endpoint ends \"/$metadata\".")}");
            }

            codeGenDescriptor.UseNetworkCredentials = instance.ServiceConfig.UseNetworkCredentials;
            codeGenDescriptor.NetworkCredentialsUserName = instance.ServiceConfig.NetworkCredentialsUserName;
            codeGenDescriptor.NetworkCredentialsPassword = instance.ServiceConfig.NetworkCredentialsPassword;
            codeGenDescriptor.NetworkCredentialsDomain = instance.ServiceConfig.NetworkCredentialsDomain;

            codeGenDescriptor.UseWebProxy = instance.ServiceConfig.UseWebProxy;
            codeGenDescriptor.UseWebProxyCredentials = instance.ServiceConfig.UseWebProxyCredentials;
            codeGenDescriptor.WebProxyNetworkCredentialsUserName = instance.ServiceConfig.WebProxyNetworkCredentialsUserName;
            codeGenDescriptor.WebProxyNetworkCredentialsPassword = instance.ServiceConfig.WebProxyNetworkCredentialsPassword;
            codeGenDescriptor.WebProxyNetworkCredentialsDomain = instance.ServiceConfig.WebProxyNetworkCredentialsDomain;
            codeGenDescriptor.WebProxyUri = instance.ServiceConfig.WebProxyUri;

            await codeGenDescriptor.AddNugetPackagesAsync();
            await codeGenDescriptor.AddGeneratedClientCodeAsync();

            return codeGenDescriptor;
        }
        #endregion
    }
}
