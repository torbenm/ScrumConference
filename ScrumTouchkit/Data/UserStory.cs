using ScrumTouchkit.Controls;
using System;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Data.Effort;
using Newtonsoft.Json;

namespace ScrumTouchkit.Data
{
    /// <summary>
    /// Repräsentiert eine User Story
    /// Speichert zudem alle Darstellungen für diese
    /// </summary>
    public class UserStory : ItemBase
    {
        #region vars, getter, setter
        private double _priority = 1.0;
        private ItemBacklogStatus _status = ItemBacklogStatus.PRODUCT_BACKLOG;
        private String _text = "Description";
        private Epic _epic = null;
        public short temp_epicID;

        /// <summary>
        /// Beschreibungstext
        /// </summary>
        public String Text
        {
            get { return _text; }
            set { _text = value; OnDataChanged(); }
        }

        /// <summary>
        /// Priorität
        /// Angegeben zwischen 0 und 1 
        /// Mit 0 sehr hoch und 1 sehr niedrig
        /// </summary>
        public double Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                _timer.OnUpdate();
            }
        }

        /// <summary>
        /// Der Aufwand für diese User Story
        /// </summary>
        public EffortPoints Effort
        {
            get;
            private set;
        }

        /// <summary>
        /// Gibt an, zu welchem Backlog das Item gehört
        /// </summary>
        public ItemBacklogStatus BacklogStatus
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnDataChanged();
                OnProjectStatusChanged();
            }
        }

        /// <summary>
        /// Verweis zu der Epic, zu welcher diese User Story gehört
        /// </summary>
        [JsonIgnore]
        public Epic Epic
        {
            get { return _epic; }
            set
            {
                SetEpic(value);
            }
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Initialisiert eine neue User Story mit Standardwerten
        /// Backlog: Product Backlog
        /// EffortPoints: Unendlich
        /// Priorität: 1
        /// </summary>
        public UserStory()
        {
            this.BacklogStatus = ItemBacklogStatus.PRODUCT_BACKLOG;
            this.Type = SubType.USER_STORY;
            this.ItemID = NextUserStoryID++;
            Effort = new EffortPoints();
            Effort.EffortPointsChanged += (s, e) => { if(e.ExternallyTriggered) this.OnDataChanged(); };
        }
        #endregion
        #region Update
        /// <summary>
        /// Aktualisert die Daten dieser User Story und setzt sie 
        /// auf die Werte der übergebenen User Story
        /// </summary>
        /// <param name="data">Die neuen Daten (muss eine User Story sein, sonst passiert nichts!</param>
        /// <param name="db"></param>
        public override void UpdateData(ItemBase data, ScrumDatabase db)
        {
            UserStory us = data as UserStory;
            if (us != null)
            {
                if (this._status != us._status)
                {
                    this._status = us._status;
                    OnProjectStatusChanged();
                }
                else
                    this._status = us._status;
                this._priority = us._priority;
                this.Effort.SetValue(us.Effort);
                this._text = us.Text;
                this.SetEpic(db.GetItem(us.temp_epicID) as Epic, true);

                base.UpdateData(data, db);
            }
        }
        #endregion
        /// <summary>
        /// Erstellt eine neue Darstellung dieser User Story auf der angebenen ScrumSurface
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        protected override Controls.Abstract.ItemControl GenerateRepresentation(Controls.ScrumSurface surface)
        {
            return new UserStoryControl(this, surface);
        }
        #region events
        public event EventHandler ProjectStatusChanged;

        protected void OnProjectStatusChanged()
        {
            if (ProjectStatusChanged != null)
                ProjectStatusChanged(this, new EventArgs());
        }
        #endregion
        /// <summary>
        /// Ändert die verlinkte Epic auf einen neuen Wert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="external">TRUE, wenn die neue Information aus dem Netzwerk stammt und nicht auf diesem Computer geändert wurde</param>
        private void SetEpic(Epic value, bool external = false)
        {
            if (Epic != null)
                Epic.UserStories.Remove(this);
            this._epic = null;
            if (!external)
                OnDataChanged();

            this._epic = value;

            if (Epic != null)
            {
                this.IsVisible = this.Epic.IsVisible;
                Epic.UserStories.Add(this);
                foreach (ItemControl ic in this.Representations)
                {
                    ic.AutoPos();
                }
                temp_epicID = Epic.ItemID;
            }
            else
            {
                this.IsVisible = true;
                temp_epicID = 0;
            }
            if(!external)
                OnDataChanged();


        }
        
    }
}
