using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {
        private static readonly string EncryptionKey = "MySecretKey12345"; // Must be 16, 24, or 32 bytes long
        private IDepartmentContract _departmentContract;
        private ResponseViewModel _responseViewModel;
        private IUserContract _userContract;
        private IConfiguration _config;
        public AuthController(IDepartmentContract departmentContract, ResponseViewModel responseViewModel, IUserContract userContract, IConfiguration config)
        {
            _departmentContract = departmentContract;
            _responseViewModel = responseViewModel;
            _userContract = userContract;
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogInWithSSERP()
        {
            // Extract the domain from the request URL
            //var requestDomain = $"{Request.Scheme}://{Request.Host}";
            var requestDomain = $"{Request.Host}";
            var tok = GenerateToken();
            // Define the valid domain(s) for validation
            var validDomains = new List<string>
            {
                "dev-erp.jabaschool.com",
                "qc-erp.jabaschool.com",
                "erp.mycampus24.com",
                "bgfscpayroll.jabawin.com",
                "https://dev-erp.jabaschool.com",
                "https://qc-erp.jabaschool.com",
                "https://erp.mycampus24.com",
                "https://localhost:44365",
                "localhost:44365"
            };

            // Check if the request domain is in the list of valid domains
            if (!validDomains.Contains(requestDomain))
            {
                return BadRequest($"Invalid domain: {requestDomain}");
            }

            // Extract the token and username from the request headers
            var token = Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            var username = Request.Headers["Username"].ToString();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username))
            {
                return BadRequest("Token or username is missing");
            }

        
            // Decrypt the token (assuming DecodeJwtToken is your method for this)
            //var decryptedToken = DecodeJwtToken(token);
            var user = "Erpuser";
            var bearertoken = "SchoolSystemERPGenerateTokenForBGFCL123456789";

            if (token== bearertoken & username == user)
            {
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1)
                };
                Response.Cookies.Append("bgfcl_user_name", username, cookieOptions);
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return Unauthorized();
            }
           

            if (ValidateJwtToken(token, out ClaimsPrincipal claimsPrincipal))
            {
                // Extract the username from the token claims
                var validatedUsername = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (username == validatedUsername)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            return Unauthorized();
        }

        public static string DecodeJwtToken(string jwtToken)
        {
            // JWT consists of 3 parts: header, payload, and signature, separated by periods (.)
            var parts = jwtToken.Split('.');

            // Decode each part (header and payload)
            var header = DecodeBase64Url(parts[0]);
            var payload = DecodeBase64Url(parts[1]);

            return $"Header: {header}, Payload: {payload}";
        }

        private static string DecodeBase64Url(string base64Url)
        {
            // Replace Base64Url characters with Base64 characters
            base64Url = base64Url.Replace('-', '+').Replace('_', '/');

            // Add padding if necessary
            switch (base64Url.Length % 4)
            {
                case 2: base64Url += "=="; break;
                case 3: base64Url += "="; break;
            }

            // Decode the Base64 string
            var base64Bytes = Convert.FromBase64String(base64Url);
            return Encoding.UTF8.GetString(base64Bytes);
        }
        private bool ValidateJwtToken(string token, out ClaimsPrincipal claimsPrincipal)
        {
            claimsPrincipal = null;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SchoolSystemERPGenerateTokenForBGFCL123456789"));

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "SchoolSystemERP",
                    ValidAudience = "BGFCL",
                    IssuerSigningKey = securityKey,
                    RequireSignedTokens = true
                };

                claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }

        public static string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SchoolSystemERPGenerateTokenForBGFCL123456789"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, "Erpuser"),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim("role", "User")
            };

            var token = new JwtSecurityToken(
                issuer: "SchoolSystemERP",
                audience: "BGFCL",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] User _user)
        {
            string username = _user.Username;
            string password = _user.Password;

            var usersData = await _userContract.GetUser();
            var user = usersData.FirstOrDefault(u => u.Username.Trim().Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return NotFound("User not found");
            }
            var decryptpasword = DecryptPassword(user.Password);
            if (decryptpasword != password)
            {
                return Unauthorized("Invalid username or password");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
                          _config["Jwt:Issuer"],
                          null,
                          expires: DateTime.Now.AddMinutes(120),
                          signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
            return Ok(token);
        }

        public static string DecryptPassword(string encryptedPassword)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.IV = new byte[16]; // AES IV should be 16 bytes (zeroed for simplicity)

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword); // Convert from Base64
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes); // Return decrypted string
                    }
                }
            }
            catch
            {
                return "Decryption Failed"; // Handle errors
            }
        }
    }
}
