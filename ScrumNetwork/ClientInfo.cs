using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;

namespace ScrumNetwork
{
    /// <summary>
    /// Repräsentiert einen Client bzw End-Point
    /// Enthält alle relevanten Informationen zu diesem
    /// </summary>
    public class ClientInfo : EventArgs
    {
        #region vars, getter, setter
        private IPAddress _ipadress;

        /// <summary>
        /// Name des Clients
        /// </summary>
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Die ID des Clients
        /// </summary>
        public short ClientID
        {
            get;
            set;
        }

        /// <summary>
        /// Die IP-Adresse des Clients
        /// </summary>
        public String IPAddress
        {
            get { return _ipadress.ToString(); }
            set { _ipadress = System.Net.IPAddress.Parse(value); }
        }

        /// <summary>
        /// Gibt an, ob es sich um einen Server handelt
        /// </summary>
        public bool IsServer
        {
            get;
            set;
        }

        /// <summary>
        /// Gibt den UDP-Port an
        /// </summary>
        public int UdpPort
        {
            get;
            set;
        }

        /// <summary>
        /// Git den TCP-Port an
        /// </summary>
        public int TcpPort
        {
            get;
            set;
        }


        /// <summary>
        /// TCP-Client für die Verbindung
        /// </summary>
        [JsonIgnore]
        public TcpClient TcpClient
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Erstellt eine leere ClientInfo.
        /// Sollte vor der weiteren Verwendung mit Daten gefüllt werden
        /// </summary>
        public ClientInfo()
        {

        }
        /// <summary>
        /// Erstellt eine leere ClientInfo, nur mit einem TCP-Client versehen
        /// </summary>
        /// <param name="client">Der TCP-Client</param>
        public ClientInfo(TcpClient client)
        {
            this.TcpClient = client;

        }
        /// <summary>
        /// Erstellt eine gefüllte ClientInfo, nur der TCP-Client ist nicht initialisiert
        /// </summary>
        /// <param name="name">Der Name des Clients</param>
        /// <param name="ip">Die IP-Adresse als String des Clients</param>
        /// <param name="server">TRUE, wenn es sich um einen Server handelt</param>
        /// <param name="udp_port">Der Port, welchen der Client für UDP benutzt</param>
        /// <param name="tcp_port">Der Port, welchen der Client für TCP benutzt. Entspricht, falls es sich nicht um einen Server handelt, meistens dem TCP-Port des Servers.</param>
        public ClientInfo(String name, String ip, bool server, int udp_port, int tcp_port)
        {
            Name = name;
            IPAddress = ip;
            IsServer = server;
            UdpPort = udp_port;
            TcpPort = tcp_port;
        }

        /// <summary>
        /// Erstellt eine gefüllte ClientInfo, nur der TCP-Client ist nicht initialisiert. Für die IP-Adresse wird des ausführenden Computers benutzt.
        /// </summary>
        /// <param name="name">Der Name des Clients</param>
        /// <param name="server">TRUE, wenn es sich um einen Server handelt</param>
        /// <param name="udp_port">Der Port, welchen der Client für UDP benutzt</param>
        /// <param name="tcp_port">Der Port, welchen der Client für TCP benutzt. Entspricht, falls es sich nicht um einen Server handelt, meistens dem TCP-Port des Servers.</param>
        public ClientInfo(String name, bool server, int udp_port, int tcp_port)
            : this(name, GetLocalIPAddress(), server, udp_port, tcp_port)
        {
        }
        #endregion

        #region TCP & UDP
        /// <summary>
        /// Erstellt ein (IP, Port)-Tupel für UDP
        /// </summary>
        /// <returns>Das (IP, Port)-Tupel</returns>
        public IPEndPoint GetUDPEndPoint()
        {
            return new IPEndPoint(_ipadress, UdpPort);
        }


        /// <summary>
        /// Wenn IP-Adresse und TCP-Port angegeben sind, kann mit dieser Funktion der TCP-Client gestartet werden
        /// </summary>
        public void StartTCPClient()
        {
            this.TcpClient = new TcpClient();
            this.TcpClient.Connect(new IPEndPoint(this._ipadress, this.TcpPort));
        }
        #endregion

        #region static
        /// <summary>
        /// Ermittelt die  IP-Adresse dieses Computers
        /// </summary>
        /// <returns></returns>
        public static String GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
        #endregion


    }
}
