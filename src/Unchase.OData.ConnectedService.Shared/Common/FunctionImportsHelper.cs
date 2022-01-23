// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Csdl;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Models;
using IEdmModel = Microsoft.Data.Edm.IEdmModel;
using IEdmSchemaElement = Microsoft.Data.Edm.IEdmSchemaElement;
using IEdmTypeReference = Microsoft.Data.Edm.IEdmTypeReference;

namespace Unchase.OData.ConnectedService.Common
{
    internal static class FunctionImportsExtensions
    {
        #region Extension methods

        #region For Data.Edm
        internal static string FullNameWithNamespace(this IEdmTypeReference type, string proxyClassNamespace)
        {
            var definition = type?.Definition as IEdmSchemaElement;
            if (definition == null && type?.IsCollection() == true)
                definition = type?.AsCollection()?.ElementType()?.Definition as IEdmSchemaElement;

            return definition?.FullNameWithNamespace(proxyClassNamespace);
        }

        internal static string FullNameWithNamespace(this IEdmSchemaElement element, string proxyClassNamespace)
        {
            if (element?.Namespace != null)
                return (string.IsNullOrWhiteSpace(proxyClassNamespace) ? element.Namespace ?? string.Empty : proxyClassNamespace) + "." + (element?.Name ?? string.Empty);
            return (element?.Namespace ?? string.Empty) + "." + (element?.Name ?? string.Empty);
        }

        internal static string ToCodeStringType(this IEdmTypeReference edmTypeReference, string proxyClassNamespace)
        {
            var result = string.Empty;
            switch (edmTypeReference)
            {
                case var pt when pt.IsPrimitive():
                    result = $"global::System.{pt.PrimitiveKind()}";
                    if (pt.IsNullable && !pt.IsString())
                        result = $"Nullable<{result}>";
                    break;
                case var pt when pt.IsCollection():
                    var elementType = ToCodeStringType(pt.AsCollection().ElementType(), proxyClassNamespace);
                    result = $"IEnumerable<{elementType}>";
                    break;
                case var pt when pt.IsComplex():
                    result = pt.FullNameWithNamespace(proxyClassNamespace);
                    break;
                case var pt when pt.IsEnum():
                    result = pt.FullNameWithNamespace(proxyClassNamespace);
                    break;
                case var pt when pt.IsEntity():
                    result = pt.FullNameWithNamespace(proxyClassNamespace);
                    break;
                case var pt when pt.IsEntityReference():
                    result = pt.FullNameWithNamespace(proxyClassNamespace);
                    break;
            }
            return result;
        }

        internal static IEdmModel GetDataEdmModel(this ServiceConfiguration serviceConfiguration, ConnectedServiceHandlerContext context = null)
        {
            using (var reader = serviceConfiguration.GetXmlReaderForEndpoint())
            {
                if (EdmxReader.TryParse(reader, out var model, out var parseErrors))
                    return model;
                else if (context != null)
                {
                    foreach (var error in parseErrors)
                    {
                        var task = context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, error.ErrorMessage);
                        task.RunSynchronously();
                    }
                }
            }

            return null;
        }
        #endregion

        #endregion
    }

    internal static class FunctionImportsHelper
    {
        #region Methods
        internal static List<FunctionImportModel> GetFunctionImports(IEdmModel model, string proxyClassNamespace, string endpointUri)
        {
            var result = new List<FunctionImportModel>();

            foreach (var modelEntityContainer in model.EntityContainers())
            {
                foreach (var functionImport in modelEntityContainer.FunctionImports())
                    result.Add(new FunctionImportModel(model, functionImport, endpointUri, proxyClassNamespace));
            }

            return result;
        }

        internal static string GetFunctionImportsCode(ServiceConfigurationV3 serviceConfiguration)
        {
            var generator = serviceConfiguration.OperationImportsGenerator;

            var functionMethods = new StringBuilder();
            functionMethods.AppendLine("\t\t#region FunctionImports");
            functionMethods.AppendLine();
            foreach (var functionImport in serviceConfiguration.FunctionImports.Where(fi => fi.IsChecked))
            {
                var functionRegion = string.Empty;
                switch (generator)
                {
                    case Constants.OperationImportsGenerator.Inner:
                        functionRegion = GetFunctionMethodRegionWithInnerMethods(functionImport);
                        break;
                    case Constants.OperationImportsGenerator.SimpleOData:
                        functionRegion = GetFunctionMethodRegionWithSimpleOdataClient(functionImport);
                        break;
                    case Constants.OperationImportsGenerator.Vipr:
                        //ToDo: need to add
                        functionRegion = string.Empty;
                        break;
                }
                functionMethods.AppendLine(functionRegion);
            }
            functionMethods.Append("\t\t#endregion");
            return functionMethods.ToString();
        }

        private static string GetFunctionMethodRegionWithInnerMethods(FunctionImportModel functionImportModel)
        {
            if (functionImportModel.FunctionImport.IsBindable)
                return string.Empty;

            var methodName = functionImportModel.EntitySetName + functionImportModel.FunctionImport.Name;
            var realFunctionImportReturnType = functionImportModel.FunctionReturnType?.ToCodeStringType(functionImportModel.Namespace);

            var functionRegion = new StringBuilder();
            functionRegion.AppendLine($"\t\t#region {methodName}Async");
            functionRegion.AppendLine();

            //// skip the first parameter for a bound function
            //if (functionImportModel.HasParameters && functionImportModel.FunctionImport.IsBindable)
            //    functionImportModel.FunctionParameters.Remove(functionImportModel.BindableParameter);

            // add model class if parameters count gt 0
            if (functionImportModel.HasParameters)
                functionRegion.AppendLine(GetModelRegion($"{methodName}Model", functionImportModel.Namespace, functionImportModel.FunctionParameters, functionImportModel.FunctionImport.IsBindable));
            functionRegion.AppendLine();

            if (functionImportModel.FunctionReturnType == null)
            {
                functionRegion.AppendLine($"\t\tpublic async Task<ODataStandartResponse> {methodName}Async({(functionImportModel.HasParameters ? $"{methodName}Model model" : string.Empty)})");
                functionRegion.AppendLine("\t\t{");
                functionRegion.AppendLine($"\t\t\treturn await this.CallOdataMethodWithVoidResultAsync(");
            }
            else
            {
                functionRegion.AppendLine($"\t\tpublic async Task<(ODataStandartResponse OdataResponse, {realFunctionImportReturnType} ODataResult)> {methodName}Async({(functionImportModel.HasParameters ? $"{methodName}Model model)" : ")")}");
                functionRegion.AppendLine("\t\t{");
                functionRegion.AppendLine($"\t\t\treturn await this.{(functionImportModel.FunctionReturnType.IsCollection() ? "CallOdataMethodWithMultipleResultAsync" : "CallOdataMethodWithSingleResultAsync")}<{(functionImportModel.FunctionReturnType.IsCollection() ? $"{realFunctionImportReturnType.Replace("IEnumerable<", string.Empty).Replace(">", string.Empty)}" : $"{realFunctionImportReturnType}")}, {methodName}Model>(");
            }
            functionRegion.AppendLine($"\t\t\t\t\"{functionImportModel.EntitySetName}\"");
            functionRegion.AppendLine($"\t\t\t\t, \"{functionImportModel.FunctionImport.Name}\"");
            if (functionImportModel.FunctionReturnType != null)
                functionRegion.AppendLine($"\t\t\t\t, HttpMethod.{functionImportModel.HttpMethod}");
            functionRegion.AppendLine(functionImportModel.HasParameters ? "\t\t\t\t, model);" : "\t\t\t\t);");
            functionRegion.AppendLine("\t\t}");
            functionRegion.AppendLine("\t\t#endregion");
            return functionRegion.ToString();
        }

        private static string GetFunctionMethodRegionWithSimpleOdataClient(FunctionImportModel functionImportModel)
        {
            var methodName = functionImportModel.EntitySetName + functionImportModel.FunctionImport.Name;
            var realFunctionImportReturnType = functionImportModel.FunctionReturnType?.ToCodeStringType(functionImportModel.Namespace);
            var realFunctionImportReturnElementCollectionType = string.Empty;
            if (functionImportModel.FunctionReturnType != null && functionImportModel.FunctionReturnType.IsCollection())
                realFunctionImportReturnElementCollectionType = functionImportModel.FunctionReturnType?.AsCollection()?.ElementType()?.ToCodeStringType(functionImportModel.Namespace);

            var functionRegion = new StringBuilder();
            functionRegion.AppendLine($"\t\t#region {methodName}Async");
            functionRegion.AppendLine();

            // add model class if parameters count gt 0
            if (functionImportModel.HasParameters)
                functionRegion.AppendLine(GetModelRegion($"{methodName}Model", functionImportModel.Namespace, functionImportModel.FunctionParameters, functionImportModel.FunctionImport.IsBindable));

            functionRegion.AppendLine(functionImportModel.FunctionReturnType == null
                ? $"\t\tpublic static async Task<ODataStandartResponse> {methodName}Async({(functionImportModel.HasParameters ? $"{methodName}Model model" : string.Empty)})"
                : $"\t\tpublic static async Task<(ODataStandartResponse ODataResponse, {realFunctionImportReturnType} ODataResult)> {methodName}Async({(functionImportModel.HasParameters ? $"{methodName}Model model" : string.Empty)})");
            functionRegion.AppendLine("\t\t{");
            functionRegion.AppendLine("\t\t\ttry");
            functionRegion.AppendLine("\t\t\t{");
            functionRegion.AppendLine("\t\t\t\tvar validationResult = model.ValidateModel();");
            functionRegion.AppendLine("\t\t\t\tif (validationResult.ErrorCode != \"OK\")");
            functionRegion.AppendLine(functionImportModel.FunctionReturnType == null
                ? "\t\t\t\t\treturn validationResult;"
                : $"\t\t\t\t\treturn (validationResult, default({realFunctionImportReturnType}));");
            functionRegion.AppendLine();
            functionRegion.AppendLine($"\t\t\t\tvar client = new ODataClient(@\"{functionImportModel.EndpointUri}\");");
            functionRegion.AppendLine(functionImportModel.FunctionReturnType == null
                ? "\t\t\t\tawait client"
                : "\t\t\t\tvar result = await client");
            if (functionImportModel.FunctionImport.IsBindable && functionImportModel.BindableParameter != null)
            {
                var bindingParameterTypeFulName = functionImportModel.BindableParameter.Type.FullNameWithNamespace(functionImportModel.Namespace);
                if (functionImportModel.BindableParameter.Type.IsCollection())
                    bindingParameterTypeFulName = functionImportModel.BindableParameter.Type.AsCollection().ElementType().FullNameWithNamespace(functionImportModel.Namespace);
                functionRegion.AppendLine($"\t\t\t\t\t.For<{bindingParameterTypeFulName?.Replace("IEnumerable<", string.Empty)?.Replace(">", string.Empty)}>().Key(model.{functionImportModel.BindableParameter.Name})");
                functionRegion.AppendLine($"\t\t\t\t\t.Function(\"{functionImportModel.FunctionImport.Name}\")");
            }
            else
            {
                functionRegion.AppendLine(functionImportModel.FunctionReturnType == null
                    ? "\t\t\t\t\t.Unbound()"
                    : $"\t\t\t\t\t.Unbound<{realFunctionImportReturnElementCollectionType}>()");
                functionRegion.AppendLine($"\t\t\t\t\t.Function(\"{functionImportModel.FunctionImport.Name}\")");
            }
            if (functionImportModel.HasParameters)
            {
                functionRegion.AppendLine("\t\t\t\t\t.Set(new");
                functionRegion.AppendLine("\t\t\t\t\t{");
                var first = true;
                foreach (var functionParameter in functionImportModel.FunctionParameters)
                {
                    if (!functionImportModel.FunctionImport.IsBindable ||
                        functionImportModel.BindableParameter == null ||
                        functionImportModel.BindableParameter.Name != functionParameter.Name)
                    {
                        functionRegion.AppendLine($"\t\t\t\t\t\t{(!first ? "," : string.Empty)}{functionParameter.Name} = model.{functionParameter.Name}");
                        first = false;
                    }
                }
                functionRegion.AppendLine("\t\t\t\t\t})");
            }
            if (functionImportModel.FunctionReturnType == null)
                functionRegion.AppendLine("\t\t\t\t\t.ExecuteAsync();");
            else
            {
                if (functionImportModel.FunctionReturnType.IsCollection())
                {
                    functionRegion.AppendLine(!functionImportModel.FunctionReturnType.IsPrimitive()
                        ? "\t\t\t\t\t.ExecuteAsEnumerableAsync();"
                        : $"\t\t\t\t\t.ExecuteAsScalarAsync<{realFunctionImportReturnElementCollectionType}>();");
                }
                else
                {
                    functionRegion.AppendLine(!functionImportModel.FunctionReturnType.IsPrimitive()
                        ? "\t\t\t\t\t.ExecuteAsSingleAsync();"
                        : $"\t\t\t\t\t.ExecuteAsScalarAsync<{realFunctionImportReturnType}>();");
                }
            }
            functionRegion.AppendLine(functionImportModel.FunctionReturnType == null
                ? "\t\t\t\treturn new ODataStandartResponse { ErrorCode = \"OK\" };"
                : "\t\t\t\treturn (new ODataStandartResponse { ErrorCode = \"OK\" }, result);");
            functionRegion.AppendLine("\t\t\t}");
            functionRegion.AppendLine("\t\t\tcatch (Simple.OData.Client.WebRequestException ex)");
            functionRegion.AppendLine("\t\t\t{");
            functionRegion.AppendLine(functionImportModel.FunctionReturnType == null
                ? "\t\t\t\treturn new ODataStandartResponse { ErrorCode = ex.Code.ToString(), ErrorMessage = ex.Message };"
                : "\t\t\t\treturn (new ODataStandartResponse { ErrorCode = ex.Code.ToString(), ErrorMessage = ex.Message }, " + $"default({realFunctionImportReturnType}));");
            functionRegion.AppendLine("\t\t\t}");
            functionRegion.AppendLine("\t\t\tcatch (Exception ex)");
            functionRegion.AppendLine("\t\t\t{");
            functionRegion.AppendLine(functionImportModel.FunctionReturnType == null
                ? "\t\t\t\treturn new ODataStandartResponse { ErrorCode = \"ApplicationException\", ErrorMessage = ex.Message };"
                : "\t\t\t\treturn (new ODataStandartResponse { ErrorCode = \"ApplicationException\", ErrorMessage = ex.Message }, " + $"default({realFunctionImportReturnType}));");
            functionRegion.AppendLine("\t\t\t}");
            functionRegion.AppendLine("\t\t}");
            functionRegion.AppendLine("\t\t#endregion");
            return functionRegion.ToString();
        }

        private static string GetModelRegion(string modelClassName, string proxyClassNamespace, List<IEdmFunctionParameter> functionParameters, bool isBindable = false)
        {
            // if has no parameters returns empty string
            if (functionParameters.Count == 0)
                return string.Empty;

            var regionModel = new StringBuilder();
            regionModel.AppendLine("\t\t#region Model");
            regionModel.AppendLine($"\t\tpublic class {modelClassName}");
            regionModel.AppendLine("\t\t{");
            var current = 0;
            foreach (var functionImportParameter in functionParameters)
            {
                string functionImportParameterType;
                if (isBindable && current == 0 && functionImportParameter.Type.IsCollection())
                    functionImportParameterType = functionImportParameter.Type.AsCollection().ElementType().ToCodeStringType(proxyClassNamespace);
                else
                    functionImportParameterType = functionImportParameter.Type.ToCodeStringType(proxyClassNamespace);

                regionModel.Append($"\t\t\tpublic {functionImportParameterType} {functionImportParameter.Name}");
                regionModel.AppendLine(" { get; set; }");
                if (++current != functionParameters.Count)
                    regionModel.AppendLine();
            }
            regionModel.AppendLine("\t\t}");
            regionModel.AppendLine("\t\t#endregion");
            return regionModel.ToString();
        }
        #endregion
    }
}
