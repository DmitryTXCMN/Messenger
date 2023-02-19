using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Planscam.MobileApi.Tests;

public class AuthorsTests : TestBase
{
    public AuthorsTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Index()
    {
        var res = await SimpleTest("/Authors/Index?id=1");
        dynamic resObj = JsonConvert.DeserializeObject(await res.Content.ReadAsStringAsync());
        Assert.Equal("Author1", (string)resObj.name);
        Assert.Equal("track10", (string)resObj.recentReleases.tracks[0].name);
    }

    [Fact]
    public async Task Search()
    {
        var res = await SimpleTest("/Authors/Search?Query=t");
        dynamic resObj = JsonConvert.DeserializeObject(await res.Content.ReadAsStringAsync());
        Assert.Equal("Author1", (string)resObj[0].name);
    }
}
