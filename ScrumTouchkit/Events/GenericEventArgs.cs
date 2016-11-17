using System;
namespace ScrumTouchkit.Events
{
    /// <summary>
    /// Stellt generische EventArgs zur Verfügung,
    /// um beliebige Klassen und Objekte als EventArgs zu verwenden.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericEventArgs<T> : EventArgs
    {
        public T Value
        {
            get;
            private set;
        }

        public GenericEventArgs(T val)
        {
            this.Value = val;
        }
    }
}
