using System;
using Microsoft.AspNetCore.Identity;

public class Like
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; } // Reference to the user who liked the post
    public DateTime CreatedAt { get; set; }

    public Post Post { get; set; } // Navigation property to the liked post
}
