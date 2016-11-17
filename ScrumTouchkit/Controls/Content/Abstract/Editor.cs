using ScrumTouchkit.Controls.Network;
using ScrumTouchkit.Events;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ScrumTouchkit.Controls.Content.Abstract
{
    /// <summary>
    /// Grundklasse für die Editoren von Epics und User Stories
    /// </summary>
    public abstract class Editor : UserControl, IContent
    {
        #region Textbox Registration
        /// <summary>
        /// Alle Textfelder, die dargestellt werden und deren ID
        /// Mithilfe der ID kann bei der Übertragung der Livedarstellung
        /// schnell das Zieltextfeld erkannt werden
        /// </summary>
        protected Dictionary<int, Network.NetworkTextbox> editorFields =
               new Dictionary<int, NetworkTextbox>();

        /// <summary>
        /// Registriert eine neue Textbox und fügt die passenden Eventlistener hinzu
        /// </summary>
        /// <param name="textBoxType"></param>
        /// <param name="textBox"></param>
        protected void RegisterTextBox(int textBoxType, Network.NetworkTextbox textBox)
        {
            textBox.TextBoxType = textBoxType;
            editorFields.Add(textBoxType, textBox);
            textBox.StateChanged += textBox_StateChanged;
        }
        #endregion
        #region TextBoxChange events
        public event EventHandler<TextBoxStateEventArgs> StateChanged;

        
        private void textBox_StateChanged(object sender, Events.TextBoxStateEventArgs e)
        {
            e.State.TextBoxType = (sender as Network.NetworkTextbox).TextBoxType;
            if (StateChanged != null)
                StateChanged(this, e);
        }
        #endregion
        #region Updates
        /// <summary>
        /// Inhalte aus dem Netzwerk an die entsprechende textbox weiterleiten
        /// </summary>
        /// <param name="state"></param>
        public void ReceiveLiveData(TextBoxState state)
        {
            Network.NetworkTextbox ntb = editorFields[state.TextBoxType];
            if (ntb != null)
                ntb.UpdateState(state);
        }

        /// <summary>
        /// Alle Textfelder auf readonly setzen
        /// </summary>
        /// <param name="readOnly"></param>
        public void SetReadOnly(bool readOnly)
        {
            foreach (Network.NetworkTextbox ntb in editorFields.Values)
            {
                ntb.SetReadOnly(readOnly);
            }
        }
        #endregion

        #region Abstracts
        public abstract void ReadItem(Data.ItemBase item);
        public abstract void UpdateData(Data.ItemBase item);
        #endregion
    }
}
