using ScrumTouchkit.Controls.Network;
using ScrumTouchkit.Data;
using System;

namespace ScrumTouchkit.Events
{
    /// <summary>
    /// Wird versendet, wenn sich der TextBoxState einer TextBox geändert hat
    /// </summary>
    public class TextBoxStateEventArgs : EventArgs
    {
        /// <summary>
        /// Der neue Zustand der Textbox
        /// </summary>
        public TextBoxState State
        {
            get;
            set;
        }

        /// <summary>
        /// Das Item, zu welchem die TextBox gehört
        /// </summary>
        public ItemBase Data
        {
            get; set;
        }
        public TextBoxStateEventArgs(NetworkTextbox textbox)
        {
            State = new TextBoxState(textbox);
        }
    }
}
