﻿
using System;
using System.Collections.Generic;
using UIKit;

namespace CartoMobileSample
{
	public class PackageListDataSource : UITableViewSource
	{
		public EventHandler<EventArgs> CellActionButtonClicked;

		static nfloat ROWHEIGHT = 60;

		const string identifier = "PackageListCell";

		public List<Package> Items = new List<Package>();

		public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return ROWHEIGHT;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Items.Count;
		}

		public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			CoreGraphics.CGRect selectedArea = tableView.RectForRowAtIndexPath(indexPath);
			Console.WriteLine(selectedArea);

			base.RowSelected(tableView, indexPath);
		}

		public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			Package package = Items[indexPath.Row];

			PackageListCell cell = (PackageListCell)tableView.DequeueReusableCell(identifier);

			if (cell == null)
			{
				cell = new PackageListCell();
			}

			cell.Update(package);

			// Always detach and reattach handlers to prevent multiple handlers, memory leaks
			cell.CellActionButtonClicked -= OnActionButtonClick;
			cell.CellActionButtonClicked += OnActionButtonClick;

			return cell;
		}

		void OnActionButtonClick(object sender, EventArgs e)
		{
			if (CellActionButtonClicked != null)
			{
				CellActionButtonClicked(sender, e);
			}
		}
	}
}

