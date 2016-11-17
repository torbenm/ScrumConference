using ScrumGestures.DragDrop;
using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Content;
using ScrumTouchkit.Controls.Content.Lines;
using ScrumTouchkit.Data;
using ScrumTouchkit.Threading;
using System;
using System.Linq;
using System.Windows.Shapes;
using System.Windows;
using ScrumTouchkit.Utilities;

namespace ScrumTouchkit.Controls
{
    /// <summary>
    /// Stellt eine UserStory auf der Oberfläche dar
    /// </summary>
    public class UserStoryControl : Abstract.ItemControl, IDraggable
    {
        
        #region vars, get, set
        protected DragDropController _ddc;

        /// <summary>
        /// Verbindungslinie zur Epic (Falls vorhanden)
        /// </summary>
        public ConnectionLine Line
        {
            get;
            private set;
        }

        /// <summary>
        /// Standardbreite von User Stories
        /// </summary>
        public static int StdWidth
        {
            get { return 475; }
        }

        /// <summary>
        /// Standardhöhe von User Stories
        /// </summary>
        public static int StdHeight
        {
            get { return 300; }
        }

        /// <summary>
        /// DIe zugehörige User Story
        /// </summary>
        public UserStory UserStory
        {
            get { return Item as UserStory; }
        }

        /// <summary>
        /// Gibt an, ob das Kreuz in der Mitte der User Story sichtbar ist
        /// </summary>
        public bool CrossVisible
        {
            get { return ((UserInterface as UserStoryUI_Base).CrossVisible); }
            set { (UserInterface as UserStoryUI_Base).CrossVisible = value; }
        }
        /// <summary>
        /// Gibt an, ob das Pluszeichen in der Mitte der User Story sichtbar ist
        /// </summary>
        public bool PlusVisible
        {
            get { return ((UserInterface as UserStoryUI_Base).PlusVisible); }
            set { (UserInterface as UserStoryUI_Base).PlusVisible = value; }
        }
        /// <summary>
        /// Gibt an, ob das Minuszeichen in der Mitte der User Story sichtbar ist
        /// </summary>
        public bool MinusVisible
        {
            get { return ((UserInterface as UserStoryUI_Base).MinusVisible); }
            set { (UserInterface as UserStoryUI_Base).MinusVisible = value; }
        }

        /// <summary>
        /// Gibt an, ob das große rote Kreuz über der User Story sichtbar ist
        /// </summary>
        public bool DeleteVisible
        {
            get { return ((UserInterface as UserStoryUI_Base).DeleteVisible); }
            set { (UserInterface as UserStoryUI_Base).DeleteVisible = value; }
        }
        #endregion
        #region Constructor
        public UserStoryControl(UserStory story, ScrumSurface surface)
            : base(story, surface)
        {
            _ddc = new DragDropController(surface, this, DragDropController.MODE_OBJCENTER);
            UserInterface = new UserStoryUI_Base();
            StdView = new UserStoryUI_View();
            EditorView = new UserStoryUI_Editor();
            this.DisplaySettings.ViewChanged += DisplaySettings_ViewChanged;
            UserStory.DataChanged += UserStory_DataChanged;
            CheckLine();
            AfterInit();
            UpdateEffortSum();
            Surface.Database.EffortSum.EffortPointsChanged += this.UpdateEffortSum;
        }


        void UserStory_DataChanged(object sender, EventArgs e)
        {
            CheckLine();
        }

        void DisplaySettings_ViewChanged(object sender, EventArgs e)
        {
            CheckLine();
        }
        
        protected override void ExternalDataChanged(object sender, EventArgs e)
        {
            base.ExternalDataChanged(sender, e);
            CheckLine();
        }

        /// <summary>
        /// Entfernt die UserStory von der Oberfläche (und aus der Datenbank)
        /// </summary>
        public override void RemoveFromSurface()
        {
            base.RemoveFromSurface();
            if (Line != null)
                Line.RemoveLine();
        }
        #endregion
        #region Gestures
        public const string TOGGLE_BACKLOG_GESTURE = "togglebacklog";

        /// <summary>
        /// Aktiviert alle Gesten
        /// Zusätzlich zu den in ItemControl definierten Gesten kommt an dieser Stelle eine hinzu
        /// </summary>
        public override void InitializeGestures()
        {
            base.InitializeGestures();
            this.AddGesture(TOGGLE_BACKLOG_GESTURE, DefinedGestures.Tap, ToggleBacklog);
            this.SetGestureActive(TOGGLE_BACKLOG_GESTURE, false);
        }

      
        protected override void DragCallback(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {
            base.DragCallback(element, points);
            // Drag and Drop Test!
            _ddc.TestDrop(points[0]);
        }
        protected override void TouchUpCallback(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {
            base.TouchUpCallback(element, points);
            // Drag and Drop Ausführen
            _ddc.DoDrop(points[0]);
        }

        /// <summary>
        /// Wechselt die Backlog Zugehörigkeit
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        private void ToggleBacklog(System.Windows.UIElement element, ScrumGestures.TouchGroup points)
        {

            switch(UserStory.BacklogStatus)
            {
                case ItemBacklogStatus.PRODUCT_BACKLOG:
                    UserStory.BacklogStatus = ItemBacklogStatus.SPRINT_BACKLOG;
                    break;
                case ItemBacklogStatus.SPRINT_BACKLOG:
                    UserStory.BacklogStatus = ItemBacklogStatus.PRODUCT_BACKLOG;
                    break;
            }
            UserInterface.UpdateData(Item);
        }
        #endregion
        #region Line

        /// <summary>
        /// Erstellt eine Linie zu der angegebenen EpicControl
        /// </summary>
        /// <param name="ec"></param>
        private void CreateLine(EpicControl ec)
        {
            ec.IsHiddenChanged += ec_IsHiddenChanged;
            Line = new ConnectionLine(this, ec);
        }
  
        /// <summary>
        /// Entfernt die Linie
        /// </summary>
        private void RemoveLine()
        {
            Line.ControlB.IsHiddenChanged -= ec_IsHiddenChanged;
            Line.Dispose();
            Line = null;
        }

        /// <summary>
        /// Überprüft ob eine Linie erstellt oder gelöscht werden muss.
        /// ----
        /// User Story hat Epic & Linie existiert => nichts tun
        /// User Story hat Epic & Linie existiert nicht => Linie erstellen
        /// ----
        /// User Story hat keine Epic & Linie existiert => Linie löschen
        /// User Story hat Epic & Linie existiert nicht => nichts tun
        /// </summary>
        public virtual void CheckLine()
        {
            if (UserStory.Epic != null)
            {
                EpicControl ec = null;
                if (UserStory.Epic.Representations.Count > 0)
                    ec = UserStory.Epic.Representations[0] as EpicControl;

                if (Line == null)
                {
                    if (ec != null && !ec.IsHidden)
                    {
                        CreateLine(ec);
                    }
                }
                else
                {
                    if (ec == null || ec.IsHidden)
                    {
                        RemoveLine();
                    }
                }
            }
            else
            {
                if (Line != null)
                {
                    RemoveLine();
                }
            }
        }

       /// <summary>
       /// Entfernt oder löscht die Linie, je nachdem, ob die zu dieser User Story gehörende Epic
       /// Sichtbar ist.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       private  void ec_IsHiddenChanged(object sender, EventArgs e)
        {
            EpicControl ec = ((sender) as EpicControl);
            if (ec != null)
            {
                if (ec.IsHidden && Line != null)
                {
                    RemoveLine();
                }
                else
                {
                    CreateLine(ec);
                }
            }
        }
        
        #endregion
        #region AutoPos
       private void AutoPos(object o, RoutedEventArgs e)
       {
           AutoPos();
       }
        /// <summary>
        /// Platziert die User Story automatisch in die nähere Umgebung der EpicControl (falls der User Story eine Epic zugewiesen wurde)
        /// </summary>
       public override void AutoPos()
       {
           if (this.UserStory.Epic == null)
               base.AutoPos();
           else
           {
               if (this.UserStory.Epic.Representations.Count > 0)
               {
                   EpicControl epic = (EpicControl)this.UserStory.Epic.Representations.First();
                   if (epic.IsLoaded)
                   {
                       epic.Loaded -= AutoPos;

                       this.Scale(UserStoryControl.StaticDefaultSettings.Scale, true);
                       double w = UserStoryControl.StaticDefaultSettings.Scale * UserStoryControl.StdWidth;
                       double h = UserStoryControl.StaticDefaultSettings.Scale * UserStoryControl.StdHeight;
                       double r = Math.Sqrt(w * w + h * h)/2;

                       Point pt = MathHelper.GetPointOnCircle(epic.Center, epic.ScaledWidth/2);
                       Vector vec = new Vector(pt.X - epic.CenterX, pt.Y - epic.CenterY);
                       pt = vec.MovePoint(pt, 3 * r / 2);
                       this.MoveCenter(pt.X, pt.Y, true);
                   }
                   else
                   {
                       epic.Loaded += AutoPos;
                   }
               }
               else
                   base.AutoPos();
           }
       }
        #endregion
        #region EffortSum
       private void UpdateEffortSum(object o, EventArgs e)
       {
           UpdateEffortSum();
       }
        /// <summary>
        /// Aktualisiert die Anzeige des Aufwands
        /// </summary>
       public void UpdateEffortSum()
       {
           this.Invoke(() =>
               {
                   ((StdView) as UserStoryUI_View).EffortSum.Text = "\u03A3 " + Surface.Database.EffortSum;
               });
       }
        #endregion
    }
}
