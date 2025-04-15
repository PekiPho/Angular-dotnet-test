namespace WebTemplate.Models;


public class CommentVote{


    public Guid Id { get; set; }=Guid.NewGuid();

    public Comment? Comment { get; set; }

    public User? User { get; set; }

    public bool VoteValue { get; set; }
}