// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Unchase.OData.ConnectedService.Common;

namespace Unchase.OData.ConnectedService.Models
{
    internal class OperationImportModel : INotifyPropertyChanged
    {
        #region Properties and fields
        public IEdmOperationImport OperationImport { get; }

        internal string EndpointUri { get; }

        internal List<IEdmOperationParameter> OperationParameters { get; set; }

        internal IEdmOperationParameter BindableParameter { get; }

        internal bool HasParameters => OperationParameters.Count > 0;

        public string EntitySetName { get; }

        public string HttpMethod { get; }

        internal IEdmTypeReference OperationReturnType { get; }

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

        private bool _isCollectionReturnTypeChecked;
        public bool IsCollectionReturnTypeChecked
        {
            get => _isCollectionReturnTypeChecked;
            set
            {
                _isCollectionReturnTypeChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsCollectionReturnTypeChecked"));
            }
        }

        public string OperationImportName { get; set; }

        public string ParametersString
        {
            get
            {
                var parametersString = new StringBuilder();
                parametersString.Append("(");
                var first = true;
                foreach (var operationParameter in OperationParameters)
                {
                    if (!first)
                        parametersString.Append(", ");
                    parametersString.Append($"{operationParameter.Type.ToCodeStringType(Namespace)} {operationParameter.Name}");
                    if (first)
                        first = false;
                }
                parametersString.Append(")");
                return parametersString.ToString();
            }
        }

        public string OperationImportReturnTypeFullName { get; set; }

        public string Separator => "|";
        #endregion

        #region Constructors
        internal OperationImportModel(IEdmModel model, IEdmOperationImport operationImport, string endpointUri, string proxyClassNamespace)
        {
            OperationImport = operationImport;
            OperationImportName = operationImport.Name;
            EndpointUri = endpointUri;
            HttpMethod = model.GetHttpMethod(operationImport) ?? "UNKN";
            OperationParameters = operationImport.Operation.Parameters.ToList();
            BindableParameter = operationImport.Operation.IsBound ? OperationParameters.FirstOrDefault(fp => fp.Type.IsEntity() || fp.Type.IsCollection() && fp.Type.AsCollection().ElementType().IsEntity()) : null;
            EntitySetName = operationImport.Operation.IsBound && BindableParameter != null
                ? (BindableParameter.Type.IsCollection() ? BindableParameter.Type.AsCollection().ElementType().FullNameWithNamespace(proxyClassNamespace).Split('.').Last() : BindableParameter.Type.FullNameWithNamespace(proxyClassNamespace).Split('.').Last())
                : string.Empty;
            OperationReturnType = operationImport.Operation.ReturnType;
            OperationImportReturnTypeFullName = operationImport.Operation.ReturnType?.ToCodeStringType(proxyClassNamespace) ?? "void";
            IsCollectionReturnTypeChecked = operationImport.Operation.ReturnType?.IsCollection() ?? false;
            Namespace = proxyClassNamespace;
            IsChecked = true;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
