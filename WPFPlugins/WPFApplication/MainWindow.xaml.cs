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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using PluginInterfaces;
using WPFApplication.Models;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Plugin Model List
        /// </summary>
        private List<PluginModel> m_data = null;

        /// <summary>
        /// Selected Plugin for "Add" button
        /// </summary>
        private IWPFApplicationPlugin m_addplugin;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add new model of type m_addplugin
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            if (sw.ShowDialog(this.m_addplugin) == true)
            {
                this.m_data.Add(new PluginModel
                {
                    Plugin = sw.GetName(),
                    Data = sw.GetData(),
                    Type = this.m_addplugin.GetType()
                });

                this.dataGrid.Items.Refresh();
                this.dataGrid.UpdateLayout();

                PluginStorage.SaveModels(this.m_data);
            }
            
        }

        /// <summary>
        /// Application Initialization
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //load models from file, if exists
            this.m_data = PluginStorage.GetModels();

            //binding for datagrid
            this.dataGrid.ItemsSource = this.m_data;
            ((DataGridTextColumn)this.dataGrid.Columns[0]).Binding = new Binding("Plugin");
            ((DataGridTextColumn)this.dataGrid.Columns[1]).Binding = new Binding("Data");

            //get all plugins
            var plugins = Plugins.ApplicationPlugins;

            //populate "add" menu items with plugins
            btnAddDropDownContainer.Children.Clear();

            foreach (var plugin in plugins)
            {
                var newmi = new MenuItem
                {
                    Header = plugin.GetName(),
                    DataContext = plugin
                    
                };
                newmi.Click += this.MenuItem_Click;

                btnAddDropDownContainer.Children.Add(newmi);
            }

            if (btnAddDropDownContainer.Children.Count == 0)
            {
                btnAdd.Content = "No Plugins";
                btnAdd.IsEnabled = false;
            }
            else
            {
                var mi = ((MenuItem)btnAddDropDownContainer.Children[0]);
                SetPluginFromMenuItem(mi);
            }
        }

        /// <summary>
        /// SplitButton menu selection click
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetPluginFromMenuItem((MenuItem)sender);
            btnAdd.IsOpen = false;
        }

        //Sets button's text and m_addplugin for the selected plugin
        private void SetPluginFromMenuItem(MenuItem sender)
        {
            var mi = sender;
            btnAdd.Content = "Add " + mi.Header;
            m_addplugin = (IWPFApplicationPlugin)mi.DataContext;
        }

        /// <summary>
        /// DataGrid's Edit button click handler
        /// </summary>
        private void datagridEdit_Click(object sender, RoutedEventArgs e)
        {
            PluginModel pm = ((FrameworkElement)sender).DataContext as PluginModel;
            if (pm == null)
                throw new ApplicationException();

            var plugin = (IWPFApplicationPlugin)Activator.CreateInstance(pm.Type);

            SettingsWindow sw = new SettingsWindow();
            if (sw.ShowDialog(plugin, pm.Data) == true)
            {
                pm.Data = sw.GetData();

                this.dataGrid.Items.Refresh();
                this.dataGrid.UpdateLayout();
            }
        }

        /// <summary>
        /// DataGrid's Delete button click
        /// </summary>
        private void datagridDelete_Click(object sender, RoutedEventArgs e)
        {
            PluginModel pm = ((FrameworkElement)sender).DataContext as PluginModel;
            if (pm == null)
                throw new ApplicationException();

            this.m_data.Remove(pm);

            this.dataGrid.Items.Refresh();
            this.dataGrid.UpdateLayout();

            PluginStorage.SaveModels(this.m_data);

        }

    }
}
