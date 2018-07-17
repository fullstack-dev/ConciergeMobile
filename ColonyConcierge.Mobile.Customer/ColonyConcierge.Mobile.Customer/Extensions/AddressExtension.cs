using System;
using System.Text;
using ColonyConcierge.APIData.Data;

namespace ColonyConcierge.Mobile.Customer
{
	public static class AddressExtension
	{
		public static string ToAddressWithoutZip(this Address address)
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(address.Line1))
			{
				sb.AppendLine(address.Line1 + " ");
			}

			if (!string.IsNullOrEmpty(address.City))
			{
				sb.Append(address.City + ", ");
			}

			if (!string.IsNullOrEmpty(address.StateProv))
			{
				sb.Append(address.StateProv);
			}

			//if (!string.IsNullOrEmpty(address.ZipCode))
			//{
			//	sb.Append(" " + address.ZipCode);
			//}

			return sb.ToString();
		}

		public static string ToAddressLine(this Address address)
		{
			return address.ToAddressLine1().Replace("\n", " ") + "\n" + address.ToAddressLine2().Replace("\n", " ");
		}

		public static string ToAddressLine1(this Address address)
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(address.Line1))
			{
				sb.Append(address.Line1 + " ");
			}

			if (!string.IsNullOrEmpty(address.Line2))
			{
				sb.Append(address.Line2);
			}

			//if (!string.IsNullOrEmpty(address.Line3))
			//{
			//	sb.AppendLine(address.Line3);
			//}

			return sb.ToString();
		}

		public static string ToAddressLine2(this Address address)
		{
			var sb = new StringBuilder();

			if (!string.IsNullOrEmpty(address.City))
			{
				sb.Append(address.City + ", ");
			}

			if (!string.IsNullOrEmpty(address.StateProv))
			{
				sb.Append(address.StateProv + " ");
			}

			if (!string.IsNullOrEmpty(address.ZipCode))
			{
				sb.Append(address.ZipCode);
			}

			//if (!string.IsNullOrEmpty(address.Country))
			//{
			//	sb.AppendLine("");
			//	sb.Append(address.Country);
			//}

			return sb.ToString();
		}

		public static string ToAddress(this Address address)
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(address.Line1))
			{
				sb.AppendLine(address.Line1 + " ");
			}

			if (!string.IsNullOrEmpty(address.City))
			{
				sb.Append(address.City + ", ");
			}

			if (!string.IsNullOrEmpty(address.StateProv))
			{
				sb.Append(address.StateProv + ", ");
			}

			if (!string.IsNullOrEmpty(address.ZipCode))
			{
				sb.Append(" " + address.ZipCode);
			}

			return sb.ToString();
		}
	}
}
