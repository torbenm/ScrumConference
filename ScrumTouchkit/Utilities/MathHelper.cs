using System;
using System.Windows;

namespace ScrumTouchkit.Utilities
{
    /// <summary>
    /// Stellt ein paar Mathematische Funktionen zur Verfügung
    /// </summary>
    public static class MathHelper
    {
        public static Random Random = new Random();
        /// <summary>
        /// Gibt eine zufällige Position auf der Oberfläche zurück
        /// </summary>
        /// <param name="width">Breite der Oberfläche</param>
        /// <param name="height">Höhe der Oberfläche</param>
        /// <returns></returns>
        public static Point GetRandomLocation(double width, double height)
        {
            Point p = new Point();
            p.X = Random.Next((int)width);
            p.Y = Random.Next((int)height);
            return p;
        }

        /// <summary>
        /// Bestimmt einen Punkt auf einem Kreis um einen Punkt.
        /// Der Punkt wird definiert durch den Winkel, in dem er zu der Kreismitte steht
        /// </summary>
        /// <param name="center">Die Kreismitte</param>
        /// <param name="radius">Radius des Kreises</param>
        /// <param name="angle">Der Winkel des Punktes auf dem Kreis</param>
        /// <returns>Der Punkt auf dem Kreis</returns>
        public static Point GetPointOnCircle(Point center, double radius, double angle)
        {
            Point p = new Point();
            p.X = (int)Math.Round(center.X + radius * Math.Cos(angle));
            p.Y = (int)Math.Round(center.Y + radius * Math.Sin(angle));
            return p;
        }
        /// <summary>
        /// Bestimmt einen Zufälligen Punkt auf einem Kreis
        /// </summary>
        /// <param name="center">Position der Kreismitte</param>
        /// <param name="radius">Radius des Kreises</param>
        /// <returns>Ein Punkt auf dem Kreis</returns>
        public static Point GetPointOnCircle(Point center, double radius)
        {
            double angle = Random.NextDouble() * (2 * Math.PI);
            return GetPointOnCircle(center, radius, angle);
        }
    }
}
