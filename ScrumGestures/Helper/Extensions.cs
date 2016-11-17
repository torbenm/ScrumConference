using System;
using System.Collections.Generic;
using System.Windows;
using TUIO;

namespace ScrumGestures
{
    public static class Extensions
    {
        /// <summary>
        /// Verwandelt einen TuioPoint in einen C#-Point
        /// </summary>
        /// <param name="self">Der TuioPoint</param>
        /// <param name="manager">Der GestureManager, um die Größe des Screens abzurufen</param>
        /// <returns>Der C#-Point</returns>
        public static Point ToPoint(this TuioPoint self, GestureManager manager)
        {
            int x = self.getScreenX((int)manager.Root.ActualWidth);
            int y = self.getScreenY((int)manager.Root.ActualHeight);
           
            return new Point(x, y);
        }

        /// <summary>
        /// Berechnet die Entferung zweier TuioPoints in Pixeln
        /// </summary>
        /// <param name="self">Der erste TuioPoint</param>
        /// <param name="other">Der zweite TuioPoint</param>
        /// <param name="manager">Der GestureManager</param>
        /// <returns>Die Entfernung in Pixel</returns>
        public static float GetPixelDistance(this TuioPoint self, TuioPoint other, GestureManager manager)
        {
            Point pt = self.ToPoint(manager);
            Point pt2 = other.ToPoint(manager);
            return pt.GetDistance(pt2);
        }

        /// <summary>
        /// Berechnet die Entferung zweier Punkte auf der Oberfläche 
        /// mithilfe vom Satz des Pythagoras.
        /// </summary>
        /// <param name="pt">Der erste Punkt</param>
        /// <param name="pt2">Der zweite Punkt</param>
        /// <returns>Die Entfernung in Pixel</returns>
        public static float GetDistance(this Point pt, Point pt2)
        {
            double dx = pt.X - pt2.X;
            double dy = pt.Y - pt2.Y;

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Berechnet die Summe der Entfernungen der Einzelsegmente eines TuioCursors.
        /// In anderen Worten, die bisher zurückgelegte Entfernung einer Berührung
        /// </summary>
        /// <param name="self">Der TuioCursor</param>
        /// <returns>Die berechnete Entfernung, in Relation zur Bildschirmgröße (0 bis 1)</returns>
        public static float getTotalDistance(this TuioCursor self)
        {
            float dist = 0;
            TuioPoint last = null;
            List<TuioPoint> allPoints = self.getPath();
            for (int i = 0; i < allPoints.Count; i++)
            {
                TuioPoint tp = allPoints[i];
                if (last != null)
                {
                    dist += tp.getDistance(last);
                }
                last = tp;
            }
            return dist;
        }
        /// <summary>
        /// Berechnet die Summe der Entfernungen der Einzelsegmente eines TuioCursors.
        /// In anderen Worten, die bisher zurückgelegte Entfernung einer Berührung
        /// </summary>
        /// <param name="self">Der TuioCursor</param>
        /// <returns>Die berechnete Entfernung, in Pixel</returns>
        public static float getTotalDistanceInPixel(this TuioCursor self, GestureManager manager)
        {
            float dist = 0;
            TuioPoint last = null;
            foreach (TuioPoint tp in self.getPath())
            {
                if (last != null)
                {
                    dist += tp.GetPixelDistance(last, manager);
                }
                last = tp;
            }
            return dist;
        }
    }
}
