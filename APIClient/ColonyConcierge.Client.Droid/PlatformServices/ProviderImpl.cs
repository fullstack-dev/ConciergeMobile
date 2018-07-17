using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ColonyConcierge.Client.PlatformServices;

namespace ColonyConcierge.Client.Droid.PlatformServices
{
    //TODO, I think this class might be able to be moved to the shared project eventually.
    class ProviderImpl : Provider
    {

        static ProviderImpl()
        {
            Provider.G = new ProviderImpl();
        }


        #region Private Data
        TraceImpl _trace = new TraceImpl();
        FactoryImpl _factory = new FactoryImpl();
        RestImpl _rest = new RestImpl();
        ConfigImpl _config = new ConfigImpl();
        MetadataImpl _metadata = new MetadataImpl();
        #endregion


        public override ITrace Tracer
        {
            get
            {
                return _trace;
            }
        }

        public override IFactory Factory
        {
            get
            {
                return _factory;
            }
        }

        public override IRest Rest
        {
            get
            {
                return _rest;
            }
        }

        public override IConfig Config
        {
            get
            {
                return _config;
            }
        }

        public override IMetadata Metadata
        {
            get
            {
                return _metadata;
            }
        }

        public override void Init()
        {
            base.Init();
        }

        internal static void StaticInit()
        {

        }
    }
}