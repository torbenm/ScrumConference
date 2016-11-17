using ScrumTouchkit.Data;

namespace ScrumTouchkit.Controls.Content.Abstract
{
    /// <summary>
    /// Das IContent-Interface wird von Klassen geerbt, die in ItemControls als Inhalt angezeigt werden können.
    /// </summary>
    public interface IContent
    {
        void UpdateData(ItemBase data);
    }
}
