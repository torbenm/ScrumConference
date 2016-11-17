using ScrumTouchkit.Data;
using System.Collections.Generic;

namespace ScrumNetwork.Utilities
{
    /// <summary>
    /// Schlanke Representation einer ScrumDatabase,
    /// um diese einfach per JSON empfangen und senden zu können
    /// </summary>
    public class SendableDatabase
    {
        public List<UserStory> UserStories;
        public List<Epic> Epics;

        private ScrumDatabase datab;

        public void LoadIntoDB(ScrumDatabase db)
        {
            datab = db;
            db.Surface.AskToSave("You have just received a new Database over the network." +
                "\n Do you want to save the existing data before loading a new database?",
                ContinueLoading);

        }
        private void ContinueLoading()
        {
            datab.ClearItems();
            datab.LoadItems(Epics, UserStories);
        }
    }
}
