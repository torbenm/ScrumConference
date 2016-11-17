using ScrumTouchkit.Controls;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Data.Effort;
using ScrumTouchkit.Events;
using ScrumTouchkit.Utilities.Serializer;
using System;
using System.Collections.Generic;

namespace ScrumTouchkit.Data
{

    /// <summary>
    /// Speichert User Stories und Epics und 
    /// bietet Methoden zur Verwaltung dieser an
    /// </summary>
    public class ScrumDatabase
    {
        private IFileManager fileManager = new XMLSerializer();
        #region Getter & Setter

        /// <summary>
        /// Die Liste von User Stories
        /// </summary>
        public List<UserStory> UserStories
        {
            get;
            set;
        }

        /// <summary>
        /// Die Liste von Epics
        /// </summary>
        public List<Epic> Epics
        {
            get;
            set;
        }

        /// <summary>
        /// Die Scrum Surface, zu welcher die Datenbank gehört
        /// </summary>
        public ScrumSurface Surface
        {
            get;
            private set;
        }

        /// <summary>
        /// Summe <u>aller</u> EffortPoints, die User Stories im Sprint Backlog besitzen
        /// Ermöglicht somit das bessere abschätzen des Aufwands für den Sprint
        /// </summary>
        public EffortPoints EffortSum
        {
            get;
            private set;
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Aktualisiert eine neue Datenbank
        /// </summary>
        /// <param name="surface"></param>
        public ScrumDatabase(ScrumSurface surface)
        {
            this.EffortSum = new EffortPoints(0);
            this.Surface = surface;
            this.UserStories = new List<UserStory>();
            this.Epics = new List<Epic>();
        }
        #endregion
        #region Adding Items
        /// <summary>
        /// Löscht alle Items aus der Datenkbank
        /// </summary>
        public void ClearItems()
        {
            for(int i = Epics.Count - 1; i >= 0; i--)
            {
                RemoveItem(Epics[i]);
            }
            for(int i = UserStories.Count - 1; i >= 0; i--)
                RemoveItem(UserStories[i]);
        }

        /// <summary>
        /// Fügt ein neues Item zu der Datenbank hinzu
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ItemBase item)
        {
            if (item.GetType() == typeof(UserStory))
            {
                UserStory us = item as UserStory;
                UserStories.Add(us);
                us.ProjectStatusChanged += us_ProjectStatusChanged;
                us.Effort.EffortPointsChanged += (s, e) => { RecalcEffortSum(); };
                if (us.temp_epicID > 0)
                {
                    us.Epic = GetItem(us.temp_epicID) as Epic;
                }
                if (us.BacklogStatus == ItemBacklogStatus.SPRINT_BACKLOG)
                {
                    this.EffortSum += us.Effort;
                }
            }
            else
                Epics.Add(item as Epic);
            Surface.ViewController.CurrentView.LoadItem(item);
            item.DataChanged += OnItemChanged;
        }


        private void us_ProjectStatusChanged(object sender, EventArgs e)
        {
            UserStory us = sender as UserStory;
            if (us != null)
            {
                if (us.BacklogStatus == ItemBacklogStatus.SPRINT_BACKLOG)
                    this.EffortSum += us.Effort;
                else
                    RecalcEffortSum();
            }
        }

        /// <summary>
        /// Berechnet die Effort-Summe neu
        /// </summary>
        private void RecalcEffortSum()
        {
            EffortPoints ep = new EffortPoints(0);
            foreach (UserStory us in UserStories)
                if (us.BacklogStatus == ItemBacklogStatus.SPRINT_BACKLOG)
                    ep += us.Effort;
            this.EffortSum.Value = ep.Value;
        }

        /// <summary>
        /// Entfernt ein Item aus der Datenbank
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(ItemBase item)
        {
            foreach (ItemControl ic in item.Representations)
            {
                ic.RemoveFromSurface();
            }
            if (item.GetType() == typeof(UserStory))
            {
                UserStories.Remove(item as UserStory);
            }
            if (item.GetType() == typeof(Epic))
            {
                Epics.Remove(item as Epic);
            }
        }

        /// <summary>
        /// Gibt ein Item anhand seiner ID zurück
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemBase GetItem(short id)
        {
            if (id < 10000)
            {
                foreach (Epic e in Epics)
                {
                    if (e.ItemID == id)
                        return e;
                }
                return null;
            }
            else
            {
                foreach (UserStory u in UserStories)
                {
                    if (u.ItemID == id)
                        return u;
                }
                return null;
            }
        }

        #endregion
        #region Save & Load
        /// <summary>
        /// Lädt eine Liste von Epics in die Datenbank
        /// </summary>
        /// <param name="epics"></param>
        /// <returns></returns>
        public bool LoadItems(List<Epic> epics)
        {
            short maxID = 10;
            for (int i = 0; i < epics.Count; i++)
            {
                AddItem(epics[i]);
                maxID = Math.Max(maxID, epics[i].ItemID);
            }
            Epic.NextEpicID = maxID++;
            return true;
        }
        /// <summary>
        /// Lädt eine Liste von User Stories in die Datenbank
        /// </summary>
        /// <param name="stories"></param>
        public void LoadItems(List<UserStory> stories)
        {
            short maxID = 10000;
            for (int i = 0; i < stories.Count; i++)
            {
                AddItem(stories[i]);
                maxID = Math.Max(maxID, stories[i].ItemID);
            }
            UserStory.NextUserStoryID = maxID++;

        }
        /// <summary>
        /// Lädt eine Liste von Epics und User Stories in die Datenbank
        /// </summary>
        /// <param name="epics"></param>
        /// <param name="us"></param>
        public void LoadItems(List<Epic> epics, List<UserStory> us)
        {
            if(LoadItems(epics))
                LoadItems(us);
        }

        /// <summary>
        /// Lädt eine in einer Datei gespeicherten Liste von User Stories/Epics
        /// in die Datenbank (XML)
        /// </summary>
        /// <param name="filepath"></param>
        public void LoadFromFile(string filepath)
        {
            fileManager.LoadFile(this, filepath, Surface);
        }

        /// <summary>
        /// Speichert die Datenbank in einer Datei (XML)
        /// </summary>
        /// <param name="filepath"></param>
        public void SaveToFile(string filepath)
        {
                fileManager.WriteFile(this.Epics, this.UserStories, filepath);
        }
        #endregion
        #region Events
        public event EventHandler<EditorStateEventArgs> EditorStateChanged;
        public event EventHandler<TextBoxStateEventArgs> NetworkDataAvailable;
        public event EventHandler ItemChanged;
        public event EventHandler FocusRequested;

        internal void OnEditorStateChanged(object o, EditorStateEventArgs e)
        {
            if (EditorStateChanged != null)
                EditorStateChanged(o, e);
        }
        internal void OnNetworkDataAvailable(object o, TextBoxStateEventArgs e)
        {
            if (NetworkDataAvailable != null)
                NetworkDataAvailable(o, e);
        }
        internal void OnFocusRequested(object sender, EventArgs e)
        {
            if (FocusRequested != null)
                FocusRequested(sender, e);
        }
        protected void OnItemChanged(object o, EventArgs e)
        {
            if (ItemChanged != null)
                ItemChanged(o, e);
        }
        #endregion

        
    }
}
