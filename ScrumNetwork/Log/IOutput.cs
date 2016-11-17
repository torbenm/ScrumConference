namespace ScrumNetwork.Log
{
    /// <summary>
    /// Stellt ein Interface zur Ausgabe von Log-Nachrichten dar
    /// </summary>

    public interface IOutput
    {
        /// <summary>
        /// Wird beim Starten des Programmes aufgerufen
        /// </summary>
        void StartLogging();

        /// <summary>
        /// Schreibt einen Text (ohne Zeilenumbruch am Ende) in den Log
        /// </summary>
        /// <param name="text">Der Text</param>
        void Write(string text);

        /// <summary>
        /// Schreibt einen Text (mit Zeilenumbruch am Ende) in den Log
        /// </summary>
        /// <param name="text">Der Text</param>
        void WriteLine(string text);

        /// <summary>
        /// Wird beim Beenden des Programmes aufgerufen, um z.B. die geöffnete
        /// Datei zu schließen.
        /// </summary>
        void EndLogging();
    }
}
