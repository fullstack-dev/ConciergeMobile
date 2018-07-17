using ColonyConcierge.Client.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Windows.PlatformServices
{
    class MetadataImpl : IMetadata
    {
        public DateTime GetClientBuildDate(Type assemblyType)
        {
            DateTime result = MiscUtil.RetrieveLinkerTimestamp(assemblyType.Assembly);

            return result;
        }

        public string GetClientVersion(Type assemblyType)
        {
            return assemblyType.Assembly.GetName().Version.ToString();
        }
    }
}
