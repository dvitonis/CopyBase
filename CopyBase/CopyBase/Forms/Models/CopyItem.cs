using CopyBase.DataTier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        private string[] formats;
        private DataObject item;
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

        public DataObject Item
        {
            get
            {
                return item;
            }
            set
            {
                if (item != value)
                {
                    item = value;
                    NotifyPropertyChanged("Item");
                }
            }
        }

        public string[] Formats
        {
            get
            {
                return formats;
            }
            set
            {
                if (formats != value)
                {
                    formats = value;
                    NotifyPropertyChanged("Formats");
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

        public CopyItem(DataObject O, string[] formats)
        {
            id = new Guid();
            Item = O;
            Data = O.GetData(formats[0]).ToString();
            Entry = O.GetData(formats[0]).ToString();
            Formats = formats;
        }

        public bool Equals(CopyItem obj)
        {
            if (this.Formats.OrderBy(i => i).SequenceEqual(obj.Formats.OrderBy(i => i)))
            {
                foreach (var f in this.Formats)
                {
                    if (f != "EnhancedMetafile" && f != "HTML Format")
                    {
                        var one = this.Item.GetData(f).ToString();
                        var two = obj.Item.GetData(f).ToString();
                        if (!one.Equals(two))
                            return false;
                    }
                }
                return true;
                //var one = GetHash(this);
                //var two = GetHash(obj);
                //if (GetHash(this) == GetHash(obj))
                //    return true;
                //else return false;
            }
            return false;
        }

        private int GetHash(CopyItem ci)
        {
            var hashcode = 0;
            if (ci.Formats.Contains("FileDrop"))
            {
                foreach (String i in (ci.Item.GetData("FileDrop") as String[]))
                {
                    hashcode += i.GetHashCode();
                }
            }
            else foreach (var f in ci.Formats)
                {
                    if (f != "EnhancedMetafile")
                    {
                        var it = ci.Item.GetData(f);
                        hashcode += it.GetHashCode();
                    }
                }
            return hashcode;
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
