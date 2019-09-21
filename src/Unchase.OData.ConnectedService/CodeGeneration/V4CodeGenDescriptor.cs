// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Services.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.Templates;

namespace Unchase.OData.ConnectedService.CodeGeneration
{
    internal class V4CodeGenDescriptor : BaseCodeGenDescriptor
    {
        #region Properties and fields
        private ServiceConfigurationV4 ServiceConfiguration { get; }
        #endregion

        #region Constructors
        public V4CodeGenDescriptor(ConnectedServiceHandlerContext context, Instance serviceInstance)
            : base(context, serviceInstance)
        {
            this.ClientNuGetPackageName = Common.Constants.V4ClientNuGetPackage;
            this.ClientDocUri = Common.Constants.V4DocUri;
            this.ServiceConfiguration = base.Instance.ServiceConfig as ServiceConfigurationV4;
        }
        #endregion

        #region Methods

        #region NuGet
        public override async Task AddNugetPackagesAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages for OData V4...");

            foreach (var nugetPackage in Common.Constants.V4NuGetPackages)
                await CheckAndInstallNuGetPackageAsync(Common.Constants.NuGetOnlineRepository, nugetPackage);

            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages for OData V4 were installed.");
        }

        internal async Task CheckAndInstallNuGetPackageAsync(string packageSource, string nugetPackage)
        {
            try
            {
                if (!PackageInstallerServices.IsPackageInstalled(this.Project, nugetPackage))
                {
                    Version packageVersion = null;
                    PackageInstaller.InstallPackage(packageSource, this.Project, nugetPackage, packageVersion, false);

                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" for OData V4 was added.");
                }
                else
                {
                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" for OData V4 already installed.");
                }
            }
            catch (Exception ex)
            {
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Error, $"Nuget Package \"{nugetPackage}\" for OData V4 not installed. Error: {ex.Message}.");
            }
        }
        #endregion

        public override async Task AddGeneratedClientCodeAsync()
        {
            if (this.ServiceConfiguration.IncludeT4File)
            {
                await AddT4FileAsync();
            }
            else
            {
                await AddGeneratedCodeAsync();
            }
        }

        private async Task AddT4FileAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding T4 file for OData V4...");

            var tempFile = Path.GetTempFileName();
            var t4Folder = Path.Combine(this.CurrentAssemblyPath, "Templates");

            this.ServiceConfiguration.ExcludedOperationImportsNames += GetExcludedOperationImportsNames();

            // generate .tt
            using (var writer = File.CreateText(tempFile))
            {
                var text = File.ReadAllText(Path.Combine(t4Folder, "ODataT4CodeGenerator.tt"));

                text = Regex.Replace(text, "ODataT4CodeGenerator(\\.ttinclude)", this.GeneratedFileNamePrefix + "$1");
                text = Regex.Replace(text, "(public const string MetadataDocumentUri = )\"\";", "$1\"" + ServiceConfiguration.Endpoint + "\";");
                text = Regex.Replace(text, "(public const bool UseDataServiceCollection = ).*;", "$1" + ServiceConfiguration.UseDataServiceCollection.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const string NamespacePrefix = )\"\\$rootnamespace\\$\";", "$1\"" + ServiceConfiguration.NamespacePrefix + "\";");
                if (this.ServiceConfiguration.LanguageOption == LanguageOption.GenerateCSharpCode)
                    text = Regex.Replace(text, "(public const string TargetLanguage = )\"OutputLanguage\";", "$1\"CSharp\";");
                else if (this.ServiceConfiguration.LanguageOption == LanguageOption.GenerateVBCode)
                    text = Regex.Replace(text, "(public const string TargetLanguage = )\"OutputLanguage\";", "$1\"VB\";");

                text = Regex.Replace(text, "(public const bool EnableNamingAlias = )true;", "$1" + this.ServiceConfiguration.EnableNamingAlias.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool IgnoreUnexpectedElementsAndAttributes = )true;", "$1" + this.ServiceConfiguration.IgnoreUnexpectedElementsAndAttributes.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool GenerateDynamicPropertiesCollection = )true;", "$1" + this.ServiceConfiguration.GenerateDynamicPropertiesCollection.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const string DynamicPropertiesCollectionName = )\"DynamicProperties\";", "$1\"" + $"{(!string.IsNullOrWhiteSpace(ServiceConfiguration.DynamicPropertiesCollectionName) ? ServiceConfiguration.DynamicPropertiesCollectionName : Common.Constants.DefaultDynamicPropertiesCollectionName)}" + "\";");

                text = Regex.Replace(text, "(public const string ExcludedOperationImportsNames = )\"\";", "$1\"" + this.ServiceConfiguration.ExcludedOperationImportsNames + "\";");

                await writer.WriteAsync(text);
                await writer.FlushAsync();
            }
            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            await this.Context.HandlerHelper.AddFileAsync(tempFile, Path.Combine(this.ReferenceFileFolder, this.GeneratedFileNamePrefix + ".tt"), new AddFileOptions { OpenOnComplete = this.Instance.ServiceConfig.OpenGeneratedFilesOnComplete });

            // generate .ttinclude
            tempFile = Path.GetTempFileName();
            using (var writer = File.CreateText(tempFile))
            {
                var includeText = File.ReadAllText(Path.Combine(t4Folder, "ODataT4CodeGenerator.ttinclude"));
                if (this.ServiceConfiguration.LanguageOption == LanguageOption.GenerateVBCode)
                    includeText = Regex.Replace(includeText, "(output extension=)\".cs\"", "$1\".vb\"");
                await writer.WriteAsync(includeText);
                await writer.FlushAsync();
            }

            await this.Context.HandlerHelper.AddFileAsync(tempFile, Path.Combine(this.ReferenceFileFolder, this.GeneratedFileNamePrefix + ".ttinclude"), new AddFileOptions { OpenOnComplete = this.Instance.ServiceConfig.OpenGeneratedFilesOnComplete });

            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "T4 file for OData V4 was added.");
        }

        private async Task AddGeneratedCodeAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy for OData V4...");

            this.ServiceConfiguration.ExcludedOperationImportsNames += GetExcludedOperationImportsNames();

            var t4CodeGenerator = new ODataT4CodeGenerator
            {
                MetadataDocumentUri = MetadataUri,
                UseDataServiceCollection = this.ServiceConfiguration.UseDataServiceCollection,
                TargetLanguage = this.ServiceConfiguration.LanguageOption == LanguageOption.GenerateCSharpCode ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB,
                IgnoreUnexpectedElementsAndAttributes = this.ServiceConfiguration.IgnoreUnexpectedElementsAndAttributes,
                EnableNamingAlias = this.ServiceConfiguration.EnableNamingAlias,
                NamespacePrefix = this.ServiceConfiguration.NamespacePrefix,
                ExcludedOperationImportsNames = this.ServiceConfiguration?.ExcludedOperationImportsNames,
                GenerateDynamicPropertiesCollection = this.ServiceConfiguration.GenerateDynamicPropertiesCollection,
                DynamicPropertiesCollectionName = this.ServiceConfiguration?.DynamicPropertiesCollectionName
            };

            var tempFile = Path.GetTempFileName();

            using (var writer = File.CreateText(tempFile))
            {
                var proxyClassText = t4CodeGenerator.TransformText();
                await writer.WriteAsync(proxyClassText);
                await writer.FlushAsync();
                if (t4CodeGenerator.Errors != null && t4CodeGenerator.Errors.Count > 0)
                {
                    foreach (var err in t4CodeGenerator.Errors)
                        await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, err.ToString());
                }
            }
            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var outputFile = Path.Combine(this.ReferenceFileFolder, $"{this.GeneratedFileNamePrefix}{(this.ServiceConfiguration.LanguageOption == LanguageOption.GenerateCSharpCode ? ".cs" : ".vb")}");
            await this.Context.HandlerHelper.AddFileAsync(tempFile, outputFile, new AddFileOptions { OpenOnComplete = this.Instance.ServiceConfig.OpenGeneratedFilesOnComplete });

            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Client Proxy for OData V4 was generated.");
        }

        private string GetExcludedOperationImportsNames()
        {
            var result = string.Empty;
            var excludedOperationImportsNames = new List<string>();
            if (this.ServiceConfiguration.OperationImports?.Any() == true)
            {
                excludedOperationImportsNames.AddRange(this.ServiceConfiguration?.OperationImports?
                    .Where(oi => !oi.IsChecked && !string.IsNullOrWhiteSpace(oi.OperationImportName))
                    .Select(oi => oi.OperationImportName)
                    .ToList());
            }

            foreach (var excludedOperationImportName in excludedOperationImportsNames)
                result += $",{excludedOperationImportName}";

            return result;
        }
        #endregion
    }
}
