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
	public class SerivesFacade
	{
		public List<Service> GetAvailableServices(ExtendedAddress extendedAddress)
		{
			if (extendedAddress == null)
			{
				return null;
			}
			var zipCode = extendedAddress.BasicAddress.ZipCode;

			if (!String.IsNullOrEmpty(zipCode))
			{
				var serviceAvailables = Shared.APIs.IServices.GetAvailableServices(zipCode);
				return serviceAvailables;
			}
			else
			{
				return new List<Service>();
			}
		}

		public string CheckAvailableServices(ExtendedAddress extendedAddress, List<Service> serviceAvailables)
		{
			if (extendedAddress == null)
			{
				return null;
			}

			var zipCode = extendedAddress.BasicAddress.ZipCode;
			string selectedZipCode = string.Empty;
			if (!String.IsNullOrEmpty(zipCode))
			{
				if (serviceAvailables == null || serviceAvailables.Count == 0)
				{
					selectedZipCode = string.Empty;
					extendedAddress.BasicAddress.ZipCode = string.Empty;
				}
				else
				{
					selectedZipCode = zipCode;
				}
			}
			return selectedZipCode;
		}

        public async Task<string> CheckAvailableServicesAsync(ExtendedAddress extendedAddress)
        {
            if (extendedAddress == null)
            {
                return null;
            }
            var zipCode = extendedAddress.BasicAddress.ZipCode;
            string selectedZipCode = string.Empty;
            if (!String.IsNullOrEmpty(zipCode))
            {
                var serviceAvailables = await Shared.APIs.IServices.GetAvailableServices_Async(zipCode);
                if (serviceAvailables == null || serviceAvailables.Count == 0)
                {
                    selectedZipCode = string.Empty;
                    extendedAddress.BasicAddress.ZipCode = string.Empty;
                }
                else
                {
                    selectedZipCode = zipCode;
                }
            }
            return selectedZipCode;
        }

        public string CheckAvailableServices(ExtendedAddress extendedAddress)
		{
			if (extendedAddress == null)
			{
				return null;
			}

			var zipCode = extendedAddress.BasicAddress.ZipCode;
			string selectedZipCode = string.Empty;
			if (!String.IsNullOrEmpty(zipCode))
			{
				var serviceAvailables = Shared.APIs.IServices.GetAvailableServices(zipCode);
				if (serviceAvailables == null || serviceAvailables.Count == 0)
				{
					selectedZipCode = string.Empty;
					extendedAddress.BasicAddress.ZipCode = string.Empty;
				}
				else
				{
					selectedZipCode = zipCode;
				}
			}
			return selectedZipCode;
		}
	}
}