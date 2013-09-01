using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIControls
{
    public static class StaticUICommands
    {
        public static EventHandler<EventArgs> OnDelete;

        public static void DeleteCommand(object item)
        {
            EventHandler<EventArgs> deleteEvent = OnDelete;
            if (deleteEvent != null)
            {
                deleteEvent(item, new EventArgs());
            }
        }
    }
}
