var Request = function(enpoint_string, method)
{
    this.endpoint_string = enpoint_string;
    this.method = method;
    this.body = null;
    this.parameters = [];
    this.queryParameters = [];   
    this.headers = [];
};


Request.prototype.add_header = function(name, value)
{
    this.headers.push( {name, value});
}

Request.prototype.add_parameter = function(name, value)
{
    this.parameters.push( {name, value} );
}