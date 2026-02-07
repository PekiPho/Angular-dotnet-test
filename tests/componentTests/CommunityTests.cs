using System.Net;
using System.Net.Http.Json;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ComponentTests;

[TestFixture]
public class CommunityTests : ApiTestBase
{
    [Test]
    public async Task CreateCommunity_Correct_OK()
    {
        var comm = new Community{Name="Test Community",Description = "Descriptionnn"};
        var response = await client.PostAsJsonAsync("/Community/CreateCommunity",comm);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task CreateCommunity_CheckSaveDB()
    {
        var name = "DB Test";
        var comm = new Community{Name=name};

        await client.PostAsJsonAsync("/Community/CreateCommunity",comm);

        using var context = GetDbContext();

        var saved= await context.Communities.FirstOrDefaultAsync(c=>c.Name == name);
        Assert.That(saved,Is.Not.Null);
        Assert.That(saved.Name,Is.EqualTo(name));
    }

    [Test]
    public async Task CreateCommunity_NoPosts()
    {
        var name = "Comm with Posts";
        var comm = new Community
        {
            Name=name,
            Posts = new List<Post>
            {
                new Post{Title = "aaaaaaaa"},
                new Post{Title = "bbbbbbbb"}
            }
        };

        await client.PostAsJsonAsync("/Community/CreateCommunity",comm);

        using var context=GetDbContext();
        var saved = await context.Communities.Include(c=>c.Posts).FirstOrDefaultAsync(c=>c.Name==name);

        Assert.That(saved,Is.Not.Null);
        Assert.That(saved.Posts,Is.Empty);
    }


    [Test]
    public async Task CreateCommunity_NoName()
    {
        var comm = new Community{Name="",Description="No name"};

        var response = await client.PostAsJsonAsync("/Community/CreateCommunity",comm);

        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task CreateCommunity_AlreadyExists()
    {
        var old = new Community{Name="exists"};

        await client.PostAsJsonAsync("/Community/CreateCommunity",old);

        using var context=GetDbContext();

        var newC = new Community{Name="exists"};

        var response = await client.PostAsJsonAsync("/Community/CreateCommunity",newC);

        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.BadRequest));

    }
}