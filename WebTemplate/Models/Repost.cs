namespace WebTemplate.Models;




public class Repost
{
    [Key]
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    //public Guid PostId { get; set; }
    public required Post Post { get; set; }

    //public int CommunityId { get; set; }
    public required Community Community { get; set; }

    public List<Comment>? Comments { get; set; }

    public int Vote { get; set; }

    public DateTime DateOfCrossPost { get; set; } = DateTime.Now;
}