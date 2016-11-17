using ScrumGestures;
using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Threading;
using System;
using System.Windows;

namespace ScrumTouchkit.Controls.Dialogs.UI
{
    /// <summary>
    /// Im ChooseEndPointType-Dialog wird gewählt, ob man einen neuen
    /// Server erstellen möchte oder sich als Client bei einem bestehenden Server anmelden will.
    /// </summary>
    public partial class ChooseEndPointType : Dialog
    {
        #region const
        public const int SERVER = 1;
        public const int CLIENT = 2;
        #endregion

        public ChooseEndPointType(SurfaceObject obj) : base(obj)
        {
            InitializeComponent();

        }
        #region Gestures
        public override void InitializeGestures()
        {
            this.AddGesture("server_tap", _serverrect, DefinedGestures.Tap, ServerTappedCallback);
            this.AddGesture("client_tap", _clientrect, DefinedGestures.Tap, ClientTappedCallback);
        }

        private void ServerTappedCallback(UIElement element, TouchGroup points)
        {
            PickType(SERVER);
        }

        private void ClientTappedCallback(UIElement element, TouchGroup points)
        {
            PickType(CLIENT);
        }
        #endregion
        /// <summary>
        /// Die andere, nicht gewählte Option wird hier ausgeblendet.
        /// Die gewählte Option is zunächst noch etwas zu sehen und blendet sich dann auch langsam aus.
        /// </summary>
        /// <param name="mode"></param>
        private void PickType(int mode)
        {

            OnDialogFinished(new Events.DialogEventArgs(mode, this));

            this.Invoke(new Action(() =>
            {
                if (mode == SERVER)
                {
                    _clienttext.Opacity = 0;
                    _clientrect.Opacity = 0;
                }
                else
                {
                    _servertext.Opacity = 0;
                    _serverrect.Opacity = 0;
                }
                Animation.Animator.FadeOut(DialogParent, (s, e) => { DialogParent.RemoveFromSurface();  });
            }));

        }
        #region Other Implementation
        protected override void InitializeDialog()
        {
            DialogParent.Width = 400;
            DialogParent.Height = 180;
        }
        public override void InitializeDialogContent(DialogInfo info)
        {

        }
        #endregion


        
    }
}
