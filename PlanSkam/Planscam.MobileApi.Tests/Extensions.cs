using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Planscam.MobileApi.Tests;

internal static class Extensions
{
    public static HttpResponseMessage StatusCodeIsOk(this HttpResponseMessage response)
    {
        Assert.True(response.StatusCode == HttpStatusCode.OK);
        return response;
    }

    public static HttpResponseMessage WriteToOutput(this HttpResponseMessage response, ITestOutputHelper? output)
    {
        output?.WriteLine($"StatusCode : {response.StatusCode}");
        output?.WriteLine(response.Content.ReadAsStringAsync().Result);
        return response;
    }

    private static string? GetToken(HttpClient client, ITestOutputHelper? output = default, string? userName = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/Auth/Login");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "password"},
            {"username", userName ?? "qwe"},
            {"password", "qweQWE123!"}
        });
        var response = client.SendAsync(request).Result
            .WriteToOutput(output)
            .StatusCodeIsOk();
        return (string) (JsonConvert.DeserializeObject(
                response.Content.ReadAsStringAsync().Result) as dynamic)
            .access_token;
    }

    public static HttpRequestMessage AddTokenToHeaders(
        this HttpRequestMessage request,
        HttpClient client,
        ITestOutputHelper? output = default,
        string? userName = default)
    {
        request.Headers.Add("Authorization", $"Bearer {GetToken(client, output, userName)}");
        return request;
    }
}
