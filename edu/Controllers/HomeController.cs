using edu.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.VisualStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace edu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly edu_portal_dbContext _context;
    

        public HomeController(ILogger<HomeController> logger, edu_portal_dbContext context)
        {
            _logger = logger;
            _context = context;
           
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
        //public IActionResult teacher()
        //{
        //    return View();
        //}
        //public IActionResult student()
        //{
        //    return View();
        //}
        //public IActionResult forgot()
        //{
        //    return View();
        //}
        //public IActionResult login()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> login(User _user)
        //{
        //    try
        //    {
        //        var userdata = _context.Users.First(vs => vs.Username == _user.Username);
        //        if (userdata != null && VerifyPassword(_user.Userpass,userdata.Userpass))
        //        {
        //                string rol = userdata.Rolename == null ? "user" : userdata.Rolename;


        //                    var claims = new List<Claim>
        //                {
        //                    new Claim(ClaimTypes.Name, _user.Username.ToString()),
        //                    new Claim(ClaimTypes.Role,  rol),
        //                    new Claim(ClaimTypes.Dns,  userdata.Id.ToString()),
        //                    // Add any additional claims as needed
        //                };

        //            // Create claims for authentication
        //            try
        //            {
        //                var directoryInfo = new DirectoryInfo("C:\\Users\\Newtech\\source\\repos\\edu\\edu\\wwwroot\\Shared\\UserFiles\\images\\");

        //                if (directoryInfo.Exists)
        //                {
        //                    directoryInfo.CreateSubdirectory(_user.Username.ToString());
        //                }
        //            }catch(Exception ex)
        //            {

        //            }
                   


        //            var claimsIdentity = new ClaimsIdentity(
        //                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //                var authProperties = new AuthenticationProperties
        //                {
        //                    // Customize authentication properties if needed
        //                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
        //                    IsPersistent = true
        //                };

        //                await HttpContext.SignInAsync(
        //                      CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
        //                      authProperties);
        //                if (userdata.Rolename == "admin")
        //                {
        //                    return RedirectToAction("index","newscats");
        //                }
        //                if (userdata.Rolename == "teacher")
        //                {
        //                    return RedirectToAction("index", "GroupLessons");
        //                }
        //                if (userdata.Rolename == null )
        //                {
        //                     return RedirectToAction("indexstudent", "GroupLessons");
        //                }

              
               
        //        }   
        //        else
        //        {
        //            ViewBag.err = "и-мэйл хаяг эсвэл нууц үг буруу !!!";
        //   }
        //}
        //    catch (Exception ex) {
        //        ViewBag.err = "и-мэйл хаяг эсвэл нууц үг шалгана уу !!!";
        //    }

        //    return View();
        //}

        public class LoginRequest { public string username { get; set; } public string password { get; set; } }

        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginRequest req)
        {
            try
            {
                var userdata = _context.Users.FirstOrDefault(vs => vs.Username == req.username);
                if (userdata != null && VerifyPassword(req.password, userdata.Userpass))
                {
                    string rol = userdata.Rolename == null ? "user" : userdata.Rolename;
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userdata.Username.ToString()),
                        new Claim(ClaimTypes.Role,  rol),
                        new Claim(ClaimTypes.Dns,  userdata.Id.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
                            authProperties);
                            
                    return Ok(new { success = true });
                }
                return BadRequest(new { success = false, message = "Invalid username or password" });
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "An error occurred" });
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        //public IActionResult register()
        //{
            
        //    return View();
        //}
        //[HttpPost]
    
        //public async Task<IActionResult> register(User _user)
        //{
        //    var userdata = _context.Users.Where(vs=>vs.Username==_user.Username);
        //    if (userdata.Count()<1)
        //    {
        //        string hashedPassword = HashPassword(_user.Userpass);
        //        _user.Username = _user.Username;
        //        _user.Userpass = hashedPassword;
        //        _context.Add(_user);
        //        await _context.SaveChangesAsync();
        //        ViewBag.err = "Амжилттай ";
        //        //    return RedirectToAction(nameof(register));
        //    }
        //    else
        //    {
        //        ViewBag.err = "Хэрэглэгч бүртгэлтэй байна !!!";
        //    }
           
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View();
        //}
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    builder.Append(b.ToString("x2")); // Convert byte to hexadecimal string
                }

                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = HashPassword(password);
            return string.Equals(hashedInput, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
