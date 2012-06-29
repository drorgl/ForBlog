using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemoInterfaces;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace DemoProgram
{
    /// <summary>
    /// Infrastructure demo composing a class using attributes.
    /// </summary>
    public class PluginInfrastructureImports
    {
        private CompositionContainer _container = null;


        [ImportMany]
        public IEnumerable<IPlugin> Plugins { get; set; }

        public void Initialize()
        {
            var catalog = new AggregateCatalog();

            //Adds the program's assembly
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(PluginInfrastructureImports).Assembly));


            string programassembly = System.Reflection.Assembly.GetAssembly(typeof(PluginInfrastructureImports)).Location;

            string programpath = Path.GetDirectoryName(programassembly);

            //add the program's path
            catalog.Catalogs.Add(new DirectoryCatalog(programpath));

            _container = new CompositionContainer(catalog);

            try
            {
                //Initialize types found and assign new instances to Plugins
                _container.ComposeParts(this);

            }
            catch (CompositionException compositionException)
            {
                throw;
            }
        }
    }
}
