namespace WebTemplate.Models;


public class Vote{

    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Post? Post {get;set;}

    public User? User { get; set; }

    public bool VoteValue { get; set; }
}