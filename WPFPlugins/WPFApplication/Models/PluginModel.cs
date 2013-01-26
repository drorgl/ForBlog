using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterfaces;

namespace WPFApplication.Models
{
    /// <summary>
    /// Plugin Model
    /// <para>
    /// Each record is stored in this Model.
    /// </para>
    /// </summary>
    [Serializable]
    internal class PluginModel
    {
        public Type Type { get; set; }
        public string Plugin { get; set; }
        public string Data { get; set; }
    }
}
