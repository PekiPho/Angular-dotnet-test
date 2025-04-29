using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController:ControllerBase{

    public IspitContext Context {get;set;}
    public readonly IMapper Mapper;

    SearchController(IspitContext context,IMapper mapper){
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




}