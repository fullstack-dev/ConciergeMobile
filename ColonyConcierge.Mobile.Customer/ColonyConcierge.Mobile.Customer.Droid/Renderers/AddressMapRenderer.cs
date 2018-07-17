using System;
using System.Collections.Generic;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(AddressMap), typeof(AddressMapRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class AddressMapRenderer : MapRenderer, IOnMapReadyCallback
	{
		GoogleMap map;
		AddressMap AddressMap;
		MapView MapView;

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				//map.InfoWindowClick -= OnInfoWindowClick;
			}

			if (e.NewElement != null)
			{
				MapView = (MapView)Control;
				AddressMap = (AddressMap)e.NewElement;
				MapView.GetMapAsync(this);
			}
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			//map.InfoWindowClick += OnInfoWindowClick;
			//map.SetInfoWindowAdapter(this);

			map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(
				new LatLng(AddressMap.Center.Latitude, AddressMap.Center.Longitude), 17f));
			map.CameraChange += (sender, e) =>
			{
				if (map.Projection.VisibleRegion != null)
				{
					AddressMap.RaiseOnRegionChanged(
									new Xamarin.Forms.Maps.Position(map.Projection.VisibleRegion.LatLngBounds.Center.Latitude,
										map.Projection.VisibleRegion.LatLngBounds.Center.Longitude));
				}
			};
			try
			{
				map.UiSettings.MyLocationButtonEnabled = false;
				map.MyLocationEnabled = false;
			}
			catch (Exception)
			{
			}

			if (AddressMap.CurrentPin != null)
			{
				var markerOptions = new MarkerOptions();
				markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.icon_pinblue));
				markerOptions.SetPosition(new LatLng(AddressMap.CurrentPin.Pin.Position.Latitude, AddressMap.CurrentPin.Pin.Position.Longitude));
				map.AddMarker(markerOptions);
			}

			if (AddressMap.AddressPin != null)
			{
				var markerOptions = new MarkerOptions();
				markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker());
				markerOptions.SetPosition(new LatLng(AddressMap.AddressPin.Pin.Position.Latitude, AddressMap.AddressPin.Pin.Position.Longitude));
				var marker = map.AddMarker(markerOptions);

				map.MapLongClick += (sender, e) =>
				{
					marker.Position = new LatLng(e.Point.Latitude, e.Point.Longitude);
					AddressMap.AddressPin.Pin.Position = new Xamarin.Forms.Maps.Position(e.Point.Latitude, e.Point.Longitude);
				};
			}
		}
    }
}
