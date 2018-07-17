using ColonyConcierge.Mobile.Customer.Localization.Resx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class ContentScroll : Behavior<Entry>
	{
		public ContentScroll()
		{
		}

		static BindableProperty ScrollProperty = BindableProperty.Create("Scroll", typeof(ScrollView), typeof(ContentScroll), null);

		static BindableProperty ScrollTargetProperty = BindableProperty.Create("ScrollTarget", typeof(Label), typeof(ContentScroll), null);

		public ScrollView Scroll
		{
			get { return (ScrollView)base.GetValue(ScrollProperty); }
			set { base.SetValue(ScrollProperty, value); }
		}

		public Label ScrollTarget
		{
			get { return (Label)base.GetValue(ScrollTargetProperty); }
			set { base.SetValue(ScrollTargetProperty, value); }
		}

		protected override void OnAttachedTo(Entry bindable)
		{
			bindable.Focused += OnEntryFocus;
		}

		async private void OnEntryFocus(object sender, FocusEventArgs e)
		{
			await this.Scroll.ScrollToAsync(ScrollTarget, ScrollToPosition.End, false);
		}

		protected override void OnDetachingFrom(Entry bindable)
		{
			bindable.Focused -= OnEntryFocus;
		}
	}
}
