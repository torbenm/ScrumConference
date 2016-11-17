using System.Collections.Generic;
using System.Windows;

namespace ScrumGestures
{
    /// <summary>
    /// Eine Gruppe von Berührungen.
    /// Pro UIElement gibt es immer genau eine Gruppe von Berührungen (sofern vorhanden)
    /// Jede neue Berührung uaf diesem Element wird dann zu der Gruppe hinzugefügt.
    /// </summary>
    public class TouchGroup : List<TouchPoint>
    {
        /// <summary>
        /// Das zugehörige UIElement
        /// </summary>
        public UIElement Element
        {
            get;
            private set;
        }

        /// <summary>
        /// Speichert, wieviele Berührungspunkte in der aktuellen Berührungssequenz (also dieser TouchGroup) MAXIMAL vorhanden waren.
        /// </summary>
        public int MaxPoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Initialisiert eine neue TouchGroup mit einem TouchPoint und einem UIElement
        /// </summary>
        /// <param name="uie">Das dazugehörige UIElement</param>
        /// <param name="start">Der TouchPoint zur initialisierung</param>
        public TouchGroup(UIElement uie, TouchPoint start) : base()
        {
            this.Element = uie;
            this.Add(start);
            this.MaxPoints = 1;
        }


        /// <summary>
        /// Kopiert eine TouchGroup
        /// </summary>
        /// <param name="tg">Die zu kopierende TouchGroup</param>
        private TouchGroup(TouchGroup tg)
            : base(tg)
        {
            this.Element = tg.Element;
            this.MaxPoints = tg.MaxPoints;
        }

        /// <summary>
        /// Fügt einen neuen Berührungspunkt zu dieser Gruppe hinzu
        /// </summary>
        /// <param name="item">Der neue Berührungspunkt</param>
        public new void Add(TouchPoint item)
        {
            this.MaxPoints = this.Count + 1 > MaxPoints ? this.Count + 1 : MaxPoints;
            base.Add(item);
        }

        /// <summary>
        /// Überprüft, ob ein TouchPoint mit der entsprechenden SessionID in dieser TouchGroup gespeichert ist.
        /// </summary>
        /// <param name="sessionID">Die SessionID, die zu suchen ist</param>
        /// <returns>TRUE wenn der entsprechende TouchPoint in dieser TouchGroup vorhanden ist</returns>
        public bool Contains(long sessionID)
        {
            foreach (TouchPoint tp in this)
            {
                if (tp.SessionID == sessionID)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gibt den TouchPoint mit der entsprechenden SessionID zurück, wenn dieser sich in dieser Touch 
        /// </summary>
        /// <param name="sessionID">Die SessionID, die zu suchen ist</param>
        /// <returns>Der gefundene TouchPoint</returns>
        public TouchPoint Get(long sessionID)
        {
            foreach (TouchPoint tp in this)
            {
                if (tp.SessionID == sessionID)
                    return tp;
            }
            return null;
        }

        /// <summary>
        /// Entfernt einen TouchPoint mit der entsprechenden SessionID, falls in dieser TouchGroup vorhanden
        /// </summary>
        /// <param name="sessionID">Die SessionID, dessen TouchPoint zu löschen ist</param>
        public void Remove(long sessionID)
        {
            for(int i = 0; i < this.Count; i++)
            {
                if (this[i].SessionID == sessionID)
                    Remove(this[i]);
            }
        }
        /// <summary>
        /// Kopiert eine TouchGroup
        /// </summary>
        /// <returns>Die Kopie</returns>
        public TouchGroup Copy()
        {
            return new TouchGroup(this);
        }
    }
}
