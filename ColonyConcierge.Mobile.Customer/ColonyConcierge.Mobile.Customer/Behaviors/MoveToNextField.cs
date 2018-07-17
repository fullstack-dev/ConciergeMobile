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
	public class MoveToNextField : Behavior<Entry>
	{
		public MoveToNextField()
		{
		}

        // Creating BindableProperties with Limited write access: http://iosapi.xamarin.com/index.aspx?link=M%3AXamarin.Forms.BindableObject.SetValue(Xamarin.Forms.BindablePropertyKey%2CSystem.Object) 

        static BindableProperty MoveToNextProperty = BindableProperty.Create("MoveToNext", typeof(Entry), typeof(MoveToNextField), null);

        public Entry MoveToNext
        {
            get { return (Entry)base.GetValue(MoveToNextProperty); }
            set { base.SetValue(MoveToNextProperty, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
		{
			bindable.Completed += OnCompleted;
		}

		private void OnCompleted(object sender, EventArgs e)
		{
			if (this.MoveToNext != null)
			{
				this.MoveToNext.Focus();
			}
		}

		protected override void OnDetachingFrom(Entry bindable)
		{
			bindable.Completed -= OnCompleted;
		}
	}
}
