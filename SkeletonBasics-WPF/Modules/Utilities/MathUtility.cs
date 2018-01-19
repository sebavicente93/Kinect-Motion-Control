using System;
using System.Drawing;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules.Utilities
{
    public static class MathUtility
    {
        public static double DistanceFromPointToLine(PointF point, PointF l1, PointF l2)
        {
            // given a line based on two points, and a point away from the line,
            // find the perpendicular distance from the point to the line.
            // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            // for explanation and defination.

            return Math.Abs((l2.X - l1.X) * (l1.Y - point.Y) - (l1.X - point.X) * (l2.Y - l1.Y)) /
                    Math.Sqrt(Math.Pow(l2.X - l1.X, 2) + Math.Pow(l2.Y - l1.Y, 2));
        }
    }
}
