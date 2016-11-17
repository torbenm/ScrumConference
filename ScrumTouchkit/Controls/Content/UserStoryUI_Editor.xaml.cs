using ScrumTouchkit.Controls.Network;
using ScrumTouchkit.Data;
using ScrumTouchkit.Threading;

namespace ScrumTouchkit.Controls.Content
{
    /// <summary>
    /// Stellt einen Editor für User Stories dar.
    /// </summary>
    public partial class UserStoryUI_Editor : Abstract.Editor
    {
        public UserStoryUI_Editor()
        {
            InitializeComponent();
            //Registriert die beiden Textfelder
            RegisterTextBox(TextBoxState.TITLE, this.TitleBox);
            RegisterTextBox(TextBoxState.DESCRIPTION, this.DescriptionBox);
        }

        /// <summary>
        /// Aktualisiert die Werte des items auf die Werte in den Textboxen
        /// </summary>
        /// <param name="item"></param>
        public override void ReadItem(Data.ItemBase item)
        {
            this.Invoke(() =>
                {
                    UserStory us = item as UserStory;
                    if (us != null)
                    {
                        us.Title = this.TitleBox.Text;
                        us.Text = this.DescriptionBox.Text;
                    }
                });
        }

        /// <summary>
        /// Aktualisiert die Darstellung des Editors
        /// </summary>
        /// <param name="item"></param>
        public override void UpdateData(Data.ItemBase item)
        {
            UserStory us = item as UserStory;
            if (us != null)
            {
                this.TitleBox.Text = us.Title;
                this.DescriptionBox.Text = us.Text;
                this.EffortBox.Text = us.Effort;
            }
        }
    }
}
