using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Controls.ViewModes;
using System;
using System.Collections.Generic;

namespace ScrumTouchkit.Controls.DisplaySettings
{
    /// <summary>
    /// Speichert eine Sammlung von DisplaySettings für eine ItemControl
    /// und Verwaltet somit die verschiedenen Position für die Ansichten
    /// </summary>
    public class DisplaySettingsCollection : Dictionary<int, DisplaySettings>
    {
        public const int DEFAULT_SETTINGS = -1;
        #region vars, getter, setter
        /// <summary>
        /// Die mit dieser DisplaySettingsCollection verknüpfte ItemControl
        /// </summary>
        public ItemControl Representation
        {
            get;
            private set;
        }

        /// <summary>
        /// Die aktuell gewählte Ansicht
        /// </summary>
        public int CurrentViewMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Die aktuell benutzten DisplaySettings
        /// </summary>
        public DisplaySettings CurrentDisplaySettings
        {
            get { return this[CurrentViewMode]; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initialisiert die DisplaySettingsCollection,
        /// und initialisiert zunächst Standardwerte 
        /// (die aber in der Regel vor dem Anzeigen noch abgeändert werden)
        /// </summary>
        /// <param name="rep"></param>
        public DisplaySettingsCollection(ItemControl rep)
        {
            this.CreateSettings(DEFAULT_SETTINGS);
            this.CurrentViewMode = DEFAULT_SETTINGS;
            this.Representation = rep;
        }
        #endregion
        #region Get Settings
        /// <summary>
        /// Lädt die DisplaySettings für den angegebenen View und aktiviert somit
        /// die gespeicherten Anzeigedaten für diesen
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public DisplaySettings SetDisplaySettings(BaseView mode)
        {
            int viewModeID = mode.ViewID;
            if (!this.ContainsKey(viewModeID))
            {
                //Erstelle neue Einstellungen, wenn die Ansicht bisher noch nicht
                //geladen wurde. Kopiere dafür die aktuellen
                this.CreateSettings(viewModeID, this.CurrentDisplaySettings);
            }
            CurrentViewMode = viewModeID;
            mode.AfterSettingsLoaded(this.Representation);
            OnViewChanged();
            return this[CurrentViewMode];
        }
        #endregion
        #region Events
        public event EventHandler ViewChanged;
        public void OnViewChanged()
        {
            if (ViewChanged != null)
                ViewChanged(this, new EventArgs());
        }
        #endregion
        #region Create Settings
        /// <summary>
        /// Erstellt neue DisplaySettings für eine Ansicht
        /// </summary>
        /// <param name="viewModeID">Die Ansicht, für welche die DisplaySettings erstellt werden sollen</param>
        /// <param name="_base">Wenn angegeben, werden diese DisplaySettings als Grundlage für die neuen gewählt.</param>
        private void CreateSettings(int viewModeID, DisplaySettings _base = null)
        {
            DisplaySettings temp;
            if (_base != null)
                temp = _base.Copy();
            else
                temp = UserStoryControl.StaticDefaultSettings.Copy();
            this.Add(viewModeID, temp);
        }
        #endregion

    }
}
