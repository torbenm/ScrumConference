using ScrumGestures;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ScrumTouchkit.Threading;

namespace ScrumTouchkit.Controls.Feedback
{
    public class TouchFeedback
    {

        ScrumSurface Surface;

        Timer _uiUpdateTimer;
        List<FeedbackBubble> feedback = new List<FeedbackBubble>();

        public void Init(ScrumSurface surface)
        {
            this.Surface = surface;

            // Start auto update 
            TimerCallback callback = new TimerCallback(UpdateUI);
            _uiUpdateTimer = new Timer(callback, null, 100, 100);

        }


        public void CaptureTouch(TouchPoint tp)
        {
            Surface.Invoke(() =>
            {
                    FeedbackBubble po = new FeedbackBubble(tp.CurrentPoint.X, tp.CurrentPoint.Y);
                    Surface.Children.Add(po);
                    feedback.Add(po);
            });
        }


        private void UpdateUI(object state)
        {
            Surface.Invoke(() =>
            {
                List<FeedbackBubble> itemsToRemove = new List<FeedbackBubble>();
                foreach (var po in feedback)
                {
                    if (po.Frame > 4)
                    {
                        itemsToRemove.Add(po);
                    }
                    else
                    {
                        po.UpdateUI();
                    }
                }
                foreach (var po in itemsToRemove)
                {
                    Surface.Children.Remove(po);
                    feedback.Remove(po);
                }
            });
        }

        class FeedbackBubble: Grid
        {
            public int Frame 
            { 
                    get; 
                    private set; 
            }

            int sizeDecayRate = 2;
            float opacityDecayRate = 0.1f;

            public FeedbackBubble(double x, double y)
            {
                Frame = 0;

                this.SetValue(Canvas.TopProperty, y);
                this.SetValue(Canvas.LeftProperty, x);
                this.Height = 20;
                this.Width = 20;

                Ellipse e = new Ellipse();
                e.Fill = new SolidColorBrush(Colors.LightGray);
                e.Opacity = 0.6;
                this.Children.Add(e);
            }

            public void UpdateUI()
            {
                if (!(this.Height < sizeDecayRate || Width < sizeDecayRate || Opacity < opacityDecayRate))
                {
                    this.Frame++;
                    this.Height -= sizeDecayRate;
                    this.Width -= sizeDecayRate;
                    this.Opacity -= opacityDecayRate;
                }
            }
        }
    }
}
