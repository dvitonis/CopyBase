using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace CopyBase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"Errorlog.txt", true))
            {
                file.WriteLine(DateTime.Now + "\t" + e.ToString());
            }

            if (args.IsTerminating)
                System.Windows.Forms.MessageBox.Show("Runtime terminating \n\n" + e.Message);
        }
    }
}
