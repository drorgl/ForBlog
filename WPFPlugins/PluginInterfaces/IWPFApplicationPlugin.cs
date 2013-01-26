using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PluginInterfaces
{
    /// <summary>
    /// WPF Application Plugin Interface
    /// </summary>
    public interface IWPFApplicationPlugin
    {
        /// <summary>
        /// Gets User Control instance
        /// </summary>
        UserControl GetUserControl();

        /// <summary>
        /// Resets the control
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets the text name of the plugin
        /// </summary>
        string GetName();

        /// <summary>
        /// Gets data from the plugin
        /// </summary>
        string GetData();

        /// <summary>
        /// Updates control's UI based on data
        /// </summary>
        void SetData(string data);
    }
}
