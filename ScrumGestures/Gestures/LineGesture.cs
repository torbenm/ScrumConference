namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Erkennt eine Linie, die mindestens die Hälfte der Oberfläche lang ist
    /// </summary>
    public class LineGesture : GestureBase
    {

        protected override bool InternalValidation(System.Windows.UIElement ui, TouchGroup points)
        {
            //Maximal eine Berührungs ist für diese Geste zulässig!
            if (points.MaxPoints == 1 && points.Count == 1)
            {
                TouchPoint tp = points[0];

                //Mindestens vier Positionen müssen bisher gespeichert sein,
                //außerdem muss die Berührung gerade entfernt worden sein
                if (tp.PathLength > 4 && tp.Mode == TouchPoint.TouchMode.UP)
                {
                    //Länge der Berührung als Summe der Distanzen aller Zwischenpunkte
                    float total_dist = tp.Distance;

                    //Distanz zwischen Start und Endpunkt  der Berührung
                    float start_end = tp.StartPoint.GetDistance(tp.CurrentPoint);
                    //Mindestens 50% der Oberflächengröße lang
                    if (tp.DistanceRelative > 0.5)
                    {
                        //10% Abweichung einer perfekten Linie (berechnet in start_end) 
                        //ist zulässig, dadurch werden auch Linien mit einer leichten
                        //krümmung erkannt
                        return (start_end / total_dist) > 0.9;
                    }
                }
            }
            return false;
        }
    }
}
