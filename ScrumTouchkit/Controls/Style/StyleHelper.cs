using ScrumTouchkit.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ScrumTouchkit.Controls.Style
{
    /// <summary>
    /// In dieser Klasse werden ein paar Hilfsfunktionen für Styles bereit gestellt
    /// </summary>
    public static class StyleHelper
    {
       /// <summary>
       /// Erstellt einen Schlagschatten, der wie ein Leuchtrahmen aussieht
       /// </summary>
       /// <param name="col">Die Farbe für den Leuchtrahmen</param>
       /// <returns></returns>
       public static DropShadowEffect GetOuterGlow(Color col)
        {
            DropShadowEffect effect = new DropShadowEffect();
            effect.BlurRadius = 15;
            effect.ShadowDepth = 0;
            effect.Opacity = 1;
            effect.Direction = 0;
            effect.Color = col;
          
            return effect;
        }

        /// <summary>
        /// Lädt einen Leuchtrahmen mit einer Standard-Farbe
        /// </summary>
        /// <returns></returns>
       public static DropShadowEffect GetOuterGlow()
       {
           return GetOuterGlow(Colors.ObjectGlowLocal);
       }

        /// <summary>
        /// Gibt einen Brush für den BacklogStatus eines Items zurück
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static Brush GetBackgroundBrush(ItemBacklogStatus status)
        {
            return new SolidColorBrush(GetBackgroundColor(status));
        }

        /// <summary>
        /// Gibt einen Brush für den BacklogStatus eines Items zurück
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Brush GetBackgroundBrush(ItemBase item)
        {
            return new SolidColorBrush(GetBackgroundColor(item));
        }

        /// <summary>
        /// Gibt eine Farbe für den BacklogStatus eines Items zurück
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Color GetBackgroundColor(ItemBase item)
        {
            UserStory us = item as UserStory;
            if (us != null)
                return GetBackgroundColor(us.BacklogStatus);
            else
                return Color.FromRgb(255, 255, 255);
        }

        /// <summary>
        /// Gibt einen Brush für den BacklogStatus eines Items zurück
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static Color GetBackgroundColor(ItemBacklogStatus status)
        {
            Color c;
            switch (status)
            {
                case ItemBacklogStatus.SPRINT_BACKLOG:
                    c = Colors.ObjectSprintBacklog;
                    break;
                default:
                case ItemBacklogStatus.PRODUCT_BACKLOG:
                    c = Colors.ObjectProductBacklog;
                    break;
            }
            return c;
        }
    }
}
