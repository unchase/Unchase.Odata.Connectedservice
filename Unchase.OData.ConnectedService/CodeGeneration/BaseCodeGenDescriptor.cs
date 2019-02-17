// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ConnectedServices;
using NuGet.VisualStudio;
using Unchase.OData.ConnectedService.Common;
using Unchase.OData.ConnectedService.Models;

namespace Unchase.OData.ConnectedService.CodeGeneration
{
    internal abstract class BaseCodeGenDescriptor
    {
        public IVsPackageInstaller PackageInstaller { get; private set; }
        public IVsPackageInstallerServices PackageInstallerServices { get; private set; }
        public ConnectedServiceHandlerContext Context { get; private set; }
        public ServiceConfiguration ServiceConfiguration { get; set; }
        public Project Project { get; private set; }
        public string MetadataUri { get; private set; }
        public string ClientNuGetPackageName { get; set; }
        public string ValueTupleNuGetPackageName { get; set; }
        public string SimpleODataClientNuGetPackageName { get; set; }
        public string SystemComponentModelAnnotationsNuGetPackageName { get; set; }
        public string ClientDocUri { get; set; }
        protected string GeneratedFileNamePrefix => string.IsNullOrWhiteSpace(this.ServiceConfiguration.GeneratedFileNamePrefix)
            ? Common.Constants.DefaultReferenceFileName : this.ServiceConfiguration.GeneratedFileNamePrefix;

        protected string CurrentAssemblyPath => Path.GetDirectoryName(this.GetType().Assembly.Location);

        protected BaseCodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext Context, Project project)
        {
            this.Init();

            this.MetadataUri = metadataUri;
            this.Context = Context;
            this.Project = project;
            this.ServiceConfiguration = ((Instance)this.Context.ServiceInstance).ServiceConfig;
        }

        private void Init()
        {
            var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            this.PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            this.PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
        }

        public abstract Task AddNugetPackages();
        public abstract Task AddGeneratedClientCode();

        protected string GetReferenceFileFolder()
        {
            var serviceReferenceFolderName = this.Context.HandlerHelper.GetServiceArtifactsRootFolder();

            return Path.Combine(ProjectHelper.GetProjectFullPath(this.Project), serviceReferenceFolderName, this.Context.ServiceInstance.Name);
        }
    }
}
