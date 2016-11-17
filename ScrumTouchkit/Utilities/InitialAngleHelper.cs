using System.Windows;

namespace ScrumTouchkit.Utilities
{
    /// <summary>
    /// Stellt eine Funktion bereit,
    /// um die Anfangsdrehung einer UserStory zu bestimmen,
    /// nachdem sie erstellt wurde.
    /// </summary>
    public static class InitialAngleHelper
    {
        /// <summary>
        /// Berechnet die Anfangsdrehung einer User Story abhängig von Ihrer Startposition.
        /// Da Nutzer dazu tendieren, User Stories in Ihrer unmittelbaren Nähe zu erstellen (Scott, 2004)
        /// kann man durch die Startposition abschätzen, wo die erstellende Person steht.
        /// -------------------------------------------------------------------------
        /// |               |                                       |               |
        /// |    135°       |                                       |     225°      |
        /// |               |                                       |               |
        /// |               |                180°                   |               |
        /// |---------------|                                       |---------------|
        /// |               |                                       |               |
        /// |               |                                       |               |
        /// |     90°       |---------------------------------------|     270°      |
        /// |               |                                       |               |
        /// |               |                                       |               |
        /// |---------------|                                       |---------------|
        /// |               |                 0°                    |               |
        /// |      45°      |                                       |     315°      |
        /// |               |                                       |               |
        /// |               |                                       |               |
        /// -------------------------------------------------------------------------
        /// </summary>
        /// <param name="w">Die Breite der Oberfläche</param>
        /// <param name="h">Die Höhe der Oberfläche</param>
        /// <param name="pt">Der Punkt, an dem die User Story erstellt wurde</param>
        /// <returns></returns>
        public static double GetInitialAngle(double w, double h, Point pt)
        {
            if (pt.X < w / 4)
            {
                //LINKS
                if (pt.Y < h / 4)
                {
                    return 135;
                }
                else if (pt.Y - h / 4 < h / 2)
                {
                    return 90;
                }
                else
                {
                    return 45;
                }
            }
            else if (pt.X - (w / 4) < w / 2)
            {
                //MITTE
                if (pt.Y < h / 2)
                {
                    return 180;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                //RECHTS
                if (pt.Y < h / 4)
                {
                    return 225;
                }
                else if (pt.Y - h / 4 < h / 2)
                {
                    return 270;
                }
                else
                {
                    return 315;
                }
            }
        }
    }
}
