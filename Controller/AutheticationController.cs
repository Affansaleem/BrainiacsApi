using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrainiacsApi.Dtos;
using BrainiacsApi.Dtos.SignUpDto;
    using BrainiacsApi.Interface;
using BrainiacsApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace BrainiacsApi.Controller
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AutheticationController : ControllerBase
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IConfiguration _configuration;
            private readonly string _secretKey;

            private readonly IEmailService _emailService; // Inject your email service
            private readonly ILogger<AutheticationController> _logger;

            public AutheticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService, ILogger<AutheticationController> logger)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _configuration = configuration;
                _emailService = emailService;
                _logger = logger;
                _secretKey = _configuration["Jwt:SecretKey"];
            }

            [HttpPost]
            [Route("register")]
            public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, string role)
            {
                try
                {
                    _logger.LogInformation("Register method called."); // Log method entry
                    
                    // Check if user exists
                    var userExist = await _userManager.FindByEmailAsync(registerDto.Email);
                    if (userExist != null)
                    {
                        _logger.LogWarning("User already exists."); // Log user existence
                        return BadRequest("User already exists!");
                    }

                    // Create the user
                    var user = new IdentityUser
                    {
                        Email = registerDto.Email,
                        UserName = registerDto.UserName,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        EmailConfirmed = false
                    };

                    var result = await _userManager.CreateAsync(user, registerDto.Password);
                    if (result.Succeeded)
                    {
                        // Assign a role
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                        else
                        {
                            _logger.LogError("Role does not exist: {Role}", role); // Log role non-existence
                            return BadRequest("This role does not exist");
                        }

                        // Generate and send verification email
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Authetication", new { userId = user.Id, token = token }, Request.Scheme);
                        await _emailService.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by clicking this link: {confirmationLink}");

                        _logger.LogInformation("User created successfully."); // Log user creation
                        return Ok("User created successfully! Please check your email for verification.");
                    }
                    else
                    {
                        _logger.LogError("Failed to create user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description))); // Log user creation failure
                        return BadRequest("Failed to create user.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during user registration."); // Log any exception
                    return StatusCode(500, "An error occurred during user registration.");
                }
            }
            
            [HttpGet]
            [Route("confirm-email")]
            [AllowAnonymous] // Allow anonymous access to this endpoint
            public async Task<IActionResult> ConfirmEmail(string userId, string token)
{
    if (userId == null || token == null)
    {
        return BadRequest("Invalid token.");
    }

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        return BadRequest("User not found.");
    }

    if (user.EmailConfirmed)
    {
        return Ok("Email is already confirmed.");
    }

    var result = await _userManager.ConfirmEmailAsync(user, token);
    if (result.Succeeded)
    {
        return Ok("Email confirmed successfully. You can now log in.");
    }

    return BadRequest("Failed to confirm email.");
}

            [HttpPost]
            [Route("login")]
            public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
    try
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return BadRequest("Invalid email or password.");
        }

        if (!user.EmailConfirmed)
        {
            return BadRequest("Email is not confirmed.");
        }

        // Log user information
        _logger.LogInformation($"User found: {user.UserName} ({user.Email})");

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
            // Add more claims as needed
        };

        // Generate token
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Log generated token
        _logger.LogInformation($"Generated token: {tokenString}");

        // Return token in response
        return Ok(new { Token = tokenString });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating JWT token.");
        return StatusCode(500, "An error occurred while processing the request.");
    }
    }
    }
}
