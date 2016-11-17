using ScrumGestures.DragDrop;
using ScrumTouchkit.Controls.Content;
using ScrumTouchkit.Data;
using ScrumTouchkit.Threading;
using ScrumGestures;
using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Abstract;
using System.Windows;

namespace ScrumTouchkit.Controls
{
    /// <summary>
    /// Stellt eine Klasse dar, die Epics auf der Oberfläche darstellen kann
    /// Auf Epics können zudem User Stories "fallengelassen" werden, EpicControl 
    /// implementiert daher auch den IDropContainer
    /// </summary>
    public class EpicControl : Abstract.ItemControl, IDropContainer
    {

        #region vars, getter, setter

        /// <summary>
        /// Die Epic, die in dieser EpicControl angezeigt wird
        /// </summary>
        public Epic Epic
        {
            get { return Item as Epic; }
        }
        #endregion
        #region Constructor
        public EpicControl(Epic epic, ScrumSurface surface)
            : base(epic, surface)
        {
            UserInterface = new EpicUI_Base();
            StdView = new EpicUI_View();
            EditorView = new EpicUI_Editor();
            this.Epic.DataChanged += (s, e) => { this.Invoke(CheckVisibility); };
            this.Epic.ExternalDataChanged += (s, e) => { this.Invoke(CheckVisibility); };
            AfterInit();
            
        }
        #endregion
        #region DragDrop
        /// <summary>
        /// Eine User Story wurde über diese Epic gezogen -> auf der User 
        /// Story ein Plus oder Minus anzeigen (je nachdem, ob sie aktuell dieser
        /// Epic zugeordnet ist)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pt"></param>
        public void NotifyDragEnter(IDraggable obj, TouchPoint pt)
        {
             UserStoryControl usc = obj as UserStoryControl;
            if (usc != null)
                this.Invoke(() =>
                    {
                        if (usc.UserStory.Epic == this.Epic)
                            usc.MinusVisible = true;
                        else
                            usc.PlusVisible = true;
                    });
        }

        /// <summary>
        /// User Story wurde fallengelassen -> entweder dieser Epic zuordnen oder die 
        /// Zuordnung entfernen
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pt"></param>
        public void NotifyDragDrop(IDraggable obj, TouchPoint pt)
        {
            UserStoryControl usc = obj as UserStoryControl;
            if (usc != null)
            {
                this.Invoke(() =>
                {
                    if (usc.UserStory.Epic == this.Epic)
                    {
                        //Zuordnung entfernen
                        usc.MinusVisible = false;
                        usc.UserStory.Epic = null;
                        usc.MoveCenter(pt.StartPoint.X, pt.StartPoint.Y, true);
                    }
                    else
                    {
                        usc.UserStory.Epic = this.Epic;
                        usc.PlusVisible = false;
                    }
                    usc.CheckVisibility();
                });
            }
        }

        /// <summary>
        /// Die User Story wurde von der Epic herunter gezogen (nachdem sie vorher drauf gezogen wurde)
        /// --> Plus bzw. Minus entfernen
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pt"></param>
        public void NotifyDragExit(IDraggable obj, TouchPoint pt)
        {
            UserStoryControl usc = obj as UserStoryControl;
            if (usc != null)
                this.Invoke(() =>
                {
                    if (usc.UserStory.Epic == this.Epic)
                        usc.MinusVisible = false;
                    else
                        usc.PlusVisible = false;
                });
        }
        #endregion
        #region Gestures
        public const string TOGGLE_VISIBILITY_GESTURE = "togglevisibility";
        public const string GROUP_MOVE_GESTURE = "groupmove";

        public override void InitializeGestures()
        {
            base.InitializeGestures();
            this.AddGesture(TOGGLE_VISIBILITY_GESTURE, DefinedGestures.Tap, ToggleVisibility);
           
        }

        /// <summary>
        /// Das Bewegen bei einer Epic ist anders:
        /// Es wird nicht bloß die Epic bewegt, sondern alle an dieser Epic hängenden User Stories ebenfalls
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        protected override void DragCallback(UIElement element, TouchGroup points)
        {
            RemoveMovingAnimation();
            this.Invoke(RemoveBackgroundAnim);
            this.MoveGroupBy(points[0].PositionChange.X, points[0].PositionChange.Y, byUser: true);
        }

        /// <summary>
        /// Einmaliges antippen der Epic verändert die Visibility
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        private void ToggleVisibility(System.Windows.UIElement element, TouchGroup points)
        {
            bool visibility = !Epic.IsVisible;
            Epic.IsVisible = visibility;

            foreach (UserStory us in Epic.UserStories)
                us.IsVisible = visibility;

            CheckVisibility();
        }
        #endregion
        #region Visibility
        /// <summary>
        /// Überprüft, ob die Epic sichtbar ist und passt die 
        /// Darstellung dementsprechend an
        /// </summary>
        public override void CheckVisibility()
        {
            base.CheckVisibility();
            foreach (UserStory us in Epic.UserStories)
            {
                foreach (ItemControl ic in us.Representations)
                {
                    ic.CheckVisibility();
                }
            }
        }
        #endregion
        #region Group Actions
        /// <summary>
        /// Bewegt die ganze Gruppe (Epics + deren User Stories) um die angegebene Anzahl von Pixeln
        /// </summary>
        /// <param name="x">Die Pixel, um welche die Gruppe verschoben wird , auf der X-Achse</param>
        /// <param name="y">Die Pixel, um welche die Gruppe verschoben wird , auf der Y-Achse</param>
        /// <param name="animate">Wenn TRUE, wird der Schiebe-Vorgang animiert</param>
        /// <param name="byUser">Wenn TRUE, wurde die Aktion aktiv von einem Nutzer an diesem Computer ausgeführt</param>
        public void MoveGroupBy(double x, double y, bool animate = false, bool byUser = false)
        {
           foreach (UserStory u in Epic.UserStories)
            {
                foreach (ItemControl ui in u.Representations)
                {
                    ui.RemoveMovingAnimation();
                    ui.MoveBy(x, y, animate, byUser);
                }
            }
            foreach (ItemControl ui in Item.Representations)
            {
                ui.MoveBy(x, y, animate, byUser);
            }
        }

        /// <summary>
        /// Bewegt die ganze Gruppe (Epic + deren User Stories) an die angegebene Position. 
        /// Dabei bleiben die relativen Positonen (Abstand der User Stories zur Epic) bestehen
        /// und die Epic befindet sich schlussendlich an den angegebenen Koordinaten
        /// </summary>
        /// <param name="x">Die neue X-Koordinate für die Epic</param>
        /// <param name="y">Die neue Y-Koordinate für die Epic</param>
        /// <param name="animate">Wenn TRUE, wird der Vorgang animiert</param>
        /// <param name="byUser">Wenn TRUE, wurde die Aktion aktiv von einem Nutzer an diesem Computer ausgeführt</param>
        public void MoveGroupTo(double x, double y, bool animate = false, bool byUser = false)
        {
            double diff_x = x - this.CenterX;
            double diff_y = y - this.CenterY;
            MoveGroupBy(diff_x, diff_y, animate, byUser);
        }
        #endregion

    }
}
