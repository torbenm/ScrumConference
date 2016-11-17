using ScrumTouchkit.Data;
using System;

namespace ScrumTouchkit.Controls.ViewModes.SettingsLoader
{
    /// <summary>
    /// Prioritäten werden folgendermaßen geladen
    /// y-Koordinate gibt an wie weit die y koord vom zentrum entfernt ist,
    /// max. prio * smallest_side
    /// x-koordinate - x/2 gibt an ob links oder rechts davon
    /// danach satz des pythagoras
    /// </summary>
    public class PriorityLoader : SettingsLoader
    {

        public PriorityLoader(BaseView viewMode) : base(viewMode)
        {

        }

        protected override void LoadSettingsFinally(DisplaySettings.DisplaySettingsCollection settings)
        {
            UserStory us = settings.Representation.Item as UserStory;
            if (us != null)
            {
                DisplaySettings.DisplaySettings ds = settings.SetDisplaySettings(viewMode);

                /***
                 * Prioritäten werden folgendermaßen geladen
                 * y-Koordinate gibt an wie weit die y koord vom zentrum entfernt ist,
                 * max. prio * smallest_side
                 * x-koordinate - x/2 gibt an ob links oder rechts davon
                 * danach satz des pythagoras
                 * **/

                double smallest_side = (viewMode.Surface.ActualHeight > viewMode.Surface.ActualWidth) ?
                    viewMode.Surface.ActualWidth : viewMode.Surface.ActualHeight;

                double y_dist = ds.CenterY - (viewMode.Surface.ActualHeight / 2);
                bool add_y = y_dist > 0;
                bool add_x = (ds.CenterX - (viewMode.Surface.ActualWidth / 2)) > 0;
                double mid_dist = us.Priority * (smallest_side / 2);

                y_dist = Math.Abs(y_dist);
                y_dist = y_dist > mid_dist ? mid_dist : y_dist;

                double x_dist = Math.Sqrt(mid_dist * mid_dist - y_dist * y_dist);

                double x = (viewMode.Surface.ActualWidth / 2);
                double y = (viewMode.Surface.ActualHeight / 2);
                x += add_x ? x_dist : -x_dist;
                y += add_y ? y_dist : -y_dist;

                settings.Representation.Scale(ds.Scale, true);
                settings.Representation.Rotate(ds.Rotation, true);
                
                settings.Representation.MoveCenter(x, y, true);
            }
            
        }
    }
}
