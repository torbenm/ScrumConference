using ScrumGestures;
using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Threading;
using System;
using System.Net;
using System.Windows;


namespace ScrumTouchkit.Controls.Dialogs.UI
{
    /// <summary>
    /// Stellt einen Dialog zum Erstellen eines Clients dar
    /// </summary>
    public partial class CreateClient : Dialog
    {
        #region const
        public const int CANCEL = 1;
        public const int CONNECT = 2;
        #endregion
        #region constructor
        public CreateClient(SurfaceObject obj) : base(obj)
        {
            InitializeComponent();
            this._port.Text = Utilities.Settings.Default.STD_TCP_PORT.ToString();
            this._connect.Click += _connect_Click;
            this._cancel.Click += _cancel_Click;
        }
        #endregion

        #region Gestures
        public override void InitializeGestures()
        {
           AddGesture("cancel", _cancel, DefinedGestures.Tap, CancelCallback);
            AddGesture("connect", _connect, DefinedGestures.Tap, ConnectCallback);
        }
        void _cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelCallback(null, null);
        }

        void _connect_Click(object sender, RoutedEventArgs e)
        {
            ConnectCallback(null, null);
        }


        /// <summary>
        /// Überprüft, ob die Angaben im Dialog korrekt sind und beendet anschließend (ggf.) den DIalog
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        private void ConnectCallback(UIElement element, TouchGroup points)
        {
            this.Invoke(new Action(() =>
            {
                try
                {
                    IPAddress ip = IPAddress.Parse(_ipaddress.Text);
                    int port = int.Parse(_port.Text);
                    if (port > 0 && port <= 9999)
                    {
                        OnDialogFinished(new Events.DialogEventArgs(this, CONNECT, port, ip));
                    }
                    else
                    {
                        DialogControl<MessageDialog>.ShowDialog(DialogParent.Surface, 
                            DialogInfo.CreateMessageInfo("The port must be between 0 and 9999!", true));
                    }
                }
                catch
                {
                    DialogControl<MessageDialog>.ShowDialog(DialogParent.Surface, 
                        DialogInfo.CreateMessageInfo(this._ipaddress.Text + " is not a valid ip adress", true));
                }
            }));
        }

        private void CancelCallback(UIElement element, TouchGroup points)
        {
            OnDialogFinished(new Events.DialogEventArgs(CANCEL, this));
        }

        public override void InitializeDialogContent(DialogInfo info)
        {
            
        }
        #endregion
        protected override void InitializeDialog()
        {
            DialogParent.Width = 514;
            DialogParent.Height = 210;
            ((DialogControl<CreateClient>)DialogParent).InstantClosing = true;
        }
    }
}
