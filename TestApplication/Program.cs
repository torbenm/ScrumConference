using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using ScrumTouchkit.Data;
using System.Windows;
using ScrumNetwork;
using ScrumTouchkit.Controls.Network;


namespace TestApplication
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ScrumTouchkit.Utilities.Serializer.JSONSerializer json = new ScrumTouchkit.Utilities.Serializer.JSONSerializer();
            TextBoxState tbst = new TextBoxState
             {
                 Text = "Beispieltext wird gerade verä",
                 TextBoxType = 0,
                 SelectionStart = 10,
                 SelectionLength = 0,
                 HorizontalOffset = 15,
                 VerticalOffset = 0
             };
            Console.WriteLine(json.ObjectToString(tbst));

                    System.Windows.Forms.Clipboard.SetText(json.ObjectToString(tbst));
              
            Console.Read();
        }

    }
}
