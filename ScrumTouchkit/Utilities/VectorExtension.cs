using System;
using System.Windows;

namespace ScrumTouchkit.Utilities
{
    public static class VectorExtension
    {

        public static Vector FromPoint(Point pt)
        {
            return new Vector(pt.X, pt.Y);
        }

        public static Vector FromPoints(Point pt1, Point pt2)
        {          
            return new Vector(pt2.X - pt1.X, pt2.Y - pt1.Y);
        }

        public static double GetAngle(this Vector v1, Vector v2)
        {
            return Vector.AngleBetween(v1, v2);
        }

        public static double GetAngleDegrees(this Vector v1, Vector v2)
        {
            double rad = v1.GetAngle(v2);
            return rad * 180 / Math.PI;
        }

        public static double GetDistance(this Vector v1, Vector v2)
        {
            double dx = v1.X - v2.X;
            double dy = v1.Y - v2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Vector GetNormalizedVector(this Vector v)
        {
            Vector v2 = new Vector(v.X, v.Y);
            v2.Normalize();
            return v2;
        }

        public static Point MovePoint(this Vector v, Point start, double length)
        {
            Vector normalizedVector = v.GetNormalizedVector();
            Point ret = new Point(start.X, start.Y);
            ret.X += normalizedVector.X * length;
            ret.Y += normalizedVector.Y * length;
            return ret;
        }

        public static Point PointOnBounds(this Vector v, Point start, Rect bounds)
        {
            Point p = new Point();
            Vector normalizedVector = v.GetNormalizedVector();
            double factor = 0;
            double distance = 0;
            if (Math.Abs(v.X) > Math.Abs(v.Y))
            {
                if (v.X > 0)
                    p.X = bounds.Width;
                else
                    p.X = bounds.X;
                distance = p.X - start.X;
                factor = distance / normalizedVector.X;
                p.Y = start.Y + normalizedVector.Y * factor;

            }
            else
            {
                if (v.Y > 0)
                    p.Y = bounds.Height;
                else
                    p.Y = bounds.Y;

                distance = p.Y - start.Y;
                factor = distance / normalizedVector.Y;
                p.X = start.X + normalizedVector.X * factor;
            }
            return p;
        }

       



    }
}
