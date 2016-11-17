using ScrumGestures.Gestures;
using System;
using System.Windows;

namespace ScrumGestures.Events
{
    /// <summary>
    /// EventArgs, die von den Events AfterValidation and BeforeValidation übergeben werden. (Siehe Klassen GestureBase)
    /// </summary>
    public class GestureValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Wurde die Geste bereits validiert?
        /// </summary>
        public bool HasValidated
        {
            get;
            set;
        }
        /// <summary>
        /// Wurde die Geste erkannt?
        /// </summary>
        public bool IsValid
        {
            get;
            set;
        }
        /// <summary>
        /// Das UIElement, auf dem diese Geste validiert werden soll/wurde
        /// </summary>
        public UIElement Element
        {
            get;
            set;
        }
        /// <summary>
        /// Die Berührungspunkte, die für diese Geste betrachtet werden/wurden
        /// </summary>
        public TouchGroup Points
        {
            get;
            set;
        }
        /// <summary>
        /// Die Geste, die dieses Event aufgerufen hat.
        /// </summary>
        public GestureBase Gesture
        {
            get;
            set;
        }

        /// <summary>
        /// Initialisiert neue GestureValidationEventArgs
        /// </summary>
        /// <param name="hasvalidated">Wurde die Geste bereits validiert?</param>
        /// <param name="isvalid">Wurde die Geste erkannt?</param>
        /// <param name="ui">Das UIElement, auf dem diese Geste validiert werden soll/wurde</param>
        /// <param name="points">Die Berührungspunkte, die für diese Geste betrachtet werden/wurden</param>
        /// <param name="gesture">Die Geste, die dieses Event aufgerufen hat.</param>
        public GestureValidationEventArgs(bool hasvalidated, bool isvalid, UIElement ui, TouchGroup points, GestureBase gesture)
        {
            HasValidated = hasvalidated;
            IsValid = isvalid;
            Element = ui;
            Points = points;
            Gesture = gesture;
        }

    }
}
