var RestClient = function(root_url)
{
    this.root_url = root_url;
}

RestClient.prototype.execute = function (request, on_result)
{
    var workingUrl = this.root_url;
    workingUrl = workingUrl + request.endpoint_string;
    var i;
    for (i = 0; i < request.parameters.length; i++)
    {
        var p = request.parameters[i];
        workingUrl = workingUrl.replace("{" + p.name + "}", p.value);
    }
    
    var queryStrings = [];
    
    for (i = 0; i < request.queryParameters.length; i++)
    {
        var q = request.queryParameters[i];
        if (q.value !== undefined)
        {
            queryStrings.push(q.name + "=" + q.value);//= workingUrl.replace("{" + q.name + "}", q.value);
            
        }
    }
    
    if (queryStrings.length > 0)
    {
        workingUrl += "?";
        
        var qparams = queryStrings.join("&");
        
        workingUrl += qparams;
    }
    
    var xhr = new XMLHttpRequest();
    
    xhr.onreadystatechange = function() {
      
        if (this.readyState == 4 && this.status == 200) {
            var result_ob = JSON.parse(xhr.response);
            on_result(result_ob);  
        };
    };
    
    xhr.open(request.method, workingUrl);
    
    for (i = 0; i < request.headers.length; i++)
    {
        var h = request.headers[i];
        xhr.setRequestHeader(h.name, h.value);
    }
    
    xhr.send(request.body);
    
};