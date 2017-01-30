using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shunty.GymMembershipManager
{
    public interface IConnectionManager : IDisposable
    {
        Task Login(string username, string password);
        void Logout();

        Task<HttpResponseMessage> GetAsync(string url);
        Task<string> GetResultAsync(string url);
        Task<string> GetResultAsync(string url, IEnumerable<KeyValuePair<string, string>> queryParams, IEnumerable<KeyValuePair<string, string>> headers = null);

        Task<HttpResponseMessage> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> content);
        Task<string> PostResultAsync(string url, IEnumerable<KeyValuePair<string, string>> content);
    }
}