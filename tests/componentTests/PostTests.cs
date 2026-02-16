using System.Net;
using System.Net.Http.Json;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ComponentTests;
using WebTemplate.Dtos;

[TestFixture]
public class PostTests: ApiTestBase
{
    private async Task<(User User, Community Comm)> SetupUserAndCommunity(string uName, string cName)
    {
        var user = new User { Username = uName, Password = "Password123", Email = $"{uName}@test.com" };
        var comm = new Community { Name = cName, Description = "Test Desc" };
        
        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.Add(comm);
        await context.SaveChangesAsync();
        
        return (user, comm);
    }


    [Test]
    public async Task AddPost_ValidData()
    {
        var (user, comm) = await SetupUserAndCommunity("username", "vest");
        var post = new Post { Title = "first post", Description = "hellooo" };

        var response = await client.PostAsJsonAsync($"/Post/AddPost/{user.Id}/{comm.Id}", post);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("Added post with ID"));
    }


    [Test]
    public async Task AddPostByName_ValidDto(){
        var (user, comm) = await SetupUserAndCommunity("username", "game");
        var post = new Post { Title = "testtest", Description = "random" };

        var response = await client.PostAsJsonAsync($"/Post/AddPostByName/{user.Username}/{comm.Name}", post);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var dto = await response.Content.ReadFromJsonAsync<PostDto>();
        Assert.That(dto.Title, Is.EqualTo("testtest"));
    }

    [Test]
    public async Task AddPost_DuplicateTitle()
    {
        var (user, comm) = await SetupUserAndCommunity("duplikat", "duplikat");
        
        using var context = GetDbContext();
        var dbUser = await context.Users.FindAsync(user.Id);
        var dbComm = await context.Communities.FindAsync(comm.Id);

        context.Posts.Add(new Post { Title = "unique", Description = "prvi", User = dbUser, Community = dbComm });
        await context.SaveChangesAsync();

        var post2 = new Post { Title = "unique", Description = "drugi" };
        var response = await client.PostAsJsonAsync($"/Post/AddPostByName/{user.Username}/{comm.Name}", post2);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetPostById_ReturnsPost()
    {
        var (user, comm) = await SetupUserAndCommunity("citac", "knjige");
    
        using var context = GetDbContext();
        var dbUser = await context.Users.FindAsync(user.Id);
        var dbComm = await context.Communities.FindAsync(comm.Id);
        
        var post = new Post { Title = "review", User = dbUser, Community = dbComm };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var response = await client.GetAsync($"/Post/GetPostByID/{post.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var dto = await response.Content.ReadFromJsonAsync<PostDto>();
        Assert.That(dto.Id, Is.EqualTo(post.Id));
        
    }

    [Test]
    public async Task GetPostsByUser_Count()
    {
        var (user, comm) = await SetupUserAndCommunity("visePosta", "general");
    
        using var context = GetDbContext();
        var dbUser = await context.Users.FindAsync(user.Id);
        var dbComm = await context.Communities.FindAsync(comm.Id);
        
        context.Posts.Add(new Post { Title = "P1", User = dbUser, Community = dbComm });
        context.Posts.Add(new Post { Title = "P2", User = dbUser, Community = dbComm });
        await context.SaveChangesAsync();

        var response = await client.GetAsync($"/Post/GetPostsByUser/{user.Username}/1");

        var results = await response.Content.ReadFromJsonAsync<List<PostDto>>();
        Assert.That(results.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetHotPosts_FiltersSubscribed()
    {
        var user = new User { Username = "subscriber", Password ="Password123", Email ="sub@t.com" };
        var comm1 = new Community { Name = "subbedComm" };
        var comm2 = new Community { Name = "otherComm" };
        user.Subscribed = new List<Community> { comm1 };

        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.AddRange(comm1, comm2);
        context.Posts.Add(new Post { Title = "inSub", Community = comm1, User = user });
        context.Posts.Add(new Post { Title = "notInSub", Community = comm2, User = user });
        await context.SaveChangesAsync();

        var response = await client.GetAsync($"/Post/GetHotPosts/{user.Username}/1");
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();

        Assert.That(posts.All(p => p.CommunityName == "subbedComm"), Is.True);

    }


    [Test]
    public async Task UpdateDescription_InvalidPost()
    {
        var response = await client.PutAsync($"/Post/UpdateDescription/{Guid.NewGuid()}/SomeText", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdateTitle_Valid()
    {
        var (user, comm) = await SetupUserAndCommunity("updater", "tech");
    
        using var context = GetDbContext();
        var dbUser = await context.Users.FindAsync(user.Id);
        var dbComm = await context.Communities.FindAsync(comm.Id);
        
        var post = new Post { Title = "old title", User = dbUser, Community = dbComm };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var newTitle = "new title";
        var response = await client.PutAsync($"/Post/UpdateTitle/{post.Id}/{newTitle}", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        var updated = await checkContext.Posts.FindAsync(post.Id);
        Assert.That(updated.Title, Is.EqualTo(newTitle));
    }

    [Test]
    public async Task DeletePostByName_Valid()
    {
        var (user, comm) = await SetupUserAndCommunity("deleter", "tech");
    
        using var context = GetDbContext();
        var dbUser = await context.Users.FindAsync(user.Id);
        var dbComm = await context.Communities.FindAsync(comm.Id);
        
        var post = new Post { Title = "to delete", User = dbUser, Community = dbComm };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Post/DeletePostByName/{post.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        Assert.That(await checkContext.Posts.AnyAsync(p => p.Id == post.Id), Is.False);
    }

    [Test]
    public async Task DeleteUser_PostsStay()
    {
        var (user, comm) = await SetupUserAndCommunity("aaaa", "bbbb");
    
        using var context = GetDbContext();
        var dbUser = await context.Users.FindAsync(user.Id);
        var dbComm = await context.Communities.FindAsync(comm.Id);
        
        var post = new Post { Title = "aaa post", User = dbUser, Community = dbComm };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        await client.DeleteAsync($"/Ispit/DeleteUser/{user.Id}");

        using var checkContext = GetDbContext();
        var dbPost = await checkContext.Posts.FirstOrDefaultAsync(p => p.Title == "aaa post");
        
        Assert.That(dbPost, Is.Not.Null);
        Assert.That(dbPost.User, Is.Null); 


    }

    
    [Test]
    public async Task GetPostsBySort_InvalidSort()
    {
        var response = await client.GetAsync("/Post/GetPostsBySort/testComm/NotARealSort/1/10/All time");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Contains.Substring("No such sort"));
    }

    [Test]
    public async Task UpdateTitle_InvalidId()
    {
        var randomId = Guid.NewGuid();
        var response = await client.PutAsync($"/Post/UpdateTitle/{randomId}/NewTitle", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Contains.Substring("Post does not exist"));
    }

    [Test]
    public async Task UpdateDescription_InvaldId()
    {
        var randomId = Guid.NewGuid();
        var response = await client.PutAsync($"/Post/UpdateDescription/{randomId}/newDesc", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddPostByName_UserDoesNotExist()
    {
        using var context = GetDbContext();
        context.Communities.Add(new Community { Name = "exists" });
        await context.SaveChangesAsync();

        var post = new Post { Title = "Title", Description = "Desc" };
        var response = await client.PostAsJsonAsync("/Post/AddPostByName/ghostUser/exists", post);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Contains.Substring("User does not exist"));
    }

    [Test]
    public async Task AddPostByName_CommunityDontExist()
    {
        using var context = GetDbContext();
        context.Users.Add(new User { Username = "real", Email = "u@t.com", Password = "123" });
        await context.SaveChangesAsync();

        var post = new Post { Title = "title", Description = "desc" };
        var response = await client.PostAsJsonAsync("/Post/AddPostByName/real/fake", post);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Contains.Substring("Community does not exist"));
    }

    [Test]
    public async Task AddPost_UserOrCommunityNull()
    {
        var post = new Post { Title = "fail", Description = "..." };
        var response = await client.PostAsJsonAsync("/Post/AddPost/0/0", post);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Contains.Substring("User or community don't exist"));
    }

    [Test]
    public async Task AddPost_TitleExceeded()
    {
        using var context = GetDbContext();
        var user = new User { Username = "valid", Email = "v@t.com", Password = "Password123" };
        var comm = new Community { Name = "valid" };
        context.Users.Add(user);
        context.Communities.Add(comm);
        await context.SaveChangesAsync();

        var longTitle = new string('A', 251);
        var post = new Post { Title = longTitle, Description = "lengthhh" };

        var response = await client.PostAsJsonAsync($"/Post/AddPostByName/{user.Username}/{comm.Name}", post);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DeletePostByName_NonExistingPost()
    {
        var randomId = Guid.NewGuid();
        var response = await client.DeleteAsync($"/Post/DeletePostByName/{randomId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Contains.Substring("Post does not exist"));
    }

    [Test]
    public async Task AddPostWithMedia_InvalidJson()
    {
        using var context = GetDbContext();
        context.Users.Add(new User { Username = "json", Email = "j@t.com", Password = "123" });
        context.Communities.Add(new Community { Name = "json" });
        await context.SaveChangesAsync();

        var content = new MultipartFormDataContent();
        content.Add(new StringContent("{ 'Title': 'nema viticasta zagrada' "), "postJson"); 
        var fileContent = new ByteArrayContent(new byte[0]);
        content.Add(fileContent, "file", "test.jpg");

        var response = await client.PostAsync("/Post/AddPostWithMedia/json/json", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.That(responseString, Contains.Substring("Invalid post data"));
    }

    [Test]
    public async Task DeletePostByName_RemovesAllData()
    {
        Guid postId;
        using var context = GetDbContext();
    
        var user = new User { Username = "cleanupUser", Email = "c@t.com", Password = "123" };
        var post = new Post { Title = "Cleanup Post", User = user };
        
        var postVote = new Vote { Post = post, User = user, VoteValue = true };
        
        var comment = new Comment { Content = "Cleanup Comment", Post = post };
        
        var commentVote = new CommentVote { Comment = comment, User = user, VoteValue = true };

        context.Posts.Add(post);
        context.Votes.Add(postVote);
        context.Comments.Add(comment);
        context.CommentVotes.Add(commentVote);
        await context.SaveChangesAsync();
        
        postId = post.Id;
        

        var response = await client.DeleteAsync($"/Post/DeletePostByName/{postId}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        Assert.Multiple(() =>
        {
            Assert.That(checkContext.Posts.Any(p => p.Id == postId), Is.False);
            Assert.That(checkContext.Votes.Any(v => v.Post.Id == postId), Is.False);
            Assert.That(checkContext.Comments.Any(c => c.Post.Id == postId), Is.False);
            Assert.That(checkContext.CommentVotes.Count(), Is.EqualTo(0));
        });
    }

    [Test]
    public async Task GetPostsBySort_TopInvalidTime()
    {
        var response = await client.GetAsync("/Post/GetPostsBySort/AnyComm/Top/1/10/vek");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Contains.Substring("Wrong time parameter"));
    }

    [Test]
    public async Task GetHotPosts_PaginationValid()
    {
        using var context = GetDbContext();
        var user = new User { Username = "pageUser", Email = "p@t.com", Password = "123" };
        context.Users.Add(user);

        for (int i = 0; i < 60; i++)
        {
            context.Posts.Add(new Post { Title = $"post{i}", DateOfPost = DateTime.UtcNow, User = user });
        }
        await context.SaveChangesAsync();

        var resp1 = await client.GetAsync("/Post/GetHotPosts/pageUser/1");
        var page1 = await resp1.Content.ReadFromJsonAsync<List<PostDto>>();

        var resp2 = await client.GetAsync("/Post/GetHotPosts/pageUser/2");
        var page2 = await resp2.Content.ReadFromJsonAsync<List<PostDto>>();

        Assert.Multiple(() => {
            Assert.That(page1.Count, Is.EqualTo(50));
            Assert.That(page2.Count, Is.EqualTo(10));
        });
    }

    [Test]
    public async Task GetHotPosts_NoSubscripeGlobal()
    {
        using var context = GetDbContext();
        var user = new User { Username = "user", Email = "l@t.com", Password = "123" };
        var comm = new Community { Name = "gen" };
        context.Users.Add(user);
        context.Posts.Add(new Post { Title = "postt", Community = comm, DateOfPost = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var response = await client.GetAsync("/Post/GetHotPosts/user/1");
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();

        Assert.That(posts.Any(p => p.Title == "postt"), Is.True);
    }

    [Test]
    public async Task GetPostsBySort_TopTodayValidOrder()
    {
        using var context = GetDbContext();
        var comm = new Community { Name = "sort" };
        context.Posts.Add(new Post { Title = "low", Vote = 10, Community = comm, DateOfPost = DateTime.UtcNow });
        context.Posts.Add(new Post { Title = "high", Vote = 100, Community = comm, DateOfPost = DateTime.UtcNow });

        context.Posts.Add(new Post { Title = "old", Vote = 500, Community = comm, DateOfPost = DateTime.UtcNow.AddDays(-5) });
        await context.SaveChangesAsync();

        var response = await client.GetAsync("/Post/GetPostsBySort/sort/Top/1/10/Today");
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();

        Assert.Multiple(() => {
            Assert.That(posts.Count, Is.EqualTo(2));
            Assert.That(posts[0].Title, Is.EqualTo("high"));
            Assert.That(posts.Any(p => p.Title == "old"), Is.False);
        });
    }
}

