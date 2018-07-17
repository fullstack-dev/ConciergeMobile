using System;
using System.Collections.Generic;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartItemView : ContentView
	{
		public event EventHandler Clicked;

		public CartItemView()
		{
			InitializeComponent();
			this.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() =>
				{
					if (Clicked != null)
					{
						Clicked(this, EventArgs.Empty);
					}
				})
			});
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			if (this.BindingContext is ScheduledRMenuItem)
			{
				StackLayoutAppliedModifiers.Children.Clear();
				ScheduledRMenuItem scheduledRMenuItem = this.BindingContext as ScheduledRMenuItem;
				LabelSpecialInstructions.IsVisible = !string.IsNullOrEmpty(scheduledRMenuItem.SpecialInstructions);
				var appliedModifiersParent = scheduledRMenuItem.AppliedModifiers
														 .Where(t => t.EstimatedParentModifer == 0)
														 .OrderBy(t => t.ID)
														 .ToList();


				appliedModifiersParent.Sort((x, y) =>
					{
						if (x.Quantity == 0 && y.Quantity == 0)
						{
							return x.ID.CompareTo(y.ID);
						}
						else if (x.Quantity == 0)
						{
							return -1;
						}
						else if (y.Quantity == 0)
						{
							return 1;
						}
						else
						{
							return x.ID.CompareTo(y.ID); ;
						}
					});

				foreach (var appliedModifier in appliedModifiersParent)
				{
                    if (appliedModifier.Quantity != 1 || !appliedModifier.AppliedByDefault)
                    {
                        Label label = new Label();
                        label.FontSize = Device.GetNamedSize(NamedSize.Small, label);
                        label.Text = (appliedModifier.Quantity > 0 ? AppResources.Add : AppResources.Remove) + ": " +
                            appliedModifier.DisplayName;
                        StackLayoutAppliedModifiers.Children.Add(label);

                        var appliedModifiersChild = scheduledRMenuItem.AppliedModifiers
                                                               .Where(t => t.EstimatedParentModifer == appliedModifier.ID)
                                                             .ToList();
                        appliedModifiersChild.Sort((x, y) =>
                        {
                            if (x.Quantity == 0 && y.Quantity == 0)
                            {
                                return x.ID.CompareTo(y.ID);
                            }
                            else if (x.Quantity == 0)
                            {
                                return -1;
                            }
                            else if (y.Quantity == 0)
                            {
                                return 1;
                            }
                            else
                            {
                                return x.ID.CompareTo(y.ID); ;
                            }
                        });

                        foreach (var appliedModifierChild in appliedModifiersChild)
                        {
                            if (appliedModifierChild.Quantity != 1 || !appliedModifierChild.AppliedByDefault)
                            {
                                Label labelChild = new Label();
                                labelChild.FontSize = Device.GetNamedSize(NamedSize.Small, label);
                                if (appliedModifierChild.Quantity > 0)
                                {
                                    labelChild.Text = AppResources.Add + ": " + appliedModifierChild.DisplayName;
                                    if (appliedModifierChild.Quantity > 1)
                                        labelChild.Text += " (" + appliedModifierChild.Quantity.ToString() + ")";
                                }
                                else
                                    labelChild.Text = AppResources.Remove + ": " + appliedModifierChild.DisplayName;

                                labelChild.Margin = new Thickness(10, 0, 0, 0);
                                StackLayoutAppliedModifiers.Children.Add(labelChild);

                            }
                                
                        }
                    }
					
				}
			}
		}
	}
}
