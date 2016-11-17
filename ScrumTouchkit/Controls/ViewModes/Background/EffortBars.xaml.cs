using ScrumTouchkit.Data.Effort;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ScrumTouchkit.Controls.ViewModes.Background
{
    /// <summary>
    /// Interaktionslogik für Effort.xaml
    /// </summary>
    public partial class EffortBars : UserControl
    {
        #region vars, getter, setter
        private List<EffortBar> bars = new List<EffortBar>();
        private ScrumSurface surface;
        #endregion
        #region Constructor
        public EffortBars(ScrumSurface sur)
        {
            surface = sur;
            InitializeComponent();
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            InitBars();
            surface.SizeChanged += Effort_SizeChanged;
        }
        #endregion
        #region Event Listener
        void Effort_Loaded(object sender, RoutedEventArgs e)
        {
            SetSizes();
        }

        void Effort_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetSizes();
        }
        #endregion
        #region Bars
        /// <summary>
        /// Zeigt die 9 Spalten an
        /// </summary>
        public void InitBars()
        {
            for (int i = 0; i < 9; i++)
            {
                EffortBar ebox = new EffortBar();
                ebox.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                ebox.SetEffort(EffortPoints.PreDefined[i].Copy());
                bars.Add(ebox);
                stack.Children.Add(ebox);
            }
            SetSizes();
        }
        /// <summary>
        /// Passt die Größe der Spalten der Bildschirmgröße an
        /// </summary>
        public void SetSizes()
        {
            double width = surface.ActualWidth / 9;

            for (int i = 0; i < 9; i++)
            {
                bars[i].Width = width;
                bars[i].Height = surface.ActualHeight;
            }
        }
        #endregion
    }
}
