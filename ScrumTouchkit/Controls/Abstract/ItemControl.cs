using ScrumTouchkit.Data;
using System;
using ScrumTouchkit.Threading;
using System.Windows;
using ScrumTouchkit.Controls.Content.Abstract;
using ScrumGestures.Gestures;
using ScrumGestures;
using System.Windows.Media;
using ScrumTouchkit.Events;
using ScrumTouchkit.Controls.Network;
using ScrumTouchkit.Controls.DisplaySettings;
using ScrumTouchkit.Utilities;

namespace ScrumTouchkit.Controls.Abstract
{

    /// <summary>
    /// Definiert Funktionen, die sowohol die EpicControl aus auch die UserStoryControl
    /// benötigen. Wichtig:
    /// Erbende Klassen müssen am Ende des Konstrukturs die geerbte Methode "AfterInit" aufrufen.
    /// </summary>
    public abstract class ItemControl : StandardSurfaceObject
    {

        #region States
        public const int STATE_VIEW = 0x00;
        public const int STATE_EDITOR = 0x01;
        #endregion
        #region vars, getter, setter
        /// <summary>
        /// Das grundlegende Interface. In diesem wird bspw. die Form (Kreis, Ellipse, Rechteck) und der Hintergrund festgelegt.
        /// </summary>
        public BaseUI UserInterface
        { get; set; }

        /// <summary>
        /// Im StdView wird die standardmäßige Ansicht gespeichert. Diese zeigt alle 
        /// relevanten Inhalte an.
        /// </summary>
        protected IContent StdView = null;

        /// <summary>
        /// Im EditorView wird die Bearbeitungsansicht gespeichert.
        /// </summary>
        protected Editor EditorView = null;

        /// <summary>
        /// State gibt an, in welchem View (StdView, EditorView, ein anderer View) man sich gereade befindet
        /// </summary>
        protected int State = STATE_VIEW;


        /// <summary>
        /// Das dieser ItemControl zugeordnetete Item
        /// </summary>
        public ItemBase Item
        {
            get;
            set;
        }

        /// <summary>
        /// Die Sammlung an DisplaySettings für diese Darstellung eines Items
        /// </summary>
        public DisplaySettingsCollection DisplaySettings
        {
            get;
            set;
        }

        /// <summary>
        /// Default Settings definieren die initialen Einstellungen der DisplaySettings
        /// </summary>
        protected static DisplaySettings.DisplaySettings _defaultSettings = new DisplaySettings.DisplaySettings
            {
                CenterX = 200,
                CenterY = 200,
                Rotation = 30,
                Scale = 0.3
            };
        public static DisplaySettings.DisplaySettings StaticDefaultSettings
        {
            get { return _defaultSettings; }
        }
        #endregion
        #region Constructor
        public ItemControl(ItemBase data, ScrumSurface surface)
            : base(surface)
        {
            Item = data;
            Item.AddRepresentation(this);
            Item.ExternalDataChanged += ExternalDataChanged;
            this.DisplaySettings = new DisplaySettingsCollection(this);
            this.ScaleChanged += ItemControl_ScaleChanged;
            this.RotateTransform.Changed += RotateTransform_Changed;
            Animation.Animator.FadeIn(this);
            this.Moved += ItemControl_Moved;
            AutoPos();
        }

       

        protected virtual void ExternalDataChanged(object sender, EventArgs e)
        {
            if (UserInterface != null)
                UserInterface.UpdateData(Item);
        }
        /// <summary>
        /// Wird nach der Initialisierung aufgerufen (muss von den erbenden Klassen gemacht werden!)
        /// </summary>
        protected void AfterInit()
        {
            Root.Children.Add(UserInterface);
            this.Width = UserInterface.Width;
            this.Height = UserInterface.Height;
            ShowContent();
            UpdateCenter(null, null);
        }
        #endregion
        #region Event Handling

        /***
         * Jede relevante Änderung der Skalierung, Drehung und Position wird
         * in den DisplaySettings festgehalten
         **/ 
        private void ItemControl_Moved(object sender, MovedEventArgs e)
        {
            
            DisplaySettings.CurrentDisplaySettings.CenterX = e.X;
            DisplaySettings.CurrentDisplaySettings.CenterY = e.Y; 
        }
        private void ItemControl_ScaleChanged(object sender, EventArgs e)
        {
            DisplaySettings.CurrentDisplaySettings.Scale = this.ScaleFactor;
        }
        private void RotateTransform_Changed(object sender, EventArgs e)
        {
            DisplaySettings.CurrentDisplaySettings.Rotation = this.RotateAngle;
        }

        
        #endregion
        #region Events
        public event EventHandler<EditorStateEventArgs> EditorStateChanged;
        public event EventHandler<TextBoxStateEventArgs> NetworkDataAvailable;
        public event EventHandler FocusRequested;

        protected void OnFocusRequested()
        {
            if (FocusRequested != null)
                FocusRequested(this, new EventArgs());
        }

        #endregion
        #region Visibility Methods
        /// <summary>
        /// Zeigt die richtigen Inhalte für den gewählten State an
        /// </summary>
        public virtual void ShowContent()
        {
            if (State == STATE_VIEW)
                ShowContent(StdView);
            else
                ShowContent(EditorView);
        }

        /// <summary>
        /// Zeigt ein IContent-Objekt an
        /// </summary>
        /// <param name="content"></param>
        public void ShowContent(IContent content)
        {
            this.Invoke(() =>
                {
                    UserInterface.ShowContent(content);
                    UserInterface.UpdateData(Item);
                    SwitchSize(UserInterface.Width, UserInterface.Height);
                });
        }
        public void RequestFocus()
        {
            this.Invoke(() => {
                if (Surface.ViewController.CurrentView.FreeMovement)
                {
                    this.Scale(1, true);
                    this.MoveCenter(Surface.ActualWidth / 2, Surface.ActualHeight / 2, true);
                }
            });           
        }

        /// <summary>
        /// Überprüft, ob die Sichtbarkeit des ItemControl-Objektes angepasst werden muss
        /// (Siehe Epic-Ansicht -> Gruppen ausblenden)
        /// </summary>
        public virtual void CheckVisibility()
        {
            this.Invoke(() =>
                {
                    double opa = this.Item.IsVisible ? 1 : 0.5;
                    UserInterface.Opacity = opa;
                });
        }
        #endregion
        #region Editing Methods
        /// <summary>
        /// Leitet einen Zustand für eine Textbox an den Editor weiter
        /// </summary>
        /// <param name="e"></param>
        public void ReceiveLiveFeed(TextBoxStateEventArgs e)
        {
            if (EditorView != null)
                EditorView.ReceiveLiveData(e.State);
        }

        /// <summary>
        /// Leitet einen Zustand für eine Textbox an den Editor weiter
        /// </summary>
        /// <param name="e"></param>
        public void ReceiveLiveFeed(TextBoxState state)
        {
            if (EditorView != null)
                EditorView.ReceiveLiveData(state);
        }

        /// <summary>
        /// Startet bzw. beendet den Bearbeitungsmodus (readonly!)
        /// </summary>
        /// <param name="starting"></param>
        public void ChangeEditorState(bool starting)
        {
            if (EditorView != null)
            {
                if (starting)
                {
                    ActivateEditor(true);
                }
                else
                {
                    ActivateStdView();
                }
            }
        }

        /// <summary>
        /// Beginnt den Bearbeitungsmodus (nicht readonly!)
        /// </summary>
        public void StartEditing()
        {
            if (EditorView == null || State == STATE_EDITOR)
                return;
            this.Invoke(new Action(() =>
            {
               foreach (ItemControl rep in Item.Representations)
                {
                    if (rep != this)
                        rep.ChangeEditorState(true);

                } 
                if (EditorStateChanged != null)
                    EditorStateChanged(this, new EditorStateEventArgs(this.Item, true));

                EditorView.StateChanged += this.ReceiveData;
                ActivateEditor(false);
            }));
        }

        /// <summary>
        /// Beendet den Bearbeitungsmodus
        /// </summary>
        public void EndEditing()
        {
            if (State != STATE_EDITOR)
                return;
            foreach (ItemControl rep in Item.Representations)
            {
                if (rep != this)
                    rep.ChangeEditorState(false);
            } 
            if (EditorStateChanged != null)
                EditorStateChanged(this, new EditorStateEventArgs(this.Item, false));
            EditorView.StateChanged -= this.ReceiveData;
            EditorView.ReadItem(Item);
            ActivateStdView();
        }

        /// <summary>
        /// Leitet einen neuen Zustand einer Textbox an das Netzwerk weiter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveData(object sender, TextBoxStateEventArgs e)
        {
            if (State != STATE_EDITOR)
                return;
            e.Data = this.Item;
            foreach (ItemControl rep in Item.Representations)
            {
                if (rep != this)
                    rep.ReceiveLiveFeed(e);

            } 
            if (NetworkDataAvailable != null)
                NetworkDataAvailable(this, e);
        }
        #endregion
        #region Gestures
        public const string TOUCH_DOWN_GESTURE = "touchdown";
        public const string TOUCH_UP_GESTURE = "touchup";
        public const string EDITOR_TOGGLE_GESTURE = "editortoggle";

        public const string FOCUS_IN_CENTER_GESTURE = "focus";

        public override void InitializeGestures()
        {
            base.InitializeGestures();

            AddGesture(EDITOR_TOGGLE_GESTURE, DefinedGestures.Hold, EditorToggleCallback);
            
            AddGesture(TOUCH_DOWN_GESTURE, DefinedGestures.TouchDown, TouchDownCallback);
            AddGesture(TOUCH_UP_GESTURE, DefinedGestures.TouchUp, TouchUpCallback);
            this.AddGesture(FOCUS_IN_CENTER_GESTURE, DefinedGestures.DoubleTap, CenterFocusCallback);

        }
        private void CenterFocusCallback(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {
            this.Invoke(() =>
            {
                if (Surface.ViewController.CurrentView.FreeMovement)
                {
                    RequestFocus();
                    OnFocusRequested();
                }
            });
        }




        protected virtual void TouchUpCallback(UIElement element, TouchGroup points)
        {
            this.Invoke(RemoveBackgroundAnim);
        }
        protected virtual void TouchDownCallback(UIElement element, TouchGroup points)
        {
            this.Invoke(
                new Action(() =>
                {
                    _backgroundanim = true;
                    Animation.Animator.AnimateColor(
                            this.UserInterface,
                            Controls.Style.StyleHelper.GetBackgroundColor(Item),
                               Color.FromRgb(0, 0, 0),
                               new Duration(TimeSpan
                                   .FromMilliseconds(DefinedGestures.HOLD_LENGTH + 100)));
                })
                );
        }
        private bool _backgroundanim = false;
        protected void RemoveBackgroundAnim()
        {
            if (_backgroundanim)
            {
                _backgroundanim = false;
                UserInterface.UpdateData(Item, true);
            }
        }
        protected override void DragCallback(UIElement element, TouchGroup points)
        {
            this.Invoke(RemoveBackgroundAnim);
            base.DragCallback(element, points);
        }
        protected override void ResizeCallback(UIElement element, TouchGroup points)
        {
            this.Invoke(RemoveBackgroundAnim);
            base.ResizeCallback(element, points);
            DisplaySettings.CurrentDisplaySettings.Update(this);
        }
        protected override void RotateCallback(UIElement element, TouchGroup points)
        {
            this.Invoke(RemoveBackgroundAnim);
            base.RotateCallback(element, points);
            DisplaySettings.CurrentDisplaySettings.Update(this);
        }

        protected virtual void EditorToggleCallback(UIElement element, TouchGroup points)
        {
            if (State == STATE_VIEW)
            {
                StartEditing();
            }
            else
            {
                EndEditing();
            }
        }


        #endregion
        #region State Activation
        /// <summary>
        /// Setzt die Anicht auf die Standardansicht zurück
        /// </summary>
        public virtual void ActivateStdView()
        {
            State = STATE_VIEW;
            SetGestureActive(EDITOR_TOGGLE_GESTURE, true);
            this.Invoke(ShowContent);
        }

        /// <summary>
        /// Aktiviert den Editor
        /// </summary>
        /// <param name="remote">TRUE , wenn über das Netzwerk ankommend -> remote bedeutet auch readonly-Bearbeitungsmodus!</param>
        public virtual void ActivateEditor(bool remote)
        {
            State = STATE_EDITOR;
            this.Invoke(ShowContent);

            if (remote)
            {
                SetGestureActive(EDITOR_TOGGLE_GESTURE, false);
            }

            EditorView.SetReadOnly(remote);
        }
        #endregion
        #region AutoPos
        /// <summary>
        /// Platziert das ItemControl automatisch an einer Position auf der Surface
        /// </summary>
        public virtual void AutoPos()
        {
            Point rnd = MathHelper.GetRandomLocation(Surface.ActualWidth, Surface.ActualHeight);
            this.MoveCenter(rnd.X, rnd.Y);
            this.RotateAngle = MathHelper.Random.Next(365);
        }
        #endregion
    }
}
