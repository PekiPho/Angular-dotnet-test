using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;



namespace ComponentTests;

public class ApiTestBase
{
    protected HttpClient client;
    protected WebApplicationFactory<Program> factory;

    private InMemoryDatabaseRoot dbRoot;

    [SetUp]
    public void SetUp()
    {
        dbRoot = new InMemoryDatabaseRoot();

        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<IspitContext>>();

                    services.AddDbContext<IspitContext>(options =>
                        options.UseInMemoryDatabase("TestDb", dbRoot));
                });
            });

        client = factory.CreateClient();
    }

    protected IspitContext GetDbContext()
    {
        var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IspitContext>();
    }

    [TearDown]
    public void TearDown()
    {
        client.Dispose();
        factory.Dispose();
    }
}