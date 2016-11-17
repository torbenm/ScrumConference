using System.Windows;

namespace ScrumGestures.Helper
{
    public static class VectorExtension
    {
        /// <summary>
        /// Berechnet einen Vektor als Differenz aus zwei Punkten 
        /// Berechnung: P2 - P1
        /// Bei P1 links liegend von P2 ist somit der Vektor positiv an der X-Position
        /// </summary>
        /// <param name="pt1">Der erste Punkt</param>
        /// <param name="pt2">Der zweite Punkt</param>
        /// <returns>Der Vektor</returns>
        public static Vector FromPoints(Point pt1, Point pt2)
        {          
            return new Vector(pt2.X - pt1.X, pt2.Y - pt1.Y);
        }

    }
}
