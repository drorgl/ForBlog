using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using PluginInterfaces;
using System.IO;
using System.ComponentModel.Composition;

namespace WPFApplication
{
    /// <summary>
    /// Plugin loader
    /// </summary>
    internal class Plugins
    {
        /// <summary>
        /// Composition Container
        /// </summary>
        private static CompositionContainer _container = null;

        /// <summary>
        /// Available Plugins Collection
        /// </summary>
        public static IEnumerable<IWPFApplicationPlugin> ApplicationPlugins;

        /// <summary>
        /// Static Initialization for loading the plugins into the ApplicationPlugins Collection
        /// </summary>
        static Plugins()
        {
            var catalog = new AggregateCatalog();

            //Adds the program's assembly
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Plugins).Assembly));


            string programassembly = System.Reflection.Assembly.GetAssembly(typeof(Plugins)).Location;

            string programpath = Path.GetDirectoryName(programassembly);

            //add the program's path
            catalog.Catalogs.Add(new DirectoryCatalog(programpath));

            _container = new CompositionContainer(catalog);

            try
            {
                ApplicationPlugins = _container.GetExportedValues<IWPFApplicationPlugin>();
            }
            catch (CompositionException compositionException)
            {
                throw compositionException;
            }

        }
    }
}
