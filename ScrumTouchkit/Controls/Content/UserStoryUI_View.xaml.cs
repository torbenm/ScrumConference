using ScrumTouchkit.Controls.Content.Abstract;
using ScrumTouchkit.Data;
using System.Windows;
using System.Windows.Controls;

namespace ScrumTouchkit.Controls.Content
{
    /// <summary>
    /// Stellt den Inhalt einer User Story an.
    /// Die Summe der EffortPoints ist nur sichtbar, wenn sich die User Story im Sprint Backlog befindet.
    /// </summary>
    public partial class UserStoryUI_View : UserControl, IContent
    {
        public UserStoryUI_View()
        {
            InitializeComponent();
        }

        public void UpdateData(Data.ItemBase data)
        {
            UserStory us = data as UserStory;
            if (us != null)
            {
                this.EffortBox.Text = us.Effort;
                this.DescriptionBox.Text = us.Text;
                this.TitleBox.Text = us.Title;
                EffortSum.Visibility = us.BacklogStatus == ItemBacklogStatus.SPRINT_BACKLOG ? Visibility.Visible : Visibility.Hidden;
                    
            }
        }


    }
}
