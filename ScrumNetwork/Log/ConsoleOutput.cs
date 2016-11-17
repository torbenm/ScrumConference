using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumNetwork.Log
{
    /// <summary>
    /// Implementiert das IOutput-Interface zur Ausgabe auf der Konsole
    /// </summary>
    public class ConsoleOutput : IOutput
    {

        public void StartLogging()
        {
            //nothing
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public void EndLogging()
        {
           //nothing
        }
    }
}
