using ScrumGestures.Gestures;
using System.Windows.Controls;
using ScrumTouchkit.Threading;

namespace ScrumTouchkit.Controls.ViewModes.ViewPicker
{
    /// <summary>
    /// Interaktionslogik für den ViewPicker - in diesem kann die Ansicht gewechselt werden
    /// </summary>
    public class ViewPicker : Abstract.StandardSurfaceObject
    {
        #region var, get, set
        private bool _collapsed = true;
        private CollapsedPicker cpick;
        private ExtendedPicker epick;

        /// <summary>
        /// Gibt an, ob der ViewPicker aktuell ausgeklappt oder eingeklappt ist
        /// </summary>
        public bool Collapsed
        {
            get
            {
                return _collapsed;
            }
            set
            {
                _collapsed = value;
                ShowPicker();
            }
        }
        #endregion
        #region Constructor
        public ViewPicker(ScrumSurface surface) : base(surface)
        {
            cpick = new CollapsedPicker();
            epick = new ExtendedPicker();
            this.Loaded += ViewPicker_Loaded;
            //Anzeige der aktuellen Ansicht anpassen, wenn sie geändert wurde
            Surface.ViewController.ViewChanged += (s, e) =>
                {
                    cpick.currentView.Text = Surface.ViewController.CurrentView.Name;
                    epick.currentView.Text = Surface.ViewController.CurrentView.Name;
                };
            
        }

        void ViewPicker_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowPicker();


            this.SetCenter(Surface.ActualWidth - 100, 50);
            InitializeGesturesInt();
        }
        #endregion
        #region Gestures
        public void InitializeGesturesInt()
        {
            AddGesture("collapsed_expand", cpick.expandRect, DefinedGestures.Tap, ExpandCallback);
            AddGesture("collapsed_expand2", cpick.currentView, DefinedGestures.Tap, ExpandCallback);
            AddGesture("expaneded_collapse", epick.collapseRect, DefinedGestures.Tap, ExpandCallback);
            AddGesture("expaneded_collapse2", epick.currentView, DefinedGestures.Tap, ExpandCallback);
            AddGesture("epic_picked", epick.epics_view, DefinedGestures.Tap, EpicPicked);
            AddGesture("effort_picked", epick.effort_view, DefinedGestures.Tap, EffortPicked);
            AddGesture("std_picked", epick.std_view, DefinedGestures.Tap, StandardPicked);
            AddGesture("prio_picked", epick.prio_view, DefinedGestures.Tap, PrioPicked);
        }

        private void PrioPicked(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {
            this.Invoke(() =>
                {
                    Surface.ViewController.ActivateView(ScrumViewController.PRIORITY_VIEW);
                    Collapsed = true;
                });
        }

        private void StandardPicked(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {
            this.Invoke(() =>
            {
                Surface.ViewController.ActivateView(ScrumViewController.STANDARD_VIEW);
                Collapsed = true;
            });
        }

        private void EffortPicked(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {
            this.Invoke(() =>
            {
                Surface.ViewController.ActivateView(ScrumViewController.EFFORT_VIEW);
                Collapsed = true;
            });
        }

        private void EpicPicked(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {
            this.Invoke(() =>
            {
                Surface.ViewController.ActivateView(ScrumViewController.EPIC_VIEW);
                Collapsed = true;
            });
        }

        private void ExpandCallback(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {
            this.Invoke(() =>
                {
                    Collapsed = !Collapsed;
                });
        }
        #endregion

        #region View
        private void ShowPicker()
        {
            if (Collapsed)
            {
                ShowPicker(cpick);
                cpick.currentView.Text = Surface.ViewController.CurrentView.Name;
            }
            else
            {
                ShowPicker(epick);
                epick.currentView.Text = Surface.ViewController.CurrentView.Name;
            }
           
            this.Scale(0.6, false);
            
        }

        /// <summary>
        /// Zeigt einen der beiden Picker an (CollapsedPicker oder ExtendedPicker)
        /// </summary>
        /// <param name="picker">CollapsedPicker oder ExtendedPicker</param>
        private void ShowPicker(UserControl picker)
        {
            this.Root.Children.Clear();
            this.Root.Children.Add(picker);
            this.SwitchSize(picker.Width, picker.Height);
        }
        #endregion

    }
}
