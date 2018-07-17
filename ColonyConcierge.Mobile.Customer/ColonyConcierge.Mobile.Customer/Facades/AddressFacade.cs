using System;
using TK.CustomMap.Api.Google;
using System.Linq;
using ColonyConcierge.APIData.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.Client;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using TK.CustomMap.Api;
using Xamarin.Forms.Maps;
using System.Net.Http;
using PCLAppConfig;
using Newtonsoft.Json;

namespace ColonyConcierge.Mobile.Customer
{
	public class AddressFacade
	{
		private TimeSpan HttpTimeout = TimeSpan.FromSeconds(10);

		public AddressFacade()
		{
		}

		public bool CheckLocalLocation()
		{
			return CheckLocation(Shared.LocalAddress);
		}

		public bool CheckLocation(ExtendedAddress serviceAddress)
		{
			if (serviceAddress != null)
			{
				return serviceAddress.Latitude != 0 || serviceAddress.Longitude != 0;
			}
			return false;
		}

		public ExtendedAddress GetUserAddress(bool isLocal = true)
		{
			ExtendedAddress serviceAddress = null;
			if (isLocal)
			{
				serviceAddress = Shared.LocalAddress;
			}
			if (serviceAddress == null && Shared.IsLoggedIn)
			{
				var serviceAddresses = Shared.APIs.IUsers.GetServiceAddresses(Shared.UserId);
				if (serviceAddresses.Count > 0)
				{
					serviceAddress = serviceAddresses.FirstOrDefault((t) => t.IsPreferred);
					if (serviceAddress == null)
					{
						serviceAddress = serviceAddresses[0];
					}
				}
				if (isLocal)
				{
					Shared.LocalAddress = serviceAddress;
				}
			}
			return serviceAddress;
		}

		public bool ValidateAddress(ExtendedAddress extendedAddress)
		{
			if (extendedAddress== null
				|| extendedAddress.BasicAddress == null
				|| string.IsNullOrEmpty(extendedAddress.BasicAddress.Line1))
			{
				return false;
			}
			return true;
		}

		public ExtendedAddress GetExtendedAddress(GmsDetailsResultItem gmsDetailsResult)
		{
			var extendedAddress = new ExtendedAddress();
			FillExtendedAddress(extendedAddress, gmsDetailsResult);
			return extendedAddress;
		}

		public string GetStreetAddress(GmsDetailsResultItem gmsDetailsResult)
		{
			if (gmsDetailsResult != null && gmsDetailsResult.AddressComponents != null)
			{
				foreach (var component in gmsDetailsResult.AddressComponents)
				{
					if (component.Types.Contains("street_number"))
					{
						if (!string.IsNullOrEmpty(component.ShortName))
						{
							return component.ShortName;
						}
						else if (!string.IsNullOrEmpty(component.LongName))
						{
							return component.LongName;
						}
					}
				}
			}
			return string.Empty;
		}

		public void FillExtendedAddress(ExtendedAddress extendedAddress, GmsDetailsResultItem gmsDetailsResult)
		{
			extendedAddress.BasicAddress = new Address { };

			if (gmsDetailsResult == null)
			{
				return;
			}

			//registrationEntry.ServiceAddress = new ExtendedAddress() { BasicAddress = new Address() };
			extendedAddress.Longitude = Convert.ToDecimal(gmsDetailsResult.Geometry.Location.Longitude);
			extendedAddress.Latitude = Convert.ToDecimal(gmsDetailsResult.Geometry.Location.Latitude);
			string route = string.Empty;

			foreach (var component in gmsDetailsResult.AddressComponents)
			{
				if (component.Types.Contains("postal_code"))
				{
					extendedAddress.BasicAddress.ZipCode = component.LongName;
				}
				if (component.Types.Contains("country"))
				{
					extendedAddress.BasicAddress.Country = component.LongName;
				}
				if (component.Types.Contains("administrative_area_level_1"))
				{
					extendedAddress.BasicAddress.StateProv = component.ShortName;
				}
				if (component.Types.Contains("locality"))
				{
					extendedAddress.BasicAddress.City = component.LongName;
				}
				if (component.Types.Contains("route"))
				{
					route = component.ShortName;
				}
				if (component.Types.Contains("neighborhood"))
				{
					extendedAddress.BasicAddress.Line3 = component.LongName;
				}
				if (component.Types.Contains("street_number"))
				{
					if (!string.IsNullOrEmpty(component.ShortName))
					{
						extendedAddress.BasicAddress.Line1 = component.ShortName;
					}
					else if (!string.IsNullOrEmpty(component.LongName))
					{
						extendedAddress.BasicAddress.Line1 = component.LongName;
					}
				}
			}

			if (!string.IsNullOrEmpty(route) && !string.IsNullOrEmpty(extendedAddress.BasicAddress.Line1))
			{
				extendedAddress.BasicAddress.Line1 += " ";
				extendedAddress.BasicAddress.Line1 += route;
			}
		}

		public Task FillServiceAddress(ExtendedAddress extendedAddress, Prediction prediction)
		{
			//var url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?place_id={0}&key={1}", prediction.PlaceId, string.Empty);
			//HttpClient _httpClient = new HttpClient();
			//var response = await _httpClient.GetAsync(url);
			//var content = await response.Content.ReadAsStringAsync();
			//var geoResult = JsonConvert.DeserializeObject<GeoResult>(content);
			//if (geoResult != null && geoResult.GmsDetailsResultItems.Count > 0)
			//{
			//	return geoResult.GmsDetailsResultItems.FirstOrDefault();
			//}

			return GetGmsDetails(prediction.Description).ContinueWith((arg) =>
			{
				FillExtendedAddress(extendedAddress, arg.Result);
			});

			//return GmsPlace.Instance.GetDetails(prediction.PlaceId).ContinueWith((arg) =>
			//{
			//	FillExtendedAddress(extendedAddress, arg.Result.Item);
			//});
		}

		public async Task<GmsDetailsResultItem> GetGmsDetails(string address)
		{
			var url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}", address, string.Empty);
			HttpClient _httpClient = new HttpClient();
			_httpClient.Timeout = HttpTimeout;
			var response = await _httpClient.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();
			var geoResult = JsonConvert.DeserializeObject<GeoResult>(content);
			if (geoResult != null && geoResult.GmsDetailsResultItems.Count > 0)
			{
				return geoResult.GmsDetailsResultItems.FirstOrDefault();
			}

			return null;
		}

		public async Task<GmsDetailsResultItem> GetGmsDetails(double latitude, double longitude)
		{
			var url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key={2}", latitude, longitude, string.Empty);
			HttpClient _httpClient = new HttpClient();
			_httpClient.Timeout = HttpTimeout;
			var response = await _httpClient.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();
			var geoResult = JsonConvert.DeserializeObject<GeoResult>(content);
			if (geoResult != null && geoResult.GmsDetailsResultItems.Count > 0)
			{
				return geoResult.GmsDetailsResultItems.FirstOrDefault();
			}

			return null;
		}

		public async Task<ObservableCollection<Prediction>> SearchPlaces(string searchText)
		{
			ObservableCollection<Prediction> places = new ObservableCollection<Prediction>();
			try
			{
				if (string.IsNullOrEmpty(searchText))
				{
					return places;
				}

				List<Prediction> result = new List<Prediction>();
				HttpResponseMessage response = null;
				try
				{
					var url = string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&components=country:us&key={1}", searchText, ConfigurationManager.AppSettings["PlacesApiKey"]);
					HttpClient _httpClient = new HttpClient();
					_httpClient.Timeout = HttpTimeout;
					response = await _httpClient.GetAsync(url).ConfigureAwait(false);
				}
				catch (Exception)
				{
					return null;
				}
				if (response == null)
				{
					return null;
				}

				var content = await response.Content.ReadAsStringAsync();
				var placeResult = JsonConvert.DeserializeObject<PlaceResult>(content);
				if (placeResult != null)
				{
					foreach (var predictions in placeResult.Predictions)
					{
						predictions.SearchText = searchText.Trim();
					}
					result = placeResult.Predictions;
					//result.Sort((x, y) => y.MainTextMatch.Length.CompareTo(x.MainTextMatch.Length));
				}

				//var gmsresult = await GmsPlace.Instance.GetPredictions(searchText + "&components=country:us");
				//if (gmsresult != null)
				//{
				//	result = gmsresult.Predictions.ToList();
				//}

				if (result != null && result.Any())
				{
					places = new ObservableCollection<Prediction>(result);
				}
				else
				{
					return places;
				}
			}
			catch (Exception)
			{
				return places;
			}
			return places;
		}

		public Dictionary<string, string> GetStates()
		{
			return new Dictionary<string, string>
			{
				{"Alabama", "AL"},
				{"Alaska", "AK"},
				{"American Samoa", "AS"},
				{"Arizona", "AZ"},
				{"Arkansas", "AR"},
				{"California", "CA"},
				{"Colorado", "CO"},
				{"Connecticut", "CT"},
				{"Delaware", "DE"},
				{"Dist. of Columbia", "DC"},
				{"Florida", "FL"},
				{"Georgia", "GA"},
				{"Guam", "GU"},
				{"Hawaii", "HI"},
				{"Idaho", "ID"},
				{"Illinois", "IL"},
				{"Indiana", "IN"},
				{"Iowa", "IA"},
				{"Kansas", "KS"},
				{"Kentucky", "KY"},
				{"Louisiana", "LA"},
				{"Maine", "ME"},
				{"Maryland", "MD"},
				{"Marshall Islands", "MH"},
				{"Massachusetts", "MA"},
				{"Michigan", "MI"},
				{"Micronesia", "FM"},
				{"Minnesota", "MN"},
				{"Mississippi", "MS"},
				{"Missouri", "MO"},
				{"Montana", "MT"},
				{"Nebraska", "NE"},
				{"Nevada", "NV"},
				{"New Hampshire", "NH"},
				{"New Jersey", "NJ"},
				{"New Mexico", "NM"},
				{"New York", "NY"},
				{"North Carolina", "NC"},
				{"North Dakota", "ND"},
				{"Northern Marianas", "MP"},
				{"Ohio", "OH"},
				{"Oklahoma", "OK"},
				{"Oregon", "OR"},
				{"Palau", "PW"},
				{"Pennsylvania", "PA"},
				{"Puerto Rico", "PR"},
				{"Rhode Island", "RI"},
				{"South Carolina", "SC"},
				{"South Dakota", "SD"},
				{"Tennessee", "TN"},
				{"Texas", "TX"},
				{"Utah", "UT"},
				{"Vermont", "VT"},
				{"Virginia", "VA"},
				{"Virgin Islands", "VI"},
				{"Washington", "WA"},
				{"West Virginia", "WV"},
				{"Wisconsin", "WI"},
				{"Wyoming", "WY"},
			};
		}
	}

	public class PlaceResult
	{
		[JsonProperty("predictions")]
		public List<Prediction> Predictions
		{
			get;
			set;
		} = new List<Prediction>();
	}

	public class Prediction
	{
		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("place_id")]
		public string PlaceId
		{
			get;
			set;
		}

		[JsonProperty("reference")]
		public string Reference
		{
			get;
			set;
		}

		[JsonProperty("structured_formatting")]
		public StructuredFormatting StructuredFormatting
		{
			get;
			set;
		}

		public string SearchText
		{
			get;
			set;
		}

		public string MainTextLeft
		{
			get
			{
				try
				{
					if (!string.IsNullOrEmpty(SearchText))
					{
						var text = StructuredFormatting.MainText;
						var index = text.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase);
						if (index >= 0 && StructuredFormatting.MainText.Length > index)
						{
							return StructuredFormatting.MainText.Substring(0, index);
						}
						else
						{
							index = SearchText.IndexOf(text, StringComparison.CurrentCultureIgnoreCase);
							if (index == 0)
							{
								return string.Empty;
							}
						}
					}
				}
				catch (Exception)
				{
					return StructuredFormatting.MainText;
				}
				return StructuredFormatting.MainText;
			}
		}

		public string MainTextRight
		{
			get
			{
				try
				{
					if (!string.IsNullOrEmpty(SearchText))
					{
						var text = StructuredFormatting.MainText;
						var index = text.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase);
						if (index >= 0 && StructuredFormatting.MainText.Length > index + SearchText.Length)
						{
							return StructuredFormatting.MainText.Substring(index + SearchText.Length);
						}
					}
				}
				catch (Exception)
				{
					return string.Empty;
				}
				return string.Empty;
			}
		}

		public string MainTextMatch
		{
			get
			{
				try
				{
					if (!string.IsNullOrEmpty(SearchText))
					{
						var text = StructuredFormatting.MainText;
						var index = text.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase);
						if (index >= 0 && StructuredFormatting.MainText.Length > index)
						{
							return StructuredFormatting.MainText.Substring(index, SearchText.Length);
						}
						else 
						{
							index = SearchText.IndexOf(text, StringComparison.CurrentCultureIgnoreCase);
							if (index == 0)
							{
								return text;
							}
						}
					}
				}
				catch (Exception)
				{
					return string.Empty;
				}
				return string.Empty;
			}
		}
	}

	public class StructuredFormatting
	{
		[JsonProperty("main_text")]
		public string MainText
		{
			get;
			set;
		}

		[JsonProperty("secondary_text")]
		public string SecondaryText
		{
			get;
			set;
		}
	}

	public class GeoResult
	{
		[JsonProperty("results")]
		public List<GmsDetailsResultItem> GmsDetailsResultItems
		{
			get;
			set;
		}
	}
}