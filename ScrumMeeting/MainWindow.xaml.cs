using System;
using System.Windows;
using System.Windows.Input;
using ScrumTouchkit.Controls;

namespace ScrumMeeting
{
    /// <summary>
    /// Das Hauptfenster
    /// </summary>
    public partial class MainWindow : Window
    {


        private bool _fullscreen = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.KeyUp += MainWindow_KeyUp;
            ToggleFullscreen(true);
            
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            surface.Dispose();
            Application.Current.Shutdown();
        }

        void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                ToggleFullscreen(!_fullscreen);
            }
        }
        ScrumSurface surface;
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Neues Scrum-Surface Objekt initialisieren
            surface = new ScrumSurface();
            this.AddChild(surface);
            surface.Loaded += surface_Loaded;
        }

        void surface_Loaded(object sender, RoutedEventArgs e)
        {
            //Neues ScrumNetwork starten, wenn alles geladen ist -> Heißt aber noch nicht,
            //dass auch eine Verbindung hergestellt wird
            ScrumNetwork.ScrumNetwork sc = new ScrumNetwork.ScrumNetwork(surface);
        }

        private void ToggleFullscreen(bool fs)
        {
            if (fs)
            {
                this.WindowStyle = System.Windows.WindowStyle.None;
                this.WindowState = System.Windows.WindowState.Maximized;
                _fullscreen = true;
            }
            else
            {
                this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                this.WindowState = System.Windows.WindowState.Normal;
                _fullscreen = false;
            }
        }
      

    }
}
