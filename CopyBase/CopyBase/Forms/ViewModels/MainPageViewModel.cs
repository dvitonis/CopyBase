using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CopyBase.Forms.Models;
using System.Collections.ObjectModel;
using CopyBase.DataTier;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Forms;
using System.ComponentModel;

namespace CopyBase.Forms.ViewModels
{
    public class MainPageViewModel:BaseViewModel
    {
        CopyItem selectedCopyItem;
        ObservableCollection<CopyItem> copyItems;

        public ObservableCollection<CopyItem> CopyItems
        {
            get
            {
                return copyItems;
            }
            set
            {
                if (copyItems != value)
                {
                    //var oldsize=0;
                    //if (copyItems != null)
                    //    oldsize = copyItems.Count;

                    copyItems = value;

                    //if (oldsize > copyItems.Count)
                    //{
                    //    SelectedCopyItem = CopyItems.Last();
                    //}

                    NotifyPropertyChanged("CopyItems");
                }
            }
        }

        public CopyItem SelectedCopyItem
        {
            get
            {
                return selectedCopyItem;
            }
            set
            {
                if (selectedCopyItem != value)
                {
                    selectedCopyItem = value;
                    ClipboardMonitor.AddToClipboard(selectedCopyItem);
                    NotifyPropertyChanged("SelectedCopyItem");
                }
            }
        }

        public MainPageViewModel()
        {
            if (CopyItems==null)
            {
                CopyItems = new ObservableCollection<CopyItem>(); 
            }

            ClipboardMonitor.OnClipboardChange += new ClipboardMonitor.OnClipboardChangeEventHandler(ClipboardMonitor_OnClipboardChange);
        }

        void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                (Action)delegate()
                {
                    CopyItem it;
                    if ((format == ClipboardFormat.Text || 
                        format == ClipboardFormat.UnicodeText || 
                        format == ClipboardFormat.Html) &&
                        data.ToString() !="")
                    {
                        it = new CopyItem(data.ToString());
                    }
                    else return;

                    AddToList(it);
                });
        }

        private void AddToList(CopyItem it)
        {
            try
            {
                if (CopyItems.Count > 105)
                {
                    CopyItems.RemoveAt(0);
                }
                
                CopyItem existing = copyItems.FirstOrDefault(c => c.Data == it.Data);
                if (existing != null)
                {
                    SelectedCopyItem = existing;
                }
                else CopyItems.Add(it);
            }
            catch (Exception)
            {
                CopyItems = new ObservableCollection<CopyItem>();
                    CopyItems.Add(it);
            }
            finally
            {
                SelectedCopyItem = it;
            }
        }

        internal void Selection_Clicked()
        {
            try
            {
                SendCtrlV();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException.ToString();
            }
        }

        internal void DeleteItem(object item, EventArgs e)
        {
            try
            {
                var theitem = item as CopyItem;
                if (theitem != null)
                {
                    CopyItems.Remove(theitem);
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.InnerException.ToString();
            }
        }

        #region Window Actions

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        //int WM_PASTE = 0x302;
        uint WM_PASTE = 0x302;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetGUIThreadInfo(uint hTreadID, ref GuiThreadInfo lpgui);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(uint hwnd, out uint lpdwProcessId);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int iLeft;
            public int iTop;
            public int iRight;
            public int iBottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GuiThreadInfo
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        private void SendCtrlV()
        {
            IntPtr hWnd = GetFocusedHandle();
            PostMessage(hWnd, WM_PASTE, IntPtr.Zero, IntPtr.Zero);
        }

        static IntPtr GetFocusedHandle()
        {
            var info = new GuiThreadInfo();
            info.cbSize = Marshal.SizeOf(info);
            if (!GetGUIThreadInfo(0, ref info))
                throw new Win32Exception();
            //System.Windows.Forms.MessageBox.Show("active " + info.hwndActive + " focus " + info.hwndFocus);
            return info.hwndFocus;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        private static IntPtr previousHandle = IntPtr.Zero; 

        #endregion

    }

    //public static class RestoreWindowNoActivateExtension
    //{
    //    [DllImport("user32.dll")]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    static extern bool ShowWindow(IntPtr hWnd, UInt32 nCmdShow);

    //    private const int SW_SHOWNOACTIVATE = 4;

    //    public static void RestoreNoActivate(this Window win)
    //    {
    //        WindowInteropHelper winHelper = new WindowInteropHelper(win);
    //        ShowWindow(winHelper.Handle, SW_SHOWNOACTIVATE);
    //    }
    //}
}
