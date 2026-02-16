using AutoMapper;
using Microsoft.VisualBasic;
using WebTemplate.Dtos;

namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController:ControllerBase{

    public IspitContext Context {get;set;}

    private readonly IMapper Mapper;

    public CommentController(IspitContext context,IMapper mapper){
        Context=context;
        Mapper=mapper;
    }

    [HttpPost("CreateComment/{username}/{postId}/{replyToId}")]
    public async Task<ActionResult> CreateComment(string username,Guid postId,string replyToId,[FromBody]Comment comment){
        var post=await Context.Posts.FirstOrDefaultAsync(c=>c.Id==postId);
        var user=await Context.Users.FirstOrDefaultAsync(c=>c.Username==username);

        if(post==null || user==null)
            return BadRequest("User or post invalid");


        if(replyToId!= "null"){
            Guid replyGuid;
            Guid.TryParse(replyToId,out replyGuid);
            var parent=await Context.Comments.Where(c=>c.Id==replyGuid).FirstOrDefaultAsync();

            if(parent==null){
                return BadRequest("Parent comment does not exist");
            }

            if(parent.Replies==null){
                parent.Replies=new List<Comment>();
            }

            parent.Replies.Add(comment);
            comment.ReplyTo=parent;
        }
        

        comment.Post=post;
        comment.User=user;
        comment.DateOfComment=DateTime.Now;

        comment.Votes = new List<CommentVote>
        {
            new CommentVote { VoteValue = true, User = user, Comment = comment }
        };

        await Context.Comments.AddAsync(comment);
        await Context.SaveChangesAsync();

        //DONE need to map this as well probably
        var commentDto= Mapper.Map<CommentDto>(comment);
        return Ok(commentDto);
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


        //DONE also needs a mapper
        var commentDto= Mapper.Map<CommentDto>(comment);
        return Ok(commentDto);
    }

    [HttpDelete("DeleteComment/{commentId}")]
    public async Task<ActionResult> DeleteComment(Guid commentId){

        var comment =await Context.Comments.Include(c=>c.User).Where(c=>c.Id==commentId).FirstOrDefaultAsync();

        if(comment==null)
            return BadRequest("Comment does not exist");

        comment.IsDeleted=true;
        comment.Content="[deleted]";
        comment.User=null;

        Context.Comments.Update(comment);

        await Context.SaveChangesAsync();

        return Ok(true);
    }

    [HttpDelete("ActualDelete/{commentId}")]
    public async Task<ActionResult> ActualDelete(Guid commentId){

        var comment = await Context.Comments.Include(c=>c.Replies).Where(c=>c.Id==commentId).FirstOrDefaultAsync();

        if(comment==null)
            return BadRequest("Comment does not exist");

        await DeleteChild(comment);

        Context.Comments.Remove(comment);

        await Context.SaveChangesAsync();

        return Ok("Deleted comment with its children");
    }

    private async Task DeleteChild(Comment parent){
        var children=await Context.Comments
                                    .Include(c=>c.Replies)
                                    .Include(c=>c.ReplyTo).Where(c=>c.ReplyTo.Id==parent.Id).ToListAsync();

        var votes=await Context.CommentVotes.Include(c=>c.Comment).Where(c=>c.Comment.Id==parent.Id).ToListAsync();

        Context.CommentVotes.RemoveRange(votes);

        foreach(var child in children){

            await DeleteChild(child);

            Context.Comments.Remove(child);

            await Context.SaveChangesAsync();
        }
        
    }

    [HttpGet("GetCommentsFromUser/{username}")]
    public async Task<ActionResult> GetCommentsFromUser(string username){

        var comments=await Context.Comments.Include(c=>c.User)
                                        .Include(c=>c.Post)
                                        .Where(c=>c.User.Username==username)
                                        .ToListAsync();

        //mapper and to make it so that i load 50 by 50 comments
        //DONE i also need to add a bool to the comments database to check if the comment is a reply
        var commentsDto=Mapper.Map<List<CommentDto>>(comments);
        return Ok(commentsDto);
    }

    [HttpGet("GetCommentCount/{postId}")]
    public async Task<ActionResult> GetCommentCount(Guid postId){
        var comments=await Context.Comments.Where(c=>c.Post.Id==postId).CountAsync();

        return Ok(comments);
    }

    //need to make it so that 50 comments load and 50 agane
    [HttpGet("GetCommentsFromPost/{postId}")]
    public async Task<ActionResult> GetCommentsFromPost(Guid postId){

        var comments=await Context.Comments.Include(c=>c.Post)
                                        .Include(c=>c.Replies)
                                        .Include(c=>c.ReplyTo)
                                        .Include(c=>c.User)
                                        .Include(c=>c.Votes)
                                        .Where(c=>c.Post!.Id==postId).ToListAsync();

        // if(!comments.Any())
        //     return Ok(null);


        //wont work because of cycles
        //DONE need to make a mapper
        var commentsDto=Mapper.Map<List<CommentDto>>(comments);
        return Ok(commentsDto);
    }

    [HttpGet("GetCommentsOnProfile/{username}")]
    public async Task<ActionResult> GetCommentsOnProfile(string username){

        var comments=await Context.Comments
                                    .Include(c=>c.User)
                                    .Include(c=>c.Votes)
                                    .Include(c=>c.Post)
                                    .Include(c=>c.Replies)
                                    .Include(c=>c.ReplyTo)
                                    .Where(c=>c.User.Username==username).ToListAsync();

        var commentsDto=Mapper.Map<List<CommentDto>>(comments);

        return Ok(commentsDto);
    }

}