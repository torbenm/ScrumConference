using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Controls.DisplaySettings;

namespace ScrumTouchkit.Controls.ViewModes.SettingsLoader
{
    /// <summary>
    /// In dieser Klasse werden die DisplaySettings 1-1 geladen -
    /// also genauso, wie sie gespeichert wurden.
    /// </summary>
    public class StandardSettingsLoader : SettingsLoader
    {
        private BaseView _viewMode = null;

        public StandardSettingsLoader(BaseView viewMode) : base(viewMode)
        {
            _viewMode = viewMode;

        }

        /// <summary>
        /// Setzt die DisplaySettings um
        /// </summary>
        /// <param name="settings"></param>
        protected override void LoadSettingsFinally(DisplaySettingsCollection settings)
        {
            ItemControl control = settings.Representation;
            DisplaySettings.DisplaySettings ds = settings.SetDisplaySettings(_viewMode);

            control.MoveCenter(ds.CenterX, ds.CenterY, true);
            control.Rotate(ds.Rotation, true);
            control.Scale(ds.Scale, true);
            
        }
    }
}
