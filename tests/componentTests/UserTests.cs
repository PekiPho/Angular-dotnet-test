using System.Net;
using System.Net.Http.Json;
using WebTemplate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ComponentTests;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}

[TestFixture]
public class UserTests: ApiTestBase
{
    [Test]
    public async Task AddUser_Valid()
    {
        var user = new User { Username = "valid", Password = "Password123", Email = "val@test.com" };
        var response = await client.PostAsJsonAsync("/Ispit/AddUser", user);


        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task AddUser_VerifyTokenGenerated()
    {
        var user = new User { Username = "tokennnnnn", Password = "Password123", Email = "tok@test.com" };
    
        var response = await client.PostAsJsonAsync("/Ispit/AddUser", user);


        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(); 
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Token.Length, Is.GreaterThan(50));
    }

    [Test]
    public async Task AddUser_VerifyDatabaseSave()
    {
        var rawPass = "Password123";
        var name = "agfslkjnn";
        var user = new User { Username = name, Password = rawPass, Email = "sav@test.com" };
        
        await client.PostAsJsonAsync("/Ispit/AddUser", user);

        using var context = GetDbContext();
        var saved = await context.Users.FirstOrDefaultAsync(u => u.Username == name);
        
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved.Password, Is.Not.EqualTo(rawPass));
    }

    [Test]
    public async Task AddUser_DuplicateUsername()
    {
        var user1 = new User { Username = "Duplikat", Password = "Password123", Email = "1@test.com" };
        using var context = GetDbContext();
        context.Users.Add(user1);
        await context.SaveChangesAsync();
        

        var user2 = new User { Username = "Duplikat", Password = "AnotherPassword1", Email = "2@test.com" };
        var response = await client.PostAsJsonAsync("/Ispit/AddUser", user2);

        Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task AddUser_InvalidEmail()
    {
        var user = new User { Username = "EmailTest", Password = "Password123", Email = "nije_email" };

        var response = await client.PostAsJsonAsync("/Ispit/AddUser", user);


        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddUser_UsernameShort()
    {
        var user = new User { Username = "Abc", Password = "Password123", Email = "abc@test.com" };

        var response = await client.PostAsJsonAsync("/Ispit/AddUser", user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    }

    [Test]
    public async Task GetUserUsername_CaseInsensitive_Valid()
    {
        using var context = GetDbContext();
        context.Users.Add(new User { Username = "TestUser", Password = "Password1", Email = "testmail@test.com" });
        await context.SaveChangesAsync();

        var response = await client.GetAsync("/Ispit/GetUserByUsername/testuser");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetUserId_NotFound()
    {
        var response = await client.GetAsync("/Ispit/GetUserById/9999");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    }

    [Test]
    public async Task GetUsers_ReturnsAll()
    {
        using var context = GetDbContext();
        context.Users.Add(new User { Username = "UserA", Password = "P1", Email = "a@t.com" });
        context.Users.Add(new User { Username = "UserB", Password = "P2", Email = "b@t.com" });
        await context.SaveChangesAsync();

        var response = await client.GetAsync("/Ispit/GetUsers");
        var result = await response.Content.ReadFromJsonAsync<List<User>>();

        Assert.That(result?.Count, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task UpdatePassword_CorrectRegex()
    {
        var username = "Regex";
        using var context = GetDbContext();
        context.Users.Add(new User { Username = username, Password = "OldPassw1", Email = "r@t.com" });
        await context.SaveChangesAsync();

        var response = await client.PutAsync($"/Ispit/UpdatePassword/{username}/NewPasswStronk12", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdatePassword_NoDigit()
    {
        var username = "DigitsTest";
        using var context = GetDbContext();
        context.Users.Add(new User { Username = username, Password ="OldPassword1", Email ="d@t.com" });
        await context.SaveChangesAsync();

        var response = await client.PutAsync($"/Ispit/UpdatePassword/{username}/NoDigitsss", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    }

    [Test]
    public async Task UpdatePassword_UserNotFound()
    {
        var response = await client.PutAsync("/Ispit/UpdatePassword/DoesntExist/Pass12345", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DeleteAccount_UserDoesNotExist()
    {
        var response = await client.DeleteAsync("/Ispit/DeleteUser/1563163");

        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.BadRequest));

    }

    [Test]
    public async Task DeleteAccount_Valid()
    {
        var user = new User { Username ="toDelete", Password ="Passwordsdsa12", Email ="d@t.com" };
        using var context = GetDbContext();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = await client.DeleteAsync($"/Ispit/DeleteUser/{user.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task DeleteUser_WithSubscribes()
    {
        var user = new User { Username = "Connected", Password = "Password123", Email = "conn@test.com" };
        var comm = new Community { Name = "TestCommmmm" };
        
        using var context = GetDbContext();
        context.Users.Add(user);
        context.Communities.Add(comm);

        user.Subscribed = new List<Community> { comm };
        await context.SaveChangesAsync();
        
        var userId = user.Id;

        var response = await client.DeleteAsync($"/Ispit/DeleteUser/{userId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var checkContext = GetDbContext();
        var deletedUser = await checkContext.Users.FindAsync(userId);
        Assert.That(deletedUser, Is.Null);

        var communityStillExists = await checkContext.Communities.AnyAsync(c => c.Name== "TestCommmmm");
        Assert.That(communityStillExists, Is.True);
    }

    [Test]
    public async Task AddUser_PasswordTooShort()
    {
        var user = new User { Username = "short", Password = "Pa1", Email = "s@t.com" };
        var response = await client.PostAsJsonAsync("/Ispit/AddUser", user);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddUser_NoUppercase()
    {
        var user = new User { Username = "noUpper", Password = "password123", Email = "n@t.com" };
        var response = await client.PostAsJsonAsync("/Ispit/AddUser", user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    }

    [Test]
    public async Task AddUser_NoLowercase()
    {
        var user = new User { Username = "noLower", Password = "PASSWORD123", Email = "l@t.com" };
        var response = await client.PostAsJsonAsync("/Ispit/AddUser", user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    }

    [Test]
    public async Task AddUser_NullBody()
    {
        var response = await client.PostAsJsonAsync<User>("/Ispit/AddUser", null!);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Login_Valid_ReturnsToken()
    {
        var e ="v@t.com";
        var p= "Pass1234";
        var u =new User { Username= "user1", Password= p, Email =e };
        await client.PostAsJsonAsync("/Ispit/AddUser", u);

        var res= await client.GetAsync($"/Ispit/GetUserByMailAndPassword/{e}/{p}");

        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var outObj =await res.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.That(outObj, Is.Not.Null);
        Assert.That(outObj.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Login_WrongPass_Returns400()
    {
        var e = "w@t.com";
        var u = new User { Username ="user2", Password ="Correct123", Email= e };
        await client.PostAsJsonAsync("/Ispit/AddUser", u);

        var res = await client.GetAsync($"/Ispit/GetUserByMailAndPassword/{e}/Wrong123");

        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var msg = await res.Content.ReadAsStringAsync();
        Assert.That(msg, Is.EqualTo("Wrong email or password"));
    }

    [Test]
    public async Task Login_BadMail_Returns400()
    {
        var res = await client.GetAsync("/Ispit/GetUserByMailAndPassword/none@t.com/Pass123");

        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var msg =await res.Content.ReadAsStringAsync();
        Assert.That(msg, Is.EqualTo("Wrong email or password"));
    }

    [Test]
    public async Task UpdatePass_VerifyDb()
    {
        var username = "u3";
        var oldP = "OldPass123";
        var newP = "NewPass123";
        
        using var ctx= GetDbContext();
        ctx.Users.Add(new User { Username= username, Password= oldP, Email = "u3@t.com" });
        await ctx.SaveChangesAsync();

        await client.PutAsync($"/Ispit/UpdatePassword/{username}/{newP}", null);

        using var checkContext =GetDbContext();
        var dbUser = await checkContext.Users.FirstAsync(u=> u.Username ==username);
        
        var h = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        var result = h.VerifyHashedPassword(dbUser, dbUser.Password, newP);
        
        Assert.That(result, Is.EqualTo(Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success));
    }
    
}