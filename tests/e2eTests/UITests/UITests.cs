using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace e2eTests;


[TestFixture]
public class UiTests : PageTest
{
    private readonly string testUser = "Zorannn";
    private readonly string testMail = "jaaaasakdm@gmail.com";
    private readonly string testPass = "Password123";
    private readonly string commName = "Comm"; 

    private readonly string baseUrl = "http://localhost:4200";
    private readonly string apiUrl = "https://localhost:7080";

    [OneTimeSetUp]
    public async Task Setup()
    {
        //Page.SetDefaultExpectTimeout(5000);
        await CleanupEverything();
    }


    private async Task CleanupEverything()
    {
        using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        var request = await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = apiUrl,
            IgnoreHTTPSErrors = true
        });

        var communityResponse = await request.GetAsync($"Community/GetCommunityByName/{commName}");
        if (communityResponse.Status == 200)
        {
            var communityJson = (await communityResponse.JsonAsync()).Value;
            int communityId = communityJson.GetProperty("id").GetInt32();

            var userForUnsub = await request.GetAsync($"Ispit/GetUserByUsername/{testUser}");
            if (userForUnsub.Status == 200)
            {
                var userForUnsubJson = (await userForUnsub.JsonAsync()).Value;
                int userForUnsubId = userForUnsubJson.GetProperty("id").GetInt32();
                await request.DeleteAsync($"Subscribe/UnsubscribeUserFromCommunity/{userForUnsubId}/{communityId}");
            }

            await request.DeleteAsync($"Community/DeleteCommunityByName/{commName}");
        }

        var userResponse = await request.GetAsync($"Ispit/GetUserByUsername/{testUser}");
        if (userResponse.Status == 200)
        {
            var userJson = (await userResponse.JsonAsync()).Value;
            int userId = userJson.GetProperty("id").GetInt32();

            var commentsResponse = await request.GetAsync($"Comment/GetCommentsFromUser/{testUser}");
            if (commentsResponse.Status == 200)
            {
                var commentsJson = (await commentsResponse.JsonAsync()).Value;
                foreach (var comment in commentsJson.EnumerateArray())
                {
                    var commentId = comment.GetProperty("id").GetGuid();
                    await request.DeleteAsync($"Comment/ActualDelete/{commentId}");
                }
            }

            int page = 1;
            while (true)
            {
                var postsResponse = await request.GetAsync($"Post/GetPostsByUser/{testUser}/{page}");
                if (postsResponse.Status != 200)
                    break;

                var postsJson = (await postsResponse.JsonAsync()).Value;
                if (!postsJson.EnumerateArray().Any())
                    break;

                foreach (var post in postsJson.EnumerateArray())
                {
                    var postId = post.GetProperty("id").GetGuid();
                    await request.DeleteAsync($"Post/DeletePostByName/{postId}");
                }
                page++;
            }

            await request.DeleteAsync($"Ispit/DeleteUser/{userId}");
        }
    }


    private async Task PerformLogin()
    {
        await Page.GotoAsync($"{baseUrl}/login");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).FillAsync(testMail);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(testPass);
        //await Page.PauseAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        
        await Expect(Page.Locator("nav").GetByText(testUser)).ToBeVisibleAsync();
        
    }

    [Test,Order(1)]
    public async Task RegisterAndRedirect_Valid()
    {
        await Page.GotoAsync($"{baseUrl}");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).Nth(1).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(testUser);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).FillAsync(testMail);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(testPass);
        //await Page.PauseAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login$"));
    }

    [Test,Order(2)]
    public async Task LoginAndRedirect_Valid()
    {
        await Page.GotoAsync($"{baseUrl}/login");
        await Expect(Page.GetByText("Log In")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).FillAsync(testMail);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(testPass);
        //await Page.PauseAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(Page.Locator("nav").GetByText(testUser)).ToBeVisibleAsync();
    }

    [Test, Order(3)]
    public async Task CreateCommunity_Valid()
    {
        await PerformLogin();
        await Expect(Page.Locator(".bar")).ToBeVisibleAsync();
        await Page.Locator("#create").ClickAsync(new() { Force = true });
        var modal = Page.Locator(".add-modal");
        await Expect(modal).ToBeVisibleAsync();
        await Page.Locator("#name").FillAsync(commName);
        await Page.Locator("#description").FillAsync("Descccc");
        //await Page.PauseAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save changes" }).ClickAsync();
        //await Page.PauseAsync();
        await Expect(modal).ToBeHiddenAsync();
        await Expect(Page.Locator(".bar").GetByText(commName, new() { Exact = true })).ToBeVisibleAsync();   
    }

    [Test, Order(4)]
    public async Task NavigateToCommunity_VerifyDescription()
    {
        await PerformLogin();
        await Page.Locator(".bar").GetByText("Comm", new() { Exact = true }).ClickAsync(new() { Force = true });
        await Expect(Page.Locator(".comm-card").GetByText("Comm")).ToBeVisibleAsync();
        
        
    }

    [Test, Order(5)]
    public async Task NavigateToCommunity_VerifySubs()
    {
        await PerformLogin();
        await Page.Locator(".bar").GetByText("Comm", new() { Exact = true }).ClickAsync(new() { Force = true });
        await Expect(Page.Locator(".comm-card").GetByText("1")).ToBeVisibleAsync();
    }

    [Test, Order(6)]
    public async Task NavigateToCommunity_VerifyDesc()
    {
        await PerformLogin();
        await Page.Locator(".bar").GetByText("Comm", new() { Exact = true }).ClickAsync(new() { Force = true });
        await Expect(Page.Locator(".comm-card").GetByText("Descccc")).ToBeVisibleAsync();
    }

    [Test, Order(7)]
    public async Task NavigateToCommunity_AndBackToHome()
    {
        await PerformLogin();
        await Page.Locator(".bar").GetByText("Comm", new() { Exact = true }).ClickAsync(new() { Force = true });
        await Expect(Page.Locator(".comm-card")).ToBeVisibleAsync();
        await Page.GetByText("Peki App").ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/mainPage$"));
    }

    [Test, Order(8)]
    public async Task CreatePost_VerifyOnMainPage()
    {
        await PerformLogin();
        await Page.GetByText("Create post").ClickAsync();
        await Page.Locator("#title").FillAsync("Title1");
        await Page.Locator("#community").FillAsync("Comm");
        await Page.Locator("#description").FillAsync("description test");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save changes" }).ClickAsync();
        await Expect(Page.GetByText("Title1")).ToBeVisibleAsync();
    }

    [Test, Order(9)]
    public async Task UpvotePost_VerifyCountIncrements()
    {
        await PerformLogin();
        var post = Page.Locator("post").Filter(new() { HasText = "Title1" });
        await post.Locator(".upvote-btn").ClickAsync();
        var voteCount = post.Locator(".vote-count");
        await Expect(voteCount).ToHaveTextAsync("1");
        await Expect(post.Locator(".upvote-btn")).ToHaveClassAsync(new Regex(".*upvoted.*"));
    }

    [Test, Order(10)]
    public async Task ToggleUpvote_VerifyCountReturnsToZero()
    {
        await PerformLogin();
        var post = Page.Locator("post").Filter(new() { HasText = "Title1" });
        await post.Locator(".upvote-btn").ClickAsync();
        await Expect(post.Locator(".vote-count")).ToHaveTextAsync("0");
    }

    [Test, Order(11)]
    public async Task DownvotePost_VerifyCountIsNegativeOne()
    {
        await PerformLogin();
        var post = Page.Locator("post").Filter(new() { HasText = "Title1" });
        await post.Locator(".downvote-btn").ClickAsync();
        await Expect(post.Locator(".vote-count")).ToHaveTextAsync("-1");
    }

    [Test, Order(12)]
    public async Task ToggleDownvote_VerifyCountReturnsToZero()
    {
        await PerformLogin();
        var post = Page.Locator("post").Filter(new() { HasText = "Title1" });
        await post.Locator(".downvote-btn").ClickAsync();
        await Expect(post.Locator(".vote-count")).ToHaveTextAsync("0");
    }

    [OneTimeTearDown]
    public async Task GlobalCleanup()
    {
        await CleanupEverything();
    }

}