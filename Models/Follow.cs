using Microsoft.AspNetCore.Identity;

public class Follow
{
    public int Id { get; set; }
    public string FollowerId { get; set; } // Id of the follower
    public IdentityUser Follower { get; set; } // Reference to the follower
    public string FollowedUserId { get; set; } // Id of the user being followed
    public IdentityUser FollowedUser { get; set; } // Reference to the user being followed
}