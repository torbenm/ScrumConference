using System.Collections.Generic;
using System.Windows;

namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Eine DoubleTap-Geste: Mit einer bestimmten Anzahl von Fingern wird kurz hintereinander dasselbe Objekt berührt.
    /// Erweitert die GestureBase-Klasse.
    /// </summary>
    public class DoubleTap : GestureBase
    {

        /// <summary>
        /// Anzahl der Finger für den DoubleTap
        /// </summary>
        public int FingerCount 
        { 
            get; 
            set; 
        }


        private Dictionary<UIElement, long> currentElements = new Dictionary<UIElement, long>();
        private FingerHold tapValidator;

        /// <summary>
        /// Initialisiert eine neue DoubleTap-Geste.
        /// </summary>
        /// <param name="fingerCount">Die Anzahl Finger, die für diese Geste verwendet werden sollen.</param>
        public DoubleTap(int fingerCount)
        {
            FingerCount = fingerCount;
            tapValidator = new FingerHold(fingerCount, DefinedGestures.TAP_LENGTH, FingerHold.ComparisonMode.MAX);
        }



        protected override bool InternalValidation(UIElement ui, TouchGroup points)
        {
            bool valid = false;
            bool create = true;
            //Validiere, ob es eine Tap-Geste ist.
                if(tapValidator.Validate(ui, points))
                {
                    lock (currentElements)
                    {
                        //Suche nach dem entsprechendem Objekt auf der Oberfläche.
                        //Falls dieses bereits gespeichert wurde, wird überprüft
                        //ob die Pause zwischen diesem und dem vorhergehendem "Tap" kurz genug war.
                        //Anschließend wird die Zeit mit dem entsprechenden Objekt für diesen Tap gespeichert.
                        if (currentElements.ContainsKey(ui))
                        {
                            long lastTap = currentElements[ui];
                            long thisTap = points[0].CurrentTimeMS;
                            if (lastTap != thisTap)
                            {
                                if (thisTap - lastTap <= DefinedGestures.DOUBLE_TAP_BREAK)
                                {
                                    valid = true;
                                    create = false;
                                }
                                else
                                    currentElements.Remove(ui);
                            }
                            else
                            {
                                create = false;
                            }
                        }
                        
                            if (create)
                            {
                                currentElements.Add(ui, points[0].CurrentTimeMS);
                            }
                        
                    }
                }
            return valid;
        }
    }
}
