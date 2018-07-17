using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// The required base class of all REST objects that can be in an inheritance hierarchy
    /// </summary>
    /// <remarks>
    /// This is needed by WCF to properly deserialize objects that can inherit from other classes.
    /// </remarks>
    public abstract class InheritedTypeBase
    {
        /// <summary>
        /// For Internal use only!
        /// </summary>
        public virtual string __type
        {
            get
            {
                var type = GetType();
                var name = type.Name;
                var @namespace = type.Namespace;
                var result = string.Format("{0}:#{1}", name, @namespace);
                return result;
            }

            set
            {
                //ignored... 
            }

        }
    }
}
