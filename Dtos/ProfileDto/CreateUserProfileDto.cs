using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainiacsApi.Dtos.ProfileDto
{
    public class CreateUserProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string UniversityName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}