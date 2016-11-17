using ScrumGestures;
using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Abstract;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScrumTouchkit.Controls.Dialogs
{
    /// <summary>
    /// Der NoTouchLayer ist ein grauer Layer, der unter ein Dialog gelegt werden kann.
    /// Dadurch wird vermieden, dass Steuer
    /// </summary>
    public class NoTouchLayer : SurfaceObject
    {
        #region Constructor
        public NoTouchLayer(ScrumSurface surface)
            : base(surface)
        {
            this.Width = Surface.ActualWidth;
            this.Height = Surface.ActualHeight;

            this.SetUpperLeftCorner(0, 0);
            surface.SizeChanged += surface_SizeChanged;

            InitializeComponents();

            MaxWidth = 4000;
            MaxHeight = 4000;
        }

        private void InitializeComponents()
        {
            Rectangle Rectangle = new Rectangle();
            Rectangle.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Rectangle.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Rectangle.Fill = new SolidColorBrush(Color.FromArgb(125, 100, 100, 100));
            Root.Children.Add(Rectangle);
        }
        #endregion
        #region Event Listener
        void surface_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            // An die Größe des Fensters anpassen
            this.Width = Surface.ActualWidth;
            this.Height = Surface.ActualHeight;
          
           
        }
        #endregion
        #region Gestures
        public const string TAP_GESTURE = "tap";
        public override void InitializeGestures()
        {
            AddGesture(TAP_GESTURE, DefinedGestures.Tap, TapCallback);
        }

        private void TapCallback(System.Windows.UIElement element, TouchGroup points)
        {
            //Keine richtigen Gesten implementiert - eine leere Geste um im HitTest gefunden zu werden
            //(Dadurch werden darunter liegende Elemente nicht gefunden)
        }
        #endregion
    }
}
