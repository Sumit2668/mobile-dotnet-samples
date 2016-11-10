﻿
using System;
using Android.App;
using Carto.Layers;
using Shared.Droid;

namespace AdvancedMap.Droid
{
	[Activity]
	public class PackagedMapActivity : MapBaseActivity
	{
		protected override void OnCreate(Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			AddBaseLayer(CartoBaseMapStyle.CartoBasemapStyleDefault);
		}

	}
}
