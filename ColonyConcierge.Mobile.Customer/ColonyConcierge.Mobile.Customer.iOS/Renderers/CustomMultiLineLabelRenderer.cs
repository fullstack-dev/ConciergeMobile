using System;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MultiLineLabel), typeof(CustomMultiLineLabelRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
    public class CustomMultiLineLabelRenderer : LabelRenderer
    {
        public CustomMultiLineLabelRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            MultiLineLabel multiLineLabel = (MultiLineLabel)Element;

            if (multiLineLabel != null && multiLineLabel.Lines != -1)
                Control.Lines = multiLineLabel.Lines;
        }
    }
}
