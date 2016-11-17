using System.Windows;

namespace ScrumGestures.Helper
{
    /// <summary>
    /// HitTestExecution für Steuerelemente, die Gesten besitzen
    /// </summary>
    public class HitTestExecution : GenericHitTestExecution<IHasGestures>
    {
        private GestureManager manager;

        public HitTestExecution(GestureManager mngr)
        {
            manager = mngr;
        }

        protected override bool IsValid(UIElement element)
        {
            return manager.HasGesture(element);
        }
        

    }
}
