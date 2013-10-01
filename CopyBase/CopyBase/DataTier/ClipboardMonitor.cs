using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using CopyBase.Forms.Models;
using System.Timers;

namespace CopyBase.DataTier
{
    public static class ClipboardMonitor
    {
        public delegate void OnClipboardChangeEventHandler(DataObject data, string[] formats);
        public static event OnClipboardChangeEventHandler OnClipboardChange;

        public static void Start()
        {

            ClipboardWatcher.Start();
            ClipboardWatcher.OnClipboardChange += (DataObject data, string[] formats) =>
            {
                if (OnClipboardChange != null)
                    OnClipboardChange(data, formats);
            };
        }

        public static void Stop()
        {
            OnClipboardChange = null;
            ClipboardWatcher.Stop();
        }

        public static void AddToClipboard(CopyItem item)
        {
            try
            {
                Clipboard.Clear();
                
                Clipboard.SetDataObject(item.Item, true, 2, 100);
                
                return;
            }
            catch (Exception e)
            {
                try
                {
                    System.Threading.Thread.Sleep(100);
                    //Clipboard.SetData(item.Format.ToString(), item.Item);

                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }

        class ClipboardWatcher : Form
        {
            private System.Timers.Timer timer = new System.Timers.Timer(clipIdle);
            private ClipboardState clipState = ClipboardState.Ready;
            const int clipIdle = 100;

            ClipboardWatcher()
            {
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            }

            public ClipboardState ClipState
            {
                get
                {
                    return clipState;
                }
                set
                {
                    if (clipState != value && value == ClipboardState.Busy)
                    {
                        clipState = value;
                        timer.Start();
                    }
                }
            }

            void timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                timer.Stop();
                clipState = ClipboardState.Ready;
            }
            // static instance of this form
            private static ClipboardWatcher mInstance;

            // needed to dispose this form
            static IntPtr nextClipboardViewer;

            public delegate void OnClipboardChangeEventHandler(DataObject data, string[] formats);
            public static event OnClipboardChangeEventHandler OnClipboardChange;

            // start listening
            public static void Start()
            {
                // we can only have one instance if this class
                if (mInstance != null)
                    return;

                Thread t = new Thread(new ParameterizedThreadStart(x =>
                {
                    Application.Run(new ClipboardWatcher());
                }));
                t.SetApartmentState(ApartmentState.STA); // give the [STAThread] attribute
                t.Start();
            }

            // stop listening (dispose form)
            public static void Stop()
            {
                mInstance.Invoke(new MethodInvoker(() =>
                {
                    ChangeClipboardChain(mInstance.Handle, nextClipboardViewer);
                }));
                mInstance.Invoke(new MethodInvoker(mInstance.Close));

                mInstance.Dispose();

                mInstance = null;
            }

            // on load: (hide this window)
            protected override void SetVisibleCore(bool value)
            {
                CreateHandle();

                mInstance = this;

                nextClipboardViewer = SetClipboardViewer(mInstance.Handle);

                base.SetVisibleCore(false);
            }

            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_DRAWCLIPBOARD:
                        if (ClipState == ClipboardState.Ready)
                        {
                            ClipChanged();
                            ClipState = ClipboardState.Busy;
                        }
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;

                    case WM_CHANGECBCHAIN:
                        if (m.WParam == nextClipboardViewer)
                            nextClipboardViewer = m.LParam;
                        else
                            SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;

                    default:
                        base.WndProc(ref m);
                        break;
                }
            }

            private void ClipChanged()
            {
                try
                {
                    
                    IDataObject iData = Clipboard.GetDataObject();

                    var formats = iData.GetFormats();

                    if (formats.Length < 1 || iData==null)
                        return;

                    DataObject dataObj = new DataObject();
                    foreach (var f in formats)
                    {
                        if (f != "EnhancedMetafile")
                        {
                            var data = iData.GetData(f);
                            dataObj.SetData(f, data); 
                        }
                    }

                    if (OnClipboardChange != null)
                        OnClipboardChange(dataObj, formats);
                }
                catch (Exception)
                {
                    //TODO
                }
            }


        }
    }

    public enum ClipboardFormat : byte
    {
        /// <summary>Specifies the Windows device-independent bitmap (DIB) format. This static
        /// field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Dib,
        /// <summary>Specifies a Windows bitmap format. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Bitmap,
        /// <summary>Specifies the Windows enhanced metafile format. This static field is
        /// read-only.</summary>
        /// <filterpriority>1</filterpriority>
        //EnhancedMetafile,
        /// <summary>Specifies the Windows metafile format, which Windows Forms does not
        /// directly use. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        MetafilePict,
        /// <summary>Specifies the Windows symbolic link format, which Windows Forms does
        /// not directly use. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        SymbolicLink,
        /// <summary>Specifies the Windows Data Interchange Format (DIF), which Windows Forms
        /// does not directly use. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Dif,
        /// <summary>Specifies the Tagged Image File Format (TIFF), which Windows Forms does
        /// not directly use. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Tiff,
        /// <summary>Specifies the standard Windows original equipment manufacturer (OEM)
        /// text format. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        OemText,
        /// <summary>Specifies the Windows palette format. This static field is read-only.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        Palette,
        /// <summary>Specifies the Windows pen data format, which consists of pen strokes
        /// for handwriting software, Windows Forms does not use this format. This static
        /// field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        PenData,
        /// <summary>Specifies the Resource Interchange File Format (RIFF) audio format,
        /// which Windows Forms does not directly use. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Riff,
        /// <summary>Specifies the wave audio format, which Windows Forms does not directly
        /// use. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        WaveAudio,
        /// <summary>Specifies the Windows file drop format, which Windows Forms does not
        /// directly use. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        FileDrop,
        /// <summary>Specifies the Windows culture format, which Windows Forms does not directly
        /// use. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        //Locale,
        /// <summary>Specifies text consisting of HTML data. This static field is read-only.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        Html,
        /// <summary>Specifies text consisting of Rich Text Format (RTF) data. This static
        /// field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Rtf,
        /// <summary>Specifies a comma-separated value (CSV) format, which is a common interchange
        /// format used by spreadsheets. This format is not used directly by Windows Forms.
        /// This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        CommaSeparatedValue,
        /// <summary>Specifies the Windows Forms string class format, which Windows Forms
        /// uses to store string objects. This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        StringFormat,
        /// <summary>Specifies a format that encapsulates any type of Windows Forms object.
        /// This static field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Serializable,
        /// <summary>Specifies the standard Windows Unicode text format. This static field
        /// is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        UnicodeText,
        /// <summary>Specifies the standard ANSI text format. This static field is read-only.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        Text,
    }

    public enum ClipboardState : byte
    {
        /// <summary>Specifies that current clipboard item is under processing. This static
        /// field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Busy,
        /// <summary>Specifies that clipboard items are ready to be processed. This static
        /// field is read-only.</summary>
        /// <filterpriority>1</filterpriority>
        Ready,
    }
}