using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace ColonyConcierge.Client
{
    public static class ObjectDataSourceExt
    {
        /// <summary>
        /// Call this to setup your data source object.
        /// </summary>
        /// <param name="source">a <see cref="System.Web.UI.WebControls.ObjectDataSource"/> object</param>
        /// <param name="onload">A lambda or delegate that receives your underlying data object as soon as ASP.net creates it.
        /// Inside your lambda is where you can initialize the login information or tokens</param>
        public static void Setup(this ObjectDataSource source, Action<BaseDataObject> onload )
        {
            source.ObjectCreated += (s, e) =>
                {
                    BaseDataObject dbo = e.ObjectInstance as BaseDataObject;
                    if (dbo != null)
                    {
                        onload(dbo);
                    }
                };
        }

    }
}
