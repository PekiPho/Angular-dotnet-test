using System;

namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class VoteController:ControllerBase{

    public IspitContext Context{get;set;}
    public VoteController(IspitContext context){
        Context=context;
    }


    [HttpPost("AddVote/{postIdd}/{username}/{vote}")]
    public async Task<ActionResult> AddVote(string postIdd,string username,bool vote)
    {
        if(!Guid.TryParse(postIdd,out Guid postId)){
            return BadRequest("Invalid Post ID format");
        }

        var post = await Context.Posts
                .Include(c => c.Votes)
                    .ThenInclude(v => v.User)
                .Where(c => c.Id == postId)
                .FirstOrDefaultAsync();
        var user=await Context.Users.Where(c=>c.Username==username).FirstOrDefaultAsync();

        if(post==null || user == null)
            return BadRequest("Post or user does not exist");

        if(post.Votes==null)
            return BadRequest("Post unavailable");

        var currentVote=await Context.Votes
                .Include(c=>c.Post)
                .Include(c=>c.User)
                .Where(c=>c.Post!.Id== postId && c.User!.Id == user.Id)
                .FirstOrDefaultAsync();

        if(currentVote!=null){
            if(currentVote.VoteValue==vote){
                Context.Votes.Remove(currentVote);
                await Context.SaveChangesAsync();
            }
            else {
                currentVote.VoteValue=vote;
                Context.Votes.Update(currentVote);
            }
        }
        else{
            await Context.Votes.AddAsync(new Vote{
                Post=post,
                User=user,
                VoteValue=vote
            });
        }

        post.Vote=post.Upvotes-post.Downvotes;

        Context.Posts.Update(post);

        await Context.SaveChangesAsync();

        return Ok(new{
            votes=post.Vote,
            ratio=post.Ratio
        });
    }

    [HttpGet("GetCurrentVote/{postId}/{username}")]
    public async Task<ActionResult> GetCurrentVote(Guid postId,string username){

        var vote=await Context.Votes.Where(c=>c.Post!.Id==postId && c.User!.Username==username).FirstOrDefaultAsync();
       
        if(vote==null){
            return Ok(null); // user hasnt voted
        }

        if(vote.VoteValue)
            return Ok(true);
        else return Ok(false);
    }

    [HttpGet("GetAllVotesByUser/{username}")]
    public async Task<ActionResult> GetAllVotesByUser(string username){

        var votes=await Context.Votes.Include(c=>c.User)
                                        .Include(c=>c.Post)
                                        .Where(c=>c.User!.Username == username).ToDictionaryAsync(
            c=>c.Post!.Id,
            c=>c.VoteValue
        );

        return Ok(votes);
    }

    [HttpGet("GetVotesByUser/{username}/{direction}")]
    public async Task<ActionResult> GetVotesByUser(string username,bool direction){

        var votes=await Context.Votes.Include(c=>c.User)
                                        .Include(c=>c.Post)
                                        .Where(c=>c.User!.Username==username && c.VoteValue==direction).ToDictionaryAsync(
            c=>c.Post!.Id,
            c=>c.VoteValue
        );

        return Ok(votes);
    }
    
}