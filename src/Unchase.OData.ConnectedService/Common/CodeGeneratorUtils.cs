// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace Unchase.OData.ConnectedService.Common
{
    internal static class CodeGeneratorUtils
    {
        private static readonly List<string> DataFxRegistryPaths;
        private static readonly List<string> DataFxLocalPaths;

        public const string InstallLocationSubKeyName = "InstallLocation";

        static CodeGeneratorUtils()
        {
            DataFxRegistryPaths = new List<string>();
            DataFxLocalPaths = new List<string>();
            if (8 == IntPtr.Size)
            {
                DataFxRegistryPaths.Add(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft WCF Data Services\VS 2014 Tooling\");
                DataFxRegistryPaths.Add(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft WCF Data Services\VS 2010 Tooling\");
                DataFxRegistryPaths.Add(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft WCF Data Services\5.6\");

                var programFilesX86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                DataFxLocalPaths.Add(Path.Combine(programFilesX86Path, @"Microsoft WCF Data Services\VS 2014 Tooling\"));
                DataFxLocalPaths.Add(Path.Combine(programFilesX86Path, @"Microsoft WCF Data Services\VS 2010 Tooling\"));
                DataFxLocalPaths.Add(Path.Combine(programFilesX86Path, @"Microsoft WCF Data Services\5.6\"));

                var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                DataFxLocalPaths.Add(Path.Combine(programFilesPath, @"Microsoft WCF Data Services\VS 2014 Tooling\"));
                DataFxLocalPaths.Add(Path.Combine(programFilesPath, @"Microsoft WCF Data Services\VS 2010 Tooling\"));
                DataFxLocalPaths.Add(Path.Combine(programFilesPath, @"Microsoft WCF Data Services\5.6\"));
            }
            else
            {
                DataFxRegistryPaths.Add(@"SOFTWARE\Microsoft\Microsoft WCF Data Services\VS 2014 Tooling\");
                DataFxRegistryPaths.Add(@"SOFTWARE\Microsoft\Microsoft WCF Data Services\VS 2010 Tooling\");
                DataFxRegistryPaths.Add(@"SOFTWARE\Microsoft\Microsoft WCF Data Services\5.6\");

                var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                DataFxLocalPaths.Add(Path.Combine(programFilesPath, @"Microsoft WCF Data Services\VS 2014 Tooling\"));
                DataFxLocalPaths.Add(Path.Combine(programFilesPath, @"Microsoft WCF Data Services\VS 2010 Tooling\"));
                DataFxLocalPaths.Add(Path.Combine(programFilesPath, @"Microsoft WCF Data Services\5.6\"));
            }
        }

        /// <summary>
        /// Try to get the location of the installed WCF Data Service.
        /// </summary>
        /// <returns>Returns the location of the installed WCF Data Service if it exists, else returns empty string.</returns>
        public static string GetWCFDSInstallLocation()
        {
            try
            {
                foreach (var dataFxRegistryPath in DataFxRegistryPaths)
                {
                    try
                    {
                        using (var dataFxKey = Registry.LocalMachine.OpenSubKey(dataFxRegistryPath))
                        {
                            if (dataFxKey != null)
                            {
                                var runtimePath = (string)dataFxKey.GetValue(InstallLocationSubKeyName);
                                if (!string.IsNullOrEmpty(runtimePath))
                                {
                                    return runtimePath;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                foreach (var dataFxLocalPath in DataFxLocalPaths)
                {
                    if (Directory.Exists(dataFxLocalPath))
                        return dataFxLocalPath;
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }
    }
}
