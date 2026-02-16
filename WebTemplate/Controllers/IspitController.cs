using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
//[EnableCors("CORS")]
public class IspitController : ControllerBase
{
    public IspitContext Context { get; set; }

    public IspitController(IspitContext context)
    {
        Context = context;
    }

    [HttpPost("AddUser")]
    public async Task<ActionResult> AddUser([FromBody] User user){

        var exists = await Context.Users.AnyAsync(u => u.Username == user.Username);
        if (exists)
        {
            return BadRequest("Username already exists");
        }

        var hasher = new PasswordHasher<User>();
        user.Password = hasher.HashPassword(user, user.Password);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var claims=new[]{
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub,user.Email),
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti,user.Id.ToString())
        };

        var token=new JwtSecurityToken(
            issuer:"http://localhost:4200",
            audience:"http://localhost:4200",
            claims:claims,
            expires:DateTime.Now.AddDays(7),
            signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Super-Duper-Tajan-Kljuc-Ide-Gas-Brate-Najjace-Peki-App-LOoooool")),SecurityAlgorithms.HmacSha256)
        );

        return Ok(new{
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration=token.ValidTo
        });
    }

    [HttpPut("UpdatePassword/{username}/{newPassword}")]
    public async Task<ActionResult> UpdatePassword(string username, string newPassword)
    {
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$";

        bool isMatch = Regex.IsMatch(newPassword, pattern);

        if (!isMatch)
            return BadRequest("Weak password");

        var user = await Context.Users.FirstOrDefaultAsync(c => c.Username == username);

        if (user == null)
            return BadRequest("User does not exist");

        var hasher = new PasswordHasher<User>();

        user.Password = hasher.HashPassword(user, newPassword);

        Context.Users.Update(user);

        await Context.SaveChangesAsync();

        return Ok("Password updated successfully!");
    }

    

    [HttpDelete("DeleteUser/{id}")]
    public async Task<ActionResult> DeleteUser(int id){

        var user = await Context.Users.Where(x=>x.Id == id).FirstOrDefaultAsync();

        if(user==null)
            return BadRequest("User does not exist");


        Context.Users.Remove(user);
        await Context.SaveChangesAsync();
        return Ok("User deleted");
    }


    [HttpGet("GetUsers")]
    public async Task<ActionResult> GetUsers(){
            return Ok(await Context.Users.ToListAsync()); 
    }

    [HttpGet("GetUserById/{id}")]
    public async Task<ActionResult> GetUserById(int id){

        var user= await Context.Users.Where(x=>x.Id == id).FirstOrDefaultAsync();

        if(user==null)
            return BadRequest($"User with ID of: {id} does not exist");

        return Ok(user);
    }

    [HttpGet("GetUserByUsername/{username}")]
    public async Task<ActionResult> GetUserByUsername(string username){
        var user=await Context.Users.Where(x=>x.Username.ToLower()== username.ToLower()).FirstOrDefaultAsync();

        if(user==null)
            return BadRequest($"User with username of {username} does not exist");
        
        return Ok(user);
    }

    [HttpGet("GetUserByMailAndPassword/{mail}/{password}")]
    public async Task<ActionResult> GetUserByMailAndPassword(string mail,string password){

        var users= await Context.Users.Where(x=>x.Email == mail).ToListAsync();

        User? user = null; ;
        var hasher = new PasswordHasher<User>();

        foreach (var u in users)
        {
            if (hasher.VerifyHashedPassword(u, u.Password, password) == PasswordVerificationResult.Success)
            {
                user = u;
                break;
            }
               
        }

        if (user == null)
        {
            return BadRequest("Wrong email or password");
        }

        var claims=new[]{
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub,user.Email),
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti,user.Id.ToString())
        };

        var token=new JwtSecurityToken(
            issuer:"http://localhost:4200",
            audience:"http://localhost:4200",
            claims:claims,
            expires:DateTime.Now.AddDays(7),
            signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Super-Duper-Tajan-Kljuc-Ide-Gas-Brate-Najjace-Peki-App-LOoooool")),SecurityAlgorithms.HmacSha256)
        );

        return Ok(new{
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration=token.ValidTo,
        });
       
    }

    
    
    [Authorize]
    [HttpGet("GetEntry")]
    public async Task<ActionResult> GetEntry(){
        var userId=User.FindFirst("jti")?.Value;

        if(userId==null)
            return Unauthorized("No userId");
        
        var user=await Context.Users.FindAsync(int.Parse(userId));

        return Ok(user);
    }


}
