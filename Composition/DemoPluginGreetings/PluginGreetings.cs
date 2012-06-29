using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using DemoInterfaces;

namespace DemoPluginGreetings
{
    //Export tells the Composition Container which type the class is assigned to.
    [Export(typeof(IPlugin))]
    public class PluginGreetings : IPlugin
    {
        #region IPlugin Members

        public string Execute()
        {
            return "Greetings!";
        }

        #endregion

        public PluginGreetings()
        {
            //Console.WriteLine("PluginGreetings Initialized");
        }
    }
}
