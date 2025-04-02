using AutoMapper;
using WebTemplate.Dtos;

namespace WebTemplate.Controllers;


[ApiController]
[Route("[controller]")]
public class SubscribeController:ControllerBase{


    public IspitContext Context{get;set;}

    private readonly IMapper Mapper;

    public SubscribeController(IspitContext context,IMapper mapper){
        Context=context;
        Mapper=mapper;
    }

    [HttpGet("GetCommunitiesFromUser/{userID}")]
    public async Task<ActionResult> GetCommunitiesFromUser(int userID){
        
        var user=await Context.Users.AsNoTracking().Include(c=>c.Subscribed).Where(c=>c.Id==userID).FirstOrDefaultAsync();
        if(user==null)
            return BadRequest("No user");

        if( user.Subscribed ==null || !user.Subscribed.Any() )
            return BadRequest("User not subscribed to any community");


        return Ok(user.Subscribed);
    }


    [HttpPost("SubscribeUserToCommunity/{userID}/{communityID}")]
    public async Task<ActionResult> SubscribeUserToCommunity(int userID,int communityID){

        var user=await Context.Users.FindAsync(userID);
        var community=await Context.Communities.FindAsync(communityID);

        if(user!=null && community!=null){
            user.Subscribed ??=new List<Community>();
            
            user.Subscribed.Add(community);

            await Context.SaveChangesAsync();

            return Ok($"Subscribed user with ID: {userID} to the community of ID: {communityID}");
        }

        return BadRequest("User or community does not exist");
    }

    [HttpPost("AddUserToModerateCommunity/{userID}/{communityID}")]
    public async Task<ActionResult> AddUserToModerateCommunity(int userID,int communityID){

        var user=await Context.Users.FindAsync(userID);
        var community=await Context.Communities.FindAsync(communityID);

        if(user!=null && community != null)
        {
            user.Moderator ??=new List<Community>();

            user.Moderator.Add(community);

            await Context.SaveChangesAsync();

            return Ok($"Made user with ID: {userID} moderator of the community with the ID of: {communityID}");
        }

        return BadRequest("User or community does not exist");
    }

    [HttpDelete("UnsubscribeUserFromCommunity/{userID}/{communityID}")]
    public async Task<ActionResult> UnsubscribeUserFromCommunity(int userID,int communityID){

        var user=await Context.Users.Include(c=>c.Subscribed).FirstOrDefaultAsync(c=>c.Id==userID);
        var community=await Context.Communities.FindAsync(communityID);

        if(user==null || community==null)
            return BadRequest("User or community does not exist");

        if(user.Subscribed==null || !user.Subscribed.Contains(community))
            return BadRequest("User not subscribed to community");

        user.Subscribed!.Remove(community);

        await Context.SaveChangesAsync();

        return Ok($"Successfully removed User with ID: {userID} from the Community with ID: {communityID}");
    }


    [HttpDelete("RemoveModeratorFromCommunity/{userID}/{communityID}")]
    public async Task<ActionResult> RemoveModeratorFromCommunity(int userID,int communityID){

        var user= await Context.Users.Include(c=>c.Moderator).FirstOrDefaultAsync(c=>c.Id==userID);
        var community= await Context.Communities.FindAsync(communityID);

        if(user == null || community == null){
            return BadRequest("User or community does not exist");
        }

        if(user.Moderator==null || !user.Moderator.Contains(community))
            return BadRequest("User not moderator of community");

        user.Moderator.Remove(community);

        await Context.SaveChangesAsync();

        return Ok($"Removed User: {userID} from moderating community: {communityID}");
    }

    [HttpPost("AddUserToMod/{username}/{communityName}")]
    public async Task<ActionResult> AddUserToMod(string username,string communityName){

        var user=await Context.Users.Include(c=>c.Moderator).Where(c=>c.Username==username).FirstOrDefaultAsync();

        var community=await Context.Communities.Where(c=>c.Name==communityName).FirstOrDefaultAsync();

        if(user==null || community==null)
            return BadRequest("User or Community don't exist");

        user.Moderator ??=new List<Community>();

        user.Moderator.Add(community);

        await Context.SaveChangesAsync();

        return Ok($"Added moderator: {user.Username} to the community {community.Name}");
    }


    [HttpDelete("RemoveModFromCommunity/{username}/{communityName}")]
    public async Task<ActionResult> RemoveModFromCommunity(string username,string communityName){

        var user=await Context.Users.Include(c=>c.Moderator).Where(c=>c.Username== username).FirstOrDefaultAsync();

        var community=await Context.Communities.Where(c=>c.Name==communityName).FirstOrDefaultAsync();

        if(community==null || user == null){
            return BadRequest("User or community dont exist");
        }

         if(user.Moderator==null || !user.Moderator.Contains(community))
            return BadRequest("User not moderator of community");

        user.Moderator.Remove(community);

        await Context.SaveChangesAsync();

        return Ok("User successfully removed from moderating community");
    }

    [HttpGet("FindModeratorsFromCommunity/{communityName}")]
    public async Task<ActionResult> FindModeratorsFromCommunity(string communityName){

        var community=await Context.Communities.Include(c=>c.Moderators).Where(c=>c.Name==communityName).FirstOrDefaultAsync();

        if(community==null)
            return BadRequest("Community with given name does not exist");

        if(community.Moderators==null || !community.Moderators.Any())
            return Ok(null);


        var commDto= Mapper.Map<List<UserDto>>(community.Moderators);
        
        return Ok(commDto);
    }

    [HttpGet("IsUserModerating/{communityName}/{username}")]
    public async Task<ActionResult> IsUserModerating(string communityName,string username){

        var community=await Context.Communities.Include(c=>c.Moderators).Where(c=>c.Name==communityName).FirstOrDefaultAsync();

        if(community==null)
            return BadRequest("Community does not exist");

        bool check = community.Moderators?.Any(c=>c.Username==username) ?? false;

        return Ok(check);
    }
}