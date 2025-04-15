using AutoMapper;
using WebTemplate.Models;
using WebTemplate.Dtos;

public class MappingProfile:Profile{

    public MappingProfile(){
        CreateMap<Post,PostDto>()
            .ForMember(c=>c.CommentsId, a=>a.MapFrom(b=>b.Comments.Select(d=>d.Id).ToList()))
            .ForMember(a=>a.CommunityName,b=>b.MapFrom(c=>c.Community.Name))
            .ForMember(a=>a.Username,b=>b.MapFrom(c=>c.User.Username))
            .ForMember(a=>a.MediaIds,b=>b.MapFrom(c=>c.Media.Select(d=>d.Id).ToList()))
            .ForMember(a=>a.Vote,b=>b.MapFrom(c=>c.Upvotes-c.Downvotes));

        CreateMap<User,UserDto>()
            .ForMember(c=>c.CommentIds, a=>a.MapFrom(b=>b.Comments.Select(d=>d.Id).ToList()))
            .ForMember(a=>a.PostIds,b=>b.MapFrom(c=>c.Posts.Select(d=>d.Id).ToList()))
            .ForMember(a=>a.SubscribedTo,b=>b.MapFrom(c=>c.Subscribed.Select(d=>d.Id).ToList()))
            .ForMember(a=>a.Moderating,b=>b.MapFrom(c=>c.Moderator.Select(d=>d.Id).ToList()))
            .ForMember(a=>a.Email,b=>b.MapFrom(c=>c.Email))
            .ForMember(c=>c.DateOfAccountCreated,b=>b.MapFrom(a=>a.DateOfAccountCreated));

        CreateMap<Media,MediaDto>()
            .ForMember(a=>a.Id,b=>b.MapFrom(c=>c.Id))
            .ForMember(a=>a.Url,b=>b.MapFrom(c=>c.Url))
            .ForMember(a=>a.PostId,b=>b.MapFrom(c=>c.Post.Id));

        CreateMap<Comment,CommentDto>()
            .ForMember(a=>a.Id,b=>b.MapFrom(c=>c.Id))
            .ForMember(a=>a.Content,b=>b.MapFrom(c=>c.Content))
            .ForMember(a=>a.DateOfComment,b=>b.MapFrom(c=>c.DateOfComment))
            .ForMember(a=>a.IsDeleted,b=>b.MapFrom(c=>c.IsDeleted))
            .ForMember(a=>a.PostId,b=>b.MapFrom(c=>c.Post.Id))
            .ForMember(a=>a.Username,b=>b.MapFrom(c=>c.User.Username))
            .ForMember(a=>a.ReplyToId,b=>b.MapFrom(c=>c.ReplyTo.Id))
            .ForMember(a=>a.Replies,b=>b.MapFrom(c=>c.Replies))
            .ForMember(a=>a.Vote,b=>b.MapFrom(c=>c.Upvotes-c.DownVotes));
    }
}