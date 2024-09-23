using com.example;
using System;
using System.Threading.Tasks;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using TMPro;
using UnityEngine;
using static Supabase.Gotrue.Constants;
using System.Net;

public class SignInWithGoogle : MonoBehaviour
{
    [SerializeField] private string RedirectUrl = "http://localhost:3700/";

    // Public Unity References
    public TMP_Text ErrorText = null!;
    public SupabaseManager SupabaseManager = null!;

    // Private implementation
    private bool _doSignIn;
    private bool _doSignOut;
    private bool _doExchangeCode;

    private HttpListener httpListener = null;

    // Transactional data
    private string _pkce;
    private string _token;

    // Unity does not allow async UI events, so we set a flag and use Update() to do the async work
    public void SignIn()
    {
        _doSignIn = true;
    }

    // Unity does not allow async UI events, so we set a flag and use Update() to do the async work
    public void SignOut()
    {
        _doSignOut = true;
    }

    private async void Update()
    {
        // Unity does not allow async UI events, so we set a flag and use Update() to do the async work
        if (_doSignOut)
        {
            _doSignOut = false;
            await SupabaseManager.Supabase()!.Auth.SignOut();
            _doSignOut = false;
        }

        if (_doExchangeCode)
        {
            _doExchangeCode = false;
            await PerformExchangeCode();
            _doExchangeCode = false;
        }

        // Unity does not allow async UI events, so we set a flag and use Update() to do the async work
        if (_doSignIn)
        {
            _doSignIn = false;
            await PerformSignIn();
            _doSignIn = false;
        }
    }

    private void StartLocalWebserver()
    {
        if (httpListener == null)
        {
            HttpListener httpListener = new HttpListener();

            httpListener.Prefixes.Add(RedirectUrl);
            httpListener.Start();
            httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest), httpListener);
        }
    }

    private void IncomingHttpRequest(IAsyncResult result)
    {
        Debug.Log("IncomingHttpRequest");

        HttpListener httpListener;
        HttpListenerContext httpContext;
        HttpListenerRequest httpRequest;
        HttpListenerResponse httpResponse;
        string responseString;

        // Get back the reference to our http listener
        httpListener = (HttpListener)result.AsyncState;

        // Fetch the context object
        httpContext = httpListener.EndGetContext(result);

        // The context object has the request object for us, that holds details about the incoming request
        httpRequest = httpContext.Request;

        _token = httpRequest.QueryString.Get("code");

        // Build a response to send an "ok" back to the browser for the user to see
        httpResponse = httpContext.Response;
        responseString = "<html><body><b>DONE!</b><br>(You can close this tab/window now)</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

        // Send the output to the client browser
        httpResponse.ContentLength64 = buffer.Length;
        System.IO.Stream output = httpResponse.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        httpListener.Stop();
        httpListener = null;
        _doExchangeCode = true;
    }

    private async Task PerformSignIn()
    {
        try
        {
            var providerAuth = (await SupabaseManager.Supabase()!.Auth.SignIn(Provider.Google, new SignInOptions
            {
                FlowType = OAuthFlowType.PKCE,
            }))!;
            _pkce = providerAuth.PKCEVerifier;

            StartLocalWebserver();
            Application.OpenURL(providerAuth.Uri.ToString());
        }
        catch (GotrueException goTrueException)
        {
            ErrorText.text = $"{goTrueException.Reason} {goTrueException.Message}";
            Debug.Log(goTrueException.Message, gameObject);
            Debug.LogException(goTrueException, gameObject);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message, gameObject);
            Debug.Log(e, gameObject);
        }
    }

    private async Task PerformExchangeCode()
    {
        try
        {
            Session session = (await SupabaseManager.Supabase()!.Auth.ExchangeCodeForSession(_pkce, _token)!);
            ErrorText.text = $"Success! Signed Up as {session.User?.Email}";
        }
        catch (GotrueException goTrueException)
        {
            Debug.Log("GotrueException");
            ErrorText.text = $"{goTrueException.Reason} {goTrueException.Message}";
            Debug.Log(goTrueException.Message, gameObject);
            Debug.LogException(goTrueException, gameObject);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message, gameObject);
            Debug.Log(e, gameObject);
        }
    }
}