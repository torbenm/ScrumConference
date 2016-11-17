namespace ScrumTouchkit.Controls.Dialogs
{
    /// <summary>
    /// DialogInfo beinhaltet Informationen für 
    /// Nachrichten-Dialoge. Mithilfe dieser Klasse kann somit die angezeigte Nachricht 
    /// festgelegt werden.
    /// </summary>
    public class DialogInfo
    {
        public const int ERROR = 1;
        public const int INFORMATION = 0;

        /// <summary>
        /// Die Nachricht, die angezegit wird
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Der "Modus"
        /// Gibt also an, ob es sich um eine Fehler oder Informationsnachricht handelt
        /// </summary>
        public int Mode
        {
            get;
            set;
        }

        public static DialogInfo CreateMessageInfo(string msg, bool error)
        {
            DialogInfo di = new DialogInfo();
            di.Message = msg;
            di.Mode = error ? ERROR : INFORMATION;
            return di;
        }
    }
}
