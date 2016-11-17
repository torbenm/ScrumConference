using System;
using System.Reflection;
using System.Windows;
using ScrumTouchkit.Threading;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Events;
using ScrumTouchkit.Controls.Dialogs.UI;


namespace ScrumTouchkit.Controls.Dialogs
{
    /// <summary>
    /// Stellt eine Klasse zur Verfügung, die das Darstellen eines Dialog Fensters ermöglicht
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DialogControl<T> : StandardSurfaceObject where T : Dialog
    {

        #region Getter & Setter
        private T _dialog;
        /// <summary>
        /// No Touch Layer ist ein Layer, der unter das Dialogfenster gelegt wird, so das zu dem Zeitpunkt
        /// nur mit dem Dialog interagiert werden kann
        /// </summary>
        private NoTouchLayer _layer;

        public event EventHandler<DialogEventArgs> DialogFinished;

        /// <summary>
        /// Instant Closing bedeutet, dass sich das DialogFenster bei Klick auf einen Button automatisch schließt.
        /// Bei FALSE können ein paar Berechnungen vorgeschoben werden (wie: sind alle Eingaben korrekt?)
        /// </summary>
        public bool InstantClosing
        {
            get;
            set;
        }

        #endregion
        #region Construcotr
        public DialogControl(ScrumSurface surface) : base(surface)
        {
            InstantClosing = false;

            ConstructorInfo info = typeof(T).GetConstructor(new Type[1]{ typeof(SurfaceObject) });
            _dialog = (T)info.Invoke(new object[1] { this });
            this.Root.Children.Add(_dialog);
            
            _dialog.DialogFinished += _dialog_DialogFinished;

            _layer = new NoTouchLayer(surface);
            
            Surface.SizeChanged += Surface_SizeChanged;
            BringToFront();
            this.Loaded += (s, e) => { this.MoveCenter(surface.ActualWidth / 2, surface.ActualHeight / 2); };
        }
        /// <summary>
        /// Zeigt ein Meldungsdialog mit den angegebenen Button-Optionen an
        /// </summary>
        /// <param name="surface">Die Oberfläche</param>
        /// <param name="dia">Die Buttons, die angezeigt werden sollen (im Nachrichten dialog)</param>
        public DialogControl(ScrumSurface surface, MessageDialog.OptionTypes dia) : base(surface)
        {
            InstantClosing = false;

            ConstructorInfo info = typeof(T).GetConstructor(new Type[2] { typeof(SurfaceObject), typeof(MessageDialog.OptionTypes) });
            _dialog = (T)info.Invoke(new object[2] { this, dia });
            this.Root.Children.Add(_dialog);

            _dialog.DialogFinished += _dialog_DialogFinished;

            _layer = new NoTouchLayer(surface);

            Surface.SizeChanged += Surface_SizeChanged;
            BringToFront();
            this.Loaded += (s, e) => { this.MoveCenter(surface.ActualWidth / 2, surface.ActualHeight / 2); };
        }
        #endregion

        void Surface_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MoveCenter(Surface.ActualWidth / 2, Surface.ActualHeight / 2);
        }
        #region Gestures
        /// <summary>
        /// Dialogfenster können in der Größe nicht verändert werden
        /// </summary>
        public override void InitializeGestures()
        {
            base.InitializeGestures();
            SetGestureActive(RESIZE_GESTURE, false);
        }
        public override void RemoveGestures()
        {
            base.RemoveGestures();
            _dialog.RemoveGestures();
        }
        #endregion
        /// <summary>
        /// Übergibt dem Dialogfenster selber Informationen zur Darstellung
        /// </summary>
        /// <param name="info"></param>
        public void InitializeDialog(DialogInfo info)
        {
            this.Invoke(new Action(() => { _dialog.InitializeDialogContent(info); }));
        }

        private void _dialog_DialogFinished(object sender, DialogEventArgs e)
        {
            if (DialogFinished != null)
                DialogFinished(sender, e);
            if (InstantClosing)
            {
                this.RemoveFromSurface();
            }
            _layer.RemoveFromSurface();
        }

        #region ShowDialog()
        /// <summary>
        /// Erstellt ein DialogControl-Objekt um ein Dialog von Typ T darzustellen
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public static DialogControl<T> ShowDialog(ScrumSurface surface) 
        {
            Func<DialogControl<T>> f = new Func<DialogControl<T>>(() =>
                {
                    DialogControl<T> dia = new DialogControl<T>(surface);
                    return dia;
                });
            if(Application.Current.Dispatcher.CheckAccess())
            {
                return f.Invoke();
            }
            else
                return (DialogControl<T>)Application.Current.Dispatcher.Invoke(f);
        }
        public static DialogControl<T> ShowDialog(ScrumSurface surface, MessageDialog.OptionTypes opts)
        {
            Func<DialogControl<T>> f = new Func<DialogControl<T>>(() =>
            {
                DialogControl<T> dia = new DialogControl<T>(surface, opts);
                return dia;
            });
            if (Application.Current.Dispatcher.CheckAccess())
            {
                return f.Invoke();
            }
            else
                return (DialogControl<T>)Application.Current.Dispatcher.Invoke(f);
        }
        public static DialogControl<T> ShowDialog(ScrumSurface surface, DialogInfo info)
        {
            DialogControl<T> dia = ShowDialog(surface);
            dia.InitializeDialog(info);
            return dia;
        }
        public static DialogControl<T> ShowDialog(ScrumSurface surface, DialogInfo info, MessageDialog.OptionTypes types)
        {
            DialogControl<T> dia = ShowDialog(surface, types);
            dia.InitializeDialog(info);
            return dia;
        }
        #endregion

    }
}
