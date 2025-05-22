using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Webapp.Contracts.Dtos;
using ShrinkLink.Auth.Service.Models.Entities;
using ShrinkLink.Auth.Service.Models.Interfaces;
using Microsoft.IdentityModel.Tokens;
using ShrinkLink.Auth.Service.Models.DataTransferObjects;

namespace ShrinkLink.Auth.Service.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;

        public AuthorizationService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
        }

        public async Task<RegisterResponseTransferObject> Register(RegisterTransferObject request, CancellationToken ct)
        {
            var isUsernameExists = await _userRepository.IsUserExistsByUsername(request.Username, ct);
            if (!isUsernameExists)
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                if (salt != null)
                {
                    //var new12ser = new User()
                    //{
                    //    Id = new Guid(),
                    //    Username = request.Username,
                    //    CreatedAt = DateTime.Now,
                    //    Email = request.Email, // must impl verifing email 
                    //    PasswordSalt = salt,
                    //    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, salt),
                    //    RoleId = _roleRepository.FetchRolesIdAsync("User").Result,
                    //};

                    var newUser = new User
                        (
                            username:request.Username,
                            email:request.Email,
                            passwordSalt:salt,
                            passwordHash: BCrypt.Net.BCrypt.HashPassword(request.Password, salt),
                            roleId: _roleRepository.FetchRolesIdAsync("User").Result

                        );
                    await _userRepository.CreateAsync(newUser, ct);
                    return new RegisterResponseTransferObject() { Message = "register was successful" };
                }
            }

            return null;
        }

        public async Task<LoginResponseTransferObject> Login(LoginTransferObject request, CancellationToken ct)
        {
            var fetchedWantedUser = await _userRepository.FetchUserByUsername(request.Username, ct);
            if (fetchedWantedUser != null)
            {
                if (BCrypt.Net.BCrypt.Verify(request.Password, fetchedWantedUser.PasswordHash))
                {
                    var generatedToken = GenerateJwtToken(fetchedWantedUser);
                    var response = new LoginResponseTransferObject()
                    {
                        Token = generatedToken,
                        ExpiresAt = DateTime.Now.AddHours(6)
                    };

                    return response;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public async Task<UserBaseInformationTransferObject> UserBasicDetails(string jwtToken, CancellationToken ct)
        {
            try
            {
                
                var userId = ParseJwtToken(jwtToken);
                if (string.IsNullOrEmpty(userId))
                {
                    return null; 
                }

                
                var fetchedUser = await _userRepository.FetchUserById(userId, ct);
                if (fetchedUser == null)
                {
                    return null; 
                }

                var userBaseInfo = new UserBaseInformationTransferObject()
                {
                    Id = fetchedUser.Id,
                    Username = fetchedUser.Username
                };
                return userBaseInfo;
            }
            catch (SecurityTokenException)
            {
                return null; 
            }
            catch (Exception)
            {
                return null;  
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

        private string ParseJwtToken(string jwtToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return userId;
            }
            catch (SecurityTokenException)
            {
                return null; 
            }
            catch (Exception)
            {
                return null; 
            }
        }
    }
}