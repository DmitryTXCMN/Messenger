using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Planscam.MobileApi.Tests;

public class ProfileTests : TestBase
{
    public ProfileTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task Index()
    {
        await SimpleTest("Profile/Index?id=7f425417-8246-4d8c-b118-10fe2fcf7a9e");
        await SimpleTest(new HttpRequestMessage(HttpMethod.Get, "Profile/Index")
            .AddTokenToHeaders(Client, Output));
    }

    [Fact]
    public async Task Edit()
    {
        const string newName = "rew";
        const string oldName = "testttt";
        //getting data from index before editing
        var indexResponse = await SimpleTest(new HttpRequestMessage(HttpMethod.Get, "Profile/Index")
            .AddTokenToHeaders(Client, null, oldName));
        dynamic index1Obj = JsonConvert.DeserializeObject(await indexResponse.Content.ReadAsStringAsync());
        string email = index1Obj.email;
        string id = index1Obj.id;

        //changing name to new
        var edit1Request = new HttpRequestMessage(HttpMethod.Post, "Profile/Edit")
            .AddTokenToHeaders(Client, null, oldName);
        edit1Request.Content = JsonContent.Create(new {name = newName, email, id});
        var edit1Response = await SimpleTest(edit1Request);
        dynamic editObj = JsonConvert.DeserializeObject(await edit1Response.Content.ReadAsStringAsync());
        Assert.Equal(newName, (string) editObj.name);

        //restoring prev name
        var edit2Request = new HttpRequestMessage(HttpMethod.Post, "Profile/Edit")
            .AddTokenToHeaders(Client, null, newName);
        edit2Request.Content = JsonContent.Create(new {name = oldName, email, id});
        var edit2Response = await SimpleTest(edit2Request);
        dynamic edit2Obj = JsonConvert.DeserializeObject(await edit2Response.Content.ReadAsStringAsync());
        Assert.Equal(oldName, (string) edit2Obj.name);
    }
}
