using AutoMapper;
using WebTemplate.Dtos;

namespace WebTemplate.Controllers;


[ApiController]
[Route("[controller]")]
public class PostController:ControllerBase{

    public IspitContext Context{get;set;}

    private readonly IMapper Mapper;

    public PostController(IspitContext context,IMapper mapper){
        Context=context;
        Mapper=mapper;
    }

    [HttpGet("GetPostsFromCommunity/{communityName}")]
    public async Task<ActionResult> GetPostsFromCommunity(string communityName){
        var posts=await Context.Posts.Include(c=>c.Community)
                        .Include(c=>c.User)
                        .Include(c=>c.Comments)
                        .Include(c=>c.Media)
                        .Where(c=>c.Community!.Name==communityName  && c.DateOfPost>DateTime.Now.AddDays(-7))
                        .OrderBy(c=>c.Vote).ToListAsync();
        
        var postsDto=Mapper.Map<List<PostDto>>(posts);

        return Ok(postsDto);
    }

    [HttpPost("AddPost/{userID}/{communityID}")]
    public async Task<ActionResult> AddPost(int userID,int communityID,[FromBody]Post post){

        var user=await Context.Users.FindAsync(userID);
        var community=await Context.Communities.FindAsync(communityID);

        if(community==null || user == null)
            return BadRequest("User or community don't exist");

        post.Community=community;
        post.User=user;

        //post.Id=new Guid();

        await Context.Posts.AddAsync(post);
        await Context.SaveChangesAsync();

        return Ok($"Added post with ID: {post.Id}");
    }

    [HttpPost("AddPostByName/{userName}/{communityName}")]
    public async Task<ActionResult> AddPostByName(string userName,string communityName,[FromBody] Post post){
        var user=await Context.Users.Where(c=>c.Username == userName).FirstOrDefaultAsync();
        var community= await Context.Communities.Where(c=>c.Name==communityName).FirstOrDefaultAsync();

        if(user== null)
            return BadRequest("User does not exist");

        if(community==null)
            return BadRequest("Community does not exist");

        post.Community=community;
        post.User=user;

        if(post.Media !=null && post.Media.Any()){
            foreach(var media in post.Media){
                media.Post=post;    
            }
                
        }

        await Context.Posts.AddAsync(post);
        await Context.SaveChangesAsync();

        var postDto=Mapper.Map<PostDto>(post);
        Console.WriteLine(post.User.Username);
        Console.WriteLine(postDto.Username);
        return Ok(postDto);
        
    }

    [HttpPut("UpdateTitle/{communityID}/{postID}/{title}")]
    public async  Task<ActionResult> UpdateTitle(int communityID,Guid postID,string title){

        var post=await Context.Posts
        .Include(c=>c.Community)
        .Where(c=>c.Id==postID && c.Community!.Id==communityID).FirstOrDefaultAsync();

        if(post==null)
            return BadRequest("Post does not exist");
        
        post.Title=title;

        Context.Posts.Update(post);

        await Context.SaveChangesAsync();

        return Ok($"Title of the post with ID: {post.Id} updated");
    }

    [HttpPut("UpdateDescription/{communityID}/{postID}/{description}")]
    public async Task<ActionResult> UpdateDescription(int communityID,Guid postID,string description){

        var post =await Context.Posts
        .Include(c=>c.Community)
        .Where(c=>c.Community!.Id==communityID && c.Id== postID).FirstOrDefaultAsync();

        if(post==null){
            return BadRequest("Post does not exist");
        }

        post.Description=description;

        Context.Posts.Update(post);

        await Context.SaveChangesAsync();

        return Ok($"Description of the post with ID: {post.Id} updated");
    }

    [HttpDelete("DeletePost/{communityID}/{postID}")]
    public async Task<ActionResult> DeletePost(int communityID,Guid postID){

        var post=await Context.Posts.Include(c=>c.Community)
        .Where(c=>c.Id==postID && c.Community!.Id==communityID).FirstOrDefaultAsync();

        if(post==null)
            return BadRequest("Post does not exist");

        Context.Posts.Remove(post);

        await Context.SaveChangesAsync();

        return Ok($"Post  with ID: {postID} removed");
    }
}