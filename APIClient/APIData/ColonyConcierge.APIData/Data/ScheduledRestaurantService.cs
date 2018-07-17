using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public class ScheduledRestaurantService : ScheduledService
    {

        public bool Delivery { get; set; }


        /// <summary>
        /// The ID of restaurant location
        /// </summary>
        public int RestaurantLocationID { get; set; }


        /// <summary>
        /// This selected slot ID for delivery (or possibly pickup)
        /// </summary>
        public int SlotID { get; set; }

        public IList<ScheduledRMenuItem> Items { get; set; }

        /// <summary>
        /// This property is used only for validation when placing an order. 
        /// Actual computed server-side values will be available via the <see cref="ScheduledRestaurantServiceCharges"/> objects when the 
        /// final price is calculated server side.
        /// </summary>
        public decimal FrontEndSubtotal { get; set; }

        /// <summary>
        /// This property is used only for validation when placing an order. 
        /// Actual computed server-side tax amount(s) will be available via the <see cref="ScheduledRestaurantServiceCharges"/> objects when the 
        /// final price is calculated server side.
        /// </summary>
        public decimal FrontEndTaxes { get; set; }

        /// <summary>
        /// Optional string that describes the restaurant's POS reference.
        /// </summary>
        public string RestaurantPOSOrderID { get; set; }


        public ScheduledRestaurantService()
        {
            Items = new List<ScheduledRMenuItem>();
        }

    }
}


