using ScrumTouchkit.Threading;
using ScrumGestures;
using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Abstract;
using System;
using System.Windows;


namespace ScrumTouchkit.Controls.Dialogs.UI
{
    /// <summary>
    /// Im Einstellungsdialog können einige Einstellungen festgelegt werden.
    /// Unteranderem:
    /// - TCP Port
    /// - Name des Clients
    /// - UDP Port
    /// </summary>
    public partial class SettingsDialog : Dialog
    {

        #region const
        public const int CANCEL = 1;
        public const int SAVE = 2;
        #endregion
        #region Constructor

        public SettingsDialog(SurfaceObject parent) : base(parent)
        {
            InitializeComponent();
            this._clientname.Text = Utilities.Settings.Default.CLIENT_NAME;
            this._tcpport.Text = Utilities.Settings.Default.STD_TCP_PORT.ToString();
            this._udpport.Text = Utilities.Settings.Default.UDP_PORT.ToString();

            this._cancel.Click += _cancel_Click;
            this._save.Click += _save_Click;
        }
        #endregion

        #region Event Callbacks
        private void _save_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings(null, null);
        }

        private void _cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelCallback(null, null);
        }
        #endregion

        #region Gestures

        public override void InitializeGestures()
        {
           AddGesture("save", _save, DefinedGestures.Tap, SaveSettings);
           AddGesture("cancel", _cancel, DefinedGestures.Tap, CancelCallback);
        }
        private void CancelCallback(UIElement element, TouchGroup points)
        {
            OnDialogFinished(new Events.DialogEventArgs(CANCEL, this));
        }

        private void SaveSettings(UIElement element, TouchGroup points)
        {
            this.Invoke(new Action(() =>
            {
                int tcp_port;
                int udp_port;
                string client_name = _clientname.Text;

                if (client_name.Length > 0)
                {
                    if (int.TryParse(_tcpport.Text, out tcp_port) & tcp_port < 9999 & tcp_port > 0)
                    {
                        if (int.TryParse(_udpport.Text, out udp_port) & udp_port < 9999 & udp_port > 0)
                        {
                            if (udp_port != tcp_port)
                            {
                                //Endlich sind alle Bedingungen erfüllt -> Daten speichern
                                Utilities.Settings.Default.CLIENT_NAME = client_name;
                                Utilities.Settings.Default.STD_TCP_PORT = tcp_port;
                                Utilities.Settings.Default.UDP_PORT = udp_port;
                                Utilities.Settings.Default.Save();

                                OnDialogFinished(new Events.DialogEventArgs(SAVE, this));
                            }
                            else
                            {
                                MessageDialog.ShowMessage(DialogParent.Surface, "UDP and TCP Port can not be the same", true);
                            }
                        }
                        else
                        {
                            MessageDialog.ShowMessage(DialogParent.Surface, "The UDP Port has to be a valid number between 0 and 9999", true);
                        }
                    }
                    else
                    {
                        MessageDialog.ShowMessage(DialogParent.Surface, "The TCP Port has to be a valid number between 0 and 9999", true);
                    }
                }
                else
                {
                    MessageDialog.ShowMessage(DialogParent.Surface, "You have to enter a name for this client.", true);
                }
            }));
        }


        #endregion
        public override void InitializeDialogContent(DialogInfo info)
        {
            
        }

        protected override void InitializeDialog()
        {
            ((DialogControl<SettingsDialog>)DialogParent).InstantClosing = true;
            DialogParent.Width = 300;
            DialogParent.Height = 346;
        }
    }
}
