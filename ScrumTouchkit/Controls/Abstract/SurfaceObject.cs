using ScrumGestures;
using ScrumGestures.Gestures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScrumTouchkit.Threading;
using ScrumTouchkit.Events;
using ScrumTouchkit.Controls.Animation;

namespace ScrumTouchkit.Controls.Abstract
{
    /// <summary>
    /// Dies ist die nächste Stufe der Hierarchie für Objekte auf der Oberfläche.
    /// In dieser Klasse werden Rahmenfunktionen für Gesten implementiert
    /// sowie ein paar Methoden für Bewegung oder zur Animation.
    /// Zudem wird hier eine Verbindung zur ScrumSurface aufgebaut.
    /// </summary>
    public abstract class SurfaceObject : TransformableControl, IHasGestures
    {
        #region vars, getter, setter
        /// <summary>
        /// Die ScrumSurface, auf welcher das SurfaceObject dargestellt wird
        /// </summary>
        public ScrumSurface Surface
        {
            get;
            private set;
        }

        /// <summary>
        /// Das Root-Element ist das unterste Element in der Darstellung des SurfaceObjects.
        /// In dieses Grid können die tatsächlichen Elemente eingefügt werden.
        /// </summary>
        public Grid Root
        {
            get;
            private set;
        }

        /// <summary>
        /// Alle für dieses SurfaceObject registrierten Gesten
        /// </summary>
        public Dictionary<string, GestureHandler> Gestures
        {
            get;
            private set;
        }

        /// <summary>
        /// Das Standardmäßige UI-Element, welches die Gesten erhält
        /// </summary>
        protected virtual UIElement GestureElement
        {
            get { return Root; }
        }
        /// <summary>
        /// Gibt an, ob die Position bereits initialisiert wurde
        /// </summary>
        public bool IsPositionInitialized
        {
            get;
            private set;
        }

        /// <summary>
        /// TRUE, wenn das SurfaceObject aktuell eine Animation für die Bewegung besitzt.
        /// Diese muss vor einer weiteren Änderung der Position zunächst deaktiviert werden.
        /// </summary>
        public bool HasMovingAnimation
        {
            get;
            set;
        }
        /// <summary>
        /// TRUE, wenn das SurfaceObject aktuell eine Animation für die Skalierung besitzt.
        /// Diese muss vor einer weiteren Änderung der Skalierung zunächst deaktiviert werden.
        /// </summary>
        public bool HasResizeAnimation
        {
            get;
            set;
        }
        /// <summary>
        /// TRUE, wenn das SurfaceObject aktuell eine Animation für die Drehung besitzt.
        /// Diese muss vor einer weiteren Änderung der Drehung zunächst deaktiviert werden.
        /// </summary>
        public bool HasRotateAnimation
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt TRUE an, wenn das SurfaceObject aktuell nicht angezeigt wird
        /// </summary>
        public bool IsHidden
        {
            get;
            private set;
        }
        #endregion
        #region Constructor
        public SurfaceObject(ScrumSurface surface)
        {
            IsPositionInitialized = false;
            HasResizeAnimation = false;
            HasMovingAnimation = false;
            HasRotateAnimation = false;
            IsHidden = true;

            Gestures = new Dictionary<string, GestureHandler>();
            this.Surface = surface;
            
            InitializeComponents();
            InitializeGestures();
            
        }

        /// <summary>
        /// Initialisiert das Root-Grid
        /// </summary>
        private void InitializeComponents()
        {
            Root = new Grid();
            Root.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Root.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Root.IsHitTestVisible = true;
            Root.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            this.AddChild(Root);
            this.Show();
        }
        #endregion
        #region Gestures
        /// <summary>
        /// Initialisiert die Gesten. Wird automatisch vom Konstruktor aufgerufen
        /// </summary>
        public abstract void InitializeGestures();


        /// <summary>
        /// Fügt eine Geste zu den registrierten Gesten hinzu.
        /// Als UIElement für die Geste wird dabei das "GestureELement" gewählt.
        /// </summary>
        /// <param name="identifier">Interner Bezeichner für die Geste</param>
        /// <param name="gesture">Die Geste selbst</param>
        /// <param name="callback">Die Methoden, die bei erkennen der Geste ausgeführt werden soll</param>
        public virtual void AddGesture(string identifier, GestureBase gesture,
            GestureHandler.GestureCallbackHandler callback)
        {
            AddGesture(identifier, GestureElement, gesture, callback);

        }

        /// <summary>
        /// Registriert eine Geste, verwendet dafür aber nicht das "GestureElement", sondern 
        /// ein anderes, explizit angegebenes.
        /// </summary>
        /// <param name="identifier">Interner Bezeichner für die Geste</param>
        /// <param name="gesture">Die Geste selbst</param>
        /// <param name="callback">Die Methoden, die bei erkennen der Geste ausgeführt werden soll</param>
        /// <param name="element">Das UIELement für die Geste</param>
        public virtual void AddGesture(string identifier, UIElement element, GestureBase gesture,
            GestureHandler.GestureCallbackHandler callback)
        {
            Gestures.Add(identifier,
                Surface.GestureManager.AddGesture(element, gesture, callback));

        }

        /// <summary>
        /// Aktiviert oder deaktiviert eine registrierte Geste
        /// </summary>
        /// <param name="gesture">Der Identifier der Geste</param>
        /// <param name="active"></param>
        public virtual void SetGestureActive(string gesture, bool active)
        {
            if (Gestures != null)
            {
                if (Gestures.ContainsKey(gesture))
                    Gestures[gesture].IsActive = active;
            }
        }

        /// <summary>
        /// Löscht alle registrierten Gesten dieses SurfaceObjects aus der Datenbank.
        /// </summary>
        public virtual void RemoveGestures()
        {
            lock (Gestures)
            {
                foreach (GestureHandler gh in Gestures.Values)
                {
                    Surface.GestureManager.RemoveGesture(gh);
                }
            }
        }
        #endregion
        #region Position Controls
        /// <summary>
        /// Überprüft, ob die angegebenen Koordinaten valide sind
        /// und gibt entweder diese zurück oder andere, valide Koordinaten.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual Point ValidatePosition(double x, double y)
        {
            return Surface.ViewController.CurrentView.ValidatePosition(this, x, y);
        }

        /// <summary>
        /// Legt die Position der oberen linken Ecke fest.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void SetUpperLeftCorner(double x, double y)
        {
            this.Invoke(new Action(() =>
            {
                Point pt = ValidatePosition(x, y);
                this.Left = pt.X;
                this.Top = pt.Y;
                OnMoved(pt.X + (this.ActualWidth /2), pt.Y + (this.ActualHeight / 2), false);
            }));
        }

        /// <summary>
        /// LEgt die Position der Mitte des Surface Objects fest
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="byUser">TRUE, wenn diese Funktion im Laufe einer Geste aufgerufen wird (also wenn das SurfaceObject vom Nutzer verschoben wurde)</param>
        public virtual void SetCenter(double x, double y, bool byUser = false)
        {
            this.Invoke(new Action(() =>
            {
                Point pt = ValidatePosition(x, y);
                this.CenterX = pt.X;
                this.CenterY = pt.Y;
                OnMoved(pt.X, pt.Y, byUser);
            }));
        }

        #endregion
        #region Events
        public event EventHandler<MovedEventArgs> Moved;
        public event EventHandler<MovedEventArgs> PositionInitialized;
        public event EventHandler IsHiddenChanged;

        protected virtual void OnIsHiddenChanged()
        {
            if (IsHiddenChanged != null)
                IsHiddenChanged(this, new EventArgs());
        }

        protected virtual void OnMoved(double x, double y, bool byUser = false)
        {
            //Einmal bewegt -> Position ist somit initialisiert
            if (!IsPositionInitialized)
            {
                IsPositionInitialized = true;
                if (PositionInitialized != null)
                    PositionInitialized(this, new MovedEventArgs(x, y, byUser));
            }
            if (Moved != null)
                Moved(this, new MovedEventArgs(x, y, byUser));
        }
        #endregion
        #region Animations

        /// <summary>
        /// Entfernt eine Skalierungsanimation, falls das SUrfaceObject eine besitzt
        /// </summary>
        public virtual void RemoveResizeAnimation()
        {
            if (HasResizeAnimation)
            {
                this.Invoke(new Action(() =>
                {
                    double factor = this.ScaleFactor;
                    this.ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                    this.ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                    this.BeginAnimation(SurfaceObject.WidthProperty, null);
                    this.BeginAnimation(SurfaceObject.HeightProperty, null);
                    HasResizeAnimation = false;
                    this.ScaleFactor = factor;
                }));
            }
        }

        /// <summary>
        /// Entfernt eine Rotationsanimation, falls das SUrfaceObject eine besitzt
        /// </summary>
        public virtual void RemoveRotateAnimation()
        {
            if (HasRotateAnimation)
            {
                this.Invoke(new Action(() =>
                {
                    this.RotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);
                    HasRotateAnimation = false;
                }));
            }
        }

        /// <summary>
        /// Entfernt eine Bewegungsanimation, falls das SUrfaceObject eine besitzt
        /// </summary>
        public virtual void RemoveMovingAnimation()
        {
            if (HasMovingAnimation)
            {
                this.Invoke(new Action(() =>
                {
                    double x = CenterX;
                    double y = CenterY;
                    this.BeginAnimation(Canvas.LeftProperty, null);
                    this.BeginAnimation(Canvas.TopProperty, null);
                    SetCenter(x, y);
                    HasMovingAnimation = false;
                }));
            }
        }
        #endregion
        #region Visibility Methods

        /// <summary>
        /// Holt das SurfaceObject in der Darstellung nach vorne
        /// </summary>
        public virtual void BringToFront()
        {

            this.Invoke(new Action(() =>
            {
                if (Surface.Children.OfType<UIElement>().Count() > 1)
                {
                    var maxZ = Surface.Children.OfType<UIElement>()
                    .Where(x => x != this)
                    .Select(x => Panel.GetZIndex(x))
                    .Max();
                    Panel.SetZIndex(this, maxZ + 5);
                }
            }));

        }

        /// <summary>
        /// Blendet das SurfaceObject aus
        /// </summary>
        /// <param name="final">TRUE, wenn es nach dem ausblenden gelöscht werden soll, sonst FALSE</param>
        public virtual void Hide(bool final = false)
        {
            this.Invoke(new Action(() =>
            {
                if (!IsHidden)
                {
                    if (final)
                    {
                        Surface.Children.Remove(this);
                        IsHidden = true;
                        OnIsHiddenChanged();
                    }
                    else
                        this.AnimFadeOut(false);
                }
                
            }));
        }

        /// <summary>
        /// Blendet das SurfaceObject ein
        /// </summary>
        public virtual void Show()
        {
            this.Invoke(new Action(() =>
            {
                if (IsHidden)
                {
                    Surface.Children.Add(this);
                    IsHidden = false;
                    OnIsHiddenChanged();
                }
                this.AnimFadeIn();
            }));
        }

        /// <summary>
        /// Löscht das SurfaceObject von der Oberfläche
        /// </summary>
        public virtual void RemoveFromSurface()
        {
            this.Invoke(new Action(() =>
            {
                Surface.Children.Remove(this);
                RemoveGestures();
            }));
        }
        #endregion
    }
}
