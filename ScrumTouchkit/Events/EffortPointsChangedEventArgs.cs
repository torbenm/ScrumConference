using ScrumTouchkit.Data.Effort;
using System;

namespace ScrumTouchkit.Events
{

    /// <summary>
    /// Wird versendet, wenn sich die EffortPoints eines Items geändert haben
    /// </summary>
    public class EffortPointsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Neue Anzahl an Effort-Points
        /// </summary>
        public EffortPoints New
        {
            get;
            set;
        }

        /// <summary>
        /// Alte Anzahl von Effort-Points
        /// </summary>
        public EffortPoints Old
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt an, ob es durch eine Änderung im Netzwerk zu 
        /// der Veränderung kam
        /// </summary>
        public bool ExternallyTriggered
        {
            get;
            set;
        }
    }
}
