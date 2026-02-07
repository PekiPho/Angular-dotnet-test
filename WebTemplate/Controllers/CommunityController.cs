namespace WebTemplate.Controllers;


[ApiController]
[Route("[controller]")]
public class CommunityController:ControllerBase{

    public IspitContext Context {get;set;}

    public CommunityController(IspitContext context){
        Context=context;
    }

    [HttpPost("CreateCommunity")]
    public async Task<ActionResult> CreateCommunity([FromBody] Community community){

        var exists = await Context.Communities.AnyAsync(c=>c.Name == community.Name);

        if(exists)
            return BadRequest("Community with given name already exists");

        if(string.IsNullOrEmpty(community.Name))
            return BadRequest("Name cant be empty");

        if(community.Posts!=null && community.Posts.Any())
            community.Posts.Clear();

        await Context.Communities.AddAsync(community);
        await Context.SaveChangesAsync();

        return Ok(community);
    }

    

    [HttpPut("UpdateDescription/{id}/{description}")]
    public async Task<ActionResult> UpdateDescription(int id,string description){

        var community=await Context.Communities.Where(c=>c.Id==id)
            .FirstOrDefaultAsync();

        if(community==null)
            return BadRequest("Community with given ID does not exist");
        
        community.Description=description;

        Context.Communities.Update(community);

        await Context.SaveChangesAsync();

        return Ok("Description successfully changed");

    }

    [HttpPut("UpdateCommunityDescription/{name}/{description}")]
    public async Task<ActionResult> UpdateCommunityDescription(string name,string description){

        var community=await Context.Communities.Where(c=>c.Name==name).FirstOrDefaultAsync();

        if(community == null){
            return BadRequest("Community with given name does not exist");
        }

        community.Description=description;

        Context.Communities.Update(community);

        await Context.SaveChangesAsync();

        return Ok($"Updated description successfully. Community name: {community.Name}");
    }

    [HttpPut("UpdateCommInfo/{name}/{commInfo}")]
    public async Task<ActionResult> UpdateCommInfo(string name,string commInfo){

        var community=await Context.Communities.Where(c=>c.Name==name).FirstOrDefaultAsync();

        if(community==null){
            return BadRequest("Community with given name does not exist");
        }

        community.CommInfo=commInfo;

        Context.Communities.Update(community);

        await Context.SaveChangesAsync();

        return Ok($"Community Info updated. Community name : {community.Name}");
    }


    [HttpDelete("DeleteCommunity/{id}")]
    public async Task<ActionResult> DeleteCommunity(int id){

        var community=await Context.Communities.FindAsync(id);

        if(community==null)
            return BadRequest("Community does not exist");

        Context.Communities.Remove(community);

        await Context.SaveChangesAsync();

        return Ok($"Community with ID: {id} successfully deleted");
    }

    [HttpDelete("DeleteCommunityByName/{name}")]
    public async Task<ActionResult> DeleteCommunityByName(string name){

        var community=await Context.Communities.Where(c=>c.Name==name).FirstOrDefaultAsync();

        if(community==null)
            return BadRequest("Community does not exist");

        Context.Communities.Remove(community);

        await Context.SaveChangesAsync();

        return Ok($"Community with the name of: {name} is successfully deleted");
    }


    [HttpGet("GetCommunity/{id}")]
    public async Task<ActionResult> GetCommunity(int id){

        var community = await Context.Communities.FindAsync(id);

        if(community==null)
            return BadRequest($"Community with id of: {id} does not exist");

        return Ok(community);
    }

    [HttpGet("GetCommunityByName/{name}")]
    public async Task<ActionResult> GetCommunityByName(string name){
        var community = await Context.Communities.Where(c=> c.Name == name).FirstOrDefaultAsync();

        if(community==null)
            return BadRequest("Community with given name does not exist");
        
        return Ok(community);
    }

    [HttpGet("GetSubscribers/{name}")]
    public async Task<ActionResult> GetSubscribers(string name){
        var community=await Context.Communities.Where(c=>c.Name==name).Include(c=>c.Subscribers).FirstOrDefaultAsync();

        if(community==null && community?.Subscribers == null)
            return BadRequest("Community does not exist or has no subscribers");
        
        var subs=community!.Subscribers!.Count;

        return Ok(subs);

    }

}