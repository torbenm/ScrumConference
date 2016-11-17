using System;
using System.Windows.Controls;
using System.Windows.Media;
using ScrumTouchkit.Threading;
using System.Windows;

namespace ScrumTouchkit.Controls.Abstract
{
    /// <summary>
    /// Dies ist die erste Klasse in der Hierarchie für Objekte
    /// auf dem Multi-Touch-Tisch.
    /// In dieser Klasse werden die Grundlagen gelegt.
    /// Diese beinhalten vereinfachte Methoden zur
    /// Skalierung, Rotation und Positionierung der Objekte.
    /// </summary>
    public abstract class TransformableControl : UserControl
    {

        #region Getter and Setter - Scale
        /// <summary>
        /// Gibt die skalierte Breite an
        /// </summary>
        public double ScaledWidth
        {
            get
            {
                return CalcWidth(ScaleFactor);
            }
        }

        /// <summary>
        /// Gibt die skalierte Höhe an
        /// </summary>
        public double ScaledHeight
        {
            get
            {
                return CalcHeight(ScaleFactor);
            }
        }

        /// <summary>
        /// Gibt den Faktor der Skalierung an - 1 ist Originalgröße
        /// </summary>
        public double ScaleFactor
        {
            get
            {
                return this.Invoke<double>(new Func<double>(() => { return ScaleTransform.ScaleX; }));
            }
            protected set
            {
                this.Invoke(new Action(() =>
                    {
                        ScaleTransform.ScaleX = value;
                        ScaleTransform.ScaleY = value;
                    }));
            }
        }

        /// <summary>
        /// Transformationsmatrix für die Skalierung
        /// </summary>
        public ScaleTransform ScaleTransform
        {
            get;
            private set;
        }
        #endregion
        #region Getter and Setter - Positions
        /// <summary>
        /// Der Mittelpunkt dieses Objektes
        /// </summary>
        public Point Center
        {
            get
            {
                return new Point(this.CenterX, this.CenterY);
            }
        }
        /// <summary>
        /// Der Mittelpunkt des Objektes auf der X-Achse
        /// </summary>
        public double CenterX
        {
            get
            {
                return this.Invoke<double>(new Func<double>(() =>
                {
                    return Left + this.ActualWidth / 2;
                }));

            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    Left = value - this.ActualWidth / 2;
                }));
            }
        }

        /// <summary>
        /// Der Mittelpunkt des Objektes auf der Y-Achse
        /// </summary>
        public double CenterY
        {
            get
            {
                return this.Invoke<double>(new Func<double>(() =>
                {
                    return Top + this.ActualHeight / 2;
                }));
            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    Top = value - this.ActualHeight / 2;
                }));
            }
        }
        /// <summary>
        /// Die Position des linken Rands des Objektes auf der X-Achse
        /// </summary>
        public double Left
        {
            get
            {
                return this.Invoke<double>(new Func<double>(() =>
                {
                    return (double)GetValue(Canvas.LeftProperty);
                }));
            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    SetValue(Canvas.LeftProperty, value);
                }));
            }
        }

        /// <summary>
        /// Die Position des obersten Randes des Objektes auf der Y-Achse
        /// </summary>
        public double Top
        {
            get
            {
                return this.Invoke<double>(new Func<double>(() =>
                {
                    return (double)GetValue(Canvas.TopProperty);
                }));
            }
            set
            {
                this.Invoke(new Action(() =>
                {
                    SetValue(Canvas.TopProperty, value);
                }));
            }
        }
        #endregion
        #region Getter and Setter - Rotation
        /// <summary>
        /// Der Winkel, in welchem das Objekt zu der Standardausrichtung steht
        /// </summary>
        public double RotateAngle
        {
            get
            {
                return this.Invoke<double>(new Func<double>(() =>
                    {
                        return RotateTransform.Angle;
                    }));
            }
            set
            {
                this.Invoke(new Action(() =>
                    {
                        RotateTransform.Angle = value;
                    }));
            }
        }

        /// <summary>
        /// Transformationsmatrix für die Rotation
        /// </summary>
        public RotateTransform RotateTransform
        {
            get;
            private set;
        }
        #endregion
        #region Initialize
        public TransformableControl()
        {

            //Transformationsmatrizen erstellen
            this.ScaleTransform = new ScaleTransform(1,1);
            this.RotateTransform = new RotateTransform(0);

            TransformGroup group = new TransformGroup();
            group.Children.Add(ScaleTransform);
            group.Children.Add(RotateTransform);
            this.RenderTransform = group;

            this.Initialized += UpdateCenter;
            this.SizeChanged += UpdateCenter;

        }
        /// <summary>
        /// Aktualisiert die Mittelpunkte der Transformationsmatrizen auf das
        /// aktuelle Zentrum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdateCenter(object sender, EventArgs e)
        {
            ScaleTransform.CenterX = this.Width/2;
            ScaleTransform.CenterY = this.Height/2;

            RotateTransform.CenterX = this.Width / 2;
            RotateTransform.CenterY = this.Height / 2;
        }
        #endregion
        #region Events
        public event EventHandler ScaleChanged;

        protected void OnScaleChanged()
        {
            if (ScaleChanged != null)
                ScaleChanged(this, new EventArgs());
        }
        #endregion
        #region Scale Methods
        /// <summary>
        /// Berechnet die Höhe dieses Objektes mit einem bestimmten Faktor
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        private double CalcHeight(double factor)
        {           
            return this.Invoke<double>(new Func<double>(() =>
                {
                    return this.Height * factor;
                }));
        }
        /// <summary>
        /// Berechnet die Breite dieses Objektes mit einem bestimmten Faktor
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        private double CalcWidth(double factor)
        {
            return this.Invoke<double>(new Func<double>(() =>
            {
                return this.Width * factor;
            }));
        }

        /// <summary>
        /// Passt den ScaleFactor so an, dass die angegebene Größe
        /// des Objektes erreicht wird. Dabei wird das Seitenverhältnis beibehalten
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(double width, double height)
        {
            this.Invoke(new Action(() =>
                {
                    double factor = width / this.Width;
                    double cwidth = CalcWidth(factor);
                    double cheight = CalcHeight(factor);
                    if(cwidth <= MaxWidth && cwidth >= MinWidth)
                    {
                        if(cheight <= MaxHeight && cheight >= MinHeight)
                        {
                            ScaleFactor = factor;
                            OnScaleChanged();
                        }
                    }
                }));
        }

        /// <summary>
        /// Ändert die tatsächliche Größe des Objektes (nciht die Skalierung!)
        /// auf die neuen Werte. Dies ist sinnvoll, wenn z.B. ein größerer Content als vorher
        /// angezeigt wird.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SwitchSize(double width, double height)
        {
            this.Invoke(new Action(() =>
            {
                double sw = this.ScaledWidth;
                double sh = this.ScaledHeight;


                double x = this.CenterX;
                double y = this.CenterY;

                this.ScaleFactor = 1;
                this.Width = width;
                this.Height = height;

                this.CenterX = x;
                this.CenterY = y;
                this.UpdateCenter(null, null);

            }));
        }
        #endregion
    }
}
