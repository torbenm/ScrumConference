using System;
using System.Windows;
using ScrumGestures.Helper;

namespace ScrumGestures
{
    /// <summary>
    /// Stellt einen Berührungspunkt auf der Oberfläche dar
    /// </summary>
    public class TouchPoint : EventArgs
    {
        private const int finger_size = 15;
        private const int object_size = 60;
        private const int table_size = 760;

        #region Getter & Setter
        /// <summary>
        /// Der Punkt auf der Oberfläche, bei dem die Berührung begonnen hat
        /// </summary>
        public Point StartPoint
        {
            get;
            set;
        }
        /// <summary>
        /// Der vorletzte Punkt auf der Oberfläche (vor dem aktuellen)
        /// </summary>
        public Point PreviousPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Der aktuelle Punkt auf der Oberfläche
        /// </summary>
        public Point CurrentPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Aktuelle Zeit, in MS
        /// </summary>
        public long CurrentTimeMS
        {
            get;
            set;
        }

        /// <summary>
        /// Zeitpunkt, bei welchem die Berührung begonnen hat, in MS
        /// </summary>
        public long StartTimeMS
        {
            get;
            set;
        }

        /// <summary>
        /// Bisherige dauer der Berührung, in MS
        /// </summary>
        public int DurationMS
        {
            get { return (int)(CurrentTimeMS - StartTimeMS); }
        }

        /// <summary>
        /// Vektor der Veränderung der Position im Vergleich zur vorletzten Position (PreviousPoint)
        /// </summary>
        public Vector PositionChange
        {
            get
            {
                return VectorExtension.FromPoints(PreviousPoint, CurrentPoint);
            }
        }

        /// <summary>
        /// Die bisher zurückgelegte Entfernung, in Pixel
        /// </summary>
        public float Distance
        {
            get;
            set;
        }

        /// <summary>
        /// Die bisher zurückgelegte Entfernung, in Relation zur Bildschirmgröße (0 bis 1)
        /// </summary>
        public float DistanceRelative
        {
            get;
            set;
        }

        /// <summary>
        /// Anzahl der Positionen, die auf der Oberfläche bisher erkannt wurden, für diese Berührung
        /// </summary>
        public int PathLength
        {
            get;
            set;
        }

        /// <summary>
        /// ID zur Identifikation
        /// </summary>
        public long SessionID
        {
            get;
            set;
        }

        /// <summary>
        /// Der aktuelle Modus
        /// -- UP: Der Finger wurde gerade angehoben/die Berührung hat aufgehört
        /// -- DOWN: Der Finger wurde gerade auf die Oberfläche gelegt / die Berührung hat begonnen
        /// -- MOVE: Der Finger wurde bewegt / Die aktuelle Position hat sich verändert
        /// </summary>
        public TouchMode Mode
        {
            get;
            set;
        }
        #endregion
        public TouchPoint()
        {
            Mode = TouchMode.DOWN;
        }

        /// <summary>
        /// Die Art und Weise der letzten Berührung
        /// </summary>
        public enum TouchMode
        {
            /// <summary>
            /// Der Finger wurde gerade angehoben/die Berührung hat aufgehört
            /// </summary>
            UP, 
            /// <summary>
            /// Der Finger wurde gerade auf die Oberfläche gelegt / die Berührung hat begonnen
            /// </summary>
            DOWN, 
            /// <summary>
            /// Der Finger wurde bewegt / Die aktuelle Position hat sich verändert
            /// </summary>
            MOVE, 
            /// <summary>
            /// Irgendeine Bewegung
            /// </summary>
            ANY
        }


    }
}
