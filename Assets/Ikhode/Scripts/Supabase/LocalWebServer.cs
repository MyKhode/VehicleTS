using System;
using System.Net;
using System.Threading.Tasks;

public class LocalWebServer
{
    private HttpListener _listener;

    public LocalWebServer(string prefix)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(prefix);
    }

    public async Task<string> GetRequestAsync()
    {
        _listener.Start();
        var context = await _listener.GetContextAsync();
        var request = context.Request;
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.Close();

        return request.QueryString["code"];
    }

    public void Stop()
    {
        _listener.Stop();
    }
}
