using ScrumTouchkit.Controls.Abstract;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScrumTouchkit.Controls.Content.Lines
{
    /// <summary>
    /// Eine ConnectionLine ist eine Verbindung zwischen zwei SurfaceObjects,
    /// die auch bei der Bewegung der beiden die Verbindung darstellt.
    /// </summary>
    public class ConnectionLine : IDisposable
    {
        #region vars, getter, setter

        /// <summary>
        /// Das erste Objekt, das verbunden wird
        /// </summary>
        public SurfaceObject ControlA
        {
            get;
            private set;
        }

        /// <summary>
        /// Das zweite Objekt, das verbunden wird
        /// </summary>
        public SurfaceObject ControlB
        {
            get;
            private set;
        }

        /// <summary>
        /// Die Linie, die angezeigt wird
        /// </summary>
        public Line Line
        {
            get;
            private set;
        }

        /// <summary>
        /// Die ScrumSurface, auf der alles dargestellt wird
        /// </summary>
        public ScrumSurface Surface
        {
            get;
            private set;
        }
        #endregion
        #region Constructor
        public ConnectionLine(SurfaceObject controlA, SurfaceObject controlB)
        {
            this.ControlA = controlA;
            this.ControlB = controlB;
            this.Surface = this.ControlA.Surface;
            InitLine();
            InitEvents();
            UpdateLine();
        }

        /// <summary>
        /// Initialisiert die Linie
        /// </summary>
        protected virtual void InitLine()
        {
            this.Surface.Dispatcher.Invoke((Action)(() =>
                {
                    Line = new Line();
                    Line.StrokeThickness = 2;
                    Line.Stroke = new SolidColorBrush(Controls.Style.Colors.SolidLine);
                    Surface.Children.Add(Line);
                    Panel.SetZIndex(Line, -1);
                    Animation.Animator.FadeIn(Line);
                }));
        }
        protected virtual void InitEvents()
        {
            this.ControlA.Moved += ControlA_Moved;
            this.ControlB.Moved += ControlB_Moved;
        }

        /// <summary>
        /// Entfernt die Linie
        /// </summary>
        public virtual void RemoveLine()
        {
            this.Surface.Dispatcher.Invoke((Action)(() =>
                {
                    Surface.Children.Remove(Line);
                }));
        }
        #endregion
        #region Event Handler
        protected virtual void ControlB_Moved(object sender, Events.MovedEventArgs e)
        {
            //Aktualisierung der Positionen
            UpdateLine(this.ControlA.CenterX, e.X,  this.ControlA.CenterY, e.Y);
        }

        protected virtual void ControlA_Moved(object sender, Events.MovedEventArgs e)
        {
            //Aktualisierung der Positionen
            UpdateLine(e.X, this.ControlB.CenterX, e.Y, this.ControlB.CenterY);
        }
        #endregion
        #region UpdateLine
        /// <summary>
        /// Passt die Positionen der Linie den Positionen der SurfaceObjects an
        /// </summary>
        public virtual void UpdateLine()
        {
            UpdateLine(this.ControlA.CenterX, this.ControlB.CenterX, this.ControlA.CenterY, this.ControlB.CenterY);
        }
        /// <summary>
        /// Setzt die Endpunkte der Linie auf die angegebenen Punkte fest
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        public virtual void UpdateLine(double x1, double x2, double y1, double y2)
        {
            this.Surface.Dispatcher.Invoke(new Action(() =>
            {
                Line.X1 = x1;
                Line.X2 = x2;
                Line.Y1 = y1;
                Line.Y2 = y2;
            }));
        }
        #endregion
        #region Disposable
        public void Dispose()
        {
            ControlA.Moved -= ControlA_Moved;
            ControlB.Moved -= ControlB_Moved;
            RemoveLine();
        }
        #endregion
    }
}
