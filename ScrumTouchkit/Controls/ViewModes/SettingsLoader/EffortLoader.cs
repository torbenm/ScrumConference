using ScrumTouchkit.Data;
using ScrumTouchkit.Data.Effort;

namespace ScrumTouchkit.Controls.ViewModes.SettingsLoader
{
    /// <summary>
    /// Lädt die DisplaySettings für die Effort-Ansicht.
    /// Hierbei wird nur die Y-Position ungefiltert übernommen. 
    /// Bei der X-Position wird zunächst überprüft, ob sie mit dem 
    /// Aufwand der User Story und den gültigen Position für die entsprechende
    /// Spalte übereinstimmt.
    /// Wenn dies der Fall ist, wird sie übernommen.
    /// Ist dies nicht der Fall, wird die User Story mittig in der SPalte platziert.
    /// </summary>
    public class EffortLoader : SettingsLoader
    {
        public EffortLoader(EffortView mode)
            : base(mode)
        { }
        protected override void LoadSettingsFinally(DisplaySettings.DisplaySettingsCollection settings)
        {
            UserStory us = settings.Representation.Item as UserStory;
            EffortView eview = viewMode as EffortView;
            if (us != null)
            {
                DisplaySettings.DisplaySettings ds = settings.SetDisplaySettings(viewMode);
                settings.Representation.Scale(ds.Scale, true);
                settings.Representation.Rotate(ds.Rotation, true);

                int eff_ind = GetEffortIndex(us.Effort);
                double width = viewMode.Surface.ActualWidth / 9;
                double x = ds.CenterX;
                double min_x = width * eff_ind;
                double max_x = width * eff_ind;
                double mid_x = min_x + (width / 2);
                x = (x >= min_x && x < max_x) ? x : mid_x;

                settings.Representation.MoveCenter(x, ds.CenterY, true);
            }
        }

        /// <summary>
        /// Gibt den Index für die Spalte des gewünschten Aufwands an
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private int GetEffortIndex(EffortPoints pt)
        {
            for(int i = 0; i < EffortPoints.PreDefined.Length; i++)
            {
                if(EffortPoints.PreDefined[i].Value == pt.Value)
                    return i;
            }
            return 0;
        }
    }
}
