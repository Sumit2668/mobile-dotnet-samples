﻿using System;
using Carto.Layers;
using Carto.PackageManager;
using Carto.Routing;
using CoreGraphics;
using Shared;
using Shared.iOS;
using UIKit;

namespace AdvancedMap.iOS
{
	public class OfflineRoutingController : BaseRoutingController
	{
		public override string Name { get { return "Offline Routing"; } }

		public override string Description { get { return "Offline routing with OpenStreetMap data packages"; } }

		internal static string[] packageIds = { "EE-routing", "LV-routing" };

		PackageListener PackageListener { get; set; }

		CartoPackageManager Manager { get; set; }

		public PackageManagerMenu Menu { get; set; }
		MenuButton MenuButton { get; set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Menu = new PackageManagerMenu();

			MenuButton = new MenuButton("icons/icon_more.png", new CGRect(0, 10, 20, 30));
			NavigationItem.RightBarButtonItem = MenuButton;

			Manager = Routing.PackageManager;

			// Create offline routing service connected to package manager
			Routing.Service = new PackageManagerRoutingService(Manager);

			Alert("This sample uses an online map, but downloads routing packages");

			nfloat y = UIApplication.SharedApplication.StatusBarFrame.Height + NavigationController.NavigationBar.Frame.Height;
			Menu.SetFrameWithNavigationBar(y);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			PackageListener = new PackageListener();

			PackageListener.OnPackageCancel += UpdatePackage;
			PackageListener.OnPackageUpdate += UpdatePackage;
			PackageListener.OnPackageStatusChange += UpdatePackage;
			PackageListener.OnPackageFail += UpdatePackage;

			Manager.PackageManagerListener = PackageListener;
			Manager.Start();

			// Fetch list of available packages from server. 
			// Note that this is asynchronous operation 
			// and listener will be notified via onPackageListUpdated when this succeeds.        
			Manager.StartPackageListDownload();

			Menu.List.AddRows(Manager.GetPackages());
			Menu.BackgroundClick += OnMenuBackgroundClicked;
			MenuButton.Click += OnMenuButtonClick;

			Menu.List.ListSource.CellActionButtonClicked += OnCellActionButtonClick;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			PackageListener.OnPackageCancel -= UpdatePackage;
			PackageListener.OnPackageUpdate -= UpdatePackage;
			PackageListener.OnPackageStatusChange -= UpdatePackage;
			PackageListener.OnPackageFail -= UpdatePackage;

			Manager.Stop(true);
			Manager.PackageManagerListener = null;
			Menu.BackgroundClick -= OnMenuBackgroundClicked;
			MenuButton.Click -= OnMenuButtonClick;

			Menu.List.ListSource.CellActionButtonClicked -= OnCellActionButtonClick;
		}

		void OnMenuBackgroundClicked(object sender, EventArgs e)
		{
			Menu.Hide();
		}

		void OnMenuButtonClick(object sender, EventArgs e)
		{
			if (Menu.IsVisible)
			{
				Menu.Hide();
			}
			else {
				Menu.Show();
			}
		}

		protected override void SetBaseLayer()
		{
			AddOnlineBaseLayer(CartoBaseMapStyle.CartoBasemapStyleDefault);
		}


		void UpdatePackage(object sender, PackageEventArgs e)
		{
			UpdatePackage(e.Id);
		}

		void UpdatePackage(object sender, PackageStatusEventArgs e)
		{
			UpdatePackage(e.Id);
		}

		void UpdatePackage(object sender, PackageFailedEventArgs e)
		{
			Alert("Error: " + e.ErrorType);
			UpdatePackage(e.Id);
		}

		void UpdatePackage(string id)
		{
			InvokeOnMainThread(delegate
			{
				Menu.List.Update(Manager, id);
			});
		}

		public void OnCellActionButtonClick(object sender, EventArgs e)
		{
			PackageManagerButton button = (PackageManagerButton)sender;

			if (button.Type == PMButtonType.CancelPackageTasks)
			{
				Manager.CancelPackageTasks(button.PackageId);
			}
			else if (button.Type == PMButtonType.SetPackagePriority)
			{
				Manager.SetPackagePriority(button.PackageId, button.PriorityIndex);
			}
			else if (button.Type == PMButtonType.StartPackageDownload)
			{
				Manager.StartPackageDownload(button.PackageId);
			}
			else if (button.Type == PMButtonType.StartRemovePackage)
			{
				Manager.StartPackageRemove(button.PackageId);
			}
		}
	}
}
