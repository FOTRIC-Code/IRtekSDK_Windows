using System.Drawing;
using System.Windows.Forms;

using AcousticImage;

namespace AcousticFileReaderDemo
{
    public class MarkersListView : ListView
    {
        private ListViewItem FindMarker(AcousticImageFile.Marker marker)
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

        private ListViewItem CreateSpotItem(AcousticImageFile.MarkerSpot markerSpot)
        {
            if (markerSpot == null)
            {
                return null;
            }

            Point p = markerSpot.Point;

            ListViewItem item = new ListViewItem(markerSpot.Name);
            item.SubItems.Add((markerSpot.MaxInternal).ToString("F4"));
            item.SubItems.Add("---");
            item.SubItems.Add(p.X.ToString());
            item.SubItems.Add(p.Y.ToString());
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.SubItems.Add("---");
            item.Tag = markerSpot;

            return item;
        }

        private void UpdateSpotItem(AcousticImageFile.MarkerSpot markerSpot, ListViewItem item)
        {
            if (markerSpot == null)
            {
                return;
            }

            Point p = markerSpot.Point;

            item.Text = markerSpot.Name;
            int index = 1;
            item.SubItems[index++].Text = (markerSpot.Value).ToString("F4");
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = p.X.ToString();
            item.SubItems[index++].Text = p.Y.ToString();
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = "---";
            item.Tag = markerSpot;
        }

        private ListViewItem CreateEllipseItem(AcousticImageFile.MarkerEllipse markerEllipse)
        {
            if (markerEllipse == null)
            {
                return null;
            }

            Rectangle rectangle = markerEllipse.Rectangle;

            ListViewItem item = new ListViewItem(markerEllipse.Name);
            item.SubItems.Add("---");
            item.SubItems.Add(markerEllipse.Max.ToString("F4"));
            item.SubItems.Add(rectangle.X.ToString());
            item.SubItems.Add(rectangle.Y.ToString());
            item.SubItems.Add(rectangle.Right.ToString());
            item.SubItems.Add(rectangle.Bottom.ToString());
            item.SubItems.Add(markerEllipse.Area.ToString("F4") + "m^2");
            item.Tag = markerEllipse;

            return item;
        }

        private void UpdateEllipseItem(AcousticImageFile.MarkerEllipse markerEllipse, ListViewItem item)
        {
            if (markerEllipse == null)
            {
                return;
            }

            Rectangle rectangle = markerEllipse.Rectangle;

            item.Text = markerEllipse.Name;
            int index = 1;
            item.SubItems[index++].Text = "---";
            item.SubItems[index++].Text = markerEllipse.Max.ToString("F4");
            item.SubItems[index++].Text = rectangle.X.ToString();
            item.SubItems[index++].Text = rectangle.Y.ToString();
            item.SubItems[index++].Text = rectangle.Right.ToString();
            item.SubItems[index++].Text = rectangle.Bottom.ToString();
            item.SubItems[index++].Text = markerEllipse.Area.ToString("F4") + "m^2";
            item.Tag = markerEllipse;
        }

        public void RemoveMarker(AcousticImageFile.Marker marker)
        {
            ListViewItem item = FindMarker(marker);
            if (item != null)
            {
                BeginUpdate();
                item.Remove();
                EndUpdate();
            }
        }

        public void AddSpot(AcousticImageFile.MarkerSpot markerSpot)
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

        public void UpdateSpot(AcousticImageFile.MarkerSpot markerSpot)
        {
            ListViewItem item = FindMarker(markerSpot);
            if (item != null)
            {
                BeginUpdate();
                UpdateSpotItem(item.Tag as AcousticImageFile.MarkerSpot, item);
                EndUpdate();
            }
        }
       
        public void AddEllipse(AcousticImageFile.MarkerEllipse markerEllipse)
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

        public void UpdateEllipse(AcousticImageFile.MarkerEllipse markerEllipse)
        {
            ListViewItem item = FindMarker(markerEllipse);
            if (item != null)
            {
                BeginUpdate();
                UpdateEllipseItem(item.Tag as AcousticImageFile.MarkerEllipse, item);
                EndUpdate();
            }
        }
    }
}
