namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Überprüft, ob sich ein einzelne Berührung auf einem Objekt geändert hat.
    /// </summary>
    public class SingleTouchChanged : GestureBase
    {
        private TouchPoint.TouchMode _mode = TouchPoint.TouchMode.ANY;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">Gibt an, bei welchen Änderungen diese Geste erkannt wird. TouchPoint.TouchMode.ANY erlaubt jede Änderung.</param>
        public SingleTouchChanged(TouchPoint.TouchMode mode)
        {
            _mode = mode;
        }

        protected override bool InternalValidation(System.Windows.UIElement ui, TouchGroup points)
        {
            if (points.Count == 1)
            {
                if (_mode == TouchPoint.TouchMode.ANY)
                    return true;
                else
                {
                    if (points[0].Mode == _mode)
                        return true;
                }
            }
            return false;
        }
    }
}
