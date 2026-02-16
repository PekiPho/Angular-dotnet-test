using System.Net;
using System.Net.Http.Json;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ComponentTests;
using WebTemplate.Dtos;

[TestFixture]
public class SearchTests: ApiTestBase
{
    [Test]
    public async Task OnTypeCommunities_ReturnFive()
    {
        using var context = GetDbContext();
        
        for (int i = 1; i <= 10; i++)
            context.Communities.Add(new Community { Name = $"comm{i}" });
        await context.SaveChangesAsync();
        

        var response = await client.GetFromJsonAsync<List<Community>>("/Search/OnTypeCommunities/comm");

        Assert.That(response.Count, Is.EqualTo(5));
        Assert.That(response.All(c => c.Name.Contains("comm")), Is.True);
    }

    [Test]
    public async Task GetCommunitiesAndPosts()
    {
        using var context = GetDbContext();
        
        var comm = new Community { Name = "test" };
        context.Communities.Add(comm);
        context.Posts.Add(new Post { Title = "test", Community = comm, Description = "test" });
        await context.SaveChangesAsync();


        var response = await client.GetAsync("/Search/GetCommunitiesAndPosts/test");
        var result = await response.Content.ReadFromJsonAsync<dynamic>();

        Assert.That(result.GetProperty("communities").GetArrayLength(), Is.GreaterThanOrEqualTo(1));
        Assert.That(result.GetProperty("posts").GetArrayLength(), Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task GetCommunitiesAndPosts_HotNegative()
    {
        using var context = GetDbContext();
        
        var comm = new Community { Name = "randome" };
        context.Posts.Add(new Post { Title = "Negative", Vote = -10, Community = comm, DateOfPost = DateTime.Now });
        context.Posts.Add(new Post { Title = "Positive", Vote = 100, Community = comm, DateOfPost = DateTime.Now });
        await context.SaveChangesAsync();
        

        var response = await client.GetAsync("/Search/GetCommunitiesAndPosts/tive");
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }


    [Test]
    public async Task GetCommunitiesAndPosts_Pagination()
    {
        using var context = GetDbContext();
        var comm = new Community { Name = "Pagination" };
        for (int i = 1; i <= 15; i++)
        {
            context.Posts.Add(new Post { Title = $"mult{i}", Community = comm, DateOfPost = DateTime.Now });
        }
        await context.SaveChangesAsync();
        

        var response = await client.GetAsync("/Search/GetCommunitiesAndPosts/mult/2?pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        
        Assert.That(result.GetProperty("posts").GetArrayLength(), Is.EqualTo(5));
    }

}