# Network Fundamental Functions
<a href="https://www.nuget.org/packages/ArgonautCore.Network/">
    <img src="https://img.shields.io/nuget/vpre/ArgonautCore.Network.svg?maxAge=2592000?style=plastic">
</a>

This is just a neat HttpClient wrapper that provides some easy to use helper functions 
to easily create requests and have them already mapped etc.

This library also makes use of the lightweight Result wrapper for a better error handling
experience without try catch :)

These functions are built upon eachother. The first function in the list below is the highest level and each function above calls the one below, just as a FYI. 

## Provided Methods

```csharp
/// <summary>
/// Makes a request with the specified method and tries to parse the payload and response, assuming its JSON.
/// </summary>
/// <param name="endpoint">The endpoint to request</param>
/// <param name="httpMethod">What http method to use</param>
/// <param name="payload">The payload to send on a post request</param>
/// <param name="castPayloadWithoutJsonParsing">Whether to cast the payload to HttpContent directly instead of json parsing.</param>
/// <typeparam name="T">The type that is expected to be returned and parsed</typeparam>
/// <returns>The parsed return</returns>
public async Task<Result<T, Error>> GetAndMapResponse<T>(
    string endpoint,
    HttpMethods httpMethod = HttpMethods.Get,
    object payload = null,
    bool castPayloadWithoutJsonParsing = false)

//--------------------------------------------------------------------------------------------

/// <summary>
/// Makes a request with the specified method and tries to parse the payload. 
/// </summary>
/// <param name="endpoint">The endpoint to request</param>
/// <param name="httpMethod">What http method to use</param>
/// <param name="payload">The payload to send on a post request</param>
/// <param name="expectNonJson">Whether this method should throw when the response is not json or not. Default is NOT throwing</param>
/// <param name="castPayloadWithoutJsonParsing">Whether to cast the payload to HttpContent directly instead of json parsing.</param>
/// <returns>The raw string return</returns>
public async Task<Result<string, Error>> GetResponse(
    string endpoint,
    HttpMethods httpMethod = HttpMethods.Get,
    object payload = null,
    bool expectNonJson = true,
    bool castPayloadWithoutJsonParsing = false)

//--------------------------------------------------------------------------------------------

/// <summary>
/// Makes a request with the specified method. 
/// </summary>
/// <param name="endpoint">The endpoint to request</param>
/// <param name="httpMethod">What http method to use</param>
/// <param name="payload">The payload to send on a post request</param>
/// <param name="castPayloadWithoutJsonParsing">Whether to cast the payload to HttpContent directly instead of json parsing.</param>
/// <returns>The raw response object</returns>
public async Task<Result<HttpResponseMessage, Error>> GetRawResponseAndEnsureSuccess(
    string endpoint,
    HttpMethods httpMethod = HttpMethods.Get,
    object payload = null,
    bool castPayloadWithoutJsonParsing = false)
```