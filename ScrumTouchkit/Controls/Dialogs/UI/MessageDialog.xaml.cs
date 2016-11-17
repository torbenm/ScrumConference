using ScrumGestures;
using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Abstract;
using System.Windows;
using System.Windows.Media;


namespace ScrumTouchkit.Controls.Dialogs.UI
{
    /// <summary>
    /// Einfaches DIalogfenster, in dem Nachrichten angezeigt werden können.
    /// </summary>
    public partial class MessageDialog : Dialog
    {
        #region enum Options
        public enum OptionTypes
        {
            /// <summary>
            /// Einen Button für Yes und einen für NO
            /// </summary>
            YesNo,
            /// <summary>
            /// Nur einen schließen Button
            /// </summary>
            Close
        }
        #endregion
        #region const
        //IDs für die verschiedenen Buttons
        public const int CLOSE = 1;
        public const int YES = 2;
        public const int NO = 3;
        #endregion
        #region var, get, set
        public OptionTypes Options
        {
            get;
            private set;
        }
        #endregion
        #region constructor, init
        public MessageDialog(SurfaceObject parent)
            : this(parent, OptionTypes.Close)
        { }
        public MessageDialog(SurfaceObject parent, OptionTypes type) :base(parent)
        {
            InitializeComponent();
            this.Options = type;
            _opt1.Click += _opt1_Click;
            _opt2.Click += _opt2_Click;
            InitButtons();

        }

        private void InitButtons()
        {
            if (this.Options == OptionTypes.Close)
            {
                this._opt1.Visibility = System.Windows.Visibility.Hidden;
                this._opt2.Content = "Close";
            }
        }

        void _opt2_Click(object sender, RoutedEventArgs e)
        {
            Option2Clicked();
        }

        void _opt1_Click(object sender, RoutedEventArgs e)
        {
            Option1Clicked();
        }
        public override void InitializeDialogContent(DialogInfo info)
        {
            if (info.Mode == DialogInfo.ERROR)
            {
                _title.Text = "ERROR";
                this.Background = new SolidColorBrush(Color.FromRgb(197, 47, 47));
            }
            else
            {
                _title.Text = "INFORMATION";
            }
            _message.Text = info.Message;
        }
        protected override void InitializeDialog()
        {
            ((DialogControl<MessageDialog>)DialogParent).InstantClosing = true;
            DialogParent.Width = 440;
            DialogParent.Height = 204;
        }
        #endregion
        #region Gestures
        public override void InitializeGestures()
        {
           AddGesture("tap_1", this._opt1, DefinedGestures.Tap, Opt1Callback);
           AddGesture("tap_2", this._opt2, DefinedGestures.Tap, Opt2Callback);
        }
        private void Opt1Callback(UIElement element, TouchGroup points)
        {
            Option1Clicked();
        }
        private void Opt2Callback(UIElement element, TouchGroup points)
        {
            Option2Clicked();
        }

        #endregion
        #region Button Callback
        protected void Option1Clicked()
        {
            if (this.Options == MessageDialog.OptionTypes.YesNo)
            {
                OnDialogFinished(new Events.DialogEventArgs(YES, this));
            }
        }
        protected void Option2Clicked()
        {
            switch (this.Options)
            {
                case OptionTypes.YesNo:
                    OnDialogFinished(new Events.DialogEventArgs(NO, this));
                    break;
                case OptionTypes.Close:
                    OnDialogFinished(new Events.DialogEventArgs(CLOSE, this));
                    break;
            }
        }
        #endregion
        #region ShowMessage
        public static void ShowMessage(ScrumSurface surface, string message, bool err)
        {
                DialogControl<MessageDialog>.ShowDialog(surface, DialogInfo.CreateMessageInfo(message, err));
        }
        public static DialogControl<MessageDialog> ShowMessage(ScrumSurface surface, string message, bool err, OptionTypes opts)
        {
            return DialogControl<MessageDialog>.ShowDialog(surface, DialogInfo.CreateMessageInfo(message, err), opts);
        }
        #endregion
    }
}
