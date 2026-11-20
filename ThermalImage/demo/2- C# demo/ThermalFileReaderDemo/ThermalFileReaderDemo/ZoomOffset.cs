using System.Drawing;
using ThermalImage;

namespace ThermalFileReaderDemo
{
    public class ZoomOffset
    {
        public ZoomOffset(Rectangle rectangle, ThermalImageFile image)
        {
            Rectangle = rectangle;
            Image = image;
            CalculateZoomOffset();
        }

        private double Zoom { get; set; }
        private Point Offset { get; set; }
        private Rectangle Rectangle { get; set; }
        private ThermalImageFile Image { get; set; }

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
            Size thermalSize = Image.GetThermalSize();

            double curRatio = (double)rect.Width / rect.Height;
            double newRatio = (double)thermalSize.Width / thermalSize.Height;

            if (curRatio > newRatio)
            {
                Zoom = (double)rect.Height / thermalSize.Height;
                Offset = new Point((int)((Rectangle.Width - thermalSize.Width * Zoom) / 2.0 + 0.5), 0);
            }
            else
            {
                Zoom = (double)rect.Width / thermalSize.Width;
                Offset = new Point(0, (int)((Rectangle.Height - thermalSize.Height * Zoom) / 2.0 + 0.5));
            }          
        }
    }
}
