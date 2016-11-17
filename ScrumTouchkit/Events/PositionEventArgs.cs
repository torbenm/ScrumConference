using System;

namespace ScrumTouchkit.Events
{

    /// <summary>
    /// Wird versendet, wennn sich ein Objekt auf der Oberfläche bewegt hat
    /// </summary>
    public class MovedEventArgs : EventArgs
    {
        /// <summary>
        /// Die Ziel-X-Koordinaten
        /// </summary>
        public double X
        {
            get;
            set;
        }

        /// <summary>
        /// Die Ziel-Y-Koordinaten
        /// </summary>
        public double Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt an, ob automatisch (mit dem Programm) oder manuell (durch den User) bewegt
        /// </summary>
        public bool ByUser
        {
            get;
            set;
        }
        public MovedEventArgs(double x, double y, bool bu = false)
        {
            X = x;
            Y = y;
            ByUser = bu;
        }


    }
}
