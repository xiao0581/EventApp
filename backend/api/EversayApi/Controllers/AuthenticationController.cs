using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using EversayApi.Dtos;
using EversayApi.Services;
using Modules.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using User_lib;

namespace EversayApi.Controllers
{
    [ApiController]
    [Route("api/v1/authenticate")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<Applicationuser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthenticationController(UserManager<Applicationuser> userManager,
            RoleManager<ApplicationRole> roleManager,
            UserService userService,
            IPasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost]
        [Route("roles/add")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var appRole = new ApplicationRole { Name = request.Role };
            var createrole = await _roleManager.CreateAsync(appRole);

            return Ok(new { message = "Role created succesfully" });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Dtos.RegisterRequest request)
        {
            var result = await RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        private async Task<RegisterResponse> RegisterAsync(Dtos.RegisterRequest request)
        {
            try
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return new RegisterResponse { Message = "Confirm password do not match", Success = false };
                }

                var userExists = await _userManager.FindByEmailAsync(request.Email);
                if (userExists != null) return new RegisterResponse { Message = "Email address already exists", Success = false };

                userExists = new Applicationuser
                {
                    Name = request.Name,
                    Email = request.Email,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    UserName = request.Email,
                    TwoFactorEnabled = false,
                };
                var createUserResult = await _userManager.CreateAsync(userExists, request.Password);
                if (!createUserResult.Succeeded) return new RegisterResponse
                {
                    Message = $"Creation of user failed, " +
                    $"{createUserResult?.Errors?.First()?.Description}",
                    Success = false
                };

                var addUserToRoleResult = await _userManager.AddToRoleAsync(userExists, "USER");
                if (!addUserToRoleResult.Succeeded) return new RegisterResponse
                {
                    Message = $"User created byt could not be added to role, " +
                    $"{addUserToRoleResult?.Errors?.First()?.Description}",
                    Success = false
                };

                var mongoUser = new User
                {
                    userId = userExists.Id.ToString(),
                    UserName = request.Name,
                    Email = request.Email,
                    UserRole = UserType.Guest,
                    ProfilePicture = "https://eversaydevne.blob.core.windows.net/eversaydev/unicorn_avatar.png",
                    PasswordHash = ""
                };

                await _userService.RegisterUser(mongoUser, request); ;

                return new RegisterResponse
                {
                    Success = true,
                    Message = "User registered succesfully"
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse { Message = ex.Message, Success = false };
            }
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await LoginAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        private async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) return new LoginResponse { Message = "Invalid email or password", Success = false };

                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!passwordValid) return new LoginResponse { Message = "Invalid email or password", Success = false };

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
                claims.AddRange(roleClaims);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("d2f8b7e3a6c9f4d5e8f2c7b9a3d4e6f7c8b2a5f9d3e7c6b4a8f1d9e3b5c7a6f4"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddHours(24);

                var token = new JwtSecurityToken(
                    issuer: "http://localhost:5102",
                    audience: "http://localhost:5102",
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                    );

                return new LoginResponse
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Message = "Login successful",
                    Email = user?.Email,
                    Success = true,
                    UserId = user?.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new LoginResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
