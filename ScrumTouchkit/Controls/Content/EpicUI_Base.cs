using ScrumTouchkit.Controls.Content.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScrumTouchkit.Controls.Content
{

    /// <summary>
    /// Hintergrund einer EpicControl
    /// </summary>
    public class EpicUI_Base : BaseUI
    {
        protected override void InitializeComponents()
        {
            this.Height = 143;
            this.Width = 500;

            //Initialisierung der BackgroundShape

            BackgroundShape = new Ellipse();
            BackgroundShape.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            BackgroundShape.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            BackgroundShape.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            BackgroundShape.StrokeThickness = 3;

            BackgroundShape.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            Root.Children.Add(BackgroundShape);
        }

        protected override void UpdateMe(Data.ItemBase item)
        {
           //None necessary
        }
    }
}
