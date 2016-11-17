namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Diese Geste überprüft, ob sich die Anzahl der Finger auf einem Objekt geändert hat.
    /// </summary>
    public class FingerChanged : GestureBase
    {

        protected override bool InternalValidation(System.Windows.UIElement ui, TouchGroup points)
        {
            foreach (TouchPoint tp in points)
            {
                if (tp.Mode == TouchPoint.TouchMode.UP || tp.Mode == TouchPoint.TouchMode.DOWN)
                    return true;
            }
            return false;
        }
    }
}
