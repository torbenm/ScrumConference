using ScrumTouchkit.Controls.Content.Abstract;
using ScrumTouchkit.Controls.Network;
using ScrumTouchkit.Threading;

namespace ScrumTouchkit.Controls.Content
{
    /// <summary>
    /// Stellt einen Editor für Epics dar
    /// </summary>
    public partial class EpicUI_Editor : Editor
    {
        public EpicUI_Editor()
        {
            InitializeComponent();
            //Registriert die existierenden Textboxen.
            RegisterTextBox(TextBoxState.TITLE, this.Title);
        }

        /// <summary>
        /// Aktualisiert die Inhalte des Items auf die Werte in der Textbox
        /// </summary>
        /// <param name="item"></param>
        public override void ReadItem(Data.ItemBase item)
        {
            this.Invoke(() =>
                {
                    item.Title = this.Title.Text;
                });
        }

        /// <summary>
        /// AKtualisiert die Inhalte der Textboxen auf die Werte in den Items
        /// </summary>
        /// <param name="item"></param>
        public override void UpdateData(Data.ItemBase item)
        {
            this.Invoke((() =>
                {
                    this.Title.Text = item.Title;
                }));
        }
    }
}
