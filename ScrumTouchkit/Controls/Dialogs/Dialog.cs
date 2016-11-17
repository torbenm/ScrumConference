using ScrumGestures;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Events;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ScrumTouchkit.Controls.Dialogs
{
    /// <summary>
    /// Klassen, die von dieser Klasse erben können als Dialoge benutzt werden
    /// </summary>
    public abstract class Dialog : UserControl
    {
        #region vars, getter, setter
        /// <summary>
        /// Das DialogControl<> Objekt
        /// </summary>
        protected SurfaceObject DialogParent
        {
            get;
            private set;
        }

        /// <summary>
        /// Die Gesten, die in diesem Dialog gespeichert sind
        /// </summary>
        protected Dictionary<string, GestureHandler> Gestures
        {
            get;
            private set;
        }
        #endregion
        #region Constructor
        public Dialog(SurfaceObject obj)
        {
            Gestures = new Dictionary<string, GestureHandler>();
            DialogParent = obj;
            InitializeDialog();
            this.Loaded += (s, e) => { InitializeGestures(); };
        }
        protected abstract void InitializeDialog();
        public abstract void InitializeDialogContent(DialogInfo info);
        #endregion
        #region Events
        public event EventHandler<DialogEventArgs> DialogFinished;

        protected void OnDialogFinished(DialogEventArgs args)
        {
            if (DialogFinished != null)
                DialogFinished(this, args);
        }
        #endregion
        #region Gestures
        public void AddGesture(string name, UIElement elem, ScrumGestures.Gestures.GestureBase gesture, GestureHandler.GestureCallbackHandler callback)
        {
            this.Gestures.Add(name,
                DialogParent.Surface.GestureManager.AddGesture(elem, gesture, callback));
        }
        public void RemoveGestures()
        {
            foreach (GestureHandler gh in Gestures.Values)
                DialogParent.Surface.GestureManager.RemoveGesture(gh);
        }
        public abstract void InitializeGestures();
        #endregion



    }
}
