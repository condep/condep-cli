﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ConDep.Dsl.Core;

namespace ConDep.Console
{
    internal class ConfigurationAssemblyHandler
    {
        private readonly string _assemblyName;

        public ConfigurationAssemblyHandler(string assemblyName)
        {
            _assemblyName = assemblyName;
        }

        public Assembly GetConfigAssembly()
        {
            Assembly assembly;
            if (!TryGetAssembly(_assemblyName, out assembly))
            {
                throw new AssemblyNotFoundException(string.Format("Assembly [{0}] could not be resolved as a file path, in current directory or in executing directory.", _assemblyName));
            }
            return assembly;
        }

        private static bool TryGetAssembly(string assemblyName, out Assembly assembly)
        {
            string path;

            if (TryGetAbsolutePath(assemblyName, out path))
            {
                assembly = Assembly.LoadFile(path);
                return true;
            }

            var currentPath = Directory.GetCurrentDirectory();
            var combinedPath = Path.Combine(currentPath, assemblyName);
            if (File.Exists(combinedPath))
            {
                assembly = Assembly.LoadFile(path);
                return true;
            }

            var executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(executingPath))
            {
                assembly = null;
                return false;
            }

            combinedPath = Path.Combine(executingPath, assemblyName);
            if (File.Exists(combinedPath))
            {
                assembly = Assembly.LoadFile(path);
                return true;
            }

            assembly = null;
            return false;
        }

        private static bool TryGetAbsolutePath(string assemblyName, out string absolutePath)
        {
            if (File.Exists(assemblyName))
            {
                absolutePath = assemblyName;
                return true;
            }
            var absPath = Path.GetFullPath(assemblyName);
            if (File.Exists(absPath))
            {
                absolutePath = absPath;
                return true;
            }
            absolutePath = string.Empty;
            return false;
        }
    }
}