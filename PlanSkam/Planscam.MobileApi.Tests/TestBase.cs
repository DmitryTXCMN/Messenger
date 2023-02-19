using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Planscam.MobileApi.Tests;

public abstract class TestBase
{
    protected readonly HttpClient Client;
    protected readonly ITestOutputHelper Output;

    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
        Client = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
                builder.ConfigureAppConfiguration((_, b) =>
                    b.AddJsonFile("appsettings.Test.json")))
            .CreateClient();
    }

    protected async Task<HttpResponseMessage> SimpleTest(string uri) =>
        (await Client.GetAsync(uri))
        .WriteToOutput(Output)
        .StatusCodeIsOk();

    protected async Task<HttpResponseMessage> SimpleTest(HttpRequestMessage request) =>
        (await Client.SendAsync(request))
        .WriteToOutput(Output)
        .StatusCodeIsOk();
}
