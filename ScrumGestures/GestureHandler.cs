using ScrumGestures.Gestures;
using System.Windows;

namespace ScrumGestures
{
    /// <summary>
    /// Beschreibt eine Verknüpfung zwischen einem UIElement und einer Geste
    /// Das UIElement erhält somit die entsprechende Geste
    /// </summary>
    public class GestureHandler
    {
        /// <summary>
        /// Das UIElement
        /// </summary>
        public UIElement Element
        {
            get;
            set;
        }

        /// <summary>
        /// Die Geste
        /// </summary>
        public GestureBase Gesture
        {
            get;
            set;
        }

        /// <summary>
        /// Die Methode, die aufgerufen wird, wenn die Geste erkannt wurde
        /// </summary>
        public GestureCallbackHandler Callback
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt an, ob die Geste für dieses UIElement aktiviert ist
        /// </summary>
        public bool IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// Initialisiert eine neue Geste-Element Verknüpfung
        /// </summary>
        /// <param name="element">Das UIElement auf welchem die Geste ausgeführt wid</param>
        /// <param name="gesture">Die Geste</param>
        /// <param name="callback">Die Methode die aufgerufen wird, wenn die Geste entdeckt wurde</param>
        public GestureHandler(UIElement element, GestureBase gesture, GestureCallbackHandler callback)
        {
            Element = element;
            Gesture = gesture;
            Callback = callback;
            IsActive = true;
        }

        /// <summary>
        /// Überprüft, ob die Geste stattgefunden hat und führt dann ggf. den Callback aus
        /// </summary>
        /// <param name="group">Die Berührungspunkte, die zur Erkennung dienen</param>
        public void Execute(TouchGroup group)
        {
            if (IsActive)
            {
                TouchGroup g2 = group.Copy();
                if (Gesture.Validate(this.Element, g2))
                {
                    Callback(this.Element, g2);
                }
            }
        }
        public delegate void GestureCallbackHandler(UIElement element, TouchGroup points);

        
    }
}
