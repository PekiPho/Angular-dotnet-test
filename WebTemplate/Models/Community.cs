using System.Text.Json.Serialization;

namespace WebTemplate.Models;


public class Community{

    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(20000)]
    public string? Description { get; set; }

    public DateTime CreationDate { get; set; }=DateTime.Now;

    public List<User>? Moderators { get; set; }


    public List<User>? Subscribers { get; set; }

    public List<Post>? Posts { get; set; }


    public string? CommInfo { get; set; }
}