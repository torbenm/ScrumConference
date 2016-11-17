using ScrumTouchkit.Utilities;
using System;
using ScrumTouchkit.Threading;
using ScrumTouchkit.Data;
using ScrumGestures.DragDrop;

namespace ScrumTouchkit.Controls.Buttons
{
    /// <summary>
    /// Da der Delete-Button zusätzlich zu den Funktionalitäten eines Buttons
    /// vor allem in IDropCOntainer ist, erhält er eine eigene Klasse, in welcher
    /// er das notwendige Interface implementiert.
    /// </summary>
    public class DeleteButton : UI.Button, ScrumGestures.DragDrop.IDropContainer
    {
        public DeleteButton(ScrumSurface surface) : base(surface, ButtonType.Image)
        {
            this.SetValue(Images.delete96);
        }

        #region Drag & Drop
        public void NotifyDragEnter(IDraggable obj, ScrumGestures.TouchPoint pt)
        {
            if (obj.GetType() == typeof(UserStoryControl))
            {
                UserStoryControl usc = obj as UserStoryControl;
                this.Invoke(() =>
                {
                    //Zeigen: Wenn du jetzt das Element loslässt, wird es gelöscht
                    usc.DeleteVisible = true;
                });
            }
        }

        public void NotifyDragExit(IDraggable obj, ScrumGestures.TouchPoint pt)
        {
            if (obj.GetType() == typeof(UserStoryControl))
            {
                UserStoryControl usc = obj as UserStoryControl;
                this.Invoke(() =>
                    {

                        //Gefahr gebannt 
                        usc.DeleteVisible = false;
                    });
            }
        }

        public void NotifyDragDrop(IDraggable obj, ScrumGestures.TouchPoint pt)
        {
            if (obj.GetType() == typeof(UserStoryControl))
            {
                UserStoryControl usc = obj as UserStoryControl;
                this.Invoke(() =>
                {
                    // Element wurde doch losgelassen! Also löschen
                    Surface.Database.RemoveItem(usc.Item);
                    if (ItemRemoved != null)
                        ItemRemoved(this, usc.Item);
                });
            }
        }

        public event EventHandler<ItemBase> ItemRemoved;
        #endregion
    }
}
