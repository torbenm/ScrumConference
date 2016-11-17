using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;

namespace ScrumTouchkit.Threading
{
    /// <summary>
    /// Diese Klasse wird nicht mehr wirklich benutzt.
    /// </summary>
    public class DispatcherClass
    {
        public Dispatcher Dispatcher
        {
            get;
            private set;
        }

        protected Thread _dispatcherThread;

        public DispatcherClass(bool start)
        {
            if (start)
            {
                CreateDispatcherThread();
            }
        }
        #region Initializing Dispatcher
        protected void SetDispatcherThread(Thread thread)
        {
            Dispatcher = System.Windows.Threading.Dispatcher.FromThread(thread);
        }
        protected void CreateDispatcherThread()
        {
            _dispatcherThread = new Thread(new ThreadStart(InitializeDispatcher));
            _dispatcherThread.Start();
        }

        private void InitializeDispatcher()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }
        #endregion
        #region Checking Access
        public bool CheckAccess()
        {
            bool access = true;
            if (Dispatcher != null)
            {
                access = Dispatcher.CheckAccess();
            }
            return access;
        }
        #endregion
        #region Invoking
        public void Invoke(Action action)
        {
           /* if (CheckAccess())
            { */
                action.Invoke();
           /* }
            else
                Dispatcher.Invoke(action); */
        }
        public T Invoke<T>(Func<T> func)
        {
           /* if (CheckAccess())
            { */
                return func.Invoke();
          /*  }
            else 
                return (T)Dispatcher.Invoke(func); */
        }
        #endregion
    }
}
