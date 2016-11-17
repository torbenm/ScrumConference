namespace ScrumGestures
{
    /// <summary>
    /// Wird von Klassen implementiert, die Gestensteuerung unterstützen
    /// </summary>
    public interface IHasGestures
    {
        /// <summary>
        /// Initialisiert die Gesten, wird meistens kurz nach der Konstruktion aufgerufen
        /// </summary>
        void InitializeGestures();
        /// <summary>
        /// Entfernt alle Gestenverknüpfungen, wird aufgerufen, wenn ein Objekt zerstört wird
        /// </summary>
        void RemoveGestures();
    }
}
