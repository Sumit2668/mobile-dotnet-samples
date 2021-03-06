﻿using System;
using System.Threading;
using Carto.DataSources;
using Carto.Layers;
using Carto.Styles;
using Carto.VectorTiles;
using Shared;
using Shared.iOS;

namespace CartoMap.iOS
{
	public class TorqueShipController : MapBaseController
	{
		public override string Name { get { return "Carto Torque Map"; } }

		public override string Description { get { return "Torque tiles of WWII ship movement"; } }

		const long FRAMETIME = 100;

		TorqueTileDecoder decoder;
		TorqueTileLayer tileLayer;

		Timer timer;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AddOnlineBaseLayer(CartoBaseMapStyle.CartoBasemapStyleGray);

			string encoded = JsonUtils.GetTorqueQuery();

			string url = "http://viz2.cartodb.com/api/v2/sql?q=" + encoded + "&cache_policy=persist";

			// Define datasource with the query
			HTTPTileDataSource source = new HTTPTileDataSource(0, 14, url);

			// Create persistent cache to make it faster
			string cacheFile = Utils.GetDocumentDirectory() + "/torque_tile_cache.db";
			TileDataSource cacheSource = new PersistentCacheTileDataSource(source, cacheFile);

			// Create CartoCSS style from Torque points
			CartoCSSStyleSet styleSheet = new CartoCSSStyleSet(JsonUtils.TorqueCartoCSS);

			// Create tile decoder and Torque layer
			decoder = new TorqueTileDecoder(styleSheet);

			tileLayer = new TorqueTileLayer(cacheSource, decoder);

			// Lower priority so it would load the base layer first
			tileLayer.UpdatePriority = -1;

			MapView.Layers.Add(tileLayer);

			MapView.SetZoom(1, 0);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			timer = new Timer(new TimerCallback(UpdateTorque), null, FRAMETIME, FRAMETIME);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			timer.Dispose();
			timer = null;
		}

		void UpdateTorque(object state)
		{
			System.Threading.Tasks.Task.Run(delegate
			{
				int frameNumber = (tileLayer.FrameNr + 1) % decoder.FrameCount;
				tileLayer.FrameNr = frameNumber;
			});
		}
	}
}

