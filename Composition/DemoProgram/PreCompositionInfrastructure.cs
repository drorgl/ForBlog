using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemoInterfaces;
using System.IO;
using System.Reflection;

namespace DemoProgram
{
    /// <summary>
    /// Pre composition demo of implementing plugins
    /// </summary>
    public class PreCompositionInfrastructure
    {
        public IEnumerable<IPlugin> Plugins;


        private Assembly GetLoadedAssembly(string filename)
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            return asms.FirstOrDefault(i => i.Location == filename);
        }

        public void Initialize()
        {
            string programassembly = System.Reflection.Assembly.GetAssembly(typeof(PreCompositionInfrastructure)).Location;

            string programpath = Path.GetDirectoryName(programassembly);

            List<Type> plugintypes = new List<Type>();

            foreach (var filename in Directory.EnumerateFiles(programpath,"*.dll"))
            {
                //check assembly is loaded already, if not, load it.
                var assembly = GetLoadedAssembly(filename);
                if (assembly == null)
                    assembly = Assembly.LoadFile(filename);

                foreach (var importtype in assembly.GetTypes())
                {
                    if ((typeof(IPlugin).IsAssignableFrom(importtype)) && (!importtype.IsInterface))
                        plugintypes.Add(importtype);
                }
            }

            List<IPlugin> plugininitialized = new List<IPlugin>();
            foreach (var pt in plugintypes)
            {
                plugininitialized.Add((IPlugin)Activator.CreateInstance(pt));
            }

            this.Plugins = plugininitialized;
        }
    }
}
