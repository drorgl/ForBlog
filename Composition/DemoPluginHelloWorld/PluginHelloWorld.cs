using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemoInterfaces;
using System.ComponentModel.Composition;

namespace DemoPluginHelloWorld
{
    //Export tells the Composition Container which type the class is assigned to.
    [Export(typeof(IPlugin))]
    public class PluginHelloWorld : IPlugin
    {

        #region IPlugin Members

        public string Execute()
        {
            return "Hello World!";
        }

        #endregion

        public PluginHelloWorld()
        {
            //Console.WriteLine("PluginHelloWorld Initialized");
        }
    }
}
