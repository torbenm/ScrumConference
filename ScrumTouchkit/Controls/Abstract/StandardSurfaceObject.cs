using System;
using ScrumTouchkit.Threading;
using System.Windows;
using ScrumGestures.Gestures;
using ScrumGestures;
using ScrumTouchkit.Utilities;
using ScrumTouchkit.Controls.Animation;

namespace ScrumTouchkit.Controls.Abstract
{

    /// <summary>
    /// Diese Klasse erweitert das SurfaceObject
    /// um einige häufige Funktionen ,
    /// wie die Gesten zum Bewegen, Drehen und Skalieren
    /// </summary>
    public class StandardSurfaceObject : SurfaceObject
    {
        #region Initialize & Constructor
        public StandardSurfaceObject(ScrumSurface surface)
            : base(surface)
        {

            MaxHeight = 1000;
            MaxWidth = 1000;
            MinHeight = 40;
            MinWidth = 40;
            Left = 0;
            Top = 0;
            Width = 10;
            Height = 10;
        
        }
        #endregion
        #region Move Methods
        /// <summary>
        /// Bewegt den Mittelpunkt des Objektes zu der angegebenen Position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="animate">TRUE, wenn die Bewegung zu dieser Position animiert werden soll</param>
        /// <param name="byUser">TRUE, wenn diese AKtion durch den Nutzer (als Geste) ausgeführt wird</param>
        public void MoveCenter(double x, double y, bool animate = false, bool byUser = false)
        {
            if (animate)
            {
                this.Invoke(new Action(() =>
                {
                    Point pt = ValidatePosition(x, y);
                    this.AnimMove(new Point(pt.X - this.ActualWidth / 2, pt.Y - this.ActualHeight / 2));
                    OnMoved(pt.X, pt.Y);
                }));
            }
            else
                SetCenter(x, y, byUser);
        }
        /// <summary>
        /// Bewegt den Mittelpunkt des Objektes um einen angegebenen Vektor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="animate">TRUE, wenn die Bewegung zu dieser Position animiert werden soll</param>
        /// <param name="byUser">TRUE, wenn diese AKtion durch den Nutzer (als Geste) ausgeführt wird</param>
        public void MoveBy(double x, double y, bool animate = false, bool byUser = false)
        {
            this.Invoke(new Action(() =>
            {
                MoveCenter(CenterX + x, CenterY + y, animate, byUser);
            }));
        }
        #endregion
        #region Rotate, Scale
        /// <summary>
        /// Rotiert das Objekt zu dem angegebenen Winkel
        /// </summary>
        /// <param name="toAngle"></param>
        /// <param name="animate"></param>
        public void Rotate(double toAngle, bool animate = false)
        {
            if (animate)
                this.AnimRotate(toAngle);
            else
                this.RotateAngle = toAngle;
        }
        /// <summary>
        /// Skaliert das Objekt auf den angegebenen Faktor
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="animate"></param>
        public void Scale(double factor, bool animate = false)
        {
            if (animate)
                this.AnimScale(factor);
            else
                this.ScaleFactor = factor;
        }
        #endregion
        #region Gesture Callbacks
        public const string DRAG_GESTURE = "drag";
        public const string RESIZE_GESTURE = "resize";
        public const string ROTATE_GESTURE = "rotate";
        public const string FINGERS_CHANGED_GESTURE = "fingerschanged";

        public override void InitializeGestures()
        {
            this.AddGesture(FINGERS_CHANGED_GESTURE, DefinedGestures.FingerCountChanged, FingerCountChangedCallback);
            this.AddGesture(DRAG_GESTURE, DefinedGestures.DragAndDrop, DragCallback);
            this.AddGesture(ROTATE_GESTURE, DefinedGestures.Rotate, RotateCallback);
            this.AddGesture(RESIZE_GESTURE, DefinedGestures.Resize, ResizeCallback);
        }

        
        protected virtual void FingerCountChangedCallback(UIElement element, TouchGroup points)
        {
            // Jede Berührung holt das Objekt zunächst nach vorne
            BringToFront();
        }

        protected virtual void DragCallback(UIElement element, TouchGroup points)
        {
            RemoveMovingAnimation();
            MoveBy(points[0].PositionChange.X, points[0].PositionChange.Y, byUser: true);
        }
        protected virtual void ResizeCallback(UIElement element, TouchGroup points)
        {
            RemoveResizeAnimation();

            Vector vec1 = VectorExtension.FromPoints(points[0].CurrentPoint, points[1].CurrentPoint);
            Vector vec2 = VectorExtension.FromPoints(points[0].PreviousPoint, points[1].PreviousPoint);

            double distanceChange = vec1.Length - vec2.Length;
            
            double w = this.ScaledWidth + distanceChange;
            double h = this.ScaledHeight + distanceChange;

            //Neue Größe festlegen. Die aufgerufene Methode passt den Skalierungsfaktor der Größe an
            this.SetSize(w, h); 
        }

        protected bool rotateInProgress = false;
        protected virtual void RotateCallback(UIElement element, TouchGroup points)
        {
            RemoveRotateAnimation();

            Vector vec1 = VectorExtension.FromPoints(points[0].CurrentPoint, points[1].CurrentPoint);
            Vector vec2 = VectorExtension.FromPoints(points[0].PreviousPoint, points[1].PreviousPoint);

            double slopeChanged = vec2.GetAngle(vec1);
            if (!rotateInProgress && slopeChanged != 0)
            {
                //Neuen Winkel festlegen durch Addition
                this.RotateAngle += slopeChanged;
                rotateInProgress = false;
            }
        }
        #endregion
    }
}
