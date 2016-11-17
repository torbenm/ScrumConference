using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using ScrumTouchkit.Threading;
using ScrumTouchkit.Utilities.Serializer;
using ScrumNetwork.Protocol;

namespace ScrumNetwork
{
    /// <summary>
    /// Basisklasse für ScrumMeetingClient und ScrumMeetingServer
    /// Stellt die grundlegenden Funktionen zum Verbindungsaufbau und -verwaltung dar
    /// </summary>
    public abstract class ScrumMeetingEndPoint : DispatcherClass, IDisposable
    {
        #region Getter & Setter
        protected static ISerializer _serializer = new JSONSerializer();
        private UdpClient udp_client;
        private bool _running = false;

        /// <summary>
        /// Alle mit dem aktuellen Netzwerk verbundenden Clients
        /// </summary>
        public Dictionary<short, ClientInfo> ConnectedClients
        {
            get;
            set;
        }

        /// <summary>
        /// Der Client des hiesigen Computers
        /// </summary>
        public ClientInfo LocalClient
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt an, ob dieser EndPoint ein Server ist
        /// </summary>
        public bool IsServer
        {
            get { return this.GetType() == typeof(ScrumMeetingServer); }
        }

        /// <summary>
        /// Gibt an, ob der EndPoint läuft 
        /// </summary>
        public bool IsRunning
        {
            get { return _running; }
        }

        /// <summary>
        /// Der Serializer zum serialisieren von Nachrichten (Standard: JSON)
        /// </summary>
        public ISerializer Serializer
        {
            get { return _serializer; }
        }

        /// <summary>
        /// Gibt an, ob der EndPoint bei der nächsten Möglichkeit die Verbindung beendet
        /// </summary>
        public bool IsClosing
        {
            get;
            private set;
        }
        #endregion
        #region Constructor

        /// <summary>
        /// Erstellt ein neues EndPoint-Objekt.
        /// </summary>
        /// <param name="client">Die ClientInfos zum lokalen Computer</param>
        public ScrumMeetingEndPoint(ClientInfo client)
            : base(true)
        {
            this.ConnectedClients = new Dictionary<short, ClientInfo>();
            this.LocalClient = client;
            udp_client = new UdpClient(LocalClient.UdpPort);
            new Thread(new ThreadStart(ListenForSMLP)).Start();
        }
        #endregion
        #region Events

        public event EventHandler<ClientInfo> ClientAccepted;
        public event EventHandler<ClientInfo> ClientLeft;
        public event EventHandler<SMCPMessage> SMCPMessageReceived;
        public event EventHandler<SMLPMessage> SMLPMessageReceived;
        public event EventHandler ClientsChanged;
        public event EventHandler ConnectionClosed;

        protected void OnClientAccepted(ClientInfo info)
        {
            if (ClientAccepted != null && IsServer)
                ClientAccepted(this, info);
        }
        protected void OnClientLeft(ClientInfo info)
        {
            if (ClientLeft != null && IsServer)
                ClientLeft(null, info);
        }

        protected virtual void OnSMCPMessageReceived(SMCPMessage msg)
        {
            if (msg.ActionID > 9)
            {
                if (SMCPMessageReceived != null)
                    SMCPMessageReceived(this, msg);
            }
        }
        protected virtual void OnClientsChanged()
        {
            if (ClientsChanged != null)
                ClientsChanged(this, new EventArgs());
        }
        protected virtual void OnSMLPMessageReceived(SMLPMessage msg)
        {
            if (SMLPMessageReceived != null)
                SMLPMessageReceived(this, msg);
        }
        public virtual void OnConnectionClosed()
        {
            if (ConnectionClosed != null)
                ConnectionClosed(this, new EventArgs());
        }
        #endregion
        #region Starting the EndPoint

        /// <summary>
        /// Startet den EndPoint
        /// </summary>
        /// <returns></returns>
        public abstract bool Start();

        /// <summary>
        /// Setzt IsRunning auf TRUE
        /// </summary>
        protected void StartRunning()
        {
            _running = true;
        }
        #endregion
        #region Stream Listening
        /// <summary>
        /// Wartet für neue SMCP-Nachrichten und leitet diese zur Verarbeitung weiter
        /// </summary>
        /// <param name="remClient">Der Client, bei dessen Stream auf Nachrichten gewartet werden soll</param>
        protected void ListenForSMCP(ClientInfo remClient)
        {
            NetworkStream clientStream = remClient.TcpClient.GetStream();
            SMCPMessage message;
            //Wenn der EndPoint bald schließt --> aufhören auf Nachrichten zu warten!
            while (!IsClosing)
            {
                try
                {
                    message = SMCPMessage.ReadNextMessage(clientStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
                if (message != null)
                {
                    this.Invoke(new Action(() =>
                        {
                            OnSMCPMessageReceived(message);
                        }));
                }
            }
            //Verbindung schließen und angeben, dass die Verbindung verloren wurde (entsprechendes Event aufrufen)
            remClient.TcpClient.Close();
            ConnectionLost(remClient);
        }
        /// <summary>
        /// Erlaubt das starten dieser Funktion in einem neuen Thread:
        /// new Thread(new ParameterizedThreadStart(ListenForSMCP)).Start(clientInfo);
        /// </summary>
        /// <param name="client">Muss eine ClientInfo sein!</param>
        protected void ListenForSMCP(object client)
        {
            if (client is ClientInfo)
                ListenForSMCP((ClientInfo)client);
        }

        /// <summary>
        /// Wartet auf neue SMLP Nachrichten am UDP-Port
        /// </summary>
        protected void ListenForSMLP()
        {
            SMLPMessage message;
            while (!IsClosing)
            {
                try
                {
                    message = SMLPMessage.ReadNextMessage(udp_client);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
                if (message != null)
                {
                    OnSMLPMessageReceived(message);
                }
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn ein Client die Verbindung aufgibt.
        /// Da Standardmäßig ein Client mit einem Server verbunden ist, und somit nur
        /// eine TCP-Verbindung besitzt, wird die gesamte lokale Verbindung geschlossen.
        /// In ScrumMeetingServer muss dies abgeändert werden, da es mehrere TCP-Verbindungen
        /// zu den verschiedenen Clients gibt.
        /// </summary>
        /// <param name="client">Der Client, zu dem die Verbindung verloren wurde</param>
        protected virtual void ConnectionLost(ClientInfo client)
        {
            Log("Connection was lost");
            Dispose();
        }
        #endregion
        #region Message Sending
        /// <summary>
        /// Verschickt eine SMLP-Nachricht an den angegebenen Client
        /// </summary>
        /// <param name="msg">Die Nachricht</param>
        /// <param name="client">Der Client</param>
        public void SendMessage(SMLPMessage msg, ClientInfo client)
        {
            if (client.IPAddress != null)
            {
                Log("Send: " + msg.Data.Length + " bytes");
                msg.WriteMessage(this.udp_client, client.GetUDPEndPoint());
            }
        }
        /// <summary>
        /// Verschickt eine SMCP-Nachricht an den angegebenen Client
        /// </summary>
        /// <param name="msg">Die Nachricht</param>
        /// <param name="client">Der Client</param>
        public void SendMessage(SMCPMessage msg, ClientInfo client)
        {
            if (client != null && client.TcpClient != null)
            {
                Log("Send: " + msg.Data.Length + " bytes, " + msg);
                msg.WriteMessage(client.TcpClient.GetStream());

            }
        }

        /// <summary>
        /// Versendet eine SMLP-Nachricht an alle verbundenen Clients
        /// </summary>
        /// <param name="msg">Die Nachricht</param>
        public void MulticastMessage(SMLPMessage msg)
        {
            foreach (ClientInfo ci in ConnectedClients.Values)
            {

                if (ci.ClientID != msg.SenderID && ci.ClientID != this.LocalClient.ClientID && ci.UdpPort > 0)
                {
                    SendMessage(msg, ci);
                }

            }
        }

        /// <summary>
        /// Versendet eine SMCP-Nachricht an alle verbundenen Clients
        /// Dies hier ist die Server-Version!
        /// </summary>
        /// <param name="msg">Die Nachricht</param>
        public virtual void MulticastMessage(SMCPMessage msg)
        {

            foreach (ClientInfo ci in ConnectedClients.Values)
            {
                if (ci.ClientID != msg.SenderID && ci.ClientID != this.LocalClient.ClientID)
                {
                    SendMessage(msg, ci);
                }

            }

        }
        #endregion
        #region Create Messages

        /// <summary>
        /// Erstellt eine SMCP-Nachricht und personalisiert diese für den 
        /// lokalen Computer
        /// </summary>
        /// <param name="data">Das Object, welches verschickt weren soll (wird vorher serialisiert)</param>
        /// <param name="multicast">Gibt an, ob diese Nachricht an alle Client versendet werden soll</param>
        /// <param name="action">Die Aktion, die diese Nachricht aufruft</param>
        /// <returns>Gibt die fertige Nachricht zurück</returns>
        public SMCPMessage CreateMessage(object data, bool multicast, SMCPAction action)
        {
            SMCPMessage msg = new SMCPMessage();
            msg.SenderID = LocalClient.ClientID;
            if (data != null)
                msg.Data = Serializer.ObjectToByteArray(data);
            else
                msg.Data = new byte[0];
            msg.Multicast = multicast;
            msg.ActionID = (byte)action;
            return msg;
        }

        /// <summary>
        /// Erstellt eine SMLP-Nachricht und personalisiert diese für den 
        /// lokalen Computer
        /// </summary>
        /// <param name="data">Die Daten als Objekt, werden mit dem Serialisierer serialisiert</param>
        /// <param name="localReceiver">Der LOCAL_RECEIVER (Siehe ScrumNetwork.Protocol.SMLPMessage)</param>
        /// <returns>Die Nachricht</returns>
        public SMLPMessage CreateMessage(object data, short localReceiver)
        {
            byte[] bytes = Serializer.ObjectToByteArray(data);
            return CreateMessage(bytes, localReceiver);
        }

        /// <summary>
        /// Erstellt eine SMLP-Nachricht und personalisiert diese für den 
        /// lokalen Computer
        /// </summary>
        /// <param name="data">Die Daten als Byte-Array, werden nicht weiter verarbeitet</param>
        /// <param name="localReceiver">Der LOCAL_RECEIVER (Siehe ScrumNetwork.Protocol.SMLPMessage)</param>
        /// <returns>Die Nachricht</returns>
        public SMLPMessage CreateMessage(byte[] data, short localReceiver)
        {
            SMLPMessage msg = new SMLPMessage();
            msg.SenderID = LocalClient.ClientID;
            msg.Data = data;
            msg.LocalReceiver = localReceiver;
            return msg;
        }
        #endregion
        #region Leave Call & Disposing
        /// <summary>
        /// Schließt den UDP-Client
        /// </summary>
        public virtual void Dispose()
        {
            udp_client.Close();
            OnConnectionClosed();
        }

        /// <summary>
        /// Schließt die aktuelle Verbindung/ den Server
        /// </summary>
        /// <returns>Der Erfolg der Aktion</returns>
        public abstract bool Exit();

        /// <summary>
        /// Setzt isClosing auf TRUE und startet somit den Verbindungsabbau
        /// </summary>
        public virtual void StartClosing()
        {
            IsClosing = true;
        }
        #endregion
        #region Logging
        /// <summary>
        /// Schreibt eine Nachricht in den Network-Log
        /// </summary>
        /// <param name="msg">Die Nachricht</param>
        protected void Log(string msg)
        {
            NetworkLog.WriteLine(this.LocalClient, msg);
        }
        #endregion
    }
}
