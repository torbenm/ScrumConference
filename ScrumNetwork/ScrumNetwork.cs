using ScrumNetwork.Protocol;
using ScrumNetwork.Utilities;
using ScrumTouchkit.Controls;
using ScrumTouchkit.Controls.Abstract;
using ScrumTouchkit.Controls.Buttons;
using ScrumTouchkit.Controls.Buttons.UI;
using ScrumTouchkit.Controls.Dialogs;
using ScrumTouchkit.Controls.Dialogs.UI;
using ScrumTouchkit.Controls.Network;
using ScrumTouchkit.Data;
using ScrumTouchkit.Events;
using ScrumTouchkit.Threading;
using ScrumTouchkit.Utilities;
using System;
using System.Linq;
using System.Windows;

namespace ScrumNetwork
{
    /// <summary>
    /// Schnittstelle zwischen Oberfläche und Client/Server Funktionalitäten
    /// </summary>
    public class ScrumNetwork
    {
        #region const
        /**
         * Die verschiedenen Buttons, die es nur bei aktiviertem Netzwerk gibt
         **/
        public const int CONNECTION_BUTTON = 4;
        public const int MUTE_BUTTON = 5;
        public const int SHAREVIEW_BUTTON = 6;
        #endregion
        #region vars, get, set
        /// <summary>
        /// Der verbundene EndPoint. Kann bei keiner bestehenden Verbindung auch NULL sein.
        /// </summary>
        public ScrumMeetingEndPoint Connection
        {
            get;
            private set;
        }

        /// <summary>
        /// Die ScrumSurface, mit welcher das Netzwerk verbunden ist
        /// </summary>
        public ScrumTouchkit.Controls.ScrumSurface Surface
        {
            get;
            private set;
        }
        /// <summary>
        /// Wenn Items (Epics, User Story) mit dem Netzwerk geteilt werden, müssen sie zunächst
        /// auf eine ID warten, die ihnen vom Server zugewiesen wird.
        /// </summary>
        private BlockingQueue<ItemBase> WaitingForID = new BlockingQueue<ItemBase>();

        /// <summary>
        /// Gibt an, ob eine Verbindung existiert
        /// </summary>
        public bool IsConnected
        {
            get { return Connection != null; }
        }

        /// <summary>
        /// Der Audiocontroller
        /// </summary>
        public Audio.AudioControl Audio
        {
            get;
            private set;
        }

        #endregion
        #region Constructor, Init
        /// <summary>
        /// Erstellt ein neues ScrumNetwork-Objekt
        /// </summary>
        /// <param name="surface">Die Oberfläche, mit welcher das ScrumNetwork verbunden werden soll</param>
        public ScrumNetwork(ScrumSurface surface)
        {
            this.Surface = surface;
            InitAudio();
            InitButtons();
            InitSurfaceListener();

        }

        /// <summary>
        /// Initialisiert den AudioController
        /// </summary>
        private void InitAudio()
        {
            Audio = new Audio.AudioControl();
            Audio.AudioCaptured += Audio_AudioCaptured;
        }

        /// <summary>
        /// Initialisiert die Buttons für die Oberfläche
        /// </summary>
        private void InitButtons()
        {
            Surface.Buttons.CreateButton(CONNECTION_BUTTON, Button.ButtonType.Image)
                .SetValue(Images.connect).Tapped += ConnectionButton_Tap;
            Surface.Buttons.CreateButton(MUTE_BUTTON, Button.ButtonType.Image)
                .SetValue(Images.high23).Tapped += MuteButton_Tap;

            Surface.Buttons.CreateButton(SHAREVIEW_BUTTON, Button.ButtonType.Text)
                .SetValue("Share View").Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Initialisiert die Verbindung mit der Oberfläche
        /// </summary>
        private void InitSurfaceListener()
        {
            Surface.ViewController.ViewChanged += ViewController_ViewChanged;
            Surface.ItemCreated += Surface_StoryCreated;
            //Verbindung zur Mülltonne
            (Surface.Buttons.Buttons[ButtonController.DELETE_BUTTON] as DeleteButton).ItemRemoved += ScrumNetwork_ItemRemoved;
            Surface.Database.EditorStateChanged += Database_EditorStateChanged;
            Surface.Database.NetworkDataAvailable += Database_NetworkDataAvailable;
            Surface.Database.ItemChanged += Database_ItemChanged;
            Surface.Database.FocusRequested += Database_FocusRequested;
            Surface.FileLoaded += Surface_LoadedFile;
        }


        #endregion
        #region Buttons events
        /// <summary>
        /// Wenn Audio aufgenommen wurde, wird es mit dem Netzwerk geteilt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Audio_AudioCaptured(object sender, Audio.AudioEventArgs e)
        {
            ShareAudio(e.Data);
        }
        /// <summary>
        /// Wenn der Mute-Button berührt wurde, wird das Audio an oder ausgeschaltet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuteButton_Tap(object sender, EventArgs e)
        {
            //TODO: AUDIO IMPLEMENT
            Audio.ToggleMute();
            Surface.Buttons.Buttons[MUTE_BUTTON].SetValue(
                !Audio.Muted ? Images.high23 : Images.volume52
                );
        }

        /// <summary>
        /// Bietet den Dialog zum erstellen einer Verbindung an
        /// oder beendet eine aktive Verbindung
        /// (Verbindungsaufbau Schritt 1 von 3)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectionButton_Tap(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                CloseConnection();
                Surface.Buttons.Buttons[CONNECTION_BUTTON].SetValue(Images.connect);
                MessageDialog.ShowMessage(this.Surface, "Connection closed!", false);

            }
            else
            {
                DialogControl<ChooseEndPointType>.ShowDialog(this.Surface).DialogFinished += ChooseConnection_DialogFinished;
            }

        }

        /// <summary>
        /// Wenn der ChooseConnection-Dialog beendet ist,
        /// werden die benötigten Daten abgefragt (CLIENT)
        /// oder der Server gestartet (SERVER)
        /// (Verbindungsaufbau Schritt 2 von 3)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseConnection_DialogFinished(object sender, DialogEventArgs e)
        {
            if (e.ExitMode == ChooseEndPointType.SERVER)
            {
                if (CreateServer(Settings.Default.CLIENT_NAME,
                       Settings.Default.STD_TCP_PORT,
                        Settings.Default.UDP_PORT))
                {
                    MessageDialog.ShowMessage(this.Surface,
                        "New Server created\nIP-Address: " + Connection.LocalClient.IPAddress
                        + "\nTCP Port: " + Connection.LocalClient.TcpPort,
                        false);
                    Surface.Buttons.Buttons[CONNECTION_BUTTON].SetValue(Images.disconnect);
                }
                else
                {
                    MessageDialog.ShowMessage(this.Surface, "Failed to create Server!", true);
                }
            }
            else
            {
                DialogControl<CreateClient>.ShowDialog(this.Surface).DialogFinished += CreateClient_DialogFinished;
            }
        }

        /// <summary>
        /// Verbindet einen Client mit einem Server (Verbindungsaufbau Schritt 3 / 3)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateClient_DialogFinished(object sender, DialogEventArgs e)
        {
            if (e.ExitMode == ScrumTouchkit.Controls.Dialogs.UI.CreateClient.CONNECT)
            {
                if (CreateClient(Settings.Default.CLIENT_NAME, e.Port, Settings.Default.UDP_PORT, e.IP.ToString()))
                {
                    MessageDialog.ShowMessage(this.Surface, "Client sucessfully created!", false);
                    Surface.Buttons.Buttons[CONNECTION_BUTTON].SetValue(Images.disconnect);
                }
                else
                {
                    MessageDialog.ShowMessage(this.Surface, "Failed to create client!", true);
                }
            }
        }
        #endregion
        #region Connection Establishing
        /// <summary>
        /// Erstellt einen neuen Server
        /// </summary>
        /// <param name="host_name">Der Name des Servers</param>
        /// <param name="tcp_port">Der TCP-Port des Servers</param>
        /// <param name="udp_port">Der UDP-Port des Servers</param>
        /// <returns>TRUE bei Erfolg</returns>
        public bool CreateServer(String host_name, int tcp_port, int udp_port)
        {
            if (IsConnected)
                return false;
            ClientInfo server = new ClientInfo(host_name, true, udp_port, tcp_port);
            Connection = new ScrumMeetingServer(server);
            if (Connect())
                return true;
            else
            {
                Connection = null;
                return false;
            }
        }
        /// <summary>
        /// Erstellt einen neuen Client und verbindet mit einem Server
        /// </summary>
        /// <param name="client_name">Der Name des Clients</param>
        /// <param name="tcp_port">Der TCP-Port des  <u>Servers</u></param>
        /// <param name="udp_port">Der TCP-Port des <u>Clients</u></param>
        /// <param name="ipAddress">Die IP-Adresse des <u>Servers</u></param>
        /// <returns>TRUE bei Erfolg</returns>
        public bool CreateClient(String client_name, int tcp_port, int udp_port, String ipAddress)
        {
            if (IsConnected)
                return false;
            ClientInfo client = new ClientInfo(client_name, false, udp_port, tcp_port);
            Connection = new ScrumMeetingClient(client, ipAddress);
            if (Connect())
                return true;
            else
            {
                Connection = null;
                return false;
            }
        }
        /// <summary>
        /// Startet Server bzw. Verbindung und vernüpft alle Events
        /// </summary>
        /// <returns>TRUE bei Erfolg</returns>
        private bool Connect()
        {

            Connection.SMCPMessageReceived += Connection_SMCPMessageReceived;
            Connection.SMLPMessageReceived += Connection_SMLPMessageReceived;
            Connection.ClientAccepted += Connection_ClientAccepted;
            Connection.ConnectionClosed += Connection_ConnectionClosed;
            Audio.StartAll();
            return Connection.Start();
        }
        /// <summary>
        /// Schließt die aktuelle Verbindung
        /// </summary>
        /// <returns>TRUE bei Erfolg</returns>
        public bool CloseConnection()
        {

            if (!IsConnected || Connection.IsClosing)
                return false;
            if (Connection.Exit())
            {
                Audio.StopAll();
                Connection = null;
                return true;
            }
            return false;
        }
        #endregion
        #region Connection Event Handling
        /// <summary>
        /// Wenn die Verbindung verloren wurde, wird ein Speichern der aktuellen Items (Epics, User Stories) angeboten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Connection_ConnectionClosed(object sender, EventArgs e)
        {
            Surface.Buttons.Buttons[CONNECTION_BUTTON].SetValue(Images.connect);
            Surface.AskToSave("The connection was lost.\n Do you want to save the data?");
        }
        /// <summary>
        /// Verarbeitet ankommende SMCP-Nachrichten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Die SMCP-Nachricht</param>
        private void Connection_SMCPMessageReceived(object sender, Protocol.SMCPMessage e)
        {
            switch ((SMCPAction)e.ActionID)
            {
                case SMCPAction.VIEW_CHANGED:
                    ///Die Ansicht wurde im Netzwerk verändert -> auch auf diesem Client ändern
                    ChangeView(e.GetData<int>(this.Connection));
                    break;
                case SMCPAction.REQUEST_ITEM_ID:
                    {
                        //  Einer der Clients hat eine neue Item-ID angefragt -> Falls es sich hier um einen Server handelt,
                        //  schicke eine neue ID zurück
                        if (this.Connection.IsServer)
                        {
                            SMCPMessage respone = Connection.CreateMessage(UserStory.NextUserStoryID++, false, SMCPAction.ASSIGN_ITEM_ID);
                            Connection.SendMessage(respone, Connection.ConnectedClients[e.SenderID]);
                        }
                    }
                    break;
                case SMCPAction.ASSIGN_ITEM_ID:
                    //Neue Item-ID wurde angefragt und erhalten -> weise sie einem der wartenden Items zu
                    //Client: Item Teilen - Schritt 2 / 3
                    ItemBase item = WaitingForID.Dequeue();
                    if (item != null)
                    {
                        item.ItemID = e.GetData<short>(this.Connection);
                        ShareItemFinally(item);
                    }
                    break;
                case SMCPAction.ADD_ITEM:
                    {
                        //Neues Item erhalten, zu den Items der Oberfläche hinzufügen
                        AddItem(e.GetData<ItemBase>(this.Connection));
                    }
                    break;
                case SMCPAction.START_EDITING:
                    //In den (Readonly)-Editiermodus eines Items wechseln
                    ChangeEditorState(e.GetData<ItemBase>(this.Connection).ItemID, true);
                    break;
                case SMCPAction.END_EDITING:
                    //Den Editiermodus wieder beenden
                    ChangeEditorState(e.GetData<ItemBase>(this.Connection).ItemID, false);
                    break;
                case SMCPAction.UPDATE_ITEM:
                    //Die Daten eines Items aktualisieren (wurde von einem anderem Teilnehmer initiiert)
                    UpdateItem(e.GetData<ItemBase>(this.Connection));
                    break;
                case SMCPAction.FOCUS_ON_ITEM:
                    //Ein Item in den Fokus aller Teilnehmer rücken (wurde von einem anderem Teilnehmer initiiert)
                    RequestFocus(e.GetData<short>(this.Connection));
                    break;
                case SMCPAction.REMOVE_ITEM:
                    //Ein Item wurde von einem anderen Teilnehmer gelöscht
                    RemoveItem(e.GetData<short>(this.Connection));
                    break;
                case SMCPAction.ALL_ITEMS:
                    //Liste von Items erhalten -> Aktuelle Datenbank durch Internet Datenbank ersetzen
                    Surface.Invoke(() =>
                        {
                            SendableDatabase sdb = e.GetData<SendableDatabase>(this.Connection);
                            sdb.LoadIntoDB(Surface.Database);
                        });
                    break;
            }
        }

        /// <summary>
        /// Verarbeitet ankommende SMLP-Nachrichten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        private void Connection_SMLPMessageReceived(object sender, Protocol.SMLPMessage msg)
        {
            if (msg.LocalReceiver == SMLPMessage.AUDIO)
            {
                Audio.BufferAudio(msg.Data);
            }
            else
            {
                /**
                 * Wenn es sich nicht um ein Audio-Sample handelt, wird die Nachricht an das lokale
                 * Objekt weitergeleitet. Es handelt sich folglich um einen Zustand einer Textbox zur 
                 * Liveverfolgung von Änderungen
                 **/
                TextBoxState state = Connection.Serializer.ConvertToObject<TextBoxState>(msg.Data);
                ItemBase item = Surface.Database.GetItem(msg.LocalReceiver);
                if (item != null)
                {
                    item.ProcessLiveData(state);
                }
            }
        }
        /// <summary>
        /// Diese Funktion wird nur von Servern aufgerufen
        /// Wenn ein neuer Client sich an dem Server anmelden, 
        /// werden diesem die Items im Meeting und die aktuelle Ansicht mitgeteilt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_ClientAccepted(object sender, ClientInfo e)
        {
            ShareDatabase(e);
            ViewController_ViewChanged(null, new GenericEventArgs<int>(Surface.ViewController.CurrentViewID));
        }
        #endregion
        #region Network To Surface
        /**
         * In dieser Region findet die Kommunikation vom Netzwerk zu 
         * dieser ScrumSurface statt. Wenn somit die entsprechenden Nachrichten
         * über das Netzwerk ankommen, werden sie hier an die ScrumSurface weitergeleitet.
         **/

        /// <summary>
        /// Ändert die Ansicht zu dem angegebenem View
        /// </summary>
        /// <param name="viewID">Der View</param>
        protected void ChangeView(int viewID)
        {
            Surface.Invoke(new Action(() =>
                {
                    Surface.ViewController.ActivateView(viewID, true);
                }));
        }
        /// <summary>
        /// Fügt ein neues Item zur Oberfläche hinzu
        /// </summary>
        /// <param name="item">Das neue Item</param>
        protected void AddItem(ItemBase item)
        {
            Surface.Invoke(new Action(() =>
                {
                    Surface.Database.AddItem(item);
                }));
        }

        /// <summary>
        /// De-/Aktiviert den readonly-Editiermodus bei dem angebenen Item
        /// </summary>
        /// <param name="itemID">Das Item, bei dem der Editiermodus de-/aktiviert werden soll</param>
        /// <param name="starting">TRUE für Aktivierung, FALSE für Deaktivierung</param>
        protected void ChangeEditorState(short itemID, bool starting)
        {
            Surface.Invoke(new Action(() =>
                 {
                     ItemBase item = Surface.Database.GetItem(itemID);
                     if (item != null)
                     {
                         item.ExternalEditorChange(starting);
                     }
                 }));
        }

        /// <summary>
        /// Aktualisiert das entsprechende Item mit den angegebenen Daten
        /// </summary>
        /// <param name="item">Die neuen Daten für das Item</param>
        protected void UpdateItem(ItemBase item)
        {
            Surface.Invoke(new Action(() =>
                {
                    ItemBase l_item = Surface.Database.GetItem(item.ItemID);
                    if (l_item != null)
                    {
                        l_item.UpdateData(item, Surface.Database);

                    }
                }));
        }
        /// <summary>
        /// Rückt das entsprechende Item in den Fokus
        /// </summary>
        /// <param name="itemID">Das Item</param>
        protected void RequestFocus(short itemID)
        {
            Surface.Invoke(new Action(() =>
                {
                    ItemBase item = Surface.Database.GetItem(itemID);
                    if (item != null)
                    {
                        if (item.Representations.Count > 0)
                        {
                            item.Representations.First().RequestFocus();
                        }
                    }
                }));
        }

        /// <summary>
        /// Entfernt das Item
        /// </summary>
        /// <param name="item">Das Item, das zu entfernen ist</param>
        protected void RemoveItem(short item)
        {
            Surface.Invoke(new Action(() =>
            {
                ItemBase l_item = Surface.Database.GetItem(item);
                if (l_item != null)
                {
                    Surface.Database.RemoveItem(l_item);
                }
            }));
        }
        #endregion
        #region Surface To Network
        /**
         * Wenn Änderungen auf der ScrumSurface vorgenommen werden und eine
         * Verbindung zu einem Netzwerk besteht, werden diese an dieser
         * Stelle zum Netzwerk weitergeleitet
         **/


        /// <summary>
        /// Teilt dem Netzwerk die gerade neu geladene Datenbank mit (zum Beispiel, 
        /// wenn eine neue Datei geladen wurde)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Surface_LoadedFile(object sender, EventArgs e)
        {
            ShareDatabase();
        }
        /// <summary>
        /// Teilt dem Netzwerk die aktuelle, lokale Item-Datenbank mit.
        /// Falls eine ClientInfo angegeben ist, wird nur diesem Client
        /// die Datenbank mitgeteilt
        /// </summary>
        /// <param name="ci">[Optional] Der Empfänger</param>
        private void ShareDatabase(ClientInfo ci = null)
        {
            if (IsConnected)
            {
                SendableDatabase sdb = new SendableDatabase();
                sdb.Epics = Surface.Database.Epics;
                sdb.UserStories = Surface.Database.UserStories;

                SMCPMessage msg = Connection.CreateMessage(sdb, ci == null, SMCPAction.ALL_ITEMS);
                if (msg.Multicast)
                    Connection.MulticastMessage(msg);
                else
                    Connection.SendMessage(msg, ci);
            }
        }

        /// <summary>
        /// Wenn die Ansicht geändert wurde, wird die neu gewählte Ansicht
        /// den anderen Teilnehmern mitgeteilt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Die ID der neuen Ansicht</param>
        private void ViewController_ViewChanged(object sender, GenericEventArgs<int> e)
        {
            if (IsConnected)
            {
                SMCPMessage msg = Connection.CreateMessage(e.Value, true, SMCPAction.VIEW_CHANGED);
                Connection.MulticastMessage(msg);
            }
        }

        /// <summary>
        /// Wenn eine neue UserStory erstellt wurde, wird diese automatisch
        /// mit dem Netzwerk geteilt. Mit diesem Programm können bisher <u>keine</u>
        /// neuen UserStories erstellt werden, daher wird es an dieser Stelle auch 
        /// nicht betrachtet.
        /// Client: Item Teilen - Schritt 1 / 3
        /// Server: Item Teilen - Schritt 1 / 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Die neue UserStory</param>
        private void Surface_StoryCreated(object sender, ScrumTouchkit.Data.UserStory e)
        {
            if (IsConnected)
            {
                if (Connection.IsServer)
                {
                    //Wenn der lokale Client ein Server ist, muss nicht erst der neue Server
                    //kontaktiert werden, um an eine UserStoryID zu gelangen.
                    e.ItemID = UserStory.NextUserStoryID++;
                    ShareItemFinally(e);
                }
                else
                {
                    WaitingForID.Enqueue(e);
                    SMCPMessage msg = Connection.CreateMessage(null, false, SMCPAction.REQUEST_ITEM_ID);
                    (Connection as ScrumMeetingClient).SendMessage(msg);

                }
            }
        }
        /// <summary>
        /// Wenn die neue UserStory eine ID erhalten hat, kann sie 
        /// endgültig mit dem Netzwerk geteilt werden
        /// Client: Item Teilen - Schritt 3 / 3
        /// Server: Item Teilen - Schritt 2 / 2
        /// </summary>
        /// <param name="item">Das Item, das geteilt werden soll</param>
        private void ShareItemFinally(ItemBase item)
        {
            SMCPMessage msg = Connection.CreateMessage(item, true, SMCPAction.ADD_ITEM);
            Connection.MulticastMessage(msg);
        }

        /// <summary>
        /// Wenn ein Item gelöscht wurde, wird diese Aktion auch den anderen
        /// Teilnehmern mitgeteilt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Das gelöschte Item</param>
        private void ScrumNetwork_ItemRemoved(object sender, ItemBase e)
        {
            if (IsConnected)
            {
                SMCPMessage msg = Connection.CreateMessage(e.ItemID, true, SMCPAction.REMOVE_ITEM);
                Connection.MulticastMessage(msg);
            }
        }

        /// <summary>
        /// Wenn auf diesem Tisch der Editiermodus für ein Item aktiviert (oder beendet wurde)
        /// wird dies auch den anderen Teilnehmern mitgeteilt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Database_EditorStateChanged(object sender, ScrumTouchkit.Events.EditorStateEventArgs e)
        {
            if (IsConnected)
            {

                SMCPMessage msg = Connection
                .CreateMessage(e.Data, true, e.IsStarting ? SMCPAction.START_EDITING : SMCPAction.END_EDITING);
                Connection.MulticastMessage(msg);
            }
        }


        /// <summary>
        /// Wenn beim Bearbeiten eines Items der Inhalt eines Textfelds bearbeitet wurde, 
        /// wird an dieser Stelle der Status des Textfelds den anderen Teilnehmern mitgeteilt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Database_NetworkDataAvailable(object sender, ScrumTouchkit.Events.TextBoxStateEventArgs e)
        {
            if (!IsConnected)
                return;
            SMLPMessage msg = Connection.CreateMessage(e.State, e.Data.ItemID);
            Connection.MulticastMessage(msg);
        }

        /// <summary>
        /// Wenn die Eigenschaften eines Items geändert wurden,
        /// wird auch dies an dieser Stelle den anderen Teilnehmern mitgeteilt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Database_ItemChanged(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                ItemBase item = sender as ItemBase;
                SMCPMessage msg = Connection
                .CreateMessage(item, true, SMCPAction.UPDATE_ITEM);
                Connection.MulticastMessage(msg);
            }
        }

        /// <summary>
        /// Wenn auf diesem Tisch ein Item in den Fokus gerückt wurde,
        /// wird dies an dieser Stelle den anderen Tischen mitgeteilt
        /// </summary>
        /// <param name="sender">Das Item, welches nun im Fokus ist</param>
        /// <param name="e"></param>
        private void Database_FocusRequested(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                ItemBase item = (sender as ItemControl).Item;
                SMCPMessage msg = Connection
                .CreateMessage(item.ItemID, true, SMCPAction.FOCUS_ON_ITEM);
                Connection.MulticastMessage(msg);
            }
        }

        /// <summary>
        /// Wenn Audio aufgenommen wurde, wird es an die anderen
        /// Teilnehmer geschickt
        /// </summary>
        /// <param name="audio">Das aufgenommene Audio</param>
        public void ShareAudio(byte[] audio)
        {
            if (IsConnected)
            {
                SMLPMessage smlp = Connection.CreateMessage(audio, SMLPMessage.AUDIO);
                Connection.MulticastMessage(smlp);
            }
        }
        #endregion
    }

}
