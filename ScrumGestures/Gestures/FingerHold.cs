using System.Windows;

namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Überprüft, ob eine bestimmte Anzahl von Fingern für eine bestimmte Zeit auf einem Objet verweilt haben.
    /// Diese Klasse kann für "Hold", aber auch für "Tap" Gesten verwendet werden.
    /// </summary>
    public class FingerHold : MultiFingerGestureBase
    {
        #region Getter & Setter
        /// <summary>
        /// Die Länge der Berührung
        /// </summary>
        public int Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt an, ob die Länge der Berührung mindestens erfüllt sein muss ("Hold") oder maximal erfüllt sein darf ("Tap")
        /// </summary>
        public ComparisonMode Mode
        {
            get;
            set;
        }
        
        #endregion
        #region Constructor
        /// <summary>
        /// Initialisiert eine neue Klasse
        /// </summary>
        /// <param name="fingerCount">Anzahl der Berührungspunkte</param>
        /// <param name="durationinms">Dauer der Berührung</param>
        /// <param name="mode">Gibt an, wie die Berührungsdauer mit der Duration verglichen wird.</param>
        public FingerHold(int fingerCount, int durationinms, ComparisonMode mode) : base(fingerCount)
        {
            Duration = durationinms;
            Mode = mode;
        }
        #endregion
        #region Validation
        protected override bool ValidateFinger(TouchPoint point, UIElement ui, TouchGroup allPoints)
        {
            if (point.Mode != TouchPoint.TouchMode.UP)
                    return false;
            if (point.Distance > 50)
                return false;
            int sofar = point.DurationMS;
            return Compare(sofar);
        }
        /// <summary>
        /// Vergleicht die Berührungsdauer mit der angegebenen Duration je nach ComparisonMode.
        /// </summary>
        /// <param name="v">Die atkuelle Berühungsdauer</param>
        /// <returns></returns>
        protected bool Compare(int v)
        {
            switch(Mode)
            {
                case ComparisonMode.EQUAL:
                    return v == Duration;
                case ComparisonMode.MAX:
                    return v <= Duration;
                case ComparisonMode.MIN:
                    return v >= Duration;
                default:
                    return false;
            }
        }
        #endregion
        #region COMPARISON MODE
        /// <summary>
        /// Arten und Weisen für den Berührungsvergleich
        /// </summary>
        public enum ComparisonMode
        {
            /// <summary>
            /// Die Berührung darf maximal so lange dauern wie die angegebene Zeit (z.B. "Tap")
            /// </summary>
            MAX,
            /// <summary>
            /// Die Berührung muss mindestens so lange dauern wie die angegebene Zeit (z.B. "Hold")
            /// </summary>
            MIN,
            /// <summary>
            /// Berührungsdauer und angegebene Zeit müssen exakt gleich sein.
            /// </summary>
            EQUAL
        }
        #endregion
    }
}
