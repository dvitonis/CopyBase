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
    /// <summary>
    /// Main page view model.
    /// </summary>
    public class MainPageViewModel:BaseViewModel
    {
        CopyItem selectedCopyItem;
        static ObservableCollection<CopyItem> copyItems;

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
                    NotifyPropertyChanged("SelectedCopyItem");
                }
            }
        }

        /// <summary>
        /// Initialize model, CopyItems as an empty list and a clipboard change handler.
        /// </summary>
        public MainPageViewModel()
        {
            if (CopyItems==null)
            {
                CopyItems = new ObservableCollection<CopyItem>(); 
            }

            ClipboardMonitor.OnClipboardChange += new ClipboardMonitor.OnClipboardChangeEventHandler(ClipboardMonitor_OnClipboardChange);
        }

        /// <summary>
        /// Handle clipboard changes.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="formats"></param>
        void ClipboardMonitor_OnClipboardChange(System.Windows.Forms.DataObject data, string[] formats)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                (Action)delegate()
                {
                    CopyItem it;
                    it = new CopyItem(data, formats);
                    AddToList(it);
                });
        }

        /// <summary>
        /// Add an item from clipboard to the list.
        /// </summary>
        /// <param name="it"></param>
        private void AddToList(CopyItem it)
        {
            try
            {
                if (!it.IsCopyBaseItem())
                {
                    if (CopyItems == null)
                        CopyItems = new ObservableCollection<CopyItem>();
                    CopyItems.Add(it);
                    SelectedCopyItem = it;
                }
            }
            catch (Exception)
            {
                CopyItems = new ObservableCollection<CopyItem>();
                CopyItems.Add(it);
                SelectedCopyItem = it;
            }
            finally
            {
                
            }
        }

        /// <summary>
        /// Handle click on a list item by sending the selected item to the clipboard.
        /// </summary>
        internal void Selection_Clicked()
        {
            try
            {
                ClipboardMonitor.AddToClipboard(SelectedCopyItem);
                //SendCtrlV();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
        }

        /// <summary>
        /// Delete item from the list.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Empty the item list.
        /// </summary>
        internal void ListClear()
        {
            CopyItems.Clear();
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
