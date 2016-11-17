using ScrumTouchkit.Controls.Abstract;
using System.Windows;

namespace ScrumTouchkit.Controls.Animation
{
    /// <summary>
    /// Diese Erweiterung ermöglicht Shortcut-Methoden von SurfaceObjects auf Animationen
    /// </summary>
   public static class AnimationExtension
    {
       public static void AnimMove(this SurfaceObject self, Point pt)
       {
           Animation.Animator.Move(self, pt.X, pt.Y);
           self.HasMovingAnimation = true;
       }

       public static void AnimRotate(this SurfaceObject self, double angle)
       {
           Animation.Animator.Rotate(self.RotateTransform, angle);
           self.HasRotateAnimation = true;
       }

       public static void AnimScale(this SurfaceObject self, double factor)
       {
           Animation.Animator.Resize(self.ScaleTransform, factor);
           self.HasResizeAnimation = true;
       }

       public static void AnimFadeIn(this SurfaceObject self)
       {
           Animation.Animator.FadeIn(self);
       }

       public static void AnimFadeOut(this SurfaceObject self, bool fullremoval = true)
       {
           Animation.Animator.FadeOut(self, 
               (s, e) => {
                   if (fullremoval)
                       self.RemoveFromSurface();
                   else
                       self.Hide(true);
               });
       }

    }
}
