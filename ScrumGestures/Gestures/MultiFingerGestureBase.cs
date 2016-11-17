using System.Windows;

namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Ermöglicht Gesten, die mit einer unterschiedlichen Anzahl von Berührungspunkten arbeiten können.
    /// Dafür werden die einzelnen Berührungspunkte unabhängig (!) voneinander validiert und sobald eine
    /// der Berührungen korrekt validiert, wurde die Geste erkannt.
    /// </summary>
    public abstract class MultiFingerGestureBase : GestureBase
    {
        #region Getter & Setter
        /// <summary>
        /// Anzahl der Berührungen, die für diese Geste zulässig sind
        /// </summary>
        public int FingerCount
        { 
            get; 
            set; 
        }
        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fingerCount">Anzahl der Berührungen, die für diese Geste zulässig sind</param>
        public MultiFingerGestureBase(int fingerCount)
        {
            FingerCount = fingerCount;
        }
        #endregion
        #region Validation
        /// <summary>
        /// Einer der Berührungspunkte muss korrekt validiert werden.
        /// </summary>
        /// <param name="ui">Das UIElement, auf dem diese Geste validiert wird</param>
        /// <param name="points">Die Berührungspunkte, die für diese Validierung betrachtet werden</param>
        /// <returns>TRUE wenn mindestens ein Berührungspunkt korrekt validiert wird</returns>
        protected override bool InternalValidation(System.Windows.UIElement ui, TouchGroup points)
        {
            if (points.MaxPoints == FingerCount && points.Count == FingerCount)
            {
                bool valid = false;
                for (int i = 0; i < FingerCount; i++)
                {
                    valid = ValidateFinger(points[i], ui, points) || valid;
                }
                return valid;
            }
            return false;
        }

        /// <summary>
        /// Validiert eine einzelne Berührung
        /// </summary>
        /// <param name="point">Der aktuelle Berührungspunkt</param>
        /// <param name="ui">Das UIElement, auf dem diese Geste validiert wird</param>
        /// <param name="allPoints">Alle Berührungspunkte</param>
        /// <returns>TRUE wenn die Berührung die Bedingungen erfüllt</returns>
        protected abstract bool ValidateFinger(TouchPoint point, UIElement ui, TouchGroup allPoints);
        #endregion
    }
}
