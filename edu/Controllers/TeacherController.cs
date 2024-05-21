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
    public class TeacherController : Controller
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly edu_portal_dbContext _context;
    

        public TeacherController(ILogger<TeacherController> logger, edu_portal_dbContext context)
        {
            _logger = logger;
            _context = context;
           
        }

        public IActionResult addcourse()
        {
            return View();
        }
        public IActionResult instructorcourse()
        {
            return View();
        }
        public IActionResult instructorearnings()
        {
            return View();
        }
        public IActionResult instructoreditprofile()
        {
            return View();
        }
        public IActionResult instructororders()
        {
            return View();
        }
        public IActionResult instructorpayouts()
        {
            return View();
        }

        public IActionResult instructorreviews()
        {
            return View();
        }

        public IActionResult instructorsecurity()
        {
            return View();
        }
        public IActionResult instructorsocialprofiles()
        {
            return View();
        }
        public IActionResult instructorstudentgrid()
        {
            return View();
        }


    }
}
