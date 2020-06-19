using System.Net.Http;
using System.Threading.Tasks;
using ArgonautCore.Lw;

namespace ArgonautCore.Network.Extensions
{
    public static class HttpResponseExtensions
    {
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