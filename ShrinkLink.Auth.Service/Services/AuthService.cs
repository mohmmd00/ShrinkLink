using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ShrinkLink.Auth.Service.Models.Entities;
using ShrinkLink.Auth.Service.Models.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using ShrinkLink.Contracts.DataTransferObjects.AuthorizationService;

namespace ShrinkLink.Auth.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
        }

        public async Task<RegisterResponseTransferObject> Register(RegisterRequestTransferObject request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 5 || request.Username.Length > 35)
            {
                return new RegisterResponseTransferObject
                {
                    Message = "Username is invalid",
                    Status = RegisterResultStatus.InvalidUsername
                };
            }

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 5 || request.Password.Length > 35)
            {
                return new RegisterResponseTransferObject
                {
                    Message = "Password is invalid",
                    Status = RegisterResultStatus.InvalidPassword
                };
            }

            // Email format validation
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (string.IsNullOrWhiteSpace(request.Email) || !emailRegex.IsMatch(request.Email))
            {
                return new RegisterResponseTransferObject
                {
                    Message = "Email is invalid",
                    Status = RegisterResultStatus.InvalidEmailAddress
                };
            }

            if (await _userRepository.IsUserExistsByUsername(request.Username, ct))
            {
                return new RegisterResponseTransferObject
                {
                    Message = "Username already exists",
                    Status = RegisterResultStatus.UsernameExists
                };
            }

            if (await _userRepository.IsUserExistsByEmail(request.Email, ct))
            {
                return new RegisterResponseTransferObject
                {
                    Message = "Email address already exists",
                    Status = RegisterResultStatus.EmailAddressExists
                };
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);

            var roleId = await _roleRepository.FetchRolesIdAsync("User",ct);

            var newUser = new User(
                username: request.Username,
                email: request.Email,
                passwordSalt: salt,
                passwordHash: hashedPassword,
                roleId: roleId
            );

            await _userRepository.CreateAsync(newUser, ct);

            return new RegisterResponseTransferObject
            {
                Message = "Registration successful",
                Status = RegisterResultStatus.Success
            };
        }
        public async Task<LoginResponseTransferObject> Login(LoginRequestTransferObject request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new LoginResponseTransferObject
                {
                    Message = "Username or password must not be empty.",
                    Status = LoginResultStatus.InvalidInput
                };
            }


            var fetchedWantedUser = await _userRepository.FetchUserByUsername(request.Username, ct);
            if (fetchedWantedUser == null)
            {
                return new LoginResponseTransferObject
                {
                    Message = "Username or password is incorrect.",
                    Status = LoginResultStatus.InvalidUsernameOrPassword
                };
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, fetchedWantedUser.PasswordHash);
            if (!isPasswordValid)
            {
                return new LoginResponseTransferObject
                {
                    Message = "Username or password is incorrect.",
                    Status = LoginResultStatus.InvalidUsernameOrPassword
                };
            }

            var generatedToken = GenerateJwtToken(fetchedWantedUser);

            return new LoginResponseTransferObject
            {
                Token = generatedToken,
                ExpiresAt = DateTime.Now.AddHours(6),
                Message = "Login successful.",
                Status = LoginResultStatus.Success
            };
        }
        public async Task<UserBasicInformationTransferObject> UserBasicDetails(string token, CancellationToken ct)
        {
            try
            {
                var claims = ParseJwtToken(token);

                if (string.IsNullOrWhiteSpace(token) || claims == null)
                {
                    return new UserBasicInformationTransferObject
                    {
                        Status = UserInfoResultStatus.MissingToken,
                        Message = "Unauthorized : Token is missing or Invalid."
                    };
                }


                
                if (!claims.TryGetValue(ClaimTypes.NameIdentifier, out var userId) ||
                    !claims.TryGetValue(ClaimTypes.Name, out var userName))
                {
                    return new UserBasicInformationTransferObject
                    {
                        Status = UserInfoResultStatus.InvalidToken,
                        Message = "Unauthorized : Required claims are missing in token."
                    };
                }

                return new UserBasicInformationTransferObject
                {
                    UserInformation = new UserInformation()
                    {
                        Id = Guid.Parse(userId),
                        Username = userName,
                    },
                    Status = UserInfoResultStatus.Success,
                    Message = "User info extracted successfully."
                };
            }
            catch (SecurityTokenException)
            {
                return new UserBasicInformationTransferObject
                {
                    Status = UserInfoResultStatus.InvalidToken,
                    Message = "Invalid security token."
                };
            }
            catch (Exception ex)
            {
                return new UserBasicInformationTransferObject
                {
                    Status = UserInfoResultStatus.Failed,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }





        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(6),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private Dictionary<string, string> ParseJwtToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return null; // Invalid token: empty or null
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new SecurityTokenException("JWT key is missing in configuration.");
                }
                var key = Encoding.UTF8.GetBytes(jwtKey);

                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    throw new SecurityTokenException("JWT issuer or audience is missing in configuration.");
                }

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var cleanedUpToken = token.StartsWith("Bearer ")
                    ? token.Substring("Bearer ".Length).Trim()
                    : token;

                if (!handler.CanReadToken(cleanedUpToken))
                {
                    return null; // Invalid token: cannot be read
                }

                var principal = handler.ValidateToken(cleanedUpToken, validationParameters, out var validatedToken);

                var claims = principal.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

                return claims;
            }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                return null; // Invalid token: signature key mismatch
            }
            catch (SecurityTokenExpiredException)
            {
                return null; // Invalid token: expired
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return null; // Invalid token: invalid signature
            }
            catch (SecurityTokenException)
            {
                return null; // Invalid token: other security token issues
            }
            catch (Exception)
            {
                return null; // Invalid token: other unexpected errors
            }
        }

    }
}