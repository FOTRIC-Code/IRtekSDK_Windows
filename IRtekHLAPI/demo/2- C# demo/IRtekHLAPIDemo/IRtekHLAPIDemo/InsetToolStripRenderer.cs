using System.Drawing;
using System.Windows.Forms;

namespace IRtekHLAPIDemo
{
    /// <summary>
    /// Renders text ToolStripButtons with a bottom inset so hover/pressed borders
    /// are not clipped by the toolbar edge on tall toolbars.
    /// </summary>
    internal sealed class InsetToolStripRenderer : ToolStripProfessionalRenderer
    {
        private const int BottomInset = 3;

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            var button = e.Item as ToolStripButton;
            if (button == null || button.DisplayStyle != ToolStripItemDisplayStyle.Text)
            {
                base.OnRenderButtonBackground(e);
                return;
            }

            if (!button.Selected && !button.Pressed && !button.Checked)
                return;

            Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
            bounds.Inflate(-1, -1);
            bounds.Height -= BottomInset;
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            Color backColor = button.Pressed
                ? ProfessionalColors.ButtonPressedGradientMiddle
                : ProfessionalColors.ButtonSelectedGradientMiddle;

            using (var brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, bounds);
            }

            Color borderColor = button.Pressed
                ? ProfessionalColors.ButtonPressedBorder
                : ProfessionalColors.ButtonSelectedBorder;

            using (var pen = new Pen(borderColor))
            {
                e.Graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }
    }
}
