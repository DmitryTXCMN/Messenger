using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Planscam.MobileApi.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace Planscam.MobileApi.Tests;

public class StudioTest : TestBase
{
    public StudioTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task MyTracks() =>
        await SimpleTest(new HttpRequestMessage(HttpMethod.Get, "/Studio/MyTracks")
            .AddTokenToHeaders(Client, Output));

    //todo
    //[Fact]
    public async Task LoadNewTrack()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/Studio/LoadNewTrack")
            .AddTokenToHeaders(Client, Output);
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("qwe"), "Name");
        content.Add(new StreamContent(new MemoryStream(new byte[]{2})), "Track");
        content.Add(new StringContent("0"), "GenreId");
        request.Content = content;
        await SimpleTest(request);
    }

    //todo
    //[Fact]
    public async Task DeleteTrackSure()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/Studio/DeleteTrackSure?id=1")
            .AddTokenToHeaders(Client, Output);
        await SimpleTest(request);
    }

    [Fact]
    public async Task Albums() =>
        await SimpleTest(new HttpRequestMessage(HttpMethod.Get, "/Studio/Albums")
            .AddTokenToHeaders(Client, Output));
}