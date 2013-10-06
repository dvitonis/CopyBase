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

using CopyBase.Forms.Views;
using CopyBase.DataTier;
using System.ComponentModel;
using CopyBase.Forms.ViewModels;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;

namespace CopyBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        private HotKey hotKey;

        public MainWindow()
        {
            InitializeComponent();
            InitializeHotKey();

            CalculatePossition();

            FrameMain.NavigationService.Navigate(new MainPage());

            this.Loaded += MainWindow_Loaded;
            this.Closing += new CancelEventHandler(MainWindow_Closing);

            ni.Icon = new System.Drawing.Icon("Main.ico");
            ni.Visible = true;

            ni.Click +=
                delegate(object sender, EventArgs args)
                {
                    CallWindow();
                };

            this.WindowControl.ExitAnimationCompleted+=WindowControl_ExitAnimationCompleted;
        }

        #region HotKey
        private void InitializeHotKey()
        {
            hotKey = new HotKey(Key.C, KeyModifier.Ctrl | KeyModifier.Shift, OnHotKeyHandler);
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            CallWindow();
        }
        #endregion

        private void CallWindow()
        {
            CalculatePossition();

            if (!this.WindowControl.ReverseTransition)
            {
                this.WindowControl.ReverseTransition = true;
                this.WindowControl.Reload();
            }
            else
            {
                this.WindowControl.ReverseTransition = false;
                this.Show();
                this.WindowState = WindowState.Normal;
            }
        }

        private void WindowControl_ExitAnimationCompleted(object sender, EventArgs e)
        {
            if (this.WindowControl.ReverseTransition)
            {
                this.Hide();
                this.WindowState = WindowState.Minimized;
            }
        }

        private void CalculatePossition()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var toprightcorner = transform.Transform(new Point(workingArea.Right, workingArea.Top));
                var bottomleftcorner = transform.Transform(new Point(workingArea.Left, workingArea.Bottom));

                this.Left = toprightcorner.X - this.ActualWidth + 1;
                this.Top = toprightcorner.Y - 1;
                this.Height = bottomleftcorner.Y - toprightcorner.Y + 2;
            }));
        } 

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ClipboardMonitor.Start();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= MainWindow_Closing;
            ClipboardMonitor.Stop(); // do not forget to stop
            ni.Visible = false;
            hotKey.Unregister();
            hotKey.Dispose();
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            //if (this.IsActive)
            //{
                //Set the window style to noactivate.
                WindowInteropHelper helper = new WindowInteropHelper(this);
                SetWindowLong(helper.Handle, GWL_EXSTYLE,
                    GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
            //}
        }

        //private void Window_Activated(object sender, EventArgs e)
        //{
        //    WindowWatcher.setLastActive();
        //    this.Info.Text = WindowWatcher.LastHandle.ToString();
        //    //System.Windows.Forms.MessageBox.Show(WindowWatcher.LastHandle.ToString());
        //}

        //protected override void OnStateChanged(EventArgs e)
        //{
        //    if (WindowState == WindowState.Minimized)
        //        this.Hide();

        //    base.OnStateChanged(e);
        //}

    }
}
