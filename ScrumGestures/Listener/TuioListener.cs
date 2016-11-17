using System.Linq;
using TUIO;

namespace ScrumGestures.Listener
{
    /// <summary>
    /// TouchListener für TUIO
    /// </summary>
    public class TuioListener : TouchListener, TUIO.TuioListener
    {
        private TuioClient client;
        public TuioListener(int port = 3333)
        {
            client = new TuioClient(port);
            client.addTuioListener(this);
            client.connect();
        }
        #region Tangibles - Not implemented!
        public void addTuioObject(TuioObject tobj)
        {
          
        }

        public void updateTuioObject(TuioObject tobj)
        {
           
        }

        public void removeTuioObject(TuioObject tobj)
        {
           
        }
        public void refresh(TuioTime ftime)
        {
            
        }
        #endregion
        #region Cursors
        public void addTuioCursor(TuioCursor tcur)
        {
            if (GestureManager != null)
            {
                TouchPoint tp = this.CreateTouchPoint(tcur);
                OnTouchDown(tp);
            }
        }

        public void updateTuioCursor(TuioCursor tcur)
        {
            if (GestureManager != null)
            {
                TouchGroup tg = GestureManager.GetTouchGroup(tcur.getSessionID());
                if (tg != null)
                {
                    TouchPoint tp = tg.Get(tcur.getSessionID());
                    tp.Mode = TouchPoint.TouchMode.MOVE;
                    this.UpdateTouchPoint(tp, tcur);
                    OnTouchMove(tp);
                }
            }
        }

        public void removeTuioCursor(TuioCursor tcur)
        {
            if (GestureManager != null)
            {
                TouchGroup tg = GestureManager.GetTouchGroup(tcur.getSessionID());
                if (tg != null)
                {
                    TouchPoint tp = tg.Get(tcur.getSessionID());
                    tp.Mode = TouchPoint.TouchMode.UP;
                    this.UpdateTouchPoint(tp, tcur);
                    OnTouchUp(tp);
                }
            }
        }
        #endregion
        #region TouchPoints
        private TouchPoint CreateTouchPoint(TuioCursor cursor)
        {
            TouchPoint tp = new TouchPoint
            {
                StartPoint = cursor.getPath().First().ToPoint(GestureManager),
                StartTimeMS = cursor.getStartTime().getTotalMilliseconds(),
                CurrentPoint = cursor.getPath().First().ToPoint(GestureManager),
                SessionID = cursor.getSessionID()
            };
            return UpdateTouchPoint(tp, cursor);
        }
        private TouchPoint UpdateTouchPoint(TouchPoint tp, TuioCursor cursor)
        {
            tp.PreviousPoint = tp.CurrentPoint;
            tp.CurrentPoint = cursor.getPath().Last().ToPoint(GestureManager);
            tp.CurrentTimeMS = cursor.getTuioTime().getTotalMilliseconds();
            tp.Distance = cursor.getTotalDistanceInPixel(this.GestureManager);
            tp.PathLength = cursor.getPath().Count;
            tp.DistanceRelative = cursor.getTotalDistance();
            return tp;
        }
        #endregion

        public override void Dispose()
        {
            client.disconnect();
            client.removeAllTuioListeners();
        }
    }
}
