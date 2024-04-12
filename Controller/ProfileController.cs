using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrainiacsApi.Models.Profile;
using BrainiacsApi.DTOs.Profile;
using BrainiacsApi.Data;
using Microsoft.AspNetCore.Identity;
using BrainiacsApi.Dtos.ProfileDto;

namespace BrainiacsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly BrainiacsDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(BrainiacsDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Profile
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileDTO>>> GetProfiles()
        {
            var profiles = await _context.Profiles.ToListAsync();
            var profileDTOs = new List<ProfileDTO>();

            foreach (var profile in profiles)
            {
                profileDTOs.Add(new ProfileDTO
                {
                    Id = profile.Id,
                    Username = profile.Username,
                    Email = profile.Email,
                    UniversityName = profile.UniversityName,
                    PhoneNumber = profile.PhoneNumber,
                    ProfilePictureUrl = profile.ProfilePictureUrl
                });
            }

            return profileDTOs;
        }

        // GET: api/Profile/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileDTO>> GetProfile(string id)
        {
            var profile = await _context.Profiles.FindAsync(id);

            if (profile == null)
            {
                return NotFound();
            }

            var profileDTO = new ProfileDTO
            {
                Id = profile.Id,
                Username = profile.Username,
                Email = profile.Email,
                UniversityName = profile.UniversityName,
                PhoneNumber = profile.PhoneNumber,
                ProfilePictureUrl = profile.ProfilePictureUrl
            };

            return profileDTO;
        }

        // PUT: api/Profile/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(string id, ProfileDTO profileDTO)
        {
            if (id != profileDTO.Id)
            {
                return BadRequest();
            }

            var profile = new Profile
            {
                Id = profileDTO.Id,
                Username = profileDTO.Username,
                Email = profileDTO.Email,
                UniversityName = profileDTO.UniversityName,
                PhoneNumber = profileDTO.PhoneNumber,
                ProfilePictureUrl = profileDTO.ProfilePictureUrl
            };

            _context.Entry(profile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return Conflict(); // Resource was modified by another request
                }
            }

            return NoContent();
        }

        // POST: api/Profile
        [HttpPost]
        public async Task<ActionResult<ProfileDTO>> CreateProfile(CreateUserProfileDto createUserProfileDto)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User); // Get the currently logged-in user

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var profile = new Profile
            {
                Id = user.Id, // Set the Id property to match the IdentityUser Id
                Username = createUserProfileDto.Username,
                Email = createUserProfileDto.Email,
                UniversityName = createUserProfileDto.UniversityName,
                PhoneNumber = createUserProfileDto.PhoneNumber,
                ProfilePictureUrl = createUserProfileDto.ProfilePictureUrl
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            // Return the newly created profile DTO
            return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, createUserProfileDto);
        }

        // DELETE: api/Profile/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProfileExists(string id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}
