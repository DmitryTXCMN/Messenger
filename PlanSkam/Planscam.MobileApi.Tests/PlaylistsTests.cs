using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Planscam.MobileApi.Tests;

public class PlaylistsTests : TestBase
{
    public PlaylistsTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task All() => await SimpleTest("/Playlists/All");

    [Fact]
    public async Task Index() => await SimpleTest("Playlists/Index?id=1");

    [Fact]
    public async Task GetData()
    {
        var res = await SimpleTest("Playlists/GetData?id=2");
        dynamic resObj = JsonConvert.DeserializeObject(await res.Content.ReadAsStringAsync());
        Assert.Equal("test playlist", (string) resObj.name);
        Assert.Equal(5, (int) resObj.trackIds[0]);
        Assert.Equal(6, (int) resObj.trackIds[1]);
        Assert.Equal(7, (int) resObj.trackIds[2]);
    }

    [Fact]
    public async Task FavouriteTracks()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "Playlists/FavoriteTracks")
            .AddTokenToHeaders(Client);
        var response = await SimpleTest(request);
        dynamic resObj = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        Assert.Equal(@"qwe's favorite tracks", (string) resObj.name);
        Assert.True(((JArray) resObj.tracks).Count is >= 6 and <= 7);
    }

    [Fact]
    public async Task AddTrackToPlaylist()
    {
        Exception? exception = null;
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                    "/Playlists/AddTrackToPlaylist?playlistId=31&trackId=7")
                .AddTokenToHeaders(Client);
            await SimpleTest(request);
        }
        catch (Exception e)
        {
            exception = e;
        }

        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                    "/Playlists/RemoveTrackFromPlaylist?playlistId=31&trackId=7")
                .AddTokenToHeaders(Client);
            await SimpleTest(request);
        }
        if (exception is { })
            throw exception;
    }

    [Fact]
    public async Task LikeAndUnlikePlaylist()
    {
        Exception? exception = null;
        try
        {
            var likeRequest = new HttpRequestMessage(HttpMethod.Post, "/Playlists/LikePlaylist?id=1")
                .AddTokenToHeaders(Client, Output);
            await SimpleTest(likeRequest);
        }
        catch (Exception e)
        {
            exception = e;
        }

        var unlikeRequest = new HttpRequestMessage(HttpMethod.Post, "/Playlists/UnlikePlaylist?id=1")
            .AddTokenToHeaders(Client, Output);
        await SimpleTest(unlikeRequest);
        if (exception is { })
            throw exception;
    }

    [Fact]
    public async Task Liked()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Playlists/Liked")
            .AddTokenToHeaders(Client);
        var response = await SimpleTest(request);
        dynamic resObj = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        Assert.Equal("ddd", (string) resObj.playlists[0].name);
        Assert.Equal("fff", (string) resObj.playlists[1].name);
    }

    private async Task<HttpResponseMessage> CreatePlaylist(string name = "fff")
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/Playlists/Create")
            .AddTokenToHeaders(Client);
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(name), "Name");
        request.Content = content;
        return await SimpleTest(request);
    }

    [Fact]
    public async Task Create()
    {
        const string name = "test create playlist name";
        var res = await CreatePlaylist(name);
        dynamic resObj = JsonConvert.DeserializeObject(await res.Content.ReadAsStringAsync());
        Assert.Equal(name, (string) resObj.name);
    }

    [Fact]
    public async Task CreateAndDeleteSure()
    {
        var createResponse = await CreatePlaylist();
        int id = (JsonConvert.DeserializeObject(await createResponse.Content.ReadAsStringAsync()) as dynamic).id;
        var request = new HttpRequestMessage(HttpMethod.Post, $"/Playlists/DeleteSure?id={id}")
            .AddTokenToHeaders(Client, Output);
        await SimpleTest(request);
    }

    [Fact]
    public async Task AddAndRemoveTrackFromPlaylist()
    {
        var indexRes1 = await SimpleTest("Playlists/Index?id=31");
        dynamic indexRes1Obj = JsonConvert.DeserializeObject(await indexRes1.Content.ReadAsStringAsync());
        Assert.DoesNotContain((JArray) indexRes1Obj.tracks, t => ((dynamic) t).id == 2);
        var addRequest =
            new HttpRequestMessage(HttpMethod.Post, "Playlists/AddTrackToPlaylist?playlistId=31&trackId=2")
                .AddTokenToHeaders(Client);
        await SimpleTest(addRequest);
        var indexRes2 = await SimpleTest("Playlists/Index?id=31");
        dynamic indexRes2Obj = JsonConvert.DeserializeObject(await indexRes2.Content.ReadAsStringAsync());
        Assert.Contains((JArray) indexRes2Obj.tracks, t => ((dynamic) t).id == 2);
        var removeRequest =
            new HttpRequestMessage(HttpMethod.Post, "/Playlists/RemoveTrackFromPlaylist?playlistId=31&trackId=2")
                .AddTokenToHeaders(Client);
        await SimpleTest(removeRequest);
        var indexRes3 = await SimpleTest("Playlists/Index?id=31");
        dynamic indexRes3Obj = JsonConvert.DeserializeObject(await indexRes3.Content.ReadAsStringAsync());
        Assert.DoesNotContain((JArray) indexRes3Obj.tracks, t => ((dynamic) t).id == 2);
    }
}
