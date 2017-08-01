using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Nomad
{
    public class PluginLoader<T>
    {
        public static ICollection<T> LoadPlugins(string path)
        {
            String[] dlls = null;

            if (Directory.Exists(path))
            {
                dlls = Directory.GetFiles(path, "*.dll");

                var assemblies = new List<Assembly>();
                var pluginType = typeof(T);
                var pluginTypes = new List<Type>();
                var plugins = new List<T>();

                foreach (var dll in dlls)
                {
                    var assemblyName = AssemblyLoadContext.GetAssemblyName(dll);
                    var assembly = Assembly.Load(assemblyName);
                    assemblies.Add(assembly);
                }

                foreach (var assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();

                        foreach (var type in types)
                        {
                            if (type.GetTypeInfo().IsInterface || type.GetTypeInfo().IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (type.GetTypeInfo().GetInterface(pluginType.FullName) != null)
                                {
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                }

                foreach (var type in pluginTypes)
                {
                    var plugin = (T)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }

                return plugins;
            }

            return null;
        }
    }
}
