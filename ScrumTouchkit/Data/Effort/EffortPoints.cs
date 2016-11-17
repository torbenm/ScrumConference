using ScrumTouchkit.Events;
using System;

namespace ScrumTouchkit.Data.Effort
{

    /// <summary>
    /// Diese Klasse stellt EffortPoints zur Verwendung in User Stories dar
    /// </summary>
    public class EffortPoints : EventArgs
    {
        #region static
        /// <summary>
        /// Vordefinierte EffortPoints:
        /// 1, 2, 3, 5, 8, 13, 20, 40 und Infinity
        /// </summary>
        public static readonly EffortPoints[] PreDefined = new EffortPoints[]
        {
            new EffortPoints(1),
            new EffortPoints(2),
            new EffortPoints(3),
            new EffortPoints(5),
            new EffortPoints(8),
            new EffortPoints(13),
            new EffortPoints(20),
            new EffortPoints(40),
            new EffortPoints(INFINITY)
        };
        /// <summary>
        /// Unendlich wird mit -1 dargestellt
        /// </summary>
        public const int INFINITY = -1;
        #endregion
        #region vars, get, set
        private int _value = 1;
        
        /// <summary>
        /// Der zugewiese Wert dieser EffortPoints
        /// </summary>
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetValue(value);
            }
        }

        #endregion
        #region Constructor
        /// <summary>
        /// Initialisiert das EffortPoints-Objekt mit einem Wert
        /// </summary>
        /// <param name="val"></param>
        public EffortPoints(int val)
        {
            this._value = val >= 0 ? val : INFINITY;
        }
        /// <summary>
        /// Keine Initialisierung -> Value = INFITINITY
        /// </summary>
        public EffortPoints()
        { }
        #endregion
        #region Events
        public event EventHandler<EffortPointsChangedEventArgs> EffortPointsChanged;

        protected void OnEffortPointsChanged(EffortPoints oldVal, bool external)
        {
            if (EffortPointsChanged != null)
            {
                EffortPointsChanged(this,
                    new EffortPointsChangedEventArgs
                    {
                        Old = oldVal,
                        New = this,
                        ExternallyTriggered = external
                    });
            }
        }
        #endregion
        #region Update
        /// <summary>
        /// Wenn diese Funktion (mit EffortPoints als Parameter) aufgerufen wird,
        /// stammen die Daten immer aus dem Netzwerk
        /// </summary>
        /// <param name="val"></param>
        public void SetValue(EffortPoints val)
        {
            // Aus dem Netzwerk -> External: TRUE
            SetValue(val.Value, true);
        }
        
        /// <summary>
        /// Von diesem Computer kommen die Daten immer als INT!
        /// </summary>
        /// <param name="val"></param>
        private void SetValue(int val)
        {
            //Von diesem Computer -> External : FALSE
            SetValue(val, false);
        }

        /// <summary>
        /// Ändert den Wert und Ruft OnEffortsChanged() mit dem entsprechenden
        /// Wert für External auf
        /// </summary>
        /// <param name="newVal"></param>
        /// <param name="external"></param>
        private void SetValue(int newVal, bool external)
        {
            EffortPoints oldVal = Copy();
            this._value = newVal >= 0 ? newVal : INFINITY;
            OnEffortPointsChanged(oldVal, external);
        }
        #endregion
        #region Operators
        /// <summary>
        /// Addiert zwei EffortPoints aufeinander
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static EffortPoints operator +(EffortPoints instance, EffortPoints b)
        {
            if (instance.Value == INFINITY || b.Value == INFINITY)
            {
                instance.Value = INFINITY;
            }
            else
            {
                instance.Value += b.Value;
            }
            return instance;
        }

        /// <summary>
        /// Wandelt die EffortPoints in einen String um
        /// Für Unendlich wird dabei das Unicode-Symbol für Unendlichkeit genommen
        /// (\u221E)
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static implicit operator string(EffortPoints instance)
        {
            if (instance.Value >= 0)
                return instance.Value.ToString();
            else
                return "\u221E";
        }
        #endregion
        #region Copy
        /// <summary>
        /// Kopiert EffortPoints
        /// </summary>
        /// <returns></returns>
        public EffortPoints Copy()
        {
            return new EffortPoints(this.Value);
        }
        #endregion
    }
}
