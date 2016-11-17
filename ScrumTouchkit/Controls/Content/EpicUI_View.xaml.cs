using ScrumTouchkit.Controls.Content.Abstract;
using System.Windows.Controls;

namespace ScrumTouchkit.Controls.Content
{
    /// <summary>
    /// In Objekten dieser Klasse werden die Inhalte von Epics dargestellt.
    /// </summary>
    public partial class EpicUI_View : UserControl, IContent
    {
        public EpicUI_View()
        {
            InitializeComponent();
        }

        public void UpdateData(Data.ItemBase data)
        {
            this.Title.Text = data.Title;
        }
    }
}
