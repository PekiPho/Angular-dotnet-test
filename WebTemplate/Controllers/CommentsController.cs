using Microsoft.VisualBasic;

namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController:ControllerBase{

    public IspitContext Context {get;set;}

    public CommentController(IspitContext context){
        Context=context;
    }


    //need to make it so that 50 comments load and 50 agane
    [HttpGet("GetCommentsFromPost/{postId}")]
    public async Task<ActionResult> GetCommentsFromPost(Guid postId){

        var comments=await Context.Comments.Include(c=>c.Post)
                                        .Include(c=>c.Replies)
                                        .Include(c=>c.ReplyTo)
                                        .Include(c=>c.User)
                                        .Where(c=>c.Post!.Id==postId).ToListAsync();

        if(!comments.Any())
            return Ok(null);


        //wont work because of cycles
        //need to make a mapper
        return Ok(comments);
    }

    [HttpPost("CreateComment/{username}/{postId}")]
    public async Task<ActionResult> CreateComment(string username,Guid postId,[FromBody]Comment comment){
        var post=await Context.Posts.FirstOrDefaultAsync(c=>c.Id==postId);
        var user=await Context.Users.FirstOrDefaultAsync(c=>c.Username==username);

        if(post==null || user==null)
            return BadRequest("User or post invalid");

        comment.Post=post;
        comment.User=user;

        await Context.Comments.AddAsync(comment);
        await Context.SaveChangesAsync();

        //need to map this as well probably
        return Ok(comment);
    }

    [HttpPut("UpdateComment/{commentId}/{content}")]
    public async Task<ActionResult> UpdateComment(Guid commentId,string content){
        var comment=await Context.Comments
                                .Include(c=>c.Replies)
                                .Include(c=>c.ReplyTo)
                                .Include(c=>c.User)
                                .Include(c=>c.Post)
                                .Where(c=>c.Id==commentId).FirstOrDefaultAsync();

        if(comment==null)
            return BadRequest("Comment does not exist");

        comment.Content=content;

        Context.Comments.Update(comment);

        await Context.SaveChangesAsync();


        //also needs a mapper
        return Ok(comment);
    }

    [HttpDelete("DeleteComment/{commentId}")]
    public async Task<ActionResult> DeleteComment(Guid commentId){

        var comment =await Context.Comments.Where(c=>c.Id==commentId).FirstOrDefaultAsync();

        if(comment==null)
            return BadRequest("Comment does not exist");

        Context.Comments.Remove(comment);

        await Context.SaveChangesAsync();

        return Ok("Deleted");
    }

}