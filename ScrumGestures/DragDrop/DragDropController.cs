using ScrumGestures.Helper;
using System.Windows;
using System.Windows.Controls;

namespace ScrumGestures.DragDrop
{
    
    /// <summary>
    /// Mithilfe des DragDropControllers kann überprüft werden,
    /// ob sich an einer berstimmten Position auf dem Canvas ein IDragContainer befindet.
    /// Wenn dies der Fall ist, werden entsprechende Funktionen bei diesem aufgerufen.
    /// </summary>
    public class DragDropController
    {

        /// <summary>
        /// Im TOUCHPOINT-Modus wird der Berührungspunkt als Position zum IDragContainer-Check verwendet.
        /// </summary>
        public const int MODE_TOUCHPOINT = 0;
        /// <summary>
        /// Im OBJCENTER-Modus wird das Zentrum eines Objektes als Position für den IDragContainer-Check verwendet.
        /// </summary>
        public const int MODE_OBJCENTER = 1;

        private IDraggable _draggedObject;
        private IDropContainer _container;
        private Canvas _canvas;
        private int _mode;

        /// <summary>
        /// Initialisiert einen neuen DragDropController.
        /// </summary>
        /// <param name="canvas">Die Oberfläche, auf der sich alle Objekte befinden</param>
        /// <param name="obj">Das Objekt auf der Oberfläche, über welches die Checks initialisiert werden sollen (z.B. welches in einen Mülleimer gezogen werden können soll)</param>
        /// <param name="mode">Der Modus: MODE_TOUCHPOINT oder MODE_OBJCENTER</param>
        public DragDropController(Canvas canvas, IDraggable obj, int mode)
        {
            _draggedObject = obj;
            _canvas = canvas;
            _mode = mode;
        }
        /// <summary>
        /// Initialisiert einen neuen DragDropController im TOUCHPOINT-Modus
        /// </summary>
        /// <param name="canvas">Die Oberfläche, auf der sich alle Objekte befinden</param>
        /// <param name="obj">Das Objekt auf der Oberfläche, über welches die Checks initialisiert werden sollen (z.B. welches in einen Mülleimer gezogen werden können soll)</param>
        public DragDropController(Canvas canvas, IDraggable obj)
            : this(canvas, obj, MODE_TOUCHPOINT)
        { }

        /// <summary>
        /// Wird aufgerufen, wenn überprüft werden soll, ob sich unter der Mitte des Zentrums oder dem angegeben TouchPoint ein IDragContainer befindet. 
        /// Dies führt noch keine endgültigen Operationen aus, aber teilt dem IDragContainer mit, dass sich nun ein Objekt über diesem befindet.
        /// </summary>
        /// <param name="pt">Hier wird der aktuelle Berührungspunkt übergeben, dies sollte auch bei Modus OBJCENTER passieren.</param>
        /// <returns>Den obersten gefundenen IDropContainer oder NULL</returns>
        public IDropContainer TestDrop(TouchPoint pt)
        {
            IDropContainer dc = ContainerAt(pt);
            if (dc != null)
            {
                if (dc != _container)
                {
                    //Falls wir vorher ein anderen IDragContainer zwischengespeichert hatten, wird
                    //diesem nun mitgeteilt, das wir ihn verlassen haben
                    if (_container != null)
                        _container.NotifyDragExit(_draggedObject, pt);
                    //Benachrichtige neuen Container das wir uns nun über diesem befinden
                    dc.NotifyDragEnter(_draggedObject, pt);
                    //Zwischenspeichern des gefundenen Containers
                    _container = dc;
                }
            }
            else
            {
                if (_container != null)
                {
                    _container.NotifyDragExit(_draggedObject, pt);
                    _container = null;
                }
            }
            return dc;
        }

        /// <summary>
        /// Führt die mit dem Drag-Drop tatsächlich verbundene Aktion durch.
        /// Bei einer Mülleimerfunktion würde zum Beispiel das Element gelöscht.
        /// </summary>
        /// <param name="pt">Hier wird der aktuelle Berührungspunkt übergeben, dies sollte auch bei Modus OBJCENTER passieren.</param>
        /// <returns>Den obersten gefundenen IDropContainer oder NULL</returns>
        public IDropContainer DoDrop(TouchPoint pt)
        {
            IDropContainer dc = ContainerAt(pt);
            if (dc != null)
                dc.NotifyDragDrop(_draggedObject, pt);
            return dc;
        }

        /// <summary>
        /// Such nach einem Container am entsprechendem TouchPoint oder der Mitte des verbundenen Objektes.
        /// </summary>
        /// <param name="pt">Hier wird der aktuelle Berührungspunkt übergeben, dies sollte auch bei Modus OBJCENTER passieren.</param>
        /// <returns>Den obersten gefundenen IDropContainer oder NULL</returns>
        private IDropContainer ContainerAt(TouchPoint pt)
        {
            //Je nach Modus anderen Punkt wählen
            if (_mode == MODE_TOUCHPOINT)
            {
                return (IDropContainer)(new GenericHitTestExecution<IDropContainer>()
                    .ExecuteHitTest(_canvas, pt.CurrentPoint));
            }
            else
            {
                return (IDropContainer)(new GenericHitTestExecution<IDropContainer>()
                    .ExecuteHitTest(_canvas, new Point(_draggedObject.CenterX, _draggedObject.CenterY)));
            }
        }
    }
}
