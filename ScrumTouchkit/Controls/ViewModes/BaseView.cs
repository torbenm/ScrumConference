using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Controls.ViewModes.SettingsLoader;
using ScrumTouchkit.Data;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ScrumTouchkit.Threading;

namespace ScrumTouchkit.Controls.ViewModes
{

    /// <summary>
    /// Stellt grundlegende Funktionen für verschiedene Ansichten zur Verfügung
    /// </summary>
    public class BaseView
    {
        #region vars, getter, setter
        protected SettingsLoader.SettingsLoader settingsLoader;

        /// <summary>
        /// Speichert, welche Typen von Items wie viele Darstellungen haben dürfen
        /// Standard:   Epic        0
        ///             User Story  1
        /// </summary>
        protected Dictionary<Type, int> allowControls = new Dictionary<Type, int>();
        protected List<UIElement> _elements;

        /// <summary>
        /// Gibt an, ob unsichtbare Elemente in dieser Ansicht angezeigt werden
        /// </summary>
        protected bool ShowInvisible = false;
        protected bool freeMovement = true;

        /// <summary>
        /// Wenn hier TRUE angegeben ist,
        /// könnnen alle Elemente frei bewegt werden (und diese Bewegung hat keinerlei Auswirkungen!)
        /// Bei FALSE ist dies der Gegenteil
        /// (Beispiel für FALSE: Priority und Effort View)
        /// </summary>
        public bool FreeMovement
        {
            get
            {
                return freeMovement;
            }
        }

        /// <summary>
        /// Gibt an, ob diese Ansicht die aktuelle ist
        /// </summary>
        public bool IsActive
        {
            get;
            protected set;
        }

        /// <summary>
        /// ID zur Identifikation der Ansicht
        /// </summary>
        public int ViewID
        {
            get;
            protected set;
        }

        /// <summary>
        /// Die Oberfläche, auf welcher diese Ansicht angezeigt wird
        /// </summary>
        public ScrumSurface Surface
        {
            get;
            protected set;
        }

        /// <summary>
        /// Alle Item Controls, die angezeigt werden (in dieser Ansicht)
        /// z.B. enthält diese (außer bei dem Epic View) NICHT die EpicControls!
        /// </summary>
        public List<ItemControl> VisibleControls
        {
            get;
            private set;
        }

        /// <summary>
        /// Name zum Anzeigen
        /// </summary>
        public virtual string Name
        {
            get
            {
                return "std";
            }
        }
        #endregion
        #region Constructor
        public BaseView(ScrumSurface surface, int viewId)
        {
            this.allowControls.Add(typeof(UserStory), 1);
            this.allowControls.Add(typeof(Epic), 0);
            this.Surface = surface;
            this.ViewID = viewId;
            this.settingsLoader = new StandardSettingsLoader(this);
            _elements = new List<UIElement>();
            this.VisibleControls = new List<ItemControl>();
            
        }

       
        #endregion
        #region Activate / Deactivate / Refresh
        /// <summary>
        /// Aktiviert diese Ansicht
        /// </summary>
        public void Activate()
        {
            VisibleControls.Clear();
            InternalActivate();
            IsActive = true;
        }
        /// <summary>
        /// Muss in den tatsächlichen Ansichten implementiert werden (aber optional)
        /// Initialisiert bspw. einen Hintergrund, etc.
        /// </summary>
        protected virtual void InternalActivate()
        {
            
        }

        /// <summary>
        /// Deaktiviert eine Ansicht. Anschließend kann eine andere Ansicht aktiviert werden
        /// </summary>
        public void Deactivate()
        {
            InternalDeactivate();
            IsActive = false;
        }

        /// <summary>
        /// Deaktiviert die Darstellung der Ansicht.
        /// Entfernt automatisch alle Hinzugefügten Hintergrundelemente
        /// und ItemControls.
        /// </summary>
        protected virtual void InternalDeactivate()
        {
            SafeRemove();
            foreach (ItemControl c in this.VisibleControls)
            {
                DisableControl(c);
            }
        }


        #endregion
        #region Items
        /// <summary>
        /// Lädt ein Item (und damit dessen ItemControls) in diese Ansicht.
        /// Dadurch können auch Items gefiltert werden
        /// </summary>
        /// <param name="item"></param>
        public virtual void LoadItem(Data.ItemBase item)
        {
            int controlsLeft = allowControls[item.GetType()];
            if (controlsLeft > 0)
                HaveOneRepresentation(item);
            foreach (ItemControl ic in item.Representations)
            {
                if (controlsLeft > 0 && (item.IsVisible || this.ShowInvisible))
                {
                    ShowControl(ic);
                    controlsLeft--;
                }
                else
                    HideControl(ic);
                
            }
        }
        public virtual void UnloadItem(Data.ItemBase item)
        { }

        /// <summary>
        /// Zeigt eine ItemControl in dieser Ansicht an und lädt dafür die DisplaySettings.
        /// </summary>
        /// <param name="control"></param>
        protected virtual void ShowControl(ItemControl control)
        {
            VisibleControls.Add(control);
            settingsLoader.LoadSettings(control.DisplaySettings);
            if (control.IsHidden)
                control.Show();
        }

        protected virtual void DisableControl(ItemControl control)
        {
        }

        /// <summary>
        /// Entfernt ein ItemControl aus der Ansicht
        /// </summary>
        /// <param name="control"></param>
        protected virtual void HideControl(ItemControl control)
        {
            if (!control.IsHidden)
                control.Hide();
        }

        /// <summary>
        /// Wird aufgerufen, nachdem die DisplaySettings für ein Element geladen wurden
        /// (in DisplaySettingsCollection)
        /// </summary>
        /// <see cref="DisplaySettingsCollection"/>
        /// <param name="control"></param>
        public virtual void AfterSettingsLoaded(ItemControl control)
        {

        }

        /// <summary>
        /// Stellt sicher, dass ein Item mindestens eine Darstellung hat.
        /// Wenn dies nicht der Fall ist, wird eine erstellt.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void HaveOneRepresentation(ItemBase item)
        {
            if (item.Representations.Count == 0)
                item.CreateRepresentation(this.Surface);
        }

        #endregion
        #region Validate
        /// <summary>
        /// Validiert, ob ein Punkt für ein SurfaceObject sich an einer legalen Stelle befindet.
        /// So darf, standardmäßig, der MittelPunkt eines SurfaceObjects (welches alle Elemente auf der Oberfläche, 
        /// die mit Gesten manipuliert werden können, beschreibt) sich nicht außerhalb
        /// der Sichtbaren Fläche befinden.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Gibt einen validen Punkt zurück. Falls die übergebenen Koordinaten nicht valide sind,
        /// werden diese so angepasst, dass sie erlaubt sind.</returns>
        public virtual Point ValidatePosition(SurfaceObject obj, double x, double y)
        {
            return Surface.Invoke<Point>(new Func<Point>(() =>
            {
                x = x < 0 ? 0 : x;
                x = x > Surface.ActualWidth ? Surface.ActualWidth : x;

                y = y < 0 ? 0 : y;
                y = y > Surface.ActualHeight ? Surface.ActualHeight : y;
                return new Point(x, y);
            }));
        }
        #endregion
        #region Elements for Surface
        /// <summary>
        /// Fügt ein Hintergrundelement zur Oberfläche hinzu
        /// </summary>
        /// <param name="ui"></param>
        protected void AddElementToSurface(UIElement ui)
        {
            AddElementToSurface(ui, -5);
        }
        /// <summary>
        /// Fügt ein Hintergrundelement zur Oberfläche hinzu und vergibt ihr die angegebene Z-Positon (Z-Index)
        /// Dadurch wird angegeben, wie weit vorne (+) oder hinten (-) sich das Element befindet.
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="zIndex"></param>
        protected void AddElementToSurface(UIElement ui, int zIndex)
        {
            Canvas.SetZIndex(ui, zIndex);
            _elements.Add(ui);
            Surface.Children.Add(ui);
        }

        /// <summary>
        /// Entfernt ein Hintergrundelement von der Oberfläche
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElement(UIElement element)
        {
            Surface.Children.Remove(element);
            _elements.Remove(element);
        }

        /// <summary>
        /// Wird bei Deactivate() aufgerufen
        /// Entfernt alle Hintergrundelemente
        /// </summary>
        public void SafeRemove()
        {
            foreach (UIElement ui in _elements)
            {
                Surface.Children.Remove(ui);
            }
            _elements.Clear();
        }
        #endregion
  
    }
}
