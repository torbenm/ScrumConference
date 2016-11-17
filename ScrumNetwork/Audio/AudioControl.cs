using NAudio.Wave;
using System;
using ScrumTouchkit.Utilities;

namespace ScrumNetwork.Audio
{
    /// <summary>
    /// Kontrolliert das Audio, nimmt also Klänge auf und gibt diese wieder.
    /// </summary>
    public class AudioControl : IDisposable
    {
        public event EventHandler<AudioEventArgs> AudioCaptured;

        private WaveInEvent waveIn;
        private WaveOutEvent waveOut;
        private BufferedWaveProvider outputStream;
        private bool running;

        public bool Muted
        {
            get;
            private set;
        }

        public AudioControl()
        {
            Muted = false;
            //Neues WaveInEvent - Nimmt Audio auf
            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = Settings.Default.RECORD_DEVICE;
            waveIn.WaveFormat = new WaveFormat(Settings.Default.SAMPLE_RATE, Settings.Default.CHANNELS);
            waveIn.DataAvailable += waveIn_DataAvailable;

            //Neues WaveOutEvent - Gibt Audio wieder
            waveOut = new WaveOutEvent();
            waveOut.DeviceNumber = Settings.Default.PLAYBACK_DEVICE;
            //BufferedWaveProvider um erhaltenes Audio wiederzugeben
            outputStream = new BufferedWaveProvider(new WaveFormat(Settings.Default.SAMPLE_RATE, Settings.Default.CHANNELS));
            waveOut.Init(outputStream);

        }
        /// <summary>
        /// Startet Aufnahme und wiedergabe
        /// </summary>
        public void StartAll()
        {
            StartRecording();
            StartPlaying();
            running = true;
        }

        /// <summary>
        /// Stopt Aufnahme und Wiedergabe
        /// </summary>
        public void StopAll()
        {
            StopRecording();
            StopPlaying();
            running = false;
        }
        /// <summary>
        /// Startet Aufnahme
        /// </summary>
        public void StartRecording()
        {      
            if(!Muted)
            waveIn.StartRecording();
        }
        /// <summary>
        /// Startet Wiedergabe
        /// </summary>
        public void StartPlaying()
        {
            waveOut.Play();
        }
        /// <summary>
        /// Stoppt Wiedergabe
        /// </summary>
        public void StopPlaying()
        {
            waveOut.Stop();
        }
        /// <summary>
        /// Stoppt Aufnahme
        /// </summary>
        public void StopRecording()
        {
            waveIn.StopRecording();
        }

        /// <summary>
        /// Schaltet Aufnahme ein bzw aus
        /// </summary>
        public void ToggleMute()
        {
            if (!Muted)
            {
                Muted = true;
                if (running)
                    StopRecording();
            }
            else
            {
                Muted = false;
                if (running)
                    StartRecording();
            }
        }

        public virtual void Dispose()
        {
            waveOut.Dispose();
            waveIn.Dispose();
        }
        /// <summary>
        /// Speichert Audiodaten im Buffer
        /// </summary>
        /// <param name="data">Das Audiosampel</param>
        public void BufferAudio(byte[] data)
        {
            outputStream.AddSamples(data, 0, data.Length);
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            double dec = GetDecibel(e.Buffer);
            //Aufgenommes Audio muss laut genug sein
            if (dec > Settings.Default.MIN_DECIBEL)
            {
                AudioCaptured(this, new AudioEventArgs(e.Buffer));
            }

        }

        /// <summary>
        /// Berechnet die Anzahl Dezibel für das aktuelle Audiosample 
        /// </summary>
        /// <param name="buffer">Das Dezibel</param>
        /// <returns>Dezibel als double</returns>
        public static double GetDecibel(byte[] buffer)
        {
            double peak = 0;
            for (int i = 0; i < buffer.Length; i = i + 2)
            {
                short current = BitConverter.ToInt16(buffer, i);
                if (current > peak)
                    peak = current;
                else if (current < -peak)
                    peak = -current;
            }

            return 20 * Math.Log10(peak / (short.MaxValue+1));
        }
    }
}
