using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CopyBase.Forms.Models
{
    public class CopyItem:INotifyPropertyChanged
    {
        #region Private fields
        private Guid id;
        private DateTime copyDate;
        private DateTime pasteDate;
        private String data;
        private String entry;
        #endregion

        /*
         * Only getter
         */ 
        public Guid ID
        {
            get
            {
                return id;
            }
        }

        public String Data
        {
            get
            {
                return data;
            }
            set
            {
                if (data != value)
                {
                    data = value;
                    NotifyPropertyChanged("Data");
                }
            }
        }

        public String Entry
        {
            get
            {
                return entry;
            }
            set
            {
                try
                {
                    String newvalue = value;
                    newvalue.Trim();
                    var splitted = newvalue.Split(new string[] { "\r\n", "\n" }, 4, StringSplitOptions.RemoveEmptyEntries);
                    newvalue = splitted[0];
                    var stop = splitted.Length > 3 ? 3 : splitted.Length;
                    for (int i = 1; i < stop; i++)
                    { 
                        newvalue += "\r\n" + splitted[i];
                    }
                    if (entry != newvalue)
                    {
                        entry = newvalue;
                        NotifyPropertyChanged("Entry");
                    }
                }
                catch (Exception)
                {
                    if (entry != value)
                    {
                        entry = value;
                        NotifyPropertyChanged("Entry");
                    }
                }
            }
        }

        /*
         * Constructor
         */
        public CopyItem(String D)
        {
            id = new Guid();
            Data = D;
            Entry = D;
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
