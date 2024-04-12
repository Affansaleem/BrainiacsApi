using Microsoft.AspNetCore.Identity;

namespace BrainiacsApi.Models.Profile
{
    public class Profile
    {
        public string Id { get; set; } // Using the Id from IdentityUser as the primary key

        public virtual IdentityUser CustomUser { get; set; } // Reference to the associated user

        // Other profile properties
        public string Username { get; set; }
        public string Email { get; set; }
        public string UniversityName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
