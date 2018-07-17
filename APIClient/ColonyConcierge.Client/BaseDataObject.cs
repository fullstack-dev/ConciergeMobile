using ColonyConcierge.Client.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    /// <summary>
    /// A delegate definition to receive login tokens.
    /// </summary>
    /// <param name="loginToken">The returned login token from a successful login event.</param>
    public delegate void TokenReadyEventHandler(string loginToken);

    /// <summary>
    /// This class provides basic common functionality for the <see cref="ColonyConcierge.Client.DataSources">ObjectData API</see> classes. 
    /// </summary>
    public class BaseDataObject : IDisposable
    {


        #region ProtectedData
        internal protected Connector _connector;
        //internal protected IMetricsSink _sink;
        //internal protected bool _enameMetrics;
        #endregion

        #region Private Data
        bool _isDisposed;
        #endregion


        protected BaseDataObject(Connector connector = null)
        {
            _connector = connector;
        }

       
        /// <summary>
        /// An event that get's fired when the connector object obtains a login.
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
            get;
            internal set;
        }

        /// <summary>
        /// The username to use when logging in for API calls that require login
        /// </summary>
        public string LoginUserName
        {
            get;
            internal set;
        }

        /// <summary>
        /// The password to use when logging in for API calls that require login
        /// </summary>
        public string LoginPassword
        {
            get;
            internal set;
        }

        /// <summary>
        /// The role to pass to the login API. 
        /// This only validates that the user has all of the permissions specified by the role
        /// This does not do anything special with respect to the login token!
        /// </summary>
        public string LoginRole
        {
            get;
            internal set;
        }

        /// <summary>
        /// This returns true if this API object has access to a connector that reports
        /// it has successfully logged in. This is a lightweight property that does not 
        /// communicate with the server to see of the login token is still valid. For that use <see cref="VerifyLogin()"/>
        /// </summary>
        public bool HasLoggedIn
        {

            get
            {
                return _connector != null && _connector.HasLoggedIn;
            }
        }

        internal IMetricsSink MetricsSink
        {
            get; set;
        }

        internal bool EnableMetrics
        {
            get;
            set;
        }

        

        /// <summary>
        /// This returns true if this API has access to a connector that reports it can 
        /// verify a login token. This is a more heavyweight function that communicated with the server 
        /// to verify that the token is still valid. If you want a more lightweight function that simply
        /// checks if a login has occurred at some point in the past, use <see cref="HasLoggedIn"/>
        /// </summary>
        /// <returns>'true', if the connector object exists, has a login token and 
        /// can verify its token is valid on the server, 'false' otherwise.</returns>
        public bool VerifyLogin()
        {
            return _connector != null && _connector.VerifyLogin();
        }


        public void Dispose()
        {
            _isDisposed = true;
            //This object does not "own" it's connector, so just release it.
            _connector = null;

            
        }

        /// <summary>
        /// Called by auto-generated wrappers in the "ObjectData" API wrappers. Allows for "lazy login" scenarios.
        /// </summary>
        /// <returns>'true', if the connector object logged in, or was previously logged in, 'false' otherwise.</returns>
        /// <remarks>
        /// 
        /// </remarks>
        protected bool EnsureLogin()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name, "A call was made on a disposed object!");
            }
            if (_connector == null || _connector.HasLoggedIn == false)
            {
                if (!string.IsNullOrWhiteSpace(LoginToken))
                {
                    _connector = new Connector(LoginToken);
                }
                else
                {
                    _connector = _connector ?? new Connector();
                    if (!string.IsNullOrWhiteSpace(LoginUserName) && !string.IsNullOrWhiteSpace(LoginPassword))
                    {
                        if (!string.IsNullOrWhiteSpace(LoginRole))
                        {
                            _connector.Login(LoginUserName, LoginPassword, LoginRole);
                        }
                        else
                        {
                            _connector.Login(LoginUserName, LoginPassword);
                        }
                        LoginToken = _connector.LoginToken;
                        if (_connector.HasLoggedIn && TokenReady != null)
                        {
                            TokenReady( _connector.LoginToken);
                        }
                    }
                }

            }
            return _connector.HasLoggedIn;
        }



    }
}
