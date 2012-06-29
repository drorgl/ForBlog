using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace DemoProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing Infrastructure with GetExportedValues");
            PluginInfrastructre pi = new PluginInfrastructre();
            pi.Initialize();

            Console.WriteLine("{0} plugins detected",pi.Plugins.Count());

            foreach (var p in pi.Plugins)
            {
                Console.WriteLine("Plugin type: {0} Executed and returned: {1}", p.GetType().Name,p.Execute());
            }


            Console.WriteLine();
            Console.WriteLine("Initializing Infrastructure with Imports");
            PluginInfrastructureImports piimports = new PluginInfrastructureImports();
            piimports.Initialize();

            Console.WriteLine("{0} plugins detected", piimports.Plugins.Count());

            foreach (var p in piimports.Plugins)
            {
                Console.WriteLine("Plugin type: {0} Executed and returned: {1}", p.GetType().Name, p.Execute());
            }



            Console.WriteLine();
            Console.WriteLine("Initializing Infrastructure Pre Composition");
            PreCompositionInfrastructure pipre = new PreCompositionInfrastructure();
            pipre.Initialize();

            Console.WriteLine("{0} plugins detected", pipre.Plugins.Count());

            foreach (var p in pipre.Plugins)
            {
                Console.WriteLine("Plugin type: {0} Executed and returned: {1}", p.GetType().Name, p.Execute());
            }

            Console.WriteLine();
            Console.WriteLine("Benchmarks:");

            int iterations = 1000;

            Stopwatch sw = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
            {
                PluginInfrastructre pibench = new PluginInfrastructre();
                pibench.Initialize();
                foreach (var p in pibench.Plugins)
                {
                    p.Execute();
                }
            }

            Console.WriteLine("PluginInfrastructre: {0}ms",sw.ElapsedMilliseconds);

            sw.Restart();

            for (var i = 0; i < iterations; i++)
            {
                PluginInfrastructureImports piimportsbench = new PluginInfrastructureImports();
                piimportsbench.Initialize();
                foreach (var p in piimportsbench.Plugins)
                {
                    p.Execute();
                }
            }

            Console.WriteLine("PluginInfrastructureImports: {0}ms", sw.ElapsedMilliseconds);

            sw.Restart();


            for (var i = 0; i < iterations; i++)
            {
                PreCompositionInfrastructure piprebench = new PreCompositionInfrastructure();
                piprebench.Initialize();
                foreach (var p in piprebench.Plugins)
                {
                    p.Execute();
                }
            }
            Console.WriteLine("PreCompositionInfrastructure: {0}ms", sw.ElapsedMilliseconds);

        }
    }
}
