using System.Drawing;
using System.Windows.Forms;
using ThermalImage;

namespace ThermalFileReaderDemo
{
    public class MarkersListView : ListView
    {
        private const float CELSIUS_0 = 273.15F;

        private string ToString(LocalThermalParams thermalParams)
        {
            string str = thermalParams.LocalParams ? "ON" : "OFF";
            str += " " + thermalParams.Emissivity.ToString("F2");
            str += " " + thermalParams.Distance.ToString("F1") + "m";
            str += " " + (thermalParams.ReflTemp - CELSIUS_0).ToString("F1") + "℃";
            return str;
        }

        private ListViewItem FindMarker(ThermalImageFile.Marker marker)
        {
            foreach (ListViewItem item in Items)
            {
                if (marker == item.Tag)
                {
                    return item;
                }
            }
            return null;
        }

        private ListViewItem FindDelta(ThermalImageFile.Delta delta)
        {
            foreach (ListViewItem item in Items)
            {
                if (delta == item.Tag)
                {
                    return item;
                }
            }
            return null;
        }

        private ListViewItem CreateSpotItem(ThermalImageFile.MarkerSpot markerSpot)
        {
            if (markerSpot == null)
            {
                return null;
            }

            Point p = markerSpot.Point;

            ListViewItem item = new ListViewItem(markerSpot.Name);
            Reference reference = markerSpot.Owner.GetReference();
            item.SubItems.Add((reference.Enabled ? markerSpot.Value : markerSpot.Value - CELSIUS_0).ToString("F4"));
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.SubItems.Add(p.X.ToString());
            item.SubItems.Add(p.Y.ToString());
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.SubItems.Add(ToString(markerSpot.ThermalParams));
            item.SubItems.Add(markerSpot.ExInfo);
            item.Tag = markerSpot;

            return item;
        }

        private void UpdateSpotItem(ThermalImageFile.MarkerSpot markerSpot, ListViewItem item)
        {
            if (markerSpot == null)
            {
                return;
            }

            Point p = markerSpot.Point;

            item.Text = markerSpot.Name;
            int index = 1;
            Reference reference = markerSpot.Owner.GetReference();
            item.SubItems[index++].Text = (reference.Enabled ? markerSpot.Value : markerSpot.Value - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = p.X.ToString();
            item.SubItems[index++].Text = p.Y.ToString();
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = ToString(markerSpot.ThermalParams);
            item.SubItems.Add(markerSpot.ExInfo);
            item.Tag = markerSpot;
        }

        private ListViewItem CreateRectangleItem(ThermalImageFile.MarkerRectangle markerRectangle)
        {
            if (markerRectangle == null)
            {
                return null;
            }

            Rectangle rectangle = markerRectangle.Rectangle;

            ListViewItem item = new ListViewItem(markerRectangle.Name);
            item.SubItems.Add("---");
            Reference reference = markerRectangle.Owner.GetReference();
            item.SubItems.Add((reference.Enabled ? markerRectangle.Min : markerRectangle.Min - CELSIUS_0).ToString("F4"));
            item.SubItems.Add((reference.Enabled ? markerRectangle.Max : markerRectangle.Max - CELSIUS_0).ToString("F4"));
            item.SubItems.Add((reference.Enabled ? markerRectangle.Avg : markerRectangle.Avg - CELSIUS_0).ToString("F4"));
            item.SubItems.Add(rectangle.X.ToString());
            item.SubItems.Add(rectangle.Y.ToString());
            item.SubItems.Add(rectangle.Right.ToString());
            item.SubItems.Add(rectangle.Bottom.ToString());
            item.SubItems.Add(markerRectangle.Area.ToString("F4") + "m^2");
            item.SubItems.Add(ToString(markerRectangle.ThermalParams));
            item.SubItems.Add(markerRectangle.ExInfo);
            item.Tag = markerRectangle;

            return item;
        }

        private void UpdateRectangleItem(ThermalImageFile.MarkerRectangle markerRectangle, ListViewItem item)
        {
            if (markerRectangle == null)
            {
                return;
            }

            Rectangle rectangle = markerRectangle.Rectangle;

            item.Text = markerRectangle.Name;
            int index = 1;
            item.SubItems[index++].Text = "---";
            Reference reference = markerRectangle.Owner.GetReference();
            item.SubItems[index++].Text = (reference.Enabled ? markerRectangle.Min : markerRectangle.Min - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = (reference.Enabled ? markerRectangle.Max : markerRectangle.Max - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = (reference.Enabled ? markerRectangle.Avg : markerRectangle.Avg - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = rectangle.X.ToString();
            item.SubItems[index++].Text = rectangle.Y.ToString();
            item.SubItems[index++].Text = rectangle.Right.ToString();
            item.SubItems[index++].Text = rectangle.Bottom.ToString();
            item.SubItems[index++].Text = markerRectangle.Area.ToString("F4") + "m^2";
            item.SubItems[index++].Text = ToString(markerRectangle.ThermalParams);
            item.SubItems.Add(markerRectangle.ExInfo);
            item.Tag = markerRectangle;
        }

        private ListViewItem CreateEllipseItem(ThermalImageFile.MarkerEllipse markerEllipse)
        {
            if (markerEllipse == null)
            {
                return null;
            }

            Rectangle rectangle = markerEllipse.Rectangle;

            ListViewItem item = new ListViewItem(markerEllipse.Name);
            item.SubItems.Add("---");
            Reference reference = markerEllipse.Owner.GetReference();
            item.SubItems.Add((reference.Enabled ? markerEllipse.Min : markerEllipse.Min - CELSIUS_0).ToString("F4"));
            item.SubItems.Add((reference.Enabled ? markerEllipse.Max : markerEllipse.Max - CELSIUS_0).ToString("F4"));
            item.SubItems.Add((reference.Enabled ? markerEllipse.Avg : markerEllipse.Avg - CELSIUS_0).ToString("F4"));
            item.SubItems.Add(rectangle.X.ToString());
            item.SubItems.Add(rectangle.Y.ToString());
            item.SubItems.Add(rectangle.Right.ToString());
            item.SubItems.Add(rectangle.Bottom.ToString());
            item.SubItems.Add(markerEllipse.Area.ToString("F4") + "m^2");
            item.SubItems.Add(ToString(markerEllipse.ThermalParams));
            item.SubItems.Add(markerEllipse.ExInfo);
            item.Tag = markerEllipse;

            return item;
        }

        private void UpdateEllipseItem(ThermalImageFile.MarkerEllipse markerEllipse, ListViewItem item)
        {
            if (markerEllipse == null)
            {
                return;
            }

            Rectangle rectangle = markerEllipse.Rectangle;

            item.Text = markerEllipse.Name;
            int index = 1;
            item.SubItems[index++].Text = "---";
            Reference reference = markerEllipse.Owner.GetReference();
            item.SubItems[index++].Text = (reference.Enabled ? markerEllipse.Min : markerEllipse.Min - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = (reference.Enabled ? markerEllipse.Max : markerEllipse.Max - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = (reference.Enabled ? markerEllipse.Avg : markerEllipse.Avg - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = rectangle.X.ToString();
            item.SubItems[index++].Text = rectangle.Y.ToString();
            item.SubItems[index++].Text = rectangle.Right.ToString();
            item.SubItems[index++].Text = rectangle.Bottom.ToString();
            item.SubItems[index++].Text = markerEllipse.Area.ToString("F4") + "m^2";
            item.SubItems[index++].Text = ToString(markerEllipse.ThermalParams);
            item.SubItems.Add(markerEllipse.ExInfo);
            item.Tag = markerEllipse;
        }

        private ListViewItem CreatePolygonItem(ThermalImageFile.MarkerPolygon markerPolygon)
        {
            if (markerPolygon == null)
            {
                return null;
            }

            Point[] points = markerPolygon.Points;

            ListViewItem item = new ListViewItem(markerPolygon.Name);
            item.SubItems.Add("---");
            Reference reference = markerPolygon.Owner.GetReference();
            item.SubItems.Add((reference.Enabled ? markerPolygon.Min : markerPolygon.Min - CELSIUS_0).ToString("F4"));
            item.SubItems.Add((reference.Enabled ? markerPolygon.Max : markerPolygon.Max - CELSIUS_0).ToString("F4"));
            item.SubItems.Add((reference.Enabled ? markerPolygon.Avg : markerPolygon.Avg - CELSIUS_0).ToString("F4"));
            item.SubItems.Add(points[0].X.ToString());
            item.SubItems.Add(points[0].Y.ToString());
            item.SubItems.Add("...");
            item.SubItems.Add("...");
            item.SubItems.Add(markerPolygon.Area.ToString("F4") + "m^2");
            item.SubItems.Add(ToString(markerPolygon.ThermalParams));
            item.SubItems.Add(markerPolygon.ExInfo);
            item.Tag = markerPolygon;

            return item;
        }

        private void UpdatePolygonItem(ThermalImageFile.MarkerPolygon markerPolygon, ListViewItem item)
        {
            if (markerPolygon == null)
            {
                return;
            }

            Point[] points = markerPolygon.Points;

            item.Text = markerPolygon.Name;
            int index = 1;
            item.SubItems[index++].Text = "---";
            Reference reference = markerPolygon.Owner.GetReference();
            item.SubItems[index++].Text = (reference.Enabled ? markerPolygon.Min : markerPolygon.Min - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = (reference.Enabled ? markerPolygon.Max : markerPolygon.Max - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = (reference.Enabled ? markerPolygon.Avg : markerPolygon.Avg - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = points[0].X.ToString();
            item.SubItems[index++].Text = points[0].Y.ToString();
            item.SubItems[index++].Text = "...";
            item.SubItems[index++].Text = "...";
            item.SubItems[index++].Text = markerPolygon.Area.ToString("F4") + "m^2";
            item.SubItems[index++].Text = ToString(markerPolygon.ThermalParams);
            item.SubItems.Add(markerPolygon.ExInfo);
            item.Tag = markerPolygon;
        }

        private ListViewItem CreatePolylineItem(ThermalImageFile.MarkerPolyline markerPolyline)
        {
            if (markerPolyline == null)
            {
                return null;
            }

            Point[] points = markerPolyline.Points;

            ListViewItem item = new ListViewItem(markerPolyline.Name);
            item.SubItems.Add("---");
            Reference reference = markerPolyline.Owner.GetReference();
            item.SubItems.Add((reference.Enabled ? markerPolyline.Min : markerPolyline.Min - CELSIUS_0).ToString("F4"));
            item.SubItems.Add((reference.Enabled ? markerPolyline.Max : markerPolyline.Max - CELSIUS_0).ToString("F4"));
            item.SubItems.Add((reference.Enabled ? markerPolyline.Avg : markerPolyline.Avg - CELSIUS_0).ToString("F4"));
            item.SubItems.Add(points[0].X.ToString());
            item.SubItems.Add(points[0].Y.ToString());
            if (points.Length == 2)
            {
                item.SubItems.Add(points[1].X.ToString());
                item.SubItems.Add(points[1].Y.ToString());
            }
            else
            {
                item.SubItems.Add("...");
                item.SubItems.Add("...");
            }
            item.SubItems.Add(markerPolyline.Length.ToString("F4") + "m");
            item.SubItems.Add(ToString(markerPolyline.ThermalParams));
            item.SubItems.Add(markerPolyline.ExInfo);
            item.Tag = markerPolyline;

            return item;
        }

        private void UpdatePolylineItem(ThermalImageFile.MarkerPolyline markerPolyline, ListViewItem item)
        {
            if (markerPolyline == null)
            {
                return;
            }

            Point[] points = markerPolyline.Points;

            item.Text = markerPolyline.Name;
            int index = 1;
            item.SubItems[index++].Text = "---";
            Reference reference = markerPolyline.Owner.GetReference();
            item.SubItems[index++].Text = (reference.Enabled ? markerPolyline.Min : markerPolyline.Min - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = (reference.Enabled ? markerPolyline.Max : markerPolyline.Max - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = (reference.Enabled ? markerPolyline.Avg : markerPolyline.Avg - CELSIUS_0).ToString("F4");
            item.SubItems[index++].Text = points[0].X.ToString();
            item.SubItems[index++].Text = points[0].Y.ToString();
            if (points.Length == 2)
            {
                item.SubItems[index++].Text = points[1].X.ToString();
                item.SubItems[index++].Text = points[1].Y.ToString();
            }
            else
            {
                item.SubItems[index++].Text = "...";
                item.SubItems[index++].Text = "...";
            }
            item.SubItems[index++].Text = markerPolyline.Length.ToString("F4") + "m";
            item.SubItems[index++].Text = ToString(markerPolyline.ThermalParams);
            item.SubItems.Add(markerPolyline.ExInfo);
            item.Tag = markerPolyline;
        }

        private ListViewItem CreateDeltaItem(ThermalImageFile.Delta delta)
        {
            if (delta == null)
            {
                return null;
            }

            DeltaParams deltaParams = delta.DeltaParams;

            ListViewItem item = new ListViewItem(delta.Name);
            item.SubItems.Add(delta.Value.ToString("F4"));
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            if (string.IsNullOrEmpty(deltaParams.SrcName))
            {
                item.SubItems.Add("---");
                item.SubItems.Add((deltaParams.SrcTemp - CELSIUS_0).ToString("F4"));
            }
            else
            {
                item.SubItems.Add(deltaParams.SrcName);
                item.SubItems.Add(deltaParams.SrcValueType.ToString());
            }
            if (string.IsNullOrEmpty(deltaParams.DestName))
            {
                item.SubItems.Add("---");
                item.SubItems.Add((deltaParams.DestTemp - CELSIUS_0).ToString("F4"));
            }
            else
            {
                item.SubItems.Add(deltaParams.DestName);
                item.SubItems.Add(deltaParams.DestValueType.ToString());
            }
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.Tag = delta;

            return item;
        }

        private void UpdateDeltaItem(ThermalImageFile.Delta delta, ListViewItem item)
        {
            if (delta == null)
            {
                return;
            }

            DeltaParams deltaParams = delta.DeltaParams;

            item.Text = delta.Name;
            int index = 1;
            item.SubItems[index++].Text = delta.Value.ToString("F4");
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            if (string.IsNullOrEmpty(deltaParams.SrcName))
            {
                item.SubItems[index++].Text = "---";
                item.SubItems[index++].Text = (deltaParams.SrcTemp - CELSIUS_0).ToString("F4");
            }
            else
            {
                item.SubItems[index++].Text = deltaParams.SrcName;
                item.SubItems[index++].Text = deltaParams.SrcValueType.ToString();
            }
            if (string.IsNullOrEmpty(deltaParams.DestName))
            {
                item.SubItems[index++].Text = "---";
                item.SubItems[index++].Text = (deltaParams.DestTemp - CELSIUS_0).ToString("F4");
            }
            else
            {
                item.SubItems[index++].Text = deltaParams.DestName;
                item.SubItems[index++].Text = deltaParams.DestValueType.ToString();
            }
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            item.SubItems.Add("---");

            item.Tag = delta;
        }

        public void RemoveMarker(ThermalImageFile.Marker marker)
        {
            ListViewItem item = FindMarker(marker);
            if (item != null)
            {
                BeginUpdate();
                item.Remove();
                EndUpdate();
            }
        }

        public void RemoveDelta(ThermalImageFile.Delta delta)
        {
            ListViewItem item = FindDelta(delta);
            if (item != null)
            {
                BeginUpdate();
                item.Remove();
                EndUpdate();
            }
        }

        public void AddSpot(ThermalImageFile.MarkerSpot markerSpot)
        {
            if (markerSpot == null)
            {
                return;
            }

            ListViewItem lviItem = CreateSpotItem(markerSpot);
            if (lviItem != null)
            {
                Items.Add(lviItem);
            }
        }

        public void UpdateSpot(ThermalImageFile.MarkerSpot markerSpot)
        {
            ListViewItem item = FindMarker(markerSpot);
            if (item != null)
            {
                BeginUpdate();
                UpdateSpotItem(item.Tag as ThermalImageFile.MarkerSpot, item);
                EndUpdate();
            }
        }

        public void AddRectangle(ThermalImageFile.MarkerRectangle markerRectangle)
        {
            if (markerRectangle == null)
            {
                return;
            }

            ListViewItem lviItem = CreateRectangleItem(markerRectangle);
            if (lviItem != null)
            {
                Items.Add(lviItem);
            }
        }

        public void UpdateRectangle(ThermalImageFile.MarkerRectangle markerRectangle)
        {
            ListViewItem item = FindMarker(markerRectangle);
            if (item != null)
            {
                BeginUpdate();
                UpdateRectangleItem(item.Tag as ThermalImageFile.MarkerRectangle, item);
                EndUpdate();
            }
        }

        public void AddEllipse(ThermalImageFile.MarkerEllipse markerEllipse)
        {
            if (markerEllipse == null)
            {
                return;
            }

            ListViewItem lviItem = CreateEllipseItem(markerEllipse);
            if (lviItem != null)
            {
                Items.Add(lviItem);
            }
        }

        public void UpdateEllipse(ThermalImageFile.MarkerEllipse markerEllipse)
        {
            ListViewItem item = FindMarker(markerEllipse);
            if (item != null)
            {
                BeginUpdate();
                UpdateEllipseItem(item.Tag as ThermalImageFile.MarkerEllipse, item);
                EndUpdate();
            }
        }

        public void AddPolygon(ThermalImageFile.MarkerPolygon markerPolygon)
        {
            if (markerPolygon == null)
            {
                return;
            }

            ListViewItem lviItem = CreatePolygonItem(markerPolygon);
            if (lviItem != null)
            {
                Items.Add(lviItem);
            }
        }

        public void UpdatePolygon(ThermalImageFile.MarkerPolygon markerPolygon)
        {
            ListViewItem item = FindMarker(markerPolygon);
            if (item != null)
            {
                BeginUpdate();
                UpdatePolygonItem(item.Tag as ThermalImageFile.MarkerPolygon, item);
                EndUpdate();
            }
        }

        public void AddPolyline(ThermalImageFile.MarkerPolyline markerPolyline)
        {
            if (markerPolyline == null)
            {
                return;
            }

            ListViewItem lviItem = CreatePolylineItem(markerPolyline);
            if (lviItem != null)
            {
                Items.Add(lviItem);
            }
        }

        public void UpdatePolyline(ThermalImageFile.MarkerPolyline markerPolyline)
        {
            ListViewItem item = FindMarker(markerPolyline);
            if (item != null)
            {
                BeginUpdate();
                UpdatePolylineItem(item.Tag as ThermalImageFile.MarkerPolyline, item);
                EndUpdate();
            }
        }

        public void AddDelta(ThermalImageFile.Delta delta)
        {
            if (delta == null)
            {
                return;
            }

            Items.Add(CreateDeltaItem(delta));
        }

        public void UpdateDelta(ThermalImageFile.Delta delta)
        {
            ListViewItem item = FindDelta(delta);
            if (item != null)
            {
                BeginUpdate();
                UpdateDeltaItem(item.Tag as ThermalImageFile.Delta, item);
                EndUpdate();
            }
        }
    }
}
