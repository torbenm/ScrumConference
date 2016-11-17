using ScrumTouchkit.Controls.Dialogs;
using ScrumTouchkit.Controls.Dialogs.UI;
using ScrumTouchkit.Events;
using ScrumTouchkit.Utilities;
using System;
using System.Collections.Generic;

namespace ScrumTouchkit.Controls.Buttons
{
    /// <summary>
    /// Der ButtonController verwaltet alle angezeigten Buttons und ermöglicht das schnelle Hinzufügen neuer
    /// </summary>
    public class ButtonController
    {
        #region Buttons
        public const int SETTINGS_BUTTON = 0;
        public const int LOAD_BUTTON = 1; 
        public const int SAVE_BUTTON = 2;
        public const int DELETE_BUTTON = 3;
        public const int TOGGLE_BACKLOGS = 4;
        #endregion
        #region vars, getter, setter

        /// <summary>
        /// Die ScrumSurface, zu der die Buttons hinzugefügt werden
        /// </summary>
        public ScrumSurface Surface
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Alle angezeigten Buttons
        /// </summary>
        public List<UI.Button> Buttons
        {
            get;
            private set;
        }
        #endregion
        #region Constructor
        public ButtonController(ScrumSurface surface)
        {
            this.Surface = surface;
            InitButtons();
        }

        /// <summary>
        /// Initialisiert alle vorinstallierten Buttons
        /// </summary>
        public void InitButtons()
        {
            Buttons = new List<UI.Button>();

            CreateButton(SETTINGS_BUTTON, UI.Button.ButtonType.Image)
                .SetValue(Images.settings51).Tapped += SettingsButton_Tap;

            CreateButton(LOAD_BUTTON, UI.Button.ButtonType.Image)
                .SetValue(Images.folder247).Tapped += LoadButton_Tap;
            CreateButton(SAVE_BUTTON, UI.Button.ButtonType.Image)
                .SetValue(Images.floppy20).Tapped += SaveButton_Tap;
            CreateButton(DELETE_BUTTON, new DeleteButton(Surface));


        }

        /// <summary>
        /// Erstellt einen neuen Button.
        /// </summary>
        /// <param name="index">Mithilfe des Index wird die Startposition des Buttons berechnet</param>
        /// <param name="bttn">Der Button, der hinzugefügt werden soll</param>
        /// <returns></returns>
        public UI.Button CreateButton(int index, UI.Button bttn)
        {
            bttn.SetUpperLeftCorner(50, 50 + index * 100);
            Buttons.Insert(index, bttn);

            return bttn;
        }
        /// <summary>
        /// Erstellt einen neuen Button
        /// </summary>
        /// <param name="index">Mithilfe des Index wird die Startposition des Buttons berechnet</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public UI.Button CreateButton(int index, UI.Button.ButtonType type)
        {
            UI.Button bttn = new UI.Button(Surface, type);
            return CreateButton(index, bttn);
            
        }
        #endregion
        #region EventHandler
        /**
         * Die Events, die beim Berühren der Buttons aufgerufen werden
         **/

        private void SaveButton_Tap(object sender, EventArgs e)
        {
            Surface.SaveFile();
        }

        private void LoadButton_Tap(object sender, EventArgs e)
        {
            Surface.LoadFile();
        }
        private void SettingsButton_Tap(object sender, EventArgs e)
        {
            DialogControl<SettingsDialog>.ShowDialog(this.Surface).DialogFinished += Settings_DialogFinished;
        }
        private void Settings_DialogFinished(object sender, DialogEventArgs e)
        {
            if (e.ExitMode == SettingsDialog.SAVE)
            {
                MessageDialog.ShowMessage(this.Surface, "Settings saved!", false);
            }
        }
        #endregion
    }
}
