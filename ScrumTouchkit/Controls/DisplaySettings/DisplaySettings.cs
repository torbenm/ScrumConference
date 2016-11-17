using ScrumTouchkit.Controls.Abstract;
using System;

namespace ScrumTouchkit.Controls.DisplaySettings
{
    /// <summary>
    /// Speichert die Position, Drehung von Größe von Element in den verschiedenen Ansichten
    /// </summary>
    public class DisplaySettings
    {

        /// <summary>
        /// Gibt an, ob das Element bereits durch die Ansicht initialisiert wurde
        /// oder ob es sich um Standardwerte handelt
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        public bool Initialized
        {
            get;
            set;
        }

        /// <summary>
        /// Mitte des Elements auf der X-Achse
        /// </summary>
        public double CenterX
        {
            get;
            set;
        }

        /// <summary>
        /// Mitte des Elements auf der Y-Achse
        /// </summary>
        public double CenterY
        {
            get;
            set;
        }

        /// <summary>
        /// Skalierung des Elements
        /// </summary>
        public double Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Drehung des Elements
        /// </summary>
        public double Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// Kopiert die DisplaySettings
        /// </summary>
        /// <returns></returns>
        public DisplaySettings Copy()
        {
            return new DisplaySettings
            {
                CenterX = this.CenterX,
                CenterY = this.CenterY,
                Scale = this.Scale,
                Rotation = this.Rotation
            };
        }

        /// <summary>
        /// Aktualisiert das Objekt auf der Oberfläche, um
        /// die hier gespeicherten Einstellungen zu übernehmen
        /// </summary>
        /// <param name="obj"></param>
        public void Update(SurfaceObject obj)
        {
            obj.Dispatcher.Invoke((Action)(() =>
                {
                    this.CenterX = obj.CenterX;
                    this.CenterY = obj.CenterY;
                    this.Rotation = obj.RotateTransform.Angle;
                    this.Scale = obj.ScaleFactor;
                }));
        }
    }
}
