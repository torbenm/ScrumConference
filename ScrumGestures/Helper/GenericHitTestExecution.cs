using System;
using System.Windows;
using System.Windows.Media;

namespace ScrumGestures.Helper
{
    /// <summary>
    /// Führt einen HitTest aus. 
    /// </summary>
    /// <typeparam name="T">Das Interface oder die Klasse, die von allen Objekten implementiert wird, die als unterste Ebene eines zusammenhängenden Steuerelementes
    /// dienen</typeparam>
    public class GenericHitTestExecution<T>
    {
        private UIElement hitTestResult;
        private bool foundFirstPossible = false;

        /// <summary>
        /// Führt einen HitTest an dem angegebenen Punkt aus
        /// </summary>
        /// <param name="reference">Das Visual, auf dem der HitTest ausgeführt wird</param>
        /// <param name="pt">Der Punkt, an welchem der HitTest ausgeführt wird</param>
        /// <returns>Das gefundene UIElement oder NULL</returns>
        public UIElement ExecuteHitTest(Visual reference, Point pt)
        {
            return (UIElement)reference.Dispatcher.Invoke(new Func<UIElement>(() =>
                {
            foundFirstPossible = false;
            hitTestResult = null;
            VisualTreeHelper.HitTest(reference,
            ExecuteHitTestFilter, result => HitTestResultBehavior.Continue, new PointHitTestParameters(pt));
            return hitTestResult;
                }));
        }

        /// <summary>
        /// enn die Klasse von e von Klasse T erbt, dann haben wir das unterste Objekt eines
        /// zusammenhängenden Steuerlementes gefunden. Wir gucken zwar dort noch ein bisschen weiter,
        /// ob wir etwas tiefer (bzw höher auf der Oberfläche) ein passendes Objekt finden, es darf sich aber
        /// nur in diesem zusammenhängenden Steuerelement befinden.
        /// 
        /// Dies muss gemacht werden, da bei komplexeren Steuerelementen sonst Fehler auftreten können.
        /// So lässt sich beispielsweise ein Dialog vorstellen, in dem es einen Button gibt. Das Dialogfenster selbst
        /// erbt z.B. von einer Klasse IHasGestures. Mit dieser Klasse wird die GenericHitTestExecution initialisiert,
        /// die Geste wird aber auf dem Button erkannt und ausgeführt.
        /// Wenn wir nicht nach einer "untersten" Klasse die von IHasGestures erbt suchen würden, würden wir zwar irgendwann den
        /// Button finden, aber eventuell an diesem vorbei laufen und gar in ein anderes Dialogfenster gelangen, das eigentlich
        /// unter dem gewünschten liegt. Ursache dafür ist der Aufbau des Visual Trees in WPF.
        /// Daher wird in diesem Fall zunächst nach einem Dialogfenster gesucht und anschließend nach dem obersten Element mit einer Geste.
        /// Dies wird solange getan, bis entweder ein weiteres Dialogfesnter gefunden wurde oder es keine weiteren Elemente auf der Oberfläche
        /// an dem entsprechendem Punkt mehr gibt.
        /// </summary>
        /// <param name="potentialHitTestTarget">Potentielles Objekt, das gefunden wurde</param>
        /// <returns></returns>
        private HitTestFilterBehavior ExecuteHitTestFilter(DependencyObject potentialHitTestTarget)
        {
            UIElement e = potentialHitTestTarget as UIElement;
            Console.WriteLine(e);
            if (e != null)
            {
                //Wenn die Klasse von e von Klasse T erbt, dann haben wir das unterste Objekt eines
                //zusammenhängenden Steuerlementes gefunden. Wir gucken zwar dort noch ein bisschen weiter,
                //ob wir etwas tiefer (bzw höher auf der Oberfläche) ein passendes Objekt finden, es darf sich aber
                //nur in diesem zusammenhängenden Steuerelement befinden.
                if (typeof(T).IsAssignableFrom(e.GetType()))
                {
                    //Wir haben bereits ein unterstes Objekt eines zusammenhängendes
                    //Steuerelement gefunden! Daher jetzt aufhören
                    if (foundFirstPossible)
                    {
                        return HitTestFilterBehavior.Stop;
                    }
                    else
                    {
                        
                        foundFirstPossible = true;
                    }
                }
                if (IsValid(e))
                {

                    hitTestResult = e;
                }
            }
            return HitTestFilterBehavior.Continue;
        }

        /// <summary>
        /// Validiert, ob das gefundene UIElement den Anforderungen, in diesem Fall 
        /// eine Erbschaft des Typs, nachweist.
        /// </summary>
        /// <param name="element">Das UIElement</param>
        /// <returns>TRUE wenn die Anforderungen erfüllt sind</returns>
        protected virtual bool IsValid(UIElement element)
        {
            return typeof(T).IsAssignableFrom(element.GetType());
        }
        
    }
}
