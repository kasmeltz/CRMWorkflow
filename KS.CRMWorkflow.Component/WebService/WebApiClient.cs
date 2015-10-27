using KS.CRMWorkflow.Component.Cache.Interface;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KS.CRMWorkflow.Component.WebService
{
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class AuthenticationResult
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public class WebApiClient
    {
        public const int SafeAuthenticationSeconds = 45;
        protected string BaseApiUrl { get; set; }
        public string AuthenticationToken { get; protected set; }
        public ICacheProvider CacheProvider { get; protected set; }
        public bool UseCache { get; protected set; }

        public WebApiClient(string baseApiUrl, bool useCache, ICacheProvider cacheProvider)
        {
            CacheProvider = cacheProvider;
            BaseApiUrl = baseApiUrl;
            UseCache = useCache;
        }

        public WebApiClient(string baseApiUrl, string token, bool useCache, ICacheProvider cacheProvider)
            : this(baseApiUrl, useCache, cacheProvider)
        {
            AuthenticationToken = token;
        }

        protected HttpClient GetHttpClient(bool isAuthenticated)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(BaseApiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (isAuthenticated)
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AuthenticationToken);
            }

            return client;
        }

        protected async Task DealWithError(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new WebApiUnauthorizedException();
            }

            HttpError error = await response.Content.ReadAsAsync<HttpError>().ConfigureAwait(false);
            throw new WebApiErrorException(error.Message);
        }

        public void ClearCache()
        {
            CacheProvider.ClearContainer("httpGet");
        }

        public async Task<T> Get<T>(string serviceAddress, bool isAuthenticated = true)
        {            
            T result = default(T);

            if (UseCache)
            {
                result = (T)CacheProvider.Get(serviceAddress);
                if (result != null)
                {
                    return result;
                }
            }

            using (HttpClient client = GetHttpClient(isAuthenticated))
            {
                HttpResponseMessage response = await client.GetAsync(serviceAddress).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<T>().ConfigureAwait(false);
                }
                else
                {
                    await DealWithError(response);
                }

                if (UseCache)
                {
                    CacheProvider.Insert(
                        "httpGet", 
                        serviceAddress,
                        result,
                        new DateTimeOffset(DateTime.UtcNow.AddMinutes(2)));
                }

                return result;
            }
        }

        public async Task<T> Post<T>(string serviceAddress, T item, bool isAuthenticated = true)
        {
            return await Post<T, T>(serviceAddress, item, isAuthenticated);
        }

        public async Task<K> Post<T, K>(string serviceAddress, T item, bool isAuthenticated = true)
        {
            K result = default(K);

            using (HttpClient client = GetHttpClient(isAuthenticated))
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(serviceAddress, item).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<K>().ConfigureAwait(false);

                    ClearCache();
                }
                else
                {
                    await DealWithError(response);
                }

                return result;
            }
        }

        public async Task<T> Put<T>(string serviceAddress, T item, bool isAuthenticated = true)
        {
            return await Put<T, T>(serviceAddress, item, isAuthenticated);
        }

        public async Task<K> Put<T, K>(string serviceAddress, T item, bool isAuthenticated = true)
        {
            K result = default(K);

            using (HttpClient client = GetHttpClient(isAuthenticated))
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(serviceAddress, item).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<K>().ConfigureAwait(false);

                    ClearCache();
                }
                else
                {
                    await DealWithError(response);
                }

                return result;
            }
        }

        public async Task Delete(string serviceAddress, bool isAuthenticated = true)
        {
            using (HttpClient client = GetHttpClient(isAuthenticated))
            {
                HttpResponseMessage response = await client.DeleteAsync(serviceAddress).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    ClearCache();

                    return;
                }
                else
                {
                    await DealWithError(response);
                }
            }
        }

        public async Task Authenticate(Account account)
        {
            if (account == null)
            {
                return;
            }

            AuthenticationResult result = null;

            StringBuilder sb = new StringBuilder();
            sb.Append("grant_type=password");
            sb.Append("&username=");

            sb.Append(WebUtility.UrlEncode(account.Username));
            sb.Append("&password=");
            sb.Append(WebUtility.UrlEncode(account.Password));

            HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Post,
                BaseApiUrl + "token");

            request.Content =
                new StringContent(sb.ToString(),
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded");

            using (HttpClient client = GetHttpClient(false))
            {
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<AuthenticationResult>().ConfigureAwait(false);
                }
                else
                {
                    HttpError error = await response.Content.ReadAsAsync<HttpError>().ConfigureAwait(false);
                    throw new WebApiErrorException(error.Message);
                }

                AuthenticationToken = result.access_token;
            }
        }
    }
}
