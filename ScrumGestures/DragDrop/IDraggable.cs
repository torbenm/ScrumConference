namespace ScrumGestures.DragDrop
{
    /// <summary>
    /// Dieses Interface muss von den Klassen implementiert werden,
    /// die einen DragDropController verwenden wollen.
    /// </summary>
    public interface IDraggable
    {
        /// <summary>
        /// Gibt die Mitte des Objektes an der X-Achse zurück.
        /// </summary>
        double CenterX
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gibt die Mitte des Objektes an der Y-Achse zurück.
        /// </summary>
        double CenterY
        { 
            get; 
            set; 
        }
    }
}
