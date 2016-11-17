using System;
namespace ScrumGestures.Events
{

    /// <summary>
    /// Generische EventArgs
    /// </summary>
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
