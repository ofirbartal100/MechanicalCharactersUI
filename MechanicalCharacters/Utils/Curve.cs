using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                DrawingElements.Add(MakeBezierPath(PointsList.ToArray()));
            }

            return DrawingElements;
        }

        public Point[] GetSampledCurve(Point anchor)
        {
            return null;
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
                point_collection.Add(points[i]);
                point_collection.Add(points[i]);

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
            return path;
        }
    }
}
