using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.PlatformServices
{
    public interface IMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyType">some type that lives in the assembly</param>
        /// <returns></returns>
        string GetClientVersion(Type assemblyType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyType">some type that lives in the assembly</param>
        /// <returns></returns>
        DateTime GetClientBuildDate(Type assemblyType);

        
    }
}
