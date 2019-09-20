// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Models;

namespace Unchase.OData.ConnectedService.Common
{
    #region For update from Microsoft OData Connected Service
    internal class ConnectedServiceJsonFileData
    {
        public string ProviderId { get; set; }

        public UserSettings ExtendedData { get; set; }
    }
    #endregion

    internal static class ExtensionsHelper
    {
        #region For Views
        internal static void ChangeStackPanelVisibility(this StackPanel stackPanel)
        {
            if (stackPanel.Visibility == Visibility.Collapsed)
                stackPanel.Visibility = Visibility.Visible;
            else if (stackPanel.Visibility == Visibility.Visible)
                stackPanel.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region For OData.Edm
        internal static IEnumerable<IEdmEntityContainer> EntityContainers(this IEdmModel model)
        {
            return model?.SchemaElements?.OfType<IEdmEntityContainer>();
        }

        internal static IEnumerable<IEdmActionImport> ActionImports(this IEdmEntityContainer container)
        {
            return container?.Elements?.OfType<IEdmActionImport>();
        }

        internal static IEnumerable<IEdmFunctionImport> FunctionImports(this IEdmEntityContainer container)
        {
            return container?.Elements?.OfType<IEdmFunctionImport>();
        }

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

        internal static IEdmModel GetODataEdmModel(this ServiceConfigurationV4 serviceConfiguration, ConnectedServiceHandlerContext context = null)
        {
            using (var reader = serviceConfiguration.GetXmlReaderForEndpoint())
            {
                var edmxReaderSettings = new CsdlReaderSettings
                {
                    IgnoreUnexpectedAttributesAndElements = serviceConfiguration.IgnoreUnexpectedElementsAndAttributes
                };
                if (!CsdlReader.TryParse(reader, Enumerable.Empty<IEdmModel>(), edmxReaderSettings, out var model, out var parseErrors))
                {
                    if (context != null)
                    {
                        foreach (var error in parseErrors)
                        {
                            var task = context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning,
                                error.ErrorMessage);
                            task.RunSynchronously();
                        }
                    }
                }
                else
                    return model;
            }

            return null;
        }

        public static string GetHttpMethod(this IEdmModel model, IEdmElement annotatable)
        {
            if (!model.TryGetODataAnnotation(annotatable, "HttpMethod", out var str))
                return (string)null;
            if (str == null)
                throw new ODataException("NullValueForHttpMethodAnnotation");
            return str;
        }

        internal static bool TryGetODataAnnotation(this IEdmModel model, IEdmElement annotatable, string localName, out string value)
        {
            var annotationValue = model.GetAnnotationValue(annotatable, "http://docs.oasis-open.org/odata/ns/edmx", localName);
            if (annotationValue == null)
            {
                value = (string)null;
                return false;
            }

            if (!(annotationValue is IEdmStringValue edmStringValue))
                throw new Microsoft.Data.OData.ODataException("InvalidAnnotationValue");
            value = edmStringValue.Value;
            return true;
        }
        #endregion

        #region Common
        internal static XmlReader GetXmlReaderForEndpoint(this ServiceConfiguration serviceConfiguration)
        {
            var xmlUrlResolver = new XmlUrlResolver
            {
                Credentials = serviceConfiguration.UseNetworkCredentials ? new NetworkCredential(serviceConfiguration.NetworkCredentialsUserName, serviceConfiguration.NetworkCredentialsPassword, serviceConfiguration.NetworkCredentialsDomain) : CredentialCache.DefaultNetworkCredentials
            };
            if (serviceConfiguration.UseWebProxy)
            {
                xmlUrlResolver.Proxy = new WebProxy(serviceConfiguration.WebProxyUri, true);
                if (serviceConfiguration.UseWebProxyCredentials)
                    xmlUrlResolver.Proxy = new WebProxy(serviceConfiguration.WebProxyUri, true, new string[0], new NetworkCredential
                    {
                        UserName = serviceConfiguration.WebProxyNetworkCredentialsUserName,
                        Password = serviceConfiguration.WebProxyNetworkCredentialsPassword,
                        Domain = serviceConfiguration.WebProxyNetworkCredentialsDomain
                    });
                else
                    xmlUrlResolver.Proxy = new WebProxy(serviceConfiguration.WebProxyUri);
            }

            var settings = new XmlReaderSettings
            {
                XmlResolver = new XmlSecureResolver(xmlUrlResolver, new PermissionSet(System.Security.Permissions.PermissionState.Unrestricted))
            };

            return XmlReader.Create(serviceConfiguration.Endpoint, settings);
        }
        #endregion
    }
}
