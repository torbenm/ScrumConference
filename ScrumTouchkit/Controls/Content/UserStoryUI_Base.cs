using ScrumTouchkit.Controls.Content.Abstract;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScrumTouchkit.Controls.Content
{
    public class UserStoryUI_Base : BaseUI
    {
        #region Cross, Plus, Minus

        /**
         * Verschiedene Kreuze und Symbole, die eingeblendet werden können
         * (Zum Beispiel um Drag & Drop Situationen zu visualisieren)
         **/ 
        public bool CrossVisible
        {
            get
            {
                return cross.IsVisible;
            }
            set
            {
                if (value)
                {
                    cross.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    cross.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public bool DeleteVisible
        {
            get
            {
                return dcross.IsVisible;
            }
            set
            {
                if (value)
                {
                    dcross.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    dcross.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public bool PlusVisible
        {
            get
            {
                return aplus.IsVisible;
            }
            set
            {
                if (value)
                {
                    aplus.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    aplus.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public bool MinusVisible
        {
            get
            {
                return rminus.IsVisible;
            }
            set
            {
                if (value)
                {
                    rminus.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    rminus.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private DeleteCross dcross;
        private CenterCross cross;
        private Plus aplus;
        private Minus rminus;
        #endregion

        protected override void InitializeComponents()
        {

            this.Width = 475;
            this.Height = 300;

            // Initialisierung der BackgroundShape

            BackgroundShape = new Rectangle();
            BackgroundShape.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            BackgroundShape.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            BackgroundShape.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            BackgroundShape.StrokeThickness = 2;
            this.Root.Children.Add(BackgroundShape);

            // Initialisierung der Symbole

            cross = new CenterCross();
            cross.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            cross.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            this.Root.Children.Add(cross);
            CrossVisible = false;

            dcross = new DeleteCross();
            dcross.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            dcross.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            this.Root.Children.Add(dcross);
            DeleteVisible = false;

            aplus = new Plus();
            aplus.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            aplus.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            this.Root.Children.Add(aplus);
            PlusVisible = false;

            rminus = new Minus();
            rminus.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            rminus.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            this.Root.Children.Add(rminus);
            MinusVisible = false;

        }

        protected override void UpdateMe(Data.ItemBase item)
        {

        }
    }
}
