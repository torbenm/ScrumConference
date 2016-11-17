using ScrumGestures.Events;
using System;
using System.Windows;

namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Abstrakte Klasse, die als Basis für Gesten verwendet werden muss.
    /// Aufbauend auf ihr können Gesten implementiert werden.
    /// </summary>
    public abstract class GestureBase
    {
        #region Getter & Setter
        private bool _isactive = true;
        /// <summary>
        /// Gibt an, ob die Geste aktiviert oder deaktiviert ist.
        /// </summary>
        public bool IsActive
        {
            get { return _isactive; }
            set { _isactive = value; }
        }
        #endregion
        #region Events
        /// <summary>
        /// Wird aufgerufen, bevor die Validierungsfunktion ausgeführt wird.
        /// </summary>
        public event EventHandler<GestureValidationEventArgs> BeforeValidation;
        /// <summary>
        /// Wird aufgerufen, nachdem die Validierungsfunktion ausgeführt wurde.
        /// </summary>
        public event EventHandler<GestureValidationEventArgs> AfterValidation;

        /// <summary>
        /// Ruft das Event "BeforeValidation" auf
        /// </summary>
        /// <param name="ui">Das Element, auf welcher diese Geste validiert wird</param>
        /// <param name="points">Die Berührungspunkte, die für diese Geste einfluss haben</param>
        protected virtual void OnBeforeValidation(UIElement ui, TouchGroup points)
        {
            if (BeforeValidation != null)
                BeforeValidation(this, new GestureValidationEventArgs(false, false, ui, points, this));
        }
        /// <summary>
        /// Ruft das Event "AfterValidation" auf
        /// </summary>
        /// <param name="ui">Das Element, auf welcher diese Geste validiert wird</param>
        /// <param name="points">Die Berührungspunkte, die für diese Geste einfluss haben</param>
        protected virtual void OnAfterValidation(UIElement ui, TouchGroup points, bool valid)
        {
            if (AfterValidation != null)
                AfterValidation(this, new GestureValidationEventArgs(true, valid, ui, points, this));
        }
        #endregion
        #region Validation
        /// <summary>
        /// Validiert diese Geste. Gibt True zurück, falls die Anforderungen erfüllt wurden.
        /// </summary>
        /// <param name="ui">Das UIElement, auf welchem diese Geste validiert wird</param>
        /// <param name="points">Die Berührungspunkte, die für die Validierung betrachtet werden</param>
        /// <returns>TRUE wenn die Geste erkannt wurde, sonst FALSE</returns>
        public bool Validate(UIElement ui, TouchGroup points)
        {
            if (!IsActive)
                return false;
            else
            {
                OnBeforeValidation(ui, points);
                bool valid = InternalValidation(ui, points);
                OnAfterValidation(ui, points, valid);
                return valid;
            }
        }
        /// <summary>
        /// Validiert diese Geste. Kann von erbenden Klassen implementiert werden, um verschiedene Gesten zu implementieren
        /// </summary>
        /// <param name="ui">Das UIElement, auf welchem diese Geste validiert wird</param>
        /// <param name="points">Die Berührungspunkte, die für die Validierung betrachtet werden</param>
        /// <returns>TRUE wenn die Geste erkannt wurde, sonst FALSE</returns>
        protected abstract bool InternalValidation(UIElement ui, TouchGroup points);
        #endregion
       

    }
}
