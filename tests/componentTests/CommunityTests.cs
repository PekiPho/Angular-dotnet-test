using System.Net;
using System.Net.Http.Json;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

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

    [Test]
    public async Task GetCommunityId_ValidReturn()
    {
        var comm = new Community{Name ="Test",Description="Testtt"};
        using var context = GetDbContext();
        context.Communities.Add(comm);
        await context.SaveChangesAsync();

        var response= await client.GetAsync($"/Community/GetCommunity/{comm.Id}");

        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<Community>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(comm.Id));
    }

    [Test]
    public async Task GetCommunityId_BadID()
    {
        var response = await client.GetAsync("/Community/GetCommunity/-1");


        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var message = await response.Content.ReadAsStringAsync();
        Assert.That(message, Does.Contain("does not exist"));
    }

    [Test]
    public async Task GetCommunityName_ValidName()
    {
        var name = "Nameee";
        using var context=GetDbContext();
        context.Communities.Add(new Community{Name=name,Description="aa"});
        await context.SaveChangesAsync();

        var response = await client.GetAsync($"/Community/GetCommunityByName/{name}");

        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));

        var result=await response.Content.ReadFromJsonAsync<Community>();
        Assert.That(result?.Name,Is.EqualTo(name));
    }

    [Test]
    public async Task GetCommunityName_SpecialCharacter()
    {
        var nameWithSpace = "mmmmm";
        using var context = GetDbContext();
        context.Communities.Add(new Community { Name = nameWithSpace });
        await context.SaveChangesAsync();
        

        var encodedName = Uri.EscapeDataString(nameWithSpace);
        var response = await client.GetAsync($"/Community/GetCommunityByName/{encodedName}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<Community>();
        Assert.That(result?.Name, Is.EqualTo(nameWithSpace));
    }

    [Test]
    public async Task UpdateDescription_ValidId()
    {
        var comm = new Community{Name="Updatee",Description="old"};
        using var context =GetDbContext();
        context.Communities.Add(comm);
        await context.SaveChangesAsync();

        var newDesc = "new";
        var response = await client.PutAsync($"/Community/UpdateDescription/{comm.Id}/{newDesc}", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        var updated = await checkContext.Communities.FindAsync(comm.Id);
        Assert.That(updated?.Description, Is.EqualTo(newDesc));
    }

    [Test]
    public async Task UpdateCommInfo_OK()
    {
        var name = "info";
        using var context = GetDbContext();
        context.Communities.Add(new Community { Name = name, CommInfo = "old" });
        await context.SaveChangesAsync();

        var newInfo = "new";
        var response = await client.PutAsync($"/Community/UpdateCommInfo/{name}/{newInfo}", null);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        var updated = await checkContext.Communities.FirstOrDefaultAsync(c => c.Name == name);
        Assert.That(updated?.CommInfo, Is.EqualTo(newInfo));
    }

    [Test]
    public async Task UpdateDescription_WrongId_BadRequest()
    {
        var response = await client.PutAsync("/Community/UpdateDescription/9999/SomeDesc", null);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }


    [Test]
    public async Task DeleteCommunityId_Valid()
    {
        var comm = new Community { Name = "delete" };
        using var context = GetDbContext();
        context.Communities.Add(comm);
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Community/DeleteCommunity/{comm.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        var deleted = await checkContext.Communities.FindAsync(comm.Id);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task DeleteCommunityName_Valid()
    {
        var name = "delteeme";
        using var context = GetDbContext();
        context.Communities.Add(new Community { Name = name });
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Community/DeleteCommunityByName/{name}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        var exists = await checkContext.Communities.AnyAsync(c => c.Name == name);
        Assert.That(exists, Is.False);
    }

    [Test]
    public async Task DeleteCommunity_BadId()
    {
        var response = await client.DeleteAsync("/Community/DeleteCommunity/0");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var message = await response.Content.ReadAsStringAsync();
        Assert.That(message, Does.Contain("does not exist"));
    }

    [Test]
    public async Task DeleteCommunity_SpecCharacters_Valid()
    {
        var name = "C++ & C# Community!";
        using var context = GetDbContext();
        context.Communities.Add(new Community { Name = name });
        await context.SaveChangesAsync();

        var encoded = Uri.EscapeDataString(name);
        var response = await client.DeleteAsync($"/Community/DeleteCommunityByName/{encoded}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        var exists = await checkContext.Communities.AnyAsync(c => c.Name == name);
        Assert.That(exists, Is.False);
    }

    [Test]
    public async Task DeleteCommunity_DoubleDelete()
    {
        var comm = new Community { Name = "tempDel" };
        using var context = GetDbContext();
        context.Communities.Add(comm);
        await context.SaveChangesAsync();

        await client.DeleteAsync($"/Community/DeleteCommunity/{comm.Id}");
        
        var response = await client.DeleteAsync($"/Community/DeleteCommunity/{comm.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetCommunityByName_CaseSensitivity()
    {
        var ime = "CaseSenSitiVity12";
        using var context = GetDbContext();
        context.Communities.Add(new Community { Name = ime });
        await context.SaveChangesAsync();

        var response = await client.GetAsync($"/Community/GetCommunityByName/{ime.ToLower()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task CreateCommunity_NullBody()
    {
        var response = await client.PostAsJsonAsync<Community>("/Community/CreateCommunity", null!);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task CreateCommunity_EmptyDescription()
    {
        var c = new Community { Name = "Bez Opis", Description = "" };
        
        var response = await client.PostAsJsonAsync("/Community/CreateCommunity", c);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateCommunityInfo_EmptyInfo()
    {
        var comm = new Community { Name = "testtest", CommInfo = "stari" };
        using var context = GetDbContext();
        context.Communities.Add(comm);
        await context.SaveChangesAsync();

        var response = await client.PutAsync($"/Community/UpdateCommInfo/{comm.Name}/null", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdateDescription_CommunityNotFound()
    {
        var response = await client.PutAsync("/Community/UpdateDescription/1234123/random_opis", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}