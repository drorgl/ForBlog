using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterfaces;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows;

namespace PluginTwo
{
    /// <summary>
    /// Plugin Two - Demo
    /// <para>
    ///     This is a more complex plugin, it has two fields of different types, so it requires a bit
    ///     of processing in the GetData and SetData methods, The way its done is by saving and 
    ///     loading the data into XML.
    /// </para>
    /// </summary>
    [Export(typeof(IWPFApplicationPlugin))]
    public class PluginTwo : IWPFApplicationPlugin
    {
        private PluginTwoUC m_control = new PluginTwoUC();

        #region IWPFApplicationPlugin Members

        public UserControl GetUserControl()
        {
            return m_control;
        }

        public string GetName()
        {
            return "PluginTwo";
        }

        public string GetData()
        {
            PluginTwoModel model = new PluginTwoModel();
            model.textbox = this.m_control.textbox.Text;
            model.radiogroup = FindDescendants<RadioButton>(this.m_control, e => e.IsChecked == true).Select(i => i.Name).FirstOrDefault();
            return model.ToXml();
        }

        public void SetData(string data)
        {
            PluginTwoModel model = new PluginTwoModel(data);
            this.m_control.textbox.Text = model.textbox;
            foreach (var radio in FindDescendants<RadioButton>(this.m_control, i => true))
            {
                if (radio.Name == model.radiogroup)
                    radio.IsChecked = true;
                else
                    radio.IsChecked = false;
            }
        }

        public void Reset()
        {
            this.m_control.textbox.Text = string.Empty;
            foreach (var radio in FindDescendants<RadioButton>(this.m_control, i => true))
            {
                radio.IsChecked = false;
            }
        }

        #endregion

        /// <summary>
        /// Recursively finds all descendants with a predicate form the logical view.
        /// <remarks>
        /// Taken from: http://stackoverflow.com/questions/9396239/how-can-i-get-a-reference-to-a-group-of-radiobuttons-and-find-the-selected-one
        /// </remarks>
        /// </summary>
        public static IEnumerable<T> FindDescendants<T>(DependencyObject parent, Func<T, bool> predicate, bool deepSearch = false) where T : DependencyObject
        {
            var children = LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>().ToList();

            foreach (var child in children)
            {
                var typedChild = child as T;
                if ((typedChild != null) && (predicate == null || predicate.Invoke(typedChild)))
                {
                    yield return typedChild;
                    if (deepSearch) foreach (var foundDescendant in FindDescendants(child, predicate, true)) yield return foundDescendant;
                }
                else
                {
                    foreach (var foundDescendant in FindDescendants(child, predicate, deepSearch)) yield return foundDescendant;
                }
            }

            yield break;
        }
    }
}
