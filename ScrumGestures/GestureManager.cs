using ScrumGestures.Events;
using ScrumGestures.Gestures;
using ScrumGestures.Helper;
using ScrumGestures.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ScrumGestures
{
    /// <summary>
    /// Verwaltet alle Gesten und stellt die Verbindung zum Protokoll zur Erkennung von Berührungen dar
    /// </summary>
    public class GestureManager : IDisposable
    {
        private TouchListener _listener;

        #region Getter & Setter
        /// <summary>
        /// Der Canvas, in welchem sich alle Objekte befinden
        /// </summary>
        public Canvas Root
        {
            get;
            private set;
        }
        /// <summary>
        /// Liste aller registrierter Gesten und deren UIElements
        /// </summary>
        public List<GestureHandler> RegisteredGestures
        {
            get;
            private set;
        }

        /// <summary>
        /// Gibt an, ob der GestureManager bereits initialisiert wurde
        /// </summary>
        public bool IsInitialized
        {
            get;
            private set;
        }

        private Dictionary<UIElement, TouchGroup> currentGestures = new Dictionary<UIElement, TouchGroup>();
        #endregion

        #region Constructor & Initializisation
        /// <summary>
        /// Initialisiert einen neuen GestureManager
        /// </summary>
        /// <param name="listener">Der zu verwendende TouchListener</param>
        public GestureManager(TouchListener listener)
        {
            RegisteredGestures = new List<GestureHandler>();
            this._listener = listener;
            this._listener.RegisterManager(this);
            this._listener.TouchDown += this.TouchDown;
            this._listener.TouchUp += this.TouchUp;
            this._listener.TouchMove += this.TouchMove;
        }
        /// <summary>
        /// Initialisiert den Canvas. Muss vor der Verwendung aufgerufen werden!
        /// </summary>
        /// <param name="root">Der Canvas</param>
        public void Initialize(Canvas root)
        {
            Root = root;
            root.KeyUp += root_KeyUp;
            IsInitialized = true;
        }

        void root_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Mit F1 können die aktuellen Berührungspunkte gelöscht werden
            //Somit können Berührungspunkte, die nur noch virtuell existieren
            //entfernt werden
            if (e.Key == System.Windows.Input.Key.F1)
                currentGestures.Clear();
        }
        #endregion
        #region Gesture Management
        /// <summary>
        /// Registriert eine Geste für ein UIElement und erstellt somit einen passenden GestureHandler
        /// </summary>
        /// <param name="element">Das UIElement, welches die Geste erhalten soll</param>
        /// <param name="gesture">Die Geste</param>
        /// <param name="callback">Die Methode, die aufgerufen werden soll, wenn die Geste erkannt wurde</param>
        /// <returns>Der GestureHandler für diese Verknüpfung</returns>
        public GestureHandler AddGesture(UIElement element, GestureBase gesture, GestureHandler.GestureCallbackHandler callback)
        {
            GestureHandler newgesture = new GestureHandler(element, gesture, callback);
            if (!RegisteredGestures.Contains(newgesture))
                RegisteredGestures.Add(newgesture);
            return newgesture;

        }
        /// <summary>
        /// Entfernt einen GestureHandler (und somit die Verknüpfung von Geste und UIElement)
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveGesture(GestureHandler handler)
        {
            RegisteredGestures.Remove(handler);
        }

        /// <summary>
        /// Überprüft, ob das übergebene UIElement eine Geste besitzt
        /// </summary>
        /// <param name="e"></param>
        /// <returns>TRUE wenn eine Geste zu diesem UIElement gefunden wurde</returns>
        public bool HasGesture(UIElement e)
        {
            for (int i = 0; i < RegisteredGestures.Count; i++)
            {
                if (RegisteredGestures[i].Element == e)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gibt alle Gesten zu einem UIElement zurück
        /// </summary>
        /// <param name="e">Das UIElement, zu dem alle Gesten gesucht werden sollen</param>
        /// <param name="onlyactive">Wenn hier TRUE angegeben wird, werden nur aktive Gesten zurückgegeben</param>
        /// <returns>Die Liste der Gesten</returns>
        public List<GestureHandler> GetGestures(UIElement e, bool onlyactive = false)
        {
            List<GestureHandler> gestures = new List<GestureHandler>();
            for (int i = 0; i < RegisteredGestures.Count; i++)
            {
                if (RegisteredGestures[i].Element == e)
                    if(!onlyactive || RegisteredGestures[i].IsActive)
                        gestures.Add(RegisteredGestures[i]);
            }
            return gestures;
        }

        /// <summary>
        /// Gibt die TouchGroup eines TouchPoints mit der angegebenen SessionID zurück, falls vorhanden
        /// </summary>
        /// <param name="sessionID">Die SessionID des TouchPoints</param>
        /// <returns>Die TouchGroup</returns>
        public TouchGroup GetTouchGroup(long sessionID)
        {
            for(int i = 0; i < currentGestures.Values.Count; i++)
            {
                if (currentGestures.Values.ElementAt(i).Contains(sessionID))
                    return currentGestures.Values.ElementAt(i);
            }
            return null;
        }

        /// <summary>
        /// Überprüft eine TouchGroup und dessen UIElement auf alle möglichen Gesten, die erfüllt sein könnten 
        /// und führt diese ggf. aus
        /// </summary>
        /// <param name="group">Die TouchGroup</param>
        private void UpdateGestures(TouchGroup group)
        {
            List<GestureHandler> gestures = GetGestures(group.Element, true);
            foreach (GestureHandler g in gestures)
            {
                g.Execute(group);
            }
        }
        #endregion
        #region Listener
        /// <summary>
        /// Wird aufgerufen , wenn ein neuer TouchPoint auf der Oberfläche erkannt wurde
        /// </summary>
        /// <param name="sender">Der Listener</param>
        /// <param name="gtp">Der TouchPoint</param>
        public void TouchDown(object sender, GenericEventArgs<TouchPoint> gtp)
        {
            TouchPoint tp = gtp.Value;
            if (!IsInitialized)
                return;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //Nachschlagen welches UIElement an der entsprechenden Position liegt
                UIElement uie = new HitTestExecution(this).ExecuteHitTest(Root, tp.CurrentPoint);
                if (uie != null)
                {
                    //Nachschlagen, ob es bereits eine TouchGroup zu diesem Element gibt.
                    //Diese anschließend aktualisieren (also den TouchPoint zu dieser Hinzufügen)
                    //oder eine neue erstellen
                    if (currentGestures.ContainsKey(uie))
                    {
                        currentGestures[uie].Add(tp);
                        UpdateGestures(currentGestures[uie]);
                    }
                    else
                    {
                        TouchGroup tg = new TouchGroup(uie, tp);
                        this.currentGestures.Add(uie, tg);
                        UpdateGestures(tg);
                    }
                }
            }));
        }
        /// <summary>
        /// Wird aufgerufen , wenn ein TouchPoint von der Oberfläche entfernt wurde.
        /// Entfernt diesen aus der entsprechenden TouchGroup, und löscht diese falls der TouchPoint
        /// der letzte war
        /// </summary>
        /// <param name="sender">Der Listener</param>
        /// <param name="gtp">Der TouchPoint</param>
        public void TouchUp(object sender, GenericEventArgs<TouchPoint> gtp)
        {
            TouchPoint tp = gtp.Value;
            TouchGroup group = GetTouchGroup(tp.SessionID);
            if (group != null)
            {
                UpdateGestures(group);

                group.Remove(tp.SessionID);
                if (group.Count == 0)
                    currentGestures.Remove(group.Element);
            }
        }
        /// <summary>
        /// Wird aufgerufen , wenn ein TouchPoint auf der Oberfläche bewegt wurde
        /// </summary>
        /// <param name="sender">Der Listener</param>
        /// <param name="gtp">Der TouchPoint</param>
        public void TouchMove(object sender, GenericEventArgs<TouchPoint> gtp)
        {
            TouchPoint tp = gtp.Value;
            TouchGroup group = GetTouchGroup(tp.SessionID);
            if (group != null)
                UpdateGestures(group);
        }
        #endregion


        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}
