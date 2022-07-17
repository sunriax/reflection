using RaGae.BootstrapLib.Loader;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RaGae.ReflectionLib
{
    public class Reflection
    {
        private ReflectionConfig config;
        private IEnumerable<string> filePath;
        private List<Assembly> assemblies = new List<Assembly>();
        private List<Type> types = new List<Type>();

        public Reflection(string configFile, int section)
        {
            LoadConfig(configFile, section);

            if (!(string.IsNullOrWhiteSpace(this.config.ReflectionPath) || string.IsNullOrWhiteSpace(this.config.FileSpecifier)))
                SetFilePaths(this.config.ReflectionPath, this.config.FileSpecifier);
            else if (this.config.Files != null && this.config.Files.Count() > 0)
                SetFilePaths(this.config.Files);
            else
                throw new ReflectionException(ErrorCode.EMPTY_CONFIG, $"{configFile}:{section}");

            LoadAssemblies();
            SetTypes();
        }

        public Reflection(string libraryPath, string fileSpecifier)
        {
            SetFilePaths(libraryPath, fileSpecifier);
            LoadAssemblies();
            SetTypes();
        }

        public Reflection(IEnumerable<string> libraryFile)
        {
            SetFilePaths(libraryFile);
            LoadAssemblies();
            SetTypes();
        }

        private void LoadConfig(string configFile, int section)
        {
            try
            {
                this.config = Loader.LoadConfigSection<ReflectionConfig>(configFile, $"{nameof(ReflectionConfig)}:{section}");
            }
            catch
            {
                throw new ReflectionException(ErrorCode.MISSING_CONFIG, $"{configFile}:{section}");
            }
        }

        private void SetFilePaths(string libraryPath, string fileSpecifier)
        {
            if (!Directory.Exists(libraryPath))
                throw new ReflectionException(ErrorCode.DIRECTORY_NOT_FOUND, libraryPath);

            if(!string.IsNullOrWhiteSpace(fileSpecifier))
                filePath = Directory.GetFiles(libraryPath, fileSpecifier);

            if (string.IsNullOrWhiteSpace(fileSpecifier) || filePath.Count() == 0)
                throw new ReflectionException(ErrorCode.MISSING_FILES, $"{libraryPath}/{fileSpecifier}");
        }

        private void SetFilePaths(IEnumerable<string> libraryFile)
        {
            if (libraryFile == null || libraryFile.Count() == 0)
                throw new ReflectionException(ErrorCode.EMPTY_LIST);

            foreach (string path in libraryFile)
            {
                if(!File.Exists(path))
                    throw new ReflectionException(ErrorCode.ASSEMBLIES_NOT_FOUND, path);
            }

            filePath = libraryFile;
        }

        private void LoadAssemblies()
        {
            foreach (string path in filePath)
            {
                assemblies.Add(Assembly.LoadFrom(path));
            }
        }

        private void SetTypes()
        {
            foreach (Assembly assembly in assemblies)
            {
                types.Add(assembly.GetType(assembly.DefinedTypes.First().FullName, true, true));
            }
        }

        public T GetInstanceByProperty<T>(string propertyName, object propertyValue, object[] arguments = null)
        {
            if (propertyName == null)
                throw new ReflectionException(ErrorCode.INVALID_PROPERTY, propertyName);

            Type instanceType = null;

            foreach (Type type in types)
            {
                object instance;

                try
                {
                    // Every class that is loaded by the GetInstanceByProperty method must implement
                    // a (Public/Non-Public) Constructor that does not take any parameters and does
                    // not throw an exception on initialisation. That is necessary to search for the
                    // correct property value.
                    instance = Activator.CreateInstance(type, true);
                }
                catch (Exception)
                {
                    throw new ReflectionException(ErrorCode.INSTANCE_ERROR);
                }

                PropertyInfo instanceInfo = type.GetProperty(propertyName);
                object value = instanceInfo.GetValue(instance);

                if ((propertyValue != null) && (propertyValue.ToString() == value.ToString()))
                {
                    instanceType = type;
                    break;
                }
            }

            try
            {
                return (T)Activator.CreateInstance(instanceType, arguments);
            }
            catch
            {
                throw new ReflectionException(ErrorCode.INVALID_INSTANCE, $"{propertyName}:{propertyValue}");
            }
        }
    }
}
