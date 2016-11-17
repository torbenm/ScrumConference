using System.Timers;

namespace ScrumTouchkit.Data.Update
{
    /// <summary>
    /// Unterstützt das Verschicken eines Update über geänderte Eigenschaften eines Items
    /// maximal alle 500ms
    /// </summary>
    public class TimedUpdate
    {
        #region var, get, set
        /// <summary>
        /// Der Timer
        /// </summary>
        private Timer UpdateTimer
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt an, wie lange ein Intervall zwischen den Updates mindestens dauert
        /// Standard ist 500 ms
        /// </summary>
        [System.ComponentModel.DefaultValue(500)]
        public int Intervall
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt an, ob ein Update vorliegt
        /// </summary>
        public bool HasUpdate
        {
            get;
            private set;
        }

        /// <summary>
        /// Das mit diesem TimedUpdate-Objekt verbundene Item
        /// </summary>
        public ItemBase Item
        {
            get;
            private set;
        }
        #endregion
        #region Constructor
        public TimedUpdate(ItemBase item)
        {
            this.Item = item;
            this.Intervall = 500;
            this.UpdateTimer = new Timer(this.Intervall);
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
        }

        void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Wenn es ein Update gibt: Update rausschicken 
            if (this.HasUpdate)
            {
                this.HasUpdate = false;             
                Item.OnDataChanged();
            }
            else
            {
                 //sonst: Timer anhalten!
                this.UpdateTimer.Stop();
            }
        }
        #endregion
        #region Update
        /// <summary>
        /// Wird nach dem aktualisieren des Items aufgerufen
        /// </summary>
        public void OnUpdate()
        {
            //Timer bereits an? Nur angeben das ein Update vorliegt!
            //Ein laufender Timer bedeutet immer, dass vor kurzem bereits ein
            //Update getätigt wurde
            if (this.UpdateTimer.Enabled)
            {
                this.HasUpdate = true;
            }
            else
            {
                //Timer nicht an? Timer starten!
                this.HasUpdate = false;
                this.UpdateTimer.Interval = Intervall;
                this.UpdateTimer.Start();
                Item.OnDataChanged();
            }
        }
        #endregion
    }
}
