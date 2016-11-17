using System;

namespace ScrumNetwork
{
    /// <summary>
    /// Stellt eine Klasse zum Loggen von Nachrichten dar
    /// </summary>
    public static class NetworkLog
    {
        private static Log.IOutput output = new Log.ConsoleOutput();

        private static bool _init = false;

        /// <summary>
        /// Schreibt eine Nachricht
        /// </summary>
        /// <param name="client">Der Client, der als Ursache dieser Nachricht gilt</param>
        /// <param name="text">Der Nachrichtentext</param>
        public static void Write(ClientInfo client, string text)
        {
            if (!_init)
                output.StartLogging();
            string msg = prep(client, text);
            output.Write(msg);
        }

        /// <summary>
        /// Schreibt eine Nachricht
        /// </summary>
        /// <param name="client">Der Client, der als Ursache dieser Nachricht gilt</param>
        /// <param name="text">Der Nachrichtentext</param>
        public static void WriteLine(ClientInfo client, string text)
        {
            if (!_init)
                output.StartLogging();
            string msg = prep(client, text);
            output.WriteLine(msg);
        }

        /// <summary>
        /// Bereitet die neue Nachricht vor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string prep(ClientInfo client, string text)
        {
            return "[" + client.Name + ", " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] " + text;
        }
    }
}
