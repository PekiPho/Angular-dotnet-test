using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using WebTemplate.Dtos;

namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController:ControllerBase{

    public IspitContext Context {get;set;}
    public readonly IMapper Mapper;

    public SearchController(IspitContext context,IMapper mapper){
        Context=context;
        Mapper=mapper;
    }

    [HttpGet("OnTypeCommunities/{query}")]
    public async Task<ActionResult> OnTypeCommunities(string query){

        // Found ot SQL Server is case insensitive so thats kinda cool
        //var qSmall=query.ToLower();

        var communities=await Context.Communities.Where(c=>c.Name.Contains(query))
                                    .Take(5)
                                    .ToListAsync();

        //var commDtos=Mapper.Map<List<CommunityDto>>

        return Ok(communities);

    }


    [HttpGet("GetCommunitiesAndPosts/{query}/{page?}")]
    public async Task<ActionResult> GetCommunitiesAndPosts(string query,int page =1, int pageSize=50){

        var communities = await Context.Communities
        .Where(c => c.Name.Contains(query))
        .Select(c => new {
            c.Id,
            c.Name,
            c.Description,
        })
        .Take(20)
        .ToListAsync();

        //implement search on scroll
        
        var posts=await Context.Posts
                            .Include(c => c.Community)
                            .Include(c=>c.User)
                            .Where(c=>c.Title.Contains(query)).ToListAsync();
        var hot= posts.Select(c => new
        {
            Post = c,
            Hot = Math.Log(Math.Max(1, Math.Abs(c.Vote))) - (DateTime.UtcNow - c.DateOfPost).TotalSeconds / 45000
        })
        .OrderByDescending(c => c.Hot)
        .Select(c => c.Post)
        .Skip((page-1)*pageSize)
        .Take(pageSize)
        .ToList();

        var postsDto=Mapper.Map<List<PostDto>>(hot ?? new List<Post>());

        return Ok(new{
            communities,
            posts=postsDto
        });
    }




}