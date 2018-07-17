using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// EXPERIMENTAL class to present pre-calculated price data to clients, so that they do not have to be aware of pricing models and logic.
    /// All of this data is calculated "on the fly" from other data.
    /// <note type="note">THIS IS NOT ACTUALLY IMPLEMENTED ANYWHERE, AND MIGHT NOT BE FOR A LONG TIME.</note>
    /// </summary>
    public class RMenuItemPriceTable
    {
        int MenuItemID { get; set; }

        int NumModifiers { get; set; }

        int MaxQty { get; set; }

        List<int> ModiferIDs { get; set; }

        List<decimal?> Prices { get; set; }
        
    }

}
