using ScrumGestures.Events;
using System;

namespace ScrumGestures.Listener
{
    /// <summary>
    /// Erlaubt die Implementierung verschiedener TouchListener
    /// (nicht bloß für das TUIO-Protokoll)
    /// </summary>
    public abstract class TouchListener : IDisposable
    {

        protected GestureManager GestureManager;
        public event EventHandler<GenericEventArgs<TouchPoint>> TouchDown;
        public event EventHandler<GenericEventArgs<TouchPoint>> TouchUp;
        public event EventHandler<GenericEventArgs<TouchPoint>> TouchMove;

        public void RegisterManager(GestureManager manager)
        {
            this.GestureManager = manager;
        }
        #region Events
        protected void OnTouchDown(TouchPoint tp)
        {
            if (TouchDown != null)
                TouchDown(this, new GenericEventArgs<TouchPoint>(tp));
        }
        protected void OnTouchUp(TouchPoint tp)
        {
            if (TouchUp != null)
                TouchUp(this, new GenericEventArgs<TouchPoint>(tp));
        }
        protected void OnTouchMove(TouchPoint tp)
        {
            if (TouchMove != null)
                TouchMove(this, new GenericEventArgs<TouchPoint>(tp));
        }
        #endregion



        public abstract void Dispose();
    }
}
