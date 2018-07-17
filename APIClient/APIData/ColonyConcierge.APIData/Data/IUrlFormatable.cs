using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// This is for internal use by the client wrapper dll. It indicates to the URL builder that a type has its own custom 
    /// data formating method for use in urls.
    /// </summary>
    public interface IUrlFormatable
    {
        string GetUrlSafeString();
    }
}
