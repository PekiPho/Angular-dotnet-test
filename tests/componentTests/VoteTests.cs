using System.Net;
using System.Net.Http.Json;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ComponentTests;
using WebTemplate.Dtos;

[TestFixture]
public class VoteTests: ApiTestBase
{
    private async Task<(User User, Post Post)> SetupUserAndPost(string uName)
    {
        using var context = GetDbContext();
        var user = new User { Username = uName, Password = "123", Email = $"{uName}@v.com" };
        var comm = new Community { Name = "voteComm" };
        var post = new Post { Title = "VotePost", User = user, Community = comm };
        
        context.Users.Add(user);
        context.Communities.Add(comm);
        context.Posts.Add(post);
        await context.SaveChangesAsync();
        return (user, post);
    }

    [Test]
    public async Task AddVote_NewUpvote()
    {
        var (user, post) = await SetupUserAndPost("voter1");

        var response = await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var context = GetDbContext();
        var vote = await context.Votes.FirstOrDefaultAsync(v => v.User.Id == user.Id && v.Post.Id == post.Id);
        Assert.That(vote, Is.Not.Null);
        Assert.That(vote.VoteValue, Is.True);


    }

    [Test]
    public async Task AddVote_RemovesVotToggle()
    {
        var (user, post) = await SetupUserAndPost("voter2");

        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);
        
        var response = await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var context = GetDbContext();
        var voteExists = await context.Votes.AnyAsync(v => v.User.Id == user.Id);
        Assert.That(voteExists, Is.False, "Vote should be removed from DB when toggled.");
    }

    [Test]
    public async Task GetVotesByUser()
    {
        var (user, post) = await SetupUserAndPost("voter3");
        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);

        var response = await client.GetAsync($"/Vote/GetAllVotesByUser/{user.Username}");
        var votes = await response.Content.ReadFromJsonAsync<Dictionary<Guid, bool>>();


        Assert.That(votes.ContainsKey(post.Id), Is.True);
        Assert.That(votes[post.Id], Is.True);

    }


    [Test]
    public async Task AddVote_UpdatesPostScore()
    {
        var (user, post) = await SetupUserAndPost("voter");

        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);

        using var context = GetDbContext();
        var updatedPost = await context.Posts.FindAsync(post.Id);
        
        Assert.That(updatedPost.Vote, Is.EqualTo(1), "The 'Vote' column should have been updated to 1.");
    }

    [Test]
    public async Task AddVote_SwitchUpToDown()
    {

        var (user, post) = await SetupUserAndPost("switch");

        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);
        
        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/false", null);

        using var context = GetDbContext();
        var updatedPost = await context.Posts.FindAsync(post.Id);
        
        Assert.That(updatedPost.Vote, Is.EqualTo(-1), "Score should be -1 after switching to downvote.");

    }

    [Test]
    public async Task AddVote_SwitchDownToUp()
    {
        var (user, post) = await SetupUserAndPost("switcher");

        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/false", null);
        
        var response = await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var context = GetDbContext();
        var dbPost = await context.Posts.FindAsync(post.Id);
        Assert.That(dbPost.Vote, Is.EqualTo(1));
    }

    [Test]
    public async Task AddVote_ToggleRemovesScore()
    {
        var (user, post) = await SetupUserAndPost("toggleScore");

        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);
        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/true", null);

        using var context = GetDbContext();
        var updatedPost = await context.Posts.FindAsync(post.Id);

        Assert.That(updatedPost!.Vote, Is.EqualTo(0));
    }

    [Test]
    public async Task AddVote_InvalidGuid()
    {
        var response = await client.PostAsync(
            "/Vote/AddVote/not-a-guid/user/true", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddVote_MultipleUsers()
    {
        var (user1, post) = await SetupUserAndPost("multi1");

        using var context = GetDbContext();
        var user2 = new User { Username = "multi2", Password = "123", Email = "m2@test.com" };
        context.Users.Add(user2);
        await context.SaveChangesAsync();

        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user1.Username}/true", null);
        await client.PostAsync($"/Vote/AddVote/{post.Id}/{user2.Username}/false", null);

        using var verify = GetDbContext();
        var updatedPost = await verify.Posts.FindAsync(post.Id);

        Assert.That(updatedPost!.Vote, Is.EqualTo(0));
    }

    [Test]
    public async Task AddVote_DownvoteValid()
    {
        var (user, post) = await SetupUserAndPost("downvoter");

        var response = await client.PostAsync($"/Vote/AddVote/{post.Id}/{user.Username}/false", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var context = GetDbContext();
        var dbPost = await context.Posts.FindAsync(post.Id);
        Assert.That(dbPost.Vote, Is.EqualTo(-1));
    }

    [Test]
    public async Task AddVote_UserDoesNotExist()
    {
        using var context = GetDbContext();
        var comm = new Community { Name = "noUser" };
        var post = new Post { Title = "post", Community = comm };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var response = await client.PostAsync($"/Vote/AddVote/{post.Id}/ghostUser/true", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
}