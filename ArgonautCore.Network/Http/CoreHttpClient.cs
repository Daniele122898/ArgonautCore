using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ArgonautCore.Lw;
using ArgonautCore.Network.Enums;
using ArgonautCore.Network.Extensions;

namespace ArgonautCore.Network.Http
{
    /// <summary>
    /// <see cref="HttpClient"/> wrapper class for ease of use. Makes use of <see cref="Result{TVal,TErr}"/> lightweight wrapper.
    /// This class will handle the lifecycle of the HttpClient. So make sure that if you pass a client that you are okey with it being disposed.
    /// If this is not wished, set the boolean flag to not handle the lifecycle!
    /// </summary>
    public class CoreHttpClient : IDisposable
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// The actual <see cref="HttpClient"/>.
        /// </summary>
        public readonly HttpClient Client;
        
        private readonly bool _skipLifeCycle;

        /// <summary>
        /// Create a Core http client with a custom timeout
        /// </summary>
        public CoreHttpClient(TimeSpan timeout)
        {
            Client = new HttpClient {Timeout = timeout};
        }

        /// <summary>
        /// Create an instance with an already created HttpClient. You can choose whether
        /// this class will handle the lifecycle of the HttpClient on Dispose.
        /// </summary>
        public CoreHttpClient(HttpClient client, bool skipLifeCycle = false, TimeSpan? timeout = null)
        {
            Client = client;
            _skipLifeCycle = skipLifeCycle;

            if (timeout != null)
                Client.Timeout = timeout.Value;
        }

        /// <summary>
        /// Create an empty instance and maybe pass a base address.
        /// </summary>
        public CoreHttpClient(string baseAddress = null, TimeSpan? timeout = null)
        {
            Client = new HttpClient()
            {
                BaseAddress = string.IsNullOrWhiteSpace(baseAddress) ? null : new Uri(baseAddress)
            };
            
            if (timeout != null)
                Client.Timeout = timeout.Value;
        }

        /// <summary>
        /// Makes a request with the specified method and tries to parse the payload and response, assuming its JSON.
        /// </summary>
        /// <param name="endpoint">The endpoint to request</param>
        /// <param name="httpMethod">What http method to use</param>
        /// <param name="payload">The payload to send on a post request</param>
        /// <param name="castPayloadWithoutJsonParsing">Whether to cast the payload to HttpContent directly instead of json parsing.</param>
        /// <param name="httpContent">When the httpContent is set then it will ignore the <see cref="payload"/> and <see cref="castPayloadWithoutJsonParsing"/> params.</param>
        /// <typeparam name="T">The type that is expected to be returned and parsed</typeparam>
        /// <returns>The parsed return</returns>
        public async Task<Result<T, Error>> GetAndMapResponse<T>(
            string endpoint,
            HttpMethods httpMethod = HttpMethods.Get,
            object payload = null,
            bool castPayloadWithoutJsonParsing = false,
            HttpContent httpContent = null)
        {
            var respRes = await GetResponse(endpoint, httpMethod, payload, false,
                castPayloadWithoutJsonParsing).ConfigureAwait(false);

            return respRes.Match<Result<T, Error>>(
                some: respString => JsonSerializer.Deserialize<T>(respString, _jsonOptions),
                err: error => new Result<T, Error>(error));
        }

        /// <summary>
        /// Makes a request with the specified method and tries to parse the payload. 
        /// </summary>
        /// <param name="endpoint">The endpoint to request</param>
        /// <param name="httpMethod">What http method to use</param>
        /// <param name="payload">The payload to send on a post request</param>
        /// <param name="expectNonJson">Whether this method should throw when the response is not json or not. Default is NOT throwing</param>
        /// <param name="castPayloadWithoutJsonParsing">Whether to cast the payload to HttpContent directly instead of json parsing.</param>
        /// <param name="httpContent">When the httpContent is set then it will ignore the <see cref="payload"/> and <see cref="castPayloadWithoutJsonParsing"/> params.</param>
        /// <returns>The raw string return</returns>
        public async Task<Result<string, Error>> GetResponse(
            string endpoint,
            HttpMethods httpMethod = HttpMethods.Get,
            object payload = null,
            bool expectNonJson = true,
            bool castPayloadWithoutJsonParsing = false,
            HttpContent httpContent = null)
        {
            var respResult = await this.GetRawResponseAndEnsureSuccess(endpoint, httpMethod, payload,
                castPayloadWithoutJsonParsing).ConfigureAwait(false);

            return await respResult.Match<Task<Result<string, Error>>>(
                some: async (HttpResponseMessage response) =>
                {
                    if (!expectNonJson && response.Content.Headers.ContentType.MediaType != "application/json")
                    {
                        return new Result<string, Error>(
                            new Error(new NotSupportedException("Response was not json and thus not supported")));
                    }

                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                },
                error => Task.FromResult(new Result<string, Error>(error))).ConfigureAwait(false);
        }

        /// <summary>
        /// Makes a request with the specified method. 
        /// </summary>
        /// <param name="endpoint">The endpoint to request</param>
        /// <param name="httpMethod">What http method to use</param>
        /// <param name="payload">The payload to send on a post request</param>
        /// <param name="castPayloadWithoutJsonParsing">Whether to cast the payload to HttpContent directly instead of json parsing.</param>
        /// <param name="httpContent">When the httpContent is set then it will ignore the <see cref="payload"/> and <see cref="castPayloadWithoutJsonParsing"/> params.</param>
        /// <returns>The raw response object</returns>
        public async Task<Result<HttpResponseMessage, Error>> GetRawResponseAndEnsureSuccess(
            string endpoint,
            HttpMethods httpMethod = HttpMethods.Get,
            object payload = null,
            bool castPayloadWithoutJsonParsing = false,
            HttpContent httpContent = null)
        {
            HttpResponseMessage response;

            switch (httpMethod)
            {
                case HttpMethods.Get:
                    response = await Client.GetAsync(endpoint).ConfigureAwait(false);
                    break;
                case HttpMethods.Post:
                case HttpMethods.Patch:
                case HttpMethods.Put:

                    HttpContent content = httpContent ?? (
                        castPayloadWithoutJsonParsing
                            ? (HttpContent) payload
                            : new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8,
                                "application/json"));

                    response = httpMethod switch
                    {
                        HttpMethods.Post => await Client.PostAsync(endpoint, content).ConfigureAwait(false),
                        HttpMethods.Put => await Client.PutAsync(endpoint, content).ConfigureAwait(false),
                        HttpMethods.Patch => await Client.PatchAsync(endpoint, content).ConfigureAwait(false),
                        // ReSharper disable once UnreachableSwitchArmDueToIntegerAnalysis
                        _ => null // Just for the sake of it :P
                    };
                    break;
                case HttpMethods.Delete:
                    if (payload == null)
                    {
                        response = await Client.DeleteAsync(endpoint).ConfigureAwait(false);
                        break;
                    }

                    HttpRequestMessage requestMessage = new HttpRequestMessage()
                    {
                        Content = new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8,
                            "application/json"),
                        Method = HttpMethod.Delete,
                        RequestUri = Client.BaseAddress == null
                            ? new Uri(endpoint)
                            : new Uri(Client.BaseAddress, endpoint)
                    };

                    response = await Client.SendAsync(requestMessage).ConfigureAwait(false);

                    break;
                default:
                    throw new ArgumentException("Method not supported", nameof(httpMethod));
            }

            var successMaybe = await response.EnsureSuccessAndProperReturn().ConfigureAwait(false);
            return successMaybe;
        }

        /// <summary>
        /// Will clear the client if the skip lifecycle has not been specifically set to true.
        /// </summary>
        public void Dispose()
        {
            if (!_skipLifeCycle)
                Client.Dispose();
        }
    }
}