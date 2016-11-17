using ScrumGestures.DragDrop;
using ScrumTouchkit.Data.Effort;
using ScrumTouchkit.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ScrumTouchkit.Controls.ViewModes.Background
{
    /// <summary>
    /// Stellt eine Spalte zur Abschätzung des Aufwands dar
    /// </summary>
    public partial class EffortBar : UserControl, ScrumGestures.DragDrop.IDropContainer
    {
        #region vars, getter, setter
        /// <summary>
        /// Der Aufwand, der dieser Spalte zugewiesen ist
        /// </summary>
        public EffortPoints Effort
        {
            get;
            set;
        }
        #endregion
        #region Constructor
        public EffortBar()
        {
            
            InitializeComponent();
            this.SizeChanged += EffortBox_SizeChanged;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

        }
        #endregion
        #region Appearance
        /// <summary>
        /// Legt den Aufwand, der dieser Spalte zugewiesen ist,
        /// auf den angegebenen Wert fest.
        /// </summary>
        /// <param name="ep"></param>
        public void SetEffort(EffortPoints ep)
        {
            this.Effort = ep;
            txt_top.Text = ep;
            txt_bottom.Text = ep;
        }

        void EffortBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            vbox_top.Margin = new Thickness(20, -e.NewSize.Height / 3, 20, 0);
            vbox_bottom.Margin = new Thickness(20, e.NewSize.Height / 3, 20, 0);
        }
        #endregion
        #region Drag and Drop
        public void NotifyDragEnter(IDraggable obj, ScrumGestures.TouchPoint pt)
        {
            this.Invoke(() =>
                {
                    this.Effect = Controls.Style.StyleHelper.GetOuterGlow(Controls.Style.Colors.ObjectGlowShared);
                });
        }

        public void NotifyDragExit(IDraggable obj, ScrumGestures.TouchPoint pt)
        {
            this.Invoke(() =>
            {
                this.Effect = null;
            });
        }

        public void NotifyDragDrop(IDraggable obj, ScrumGestures.TouchPoint pt)
        {
            ((UserStoryControl)obj).UserStory.Effort.Value = this.Effort.Value;
            ((UserStoryControl)obj).UserInterface.UpdateData(((UserStoryControl)obj).UserStory);
            this.Invoke(() =>
            {
                this.Effect = null;
            });
        }
        #endregion
    }
}
