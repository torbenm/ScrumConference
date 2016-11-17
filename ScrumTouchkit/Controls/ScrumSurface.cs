using Microsoft.Win32;
using ScrumGestures;
using ScrumGestures.Gestures;
using ScrumGestures.Listener;
using ScrumTouchkit.Controls.Dialogs;
using ScrumTouchkit.Controls.Dialogs.UI;
using ScrumTouchkit.Controls.Feedback;
using ScrumTouchkit.Controls.ViewModes;
using ScrumTouchkit.Controls.ViewModes.ViewPicker;
using ScrumTouchkit.Data;
using ScrumTouchkit.Threading;
using ScrumTouchkit.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace ScrumTouchkit.Controls
{
    /// <summary>
    /// Stellt die Oberfläche dar.
    /// Auf dieser werden alle Elemente des Programmes angezeigt, mit denen 
    /// die Nutzer interagieren können
    /// </summary>
    public class ScrumSurface : Canvas, IDisposable
    {
        #region vars, getter, setter
        TouchFeedback _tfeedback;
        
        /// <summary>
        /// Der GestenManager (stellt auch die Verbindung zum Touch-Interface dar)
        /// </summary>
        public GestureManager GestureManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Die Datenbank mit den Epics und User Stories
        /// </summary>
        public ScrumDatabase Database
        {
            get;
            private set;
        }

        /// <summary>
        /// Verwaltet die verschiedenen Ansichten
        /// </summary>
        public ScrumViewController ViewController
        {
            get;
            private set;
        }

        /// <summary>
        /// Verwaltet die verschiedenen Buttons
        /// </summary>
        public Buttons.ButtonController Buttons
        {
            get;
            private set;
        }
        #endregion
        #region Constructor
        public ScrumSurface()
        {
            this.Loaded += ScrumSurface_Loaded;      
        }

        void ScrumSurface_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Hier wird die Oberfläche initialisiert
            // (Vorher abwarten, dass alles von WPF geladen wurde)
            InitializeGestureManager();
            InitializeGestures();
           
            this.Database = new ScrumDatabase(this);
            this.ViewController = new ScrumViewController(this);
            this.Buttons = new Buttons.ButtonController(this);
            
            // DebugStories() erstellt ein paar User Stories und Epics zum debuggen
            DebugStories();
            ViewController.ActivateView(ScrumViewController.STANDARD_VIEW);

            ViewPicker vp = new ViewPicker(this);
            vp.Collapsed = true;

        }
        /// <summary>
        /// Initialisiert den Gesten Manager und verbindet ihn mit TUIO
        /// </summary>
        private void InitializeGestureManager()
        {
            int port = Utilities.Settings.Default.TUIO_PORT;

            // In einer TUIO-Datei in dem Ordnet des Programms kann optional
            // ein anderer TUIO-Port gespeichert werden
            // (Dies lohnt sich vor allem beim häufigen wechseln des ausführenden Computers)
            // Ansonsten in den Settings ändern!
            if (System.IO.File.Exists("tuio.port"))
            {
                port = Int32.Parse(System.IO.File.ReadAllText("tuio.port"));
                Console.WriteLine("Connected with default TUIO-Port "+port);
            }

            TouchListener tl = new TuioListener(port);
            GestureManager = new ScrumGestures.GestureManager(tl);
            GestureManager.Initialize(this);
            InitializeTouchFeedback(tl);
        }
        /// <summary>
        /// Initialisiert das Anzeigen von Berührungspunkten auf der 
        /// Oberfläche
        /// </summary>
        /// <param name="tl"></param>
        private void InitializeTouchFeedback(TouchListener tl)
        {
            _tfeedback = new TouchFeedback();
            _tfeedback.Init(this);
            tl.TouchMove += (s, e) =>
                {
                    _tfeedback.CaptureTouch(e.Value);
                };
        }

        /// <summary>
        /// Erstellt ein paar User Stories und Epics zum Testen
        /// </summary>
        private void DebugStories()
        {
            Epic e = new Epic();
            UserStory u = new UserStory();
            u.Epic = e;
            Database.AddItem(e);
            Database.AddItem(u);
        }
        #endregion
        #region Gestures

        /// <summary>
        /// Initialisiert die Gesten, die direkt auf der Oberfläche durchgeführt werden
        /// </summary>
        public void InitializeGestures()
        {
            GestureManager.AddGesture(this, DefinedGestures.Hold, HoldGesture);
            GestureManager.AddGesture(this, DefinedGestures.TouchDown, TouchDownCallback);
            GestureManager.AddGesture(this, DefinedGestures.TouchUp, TouchUpCallback);
            GestureManager.AddGesture(this, DefinedGestures.DragAndDrop, MoveCallback);
        }

        /// <summary>
        /// Da die Oberfläche nie "geschlossen" wird, ohne dass das gesamte Programm geschlossen wird,
        /// müssen die Gesten auch nicht entfernt werden
        /// </summary>
        public void RemoveGestures()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Bei jeder neun Berührung: Creation Feedback (größer werdendes Rechteck) beginnt!
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        private void TouchDownCallback(UIElement element, TouchGroup points)
        {
            if (points.Count == 1)
                StartCreationFeedback(points[0].CurrentPoint);
            else
                RemoveCreationFeedback();
        }

        /// <summary>
        /// Berührung weg -> Creation - Feedback weg
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        private void TouchUpCallback(UIElement element, TouchGroup points)
        {
            RemoveCreationFeedback();
        }

        /// <summary>
        /// Halte-Geste zum erstellen einer neuen User Story
        /// Das Creation Feedback ist so ausgelegt, 
        /// dass das Rechteck nicht weiter wächst, wenn die User Story durch loslassen
        /// erstellt werden kann
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        private void HoldGesture(UIElement element, TouchGroup points)
        {
            this.Invoke(() =>
                {
                    UserStory us = new UserStory();
                    Database.AddItem(us);
                    us.Representations[0].MoveCenter(points[0].CurrentPoint.X, points[0].CurrentPoint.Y);

                    //Automatisch den Winkel bestimmen
                    us.Representations[0].RotateAngle = 
                                InitialAngleHelper.GetInitialAngle(
                                    this.ActualWidth,
                                    this.ActualHeight,
                                    points[0].CurrentPoint);
                    OnItemCreated(us);
                });
        }

        /// <summary>
        /// Auch bei der Bewegung des Fingers kommt das Creation Feedback weg!
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        private void MoveCallback(UIElement element, TouchGroup points)
        {
            RemoveCreationFeedback();
        }
        #endregion
        #region Creation Feedback
        private Rectangle _creationFeedback;

        /// <summary>
        /// Erstellt ein Creation-Feedback an dem angegebenen Punkt.
        /// Dabei vergrößert sich ein Rechteck solange, bis durch das loslassen des Punktes
        /// auch eine User Story erstellt werden kann. Zudem dreht sich das Rechteck bereits in
        /// die spätere Ausrichtung der User Story und zeigt auch dessen größe an.
        /// </summary>
        /// <param name="pt"></param>
        public void StartCreationFeedback(Point pt)
        {
            this.Invoke(() =>
                {
                    RemoveCreationFeedback();
                    _creationFeedback = new Rectangle();
                    lock (_creationFeedback)
                    {
                        _creationFeedback.RenderTransform =
                            new RotateTransform(
                                InitialAngleHelper.GetInitialAngle(
                                    this.ActualWidth,
                                    this.ActualHeight,
                                    pt),
                                    UserStoryControl.StaticDefaultSettings.Scale * (double)UserStoryControl.StdWidth / 2,
                                    UserStoryControl.StaticDefaultSettings.Scale * (double)UserStoryControl.StdHeight / 2);

                        _creationFeedback.SetValue(Canvas.LeftProperty, pt.X);
                        _creationFeedback.SetValue(Canvas.TopProperty, pt.Y);
                        _creationFeedback.Width = 0;
                        _creationFeedback.Height = 0;
                        _creationFeedback.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        _creationFeedback.StrokeThickness = 2;
                        this.Children.Add(_creationFeedback);
                        Animation.Animator.ResizeTo(_creationFeedback, 
                                UserStoryControl.StaticDefaultSettings.Scale * (double)UserStoryControl.StdWidth,
                                UserStoryControl.StaticDefaultSettings.Scale * (double)UserStoryControl.StdHeight, 
                                pt.X, pt.Y);
                    }
            });
        }

        /// <summary>
        /// Entfernt das Creation-Feedback von der Oberfläche
        /// </summary>
        public void RemoveCreationFeedback()
        {
            this.Invoke(() =>
                {
                    if (_creationFeedback != null)
                    {
                        this.Children.Remove(_creationFeedback);
                        _creationFeedback = null;
                    }
                });
        }
        #endregion
        #region Save & Load
        public delegate void ContinueAfterDialog();

        /// <summary>
        /// Öffnet einen Dialog, der nachfragt, ob man den aktuellen Stand speichern möchte.
        /// Falls nach dem Abschluss des Dialogs eine bestimmte Methode (außer evtl. das Speichern)
        /// ausgeführt werden soll, kann diese hier auch referenziert werden.
        /// </summary>
        /// <param name="msg">Die Nachricht, die in dem Dialog angezeigt werden soll (z.B. Wollen sie speichern?)</param>
        /// <param name="callback">Die Methode, die nach dem Abschluss des Dialogs & Speichervorgangs ausgeführt wird</param>
        public void AskToSave(string msg = "Do you want to save the data before a new file is loaded?", ContinueAfterDialog callback = null)
        {
            if (Database.Epics.Count > 0 || Database.UserStories.Count > 0)
            {
                DialogControl<MessageDialog> dia = MessageDialog.ShowMessage(this, msg, false, MessageDialog.OptionTypes.YesNo);
                dia.DialogFinished += (s, e) =>
                    {
                        if (e.ExitMode == MessageDialog.YES)
                            SaveFile();
                        if (callback != null)
                            callback();
                    };
            }
            else
                if(callback != null)
                    callback();
        }

        /// <summary>
        /// Zeigt ein Datei Speichern Dialog an und bietet dem Nutzer somit die Möglichkeit, den Zustand zu speichern
        /// </summary>
        public void SaveFile()
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = ".sdf";
            ofd.Filter = ("Sprint Data File") + "|*." + ofd.DefaultExt;

            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                Database.SaveToFile(ofd.FileName);
                MessageDialog.ShowMessage(this, "File Saved!", false);
            }
        }

        /// <summary>
        /// Lädt eine Datei, fragt aber vorher nach, ob der Zustand vorher gespeichert werden soll
        /// Datei Laden (Schritt 1 / 2)
        /// </summary>
        public void LoadFile()
        {
            AskToSave(callback: LoadFileCont);
            
        }

        /// <summary>
        /// Zeigt ein "Datei Öffnen" Dialog an
        /// </summary>
        private void LoadFileCont()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".sdf";
            ofd.Filter = ("Sprint Data File") + "|*." + ofd.DefaultExt;

            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                
                this.Invoke(() =>
                {
                    Database.LoadFromFile(ofd.FileName);
                    ViewController.ReloadItems();
                    MessageDialog.ShowMessage(this, "File Loaded!", false);
                    OnFileLoaded();
                });
            }
        }
        #endregion
        #region events
        public event EventHandler<UserStory> ItemCreated;
        public event EventHandler FileLoaded;

        protected void OnFileLoaded()
        {
            if(FileLoaded != null)
             FileLoaded(this, new EventArgs());
        }
        protected void OnItemCreated(UserStory story)
        {
            if (ItemCreated != null)
                ItemCreated(this, story);
        }
        #endregion
        #region Dispose
        public void Dispose()
        {
            GestureManager.Dispose();
        }
        #endregion
    }
}
