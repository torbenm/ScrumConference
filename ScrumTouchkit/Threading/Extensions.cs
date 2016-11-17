using System;
using System.Windows.Threading;

namespace ScrumTouchkit.Threading
{
    /// <summary>
    /// Stellt ein paar Methoden zum schnelleren Aufrufen von Dispatcher
    /// Methoden zur Verfügung
    /// </summary>
    public static class Extensions
    {

        public static void Invoke(this DispatcherObject obj, Action action)
        {
            if (!obj.Dispatcher.CheckAccess())
            {
                obj.Dispatcher.Invoke(action);
            }
            else
                action.Invoke();
        }
        public static T Invoke<T>(this DispatcherObject obj, Func<T> func)
        {
            if (!obj.Dispatcher.CheckAccess())
            {
                return (T)obj.Dispatcher.Invoke(func);
            }
            else
                return func.Invoke();
        }
    }
}
