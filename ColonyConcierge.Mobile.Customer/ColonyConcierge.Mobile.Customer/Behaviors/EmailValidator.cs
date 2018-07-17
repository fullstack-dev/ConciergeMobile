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
    class EmailValidator : Behavior<Entry>
    {
        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        // Creating BindableProperties with Limited write access: http://iosapi.xamarin.com/index.aspx?link=M%3AXamarin.Forms.BindableObject.SetValue(Xamarin.Forms.BindablePropertyKey%2CSystem.Object) 

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EmailValidator), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        static readonly BindablePropertyKey ReasonPropertyKey = BindableProperty.CreateReadOnly("Reason", typeof(string), typeof(EmailValidator), " ");

        public static readonly BindableProperty ReasonProperty = ReasonPropertyKey.BindableProperty;

        public string Reason
        {
            get { return (string)base.GetValue(ReasonProperty); }
            private set { base.SetValue(ReasonPropertyKey, value); }
        }



        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += EmailInputCompleted;
        }

        private void EmailInputCompleted(object sender, EventArgs e)
        {
            Entry input = (Entry)sender;
            IsValid = (Regex.IsMatch(input.Text, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            if (IsValid)
			{
				Reason = " ";
            }
            else
            {
                Reason = AppResources.InvalidEmailFormat;
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= EmailInputCompleted;
        }
    }
}
