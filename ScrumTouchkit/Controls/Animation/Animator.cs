using ScrumGestures.Gestures;
using ScrumTouchkit.Controls.Content.Abstract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace ScrumTouchkit.Controls.Animation
{
    /// <summary>
    /// Stellt Animationen für Objekte auf der Oberfläche dar
    /// </summary>
    public static class Animator
    {
        /**
         * Standarddauer der verschiedenen Animationen
         **/
        public static TimeSpan movingDuration = TimeSpan.FromMilliseconds(500);
        public static TimeSpan resizeDuration = TimeSpan.FromMilliseconds(500);
        public static TimeSpan rotateDuration = TimeSpan.FromMilliseconds(500);
        public static TimeSpan fadeDuration = TimeSpan.FromMilliseconds(250);

        #region FadeIn, FadeOut
        public static void FadeOut(FrameworkElement ui, EventHandler onCompleted = null)
        {
            FadeOut(ui, fadeDuration, onCompleted);
            
        }
        public static void FadeOut(FrameworkElement ui, TimeSpan fadeDur, EventHandler onCompleted = null)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 1;
            da.To = 0;
            da.Duration = fadeDur;
            if (onCompleted != null)
                da.Completed += onCompleted;
            ui.BeginAnimation(FrameworkElement.OpacityProperty, da);

        }
        public static void FadeIn(FrameworkElement ui)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 1;
            da.Duration = fadeDuration;
            ui.BeginAnimation(FrameworkElement.OpacityProperty, da);
        }
        #endregion
        #region Rotate
        public static void Rotate(RotateTransform transform, double angle)
        {
            _rotate(transform, angle, rotateDuration);
        }
        private static void _rotate(RotateTransform transform, double angle, Duration dur)
        {
            DoubleAnimation f = new DoubleAnimation();
            f.From = transform.Angle;
            f.To = angle;
            f.Duration = dur;
            transform.BeginAnimation(RotateTransform.AngleProperty, f);
        }
        #endregion
        #region Resize
        public static void Resize(ScaleTransform transform, double factor)
        {
            _resize(transform, factor, resizeDuration);
        }
        public static void Resize(ScaleTransform transform, double factor, TimeSpan span)
        {
            _resize(transform, factor, span);
        }

        public static void ResizeTo(Shape ui, double w, double h, double x, double y)
        {
            _resize(ui, w, h, 0, 0, TimeSpan.FromMilliseconds(DefinedGestures.HOLD_LENGTH), true, x, y);
        }

        private static void _resize(ScaleTransform transform, double factor, Duration dur)
        {
            DoubleAnimation f = new DoubleAnimation();
            f.From = transform.ScaleX;
            f.To = factor;
            f.Duration = dur;
            transform.BeginAnimation(ScaleTransform.ScaleXProperty, f);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, f);
        }

        private static void _resize(FrameworkElement element, double width, double height,
                                    double start_width, double start_height, Duration dur, bool respect_loc = false,
                                        double loc_x = 0, double loc_y = 0)
        {
            DoubleAnimation dw = new DoubleAnimation();
            dw.From = start_width;
            dw.To = width;
            dw.Duration = dur;
            element.BeginAnimation(FrameworkElement.WidthProperty, dw);

            DoubleAnimation dh = new DoubleAnimation();
            dh.From = start_height;
            dh.To = height;
            dh.Duration = dur;
            element.BeginAnimation(FrameworkElement.HeightProperty, dh);

            if (respect_loc)
            {
                _move(element,
                    loc_x - width / 2,
                    loc_y - height / 2,
                    loc_x,
                    loc_y,
                    dur);
            }
        }
        #endregion
        #region Move
        public static void Move(FrameworkElement element,
            double x, double y)
        {
            _move(
                element, x, y,
                (double)element.GetValue(Canvas.LeftProperty),
                (double)element.GetValue(Canvas.TopProperty),
                movingDuration);

        }
        private static void _move(FrameworkElement element, double x, double y, double start_x, double start_y, Duration dur)
        {
            DoubleAnimation dx = new DoubleAnimation();
            dx.From = start_x;
            dx.To = x;
            dx.DecelerationRatio = 0.3;
            dx.Duration = dur;

            DoubleAnimation dy = new DoubleAnimation();
            dy.From = start_y;
            dy.To = y;
            dy.DecelerationRatio = 0.3;
            dy.Duration = dur;

            element.BeginAnimation(Canvas.LeftProperty, dx);
            element.BeginAnimation(Canvas.TopProperty, dy);
        }
        #endregion
        #region BackgroundColor
        public static void AnimateColorFlashing(BaseUI content, Color from, Color to, Duration dur)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = from;
            ColorAnimation ca = new ColorAnimation(to, dur);

            ca.AutoReverse = true;
            ca.RepeatBehavior = RepeatBehavior.Forever;
            content.BackgroundShape.Fill = brush;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, ca);

        }
        public static void AnimateColor(BaseUI content, Color from, Color to, Duration dur)
        {
                LinearGradientBrush brush = new LinearGradientBrush();

                GradientStop stop2 = new GradientStop(from, 0);
                GradientStop stop1 = new GradientStop(to, 0);

                try
                {

                    content.UnregisterName("stop1");
                    content.UnregisterName("stop2");
                }
                catch { }

                content.RegisterName("stop1", stop1);
                content.RegisterName("stop2", stop2);

                brush.GradientStops.Add(stop1);
                brush.GradientStops.Add(stop2);

                content.BackgroundShape.Fill = brush;

                DoubleAnimation offsetAnimation = new DoubleAnimation();
                offsetAnimation.From = 0.0;
                offsetAnimation.To = 1.0;
                offsetAnimation.Duration = dur;

                DoubleAnimation offsetAnimation2 = new DoubleAnimation();
                offsetAnimation2.From = 0.1;
                offsetAnimation2.To = 1.0;
                offsetAnimation2.Duration = dur;


                Storyboard st = new Storyboard();
                Storyboard.SetTargetName(offsetAnimation, "stop1");
                Storyboard.SetTargetProperty(offsetAnimation,
                    new PropertyPath(GradientStop.OffsetProperty));
                Storyboard.SetTargetName(offsetAnimation2, "stop2");
                Storyboard.SetTargetProperty(offsetAnimation2,
                    new PropertyPath(GradientStop.OffsetProperty));


                st.Children.Add(offsetAnimation);
                st.Children.Add(offsetAnimation2);
                st.Begin(content);
        }
        #endregion


    }
}
