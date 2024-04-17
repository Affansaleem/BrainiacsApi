using System.Security.Claims;
using BrainiacsApi.Data;
using BrainiacsApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[Route("api/feed")]
[ApiController]
public class FeedController : ControllerBase
{
    private readonly BrainiacsDbContext _context;

    public FeedController(BrainiacsDbContext context)
    {
        _context = context;
    }

    // GET: api/feed
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetFeed()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Followers)
            .Where(p => p.UserId == userId || p.Followers.Any(f => f.FollowerId == userId))
            .ToListAsync();
        return posts;
    }

    // POST: api/feed
[HttpPost]
public async Task<ActionResult<Post>> CreatePost(PostDto postDto)
{
    // Get the current user's ID
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    // Create a new Post entity using the data from the PostDto
    var post = new Post
    {
        Content = postDto.Content,
        ImageUrl = postDto.ImageUrl,
        UserId = userId,
        Description = string.Empty, // You can set a default description if needed
        CreatedAt = DateTime.UtcNow,
        Comments = new List<Comment>(), // Initialize the comments collection with an empty list
        Likes = new List<Like>(), // Initialize the likes collection with an empty list
        Followers = new List<Follow>() // Initialize the followers collection with an empty list
    };

    // Add the post to the context and save changes
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();

    // Return a response indicating successful creation of the post
    return CreatedAtAction(nameof(GetFeed), new { id = post.Id }, post);
}

//for comment posting
[HttpPost("{postId}/comment")]
public async Task<ActionResult<Comment>> CommentOnPost(int postId, CommentDto commentDto)
{
    try
    {
        if (commentDto == null)
        {
            return BadRequest("CommentDto is null.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(); // Or handle unauthorized request as per your application's logic
        }

        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
        {
            return NotFound("Post not found.");
        }

        // Ensure that the Comments collection is initialized
        if (post.Comments == null)
        {
            post.Comments = new List<Comment>();
        }

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Content = commentDto.Content,
            CreatedAt = DateTime.UtcNow
        };

        post.Comments.Add(comment);

        // Add the comment to the context and save changes
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Return a response indicating successful creation of the comment
        return CreatedAtAction(nameof(GetFeed), new { id = postId }, comment);
    }
    catch (Exception ex)
    {
        // Log the exception or handle it appropriately
        // For now, return a generic error message
        return StatusCode(500, $"An error occurred while processing your request: {ex}");
    }
}



// POST: api/feed/{postId}/like
[HttpPost("{postId}/like")]
public async Task<ActionResult<Like>> LikePost(int postId)
{
    // Get the current user's ID
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    // Find the post by its ID, include the Likes collection
    var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == postId);

    // Check if the post exists
    if (post == null)
    {
        return NotFound();
    }

    // Check if the user has already liked the post
    var existingLike = post.Likes.FirstOrDefault(l => l.UserId == userId);
    if (existingLike != null)
    {
        // Unlike the post if already liked
        post.Likes.Remove(existingLike);
        _context.Likes.Remove(existingLike);
        await _context.SaveChangesAsync();

        return Ok("Post unliked successfully.");
    }
    else
    {
        // Like the post if not already liked
        var like = new Like
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        post.Likes.Add(like);
        _context.Likes.Add(like);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFeed), new { id = postId }, like);
    }
}

    // POST: api/feed/{userId}/follow
    [HttpPost("{userId}/follow")]
    public async Task<ActionResult<Follow>> FollowUser(string userId)
    {
        var followerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowedUserId == userId && f.FollowerId == followerId);

        if (follow != null)
        {
            return Conflict("You are already following this user.");
        }

        var newFollow = new Follow
        {
            FollowedUserId = userId,
            FollowerId = followerId
        };

        _context.Follows.Add(newFollow);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetFeed), new { userId }, newFollow);
    }

    // Add more endpoints for updating and deleting posts, comments, likes, and follows as needed
}
