using ScrumTouchkit.Data;
using System;
using System.Collections.Generic;
using ScrumTouchkit.Events;

namespace ScrumTouchkit.Controls.ViewModes
{

    /// <summary>
    /// Der ScrumViewController verwaltet alle Ansichten.
    /// Über ihn können sie aktiviert und deaktiviert werden, weiterhin
    /// ermöglicht er das einfache hinzufügen neuer Ansichten.
    /// </summary>
    public class ScrumViewController
    {
        #region Registered Views
        /*
         * ID's aller implementierter Views
         **/
        public const int STANDARD_VIEW = 0x00;
        public const int PRIORITY_VIEW = 0x01;
        public const int EFFORT_VIEW = 0x02;
        public const int EPIC_VIEW = 0x03;
        #endregion
        #region vars, getter, setter

        /// <summary>
        /// Die ID des aktuellen Views
        /// </summary>
        [System.ComponentModel.DefaultValue(-1)]
        public int CurrentViewID
        {
            get;
            private set;
        }

        /// <summary>
        /// Der aktuell angezeigte View
        /// </summary>
        public BaseView CurrentView
        {
            get { return this.Views[CurrentViewID]; }
        }
        private ScrumSurface Surface;

        //Die verschiedenen View-Modes
        private List<BaseView> Views = new List<BaseView>();
        #endregion
        #region Constructor, Init
        public ScrumViewController(ScrumSurface _surface)
        {
            Surface = _surface;
            Surface.SizeChanged += Surface_SizeChanged;
            InitViews();
        }

        private void InitViews()
        {
            Views.Insert(STANDARD_VIEW, new StandardView(Surface, STANDARD_VIEW));
            Views.Insert(PRIORITY_VIEW, new PriorityView(Surface, PRIORITY_VIEW));
            Views.Insert(EFFORT_VIEW, new EffortView(Surface, EFFORT_VIEW));
            Views.Insert(EPIC_VIEW, new EpicView(Surface, EPIC_VIEW));
        }
        #endregion
        #region Activate View
        /// <summary>
        /// Aktiviert eine Ansicht
        /// </summary>
        /// <param name="viewID">Die gewählte View</param>
        /// <param name="external">Aus dem Netzwerk angeordnet?</param>
        public void ActivateView(int viewID, bool external = false)
        {
            ActivateView(Views[viewID], external);
        }

        /// <summary>
        /// Aktiviert eine Ansicht
        /// </summary>
        /// <param name="view">Die Ansicht</param>
        /// <param name="external">Aus dem Netzwerk angeordnet?</param>
        public void ActivateView(BaseView view, bool external = false)
        {
            DeActivateView();
            CurrentViewID = view.ViewID;
            view.Activate();
            ReloadItems();
            if(!external)
                OnViewChanged();
        }
        /// <summary>
        /// ReloadItems() lädt alle Items neu
        /// Kann daher beim View wechsel benutzt werden, aber auch ohne
        /// das eine andere Ansicht aktiviert wurde 
        /// </summary>
        public void ReloadItems()
        {
            foreach (Epic e in Surface.Database.Epics)
            {
                this.CurrentView.LoadItem(e);
            }

            foreach (UserStory u in Surface.Database.UserStories)
            {
                this.CurrentView.LoadItem(u);
            }
        }
        #endregion
        #region Deactivate View
        /// <summary>
        /// Deaktiviert eine Ansicht.
        /// DIes lädt noch <u>keine</u> neue Ansicht!
        /// </summary>
        /// <param name="view">Die Ansicht, die deaktiviert werden soll</param>
        public void DeActivateView(BaseView view)
        {
            view.Deactivate();
            CurrentViewID = -1;
        }
        public void DeActivateView()
        {
            if (CurrentViewID > -1)
            {
                DeActivateView(Views[CurrentViewID]);
            }
        }
        public void UnloadItems()
        {
            foreach (Epic e in Surface.Database.Epics)
            {
                this.CurrentView.UnloadItem(e);
            }

            foreach (UserStory u in Surface.Database.UserStories)
            {
                this.CurrentView.UnloadItem(u);
            }
        }
        #endregion
        #region Events
        public event EventHandler<GenericEventArgs<int>> ViewChanged;

        protected void OnViewChanged()
        {
            if (ViewChanged != null)
                ViewChanged(this, new GenericEventArgs<int>(CurrentViewID));
        }
        #endregion
        #region Refresh
        void Surface_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Refresh();
        }
        /// <summary>
        /// Deaktiviert den aktuellen View und lädt diesen neu, um auf die Änderung an der Bildschirmgröße einzugehen
        /// </summary>
        public void Refresh()
        {
            int c_view = CurrentViewID;
            DeActivateView();
            ActivateView(c_view);
        }
        #endregion

    }
}
