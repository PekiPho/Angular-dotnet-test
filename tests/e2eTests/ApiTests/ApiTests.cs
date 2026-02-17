using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.Json;

namespace e2eTests;

[TestFixture]
public class ApiTests : PlaywrightTest
{
    private IAPIRequestContext request = null!;
    private string token = "";
    private readonly string baseUrl = "https://localhost:7080";

    private static readonly string suffix = Guid.NewGuid().ToString("N")[..6];
    private readonly string testUser = "User" + suffix;
    private readonly string testMail = "test" + suffix + "@gmail.com";
    private readonly string testPass = "Password123";
    private readonly string commName = "Comm" + suffix;
    
    private Guid postId;
    private Guid commentId;
    private int communityId;
    private int userId;

    private IPlaywright pw = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        pw = await Microsoft.Playwright.Playwright.CreateAsync();
        request = await pw.APIRequest.NewContextAsync(new()
        {
            BaseURL = baseUrl,
            IgnoreHTTPSErrors = true
        });
    }

    [Test, Order(1)]
    public async Task AddUser_Valid()
    {
        var response = await request.PostAsync("Ispit/AddUser", new()
        {
            DataObject = new { Username = testUser, Password = testPass, Email = testMail }
        });
        Assert.That(response.Status, Is.EqualTo(200));
        var json = await response.JsonAsync();
        token = json?.GetProperty("token").GetString() ?? "";
        
        request = await pw.APIRequest.NewContextAsync(new()
        {
            BaseURL = baseUrl,
            IgnoreHTTPSErrors = true,
            ExtraHTTPHeaders = new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } }
        });
    }

    [Test, Order(2)]
    public async Task AddUser_Duplicate()
    {
        var response = await request.PostAsync("Ispit/AddUser", new()
        {
            DataObject = new { Username = testUser, Password = testPass, Email = "other@mail.com" }
        });
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test, Order(3)]
    public async Task Login_Valid()
    {
        var response = await request.GetAsync($"Ispit/GetUserByMailAndPassword/{testMail}/{testPass}");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(4)]
    public async Task GetEntry_Valid()
    {
        var response = await request.GetAsync("Ispit/GetEntry");
        var json = await response.JsonAsync();
        userId = json?.GetProperty("id").GetInt32() ?? 0;
        Assert.That(userId, Is.GreaterThan(0));
    }

    [Test, Order(5)]
    public async Task CreateCommunity_Valid()
    {
        var response = await request.PostAsync("Community/CreateCommunity", new()
        {
            DataObject = new { Name = commName, Description = "Test" }
        });
        var json = await response.JsonAsync();
        communityId = json?.GetProperty("id").GetInt32() ?? 0;
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(6)]
    public async Task CreateCommunity_Duplicate()
    {
        var response = await request.PostAsync("Community/CreateCommunity", new()
        {
            DataObject = new { Name = commName, Description = "Test" }
        });
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test, Order(7)]
    public async Task SubscribeUser_Valid()
    {
        var response = await request.PostAsync($"Subscribe/SubscribeUserToCommunity/{userId}/{communityId}");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(8)]
    public async Task AddModerator_Valid()
    {
        var response = await request.PostAsync($"Subscribe/AddUserToMod/{testUser}/{commName}");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(9)]
    public async Task AddPost_Valid()
    {
        var response = await request.PostAsync($"Post/AddPostByName/{testUser}/{commName}", new()
        {
            DataObject = new { Title = "Post" + suffix, Description = "Content" }
        });
        var json = await response.JsonAsync();
        postId = json?.GetProperty("id").GetGuid() ?? Guid.Empty;
        Assert.That(postId, Is.Not.EqualTo(Guid.Empty));
    }

    [Test, Order(10)]
    public async Task AddPost_DuplicateTitle()
    {
        var response = await request.PostAsync($"Post/AddPostByName/{testUser}/{commName}", new()
        {
            DataObject = new { Title = "Post" + suffix, Description = "Content" }
        });
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test, Order(11)]
    public async Task VotePost_Valid()
    {
        var response = await request.PostAsync($"Vote/AddVote/{postId}/{testUser}/true");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(12)]
    public async Task CreateComment_Valid()
    {
        var response = await request.PostAsync($"Comment/CreateComment/{testUser}/{postId}/null", new()
        {
            DataObject = new { Content = "Comment Content" }
        });
        var json = await response.JsonAsync();
        commentId = json?.GetProperty("id").GetGuid() ?? Guid.Empty;
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(13)]
    public async Task VoteComment_Valid()
    {
        var response = await request.PostAsync($"Vote/Comment/AddCommentVote/{commentId}/{testUser}/true");
        Assert.That(new[] { 200, 204 }, Does.Contain(response.Status));
    }

    [Test, Order(14)]
    public async Task SearchType_Valid()
    {
        var response = await request.GetAsync($"Search/OnTypeCommunities/{commName[..3]}");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(15)]
    public async Task GetHotPosts_Valid()
    {
        var response = await request.GetAsync($"Post/GetHotPosts/{testUser}/1");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(16)]
    public async Task UpdateTitle_Valid()
    {
        var response = await request.PutAsync($"Post/UpdateTitle/{postId}/NewTitle");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(17)]
    public async Task GetPostById_Valid()
    {
        var response = await request.GetAsync($"Post/GetPostByID/{postId}");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(18)]
    public async Task GetModerators_Valid()
    {
        var response = await request.GetAsync($"Subscribe/FindModeratorsFromCommunity/{commName}");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(19)]
    public async Task Unsubscribe_Valid()
    {
        var response = await request.DeleteAsync($"Subscribe/UnsubscribeUserFromCommunity/{userId}/{communityId}");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test, Order(20)]
    public async Task SearchAll_Valid()
    {
        var response = await request.GetAsync($"Search/GetCommunitiesAndPosts/{suffix}");
        Assert.That(response.Status, Is.EqualTo(200));
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        if (request != null)
        {
            if (commentId != Guid.Empty) await request.DeleteAsync($"Comment/ActualDelete/{commentId}");
            if (postId != Guid.Empty) await request.DeleteAsync($"Post/DeletePostByName/{postId}");
            if (communityId != 0) await request.DeleteAsync($"Community/DeleteCommunity/{communityId}");
            if (userId != 0) await request.DeleteAsync($"Ispit/DeleteUser/{userId}");
            
            await request.DisposeAsync();
        }
        pw?.Dispose();
    }
}