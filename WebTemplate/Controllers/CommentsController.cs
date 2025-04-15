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
        //DONE need to make a mapper
        var commentsDto=Mapper.Map<List<CommentDto>>(comments);
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
        comment.DateOfComment=DateTime.Now;

        var vote=new CommentVote{
            VoteValue=true,
            User=user,
            Comment=comment
        };

        if(comment.Votes==null)
            comment.Votes= new List<CommentVote>();

        comment.Votes.Add(vote);

        await Context.CommentVotes.AddAsync(vote);

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

        var comment =await Context.Comments.Where(c=>c.Id==commentId).FirstOrDefaultAsync();

        if(comment==null)
            return BadRequest("Comment does not exist");

        comment.IsDeleted=true;
        comment.Content="[deleted]";
        comment.User=null;

        Context.Comments.Update(comment);

        await Context.SaveChangesAsync();

        return Ok(true);
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

}