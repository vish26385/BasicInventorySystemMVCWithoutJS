using ALLINONEPROJECTWITHOUTJS.Data;
using ALLINONEPROJECTWITHOUTJS.DTOs;
using ALLINONEPROJECTWITHOUTJS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ALLINONEPROJECTWITHOUTJS.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString = string.Empty;

        public AuthRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }
        public async Task<int> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == request.UserName))
                return 0;

            var PasswordHasher = new PasswordHasher<User>();
            var user = new User();
            user.UserName = request.UserName;
            user.Password = request.Password;
            request.PasswordHash = PasswordHasher.HashPassword(user, request.Password);

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_insertUser", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Username", request.UserName);
            cmd.Parameters.AddWithValue("@Email", request.UserEmail);
            cmd.Parameters.AddWithValue("@Password", request.Password);
            cmd.Parameters.AddWithValue("@PasswordHash", request.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", "User");
            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<User?> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user == null ? null : user;
        }

        public async Task<int> LoginAsync(LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == request.UserName && x.Password == request.Password);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Password is correct, proceed with login
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                    // add more claims if needed
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Issue the authentication cookie
                    await _httpContextAccessor.HttpContext.SignInAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme,
                       new ClaimsPrincipal(claimsIdentity),
                       new AuthenticationProperties { IsPersistent = true });

                    _httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.Id);
                    _httpContextAccessor.HttpContext.Session.SetString("Role", user.Role);

                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }
    }
}
