using ScrumGestures;
using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Threading;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScrumTouchkit.Controls.Buttons.UI
{
    /// <summary>
    /// Interaktionslogik für einen runden Button
    /// </summary>
    public partial class Button : StandardSurfaceObject
    {
        #region enum
        public enum ButtonType
        {
            /// <summary>
            /// Image bedeutet, dass in der Mitte des Kreises ein Bild zu sehen ist
            /// </summary>
            Image,
            /// <summary>
            /// Text bedeutet, dass in der Mitte des Kreises ein Text zu sehen ist
            /// </summary>
            Text
        }
        #endregion
        #region const
        /// <summary>
        /// Die normale Farbe für den Hintergrund
        /// </summary>
        private System.Windows.Media.Color _normalcolor = System.Windows.Media.Color.FromRgb(0xED, 0xEC, 0xEC);
        /// <summary>
        /// Die Farbe für den Hintergrund, wenn der Button berührt wird
        /// </summary>
        private System.Windows.Media.Color _touchcolor = System.Windows.Media.Color.FromRgb(0x91, 0x91, 0x91);

        #endregion
        #region vars, getter, setter
        public ButtonType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Die im Hintergrund dargestellte Ellipse
        /// </summary>
        public Ellipse Circle
        {
            get { return _circle; }
        }

        /// <summary>
        /// Das Textfeld, falls es sich um einen Text-Button handelt
        /// </summary>
        public TextBlock Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Das Bild, falls es sich um einen Bild-Button handelt
        /// </summary>
        public Image Image
        {
            get;
            private set;
        }

        /// <summary>
        /// Optionale Toggle funktion erlaubt das automatische wechseln zwischen zwei Zuständen
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        public bool Toggle
        {
            get;
            set;
        }
        protected override UIElement GestureElement
        {
            get
            {
                return this;
            }
        }
        #endregion
        #region Constructor
        public Button(ScrumSurface surface, ButtonType _type) : base(surface)
        {
            Type = _type;
            this.IsHitTestVisible = true;
            InitializeComponent();
            InitType();
        }

        /// <summary>
        /// Initialisiert den Button entsprechend des angegebenen Typs
        /// </summary>
        private void InitType()
        {
            if (Type == ButtonType.Image)
            {
                Image = new Image();
                Image.Width = this.Width;
                Image.Height = this.Height;
                _viewbox.Child = Image;
            }
            else
            {
                Text = new TextBlock();
                _viewbox.Child = Text;
            }
        }
        #endregion
        #region style
        /// <summary>
        /// Legt die Hintergrundfarbe des Buttons fest
        /// </summary>
        /// <param name="c"></param>
        public void SetBackgroundColor(System.Windows.Media.Color c)
        {
            this.Invoke(
                new Action(() =>
                {
                    Circle.Fill = new SolidColorBrush(c);
                }));
        }
        #endregion
        #region Gestures
        public const string TAP_GESTURE = "tap";
        public const string TOUCH_DOWN = "touchdown";
        public const string TOUCH_UP = "touchup";

        public override void InitializeGestures()
        {
            base.InitializeGestures();
            SetGestureActive(RESIZE_GESTURE, false);
            this.AddGesture(TAP_GESTURE, DefinedGestures.Tap, TapGestureCallback);
            this.AddGesture(TOUCH_DOWN, DefinedGestures.TouchDown, TouchDownCallback);
            this.AddGesture(TOUCH_UP, DefinedGestures.TouchUp, TouchUpCallback);
        }

        private void TouchUpCallback(System.Windows.UIElement element, TouchGroup points)
        {
            SetBackgroundColor(_normalcolor);
        }

        private void TouchDownCallback(System.Windows.UIElement element, TouchGroup points)
        {
            SetBackgroundColor(_touchcolor);
        }
        private void TapGestureCallback(System.Windows.UIElement element, TouchGroup points)
        {
            Toggle = !Toggle;
            OnTapped();
        }
        #endregion
        #region Events
        public event EventHandler Tapped;
        protected void OnTapped()
        {
            if (Tapped != null)
                Tapped(this, null);
        }
        #endregion
        #region Image
        /// <summary>
        /// Lädt ein Bitmap in die Darstellung (Nützlich bei zugriff auf Ressourcen, da 
        /// Bilder meistens als Bitmap gespeichert werden)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bitmap"></param>
        private void SetImage(Image self, System.Drawing.Bitmap bitmap)
        {
            self.Invoke(new Action(() =>
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    try
                    {
                        bitmap.Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        self.Source = bitmapImage;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }));
        }
        #endregion
        #region Value
        /// <summary>
        /// Legt den Text fest, der angezeigt wird , falls es sich um einen Text-Button handelt
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Button SetValue(string text)
        {
            if (Type == ButtonType.Text && Text != null)
            {
                Text.Text = text;
            }
            return this;
        }

        /// <summary>
        /// Legt das Bild fest, welches angezeigt wird, falls es sich um einen Image-Button handelt
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public Button SetValue(System.Drawing.Bitmap bmp)
        {
            if (Type == ButtonType.Image && Image != null)
            {
                SetImage(Image, bmp);
            }
            return this;
        }
        #endregion
    }
}
