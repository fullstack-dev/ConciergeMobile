using ColonyConcierge.Client.Metrics;
using ColonyConcierge.Client.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// Utility class to allow easier access to reusable API objects without having to explicitly instance each API.
    /// This class creates and caches instances of all types in the <see cref="ColonyConcierge.Client.DataSources">ObjectData API</see>
    /// Each instance is tied to a particular <see cref="Connector"/> object, so you may need more than one for admin/end user, etc.
    /// <note>
    /// DO NOT STORE THE INDIVIDUAL CC_API_XX OBJECTS! This class may decided to recreate them if needed,
    /// invalidating the old ones in the process!
    /// </note>
    /// </summary>
    /// <remarks>
    /// This class provides a "one-stop" location for accessing all the "ObjectData" API's without having to instance them yourself.
    /// All of the ObjectData Api object are available as members of this class and are instanced in a lazy way, and are reused by future
    /// usage.
    /// </remarks>
    /// <example>
    /// This example creates an API object from scratch, with login credentials,
    /// and stores the resulting token when the login event occurs.
    /// <code>
    /// var apis = new APIs()
    /// {
    ///     LoginUserName = "foo",
    ///     LoginPassword = "bar",
    ///     LoginToken = Session["logintoken"] as string
    /// };
    /// apis.TokenReady += t => Session.Add("logintoken", t);
    ///
    /// //now use the API 
    ///
    /// var user = apis.IUsers.GetCurrentUser();
    /// </code>
    /// This example creates an API object using an existing <see cref="Connector"/> object.
    /// The connector can be used to actively login, instead of the on-demand login used in the other example.
    /// <code>
    /// Connector connector = new Connector().Login("foo", "bar");
    /// var apis = new APIs(connector);
    /// //now use the API 
    ///
    /// var user = apis.IUsers.GetCurrentUser();
    /// </code>
    /// </example>
    public partial class APIs
    {
        #region Private Data
        Dictionary<Type, BaseDataObject> _apis = new Dictionary<Type, BaseDataObject>();
        Connector _connector;
        string _loginToken;
        string _loginUserName;
        string _loginPassword;
        string _loginRole;
        #endregion

        /// <summary>
        /// An event that get's fired when the connector object obtains a login. 
        /// <note> <b>This event does not get fired if the class was instanced with an existing connector object, or a 
        /// login token was already provided</b>
        /// </note>
        /// </summary>
        /// <example>
        /// This example makes use of <see cref="ObjectDataSourceExt.Setup"/> in conjunction with 
        /// a <see cref="System.Web.UI.WebControls.ObjectDataSource"/>
        /// object to allow for "drag and drop" ASP.Net data binding functionality.
        /// <code>
        ///ObjectDataSource1.Setup((o) =>
        ///{
        /// o.LoginUserName = "test_reg";
        /// o.LoginPassword = "nonpw";
        /// o.LoginToken = Session["logintoken"] as string;
        /// o.TokenReady += (t) =>
        /// {
        ///    //Save the token here to session variable, etc...
        ///    Session.Add("logintoken", t);
        /// };
        ///});
        ///</code>
        /// </example>
        public event TokenReadyEventHandler TokenReady;



        /// <summary>
        /// The Login Token to use for an existing login session. 
        /// </summary>
        /// <remarks>
        /// If this value is set, derived classes, (the 'CC_API_*' classes) will use this token in 
        /// subsequent API calls. See <see cref="TokenReady"/> for an example. If the <see cref="LoginUserName"/> and
        /// <see cref="LoginPassword"/> properties are set, a login event will fire when the credentials 
        /// are successfully applied. 
        /// </remarks>
        public string LoginToken
        {
            get
            {
                return _loginToken;
            }

            set
            {
                ClearCachedObjects();
                _loginToken = value;

            }
        }

        /// <summary>
        /// The username to use when logging in for API calls that require login
        /// </summary>
        public string LoginUserName
        {
            get
            {
                return _loginUserName;
            }

            set
            {
                ClearCachedObjects();
                _loginUserName = value;

            }
        }

        /// <summary>
        /// The password to use when logging in for API calls that require login
        /// </summary>
        public string LoginPassword
        {
            get
            {
                return _loginPassword;
            }
            set
            {
                ClearCachedObjects();
                _loginPassword = value;
            }
        }

        /// <summary>
        /// THIS DOES NOT DEFINE A "LOGIN TYPE" THERE IS NO SUCH THING AS A 'ADMIN LOGIN' VS 'REGULAR LOGIN'
        /// THIS ONLY VERIFIES AT LOGIN TIME THAT THE USER HAS ALL OF THE PERMISSIONS OF THE ROLE
        /// </summary>
        public string LoginRole
        {
            get
            {
                return _loginRole;
            }
            set
            {
                ClearCachedObjects();
                _loginRole = value;
            }

        }

        /// <summary>
        /// Does  weak check to see of any constituent objects have ever successfully logged in.
        /// </summary>
        public bool LoggedIn
        {
            get
            {
                return (_connector != null && _connector.HasLoggedIn) ||
                    _apis.Any(v => v.Value.HasLoggedIn);
            }
        }


        /// <summary>
        /// If set to "true", timings and call counts will be collected
        /// </summary>
        public bool EnableMetrics
        {
            get;
            set;
        }

        public IMetricsSink MetricsSink
        {
            get;
            set;
        }

        public APIs() : this (null)
        {

        }

        /// <summary>
        /// Creates a new instance of an API object, with an optional connector parameter.
        /// </summary>
        /// <param name="connector">The connector object to use for the cached API objects.</param>
        public APIs(Connector connector = null)
        {
            _connector = connector;
            bool enableMetrics;
            if (bool.TryParse(Provider.G.Config["enableRestMetrics"], out enableMetrics) && enableMetrics)
            {
                EnableMetrics = true;
                MetricsSink = MetricsMgr.Instance;
            }
        }





        T GetAPI<T>() where T : BaseDataObject, new()
        {
            lock (this)
            {
                if (_apis.ContainsKey(typeof(T)))
                {
                    return _apis[typeof(T)] as T;
                }
                else
                {
                    T newApiObject = new T();
                    newApiObject._connector = _connector;
                    newApiObject.LoginToken = LoginToken;
                    newApiObject.LoginUserName = LoginUserName;
                    newApiObject.LoginPassword = LoginPassword;
                    newApiObject.LoginRole = LoginRole;
                    newApiObject.EnableMetrics = EnableMetrics;
                    newApiObject.MetricsSink = MetricsSink;
                    newApiObject.TokenReady += t =>
                    {
                        _loginToken = t;
                        if (TokenReady != null)
                        {
                            TokenReady(t);
                        }

                    };
                    _apis.Add(typeof(T), newApiObject);
                    return newApiObject;
                }
            }

        }

        void ClearCachedObjects()
        {
            lock (this)
            {
                if (_apis.Count > 0)
                {
                    foreach (var apiObj in _apis)
                    {
                        apiObj.Value.Dispose();
                    }

                    _apis = new Dictionary<Type, BaseDataObject>();
                }
            }
        }


    }
}
