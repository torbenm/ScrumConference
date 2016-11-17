using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Data;
using System;
using System.Windows;
using ScrumTouchkit.Controls.ViewModes.Background;

namespace ScrumTouchkit.Controls.ViewModes
{
    /// <summary>
    /// In dieser Ansicht kann der Aufwand der User Stories abgeschätzt werden.
    /// </summary>
    public class EffortView : BaseView
    {


        #region vars, getter, setter
        public override string Name
        {
            get
            {
                return "Effort";
            }
        }
        public EffortBars Background
        {
            get;
            private set;
        }
        #endregion
        #region Constructor
        public EffortView(ScrumSurface surface, int viewID)
            : base(surface, viewID)
        {
            this.settingsLoader = new SettingsLoader.EffortLoader(this);
            this.Surface.SizeChanged += Surface_SizeChanged;
            this.allowControls[typeof(Epic)] = 0;
            this.freeMovement = false;
        }
        #endregion
        #region EventHandler
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
        void Surface_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (IsActive)
            {
                Surface.ViewController.ReloadItems();
            }
        }
        void Background_IntervallChanged(object sender, EventArgs e)
        {
            Surface.ViewController.ReloadItems();
        }
        #endregion
        #region Activate / Deactivate
        /***
         *  Beschreibungen zu diesen Funktionen in der Klasse BaseView
         **/
        protected void InitBackground()
        {
            //Die EffortBars-Klasse sorgt sich um alle Darstellungen im Hintergrund
            Background = new EffortBars(this.Surface);
            Background.VerticalAlignment = VerticalAlignment.Stretch;
            Background.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
        protected override void InternalActivate()
        {
            if (Background == null)
                InitBackground();
           this.AddElementToSurface(Background);

        }
        protected override void InternalDeactivate()
        {
             base.InternalDeactivate();
        }

        
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
        public override void AfterSettingsLoaded(ItemControl control)
        {
            DisplaySettings.DisplaySettings ds = control.DisplaySettings.CurrentDisplaySettings;
            if (!ds.Initialized)
            {
                control.Item.ExternalDataChanged += Item_ExternalDataChanged;
                ds.Initialized = true;
            }
        }
        #endregion
    }
}
