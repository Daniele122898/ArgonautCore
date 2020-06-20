using System.Net.Http;
using System.Threading.Tasks;
using ArgonautCore.Lw;

namespace ArgonautCore.Network.Extensions
{
    /// <summary>
    /// Http Response Extension methods.
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Ensures that the <see cref="HttpResponseMessage"/> is a success and return it. If not it will return a <see cref="Result{TVal,TErr}"/>
        /// with an error value.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task<Result<HttpResponseMessage, Error>> EnsureSuccessAndProperReturn(
            this HttpResponseMessage msg)
        {
            if (!msg.IsSuccessStatusCode)
            {
                return new Result<HttpResponseMessage, Error>(new Error(new HttpRequestException(
                    $"Response status code does not indicate success: " +
                    $"{((int) msg.StatusCode).ToString()} \n" +
                    $"Reason: {await msg.Content.ReadAsStringAsync().ConfigureAwait(false)}"
                    )));
            }

            return msg;
        }
    }
}