﻿using CopyBase.Forms.ViewModels;
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

            ListBoxCopyItems.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(ListBoxCopyItems_MouseDown), true);
        }

        private void ListBoxCopyItems_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.Selection_Clicked();
        }

    }
}