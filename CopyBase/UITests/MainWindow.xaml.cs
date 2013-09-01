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
using UIControls;

namespace UITests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //foreach (ListBoxItem item in thelist.Items)
            //{
            //    DependencyObject d = GetTemplateChild("PART_ItemClearButton");
            //    if (d != null)
            //    {
            //        (d as Button).Click += new RoutedEventHandler(ItemClearButton_Click);
            //    }
            //}
            CopyBase.Forms.Models.CopyItem item = new CopyBase.Forms.Models.CopyItem("theitemName");
            thelist.Items.Add(item);
            StaticUICommands.OnDelete += theitem_Clear;

        }

        private void theitem_Clear(object item, EventArgs e)
        {
            try
            {
                thelist.Items.Remove(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
