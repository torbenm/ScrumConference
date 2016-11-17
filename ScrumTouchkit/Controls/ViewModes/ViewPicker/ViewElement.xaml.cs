using System;
using System.Windows.Controls;

namespace ScrumTouchkit.Controls.ViewModes.ViewPicker
{
    /// <summary>
    /// Stellt ein Element zur Anzeige einer Ansichts-auswahl im ViewPicker an
    /// </summary>
    public partial class ViewElement : UserControl
    {
        /// <summary>
        /// Titel der Ansicht
        /// </summary>
        public String Text
        {
            get { return textField.Text; }
            set { textField.Text = value; }
        }
        public ViewElement()
        {
            InitializeComponent();
        }
    }
}
