using Microsoft.AspNetCore.Identity;

public class Post
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string ImageUrl { get; set; } // URL to image or video
    public string UserId { get; set; }
    public IdentityUser User { get; set; } // Reference to the user who created the post
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Comment> Comments { get; set; } // Collection of comments
    public ICollection<Like> Likes { get; set; } // Collection of likes
    public ICollection<Follow> Followers { get; set; } // Collection of followers
}
