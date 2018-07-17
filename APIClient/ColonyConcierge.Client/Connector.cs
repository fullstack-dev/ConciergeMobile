using ColonyConcierge.APIData.Data;
using ColonyConcierge.APIData.Metadata;
using ColonyConcierge.Client.PlatformServices;
using ColonyConcierge.Client.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{

    /// <summary>
    /// An event class for tracking connector activity by clients.
    /// </summary>
    public class ConnectorEventArgs : EventArgs
    {
        /// <summary>
        /// The url of the endpoint that was called..
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// The raw result of a call
        /// </summary>
        public object RawResult { get; set; }
    }

    public delegate void ConnectorEventHandler(object sender, ConnectorEventArgs eventArgs);


    /// <summary>
    /// This is the main utility class for connecting to the Concierge Service API. A connector is typically associate with a login identity 
    /// (or anonymous identity), so if you need to access the API from multiple identities, you need multiple connector instances.
    /// </summary>
    /// <remarks>
    /// You may use the following construct to have the connector automatically logout:
    /// <code>using (var connector = new Connector().Login(user, password))
    /// {
    ///     //your code here
    /// 
    /// }</code>
    /// 
    /// The connector will poll from the app.config/web.config the path to the API server.
    /// Note that this class is somewhat low-level, and for much of the functionality of the API server, you wont need to use it directly.
    /// The <see cref="ColonyConcierge.Client.DataSources">ObjectData API</see> uses the connector class "under the hood" to achieve API
    /// server connectivity.
    /// </remarks>
    public class Connector : IDisposable
    {
        #region Private Data
        string _loginToken;
        string _rootUrl;
        RestClient _client;
        bool _traceRestRequests;
        bool _traceRestResults;
        static string _appSecret;
        static readonly string _connectorClientVersion;// = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        static readonly string _connectorClientBuildDate;// = RetrieveLinkerTimestamp(Assembly.GetExecutingAssembly()).ToString();
        static string _serverAPIVersion = EndPoints.APIVersion;
        static string _serverAPIBuildDate = EndPoints.APIDate.ToString();
        //bool _autoLogout;
        #endregion

        #region Properties

        public string LoginToken
        {
            get
            {
                return _loginToken;
            }
        }

        public bool HasLoggedIn
        {
            get
            {
                return _loginToken != null;
            }
        }

        public string AppSecret
        {
            get
            {
                return _appSecret;
            }

            set
            {
                _appSecret = value;
            }
        }

        /// <summary>
        /// Set to "true" to have the connector logout when garbage collected.
        /// Note that "Dispose" will *always* logout, eg if the connector is used inside a using statement.
        /// </summary>
        public bool AutoLogout
        {
            get;
            set;
        }

        #endregion


        #region Static Events
        /// <summary>
        /// Inidicates that a REST call is about to be made.
        /// <note type="note">This can occur on any thread, make sure your handler takes appropriate actions for thread safety.</note>
        /// </summary>
        public static event ConnectorEventHandler RequestCalling;

        /// <summary>
        /// Indicates that a REST call has completed.
        /// </summary>
        public static event ConnectorEventHandler RequestCompleted;
        #endregion

        static Connector()
        {
            //static string _connectorClientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _connectorClientVersion = Provider.G.Metadata.GetClientVersion(typeof(Connector));
            //static string _connectorClientBuildDate = RetrieveLinkerTimestamp(Assembly.GetExecutingAssembly()).ToString();
            _connectorClientBuildDate = Provider.G.Metadata.GetClientBuildDate(typeof(Connector)).ToString();

            //string configSecret = ConfigurationManager.AppSettings["secret"];
            string configSecret = Provider.G.Config["secret"];
            if (!string.IsNullOrWhiteSpace(configSecret))
            {
                _appSecret = configSecret;
            }
            else
            {
                _appSecret = "nosecret";
            }


        }
        public Connector() : this(null, null)
        {

        }

        /// <summary>
        /// Create a connector object that is used to talk to the API server.
        /// </summary>
        /// <param name="LoginToken">If you have a saved login token, you may pass it to the constructor to reuse
        /// the login. Be prepared for the call to fail, if the server, or another machine has logged out the token.</param>
        /// <param name="RootUrl">The root URL of the API server. If you don't supply this parameter, it's value it retrieved from app/web settings</param>
        public Connector(string LoginToken = null, string RootUrl = null )
        {

            bool logRestRequests;
            if (bool.TryParse(Provider.G.Config["logRestRequests"], out logRestRequests) && logRestRequests)
            {
                _traceRestRequests = true;
            }

            bool logRestResults;
            if (bool.TryParse(Provider.G.Config["logRestResults"], out logRestResults) && logRestResults)
            {
                _traceRestResults = true;
            }

            if (RootUrl == null)
            {
                string connectionInfo = Provider.G.Config["BaseUrl"];
                _rootUrl = connectionInfo;
            }
            else
            {
                _rootUrl = RootUrl;

            }

            _loginToken = LoginToken;
            _client = Provider.G.Rest.CreateClient(_rootUrl);
        }

        /// <summary>
        /// Finalizes the connector object, will optionally logout if the user has logged in.
        /// </summary>
        ~Connector()
        {
            if (HasLoggedIn && AutoLogout)
            {
                Dispose(false);
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs a login. This connector the remembers the login token, and will apply the login token
        /// to future API requests automatically. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Connector Login(string username, string /*Str*/ password)
        {
            ////var request = new RestRequest("/logins/{username}", Method.POST)
            ////{
            ////    RequestFormat = DataFormat.Json,
            ////    JsonSerializer = new JSONSerializer()
            ////};


            var request = Provider.G.Rest.CreateRequest("/logins/{username}", RestMethod.POST);
            request.AddUrlSegment("username", username);
            request.AddBody( password);
            AddHeaders(request);

            if (_traceRestRequests)
            {
                LogRequest(request);
            }

            var loginResponse = _client.Execute<LoginResult>(request);

            if (_traceRestResults)
            {
                Provider.G.Tracer.WriteLine(loginResponse.Content, "REST_RESULT_DATA");
            }

            if (loginResponse != null && loginResponse.Data != null && loginResponse.Data.OK)
            {
                _loginToken = loginResponse.Data.Token;
            }
            else
            {
                if (loginResponse != null && loginResponse.Data != null && 
                    (loginResponse.Data.Error != null || loginResponse.Data.OK == false))
                {
                    throw new ServerSideErrorException(loginResponse.Data.Error, loginResponse.Data.Message);
                }
            }
                
            return this;
        }

        public Connector Login(string username, string password, string role)
        {
            ////var request = new RestRequest("/logins/{username}", Method.POST)
            ////{
            ////    RequestFormat = DataFormat.Json,
            ////    JsonSerializer = new JSONSerializer()
            ////};


            var request = Provider.G.Rest.CreateRequest("/logins/{username_or_email}/for_role/{role_name}", RestMethod.POST);
            request.AddUrlSegment("username_or_email", username);
            request.AddUrlSegment("role_name", role);
            request.AddBody(password);
            AddHeaders(request);

            if (_traceRestRequests)
            {
                LogRequest(request);
            }

            var loginResponse = _client.Execute<LoginResult>(request);

            if (_traceRestResults)
            {
                Provider.G.Tracer.WriteLine(loginResponse.Content, "REST_RESULT_DATA");
            }

            if (loginResponse != null && loginResponse.Data != null && loginResponse.Data.OK)
            {
                _loginToken = loginResponse.Data.Token;
            }
            else
            {
                if (loginResponse != null && loginResponse.Data != null &&
                    (loginResponse.Data.Error != null || loginResponse.Data.OK == false))
                {
                    throw new ServerSideErrorException(loginResponse.Data.Error, loginResponse.Data.Message);
                }
            }

            return this;
        }

        public bool Logout()
        {


            var logoutRequest = Provider.G.Rest.CreateRequest("/logins/{token}", RestMethod.DELETE);
            logoutRequest.AddUrlSegment("token", _loginToken);
            AddHeaders(logoutRequest);

            var logoutResult = _client.Execute<Result>(logoutRequest);
            bool ok = logoutResult != null && logoutResult.Data != null && logoutResult.Data.OK;
            if (ok)
            {
                _loginToken = null;
            }
            return ok;
        }

        /// <summary>
        /// Check's the validity of this login with the server
        /// This is not lightweight like the <see cref="HasLoggedIn"/> property. 
        /// </summary>
        /// <returns>True, if the login token existed and could be validated by the server.</returns>
        public bool VerifyLogin()
        {
            if (HasLoggedIn)
            {
                DataResult<User> userResult = Get<User>(EndPoints.ILoginsGetUser).Parameter("token", _loginToken);
                //This is sort of spread out because I may change the logic a bit, it will make for easier changes than a large boolean formula
                if (userResult != null)
                {
                    if (userResult.Error == null)
                    {
                        if(userResult.OK)
                        {
                            if (userResult.Data != null)
                            {
                                if (userResult.Data.ID != 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Initiate a new "GET" request from the server.
        /// You can assign the result of this function to a variable of type 'T' and it will 
        /// automatically execute the request and put the resulting type in the variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public RequestBuilder<T> Get<T>(string url) where T : new()
        {
            return new RequestBuilder<T>(this, url, RestMethod.GET);
            //return default(T);
        }

        /// <summary>
        /// Initiate a new "GET" operation asyncronously.
        /// </summary>
        /// <typeparam name="T">The type of the "wrapped" object</typeparam>
        /// <param name="url">The url endpoint to call</param>
        /// <param name="rbAction">The lambda expression that will be called with the <see cref="RequestBuilder{T}"/> object so that parameters can be set on it.</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url, Action<RequestBuilder<T>> rbAction = null) where T : new()
        {
            var builder = Get<T>(url);
            rbAction?.Invoke(builder);

            return await builder.RunAsync();
        }

        /// <summary>
        /// Helper "get" for special case where the return type is a simple string.
        /// </summary>
        /// <param name="url">The url endpoint to call</param>
        /// <returns>a <see cref="RequestBuilderString"/> object that can be implicitly converted to a string</returns>
        public RequestBuilderString Get(string url)
        {
            return new RequestBuilderString(this, url, RestMethod.GET);
        }

        /// <summary>
        /// The async version of <see cref="Get(string)"/>
        /// </summary>
        /// <param name="url">The url endpoint to call</param>
        /// <param name="rbAction">A lambda expression that can be used to populate the call parameters.</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url, Action<RequestBuilderString> rbAction = null)
        {
            var builder = Get(url);
            rbAction?.Invoke(builder);
            return await builder.RunAsync();
        }

        /// <summary>
        /// Initiate a new "PUT" request from the server.
        /// You can assign the result of this function to a variable of type 'T' and it will 
        /// automatically execute the request and put the resulting type in the variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public RequestBuilder<T> Put<T>(string url) where T : new()
        {
            return new RequestBuilder<T>(this, url, RestMethod.PUT);
        }

        /// <summary>
        /// The async version of <see cref="Put{T}(string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="rbAction"></param>
        /// <returns></returns>
        public async Task<T> PutAsync<T>(string url, Action<RequestBuilder<T>> rbAction = null) where T : new()
        {
            var builder = Put<T>(url);
            rbAction?.Invoke(builder);
            return await builder.RunAsync();
        }


        /// <summary>
        /// Helper "put" for special case where the return type is a simple string.
        /// </summary>
        /// <param name="url">The url endpoint to call</param>
        /// <returns>a <see cref="RequestBuilderString"/> object that can be implicitly converted to a string</returns>
        public RequestBuilderString Put(string url)
        {
            return new RequestBuilderString(this, url, RestMethod.PUT);
        }


        /// <summary>
        /// The async version of <see cref="Put(string)"/>
        /// </summary>
        /// <param name="url">The url endpoint to call</param>
        /// <param name="rbAction">A lambda expression that can be used to populate the call parameters.</param>
        /// <returns></returns>
        public async Task<string> PutAsync(string url, Action<RequestBuilderString> rbAction = null)
        {
            var builder = Put(url);
            rbAction?.Invoke(builder);
            return await builder.RunAsync();
        }

        /// <summary>
        /// Initiate a new "POST" request from the server.
        /// You can assign the result of this function to a variable of type 'T' and it will 
        /// automatically execute the request and put the resulting type in the variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public RequestBuilder<T> Post<T>(string url) where T : new()
        {
            return new RequestBuilder<T>(this, url, RestMethod.POST);
        }

        /// <summary>
        /// The async version of <see cref="Post{T}(string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="rbAction">A lambda expression that can be used to populate the call parameters.</param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string url, Action<RequestBuilder<T>> rbAction = null) where T : new()
        {
            var builder = Post<T>(url);
            rbAction?.Invoke(builder);
            return await builder.RunAsync();
        }


        /// <summary>
        /// Helper "Post" for special case where the return type is a simple string.
        /// </summary>
        /// <param name="url">The url endpoint to call</param>
        /// <returns>a <see cref="RequestBuilderString"/> object that can be implicitly converted to a string</returns>
        public RequestBuilderString Post(string url)
        {
            return new RequestBuilderString(this, url, RestMethod.POST);
        }


        /// <summary>
        /// The async version of <see cref="Post(string)"/>
        /// </summary>
        /// <param name="url">The url endpoint to call</param>
        /// <param name="rbAction">A lambda expression that can be used to populate the call parameters.</param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, Action<RequestBuilderString> rbAction = null)
        {
            var builder = Post(url);
            rbAction?.Invoke(builder);
            return await builder.RunAsync();
        }

        /// <summary>
        /// Initiate a new "DELETE" request from the server.
        /// You can assign the result of this function to a variable of type 'T' and it will 
        /// automatically execute the request and put the resulting type in the variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public RequestBuilder<T> Delete<T>(string url) where T : new()
        {
            return new RequestBuilder<T>(this, url, RestMethod.DELETE);
        }

        /// <summary>
        /// The async version of <see cref="Delete{T}(string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="rbAction">A lambda expression that can be used to populate the call parameters.</param>
        /// <returns></returns>
        public async Task<T> DeleteAsync<T>(string url, Action<RequestBuilder<T>> rbAction = null) where T : new()
        {
            var builder = Delete<T>(url);
            rbAction?.Invoke(builder);
            return await builder.RunAsync();

        }

        /// <summary>
        /// Returns the inner data type of a DataResult<typeparamref name="T"/> request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        internal T ExecuteRequest<T>(RestRequest request) where T : new()
        {

            var result = ExecuteResultRequest<T>(request);//_client.Execute<DataResult<T>>(request);

            if (result != null )
            {
                if (result.OK && result.Data != null)
                {
                    return result.Data;
                }
                
                if (result.Error != null)
                {
                    if (result.Error.NotImplemented)
                    {
                        throw new UnImplementedMethodException(result.Error, result.Message);
                    }

                    throw new ServerSideErrorException(result.Error, result.Message);
                }

                if (!result.OK)
                {
                    throw new BadResultException(result.Message);
                }
                
            }
            
            return default(T);

        }


        internal async Task<T> ExecutRequestAsync<T>(RestRequest request) where T : new()
        {
            var result = await ExecuteResultRequestAsync<T>(request);
            if (result != null)
            {
                if (result.OK && result.Data != null)
                {
                    return result.Data;
                }

                if (result.Error != null)
                {
                    if (result.Error.NotImplemented)
                    {
                        throw new UnImplementedMethodException(result.Error, result.Message);
                    }

                    throw new ServerSideErrorException(result.Error, result.Message);
                }

                if (!result.OK)
                {
                    throw new BadResultException(result.Message);
                }

            }

            return default(T);
        }

        /// <summary>
        /// returns result types that are not wrapped in DataResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        internal T ExecuteRawRequest<T>(RestRequest request)  where T : Result, new()
        {
            AddHeaders(request);
            if (_traceRestRequests)
            {
                LogRequest(request);
                //string dummy = request.ToString();
                //var body = request.Parameters.Where(p => p.Type == ParameterType.RequestBody).FirstOrDefault();
                //Trace.WriteLine(dummy, "REST_REQUEST_DATA");
            }

            ConnectorEventArgs callContextArgs = null;

            if (RequestCalling != null)
            {
                callContextArgs = new ConnectorEventArgs() { URL = request.Resource };
                RequestCalling(this, callContextArgs);
            }
            var result = _client.Execute<T>(request);
            try
            {
                if (result != null)
                {
                    if (result.Data != null)
                    {
                        if (_traceRestResults)
                        {
                            Provider.G.Tracer.WriteLine(result.Content, "REST_RESULT_DATA");
                        }
                        return result.Data;
                    }
                    if (result.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                    {
                        throw new WrongHttpVerbException(Enum.GetName(request.Method.GetType(), request.Method),
                            request.Resource);
                    }
                }

                return default(T);
            }
            finally
            {
                if (RequestCompleted != null)
                {
                    callContextArgs = callContextArgs ?? new ConnectorEventArgs() { URL = request.Resource };
                    callContextArgs.RawResult = result;
                    RequestCompleted(this, callContextArgs);
                }
            }

        }

        internal DataResult<T> ExecuteResultRequest<T>(RestRequest request) where T : new()
        {
            AddHeaders(request);
            if (_traceRestRequests)
            {
                LogRequest(request);
                //string dummy = request.ToString();
                //var body = request.Parameters.Where(p => p.Type == ParameterType.RequestBody).FirstOrDefault();
                //Trace.WriteLine(dummy, "REST_REQUEST_DATA");
            }
            ConnectorEventArgs callContextArgs = null;

            if (RequestCalling != null)
            {
                callContextArgs = new ConnectorEventArgs() { URL = request.Resource };
                RequestCalling(this, new ConnectorEventArgs() { URL = request.Resource });
            }
            var result = _client.Execute<DataResult<T>>(request);
            try
            {
                if (result != null)
                {

                    if (result.Data != null)
                    {
                        if (_traceRestResults)
                        {
                            Provider.G.Tracer.WriteLine(result.Content, "REST_RESULT_DATA");
                        }
                        if (result.Data.AuthorizationFailed)
                        {
                            throw new AuthorizationException(result.Data.Message);
                        }
                        return result.Data;
                    }
                    if (result.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                    {
                        throw new WrongHttpVerbException(Enum.GetName(request.Method.GetType(), request.Method),
                            request.Resource);
                    }
                }

                return default(DataResult<T>);
            }
            finally
            {
                if (RequestCompleted != null)
                {
                    callContextArgs = callContextArgs ?? new ConnectorEventArgs() { URL = request.Resource };
                    callContextArgs.RawResult = result;
                    RequestCompleted(this, callContextArgs);
                }

            }

        }

        internal async Task<DataResult<T>> ExecuteResultRequestAsync<T>(RestRequest request) where T : new()
        {
            AddHeaders(request);
            if (_traceRestRequests)
            {
                LogRequest(request);
                //string dummy = request.ToString();
                //var body = request.Parameters.Where(p => p.Type == ParameterType.RequestBody).FirstOrDefault();
                //Trace.WriteLine(dummy, "REST_REQUEST_DATA");
            }
            ConnectorEventArgs callContextArgs = null;

            if (RequestCalling != null)
            {
                callContextArgs = new ConnectorEventArgs() { URL = request.Resource };
                RequestCalling(this, new ConnectorEventArgs() { URL = request.Resource });
            }

            var result = await _client.ExecuteTaskAsync<DataResult<T>>(request);
            try
            {
                if (result != null)
                {
                    if (result.Data != null)
                    {
                        if (_traceRestResults)
                        {
                            Provider.G.Tracer.WriteLine(result.Content, "REST_RESULT_DATA");
                        }
                        if (result.Data.AuthorizationFailed)
                        {
                            throw new AuthorizationException(result.Data.Message);
                        }
                        return result.Data;
                    }
                    if (result.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                    {
                        throw new WrongHttpVerbException(Enum.GetName(request.Method.GetType(), request.Method),
                            request.Resource);
                    }

                }

                return default(DataResult<T>);
            }
            finally
            {
                if (RequestCompleted != null)
                {
                    callContextArgs = callContextArgs ?? new ConnectorEventArgs() { URL = request.Resource };
                    callContextArgs.RawResult = result;
                    RequestCompleted(this, callContextArgs);
                }

            }


        }



        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //free any managed resources
            }

            if (HasLoggedIn)
            {
                Logout();
            }
            
        }

        private void AddHeaders(RestRequest request)
        {
            if (_loginToken != null)
            {
                request.AddHeader("Authorization", _loginToken);
            }
            //These can be used by the server to reject API calls from out of date clients.
            request.AddHeader(CommonRequestHeaders.ServerAPIVersion, _serverAPIVersion);
            request.AddHeader(CommonRequestHeaders.ServerAPIDate, _serverAPIBuildDate);
            request.AddHeader("ConnectorVersion", _connectorClientVersion);
            request.AddHeader("ConnectorDate", _connectorClientBuildDate);
            request.AddHeader(CommonRequestHeaders.AppSecret, _appSecret);
        }

        private static void LogRequest(RestRequest request)
        {
            var url = request.Resource;
            var body = request.Parameters.Where(p => p.Type == RestParameterType.RequestBody).FirstOrDefault();
            var urlParameters = request.Parameters.Where(p => p.Type == RestParameterType.UrlSegment);
            var queryParameters = request.Parameters.Where(p => p.Type == RestParameterType.QueryString);

            StringBuilder logString = new StringBuilder();

            logString.AppendFormat("Resource: {0}, [{1}]\n", url, request.Method);
            foreach (var segmentParameter in urlParameters)
            {
                logString.AppendFormat("\tWith parameter '{0}' = '{1}'\n", segmentParameter.Name, segmentParameter.Value);
            }

            foreach (var queryParameter in queryParameters)
            {
                logString.AppendFormat("\tWith query parameter '{0}' = '{1}'\n", queryParameter.Name, queryParameter.Value);
            }

            if (body != null)
            {
                logString.AppendFormat("\tand request body:{0}\n", body.Value);
            }

            string finalString = logString.ToString();

            Provider.G.Tracer.WriteLine(finalString, "REST_REQUEST_DATA");
        }

    }
}
