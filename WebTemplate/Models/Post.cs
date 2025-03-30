using System.Text.Json.Serialization;

namespace WebTemplate.Models;

// public class Post{

//     [Key,Column(Order =1)]
//     public int Id { get; set; }

//     [Key,Column(Order =0)]
//     public required Community Community { get; set; }

//     [MaxLength(250)]
//     public string Title { get; set; }

//     [MaxLength(5000)]
//     public string? Description { get; set; }

//     public List<string>? Media { get; set; }

//     public List<Comment>? Comments { get; set; }

//     public int Vote { get; set; }
// }

public class Post
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    //public int CommunityId { get; set; }
    public Community? Community { get; set; }

    [MaxLength(250)]
    public required string Title { get; set; }

    [MaxLength(5000)]
    public string? Description { get; set; }

    public List<Media>? Media { get; set; }


    public List<Comment>? Comments { get; set; }


    public User? User { get; set; }

    //public int UserId { get; set; }

    public int Vote { get; set; }=0;

    public DateTime DateOfPost { get; set; }=DateTime.Now;

    public List<Vote> Votes{get;set;}= new();

    [NotMapped]
    public int Upvotes => Votes!.Count(c=>c.VoteValue);

    [NotMapped]
    public int Downvotes => Votes!.Count(c=>!c.VoteValue);

    [NotMapped]
    public int TotalVotes => Votes!.Count;

    [NotMapped]
    public double Ratio => TotalVotes == 0 ? 0 : (double)Upvotes/TotalVotes;
}