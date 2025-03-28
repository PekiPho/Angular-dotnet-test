namespace WebTemplate.Models;

public class Media
{
    [Key]
    public Guid Id { get; set; }

    public required string Url { get; set; }
    //public Guid PostId { get; set; }
    public Post? Post { get; set; }
}