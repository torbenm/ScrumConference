using ScrumTouchkit.Data;
using System.Collections.Generic;

namespace ScrumTouchkit.Utilities.Serializer
{
    /// <summary>
    /// Stellt ein Interface für die Serialisierer (XML und JSON) dar
    /// Genauere Beschreibungen der Methoden in den entsprechenden Klassen
    /// </summary>
    public interface ISerializer
    {
        byte[] ObjectToByteArray(object obj);
        string ObjectToString(object obj);
        T ConvertToObject<T>(byte[] data);
        T ConvertToObject<T>(string data);

        ItemBase ConvertToItem(byte[] data);
        ItemBase ConvertToItem(string data);
        List<ItemBase> ConvertToItemList(byte[] data);
        List<ItemBase> ConvertToItemList(string data);
        
        
    }
}
