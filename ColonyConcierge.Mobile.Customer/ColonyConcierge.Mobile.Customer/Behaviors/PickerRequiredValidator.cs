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
	class PickerRequiredValidator : Behavior<Picker>
    {

        // Creating BindableProperties with Limited write access: http://iosapi.xamarin.com/index.aspx?link=M%3AXamarin.Forms.BindableObject.SetValue(Xamarin.Forms.BindablePropertyKey%2CSystem.Object) 

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(PickerRequiredValidator), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        static readonly BindablePropertyKey ReasonPropertyKey = BindableProperty.CreateReadOnly("Reason", typeof(string), typeof(PickerRequiredValidator), " ");

        public static readonly BindableProperty ReasonProperty = ReasonPropertyKey.BindableProperty;

        public string Reason
        {
            get { return (string)base.GetValue(ReasonProperty); }
            private set { base.SetValue(ReasonPropertyKey, value); }
        }

        static BindableProperty FieldNameProperty = BindableProperty.Create("FieldName", typeof(string), typeof(PickerRequiredValidator), "Field");

        public string FieldName
        {
            get { return (string)base.GetValue(FieldNameProperty); }
            set { base.SetValue(FieldNameProperty, value); }
        }

        public static BindableProperty MinLengthProperty = BindableProperty.Create("MinLength", typeof(int), typeof(PickerRequiredValidator), 0);

        public int MinLength
        {
            get { return (int)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }

        protected override void OnAttachedTo(Picker bindable)
        {
            bindable.SelectedIndexChanged += InputCompleted;
        }

        private void InputCompleted(object sender, EventArgs e)
        {
            Picker input = (Picker)sender;
			IsValid = input.SelectedIndex > -1;
            if (!IsValid)
            {
				Reason = string.Format(AppResources.RequiredMessage, FieldName);
            }
            else
            {
                Reason = " ";
            }
        }

        protected override void OnDetachingFrom(Picker bindable)
        {
            bindable.SelectedIndexChanged -= InputCompleted;
        }
    }
}
