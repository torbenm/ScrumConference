using System;

namespace ScrumTouchkit.Controls.Network
{
       
        /// <summary>
        /// Stellt den aktuellen Status einer TextBox dar
        /// Das beinhalet
        /// -- Der aktuelle Text
        /// -- Wie weit in X-Richtung gescrollt wurde
        /// -- Wie weit in Y-Richtung gescrollt wurde
        /// -- Wo die Auswahl beginnt (auch Position des Cursors bei Länge 0)
        /// -- Wie lang die Auswahl von Text ist
        /// </summary>
        public class TextBoxState
        {
            /// <summary>
            /// TextBoxType für den Titel
            /// </summary>
            public const int TITLE = 0;
            /// <summary>
            /// TextBoxType für die Beschreibung
            /// </summary>
            public const int DESCRIPTION = 1;

            /// <summary>
            /// Inhalt der Textbox
            /// </summary>
            public String Text
            {
                get;
                set;
            }

            /// <summary>
            /// Um welche Textbox es sich handelt (TITLE oder DESCRIPTION)
            /// </summary>
            public int TextBoxType
            {
                get;
                set;
            }

            /// <summary>
            /// Scrollweite in X-Richtung
            /// </summary>
            public double VerticalOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Scrollweite in Y-Richtung
            /// </summary>
            public double HorizontalOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Beginn der Auswahl (oder Position des Cursors bei SelectionLength = 0)
            /// </summary>
            public int SelectionStart
            {
                get;
                set;
            }
            /// <summary>
            /// Länge der Auswahl
            /// </summary>
            public int SelectionLength
            {
                get;
                set;
            }
            public TextBoxState() { }

            /// <summary>
            /// Initialisiert ein neues TextBoxState-Objekt mit den Werten einer gegebenen NetworkTextbox
            /// </summary>
            /// <param name="textbox"></param>
            public TextBoxState(NetworkTextbox textbox)
            {
                this.Text = textbox.Text;
                this.HorizontalOffset = textbox.HorizontalOffset;
                this.VerticalOffset = textbox.VerticalOffset;
                this.SelectionStart = textbox.SelectionStart;
                this.SelectionLength = textbox.SelectionLength;
            }

            /// <summary>
            /// Setzt die Darstellung einer NetzworkTextBox auf die in diesem Objekt
            /// gespeicherten Werte
            /// </summary>
            /// <param name="textbox"></param>
            public void Apply(NetworkTextbox textbox)
            {
                textbox.Text = this.Text;
                textbox.ScrollToHorizontalOffset(this.HorizontalOffset);
                textbox.ScrollToVerticalOffset(this.VerticalOffset);
                textbox.Select(this.SelectionStart, this.SelectionLength);
            }
        }
}
