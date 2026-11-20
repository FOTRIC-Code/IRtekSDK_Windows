using System.Drawing;
using ThermalImage;

namespace ThermalFileReaderDemo
{
    public class Overlay
    {
        private ThermalImageFile Image { get; set; }
        private Pen Pen { get; set; }
        private Brush TransparentBrush { get; set; }
        private Font TextFont { get; set; }
        private Brush TextBrush { get; set; }

        private readonly Color SemiTransparentColor = Color.FromArgb(128, Color.Black);

        public Overlay(ThermalImageFile image)
        {
            Image = image;
            Pen = Pens.White;
            TransparentBrush = new SolidBrush(SemiTransparentColor);
            TextFont = new Font(FontFamily.GenericSansSerif, 9);
            TextBrush = Brushes.White;
        }

        public void Draw(Graphics graphics)
        {
            ThermalImageFile.MarkerCollection markers = Image.Markers;

            ThermalImageFile.MarkerRectangle globalMarker = markers[0] as ThermalImageFile.MarkerRectangle;
            DrawHotSpot(graphics, globalMarker.HotSpot);
            DrawColdSpot(graphics, globalMarker.ColdSpot);

            for (int i = 1; i < markers.Count; i++)
            {
                ThermalImageFile.Marker marker = markers[i];
                switch (marker.Type)
                {
                    case MarkerType.Spot:
                        DrawSpot(marker as ThermalImageFile.MarkerSpot, graphics);
                        break;
                    case MarkerType.Rectangle:
                        DrawRectangle(marker as ThermalImageFile.MarkerRectangle, graphics);
                        break;
                    case MarkerType.Ellipse:
                        DrawEllipse(marker as ThermalImageFile.MarkerEllipse, graphics);
                        break;
                    case MarkerType.Polyline:
                        DrawPolyLine(marker as ThermalImageFile.MarkerPolyline, graphics);
                        break;
                    case MarkerType.Polygon:
                        DrawPolygon(marker as ThermalImageFile.MarkerPolygon, graphics);
                        break;
                }
            }
        }

        private void DrawSpot(ThermalImageFile.MarkerSpot spot, Graphics graphics)
        {
            int dist = 17;
            int r = 4;
            Point p = spot.Point;
            graphics.DrawEllipse(Pen, p.X - r / 2, p.Y - r / 2, r, r);

            Point p1 = new Point(p.X, p.Y - dist);
            Point p2 = new Point(p.X, p.Y - r);
            graphics.DrawLine(Pen, p1, p2);

            Point p3 = new Point(p.X, p.Y + r);
            Point p4 = new Point(p.X, p.Y + dist);
            graphics.DrawLine(Pen, p3, p4);

            p1 = new Point(p.X - dist, p.Y);
            p2 = new Point(p.X - r, p.Y);
            graphics.DrawLine(Pen, p1, p2);

            p3 = new Point(p.X + r, p.Y);
            p4 = new Point(p.X + dist, p.Y);
            graphics.DrawLine(Pen, p3, p4);

            string str = spot.ThermalParams.LocalParams ? spot.Name + "*" : spot.Name;
            graphics.DrawString(str, TextFont, TextBrush, p.X + dist / 2, p.Y - dist);
        }

        private static Rectangle PadHitArea(Point pt)
        {
            return new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
        }

        private void DrawRectangleSelection(ThermalImageFile.MarkerRectangle rectangle, Graphics graphics)
        {
            Rectangle rect = rectangle.Rectangle;
            Rectangle selection = PadHitArea(new Point(rect.Location.X, rect.Location.Y));

            // Upper left
            graphics.DrawLine(Pen, selection.Location, new Point(selection.X + selection.Width, selection.Y));
            graphics.DrawLine(Pen, selection.Location, new Point(selection.X, selection.Y + selection.Height));

            // Upper mid
            selection = PadHitArea(new Point(rect.Location.X + rect.Width / 2, rect.Location.Y));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.X + selection.Width, selection.Y));
            graphics.DrawLine(Pen, new Point(selection.Location.X, selection.Y + selection.Height), new Point(selection.X + selection.Width, selection.Y + selection.Height));

            // Upper right
            selection = PadHitArea(new Point(rect.Location.X + rect.Width, rect.Location.Y));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.X + selection.Width, selection.Y));
            graphics.DrawLine(Pen, new Point(selection.X + selection.Width, selection.Y), new Point(selection.X + selection.Width, selection.Y + selection.Height));


            // Right mid
            selection = PadHitArea(new Point(rect.Location.X + rect.Width, rect.Location.Y + rect.Height / 2));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.X, selection.Y + selection.Height));
            graphics.DrawLine(Pen, new Point(selection.X + selection.Width, selection.Y), new Point(selection.X + selection.Width, selection.Y + selection.Height));

            // Bottom right
            selection = PadHitArea(new Point(rect.Location.X + rect.Width, rect.Location.Y + rect.Height));

            graphics.DrawLine(Pen, new Point(selection.Location.X + selection.Width, selection.Location.Y), new Point(selection.Location.X + selection.Width, selection.Y + selection.Height));
            graphics.DrawLine(Pen, new Point(selection.Location.X + selection.Width, selection.Y + selection.Height), new Point(selection.X, selection.Y + selection.Height));

            // Bottom mid
            selection = PadHitArea(new Point(rect.Location.X + rect.Width / 2, rect.Location.Y + rect.Height));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.Location.X + selection.Width, selection.Y));
            graphics.DrawLine(Pen, new Point(selection.Location.X, selection.Y + selection.Height), new Point(selection.Location.X + selection.Width, selection.Y + selection.Height));

            // Lower left
            selection = PadHitArea(new Point(rect.Location.X, rect.Location.Y + rect.Height));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.Location.X, selection.Y + selection.Height));
            graphics.DrawLine(Pen, new Point(selection.Location.X, selection.Y + selection.Height), new Point(selection.Location.X + selection.Width, selection.Y + selection.Height));

            // Left mid
            selection = PadHitArea(new Point(rect.Location.X, rect.Location.Y + rect.Height / 2));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.Location.X, selection.Y + selection.Height));
            graphics.DrawLine(Pen, new Point(selection.Location.X + selection.Width, selection.Y), new Point(selection.Location.X + selection.Width, selection.Y + selection.Height));
        }

        private void DrawRectangle(ThermalImageFile.MarkerRectangle rectangle, Graphics graphics)
        {
            Rectangle rect = rectangle.Rectangle;
            graphics.DrawRectangle(Pen, rect);

            DrawHotSpot(graphics, rectangle.HotSpot);
            DrawColdSpot(graphics, rectangle.ColdSpot);

            DrawRectangleSelection(rectangle, graphics);

            string str = rectangle.ThermalParams.LocalParams ? rectangle.Name + "*" : rectangle.Name;
            graphics.DrawString(str, TextFont, TextBrush, rect.Location.X + 5, rect.Location.Y + 5);
        }

        private void DrawEllipseSelection(ThermalImageFile.MarkerEllipse ellipse, Graphics graphics)
        {
            Rectangle rect = ellipse.Rectangle;
            Rectangle selection = PadHitArea(new Point(rect.Location.X, rect.Location.Y));

            // Upper left
            graphics.DrawLine(Pen, selection.Location, new Point(selection.X + selection.Width, selection.Y));
            graphics.DrawLine(Pen, selection.Location, new Point(selection.X, selection.Y + selection.Height));

            // Upper mid
            selection = PadHitArea(new Point(rect.Location.X + rect.Width / 2, rect.Location.Y));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.X + selection.Width, selection.Y));
            graphics.DrawLine(Pen, new Point(selection.Location.X, selection.Y + selection.Height), new Point(selection.X + selection.Width, selection.Y + selection.Height));

            // Upper right
            selection = PadHitArea(new Point(rect.Location.X + rect.Width, rect.Location.Y));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.X + selection.Width, selection.Y));
            graphics.DrawLine(Pen, new Point(selection.X + selection.Width, selection.Y), new Point(selection.X + selection.Width, selection.Y + selection.Height));


            // Right mid
            selection = PadHitArea(new Point(rect.Location.X + rect.Width, rect.Location.Y + rect.Height / 2));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.X, selection.Y + selection.Height));
            graphics.DrawLine(Pen, new Point(selection.X + selection.Width, selection.Y), new Point(selection.X + selection.Width, selection.Y + selection.Height));

            // Bottom right
            selection = PadHitArea(new Point(rect.Location.X + rect.Width, rect.Location.Y + rect.Height));

            graphics.DrawLine(Pen, new Point(selection.Location.X + selection.Width, selection.Location.Y), new Point(selection.Location.X + selection.Width, selection.Y + selection.Height));
            graphics.DrawLine(Pen, new Point(selection.Location.X + selection.Width, selection.Y + selection.Height), new Point(selection.X, selection.Y + selection.Height));

            // Bottom mid
            selection = PadHitArea(new Point(rect.Location.X + rect.Width / 2, rect.Location.Y + rect.Height));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.Location.X + selection.Width, selection.Y));
            graphics.DrawLine(Pen, new Point(selection.Location.X, selection.Y + selection.Height), new Point(selection.Location.X + selection.Width, selection.Y + selection.Height));

            // Lower left
            selection = PadHitArea(new Point(rect.Location.X, rect.Location.Y + rect.Height));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.Location.X, selection.Y + selection.Height));
            graphics.DrawLine(Pen, new Point(selection.Location.X, selection.Y + selection.Height), new Point(selection.Location.X + selection.Width, selection.Y + selection.Height));

            // Left mid
            selection = PadHitArea(new Point(rect.Location.X, rect.Location.Y + rect.Height / 2));

            graphics.DrawLine(Pen, selection.Location, new Point(selection.Location.X, selection.Y + selection.Height));
            graphics.DrawLine(Pen, new Point(selection.Location.X + selection.Width, selection.Y), new Point(selection.Location.X + selection.Width, selection.Y + selection.Height));
        }

        private void DrawEllipse(ThermalImageFile.MarkerEllipse ellipse, Graphics graphics)
        {
            Rectangle rect = ellipse.Rectangle;
            graphics.DrawEllipse(Pen, rect);

            DrawHotSpot(graphics, ellipse.HotSpot);
            DrawColdSpot(graphics, ellipse.ColdSpot);

            DrawEllipseSelection(ellipse, graphics);

            string str = ellipse.ThermalParams.LocalParams ? ellipse.Name + "*" : ellipse.Name;
            graphics.DrawString(str, TextFont, TextBrush, rect.Location.X + 5, rect.Location.Y + 5);
        }

        private void DrawPolylineSelection(ThermalImageFile.MarkerPolyline polyline, Graphics graphics)
        {
            // Left mid
            Rectangle selection = PadHitArea(polyline.Points[0]);
            graphics.DrawRectangle(Pen, selection);

            selection = PadHitArea(polyline.Points[1]);
            graphics.DrawRectangle(Pen, selection);            
        }

        private void DrawPolyLine(ThermalImageFile.MarkerPolyline polyline, Graphics graphics)
        {
            if (polyline.Points.Length == 2)
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.DrawLine(Pen, polyline.Points[0], polyline.Points[1]);

                DrawHotSpot(graphics, polyline.HotSpot);
                DrawColdSpot(graphics, polyline.ColdSpot);

                DrawPolylineSelection(polyline, graphics);

                string str = polyline.ThermalParams.LocalParams ? polyline.Name + "*" : polyline.Name;
                graphics.DrawString(str, TextFont, TextBrush, polyline.Points[0].X + 5, polyline.Points[0].Y + 5);
            }
            else
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.DrawLines(Pen, polyline.Points);

                DrawHotSpot(graphics, polyline.HotSpot);
                DrawColdSpot(graphics, polyline.ColdSpot);

                string str = polyline.ThermalParams.LocalParams ? polyline.Name + "*" : polyline.Name;
                graphics.DrawString(str, TextFont, TextBrush, polyline.Points[0].X + 5, polyline.Points[0].Y + 5);
            }
        }

        private void DrawPolygon(ThermalImageFile.MarkerPolygon polygon, Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.DrawPolygon(Pen, polygon.Points);

            DrawHotSpot(graphics, polygon.HotSpot);
            DrawColdSpot(graphics, polygon.ColdSpot);

            string str = polygon.ThermalParams.LocalParams ? polygon.Name + "*" : polygon.Name;
            graphics.DrawString(str, TextFont, TextBrush, polygon.Points[0].X + 5, polygon.Points[0].Y + 5);
        }

        private void DrawColdSpot(Graphics graphics, Point point)
        {
            Point point1 = point;
            point1.Offset(-6, -6);
            Point point2 = point;
            point2.Offset(6, -6);
            Point[] points = new Point[] { point, point1, point2 };
            graphics.FillPolygon(Brushes.Blue, points);
            graphics.DrawPolygon(Pens.Black, points);
        }

        private void DrawHotSpot(Graphics graphics, Point point)
        {
            Point point1 = point;
            point1.Offset(-6, 6);
            Point point2 = point;
            point2.Offset(6, 6);
            Point[] points = new Point[] { point, point1, point2 };
            graphics.FillPolygon(Brushes.Red, points);
            graphics.DrawPolygon(Pens.Black, points);
        }
    }
}
