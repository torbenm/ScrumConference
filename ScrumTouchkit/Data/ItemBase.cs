using Newtonsoft.Json;
using ScrumTouchkit.Controls;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Controls.Network;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ScrumTouchkit.Data
{
    /// <summary>
    /// Stellt grundlegende Eigenschaften dar, die sowohl Epics als auch User Stories besitzen
    /// </summary>
    public abstract class ItemBase : EventArgs
    {
        /// <summary>
        /// Nur für JSON interessant: Gibt an, ob es sich um eine Epic oder
        /// User Story handelt
        /// </summary>
        public enum SubType : short
        {
            USER_STORY = 0,
            EPIC = 1
        }

        /**
         * Die nächsten ID-Werte für Epics und User Stories
         * Falls mit einem Netzwerk verbunden, zählen diese jedoch nicht
         * sondern werden mit den vom Server angebotenen IDs überschrieben
         **/
        public static short NextEpicID = 10;
        public static short NextUserStoryID = 10000;

        #region vars, getter, setter
        private String _title = "Title";
        private bool _visible = true;
        protected Update.TimedUpdate _timer;

        /// <summary>
        /// Die ID des Items
        /// </summary>
        public short ItemID
        {
            get;
            set;
        }

        /// <summary>
        /// Nur für JSON interessant: Gibt an, ob es sich um eine Epic oder
        /// User Story handelt
        /// </summary>
        public SubType Type
        {
            get;
            set;
        }
        /// <summary>
        /// Titel des Items
        /// </summary>
        public String Title
        {
            get { return _title; }
            set { _title = value; OnDataChanged(); }
        }

        /// <summary>
        /// Alle Darstellungen des Items.
        /// Im Moment nur eine aktiviert,
        /// aber die Möglichkeit für mehrere ist gegeben.
        /// </summary>
        [JsonIgnore]
        public List<ItemControl> Representations
        {
            get;
            private set;
        }

        /// <summary>
        /// Gibt an, ob das Item auch außerhalb der Epic-Ansicht sichtbar ist
        /// </summary>
        public bool IsVisible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                OnDataChanged();
            }
        }
        #endregion
        #region Constructor
        public ItemBase()
        {
            Representations = new List<ItemControl>();
            _timer = new Update.TimedUpdate(this);
        }
        #endregion
        #region Update
        public event EventHandler DataChanged;
        //Only called if updated externally
        public event EventHandler ExternalDataChanged;

        public virtual void UpdateData(ItemBase data, ScrumDatabase db)
        {
   
            _title = data.Title;
            _visible = data.IsVisible;
            OnExternalDataChanged();
        }
        internal void OnDataChanged()
        {
            if(DataChanged != null)
                DataChanged(this, new EventArgs()); 
        }
        protected void OnExternalDataChanged()
        {
            if (ExternalDataChanged != null)
                ExternalDataChanged(this, new EventArgs());
        }
        #endregion
        #region Representations
        public ItemControl CreateRepresentation(ScrumSurface surface)
        {
            return GenerateRepresentation(surface);
        }

        /// <summary>
        /// Fügt der Liste von Darstellungen diese Darstellung hinzu
        /// </summary>
        /// <param name="control"></param>
        public void AddRepresentation(ItemControl control)
        {
            this.Representations.Add(control);
            control.EditorStateChanged += control.Surface.Database.OnEditorStateChanged;
            control.NetworkDataAvailable += control.Surface.Database.OnNetworkDataAvailable;
            control.FocusRequested += control.Surface.Database.OnFocusRequested;
        }

        /// <summary>
        /// Aktiviert den (readonly)-Bearbeitungsmodus, falls aus dem Netzwerk
        /// der Befehl dazu kam (oder deaktiviert diesen)
        /// </summary>
        /// <param name="starting">TRUE wenn der Bearbeitungsmodus aktiviert werden soll</param>
        public void ExternalEditorChange(bool starting)
        {
            foreach (ItemControl ic in this.Representations)
            {
                ic.ChangeEditorState(starting);
            }
        }

        /// <summary>
        /// Übermittelt Textbox-Zustände, die aus dem Netzwerk kommen, weiter an
        /// die entsprechende Darstellung
        /// </summary>
        /// <param name="state"></param>
        public void ProcessLiveData(TextBoxState state)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (ItemControl ctrl in Representations)
                {
                    ctrl.ReceiveLiveFeed(state);
                }
            }));
        }
        protected abstract ItemControl GenerateRepresentation(ScrumSurface surface);
        #endregion
    }
}
