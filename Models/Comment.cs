using Microsoft.AspNetCore.Identity;

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; } // Reference to the user who made the comment
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Post Post { get; set; } // Navigation property to the liked post

}