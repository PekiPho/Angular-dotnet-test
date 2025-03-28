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