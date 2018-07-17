using ColonyConcierge.Client;
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
    class RequiredNumberValidator : Behavior<Entry>
    {

        // Creating BindableProperties with Limited write access: http://iosapi.xamarin.com/index.aspx?link=M%3AXamarin.Forms.BindableObject.SetValue(Xamarin.Forms.BindablePropertyKey%2CSystem.Object) 

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(RequiredValidator), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        static readonly BindablePropertyKey ReasonPropertyKey = BindableProperty.CreateReadOnly("Reason", typeof(string), typeof(RequiredValidator), " ");

        public static readonly BindableProperty ReasonProperty = ReasonPropertyKey.BindableProperty;

		public string Reason
		{
			get { return (string)base.GetValue(ReasonProperty); }
			private set { base.SetValue(ReasonPropertyKey, value); }
		}

        static BindableProperty FieldNameProperty = BindableProperty.Create("FieldName", typeof(string), typeof(RequiredValidator), "Field");

        public string FieldName
        {
            get { return (string)base.GetValue(FieldNameProperty); }
            set { base.SetValue(FieldNameProperty, value); }
        }

        public static BindableProperty MinValueProperty = BindableProperty.Create("MinValue", typeof(int), typeof(RequiredValidator), 0);

		public int MinValue
        {
			get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
			InputCompleted(bindable, EventArgs.Empty);
            bindable.TextChanged += InputCompleted;
        }

        private void InputCompleted(object sender, EventArgs e)
        {
            Entry input = (Entry)sender;
			long number = 0;
			var isNumber = long.TryParse(input.Text, out number);
			IsValid = isNumber && number >= MinValue;
            if (IsValid)
            {
				Reason = string.Format(AppResources.RequiredMessage, FieldName);
            }
            else
            {
                Reason = " ";
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= InputCompleted;
        }
    }
}
