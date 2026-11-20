using System.Drawing;

using AcousticImage;

namespace AcousticFileReaderDemo
{
    public class ZoomOffset
    {
        public ZoomOffset(Rectangle rectangle, Size imageSize)
        {
            Rectangle = rectangle;
            ImageSize = imageSize;
            CalculateZoomOffset();
        }

        private double Zoom { get; set; }
        private Point Offset { get; set; }
        private Rectangle Rectangle { get; set; }
        private Size ImageSize { get; set; }

        public Point ImageToLogical(Point point)
        {
            point.X = (int)(point.X * Zoom + Offset.X);
            point.Y = (int)(point.Y * Zoom + Offset.Y);
            return point;
        }

        public Point LogicalToImage(Point point)
        {
            point.X = (int)((point.X - Offset.X) / Zoom + 0.5);
            point.Y = (int)((point.Y - Offset.Y) / Zoom + 0.5);
            return point;
        }

        public void CalculateZoomOffset()
        {
            Rectangle rect = Rectangle;

            double curRatio = (double)rect.Width / rect.Height;
            double newRatio = (double)ImageSize.Width / ImageSize.Height;

            if (curRatio > newRatio)
            {
                Zoom = (double)rect.Height / ImageSize.Height;
                Offset = new Point((int)((Rectangle.Width - ImageSize.Width * Zoom) / 2.0 + 0.5), 0);
            }
            else
            {
                Zoom = (double)rect.Width / ImageSize.Width;
                Offset = new Point(0, (int)((Rectangle.Height - ImageSize.Height * Zoom) / 2.0 + 0.5));
            }          
        }
    }
}
