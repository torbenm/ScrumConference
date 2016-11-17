using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ScrumNetwork.Protocol
{
    /// <summary>
    /// Speichert SMCP-Messages
    /// Das Protokoll hat einen 8 Byte Header:
    /// 4 Bytes für die LENGTH
    /// 2 Bytes für SENDER_ID
    /// 2 Bytes für MULTICAST_FLAG
    /// 2 BYtes für ACTION_ID
    /// Anschließend kommt ein optionaler Datenteil
    /// </summary>
    public class SMCPMessage : EventArgs
    {
        private bool multicast_flag = false;
        private int data_length = 0;
        private short sender_id = 0;
        private byte action_id = 0;

        private byte[] _data;
        private byte[] header;

        /// <summary>
        /// Wenn Multicast TRUE ist, dann wird die Nachricht
        /// vom Server automatisch an alle anderen Clients weitergeleitet
        /// </summary>
        public bool Multicast
        {
          get { return multicast_flag; }
          set { multicast_flag = value; }
        }
        /// <summary>
        /// Die Länge des DATA-Teils
        /// </summary>
        public int DataLength
        {
          get { return data_length; }
        }
        /// <summary>
        /// Die ID des Senders (des entsprechenden Clients)
        /// </summary>
        public short SenderID
        {
            get { return sender_id; }
            set { sender_id = value; }
        }
        /// <summary>
        /// Die Action, die durch diese Nachricht aufgerufen wird (siehe SMCPAction)
        /// </summary>
        /// <see cref="ScrumNetwork.Protocol.SMCPAction"/>
        public byte ActionID
        {
            get { return action_id; }
            set { 
                action_id = value;
            }
        }
        /// <summary>
        /// Die Daten der Nachricht
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; data_length = _data.Length;  }
        }


       /// <summary>
       /// Erstellt eine leere Nachricht,
       /// diese muss zunächst gefüllt werden, um versendet werden zu können
       /// </summary>
        public SMCPMessage()
        {
        }

        /// <summary>
        /// Erstellt eine fertige SMCP-Nachricht
        /// </summary>
        /// <see cref="ScrumNetwork.Protocol.SMCPAction"/>
        /// <param name="data">Die Daten</param>
        /// <param name="action_id">Die Action, die aufgerufen wird (Siehe SMCPAction)</param>
        /// <param name="sender_id">Der Sender der NAchricht</param>
        /// <param name="multicast_flag">TRUE wenn die Nachricht an alle Clients weitergeleitet werden soll</param>
        public  SMCPMessage(byte[] data, byte action_id, short sender_id, bool multicast_flag)
        {
            this.sender_id = sender_id;
            this.action_id = action_id;
            this.Data = data;
            this.multicast_flag = multicast_flag;
        }

        /// <summary>
        /// Erstellt den Header für die Nachricht
        /// </summary>
        private void marshal()
        {
            List<byte> header_list = new List<byte>();
            header_list.AddRange(BitConverter.GetBytes(data_length));
            header_list.AddRange(BitConverter.GetBytes(sender_id));
            header_list.AddRange(BitConverter.GetBytes(multicast_flag));
            header_list.Add(action_id);
            header = header_list.ToArray();
        }

        /// <summary>
        /// Verschickt die Nachricht über den angegebenen NetworkStream
        /// </summary>
        /// <param name="stream">NetworkStream, über welchen die Nachricht verschickt werden soll</param>
        /// <returns>TRUE bei Erfolg, ansonsten FALSE</returns>
        public bool WriteMessage(NetworkStream stream)
        {
            try
            {
                //Create header
                marshal();
                //Write header
                stream.Write(header, 0, 8);
                //Append data
                if(data_length > 0)
                    stream.Write(_data, 0, _data.Length);
                //Write to stream
                stream.Flush();
                return true;
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Wartet auf die nächste SMCP-Message, die über den angegebenen NetworkStream 
        /// </summary>
        /// <param name="stream">Der Stream, über welchen die Nachrichten ankommen</param>
        /// <returns>Die erhaltene SMCP-Message</returns>
        public static SMCPMessage ReadNextMessage(NetworkStream stream)
        {
            SMCPMessage msg = new SMCPMessage();

            byte[] header = new byte[8];
            //read header first
            if (stream.Read(header, 0, 8) < 8)
                return null;
            
            //unmarshal header information
            int length = BitConverter.ToInt32(header, 0);
            msg.SenderID = BitConverter.ToInt16(header, 4);
            msg.Multicast = BitConverter.ToBoolean(header, 6);
            msg.ActionID = header[7];

            
            //Wait for and read DATA segment
            byte[] message = new byte[length];
            int amountRead = 0;
            while (length > 0)
            {
                amountRead = stream.Read(message, amountRead, length);
                length -= amountRead;
            }
            msg.Data = message;
            return msg;
            
        }

        /// <summary>
        /// Gibt die SMCP-Message als String wider
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String value = "[ACTION: " + this.ActionID + ", MF: " + this.Multicast + ", Sender: " + this.SenderID + "] " +
                    new UnicodeEncoding().GetString(this.Data);
            return value;
        }

        /// <summary>
        /// Wandelt die Daten einer SMCP-Message zu einem bestimmten Typ um
        /// </summary>
        /// <typeparam name="T">Der gewünschte Typ</typeparam>
        /// <param name="ep">Der lokale ScrumMeetingEndPoint</param>
        /// <returns>Die Daten</returns>
        public T GetData<T>(ScrumMeetingEndPoint ep)
        {
            return ep.Serializer.ConvertToObject<T>(this.Data);
        }
    }
}
