using ScrumTouchkit.Data;
using System;

namespace ScrumTouchkit.Events
{
    /// <summary>
    /// Wird benutzt, wenn ein Item in den Bearbeitungsmodus wechselt
    /// </summary>
    public class EditorStateEventArgs : EventArgs
    {
        /// <summary>
        /// Das Item, welches in den Bearbeitungsmodus wechselt
        /// </summary>
        public ItemBase Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt an, ob der Bearbeitungsmodus gestartet oder beendet wird
        /// </summary>
        public bool IsStarting
        {
            get;
            set;
        }

        public EditorStateEventArgs(ItemBase data, bool isstarting)
        {
            this.Data = data;
            this.IsStarting = isstarting;
        }
    }
}
