using System;
using System.Collections.Generic;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using CoreLocation;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer (typeof(AddressMap), typeof(AddressMapRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class AddressMapRenderer : MapRenderer
	{
		AddressMap AddressMap;
		MKMapView NativeMap;

		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				NativeMap = Control as MKMapView;
				NativeMap.GetViewForAnnotation = null;

				NativeMap.RegionChanged -= NativeMap_RegionChanged;
			}
			if (e.NewElement != null)
			{
				AddressMap = (AddressMap)e.NewElement;
				if (Control is MKMapView)
				{
					NativeMap = Control as MKMapView;
					NativeMap.RegionChanged += NativeMap_RegionChanged;

					if (AddressMap.CurrentPin != null)
					{
						MKPointAnnotation mkPointAnnotation = new MKPointAnnotation();
						mkPointAnnotation.Title = AddressMap.CurrentPin.Pin.Label;
						mkPointAnnotation.Coordinate = new CLLocationCoordinate2D(AddressMap.CurrentPin.Pin.Position.Latitude, AddressMap.CurrentPin.Pin.Position.Longitude);
						NativeMap.AddAnnotation(mkPointAnnotation);
					}
					if (AddressMap.AddressPin != null)
					{
						MKPointAnnotation mkPointAnnotation = new MKPointAnnotation();
						mkPointAnnotation.Coordinate = new CLLocationCoordinate2D(AddressMap.AddressPin.Pin.Position.Latitude, AddressMap.AddressPin.Pin.Position.Longitude);
						NativeMap.AddAnnotation(mkPointAnnotation);

						NativeMap.AddGestureRecognizer(new UILongPressGestureRecognizer((gestureRecognizer) =>
						{
							if (gestureRecognizer.State != UIGestureRecognizerState.Began)
							{
								return;
							}

							var touchPoint = gestureRecognizer.LocationInView(NativeMap);
							CLLocationCoordinate2D touchMapCoordinate = NativeMap.ConvertPoint(touchPoint, NativeMap);
							mkPointAnnotation.Coordinate = touchMapCoordinate;
							AddressMap.AddressPin.Pin.Position = new Xamarin.Forms.Maps.Position(touchMapCoordinate.Latitude, touchMapCoordinate.Longitude);
						})
						{
							MinimumPressDuration = 1
						});
					}
				}

				//nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
				//nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
				//nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
			}
		}

		void NativeMap_RegionChanged(object sender, MKMapViewChangeEventArgs e)
		{
			if (this.Element != null && this.Control != null)
			{
				AddressMap.RaiseOnRegionChanged(new Position(NativeMap.Region.Center.Latitude, NativeMap.Region.Center.Longitude));
			}
		}

	}
}
