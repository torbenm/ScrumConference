using ScrumTouchkit.Controls.Dialogs;
using System;
using System.Net;

namespace ScrumTouchkit.Events
{
    /// <summary>
    /// Wird beim Schließen eines Dialogs übergeben
    /// und beinhaltet den Status des geschlossenen Dialogs
    /// </summary>
    public class DialogEventArgs : EventArgs
    {
        /// <summary>
        /// Der Dialog
        /// </summary>
        public Dialog Dialog
        {
            get;
            set;
        }
        /// <summary>
        /// Je nach Dialogtyp können verschiedene ExitModes
        /// vergeben werden
        /// (z.B. "OK" oder "CANCEL")
        /// </summary>
        public int ExitMode
        {
            get;
            set;
        }
        /// <summary>
        /// [Optional] Wird nur vom Verbindungsherstellen Dialog benutzt
        /// Beinhaltet den Port, der angegeben wurde
        /// </summary>
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// [Optional] Wird nur vom Verbindungsherstellen Dialog benutzt
        /// Beinhaltet die IP-Adresse, die angegeben wurde
        /// </summary>
        public IPAddress IP
        {
            get;
            set;
        }
        /// <summary>
        /// Initialisiert ein neues DialogEventArgs für beliebige Dialoge
        /// </summary>
        /// <param name="mode">Gibt an, mit welcher Aktion der Dialog geschlossen wurde</param>
        /// <param name="dialog">Der entsprechende Dialog</param>
        public DialogEventArgs(int mode, Dialog dialog)
        {
            ExitMode = mode;
            Dialog = dialog;
        }

        /// <summary>
        /// Initialisiert neue DialogEventArgs für den Verbindungsherstellen Dialog
        /// </summary>
        /// <param name="dialog">Das Dialog</param>
        /// <param name="mode">Die Aktion, mitwelcher der Dialog geschlossen wurde</param>
        /// <param name="port">Der im Dialog angegebene Port</param>
        /// <param name="ip">Die im Dialog angegebene IP-Adresse</param>
        public DialogEventArgs(Dialog dialog, int mode, int port, IPAddress ip)
        {
            this.Dialog = dialog;
            this.ExitMode = mode;
            this.Port = port;
            this.IP = ip;
        }
    }
}
