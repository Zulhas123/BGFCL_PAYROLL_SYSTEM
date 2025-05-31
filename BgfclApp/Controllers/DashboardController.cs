using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BgfclApp.Controllers
{
    public class DashboardController : Controller
    {
        private static readonly string EncryptionKey = "MySecretKey12345"; // Must be 16, 24, or 32 bytes long
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IDepartmentContract _departmentContract;
        private IEmployeeTypeContract _employeeTypeContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IBankContract _bankContract;
        private IEmployeeContract _employeeContract;
        private ISalarySettingContract _salarySettingContract;
        private IUserContract _userContract;
        private IConfiguration _config;
        public DashboardController(IWebHostEnvironment webHostEnvironment,
                                     IDepartmentContract departmentContract,
                                     ISalaryReportOfficerContract salaryReportOfficerContract,
                                     IEmployeeTypeContract employeeTypeContract,
                                     IBankContract bankContract,
                                     IEmployeeContract employeeContract,
                                     ISalarySettingContract salarySettingContract,
                                     IUserContract userContract,
                                     IConfiguration config
                                   )
        {

            _webHostEnvironment = webHostEnvironment;
            _departmentContract = departmentContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;
            _employeeTypeContract = employeeTypeContract;
            _bankContract = bankContract;
            _employeeContract = employeeContract;
            _salarySettingContract = salarySettingContract;
            _userContract = userContract;
            _config = config;
        }

        public IActionResult Index()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

		[HttpGet]
		public ActionResult Login()
		{
			return View();
		}
		[HttpPost]
        //public async Task<ActionResult> Login(string username, string password)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        ModelState.AddModelError("", "Invalid login attempt.");
        //        return View();
        //    }
        //    var usersData = await _userContract.GetUser();
        //    var user = usersData.FirstOrDefault(u => u.Username.Trim().Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));
        //    if (user != null)
        //    {
        //        if (user.Password.Trim() == password) 
        //        {
        //            CookieOptions cookieOptions = new CookieOptions
        //            {
        //                Expires = DateTime.Now.AddDays(1)
        //            };
        //            Response.Cookies.Append("bgfcl_user_name", username, cookieOptions);
        //            return RedirectToAction("Index", "Dashboard");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Invalid username or password");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "User not found");
        //    }

        //    return View();
        //}


        public async Task<ActionResult> Login(string username, string password)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            var usersData = await _userContract.GetUser();
            var user = usersData.FirstOrDefault(u => u.Username.Trim().Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }
            var decryptpasword = DecryptPassword(user.Password);
            if (decryptpasword != password)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
                          _config["Jwt:Issuer"],
                          null,
                          expires: DateTime.Now.AddMinutes(120),
                          signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            Response.Cookies.Append("bgfcl_user_name", username, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            });

            Response.Cookies.Append("bgfcl_auth_token", token, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(120)
            });

            //return RedirectToAction("Index", "Dashboard");
            return RedirectToAction("Index", "PayrollDashboard");
        }

        public ActionResult Logout()
        {
            Response.Cookies.Delete("bgfcl_user_name");
            Response.Cookies.Delete("bgfcl_auth_token");
            return RedirectToAction("Login");
        }

        public static string encryptPassword(string password)
        {
            var plaintext = System.Text.Encoding.UTF8.GetBytes(password);
            return System.Convert.ToBase64String(plaintext);    
        }
        public static string decryptPassword(string password)
        {
            var baseEncode = System.Convert.FromBase64String(password);
            return System.Text.Encoding.UTF8.GetString(baseEncode);
        }
        [HttpPost]
        public IActionResult LogInWithSSERP(string token, string username)
        {
            var requestDomain = $"{Request.Host}";
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

            if (!validDomains.Contains(requestDomain))
            {
                return BadRequest($"Invalid domain: {requestDomain}");
            }

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username))
            {
                return BadRequest("Token or username is missing");
            }
            var user = "Erpuser";
            var bearertoken = "SchoolSystemERPGenerateTokenForBGFCL123456789";

            if (token == bearertoken & username == user)
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
        }

        public static string EncryptPassword(string password)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey); // 16-byte key (must be 16, 24, or 32 bytes long)
                aesAlg.IV = new byte[16]; // AES IV should be 16 bytes (zeroed for simplicity)

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes); // Base64 encode to return as string
                }
            }
        }

        // Decrypt password using AES
        public static string DecryptPassword(string encryptedPassword)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.IV = new byte[16]; 

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

