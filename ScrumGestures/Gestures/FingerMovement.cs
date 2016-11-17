using System.Windows;

namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Mit dieser Klasse können Gesten implementiert werden, bei der sich die Finger um mindestens 20px bewegt haben.
    /// </summary>
    public class FingerMovement : MultiFingerGestureBase
    {

        public FingerMovement(int fingerCount)
            : base(fingerCount)
        { }

        protected override bool ValidateFinger(TouchPoint point, UIElement ui, TouchGroup allPoints)
        {
            return point.Distance  > 20;
        }
    }
}
