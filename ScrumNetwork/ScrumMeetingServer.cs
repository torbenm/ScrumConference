using ScrumNetwork.Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ScrumNetwork
{

    /// <summary>
    /// Stellt eine Klasse zur Erstellung eines Servers dar
    /// </summary>
    public class ScrumMeetingServer : ScrumMeetingEndPoint
    {
        
        #region var, get, set
        private TcpListener tcpListener;
        private short nextClientID = 1;

        /// <summary>
        /// Wenn sich ein neuer Client bei diesem Server anmeldet,
        /// erhält er diese ID und nextClientID wird einen höhergezählt.
        /// </summary>
        public short NextClientID
        {
            get { return nextClientID; }
        }

        #endregion
        #region Constructor

        /// <summary>
        /// Erstellt einen neuen Server
        /// </summary>
        /// <param name="connectionInfo">Die ClientInfo zu diesem Server</param>
        public ScrumMeetingServer(ClientInfo connectionInfo) : base(connectionInfo)
        {
             this.LocalClient.IsServer = true;
            this.ConnectedClients.Add(connectionInfo.ClientID, connectionInfo);
            this.ClientsChanged += event_ClientsChanged;
        }

       
        #endregion
        #region events
        /// <summary>
        /// Wenn sich die Liste der angemeldeten Clients verändert hat,
        /// wird die neue den anderen Clients mitgeteilt
        /// </summary>
        private void event_ClientsChanged(object sender, EventArgs e)
        {
            ShareConnectedClients();
        }
        #endregion
        #region Starting Server & Accepting Clients

        /// <summary>
        /// Startet den Server
        /// </summary>
        /// <returns>TRUE, falls erfolgreich</returns>
        public override bool Start()
        {
            if (!IsRunning)
            {
                try
                {
                    this.tcpListener = new TcpListener(IPAddress.Any, LocalClient.TcpPort);
                    new Thread(new ThreadStart(AcceptClients)).Start();
                    StartRunning();
                   Log("Created a new server");
                }
                catch
                {
                   Log("Failed to create server on port " + LocalClient.TcpPort);
                }
            }
            return IsRunning;
        }
        /// <summary>
        /// Wartet auf neue Clients und akzeptiert diese
        /// Startet zudem anschließend das Lauschen auf Nachrichten
        /// des neuen Clients sowie den Anmeldeprozess
        /// </summary>
        private void AcceptClients()
        {
            tcpListener.Start();
                while (true && !IsClosing)
                {
                    try
                    {
                        TcpClient client = this.tcpListener.AcceptTcpClient();

                        ClientInfo info = new ClientInfo(client);
                        info.ClientID = nextClientID++;

                        ConnectedClients.Add(info.ClientID, info);
                        new Thread(new ParameterizedThreadStart(ListenForSMCP)).Start(info);
                        AcceptClient(info);
                    }
                    catch(Exception e)
                    {
                        Log(e.Message);
                    }

                }
            
        }
        /// <summary>
        /// Startet den Anmeldeprozess eines neuen Clients
        /// </summary>
        /// <param name="newClient">Der neue Client</param>
        public void AcceptClient(ClientInfo newClient)
        {
            SMCPMessage msg = CreateMessage(newClient.ClientID, false, SMCPAction.ASSIGN_CLIENT_ID);
            SendMessage(msg, newClient);
            Log("New Client arrived");
        }

        /// <summary>
        /// Löscht den verlassenen Client aus der Liste der Clients
        /// </summary>
        /// <param name="client">Der Client, zu dem die Verbindung verloren wurde</param>
        protected override void ConnectionLost(ClientInfo client)
        {
            Log("Client " + client.Name + " has left");
            OnClientLeft(client);
            ConnectedClients.Remove(client.ClientID);
            OnClientsChanged();
        }
        #endregion
        #region Message Handling
        /// <summary>
        /// Verarbeitet ankommende SMCP-Nachrichten
        /// Hier werden allerdings nur für die Verbindungsverwaltung wichtige
        /// Nachrichten (ID kleiner als 9) bearbeitet.
        /// Alle weiteren Nachrichten müssen außerhalb bearbeitet werden
        /// </summary>
        /// <param name="msg">Die ankommende Nachricht</param>
        protected override void OnSMCPMessageReceived(SMCPMessage msg)
        {
            Log("Message Received: " + msg.Data);
            if (msg.Multicast)
                MulticastMessage(msg);
            if(msg.ActionID  <= 9)
            {

                switch ((SMCPAction)msg.ActionID)
                {
                    case SMCPAction.CLIENT_INFO:
                        {
                            ClientInfo ci = Serializer.ConvertToObject<ClientInfo>(msg.Data);
                            ci.TcpClient = ConnectedClients[ci.ClientID].TcpClient;
                            ConnectedClients[ci.ClientID] = ci;
                            OnClientsChanged();
                            OnClientAccepted(ci);
                        }
                        break;
  
                }
            }
            base.OnSMCPMessageReceived(msg);
        }
        #endregion
        #region Providing Information
        /// <summary>
        /// Verschickt eine Message, die alle verbundenen Clients beinhaltet
        /// </summary>
        /// <param name="receiver">Wenn angegeben, wird nur an diesen Client die NAchricht verschickt, ansonsten an alle</param>
        public void ShareConnectedClients(ClientInfo receiver = null)
        {
                SMCPMessage connectedClients = CreateMessage(ConnectedClients, true, SMCPAction.ONLINE_CLIENTS);
                if (receiver == null)
                {
                    MulticastMessage(connectedClients);
                }
                else
                {
                    SendMessage(connectedClients, receiver);
                }
        }
        #endregion
        #region Destructor
        /// <summary>
        /// Beginnt den Verbindungsabbau des Servers
        /// </summary>
        public override void StartClosing()
        {
            tcpListener.Stop();
            base.StartClosing();
        }
        /// <summary>
        /// Schließt den Server.
        /// Kann nur aufgerufen werden, wenn keine Clients mehr verbunden sind.
        /// </summary>
        /// <returns>TRUE bei Erfolg</returns>
        public override bool Exit()
        {
            if (this.ConnectedClients.Count == 0)
            {
                StartClosing();
                return true;
            }
            return false;
        }
        #endregion




    }
}
