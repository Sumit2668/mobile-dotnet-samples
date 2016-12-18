using System;
namespace CartoMap.iOS
{
	public class CountriesVisController : BaseVisController
	{
		public override string Name { get { return "Countries Vis"; } }

		public override string Description { get { return "Vis displaying countries in different colors using UTFGrid"; } }

		protected override string Url
		{
			get
			{
//				return "http://documentation.cartodb.com/api/v2/viz/2b13c956-e7c1-11e2-806b-5404a6a683d5/viz.json";
				return "https://mierune2016team.carto.com/api/v2/viz/8a9450a2-6456-11e6-8a7a-0ee66e2c9693/viz.json";
			}
		}
	}
}

