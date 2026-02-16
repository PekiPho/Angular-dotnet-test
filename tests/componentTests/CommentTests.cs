using System.Net;
using System.Net.Http.Json;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ComponentTests;
using WebTemplate.Dtos;

[TestFixture]
public class CommentTests: ApiTestBase
{
    private async Task<(User User, Post Post)> SetupUserAndPost(string uName)
    {
        using var context = GetDbContext();
        var user = new User { Username = uName, Password = "Password123", Email = $"{uName}@c.com" };
        var comm = new Community { Name =  "Comm" };
        var post = new Post { Title = "Post for comments", User=user, Community=comm, Description = "test" };
        
        context.Users.Add(user);
        context.Communities.Add(comm);
        context.Posts.Add(post);
        await context.SaveChangesAsync();
        return (user, post);
    }

    [Test]
    public async Task CreateComment_InvalidPost()
    {
        var response = await client.PostAsJsonAsync($"/Comment/CreateComment/nonexistent/{Guid.NewGuid()}/null", new Comment { Content = "fail" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task CreateComment_Valid()
    {
        var (user, post) = await SetupUserAndPost("creator");
        var comment = new Comment { Content = "hellooo" };

        var response = await client.PostAsJsonAsync($"/Comment/CreateComment/{user.Username}/{post.Id}/null", comment);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var dto = await response.Content.ReadFromJsonAsync<CommentDto>();
        Assert.That(dto.Content, Is.EqualTo("hellooo"));
    }

    [Test]
    public async Task CreateComment_ReplyLinks()
    {
        var (user, post) = await SetupUserAndPost("replier");
        
        Guid parentId;
        using var context = GetDbContext();
        var dbUser = await context.Users.FindAsync(user.Id);
        var dbPost = await context.Posts.FindAsync(post.Id);

        var parent = new Comment { Content = "parent", User = dbUser, Post = dbPost };
        context.Comments.Add(parent);
        await context.SaveChangesAsync();
        parentId = parent.Id; 

        var reply = new Comment { Content = "reply" };
        
        var response = await client.PostAsJsonAsync($"/Comment/CreateComment/{user.Username}/{post.Id}/{parentId}", reply);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        using var checkContext = GetDbContext();
        var dbReply = await checkContext.Comments.Include(c => c.ReplyTo).FirstOrDefaultAsync(c => c.Content == "reply");
        
        Assert.That(dbReply.ReplyTo, Is.Not.Null);
        Assert.That(dbReply.ReplyTo.Id, Is.EqualTo(parentId));


    }

    [Test]
    public async Task GetCommentCount_Valid()
    {
        var (user, post) = await SetupUserAndPost("counter");
        using var context = GetDbContext();
        
        var dbPost = await context.Posts.FindAsync(post.Id);
        var dbUser = await context.Users.FindAsync(user.Id);
        context.Comments.Add(new Comment { Content = "C1", Post = dbPost, User = dbUser });
        context.Comments.Add(new Comment { Content = "C2", Post = dbPost, User = dbUser });
        await context.SaveChangesAsync();
        

        var count = await client.GetFromJsonAsync<int>($"/Comment/GetCommentCount/{post.Id}");
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetCommentsFromPost_CorrectList()
    {
        var (user, post) = await SetupUserAndPost("postReader");
        using var context = GetDbContext();
        
        var dbPost = await context.Posts.FindAsync(post.Id);
        var dbUser = await context.Users.FindAsync(user.Id);
        context.Comments.Add(new Comment { Content = "startt", Post = dbPost, User = dbUser });
        await context.SaveChangesAsync();
        

        var response = await client.GetAsync($"/Comment/GetCommentsFromPost/{post.Id}");
        var results = await response.Content.ReadFromJsonAsync<List<CommentDto>>();

        Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(results[0].Content, Is.EqualTo("startt"));

    }

    [Test]
    public async Task GetCommentsFromUser_Filter()
    {
        var (user, post) = await SetupUserAndPost("userr");
        using var context = GetDbContext();
        
        var dbPost = await context.Posts.FindAsync(post.Id);
        var dbUser = await context.Users.FindAsync(user.Id);
        context.Comments.Add(new Comment { Content = "content", Post = dbPost, User = dbUser });
        await context.SaveChangesAsync();
        

        var response = await client.GetAsync($"/Comment/GetCommentsFromUser/{user.Username}");
        var results = await response.Content.ReadFromJsonAsync<List<CommentDto>>();

        Assert.That(results.All(c => c.Username == user.Username), Is.True);
    }

    [Test]
    public async Task UpdateComment_Valid()
    {
        var (user, post) = await SetupUserAndPost("updater");
        Guid commentId;
        using var context = GetDbContext();

        var c = new Comment { Content ="old", User = await context.Users.FindAsync(user.Id), Post = await context.Posts.FindAsync(post.Id) };
        context.Comments.Add(c);
        await context.SaveChangesAsync();
        commentId = c.Id;
        

        var newText = "new";
        var response = await client.PutAsync($"/Comment/UpdateComment/{commentId}/{newText}", null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        var updated = await checkContext.Comments.FindAsync(commentId);
        Assert.That(updated.Content, Is.EqualTo(newText));

    }


    [Test]
    public async Task DeleteComment_SoftDelete()
    {
        var (user, post) = await SetupUserAndPost("softDel");
        Guid commentId;
        using var context = GetDbContext();
    
        var c = new Comment { Content = "infooo", User = await context.Users.FindAsync(user.Id), Post = await context.Posts.FindAsync(post.Id) };
        context.Comments.Add(c);
        await context.SaveChangesAsync();
        commentId = c.Id;
        

        await client.DeleteAsync($"/Comment/DeleteComment/{commentId}");

        using var checkContext = GetDbContext();
        var dbComment = await checkContext.Comments.Include(c => c.User).FirstAsync(c => c.Id == commentId);
        
        Assert.Multiple(() => {
            Assert.That(dbComment.IsDeleted, Is.True);
            Assert.That(dbComment.Content, Is.EqualTo("[deleted]"));
            Assert.That(dbComment.User, Is.Null);
        });

    }

    [Test]
    public async Task AddComment_InvalidUsername()
    {
        using var context = GetDbContext();
        var comm = new Community { Name = "username" };
        var post = new Post { Title = "post", Community = comm };
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var commentDto = new CommentDto { Content = "hello", PostId = post.Id };
        
        var response = await client.PostAsJsonAsync($"/Comment/AddComment/noUser", commentDto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task UpdateComment_NonExistentComment()
    {
        var randomId = Guid.NewGuid();
        var updateDto = new CommentDto { Content = "newww" };

        var response = await client.PutAsJsonAsync($"/Comment/UpdateComment/{randomId}", updateDto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task UpdateComment_DeletedComment()
    {
        using var context = GetDbContext();
        var comment = new Comment { Content = "old", IsDeleted = true };
        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        var updateDto = new CommentDto { Content = "editDeleted" };

        var response = await client.PutAsJsonAsync($"/Comment/UpdateComment/{comment.Id}", updateDto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        
        using var checkContext = GetDbContext();
        var dbComment = await checkContext.Comments.FindAsync(comment.Id);
        Assert.That(dbComment.Content, Is.Not.EqualTo(updateDto.Content));
    }

    [Test]
    public async Task AddComment_InvalidReplyToId()
    {
        using var context = GetDbContext();
        var user = new User { Username = "replier", Email = "r@t.com", Password = "Password123" };
        var comm = new Community { Name = "replyTest" };
        var post = new Post { Title = "post", Community = comm };
        context.Users.Add(user);
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var commentDto = new CommentDto { 
            Content = "empty", 
            PostId = post.Id, 
            ReplyToId = Guid.NewGuid()
        };
        
        var response = await client.PostAsJsonAsync($"/Comment/AddComment/{user.Username}", commentDto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

}