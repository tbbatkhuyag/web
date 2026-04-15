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
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;
        private readonly edu_portal_dbContext _context;
    

        public StudentController(ILogger<StudentController> logger, edu_portal_dbContext context)
        {
            _logger = logger;
            _context = context;
           
        }

        public IActionResult depositstudentdashboard()
        {
            return View();
        }
        public IActionResult settingeditprofile()
        {
            return View();
        }
        public IActionResult settingstudentaccounts()
        {
            return View();
        }
        public IActionResult settingstudentbilling()
        {
            return View();
        }
        public IActionResult settingstudentdeleteprofile()
        {
            return View();
        }
        public IActionResult settingstudentinvoice()
        {
            return View();
        }

        public IActionResult settingstudentnotification()
        {
            return View();
        }

        public IActionResult settingstudentpayment()
        {
            return View();
        }
        public IActionResult settingstudentprivacy()
        {
            return View();
        }
        public IActionResult settingstudentreferral()
        {
            return View();
        }
        public IActionResult settingstudentsecurity()
        {
            return View();
        }
        public IActionResult settingstudentsocialprofile()
        {
            return View();
        }
        public IActionResult settingstudentsubscription()
        {
            return View();
        }
        public IActionResult settingsupportnewtickets()
        {
            return View();
        }
        public IActionResult settingsupporttickets()
        {
            return View();
        }
        public IActionResult studentprofile()
        {
            return View();
        }
        public IActionResult studentsgrid()
        {
            return View();
        }
        public IActionResult studentslist()
        {
            return View();
        }
        public IActionResult coursemessage()
        {
            return View();
        }
        public IActionResult coursestudent()
        {
            return View();
        }
        public IActionResult coursewishlist()
        {
            return View();
        }
        
       public IActionResult purchasehistory
()
        {
            return View();
        }

    }
}
