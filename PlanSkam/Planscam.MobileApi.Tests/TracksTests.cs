using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Planscam.MobileApi.Tests;

public class TracksTests : TestBase
{
    public TracksTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Index()
    {
        var response = await SimpleTest("/Tracks/Index?id=2");
        dynamic parsed = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        Assert.NotNull(parsed);
        Assert.Equal(2, (int) parsed.track.id);
        Assert.Equal(1, (int) parsed.track.author.id);
    }

    [Fact]
    public async Task Search_byTracks() =>
        await SimpleTest("/Tracks/Search?Query=t&Page=1&byAuthors=false");

    [Fact]
    public async Task Search_byAuthors() =>
        await SimpleTest("/Tracks/Search?Query=t&Page=1&byAuthors=true");

    [Fact]
    public async Task GetTrackData()
    {
        var res = await SimpleTest("/Tracks/GetTrackData?id=2");
        dynamic resObj = JsonConvert.DeserializeObject(await res.Content.ReadAsStringAsync());
        Assert.Equal(2, (int) resObj.id);
        Assert.Equal("Author1", (string) resObj.author);
        Assert.Equal(string.Empty, (string) resObj.data);
    }

    [Fact]
    public async Task AddTrackToFavourite()
    {
        Exception? exception = null;
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "Tracks/AddTrackToFavourite?id=9")
                .AddTokenToHeaders(Client, Output);
            await SimpleTest(request);
        }
        catch (Exception e)
        {
            exception = e;
        }

        {
            var request = new HttpRequestMessage(HttpMethod.Post, "Tracks/RemoveTrackFromFavourite?id=9")
                .AddTokenToHeaders(Client, Output);
            await SimpleTest(request);
        }
        if (exception is { })
            throw exception;
    }
}
