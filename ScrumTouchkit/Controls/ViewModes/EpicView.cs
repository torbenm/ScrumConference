using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Data;
using System;
using System.Timers;

namespace ScrumTouchkit.Controls.ViewModes
{
    /// <summary>
    /// In dieser Ansichten werden neben User Stories auch Epics angezeigt.
    /// Desweiteren sind auch alle als unsichtbar markierten Elemente in dieser Ansicht sichtbar.
    /// User Stories können zu Epics hinzugefügt werden.
    /// </summary>
    public class EpicView : BaseView
    {
        #region vars, getter, setter
        /// <summary>
        /// Es kann zu Problemen bei der Darstellung der Verbindungslinien kommen.
        /// Daher alle 30 ms die Linien aktualisieren ( das ganze nur am Anfang, 60 mal)
        /// </summary>
        private Timer CheckLinesTimer;
        private int timerCount = 0;


        public override string Name
        {
            get
            {
                return "Epics";
            }
        }
        #endregion
        #region Constructor
        public EpicView(ScrumSurface surface, int viewID)
            : base(surface, viewID)
        {
            InitTimer();
            this.allowControls[typeof(Epic)] = 1;
            this.ShowInvisible = true;
        }
        #endregion

        #region Activate / Deactivate
        /***
         *  Beschreibungen zu diesen Funktionen in der Klasse BaseView
         **/
        protected override void InternalActivate()
        {
            StartTimer();
            base.InternalActivate();
            
           
        }
        protected override void InternalDeactivate()
        {
            StopTimer();
            base.InternalDeactivate();
        }
        #endregion
        #region Timer
        private void InitTimer()
        {
            CheckLinesTimer = new Timer();
            CheckLinesTimer.Interval = 30;
            CheckLinesTimer.Elapsed += new ElapsedEventHandler(CheckLinesTimer_Elapsed);
            Surface.FileLoaded += new EventHandler(Surface_FileLoaded);
        }

        void Surface_FileLoaded(object sender, EventArgs e)
        {
            if (this.IsActive)
            {
                StartTimer();
            }
        }
        private void StartTimer()
        {
            timerCount = 0;
            CheckLinesTimer.Start();
        }
        private void StopTimer()
        {
            CheckLinesTimer.Stop();
        }
        void CheckLinesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Nach 60 Ausführungen aufhören
            if (timerCount < 60)
            {
                try
                {
                    foreach (ItemControl ic in this.VisibleControls)
                    {
                        UserStoryControl usc = ic as UserStoryControl;
                        if (usc != null && usc.Line != null)
                            usc.Line.UpdateLine();
                    }
                    timerCount++;
                }
                catch
                { }
            }
            else
                StopTimer();
        }
        #endregion

    }
}
