using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace SiteCore.AuthenticationAPI.Modules
{
    public class BasicAuthHttpModule : IHttpModule
    {
        const String AUTH_USER_PREFIX = "auth.user.";
        static readonly IDictionary<String, String> logins = ConfigurationManager.AppSettings.AllKeys.Where(k => k.StartsWith(AUTH_USER_PREFIX))
                                                            .ToDictionary(key => key.Replace(AUTH_USER_PREFIX, String.Empty), value => ConfigurationManager.AppSettings.Get(value));
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
            context.EndRequest += OnEndRequest;
        }

        private static void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var authHeaders = HttpContext.Current.Request.Headers["Authorization"];
            if (authHeaders != null)
            {
                var authHeadersValue = AuthenticationHeaderValue.Parse(authHeaders);
                if (authHeadersValue.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrWhiteSpace(authHeadersValue.Parameter))
                {

                    try
                    {
                        var credentials = authHeadersValue.Parameter;
                        var encoding = Encoding.GetEncoding("iso-8859-1");
                        credentials = encoding.GetString(Convert.FromBase64String(credentials));
                        string name = credentials.Split(':').First();
                        string password = credentials.Split(':').Last();

                        if (logins.Any(l => l.Key.Equals(name) && l.Value.Equals(password)))
                        {
                            //Set the principal for validated user  

                            var principal = new GenericPrincipal(new GenericIdentity(name), null);
                            Thread.CurrentPrincipal = principal;
                            if (HttpContext.Current != null)
                            {
                                HttpContext.Current.User = principal;
                            }
                        }
                        else
                        {
                            //Authentication failed  
                            HttpContext.Current.Response.StatusCode = 401;
                        }
                    }
                    catch (FormatException)
                    {
                        HttpContext.Current.Response.StatusCode = 401;
                    }

                }
            }
        }

        private static void OnEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                //Addh eaders if authentication failed  
                response.Headers.Add("WWW-Authenticate",
                    string.Format("Basic realm=\"{0}\"", ConfigurationManager.AppSettings.Get("auth.realm")));
            }
        }

        public void Dispose() { }

    }
}
