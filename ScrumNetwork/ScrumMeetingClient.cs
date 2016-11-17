using ScrumNetwork.Protocol;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace ScrumNetwork
{
    /// <summary>
    /// Stellt eine Klasse zum Erstellen eines Clients dar
    /// </summary>
    public class ScrumMeetingClient : ScrumMeetingEndPoint
    {
        #region Getter & Setter
        /// <summary>
        /// In dieser ClientInfo werden die Verbindungsdaten zum Server gespeichert
        /// </summary>
        public ClientInfo Server
        {
            get;
            set;
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Erstellt einen neuen ScrumMeetingClient. Kann anschließend mit der Funktion Start() gestartet werden.
        /// </summary>
        /// <param name="thisClient">Informationen zu dem lokalen Client. Als TCP-Port sollte allerdings der TCP-Port des Servers(!) angegeben werden</param>
        /// <param name="server_ip">Die IP-Adresse des Servers</param>
        public ScrumMeetingClient(ClientInfo thisClient, string server_ip) : base(thisClient)
        {
            Server = new ClientInfo();
            Server.IPAddress = server_ip;
            Server.TcpPort = thisClient.TcpPort;
        }
        #endregion
        #region Starting the ScrumClient
        /// <summary>
        /// Startet den Client
        /// </summary>
        /// <returns></returns>
        public override bool Start()
        {
            if(!IsRunning)
            {
                try{
                    Server.StartTCPClient();
                    //In einem extra Thread auf Nachrichten warten
                    new Thread(new ParameterizedThreadStart(ListenForSMCP)).Start(Server);
                   Log("Connected to " + Server.IPAddress + ":" + Server.TcpPort);
                    StartRunning();
                }catch(SocketException se)
                {
                    Log("ERROR: " + se.Message);
                }
            }
            return IsRunning;
        }
        #endregion
        #region Message Sending
        /// <summary>
        /// MultiCast Nachrichten werden von Clients zunächst an den Server geschickt
        /// </summary>
        /// <param name="msg">Die Nachricht</param>
        public override void MulticastMessage(SMCPMessage msg)
        {
            base.SendMessage(msg, Server);
        }
        /// <summary>
        /// Verschickt eine Nachricht an den Server
        /// Direktes senden an andere Clients ist bei Clients nicht möglich, daher zunächst an den Server schicken.
        /// </summary>
        /// <param name="msg">Die Nachricht</param>
        public void SendMessage(SMCPMessage msg)
        {
            base.SendMessage(msg, Server);
        }
        #endregion
        #region Message Handling
        /// <summary>
        /// Wird aufgerufen, wenn eine neue Nachricht empfangen wurde.
        /// An dieser Stelle werden NUR Nachrichten, die der Verbindungshaltung dienen, verarbeitet (ID kleiner als 9)
        /// </summary>
        /// <param name="msg">Die Nachricht</param>
        protected override void OnSMCPMessageReceived(SMCPMessage msg)
        {
            Log("Message Received: " + msg.ToString());
            if (msg.ActionID <= 9)
            {
                switch ((SMCPAction)msg.ActionID)
                {
                    case SMCPAction.ASSIGN_CLIENT_ID:
                        LocalClient.ClientID = Serializer.ConvertToObject<short>(msg.Data);
                        SMCPMessage reply = this.CreateMessage(LocalClient, false, SMCPAction.CLIENT_INFO);
                        SendMessage(reply);
                        break;
                    case SMCPAction.ONLINE_CLIENTS:
                        ConnectedClients = Serializer.ConvertToObject<Dictionary<short, ClientInfo>>(msg.Data);
                        if (ConnectedClients.ContainsKey(0))
                        {
                            if (ConnectedClients[0].IsServer)
                            {
                                ConnectedClients[0].TcpClient = Server.TcpClient;
                                Server = ConnectedClients[0];
                            }
                        }
                        OnClientsChanged();
                        break;
                }
            }
            base.OnSMCPMessageReceived(msg);
        }
        #endregion
        #region Destructor
        /// <summary>
        /// Schließt den TCP-Client
        /// </summary>
        public override void Dispose()
        {
            Server.TcpClient.Close();
            base.Dispose();
        }
        /// <summary>
        /// Schließt den Client
        /// </summary>
        /// <returns>TRUE</returns>
        public override bool Exit()
        {
            Dispose();
            StartClosing();
            return true;
        }
        #endregion
    }
}
