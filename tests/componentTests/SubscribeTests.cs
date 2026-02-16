using System.Net;
using System.Net.Http.Json;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ComponentTests;
using WebTemplate.Dtos;

[TestFixture]
public class SubscribeTests: ApiTestBase
{
    [Test]
    public async Task SubscribeUserToCommunity_Valid()
    {
        var user = new User { Username = "AsdfUser123", Password = "Password123", Email = "asdf@test.com" };
        var community = new Community { Name = "commmmunity" };
        
        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.Add(community);
        await context.SaveChangesAsync();

        var response = await client.PostAsync($"/Subscribe/SubscribeUserToCommunity/{user.Id}/{community.Id}", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkDb = GetDbContext();
        var userFromDb = await checkDb.Users.Include(x => x.Subscribed).FirstAsync(x => x.Id == user.Id);
        Assert.That(userFromDb.Subscribed.Any(x => x.Id==community.Id), Is.True);

    }

    [Test]
    public async Task AddUserToMod_Valid_ReturnsDto()
    {
        var username = "randomIme";
        var user = new User { Username = username, Password ="Password123", Email ="mod@mail.com" };
        var community = new Community { Name ="tajno" };

        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.Add(community);
        await context.SaveChangesAsync();

        var response = await client.PostAsync($"/Subscribe/AddUserToMod/{username}/tajno", null);
        
        var resultDto = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.That(resultDto.Username, Is.EqualTo(username));

    }

    [Test]
    public async Task SubscribeUser_UserNotExist()
    {
        var response = await client.PostAsync("/Subscribe/SubscribeUserToCommunity/11223344/1", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetCommunitiesFromUser_ReturnsData()
    {
        var user = new User { Username = "all", Password = "Password123", Email = "coll@t.com" };
        var community = new Community { Name = "data" };

        using var context = GetDbContext();
        user.Subscribed = new List<Community> { community };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.GetAsync($"/Subscribe/GetCommunitiesFromUser/{user.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task FindModerators_CommunityNotFound()
    {
        var response = await client.GetAsync("/Subscribe/FindModeratorsFromCommunity/random123");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetCommunitiesFromUser_UserNotFound()
    {
        var response = await client.GetAsync("/Subscribe/GetCommunitiesFromUser/999888");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task IsUserModerating_True_WhenLinked()
    {
        var u_name = "u1Name";
        var c_name = "c1Name";
        
        using var context = GetDbContext();
        var user = new User { Username = u_name, Password = "Password123", Email = "mod@mod.com" };
        var community = new Community { Name = c_name };
        user.Moderator = new List<Community> { community };
        
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.GetAsync($"/Subscribe/IsUserModerating/{c_name}/{u_name}");
        var result = await response.Content.ReadFromJsonAsync<bool>();

        Assert.That(result, Is.True);
    }


    [Test]
    public async Task UnsubscribeUser_Valid()
    {
        var user = new User { Username = "leave", Password = "Password123", Email = "leave@me.com" };
        var community = new Community { Name = "randome" };
        
        using var context = GetDbContext();
        user.Subscribed = new List<Community> { community };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Subscribe/UnsubscribeUserFromCommunity/{user.Id}/{community.Id}");
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkDb = GetDbContext();
        var updatedUser = await checkDb.Users.Include(x => x.Subscribed).FirstAsync(x => x.Id == user.Id);
        Assert.That(updatedUser.Subscribed.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task RemoveModerator_NotAModerator()
    {
        var user = new User { Username = "fake", Password = "Password123", Email = "fake@test.com" };
        var community = new Community { Name = "real" };

        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.Add(community);
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Subscribe/RemoveModeratorFromCommunity/{user.Id}/{community.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    }

    [Test]
    public async Task UnsubscribeUser_CommunityNotFound()
    {
        var user = new User { Username = "realUser", Password = "Password123", Email = "realUser@t.com" };
        using var context = GetDbContext();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Subscribe/UnsubscribeUserFromCommunity/{user.Id}/554433");
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RemoveModByName_Valid()
    {
        var name = "remove";
        var user = new User { Username = name, Password ="Password123", Email ="remove@mail.com" };
        var community = new Community { Name ="communiti" };

        using var context = GetDbContext();
        user.Moderator = new List<Community> { community };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Subscribe/RemoveModFromCommunity/{name}/communiti");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }


    [Test]
    public async Task SubscribeUser_Twice()
    {
        var user = new User { Username = "doubleSub", Password = "Password123", Email = "double@test.com" };
        var community = new Community { Name = "PopularGroup" };

        using var context = GetDbContext();
        user.Subscribed = new List<Community> { community };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.PostAsync($"/Subscribe/SubscribeUserToCommunity/{user.Id}/{community.Id}", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddModerator_Twice()
    {
        var name = "doubleMod";
        var user = new User { Username = name, Password = "Password123", Email = "mod2@test.com" };
        var community = new Community { Name = "twice" };

        using var context = GetDbContext();
        user.Moderator = new List<Community> { community };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.PostAsync($"/Subscribe/AddUserToMod/{name}/ModdedGroup", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }


    [Test]
    public async Task RemoveModerator_Twice()
    {
        var user = new User { Username = "wasMod", Password = "Password123", Email = "was@test.com" };
        var community = new Community { Name = "groupp" };

        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.Add(community);
        await context.SaveChangesAsync();

        await client.DeleteAsync($"/Subscribe/RemoveModeratorFromCommunity/{user.Id}/{community.Id}");
        
        var response = await client.DeleteAsync($"/Subscribe/RemoveModeratorFromCommunity/{user.Id}/{community.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Unsubscribe_WhenNotSubscribed()
    {
        var user = new User { Username = "noMember", Password = "Password123", Email = "not@test.com" };
        var community = new Community { Name = "group" };

        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.Add(community);
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Subscribe/UnsubscribeUserFromCommunity/{user.Id}/{community.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddUserToModerateCommunity_UserNotFound()
    {
        var community = new Community { Name= "LostUserComm" };
        using var context = GetDbContext();
        context.Communities.Add(community);
        await context.SaveChangesAsync();

        var response = await client.PostAsync($"/Subscribe/AddUserToModerateCommunity/999999/{community.Id}", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }


    [Test]
    public async Task AddUserToModerateCommunity_CommunityNotFound()
    {
        var user = new User { Username = "LostCommUser", Password = "Password123", Email = "lost@test.com" };
        using var context = GetDbContext();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.PostAsync($"/Subscribe/AddUserToModerateCommunity/{user.Id}/888888", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddUserToModerateCommunity_Valid()
    {
        var user = new User { Username = "IdMod_1", Password = "Password123", Email = "id1@test.com" };
        var community = new Community { Name = "id_group_1" };
        
        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.Add(community);
        await context.SaveChangesAsync();

        var response = await client.PostAsync($"/Subscribe/AddUserToModerateCommunity/{user.Id}/{community.Id}", null);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        using var checkDb = GetDbContext();
        var userFromDb = await checkDb.Users.Include(x => x.Moderator).FirstAsync(x => x.Id == user.Id);
        Assert.That(userFromDb.Moderator.Any(x => x.Id == community.Id), Is.True);
    }
}