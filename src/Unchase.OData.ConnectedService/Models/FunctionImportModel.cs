// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Unchase.OData.ConnectedService.Common;

namespace Unchase.OData.ConnectedService.Models
{
    internal class FunctionImportModel : INotifyPropertyChanged
    {
        #region Properties and fields
        public IEdmFunctionImport FunctionImport { get; }

        internal string EndpointUri { get; }

        internal List<IEdmFunctionParameter> FunctionParameters { get; set; }

        internal IEdmFunctionParameter BindableParameter { get; }

        internal bool HasParameters => FunctionParameters.Count > 0;

        internal string EntitySetName { get; }

        public string HttpMethod { get; }

        internal IEdmTypeReference FunctionReturnType { get; }

        internal string Namespace { get; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }
        public string FunctionImportName { get; set; }

        public string ParametersString
        {
            get
            {
                var parametersString = new StringBuilder();
                parametersString.Append("(");
                var first = true;
                foreach (var functionParameter in FunctionParameters)
                {
                    if (!first)
                        parametersString.Append(", ");
                    parametersString.Append($"{functionParameter.Type.FullNameWithNamespace(Namespace)} {functionParameter.Name}");
                    if (first)
                        first = false;
                }
                parametersString.Append(")");
                return parametersString.ToString();
            }
        }

        public string FunctionImportReturnTypeFullName { get; set; }

        public string Separator => "|";

        #endregion

        #region Constructors
        internal FunctionImportModel(IEdmModel model, IEdmFunctionImport functionImport, string endpointUri, string proxyClassNamespace)
        {
            FunctionImport = functionImport;
            FunctionImportName = functionImport.Name;
            EndpointUri = endpointUri;
            HttpMethod = model.GetHttpMethod(functionImport) ?? "POST";
            FunctionParameters = functionImport.Parameters.ToList();
            BindableParameter = FunctionParameters.FirstOrDefault(fp => fp.Type.IsEntity() || fp.Type.IsCollection() && fp.Type.AsCollection().ElementType().IsEntity());
            EntitySetName = functionImport.IsBindable && BindableParameter != null
                ? (BindableParameter.Type.IsCollection() ? BindableParameter.Type.AsCollection().ElementType().FullNameWithNamespace(proxyClassNamespace).Split('.').Last() : BindableParameter.Type.FullNameWithNamespace(proxyClassNamespace).Split('.').Last())
                : string.Empty;
            FunctionReturnType = functionImport.ReturnType;
            FunctionImportReturnTypeFullName = FunctionReturnType.FullNameWithNamespace(proxyClassNamespace) ?? "void";
            Namespace = proxyClassNamespace;
            IsChecked = true;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
