using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectArena.Engine.Helpers
{
    public struct Point
    {
        public int X { get; }

        public int Y { get; }

        public static Point operator *(Point point, int multiplier)
        {
            return new Point(point.X * multiplier, point.Y * multiplier);
        }

        public static Point operator /(Point point, int multiplier)
        {
            return new Point(point.X / multiplier, point.Y / multiplier);
        }

        public static Point operator +(Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point operator -(Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
