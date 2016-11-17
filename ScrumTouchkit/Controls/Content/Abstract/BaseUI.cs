using ScrumTouchkit.Data;
using System.Windows.Controls;
using ScrumTouchkit.Threading;
using System.Windows.Shapes;
using ScrumTouchkit.Controls.Style;

namespace ScrumTouchkit.Controls.Content.Abstract
{

    /// <summary>
    /// Base UI stellt den Hintergrund der Darstellungen von Items dar
    /// und stellt Funktionen zur Verwaltung von Inhalten zur Verfügung
    /// </summary>
    public abstract class BaseUI : UserControl
    {
        #region vars, getter, setter
        protected Grid Root;
        protected IContent CurrentContent;

        /// <summary>
        /// Die Hintergrundform
        /// </summary>
        public Shape BackgroundShape
        {
            get;
            protected set;
        }
        #endregion
        #region Constructor
        public BaseUI()
        {
            Root = new Grid();
            this.AddChild(Root);
            InitializeComponents();
            BackgroundShape.Effect = StyleHelper.GetOuterGlow();
        }

        #endregion
        #region Content
        /// <summary>
        /// Zeigt den angegebenen Inhalt an
        /// </summary>
        /// <param name="content"></param>
        public void ShowContent(IContent content)
        {
            this.Invoke((() =>
                {
                    UserControl former = CurrentContent as UserControl;
                    if (former != null)
                        Root.Children.Remove(former);

                    UserControl ui = content as UserControl;
                    if (ui != null)
                    {
                        Root.Children.Add(ui);
                        ui.Width = this.Width;
                        ui.Height = this.Height;
                        CurrentContent = content;
                    }
                }));
        }

        /// <summary>
        /// Aktualisiert die Werte in der Darstellung (passt sie also auf die aktuellen im Item an)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="styleonly"></param>
        public void UpdateData(ItemBase item, bool styleonly = false)
        {
            this.Invoke(() =>
                {
                    if (CurrentContent != null && !styleonly)
                        CurrentContent.UpdateData(item);

                    BackgroundShape.Fill = StyleHelper.GetBackgroundBrush(item);
                    this.UpdateMe(item);
                });
        }
        #endregion
        #region Abstracts
        protected abstract void InitializeComponents();
        protected abstract void UpdateMe(ItemBase item);
        #endregion
    }
}
