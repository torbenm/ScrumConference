using ScrumTouchkit.Controls.Abstract;
using System.Windows;

namespace ScrumTouchkit.Controls.ViewModes.SettingsLoader
{

    /// <summary>
    /// In den verschiedene Ansichten werden die DisplaySettings nicht einfach nur 
    /// geladen, sondern sie unterscheiden sich in der Art und Weise.
    /// So fließt bei der Prioritäts-Ansicht zum Beispiel auch die Priorität
    /// in die Berechnung der Position.
    /// Mit Klassen, die von dieser erben, lässt sich dies realisieren.
    /// </summary>
    public abstract class SettingsLoader
    {
        protected BaseView viewMode = null;

        public SettingsLoader(BaseView viewMode)
        {
            this.viewMode = viewMode;

        }

        public void LoadSettings(DisplaySettings.DisplaySettingsCollection settings)
        {
            ItemControl control = settings.Representation;
            // Nur laden wenn das ItemControl von WPF vollständig geladen ist!
            //sonst kommt es zu fehlern
            if (control.IsLoaded)
                LoadSettingsFinally(settings);
            else
            {
                //Falls noch nicht geladen -> One Time Check wenn geladen
                RoutedEventHandler eh = null;
                eh = (s, e) =>
                {
                    LoadSettingsFinally(settings);
                    control.Loaded -= eh;
                };
                control.Loaded += eh;
            }
        }
        protected abstract void LoadSettingsFinally(DisplaySettings.DisplaySettingsCollection settings);

    }
}
