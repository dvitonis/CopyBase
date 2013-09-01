using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UIControls;

namespace UIControls.Styles.Buttons
{
    public partial class CBButtons : ResourceDictionary
    {
        public CBButtons()
        {
            InitializeComponent();
        }

        public void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var item = button.DataContext;
                StaticUICommands.DeleteCommand(item);
            }
        }
    }
}
