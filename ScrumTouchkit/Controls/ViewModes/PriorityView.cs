using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Utilities;
using ScrumTouchkit.Data;

namespace ScrumTouchkit.Controls.ViewModes
{
    /// <summary>
    /// Prioritätsansicht.
    /// In dieser Ansicht werden nur sichtbare User Stories angezeigt,
    /// durch das Verschieben dieser kann deren Priorität angepasst werden
    /// </summary>
    public class PriorityView : BaseView
    {
        #region vars, getter, setter
        public override string Name
        {
            get
            {
                return "Priority";
            }
        }
        #endregion
        #region Constructor
        public PriorityView(ScrumSurface surface, int viewID)
            : base(surface, viewID)
        {
            this.settingsLoader = new SettingsLoader.PriorityLoader(this);
            this.allowControls[typeof(Epic)] = 0;
            this.freeMovement = false;
        }
        #endregion
        #region Items
        protected override void ShowControl(ItemControl control)
        {
            base.ShowControl(control);
            ((UserStoryControl)control).CrossVisible = true;
        }
        protected override void DisableControl(ItemControl control)
        {
          base.DisableControl(control);
          ((UserStoryControl)control).CrossVisible = false;
        }

        /// <summary>
        /// Nach dem die DisplaySettings geladen wurden, ein paar
        /// Event Listener erstellen
        /// </summary>
        /// <param name="control"></param>
        public override void AfterSettingsLoaded(ItemControl control)
        {
            DisplaySettings.DisplaySettings ds = control.DisplaySettings.CurrentDisplaySettings;
            if (!ds.Initialized)
            {
                control.Moved += control_Moved;
                control.Item.ExternalDataChanged += Item_ExternalDataChanged;
                ds.Initialized = true;
            }
        }

        /// <summary>
        /// Item neuladen, wenn es geändert wurde (Priorität könnte sich geändert haben!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_ExternalDataChanged(object sender, EventArgs e)
        {
            if (IsActive)
            {
                ItemBase it = sender as ItemBase;
                if (it != null)
                {
                    this.LoadItem(it);
                }
            }
        }

        /// <summary>
        /// Eine UserControl hat sich bewegt -> Priorität anpassen (Entfernung von der Bildschirmmitte)
        /// Außerhalb des größtmöglichen, darstellbaren Kreises ist die Priorität 1!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void control_Moved(object sender, Events.MovedEventArgs e)
        {
            if (IsActive && e.ByUser)
            {
                bool width_smaller = Surface.ActualWidth < Surface.ActualHeight;
                double smallest_side = (width_smaller ? Surface.ActualWidth : Surface.ActualHeight)/2;

                double mid_x = Surface.ActualWidth / 2;
                double mid_y = Surface.ActualHeight / 2;

                Vector mid = new Vector(mid_x, mid_y);
                Vector loc = new Vector(e.X, e.Y);

                double distance = Math.Abs(mid.GetDistance(loc));
                double prio = distance / smallest_side;
                prio = prio > 1 ? 1 : prio;
                prio = Math.Round(prio * 100) / 100;
                ((UserStoryControl)sender).UserStory.Priority = prio;
            }
        }
        #endregion
        #region Activate

        /// <summary>
        /// Hintergrund zeichnen
        /// </summary>
        protected override void InternalActivate()
        {
            bool width_smaller = Surface.ActualWidth < Surface.ActualHeight;
            double smallest_side = width_smaller ? Surface.ActualWidth : Surface.ActualHeight;

            Rectangle background = new Rectangle();

            background.Fill = new SolidColorBrush(Color.FromRgb(154, 255, 166));
            background.StrokeThickness = 1;

            background.Width = Surface.ActualWidth;
            background.Height = Surface.ActualHeight;

            this.AddElementToSurface(background);

            Rectangle circles = new Rectangle();

            circles.Width = smallest_side;
            circles.Height = smallest_side;


            if (width_smaller)
                Canvas.SetTop(circles, (Surface.ActualHeight - smallest_side) / 2);
            else
                Canvas.SetLeft(circles, (Surface.ActualWidth - smallest_side)/2);
        

            RadialGradientBrush brush = new RadialGradientBrush();
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 69, 69), 0));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(252, 255, 123), 0.75));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(154, 255, 166), 1));

            circles.Fill = brush;
            this.AddElementToSurface(circles);

          
        }
        #endregion
     }
}
