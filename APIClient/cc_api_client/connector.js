



var Connector = function (root_url, username, password)
{
    this.username = username;
    this.password = password;
    
    this.loginToken = null;
    
    this.client = new RestClient(root_url);
};

Connector.prototype.login =  function(on_logged_in)
{
    var req = new Request("/logins/{username}", "POST");
    req.add_parameter("username", this.username);
    req.body = "\"" + this.password + "\"";
    this.add_headers(req);
    
    var that = this;
    
    this.client.execute(req, function(response)
    {
        var token = response.Token;
        that.loginToken = token;
        if (on_logged_in !== undefined)
        {
            on_logged_in();
        }
    });
    
    
};


Connector.prototype.execute_request = function(request, on_results)
{
    this.add_headers(request);
    this.client.execute(request, on_results);
}


Connector.prototype.add_headers = function(request)
{
    if (this.loginToken !== null)
    {
        request.add_header("Authorization", this.loginToken);
    }
    request.add_header("ServerAPIVersion", "1.0.0.0");
    request.add_header("ServerAPIDate", "_serverAPIBuildDate");
    request.add_header("ConnectorVersion", "1.0.0.0");
    request.add_header("ConnectorDate", "null");
    request.add_header("app-secret", "web_front_end_1307F812-2C6D-44D0-9D74-48BADEBAD0A1");
    
    request.add_header("Content-Type", "application/json");
    
    
}