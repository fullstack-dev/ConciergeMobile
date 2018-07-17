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
    class RequiredValidator : Behavior<Entry>
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

        public static BindableProperty MinLengthProperty = BindableProperty.Create("MinLength", typeof(int), typeof(RequiredValidator), 0);

		public int MinLength
		{
			get { return (int)GetValue(MinLengthProperty); }
			set { SetValue(MinLengthProperty, value); }
		}

		public static BindableProperty RemoveSpaceProperty = BindableProperty.Create("RemoveSpace", typeof(bool), typeof(RequiredValidator), false);
		public bool RemoveSpace
		{
			get { return (bool)GetValue(RemoveSpaceProperty); }
			set { SetValue(RemoveSpaceProperty, value); }
		}
		public RequiredValidator()
		{
			MinLength = 1;
		}

		protected override void OnAttachedTo(Entry bindable)
        {
			var length = string.IsNullOrEmpty(bindable.Text) ? 0 : bindable.Text.Length;
			IsValid = length >= MinLength;
			this.Reason = " ";
			bindable.TextChanged += InputCompleted;
        }

        private void InputCompleted(object sender, EventArgs e)
        {
            Entry input = (Entry)sender;
			var text = input.Text;
			if (!string.IsNullOrEmpty(text) && RemoveSpace)
			{
				text = text.Replace(" ", "");
			}
			var length = string.IsNullOrEmpty(text) ? 0 : text.Length;
            IsValid = length >= MinLength;
            if (!IsValid)
            {
				if (MinLength > 1)
				{
					Reason = MinLength > 0 ? string.Format(AppResources.MinLength, FieldName, MinLength.ToString()) : string.Format(AppResources.RequiredMessage, FieldName);
				}
				else 
				{
					Reason = string.Format(AppResources.RequiredMessage, FieldName);
				}
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
