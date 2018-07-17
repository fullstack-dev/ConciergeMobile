using System;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
    public class MultiLineLabel : Label
    {
        public MultiLineLabel()
        {
        }

        private static int _defaultLineSetting = -1;

        public static readonly BindableProperty LinesProperty = BindableProperty.Create(nameof(Lines), typeof(int), typeof(MultiLineLabel), _defaultLineSetting);
        public int Lines
        {
            get { return (int)GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }
    }
}