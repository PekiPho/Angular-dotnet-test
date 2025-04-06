namespace WebTemplate.Dtos;

public class PostDto{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? CommunityName { get; set; }  

    public List<Guid>? CommentsId { get; set; }

    public string? Username { get; set; }

    public List<Guid>? MediaIds { get; set; }

    public int Vote { get; set; }

    public DateTime DateOfPost { get; set; }
}


public class UserDto{


    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public List<string>? SubscribedTo { get; set; }

    public List<string>? Moderating { get; set; }

    public List<Guid>? PostIds { get; set; }

    public List<Guid>? CommentIds { get; set; }

    public DateTime DateOfAccountCreated { get; set; }
}

public class MediaDto{
    public Guid Id { get; set; }

    public string? Url { get; set; }

    public Guid PostId { get; set; }
}