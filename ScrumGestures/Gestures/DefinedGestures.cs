namespace ScrumGestures.Gestures
{
    /// <summary>
    /// Statische Auflistung aller definierten Gesten, damit diese einfach wiederverwendet werden können.
    /// </summary>
    public static class DefinedGestures
    {
        /// <summary>
        /// Die Berührungsdauer bei einem TAP darf höchstens 200 ms dauern.
        /// </summary>
        public const int TAP_LENGTH = 200;
        /// <summary>
        /// Zwischen den Berührungen bei einem DoubleTap liegt eine Pause von maximal 500 ms
        /// </summary>
        public const int DOUBLE_TAP_BREAK = 500;
        /// <summary>
        /// Die Berührungsdauer bei einer HOLD-Geste dauert mindestens 1000 ms.
        /// </summary>
        public const int HOLD_LENGTH = 1000;

        public static FingerMovement DragAndDrop = new FingerMovement(1);
        public static FingerMovement Rotate = new FingerMovement(2);
        public static FingerMovement Resize = new FingerMovement(2);
        public static FingerMovement ThreeFingerMove = new FingerMovement(3);
        public static FingerMovement TenFingerMovement = new FingerMovement(10);
        public static LineGesture Line = new LineGesture();
        public static FingerHold Tap = new FingerHold(1, TAP_LENGTH, FingerHold.ComparisonMode.MAX);
        public static FingerHold Hold = new FingerHold(1, HOLD_LENGTH, FingerHold.ComparisonMode.MIN);
        public static FingerHold TwoFingerTap = new FingerHold(2, TAP_LENGTH, FingerHold.ComparisonMode.MAX);
        public static FingerHold TwoFingerHold = new FingerHold(2, HOLD_LENGTH, FingerHold.ComparisonMode.MIN);
        public static DoubleTap DoubleTap = new DoubleTap(1);
        public static DoubleTap TwoFingerDoubleTap = new DoubleTap(2);
        public static FingerChanged FingerCountChanged = new FingerChanged();
        public static SingleTouchChanged TouchDown = new SingleTouchChanged(TouchPoint.TouchMode.DOWN);
        public static SingleTouchChanged TouchUp = new SingleTouchChanged(TouchPoint.TouchMode.UP);
      
    }
}
