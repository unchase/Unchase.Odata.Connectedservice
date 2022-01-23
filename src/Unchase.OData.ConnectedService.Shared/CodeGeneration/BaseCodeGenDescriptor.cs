// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ConnectedServices;
using NuGet.VisualStudio;
using Unchase.OData.ConnectedService.Common;

namespace Unchase.OData.ConnectedService.CodeGeneration
{
    internal abstract class BaseCodeGenDescriptor
    {
        #region Properties and fields
        public IVsPackageInstaller PackageInstaller { get; private set; }

        public IVsPackageInstallerServices PackageInstallerServices { get; private set; }

        public ConnectedServiceHandlerContext Context { get; private set; }

        public Project Project { get; private set; }

        public string MetadataUri { get; private set; }

        public string ClientNuGetPackageName { get; set; }

        public string ValueTupleNuGetPackageName { get; set; }

        public string SimpleODataClientNuGetPackageName { get; set; }

        public string SystemComponentModelAnnotationsNuGetPackageName { get; set; }

        public string ClientDocUri { get; set; }

        protected string GeneratedFileNamePrefix => string.IsNullOrWhiteSpace(this.Instance.ServiceConfig.GeneratedFileNamePrefix)
            ? Common.Constants.DefaultReferenceFileName : this.Instance.ServiceConfig.GeneratedFileNamePrefix;

        protected string CurrentAssemblyPath => Path.GetDirectoryName(this.GetType().Assembly.Location);

        public Instance Instance { get; private set; }

        public string ReferenceFileFolder
        {
            get
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                return this.GetReferenceFileFolder();
            }
        }

        #region Network Credentials
        public bool UseNetworkCredentials { get; set; }
        public string NetworkCredentialsUserName { get; set; }
        public string NetworkCredentialsPassword { get; set; }
        public string NetworkCredentialsDomain { get; set; }
        #endregion

        #region WebProxy
        public bool UseWebProxy { get; set; }
        public bool UseWebProxyCredentials { get; set; }
        public string WebProxyNetworkCredentialsUserName { get; set; }
        public string WebProxyNetworkCredentialsPassword { get; set; }
        public string WebProxyNetworkCredentialsDomain { get; set; }
        public string WebProxyUri { get; set; }
        #endregion

        #endregion

        #region Constructors
        protected BaseCodeGenDescriptor(ConnectedServiceHandlerContext context, Instance serviceInstance)
        {
            this.InitNuGetInstaller();

            this.Instance = serviceInstance;
            this.MetadataUri = serviceInstance.ServiceConfig.Endpoint;
            this.Context = context;
            this.Project = context.ProjectHierarchy.GetProject();
        }
        #endregion

        #region Methods
        private void InitNuGetInstaller()
        {
            var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            this.PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            this.PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
        }

        public abstract Task AddNugetPackagesAsync();

        public abstract Task AddGeneratedClientCodeAsync();

        protected string GetReferenceFileFolder()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var serviceReferenceFolderName = this.Context.HandlerHelper.GetServiceArtifactsRootFolder();

            return Path.Combine(this.Project.GetProjectFullPath(), serviceReferenceFolderName, this.Context.ServiceInstance.Name);
        }
        #endregion
    }
}
