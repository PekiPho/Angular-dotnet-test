using System.Transactions;

namespace WebTemplate.Models;

public class User{

    [Key]
    public int Id { get; set; }

    [MinLength(4)]
    public required string Username { get; set; }

    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$",ErrorMessage ="Password must contain at least 8 characters and 1 uppercase letter and 1 digit")]
    public required string Password { get; set; }

    [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",ErrorMessage ="Enter valid email")]
    public required string Email { get; set; }
}