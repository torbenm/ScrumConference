using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using ScrumTouchkit.Data;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Controls.DisplaySettings;
using ScrumTouchkit.Controls;
using System.Globalization;

namespace ScrumTouchkit.Utilities.Serializer
{
    /// <summary>
    /// In dieser Klasse werden Methoden bereitgestellt, um
    /// Objekte in XML umzuwandeln.
    /// Im Moment werden dabei nur Epics, UserStories und Listen
    /// von diesen unterstützt.
    /// </summary>
    public class XMLSerializer :  IFileManager
    {
        //Das gewählte Encoding
        private Encoding encoding = new UnicodeEncoding();

        #region Encode 
        /// <summary>
        /// Verwandelt ein Objekt in ein Byte Array [nicht implementiert]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete("Diese Funktion ist nicht korrekt implementiert")]
        public byte[] ObjectToByteArray(object obj)
        {
            return encoding.GetBytes(ObjectToString(obj));
        }

        /// <summary>
        /// Serialisiert ein Objekt
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete("Diese Funktion ist nicht korrekt implementiert")]
        public string ObjectToString(object obj)
        {
           /* XElement xml = WrapObjectWithXML(obj);
            StringBuilder str = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(str))
            {
                xml.WriteTo(writer);
                writer.Flush();
            }
            return str.ToString(); */
            return "";
        }

        /// <summary>
        /// Erstellt ein vollständiges XML-Dokument
        /// für eine Liste von Epics und User Stories 
        /// (Kann somit für die Speicherung einer ScrumDatabase verwendet werden)
        /// </summary>
        /// <param name="epics">Die Liste von Epics</param>
        /// <param name="userstories">Die Liste von User Stories</param>
        /// <returns>Das XML-Dokument als ROOT-Element</returns>
        private XElement WrapObjectWithXML(List<Epic> epics, List<UserStory> userstories)
        {
            XElement root = new XElement("ScrumConference");
            XElement stories = ObjectToXElement(userstories);
            XElement children = ObjectToXElement(epics);
            if (children == null)
            {
                //Keine Epics -> Fehler
                root.Add("ERROR-Epics");
            }
            else
                root.Add(children);

            if (stories == null)
            {
                //Keine User Stories -> Fehler
                root.Add("ERROR-UserStories");
            }
            else
                root.Add(stories);
            return root;
        }

        /// <summary>
        /// Schreibt eine User Story, Epic oder eine Liste der beiden (auch gemischt)
        /// in ein XML-Element
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private XElement ObjectToXElement(object obj)
        {
            if (obj.GetType() == typeof(UserStory))
            {
                return EncodeUserStory((UserStory)obj);
            }
            else if (obj.GetType() == typeof(Epic))
            {
                return EncodeEpic((Epic)obj);
            }
            else if (obj.GetType().IsAssignableFrom(typeof(List<ItemBase>)))
            {
               return EncodeItemList((List<ItemBase>)obj);
            }
            else if(obj.GetType().IsAssignableFrom(typeof(List<UserStory>)))
            {
                return EncodeItemList((List<UserStory>)obj);
            }
            else if (obj.GetType().IsAssignableFrom(typeof(List<Epic>)))
            {
                return EncodeItemList((List<Epic>)obj);
            }
            return null;
        }

        /// <summary>
        /// Schreibt eine Liste von Items (Epics und/oder User Stories) als XML-Element
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private XElement EncodeItemList(List<ItemBase> list)
        {
            XElement root = new XElement("ItemList");
            foreach (ItemBase item in list)
            {
                root.Add(ObjectToXElement(item));
            }
            return root;
        }

        /// <summary>
        /// Schreibt eine Liste von User Storiesals XML-Element
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private XElement EncodeItemList(List<UserStory> list)
        {
            XElement root = new XElement("UserStoryList");
            foreach (UserStory item in list)
            {
                root.Add(ObjectToXElement(item));
            }
            return root;
        }

        /// <summary>
        /// Schreibt eine Liste von Epics als XML-Element
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private XElement EncodeItemList(List<Epic> list)
        {
            XElement root = new XElement("EpicList");
            foreach (Epic item in list)
            {
                root.Add(ObjectToXElement(item));
            }
            return root;
        }

        /// <summary>
        /// Schreibt eine Liste von ItemControls als Liste.
        /// Dabei wird jede ItemControl als eine Repräsentation gesehen.
        /// Zwar sind aktuell nur eine Repräsentation (Darstellung) pro Item implementiert,
        /// jedoch erlaubt dies bereits mehrere Darstellungen für ein Item.
        /// Für jede Repräsentation werden die DisplaySettings (also Position, etc.) 
        /// gespeichert.
        /// </summary>
        /// <seealso cref="EncodeRepresentation"/>
        /// <param name="list"></param>
        /// <returns></returns>
        private XElement EncodeRepresentations(List<ItemControl> list)
        {
            XElement root = new XElement("Representations");
            foreach (ItemControl item in list)
            {
                root.Add(EncodeRepresentation(item));
            }
            return root;
        }
        /// <summary>
        /// Speichert die DisplaySettings einer ItemControl als XML-Element (Schritt 1 / 2)
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private XElement EncodeRepresentation(ItemControl control)
        {
            XElement root = new XElement("Representation");
            foreach (KeyValuePair<int, DisplaySettings> settings in control.DisplaySettings)
            {
                root.Add(EncodeDisplaySettings(settings));
            }
            return root;
        }

        /// <summary>
        /// Speichert die DisplaySettings einer ItemControl als XML-Element (Schritt 2 / 2)
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private XElement EncodeDisplaySettings(KeyValuePair<int, DisplaySettings> settings)
        {
            XElement root = new XElement("DisplaySettings");
            root.Add(new XElement("View", settings.Key));
            root.Add(new XElement("CenterX", settings.Value.CenterX));
            root.Add(new XElement("CenterY", settings.Value.CenterY));
            root.Add(new XElement("Rotation", settings.Value.Rotation));
            root.Add(new XElement("Scale", settings.Value.Scale));
            return root;
        }

        /// <summary>
        /// Schreibt eine User Story als XML-Element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private XElement EncodeUserStory(UserStory item)
        {
            XElement root = new XElement("UserStory");
            root.Add(new XElement("Priority", item.Priority));
            root.Add(new XElement("Effort", (int)item.Effort.Value));
            root.Add(new XElement("Backlog", (int)item.BacklogStatus));
            root.Add(new XElement("Text", item.Text));
            //-1 heißt: Keine Epic zugewiesen!
            root.Add(new XElement("Epic", (item.Epic == null) ? -1 : item.Epic.ItemID));
            EncodeItemBase(item, root);
            
            return root;

        }

        /// <summary>
        /// Schreibt eine Epic als XML-ELement
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private XElement EncodeEpic(Epic item)
        {

            XElement root = new XElement("Epic");
            //Es gibt keine Eigenschaften, die nur Epics besitzen
            EncodeItemBase(item, root);
            return root;
        }

        /// <summary>
        /// Fügt dem XML-Element Eigenschaften zu, die sowohl
        /// User Stories als auch Epics haben
        /// </summary>
        /// <param name="item">Das Item</param>
        /// <param name="root">Das XML-Element, zu dem die Eigenschaften hinzugefügt werden sollen</param>
        private void EncodeItemBase(ItemBase item, XElement root)
        {
            root.Add(new XElement("Title", item.Title));
            root.Add(new XElement("ItemID", item.ItemID));
            root.Add(new XElement("Visibility", item.IsVisible));
            root.Add(EncodeRepresentations(item.Representations));
        }
        #endregion
        #region Decode
        /// <summary>
        /// Wandelt ein Byte Array in ein Objekt vom Typ T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="surface">Die Surface, für welche diese Aktion ausgeführt wird</param>
        /// <returns></returns>
        public T ConvertToObject<T>(byte[] data, ScrumSurface surface)
        {
            return ConvertToObject<T>(encoding.GetString(data), surface);
        }

        /// <summary>
        /// Wandelt einen String in ein Objekt vom Typ T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="surface">Die Surface, für welche diese Aktion ausgeführt wird</param>
        /// <returns></returns>
        public T ConvertToObject<T>(string data, ScrumSurface surface)
        {
            return ConvertToObject<T>(XElement.Parse(data), surface);
        }

        /// <summary>
        /// Wandelt ein XML-Element in ein Objekt vom Typ T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="surface">Die Surface, für welche diese Aktion ausgeführt wird</param>
        /// <returns></returns>
        private T ConvertToObject<T>(XElement root, ScrumSurface surface)
        {
            if (root.HasElements)
            {
                return LoadItem<T>(root.Elements().First(), surface);
            }
            return default(T);
        }

        /// <summary>
        /// Wandelt ein XML-Element in ein Objekt von Typ T.
        /// Unterstützte Typen:
        /// -- UserStory
        /// -- Epic
        /// -- List of any Items
        /// -- List of Epics
        /// -- List of UserStories
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="surface"></param>
        /// <returns></returns>
        private T LoadItem<T>(XElement root, ScrumSurface surface)
        {
            return (T)LoadItem(root, surface);
        }

         /// <summary>
        /// Wandelt ein XML-Element in ein Objekt um.
        /// Unterstützte Typen:
        /// -- UserStory
        /// -- Epic
        /// -- List of any Items
        /// -- List of Epics
        /// -- List of UserStories
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="surface"></param>
        /// <returns></returns>
        private object LoadItem(XElement root, ScrumSurface surface)
        {
            switch (root.Name.ToString())
            {
                case "UserStory":
                    return ConvertToUserStory(root, surface);
                case "Epic":
                    return ConvertToEpic(root, surface);
                case "ItemList":
                    return ConvertToItemList<ItemBase>(root, surface);
                case "EpicList":
                    return ConvertToItemList<Epic>(root, surface);
                case "UserStoryList":
                    return ConvertToItemList<UserStory>(root, surface);
                default:
                    return null;
            }
        }

         /// <summary>
        /// Wandelt ein XML-Element in eine Liste mit Objekten von Typ T.
        /// Unterstützte Typen:
        /// -- List of any Items
        /// -- List of Epics
        /// -- List of UserStories
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="surface"></param>
        /// <returns></returns>
        private List<T> ConvertToItemList<T>(XElement root, ScrumSurface surface)
        {
                List<T> items = new List<T>();
                foreach (XElement element in root.Elements())
                {
                    items.Add(LoadItem<T>(element, surface));
                }
                return items;
        }

        /// <summary>
        /// Wandelt ein XML-Element in eine Epic um
        /// </summary>
        /// <param name="root"></param>
        /// <param name="surface"></param>
        /// <returns></returns>
        private Epic ConvertToEpic(XElement root, ScrumSurface surface)
        {
                Epic item = new Epic();
                ConvertToItemBase(root, item, surface);
                return item;
        }

        /// <summary>
        /// Wandelt ein XML-Element in eine User Story um
        /// </summary>
        /// <param name="root"></param>
        /// <param name="surface"></param>
        /// <returns></returns>
        private UserStory ConvertToUserStory(XElement root, ScrumSurface surface)
        {
                UserStory item = new UserStory();
                item.Priority = int.Parse(root.Element("Priority").Value);
                item.Effort.Value = int.Parse(root.Element("Effort").Value);
                item.temp_epicID = short.Parse(root.Element("Epic").Value);
                item.BacklogStatus = (ItemBacklogStatus)int.Parse(root.Element("Backlog").Value);
                item.Text = root.Element("Text").Value;
                ConvertToItemBase(root, item, surface);
                return item;
        }

        /// <summary>
        /// Wandelt eine XML-Element in eine Liste von Repräsentation um 
        /// Erlaubt somit das Laden von gespeicherten DisplaySettings
        /// </summary>
        /// <param name="root"></param>
        /// <param name="item"></param>
        /// <param name="surface"></param>
        private void LoadReps(XElement root, ItemBase item, ScrumSurface surface)
        {
            foreach (XElement rep in root.Elements())
            {
                ItemControl ic = item.CreateRepresentation(surface);
                foreach (XElement set in rep.Elements())
                {
                    int view = int.Parse(set.Element("View").Value);
                    DisplaySettings ds = LoadSettings(set);
                    if (ic.DisplaySettings.ContainsKey(view))
                    {
                        ic.DisplaySettings[view] = ds;
                    }
                    else
                        ic.DisplaySettings.Add(view, ds);
                }
            }
            
        }

        /// <summary>
        /// Lädt aus einem XML-Element DisplaySettings
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private DisplaySettings LoadSettings(XElement root)
        {
            DisplaySettings settings = new DisplaySettings();
            settings.CenterX = double.Parse(root.Element("CenterX").Value, CultureInfo.InvariantCulture);
            settings.CenterY = double.Parse(root.Element("CenterY").Value, CultureInfo.InvariantCulture);
            settings.Rotation = double.Parse(root.Element("Rotation").Value, CultureInfo.InvariantCulture);
            settings.Scale = double.Parse(root.Element("Scale").Value, CultureInfo.InvariantCulture);
            return settings;
        }

        /// <summary>
        /// Lädt die Eigenschaften in ein Item, die User Stories und Epics teilen
        /// </summary>
        /// <param name="root"></param>
        /// <param name="item"></param>
        /// <param name="surface"></param>
        private void ConvertToItemBase(XElement root,ItemBase item, ScrumSurface surface)
        {
            item.ItemID =  short.Parse(root.Element("ItemID").Value);
            item.Title = root.Element("Title").Value;
            item.IsVisible = bool.Parse(root.Element("Visibility").Value);
            LoadReps(root.Element("Representations"), item, surface);
        }
        #endregion
        #region Not Implemented
        public ItemBase ConvertToItem(byte[] data)
        {
            throw new NotImplementedException();
        }

        public ItemBase ConvertToItem(string data)
        {
            throw new NotImplementedException();
        }

        public List<ItemBase> ConvertToItemList(byte[] data)
        {
            throw new NotImplementedException();
        }

        public List<ItemBase> ConvertToItemList(string data)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region FileManager

        /// <summary>
        /// Lädt eine Datei in eine ScrumDatabase
        /// </summary>
        /// <param name="database">Die Datenbank, in die die Datei geladen werden soll</param>
        /// <param name="filepath">Der Pfad zur Datei</param>
        /// <param name="surface"></param>
        public void LoadFile(ScrumDatabase database, string filepath, ScrumSurface surface)
        {
            XElement root = LoadFileToXElement(filepath);
            XElement epics = root.Elements("EpicList").First();
            XElement userstories = root.Elements("UserStoryList").First();
            if (epics != null && userstories != null)
            {
                database.ClearItems();
                database.LoadItems(ConvertToItemList<Epic>(epics, surface));
                database.LoadItems(ConvertToItemList<UserStory>(userstories, surface));

            }
        }

        /// <summary>
        /// SChreibt eine Liste von Epics und User Stories in eine Datei
        /// </summary>
        /// <param name="epics">Die Epics</param>
        /// <param name="userstories">Die UserStories</param>
        /// <param name="filepath">Der Pfad zu der Datei</param>
        /// <returns></returns>
        public bool WriteFile(List<Epic> epics, List<UserStory> userstories, string filepath)
        {
            try
            {
                XElement root = WrapObjectWithXML(epics, userstories);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = this.encoding;
                using (MemoryStream memstream = new MemoryStream())
                {
                    using (XmlWriter writer = XmlWriter.Create(memstream))
                    {
                        root.WriteTo(writer);
                        writer.Flush();
                        WriteFile(memstream, filepath);
                    }
                    
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Lädt eine angegebene Datei in ein Basis-XML-Element zur weiteren Verarbeitung
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private XElement LoadFileToXElement(String filepath)
        {
            using (XmlReader reader = XmlReader.Create(ReadFile(filepath)))
            {
                XElement root = XElement.Load(reader);
                return root;
            }
        }
        /// <summary>
        /// Öffnet einen Stream für eine Datei
        /// </summary>
        /// <param name="filepath">Der Pfad zur Datei</param>
        /// <param name="writeMode">TRUE wenn in die Datei geschrieben werden soll</param>
        /// <returns></returns>
        private Stream GetFileStream(string filepath, bool writeMode)
        {
            FileAccess fa = writeMode ? FileAccess.Write : FileAccess.Read;
            CompressionMode cm = writeMode ? CompressionMode.Compress : CompressionMode.Decompress;
            FileStream fstream = File.Open(filepath, FileMode.OpenOrCreate, fa);
            return (Settings.Default.COMPRESS) ? new GZipStream(fstream, cm) :
                    (Stream)fstream;
        }
        //Öffnet einen Lese-Stream für eine Datei
        private Stream ReadFile(string filepath)
        {
            MemoryStream memstream = new MemoryStream();
            using (FileStream fstream = File.Open(filepath, FileMode.Open))
            {
                //Wenn in den Einstellungen bei COMPRESS TRUE angegeben wurde, 
                //wird die Datei komprimiert gespeichert und gelesen
                if (Settings.Default.COMPRESS)
                {
                    using (GZipStream gzstream = new GZipStream(fstream, CompressionMode.Decompress))
                    {
                        gzstream.CopyTo(memstream);
                    }
                }
                else
                    fstream.CopyTo(memstream);
            }
            memstream.Position = 0;
            return memstream;
        }
        /// <summary>
        /// Schreibt einen Stream von Daten in eine Datei
        /// </summary>
        /// <param name="data">Die Daten</param>
        /// <param name="filepath">Der Pfad</param>
        private void WriteFile(MemoryStream data, string filepath)
        {
            data.Position = 0;
            using (FileStream fstream = File.Create(filepath))
            {
                Console.WriteLine(fstream.CanWrite);
                //Wenn in den Einstellungen bei COMPRESS TRUE angegeben wurde, 
                //wird die Datei komprimiert gespeichert und gelesen
                if (Settings.Default.COMPRESS)
                {
                    using (GZipStream gzstream = new GZipStream(fstream, CompressionMode.Compress))
                    {
                        data.CopyTo(gzstream);

                        gzstream.Flush();
                        fstream.Flush();
                    }
                }
                else
                {
                    Console.WriteLine(data.Length);
                    data.CopyTo(fstream);
                    Console.Write(fstream.Length);
                    fstream.Flush();
                }
               
            }
        }
        #endregion
  
    }
}
