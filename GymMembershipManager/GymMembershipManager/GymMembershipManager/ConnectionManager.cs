using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shunty.Logging;

namespace Shunty.GymMembershipManager
{
    public class ConnectionManager : IConnectionManager, IDisposable
    {
        private static readonly ILog _log = LogProvider.For<ConnectionManager>();

        private HttpClientHandler _handler;
        private CookieContainer _cookies;
        private HttpClient _http;

        public static IConnectionManager CreateManager()
        {
            var result = new ConnectionManager();
            return result;
        }

        public ConnectionManager()
        {
            _cookies = new CookieContainer();
            _handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = _cookies
            };
        }

        public async Task Login(string username, string password)
        {
            var http = GetClient();
            // First we need to make a basic GET call to load the initial cookies
            var result1 = await http.GetAsync(Consts.LoginUrl);
            _log.TraceFormat("Initial login GET. Status code: {StatusCode};", result1.StatusCode);
            result1.EnsureSuccessStatusCode();

            // Then make the login request
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("login.Email", username),
                new KeyValuePair<string, string>("login.Password", password),
                new KeyValuePair<string, string>("login.RedirectURL", ""),
            });
            var result2 = await http.PostAsync(Consts.LoginUrl, content);
            _log.TraceFormat("Login request. Status code: {StatusCode};", result2.StatusCode);
#if TRACE
            var resulttext = await result2.Content.ReadAsStringAsync();
            _log.TraceFormat("Login result content. Result: {LoginContent};", resulttext);
#endif
            result2.EnsureSuccessStatusCode();
        }

        public void Logout()
        {
            if (_http == null)
                return;

            var result = _http.GetAsync(Consts.LogoutUrl).Result; // Run this synchronously (so that dispose doesn't have to be async)
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            // Assume we are already logged in - up to the user to do that
            var http = GetClient();

            var result = await http.GetAsync(url);
            return result;
        }

        public async Task<string> GetResultAsync(string url)
        {
            var response = await GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> content)
        {
            // Assume we are already logged in - up to the user to do that
            var http = GetClient();

            var postcontent = new FormUrlEncodedContent(content);
            var result = await http.PostAsync(url, postcontent);
            return result;
        }

        public async Task<string> PostResultAsync(string url, IEnumerable<KeyValuePair<string, string>> content)
        {
            var response = await PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        private HttpClient GetClient()
        {
            if (_http == null)
            {
                _http = new HttpClient(_handler);
                _http.BaseAddress = new Uri(Consts.BaseAddress);
            }
            return _http;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_http != null)
                {
                    Logout();
                    _http.Dispose();
                    _http = null;
                }
            }
        }

    }
}
