using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Planscam.MobileApi.Tests;

public class HomeTests:TestBase
{
    public HomeTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]

    public async Task Index()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Home/Index")
            .AddTokenToHeaders(Client, Output);
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("0"), "Id");
        content.Add(new StringContent("asd"), "Name");
        content.Add(new StringContent("sdg"), "Email");
        request.Content = content;
        await SimpleTest(request);
    }
}