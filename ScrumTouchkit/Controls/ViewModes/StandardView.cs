using ScrumTouchkit.Data;
using ScrumTouchkit.Threading;
using System;
using System.Windows;
using ScrumTouchkit.Utilities;
using ScrumGestures.Gestures;
using ScrumGestures;
using System.Windows.Controls;
using System.Windows.Media;
using ScrumTouchkit.Events;

namespace ScrumTouchkit.Controls.ViewModes
{

    /// <summary>
    /// Standardansicht.
    /// Hier sind Epics nicht sichtbar, User Stories schon.
    /// Keine Hintergrunddarstellung. 
    /// Durch antippen von User Stories wechseln diese in/aus dem Sprint Backlog.
    /// Dabei wird in der Mitte die neue Summe an EffortPoints angezeigt.
    /// Weiterhin ist die Sortierung nach der Priorität durch das Zeichnen einer Linie möglich.
    /// </summary>
    public class StandardView : BaseView
    {
        #region vars, getter, setter
        public GestureHandler prioSorting;
        private Background.EffortPointsBlendIn blendIn;

        public override string Name
        {
            get
            {
                return "Standard";
            }
        }
        #endregion
        #region Constructor
        public StandardView(ScrumSurface surface, int viewID)
            : base(surface, viewID)
        {
            prioSorting = surface.GestureManager.AddGesture(surface, DefinedGestures.Line, LineCallback);
            prioSorting.IsActive = false;
            
            InitBlendIn();
        }
        #endregion
        #region Activate / Deactivate
        /***
         *  Beschreibungen zu diesen Funktionen in der Klasse BaseView
         **/
        protected override void InternalActivate()
        {
            base.InternalActivate();
            prioSorting.IsActive = true;

        }
        protected override void InternalDeactivate()
        {
            base.InternalDeactivate();
            prioSorting.IsActive = false;
        }

        protected override void ShowControl(Abstract.ItemControl control)
        {
            base.ShowControl(control);
            if(control.GetType() == typeof(UserStoryControl))
                control.SetGestureActive(UserStoryControl.TOGGLE_BACKLOG_GESTURE, true);
        }

        protected override void DisableControl(Abstract.ItemControl control)
        {
            base.DisableControl(control);
            if (control.GetType() == typeof(UserStoryControl))
                control.SetGestureActive(UserStoryControl.TOGGLE_BACKLOG_GESTURE, false);
        }
        #endregion
        #region EffortBlendIn
        /// <summary>
        /// Initiiert das Einblenden der Summe der EffortPoints
        /// </summary>
        private void InitBlendIn()
        {
            blendIn = new Background.EffortPointsBlendIn();
            Surface.Database.EffortSum.EffortPointsChanged += EffortSum_EffortPointsChanged;
            
        }
        /// <summary>
        /// Wird aufgerufen wenn sich die Summe der EffortPoints geändert hat.
        /// Blendet dann in der Mitte des DIsplays die neue Summe ein.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EffortSum_EffortPointsChanged(object sender, EffortPointsChangedEventArgs e)
        {
            if (IsActive)
            {
                blendIn.Invoke(() =>
                {
                    blendIn.Text.Text = e.New;
                    //Aktuell: Zufälliger Winkel 
                    ShowBlendIn(MathHelper.Random.Next(365));
                });
            }
        }

        /// <summary>
        /// Blendet die SUmme der EffortPoints in dem angegebenen Winkel ein
        /// </summary>
        /// <param name="angle">Der Winkel, in welchen die Effort Points gedreht werden soll</param>
        private void ShowBlendIn(double angle)
        {
            if(this._elements.Contains(blendIn))
                RemoveElement(blendIn);
            Canvas.SetLeft(blendIn, (Surface.ActualWidth - blendIn.Width) / 2);
            Canvas.SetTop(blendIn, (Surface.ActualHeight - blendIn.Height) / 2);
            AddElementToSurface(blendIn, 400);
            
            TransformGroup tg = new TransformGroup();
            ScaleTransform st = new ScaleTransform(1, 1);
            st.CenterX = blendIn.Width / 2;
            st.CenterY = blendIn.Height / 2;
            RotateTransform rt = new RotateTransform(angle);
            rt.CenterX = blendIn.Width / 2;
            rt.CenterY = blendIn.Height / 2;
            tg.Children.Add(st);
            tg.Children.Add(rt);

            blendIn.RenderTransform = tg;
            Animation.Animator.Resize(st, 3, TimeSpan.FromMilliseconds(1500));
            Animation.Animator.FadeOut(blendIn, TimeSpan.FromMilliseconds(1500));
        }
        #endregion
        #region Gestures
        /// <summary>
        /// Wird aufgerufen, wenn eine Linien-Geste erkannt wurde
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        private void LineCallback(UIElement element, ScrumGestures.TouchGroup points)
        {
            TouchPoint tp = points[0];
            //We need at least 2 points
            this.Surface.Dispatcher.Invoke(new Action(() =>
            {
                this.SortByPriority(tp.StartPoint, VectorExtension.FromPoints(tp.StartPoint, tp.CurrentPoint));
            }));
        }

        /// <summary>
        /// Sortiert alle angezeigten UserStories nach der Priorität und 
        /// zeigt sie auf der gezeichneten Linie an
        /// </summary>
        /// <param name="startingPoint">Der Anfangspunkt der Linie</param>
        /// <param name="direction">Die Richtung der Linie</param>
        public void SortByPriority(Point startingPoint, Vector direction)
        {
            //User Stories nach Priorität sortieren
            Surface.Database.UserStories.Sort(
                delegate(UserStory item1, UserStory item2)
                {
                    return item2.Priority.CompareTo(item1.Priority);
                });
            Point current = startingPoint;
            double angle = new Vector(1, 0).GetAngle(direction);
            for (int i = 0; i < Surface.Database.UserStories.Count; i++)
            {
                UserStory item = Surface.Database.UserStories[i];
                UserStoryControl obj = item.Representations[0] as UserStoryControl;
                if (obj != null)
                {
                    Canvas.SetZIndex(obj, 10 + i);
                    obj.Scale(UserStoryControl.StaticDefaultSettings.Scale, true);
                    obj.Rotate(angle, true);
                    obj.MoveCenter(current.X, current.Y, true);
                    //Jede User Story ein bisschen weiter in die Richtung des Vektors bewegen
                    current = direction.MovePoint(current, UserStoryControl.StdWidth * UserStoryControl.StaticDefaultSettings.Scale / 2);
                }

            }
        }
        #endregion
    }
}
