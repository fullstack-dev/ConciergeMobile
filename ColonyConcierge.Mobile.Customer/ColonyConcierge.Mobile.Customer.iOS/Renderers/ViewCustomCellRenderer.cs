using System.Drawing;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomCell), typeof(ViewCustomCellRenderer))]
[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCustomCellRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class ViewCustomCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			if (cell != null)
			{
				if (item is CustomCell)
				{
					var customCell = item as CustomCell;

					if (customCell.TransparentHover)
					{
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					}
					else if (customCell.ColorHover != Xamarin.Forms.Color.Transparent)
					{
						UIView selectedView = new UIView(new RectangleF(0, 0, (float)cell.Bounds.Width, (float)cell.Bounds.Height));
						selectedView.BackgroundColor = customCell.ColorHover.ToUIColor();
						cell.SelectedBackgroundView = selectedView;
					}
					else
					{
						UIView selectedView = new UIView(new RectangleF(0, 0, (float)cell.Bounds.Width, (float)cell.Bounds.Height));
						selectedView.BackgroundColor = Appearance.Instance.OrangeColor.ToUIColor();
						cell.SelectedBackgroundView = selectedView;
					}
				}
				else
				{
					UIView selectedView = new UIView(new RectangleF(0, 0, (float)cell.Bounds.Width, (float)cell.Bounds.Height));
					selectedView.BackgroundColor = Appearance.Instance.OrangeColor.ToUIColor();
					cell.SelectedBackgroundView = selectedView;
				}
			}
			return cell;
		}
	}
}
