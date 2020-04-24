using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectArena.Engine.Helpers
{
    public struct PointF
    {
        public float X { get; }

        public float Y { get; }

        public static PointF operator *(PointF point, float multiplier)
        {
            return new PointF(point.X * multiplier, point.Y * multiplier);
        }

        public static PointF operator /(PointF point, float multiplier)
        {
            return new PointF(point.X / multiplier, point.Y / multiplier);
        }

        public static PointF operator +(PointF point1, PointF point2)
        {
            return new PointF(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static PointF operator -(PointF point1, PointF point2)
        {
            return new PointF(point1.X - point2.X, point1.Y - point2.Y);
        }

        public PointF(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
