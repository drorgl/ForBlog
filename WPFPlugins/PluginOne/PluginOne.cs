using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterfaces;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace PluginOne
{
    /// <summary>
    /// Plugin One - Demo
    /// <para>
    ///     This is a demo for a simple plugin, it only contains a textbox
    /// </para>
    /// </summary>
    [Export(typeof(IWPFApplicationPlugin))]
    public class PluginOne : IWPFApplicationPlugin
    {
        /// <summary>
        /// User Control
        /// </summary>
        private PluginOneUC m_control = new PluginOneUC();

        #region IWPFApplicationPlugin Members

        public UserControl GetUserControl()
        {
            return m_control;
        }

        public string GetName()
        {
            return "PluginOne";
        }

        public string GetData()
        {
            return m_control.textboxData.Text;
        }

        public void SetData(string data)
        {
            m_control.textboxData.Text = data;
        }

        public void Reset()
        {
            m_control.textboxData.Text = string.Empty;
        }

        #endregion
    }
}
