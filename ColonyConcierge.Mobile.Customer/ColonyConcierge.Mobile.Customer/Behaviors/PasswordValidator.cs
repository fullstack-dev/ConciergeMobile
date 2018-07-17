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
	
    public class PasswordValidator : Behavior<Entry>
    {
		const string passwordRegex = @"^(?=.{10,}$)(?=.*[\d?!@#\\$%\\^&\\*,.:;_\)\(])(?=.*[A-Z])(?=.*[a-z]).*$";
		public string password;
        // Creating BindableProperties with Limited write access: http://iosapi.xamarin.com/index.aspx?link=M%3AXamarin.Forms.BindableObject.SetValue(Xamarin.Forms.BindablePropertyKey%2CSystem.Object) 

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(PasswordValidator), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        static readonly BindablePropertyKey ReasonPropertyKey = BindableProperty.CreateReadOnly("Reason", typeof(string), typeof(PasswordValidator), " ");

        public static readonly BindableProperty ReasonProperty = ReasonPropertyKey.BindableProperty;

        public string Reason
        {
            get { return (string)base.GetValue(ReasonProperty); }
            private set { base.SetValue(ReasonPropertyKey, value); }
        }

        static BindableProperty CompareToEntryProperty = BindableProperty.Create("CompareToEntry", typeof(Entry), typeof(PasswordValidator), null);

        public Entry CompareToEntry
        {
            get { return (Entry)base.GetValue(CompareToEntryProperty); }
            set { base.SetValue(CompareToEntryProperty, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += InputCompleted;
        }

		private void InputCompleted(object sender, TextChangedEventArgs e) //EventArgs e
        {
            Entry input = (Entry)sender;
            IsValid = CompareToEntry != null ? true : (Regex.IsMatch(input.Text, passwordRegex));
            if (IsValid)
			{
				IsValid = CompareToEntry != null ? CompareToEntry.Text == input.Text : true;
                if (!IsValid)
                {
                    Reason = AppResources.PasswordMatchError;
                }
                else
                {
                    Reason = " ";
                }
            }
            else
            {
                Reason = AppResources.InvalidPassword;
            }
        }


        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= InputCompleted;
        }
    }
}
