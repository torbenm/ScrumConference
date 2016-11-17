namespace ScrumGestures.DragDrop
{
    /// <summary>
    /// Wird von den Klassen implementiert, auf die ein IDraggable-Objekt gezogen werden kann
    /// </summary>
    public interface IDropContainer
    {
        /// <summary>
        /// Wird aufgerufen, wenn ein Objekt über den IDropContainer gezogen wurde, aber noch nicht losgelassen wurde.
        /// Somit können bereits Andeutungen über die beim Loslassen ausgeführte Funktion angezeigt werden.
        /// </summary>
        /// <param name="obj">Das IDraggable-Objekt, das über den IDropContainer gezogen wurde.</param>
        /// <param name="pt">Der aktuelle Berührungspunkt auf dem IDraggable</param>
        void NotifyDragEnter(IDraggable obj, TouchPoint pt);
        /// <summary>
        /// Wird aufgerufen, wenn ein Objekt aus dem IDropContainer gezogen wurden.
        /// </summary>
        /// <param name="obj">Das IDraggable-Objekt, das über den IDropContainer gezogen wurde.</param>
        /// <param name="pt">Der aktuelle Berührungspunkt auf dem IDraggable</param>
        void NotifyDragExit(IDraggable obj, TouchPoint pt);
        /// <summary>
        /// Wird aufgerufen, wenn ein Objekt über dem IDropContainer losgelassen wrude.
        /// </summary>
        /// <param name="obj">Das IDraggable-Objekt, das losgelassen wurde</param>
        /// <param name="pt">Der Berührungspunkt des IDraggable-Objektes</param>
        void NotifyDragDrop(IDraggable obj, TouchPoint pt);
    }
}
