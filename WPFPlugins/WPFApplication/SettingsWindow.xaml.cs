using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PluginInterfaces;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, IWPFApplicationPlugin
    {
        private IWPFApplicationPlugin m_plugin;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public bool? ShowDialog(IWPFApplicationPlugin plugin)
        {
            return ShowDialog(plugin, null);
        }

        public bool? ShowDialog(IWPFApplicationPlugin plugin, string data)
        {
            this.m_plugin = plugin;
            var uc = this.m_plugin.GetUserControl();
            this.m_plugin.Reset();
            if (data != null)
                this.m_plugin.SetData(data);
            this.UCContainer.Children.Add(uc);
            this.UCContainer.MinHeight = uc.MinHeight;
            this.UCContainer.MinWidth = uc.MinWidth;

           

            return this.ShowDialog();
        }

        #region IWPFApplicationPlugin Members

        public UserControl GetUserControl()
        {
           return this.m_plugin.GetUserControl();
        }

        public string GetName()
        {
            return this.m_plugin.GetName();
        }

        public string GetData()
        {
            return this.m_plugin.GetData();
        }

        public void SetData(string data)
        {
            this.m_plugin.SetData(data);
        }

        public void Reset()
        {
            this.m_plugin.Reset();
        }

        #endregion

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.UCContainer.Children.Clear();
        }

        /// <summary>
        /// Sets Minimum Window Size according to content
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Size margin = new Size();
            FrameworkElement contentElement = this.Content as FrameworkElement;
            if (contentElement != null)
            {
                margin.Width = this.Width - contentElement.ActualWidth;
                margin.Height = this.Height - contentElement.ActualHeight;
            }

            Rect size = VisualTreeHelper.GetDescendantBounds(this);

            this.MinHeight = size.Height + margin.Height;
            this.MinWidth = size.Width + margin.Width ;
        }
    }
}
