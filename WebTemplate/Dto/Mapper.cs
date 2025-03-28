using AutoMapper;
using WebTemplate.Models;
using WebTemplate.Dtos;

public class MappingProfile:Profile{

    public MappingProfile(){
        CreateMap<Post,PostDto>()
            .ForMember(c=>c.CommentsId, a=>a.MapFrom(b=>b.Comments.Select(d=>d.Id).ToList()))
            .ForMember(a=>a.CommunityName,b=>b.MapFrom(c=>c.Community.Name))
            .ForMember(a=>a.Username,b=>b.MapFrom(c=>c.User.Username))
            .ForMember(a=>a.MediaIds,b=>b.MapFrom(c=>c.Media.Select(d=>d.Id).ToList()));
    }
}