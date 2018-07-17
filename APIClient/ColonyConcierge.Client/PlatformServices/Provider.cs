using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.PlatformServices
{
    public abstract class Provider
    {

        public static Provider G
        {
            get;
            protected set;
        }

        protected Provider()
        {

        }


        public abstract ITrace Tracer
        {
            get;
        }

        public abstract IFactory Factory
        {
            get;
        }

        public abstract IRest Rest
        {
            get;
        }

        public abstract IConfig Config
        {
            get;
        }

        public abstract IMetadata Metadata
        {
            get;
        }


        public virtual void Init()
        {

        }



    }
}
