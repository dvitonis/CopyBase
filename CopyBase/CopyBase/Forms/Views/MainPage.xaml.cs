using CopyBase.Forms.ViewModels;
using CopyBase.Forms.Models;
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
using CopyBase.DataTier;
using UIControls;

namespace CopyBase.Forms.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private MainPageViewModel ViewModel;

        public MainPage()
        {
            InitializeComponent();
            ViewModel = (MainPageViewModel)this.GridMain.DataContext;

            StaticUICommands.OnDelete += ViewModel.DeleteItem;
            ListBoxCopyItems.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(ListBoxCopyItems_MouseDown), true);
        }

        /// <summary>
        /// Handle mouse click on list items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxCopyItems_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.Selection_Clicked();
        }

        /// <summary>
        /// Handle clear button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListClearButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ListClear();
        }

        /// <summary>
        /// Handle app closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);

            while (parent != null && !(parent is Window))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var parentWindow = parent as Window;

            parentWindow.Close();
        }

    }
}
