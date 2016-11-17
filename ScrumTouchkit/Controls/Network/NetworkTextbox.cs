using ScrumTouchkit.Events;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScrumTouchkit.Controls.Network
{
    /// <summary>
    /// Eine Textbox, die die Liveübertragung seiner Darstellung über das Netzwerk ermöglicht
    /// </summary>
    public class NetworkTextbox : TextBox
    {

        private bool processingExternal = false;

        /// <summary>
        /// <see cref="ScrumTouchkit.Controls.Network.TextBoxState"/>
        /// TITLE oder DESCRIPTION
        /// </summary>
        public int TextBoxType
        {
            get;
            set;
        }

        public NetworkTextbox()
        {         
            this.SelectionChanged += NetworkTextbox_SelectionChanged;
            this.TextChanged += NetworkTextbox_TextChanged;
            this.TextWrapping = System.Windows.TextWrapping.Wrap;
        }

        #region EventHandling
        void NetworkTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangeState();
        }

        void NetworkTextbox_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeState();
        }
        #endregion
        #region Events
        public event EventHandler<TextBoxStateEventArgs> StateChanged;
        private void ChangeState()
        {
            if (processingExternal || this.IsReadOnly)
                return;
            if(StateChanged != null)
                StateChanged(this, new TextBoxStateEventArgs(this));
        }

        #endregion
        #region Updates
        /// <summary>
        /// Aktualisiert den Inhalt und Darstellung anhand eines TextBoxStates
        /// </summary>
        /// <param name="state"></param>
        public void UpdateState(TextBoxState state)
        {
            processingExternal = true;
            state.Apply(this);
            processingExternal = false;
        }

        /// <summary>
        /// Setzt die Textbox auf Readonly
        /// </summary>
        /// <param name="readOnly"></param>
        public void SetReadOnly(bool readOnly)
        {
            if (readOnly)
                this.Background = new SolidColorBrush(Controls.Style.Colors.ReadOnlyBGColor);
            else
                this.Background = new SolidColorBrush(Controls.Style.Colors.EditModeBGColor);
            this.IsReadOnly = readOnly;
        }
        #endregion

    }
}
