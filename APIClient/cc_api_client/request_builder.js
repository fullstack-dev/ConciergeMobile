var RequestBuilder = function(connector, url, method)
{
    this.connector = connector;
    this.request = new Request(url, method);
};

RequestBuilder.prototype.body = function(value)
{
    this.request.body = value;
}

RequestBuilder.prototype.parameter = function(name, value)
{
    this.request.parameters.push({"name": name, "value": value});
}

RequestBuilder.prototype.query = function(name, value)
{
    this.request.queryParameters.push({"name" : name, "value" : value});
}


RequestBuilder.prototype.run = function(onResult)
{
    this.connector.execute_request(this.request, onResult);
}