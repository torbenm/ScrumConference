using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScrumTouchkit.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumTouchkit.Utilities.Serializer
{
    /// <summary>
    /// Bietet Funktionalitäten, um Objekte schnell und einfach
    /// in JSON-Strings umzuwandeln.
    /// Bentutzt dafür Newtonsoft.Json
    /// </summary>
    /// <see cref="Newtonsoft.Json"/>
    public class JSONSerializer : ISerializer
    {
        private Encoding encoding = new UnicodeEncoding();
        /// <summary>
        /// Wandelt ein beliebiges Objekt in ein Byte-Array um
        /// </summary>
        /// <param name="obj">Das Objekt, welches umgewandelt werden soll</param>
        /// <returns>Das Byte-Array. In diesem steht der JSON-String (als Unicode-Encodiert)</returns>
        public byte[] ObjectToByteArray(object obj)
        {
            return encoding.GetBytes(ObjectToString(obj));
        }
        public string ObjectToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// Wandelt das Byte-Array in ein Objekt von Typ T um
        /// </summary>
        /// <typeparam name="T">Der Typ des Objektes, in welches der JSON-Code umgewandelt werden soll</typeparam>
        /// <param name="data">Das Byte-Array, welches JSON in Unicode Encoding beinhaltet</param>
        /// <returns></returns>
        public T ConvertToObject<T>(byte[] data)
        {
           
            String json = encoding.GetString(data);
            return ConvertToObject<T>(json);
        }

        /// <summary>
        /// Wandelt einen JSON-String in ein Objekt von Typ T um
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T ConvertToObject<T>(string data)
        {
            if (typeof(ItemBase).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)ConvertToItem(data);
            }
            else if (typeof(T) == typeof(List<ItemBase>))
            {
                return (T)(object)ConvertToItemList(data);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        /// <summary>
        /// Wandelt ein Byte-Array in ein ItemBase Objekt (Epic ODER User Story) um
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ItemBase ConvertToItem(byte[] data)
        {
            String json = encoding.GetString(data);
            return ConvertToItem(json);
        }
        /// <summary>
        /// Wandelt einen JSON-String in ein ItemBase Objekt (Epic ODER User Story) um
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ItemBase ConvertToItem(string data)
        {
            JObject obj = (JObject)JsonConvert.DeserializeObject(data);
            return ConvertToItem(obj);
        }

        /// <summary>
        /// Wandelt ein Byte-Array in eine Liste von Items (Epics und/oder User Stories) um
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<ItemBase> ConvertToItemList(byte[] data)
        {
            return ConvertToItemList(encoding.GetString(data));
        }

        /// <summary>
        /// Wandelt ein JSON-String in eine Liste von Items (Epics und/oder User Stories) um
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<ItemBase> ConvertToItemList(string data)
        {
            List<ItemBase> items = new List<ItemBase>();
            object obj = JsonConvert.DeserializeObject(data);
            if (obj.GetType() == typeof(JArray))
            {
                JArray arr = (JArray)obj;
                for (int i = 0; i < arr.Count; i++)
                {
                    items.Add(ConvertToItem(arr[i]));
                }
            }
            return items;
        }

        /// <summary>
        /// Wandelt ein JToken-Objekt in ein ItemBase Objekt um.
        /// Diese Funktion ist notwendig, um zwischen User Stories und Epics
        /// bei der Umwandlung zu unterscheiden (type casting!)
        /// </summary>
        /// <param name="obj">Das JToken Objekt</param>
        /// <returns></returns>
        private ItemBase ConvertToItem(JToken obj)
        {
            try
            {
                if (obj["Type"].ToObject<ItemBase.SubType>() == ItemBase.SubType.USER_STORY)
                {
                    return (ItemBase)obj.ToObject<UserStory>();
                }
                else
                {
                    return (ItemBase)obj.ToObject<Epic>();
                }
            }
            catch
            {
                return null;
            }
        }

    }
}
