using System.Text.RegularExpressions;
using AutoMapper;
using WebTemplate.Dtos;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.ObjectPool;

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

    [HttpPost("AddPostWithMedia/{username}/{communityName}")]
    public async Task<ActionResult> AddPostWithMedia(string username,string communityName,[FromForm]string postJson,[FromForm] IFormFile file){
        var user=await Context.Users.Where(c=>c.Username==username).FirstOrDefaultAsync();
        var community=await Context.Communities.Where(c=>c.Name==communityName).FirstOrDefaultAsync();
        if(user==null || community ==null)
            return BadRequest("User or community not found");
        
        var post=JsonConvert.DeserializeObject<Post>(postJson);
        if(post==null)
            return BadRequest("Invalid post data");

        post.User=user;
        post.Community=community;

        //Console.WriteLine(file);

        if(file!=null){

            var extensionsAllowed=new List<string> { ".jpg",".jpeg",".png",".mp4",".mov",".avi"};
            var fileExt=Path.GetExtension(file.FileName).ToLowerInvariant();

            if(!extensionsAllowed.Contains(fileExt))
                return BadRequest("Extension is not yet supported");
            
            var media=new Media{
                Post=post,
                Url="",
            };

            post.Media=post.Media ?? new List<Media>();
            post.Media.Add(media);

            await Context.Media.AddAsync(media);
            await Context.Posts.AddAsync(post);
            await Context.SaveChangesAsync();


            string path= Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","Media");

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName=media.Id+fileExt;
            var filePath=Path.Combine(path,fileName);

            using (var stream = new FileStream(filePath,FileMode.Create)){
                await file.CopyToAsync(stream);
            }

            media.Url=Path.Combine("Media",fileName);

            Context.Media.Update(media);

            await Context.SaveChangesAsync();
        }

        var postDto=Mapper.Map<PostDto>(post);
        return Ok(postDto);
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

        if(IsUrl(post.Description)){
            Console.WriteLine("isUrl!!");
            var img=await GetOgImage(post?.Description);
            //Console.WriteLine(img);
            if(!string.IsNullOrEmpty(img)){
                var media = new Media{
                    Url=img,
                    Post=post
                };

                await Context.Media.AddAsync(media);
                post.Media= post.Media ?? new List<Media>();
                post.Media.Add(media);


            }
        }
        else{
            //Console.WriteLine("No url :(");
        }

        // if(post.Media !=null && post.Media.Any()){
        //     foreach(var media in post.Media){
        //         media.Post=post;    
        //     }
                
        // }

        await Context.Posts.AddAsync(post);
        await Context.SaveChangesAsync();

        var postDto=Mapper.Map<PostDto>(post);
        //Console.WriteLine(post.User.Username);
        //Console.WriteLine(postDto.Username);
        return Ok(postDto);
        
    }


    [HttpPut("UpdateTitle/{postID}/{title}")]
    public async  Task<ActionResult> UpdateTitle(Guid postID,string title){

        var post=await Context.Posts.Where(c=>c.Id==postID).FirstOrDefaultAsync();

        if(post==null)
            return BadRequest("Post does not exist");
        
        post.Title=title;

        Context.Posts.Update(post);

        await Context.SaveChangesAsync();

        return Ok($"Title of the post with ID: {post.Id} updated");
    }

    [HttpPut("UpdateDescription/{postID}/{description}")]
    public async Task<ActionResult> UpdateDescription(Guid postID,string description){

        var post =await Context.Posts
        .Where(c=> c.Id== postID).FirstOrDefaultAsync();

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

    [HttpDelete("DeletePostByName/{postId}")]
    public async Task<ActionResult> DeletePostByName(Guid postId){

        var post=await Context.Posts.Include(c=>c.Media)
                                    .Include(c=>c.Votes)
                                    .Include(c=>c.Comments)
                                        .ThenInclude(c=>c.Votes)
                                    //.Include(c=>c.)
                            .Where(c=>c.Id==postId).FirstOrDefaultAsync();

        if(post==null)
            return BadRequest("Post does not exist");

        if(post.Media !=null && post.Media.Any()){
            foreach(var media in post.Media){
               // var filePath=Path.Combine("wwwroot/Media",media.Id.ToString());
                var searchFile=Directory.GetFiles("wwwroot/Media", $"{media.Id}.*").FirstOrDefault();

                if(System.IO.File.Exists(searchFile)){
                    System.IO.File.Delete(searchFile);
                }

                Context.Media.Remove(media);
            }

            await Context.SaveChangesAsync();
        }

        if(post.Votes!=null && post.Votes.Any()){
            Context.Votes.RemoveRange(post.Votes);
            await Context.SaveChangesAsync();
        }

        if(post.Comments!=null && post.Comments.Any()){
            var allVotes=post.Comments.Where(c=>c.Votes !=null && c.Votes.Any())
                                    .SelectMany(c=>c.Votes)
                                    .ToList();
            if(allVotes.Any()){
                Context.CommentVotes.RemoveRange(allVotes);
                await Context.SaveChangesAsync();
            }

            Context.Comments.RemoveRange(post.Comments);
            await Context.SaveChangesAsync();
        }



        Context.Posts.Remove(post);

        await Context.SaveChangesAsync();

        return Ok("Deleted");
        
    }

    [HttpGet("GetHotPosts/{username}")]
    public async Task<ActionResult> GetHotPosts(string username)
    {


        var user = await Context.Users.Include(c => c.Subscribed).FirstOrDefaultAsync(c => c.Username == username);

        if (user == null)
            return BadRequest("No user");

        if (user.Subscribed != null && user.Subscribed.Any())
        {

            var comm = user.Subscribed.Select(c => c.Name).ToList();

            //change the value on total days based on however popular the thing is (should be like 3-4)
            var posts = await Context.Posts.Include(c => c.Community)
                                            .Include(c => c.Media)
                                            .Include(c => c.User)
                                            .Include(c => c.Votes)
                                            .Where(c => EF.Functions.DateDiffDay(c.DateOfPost,DateTime.UtcNow) < 120 && comm.Contains(c.Community!.Name))
                                            .ToListAsync();

            var sortEm = posts.Select(c => new
            {
                Post = c,
                HotScore = (c.Vote == 0 ? 1 : c.Vote) / Math.Pow((DateTime.UtcNow - c.DateOfPost).TotalHours +1, 1.25)
            })
            .OrderByDescending(a => a.HotScore)
            .Select(a => a.Post)
            .ToList();

            var postsDto = Mapper.Map<List<PostDto>>(sortEm);

            return Ok(postsDto);
        }
        else{
        

            //adjust the day based on popularity (only for optimization, should be like 1 here)
             var posts = await Context.Posts.Include(c => c.Community)
                                            .Include(c => c.Media)
                                            .Include(c => c.User)
                                            .Include(c => c.Votes)
                                            .Where(c => EF.Functions.DateDiffDay(c.DateOfPost,DateTime.UtcNow) < 120)
                                            .ToListAsync();

            var sortEm = posts.Select(c => new
            {
                Post = c,
                HotScore = (c.Vote == 0 ? 1 : c.Vote) / Math.Pow((DateTime.UtcNow - c.DateOfPost).TotalHours +1, 1.25)
            })
            .OrderByDescending(a => a.HotScore)
            .Select(a => a.Post)
            .ToList();

            var postsDto = Mapper.Map<List<PostDto>>(sortEm);

            return Ok(postsDto);
        }

        
    }

    [HttpGet("GetPostsBySort/{communityName}/{sort}/{page}/{limit}/{time}")]
    public async Task<ActionResult> GetPostsBySort(string communityName,string sort,int page,int limit,string? time){

        var posts=Context.Posts.Include(c=>c.Community)
                                    .Include(c=>c.Media)
                                    .Include(c=>c.User)
                                    .Include(c=>c.Votes)
                                    .Where(c=>c.Community!.Name==communityName);

        if(sort=="Top"){
            var sortTime=DateTime.MinValue;
            switch(time){
                case "Today":
                    sortTime=DateTime.UtcNow.Date;
                    break;
                case "This week":
                    sortTime=DateTime.UtcNow.AddDays(-7);
                    break;
                case "This month":
                    sortTime=DateTime.UtcNow.AddMonths(-1);
                    break;
                case "This year":
                    sortTime=DateTime.UtcNow.AddYears(-1);
                    break;
                case "All time":
                    sortTime=DateTime.MinValue;
                    break;
                default:
                    return BadRequest("Wrong time parameter");
            }

            posts=posts.Where(c=>c.DateOfPost >=sortTime);
        }
        
        switch(sort){
            case "Top":
                posts=posts.OrderByDescending(c=>c.Vote);
                break;
            case "New":
                posts=posts.OrderByDescending(c=>c.DateOfPost);
                break;
            case "Hot":
                var res=await posts.ToListAsync();
                var hot = res.Select(c => new {
                    Post = c,
                    HotScore = c.Vote / Math.Pow((DateTime.UtcNow - c.DateOfPost).TotalHours,1.25)
                })
                .OrderByDescending(a => a.HotScore)
                .Select(a => a.Post)
                .Skip(limit*(page-1)).Take(limit).ToList();

                var postsDto=Mapper.Map<List<PostDto>>(hot);

                return Ok(postsDto);

            default: return BadRequest("No such sort");
        }

        var skip=limit*(page-1);
        var postss= await posts.Skip(skip).Take(limit).ToListAsync();

        var postDto = Mapper.Map<List<PostDto>>(postss);
        return Ok(postDto);

    }

    private bool IsUrl(string? description){
        if(string.IsNullOrEmpty(description))
            return false;

        return Uri.IsWellFormedUriString(description,UriKind.Absolute);
    }

    private async Task<string?> GetOgImage(string? url){
        using var httpClient = new HttpClient();
        var keyy= System.IO.File.ReadAllLines("api.txt");
        var encoded=Uri.EscapeDataString(url!);
        //Console.WriteLine("keyyyyy");
        //Console.WriteLine(keyy.Length);
        var key=keyy[0];
        //Console.WriteLine(url);
        var ogUrl=$"https://opengraph.io/api/1.1/site/{encoded}?app_id={key}";

        var resp = await httpClient.GetStringAsync(ogUrl);
        var data = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(resp);

        //Console.WriteLine(resp);

        var image = data.GetProperty("openGraph").GetProperty("image").GetProperty("url").GetString();

        //Console.WriteLine(image);

        return image;
        
    }


    //need to add to laod 50 by 50 but its so late right now so ill do it tmrw
    [HttpGet("GetPostsByUser/{username}")]
    public async Task<ActionResult> GetPostsByUser(string username){
        var posts=await Context.Posts.Include(c=>c.User)
                                    .Include(c=>c.Media)
                                    .Include(c=>c.Comments)
                                    .Include(c=>c.Community)
                                    .Include(c=>c.Votes)
                                    .Where(c=>c.User.Username==username)
                                    .ToListAsync();

        var postsDto=Mapper.Map<List<PostDto>>(posts);

        return Ok(postsDto);
    }

    [HttpGet("GetXVotedPostsByUser/{username}/{vote}")]
    public async Task<ActionResult> GetXVotedPostByUser(string username,bool vote){

        var v=-1;
        if(vote)
            v=1;
        else v=0;

        var posts=await Context.Posts.Include(c=>c.Media)
                                        .Include(c=>c.User)
                                        .Include(c=>c.Community)
                                        .Include(c=>c.Comments)
                                        .Include(c=>c.Votes)
                                        .Where(c=>c.Votes.Any(a=>a.User.Username==username && a.VoteValue==vote))
                                        .ToListAsync();

        // i need to load 50 by 50 as well here
        
        var postsDto=Mapper.Map<List<PostDto>>(posts);

        return Ok(postsDto);
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

    [HttpGet("GetPostByID/{postId}")]
    public async Task<ActionResult> GetPostByID(Guid postId){
        var post=await Context.Posts.Where(c=>c.Id== postId).Include(c=>c.Community)
                                    .Include(c=>c.User)
                                    .Include(c=>c.Media)
                                    .Include(c=>c.Comments)
                                    .FirstOrDefaultAsync();

        if(post==null)
            return BadRequest("Post does not exist");

        var postDto= Mapper.Map<PostDto>(post);

        return Ok(postDto);
    }
}