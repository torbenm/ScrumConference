using System;

namespace ScrumNetwork.Audio
{
    /// <summary>
    /// Wenn neue Audiodaten vorliegen, wird dieses EventArg versendet
    /// </summary>
    public class AudioEventArgs : EventArgs
    {
        public byte[] Data
        {
            get;
            set;
        }
        public AudioEventArgs(byte[] data)
        {
            this.Data = data;
        }
    }
}
