using System;
using System.Drawing;

using AcousticImage;

namespace AcousticFileReaderDemo
{
    public class Overlay
    {
        private AcousticImageFile Image { get; set; }
        private Pen Pen { get; set; }
        private Brush TransparentBrush { get; set; }
        private Font TextFont { get; set; }
        private Brush TextBrush { get; set; }

        private readonly Color SemiTransparentColor = Color.FromArgb(128, Color.Black);

        public Overlay(AcousticImageFile image)
        {
            Image = image;
            Pen = Pens.White;
            TransparentBrush = new SolidBrush(SemiTransparentColor);
            TextFont = new Font(FontFamily.GenericSansSerif, 9);
            TextBrush = Brushes.White;
        }

        public void Draw(Graphics graphics, int width, int height)
        {
            AcousticImageFile.MarkerCollection markers = Image.Markers;
            var acSize = Image.GetAcousticSize();
            var scaleX = width / (double)acSize.Width;
            var scaleY = height / (double)acSize.Height;

            for (int i = 0; i < markers.Count; i++)
            {
                AcousticImageFile.Marker marker = markers[i];
                switch (marker.Type)
                {
                    case MarkerType.Spot:
                        DrawSpot(marker as AcousticImageFile.MarkerSpot, graphics, scaleX, scaleY);
                        break;
                    case MarkerType.Ellipse:
                        DrawEllipse(marker as AcousticImageFile.MarkerEllipse, graphics, scaleX, scaleY);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void DrawSpot(AcousticImageFile.MarkerSpot spot, Graphics graphics, double scaleX, double scaleY)
        {
            int dist = 17;
            int r = 4;
            Point p = new Point((int)(spot.Point.X * scaleX), (int)(spot.Point.Y * scaleY));
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

            string str = spot.Name;
            graphics.DrawString(str, TextFont, TextBrush, p.X + dist / 2, p.Y - dist);
        }

        private static Rectangle PadHitArea(Point pt)
        {
            return new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
        }

        private void DrawEllipseSelection(AcousticImageFile.MarkerEllipse ellipse, Graphics graphics, double scaleX, double scaleY)
        {
            Rectangle rect = new Rectangle((int)(ellipse.Rectangle.Location.X * scaleX), (int)(ellipse.Rectangle.Location.Y * scaleY), (int)(ellipse.Rectangle.Width * scaleX), (int)(ellipse.Rectangle.Height * scaleY));
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

        private void DrawEllipse(AcousticImageFile.MarkerEllipse ellipse, Graphics graphics, double scaleX, double scaleY)
        {
            Rectangle rect = new Rectangle((int)(ellipse.Rectangle.Location.X * scaleX), (int)(ellipse.Rectangle.Location.Y * scaleY), (int)(ellipse.Rectangle.Width * scaleX), (int)(ellipse.Rectangle.Height * scaleY));
            graphics.DrawEllipse(Pen, rect);

            DrawMaxSpot(graphics, ellipse.HotSpot, scaleX, scaleY);

            DrawEllipseSelection(ellipse, graphics, scaleX, scaleY);

            string str = ellipse.Name;
            graphics.DrawString(str, TextFont, TextBrush, rect.Location.X + 5, rect.Location.Y + 5);
        }

        private void DrawMaxSpot(Graphics graphics, Point point, double scaleX, double scaleY)
        {
            var pointRef = new Point((int)(point.X * scaleX), (int)(point.Y * scaleY));
            Point point1 = pointRef;
            point1.Offset(-6, 6);
            Point point2 = pointRef;
            point2.Offset(6, 6);
            Point[] points = new Point[] { pointRef, point1, point2 };
            graphics.FillPolygon(Brushes.Red, points);
            graphics.DrawPolygon(Pens.Black, points);
        }
    }
}
