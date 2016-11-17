using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ScrumNetwork.Protocol
{
    /// <summary>
    /// Stellt eine Scrum Meeting Live Protokoll Message dar
    /// Funktionaliäten sind senden, empfangen
    /// Der Header ist 4 Byte groß
    /// 2 Byte SENDER_ID
    /// 2 Byte LOCAL_RECEIVER
    /// Optionale Daten könnnen angehängt werden
    /// </summary>
   public class SMLPMessage : EventArgs
    {
       /// <summary>
       /// LOCAL_RECEIVER-ID für Audio samples
       /// </summary>
       public const short AUDIO = 0;
       
       private short local_receiver = SMLPMessage.AUDIO; //0 ist audio, > 0 ist ist ein Scrum Item (User Story, Epic)
       private short sender_id = -1;
       private byte[] _data, header;
       /// <summary>
       /// Mit dem Local Receiver wird das Objekt beschrieben, welches das SMLP-Packet verarbeitet
       /// Falls dieser SMLPMessage.Audio entspricht, handelt es sich um Audiodaten
       /// Ansonsten sind es Updates für ein Textfeld eines Objektes (Epic, User Story)
       /// </summary>
       public short LocalReceiver
       {
           get { return local_receiver; }
           set { local_receiver = value; }
       }
       /// <summary>
       /// Der Host, der die Nachricht verschickte
       /// </summary>
       public short SenderID
       {
           get { return sender_id; }
           set { sender_id = value; }
       }
       /// <summary>
       /// Die Daten der Nachricht
       /// </summary>
       public byte[] Data
       {
           get { return _data; }
           set { _data = value; }
       }

       /// <summary>
       /// Erstellt eine leere SMLP-Message,
       /// diese muss zunächst gefüllt werden
       /// </summary>
       public SMLPMessage()
       {
       }

       /// <summary>
       /// Initialisiert eine fertige SMLP-Message, diese muss nicht mehr gefüllt werden
       /// sonder ist dies bereits.
       /// </summary>
       /// <param name="sender_id">Die ID des Senders</param>
       /// <param name="local_receiver">Die ID des LOCAL_RECEIVERs</param>
       /// <param name="data">Die Daten der Nachricht</param>
       public SMLPMessage(short sender_id, byte local_receiver, byte[] data)
       {
           this.sender_id = sender_id;
           this.local_receiver = local_receiver;
           this._data = data;
       }

       /// <summary>
       /// Erstellt einen Header für die Nachricht
       /// </summary>
       private void marshal()
       {
           List<byte> header_list = new List<byte>();
           header_list.AddRange(BitConverter.GetBytes(sender_id));
           header_list.AddRange(BitConverter.GetBytes(local_receiver));
           header = header_list.ToArray();
       }

       /// <summary>
       /// Verschickt die Nachricht über eine UDP-Verbindung
       /// </summary>
       /// <param name="client">Der UDPClient, über welchen die Nachricht verschickt werden soll</param>
       /// <param name="endpoint">Das (IP, PORT)-Tupel, welches die Nachricht empfangen soll (steht in der Regel in der passenden ClientInfo)</param>
       /// <returns>TRUE bei Erfolg, FALSE ansonsten</returns>
       public bool WriteMessage(UdpClient client, IPEndPoint endpoint)
       {

           try{
               //Header erstellen
               marshal();
               //Nachricht finalisieren
               List<byte> msg = new List<byte>();
               msg.AddRange(header);
               msg.AddRange(_data);
               //Nachricht senden
               client.Send(msg.ToArray(), msg.Count, endpoint);
               return true;
           }
           catch
           {
               return false;
           }
       }
       /// <summary>
       /// Wartet auf die nächste Nachricht, die an einen UDP-Client gesendet wird 
       /// </summary>
       /// <param name="client">Der UDP-Client, der auf eine Nachricht wartet</param>
       /// <returns>Eine neue SMLP-Message</returns>
       public static SMLPMessage ReadNextMessage(UdpClient client)
       {
           SMLPMessage msg = new SMLPMessage();
           IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
           byte[] data = client.Receive(ref ep);
           msg.SenderID = BitConverter.ToInt16(data, 0);
           msg.LocalReceiver = BitConverter.ToInt16(data, 2);

           msg.Data = new byte[data.Length - 4];
           Array.Copy(data, 4, msg.Data, 0, msg.Data.Length);
           return msg;
       }


    }
}
