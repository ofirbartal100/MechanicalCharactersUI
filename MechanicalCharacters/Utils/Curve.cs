using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MechanicalCharacters.Utils
{
    public class Curve : IDrawable
    {
        public List<Point> PointsList { get; set; } = new List<Point>();
        public double RemoveDistanceThresh { get; private set; } = 10;

        public void Add(Point position)
        {
            PointsList.Add(position);
        }

        private Path thePath;

        public IEnumerable<UIElement> GetDrawing()
        {
            var DrawingElements = new List<UIElement>();
            foreach (var point in PointsList)
            {
                DrawingElements.Add(EllipseExtentions.CanvasPoint(point));
            }

            if (PointsList.Count > 0)
                DrawingElements.Add(new Polyline() { Points = new PointCollection(PointsList.Concat(new Point[] { PointsList.First() })), Stroke = Brushes.Black });

            if (PointsList.Count() >= 3)
            {
                var path = MakeBezierPath(PointsList.ToArray());
                DrawingElements.Add(path);
            }

            return DrawingElements;
        }

        public Point[] GetSampledCurve()
        {
            Pen np = new Pen(Brushes.Aqua, 1);
            var pg = thePath.Data.GetWidenedPathGeometry(np, 1, ToleranceType.Absolute);
            var points = ((PolyLineSegment)pg.Figures[0].Segments[0]).Points;
            var realPath = new List<Point>();
            foreach (var point in points)
            {
                //since[0] is Self;
                var closest = points.OrderBy(point1 =>
                    ((point.X - point1.X) * (point.X - point1.X) + (point.Y - point1.Y) * (point.Y - point1.Y))).ToList()[1];
                var mid = new Point((point.X + closest.X) / 2, (point.Y + closest.Y) / 2);
                if (!realPath.Contains(mid))
                {
                    realPath.Add(mid);
                }
            }
            if (realPath.Count > 72)
            {
                realPath = Downsample(realPath);
            }
            else if (realPath.Count < 72)
            {
                realPath = Upsample(realPath);
            }
            return realPath.ToArray();
        }

        private List<Point> Upsample(List<Point> realPath)
        {
            var upsampled = realPath;
            while (upsampled.Count < 72)
            {
                var a = upsampled.Zip(upsampled.Skip(1).Concat(new[] { upsampled[0] }), (point, nextPoint) =>
                   {
                       return new { first = point, second = nextPoint, idx = upsampled.IndexOf(point), dist = DistanceBetweenPoints(point, nextPoint) };
                   }).OrderBy(arg => arg.dist).Last();

                upsampled.Insert(a.idx + 1, new Point((a.first.X + a.second.X) / 2, (a.first.Y + a.second.Y) / 2));
            }
            return upsampled;
        }

        private List<Point> Downsample(List<Point> realPath)
        {
            var downsampled = realPath;
            while (downsampled.Count > 72)
            {
                var a = downsampled.Zip(downsampled.Skip(1).Concat(new[] { downsampled[0] }), (point, nextPoint) =>
                {
                    return new { first = point, second = nextPoint, idx = downsampled.IndexOf(point), dist = DistanceBetweenPoints(point, nextPoint) };
                }).OrderBy(arg => arg.dist).First();

                downsampled.RemoveAt(a.idx);
            }
            return downsampled;
        }

        public void RemoveAround(Point position)
        {
            PointsList.RemoveAll(point => DistanceBetweenPoints(position, point) < RemoveDistanceThresh);
        }

        private double DistanceBetweenPoints(Point point, Point position)
        {
            return Math.Sqrt((point.X - position.X) * (point.X - position.X) + (point.Y - position.Y) * +(point.Y - position.Y));
        }

        // Make a Path holding a series of Bezier curves.
        // The points parameter includes the points of the inscribing polygon
        private Path MakeBezierPath(Point[] points)
        {
            // Create a Path to hold the geometry.
            Path path = new Path();

            // Add a PathGeometry.
            PathGeometry path_geometry = new PathGeometry();
            path.Data = path_geometry;

            // Create a PathFigure.
            PathFigure path_figure = new PathFigure();
            path_geometry.Figures.Add(path_figure);

            // Create a PathSegmentCollection.
            PathSegmentCollection path_segment_collection =
                new PathSegmentCollection();
            path_figure.Segments = path_segment_collection;

            // Add the rest of the points to a PointCollection.
            PointCollection point_collection = new PointCollection();
            for (int i = 0; i < points.Length; i++)
            {
                //polygon vertice as 2 controls for the curve
                var control_point = points[i];
                point_collection.Add(control_point);
                point_collection.Add(control_point);

                //end point is the middle of next curve
                var endpoint = new Point(
                    (points[i].X + points[(i + 1 + points.Length) % points.Length].X) / 2,
                    (points[i].Y + points[(i + 1 + points.Length) % points.Length].Y) / 2);

                point_collection.Add(endpoint);
            }

            // Start at the first point.
            path_figure.StartPoint = point_collection.Last();

            // Make a PolyBezierSegment from the points.
            PolyBezierSegment bezier_segment = new PolyBezierSegment
            {
                Points = point_collection
            };

            // Add the PolyBezierSegment to othe segment collection.
            path_segment_collection.Add(bezier_segment);
            path.Stroke = Brushes.Orange;
            thePath = path;
            return path;
        }
    }
}