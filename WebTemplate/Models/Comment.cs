namespace WebTemplate.Models;

public class Comment
{
    [Key]
    public Guid Id { get; set; }

    // Use just PostId for foreign key
    //[Required]
    //public Guid PostId { get; set; }

    // Foreign Key linking to Post
    public Post? Post { get; set; }

    [MaxLength(2000)]
    public required string Content { get; set; }

    //public int UserId { get; set; }

    public User? User { get; set; }

    //public int? ReplyToId { get; set; }

    [ForeignKey("ReplyToId")]
    public Comment? ReplyTo { get; set; }

    public List<Comment>? Replies { get; set; } = new(); // Collection of replies

    //public int Vote { get; set; }

    public DateTime DateOfComment { get; set; } = DateTime.Now;


    public bool IsDeleted { get; set; }=false;

    public List<CommentVote>? Votes { get; set; }=new();

    [NotMapped]
    public int Upvotes => Votes.Count(c=>c.VoteValue);

    [NotMapped]
    public int DownVotes => Votes.Count(c=>!c.VoteValue);

    [NotMapped]
    public int TotalVotes => Votes.Count;

    [NotMapped]
    public double Ratio => TotalVotes == 0 ? 0 : (double)Upvotes/DownVotes;
}