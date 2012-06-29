using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel.Composition.Hosting;
using DemoInterfaces;
using System.ComponentModel.Composition;

namespace DemoProgram
{
    /// <summary>
    /// Infrastructure demo getting specific exports by type
    /// </summary>
    public class PluginInfrastructre
    {
        private CompositionContainer _container = null;

        public IEnumerable<IPlugin> Plugins;

        public void Initialize()
        {
            var catalog = new AggregateCatalog();

            //Adds the program's assembly
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(PluginInfrastructre).Assembly));


            string programassembly = System.Reflection.Assembly.GetAssembly(typeof(PluginInfrastructre)).Location;

            string programpath = Path.GetDirectoryName(programassembly);

            //add the program's path
            catalog.Catalogs.Add(new DirectoryCatalog(programpath));

            _container = new CompositionContainer(catalog);

            try
            {
                //Initialize types found and assign new instances to Plugins
                Plugins = _container.GetExportedValues<IPlugin>();
                
            }
            catch (CompositionException compositionException)
            {
                throw;
            }
        }
    }
}
