using ScrumTouchkit.Controls;
using ScrumTouchkit.Data;
using System.Collections.Generic;

namespace ScrumTouchkit.Utilities.Serializer
{
    /// <summary>
    /// Interface, das Methoden zum Laden und Speichern einer ScrumDatabase bereitstellt.
    /// Genauere Beschreibungen der Methoden in den entsprechenden Klassen
    /// </summary>
    public interface IFileManager
    {
        void LoadFile(ScrumDatabase database, string filepath, ScrumSurface surface);

        bool WriteFile(List<Epic> epics, List<UserStory> userstories, string filepath);

    }
}
