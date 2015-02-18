using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CopyBase.Forms.ViewModels
{
    /// <summary>
    /// Base view model, that implements INotifyPropertyChanged and en ErrorMessage property.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string errorMessage;

        /// <summary>
        /// Error Message.
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    NotifyPropertyChanged("ErrorMessage");
                }
            }

        }

        /// <summary>
        /// Notify of Property Changed event.
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
