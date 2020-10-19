using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin.Security;
using ShopCreator.Models;


namespace ShopCreator.Controllers
{
    public class LogInController : Controller
    {
        private IConfiguration configuration;
        public LogInController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        //DataContext db;
        //public LogInController(DataContext context)
        //{
        //    db = context;
        //}

        [Authorize]
        public IActionResult Index()
        {
            var ClientId = configuration.GetSection("ClientId").Value;
            ViewBag.ClientId = ClientId;
            return Redirect("~/");

        }

        //private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

       // POST: /Account/LogOff
       //[HttpPost]
       //[ValidateAntiForgeryToken]
       // public ActionResult LogOff()
       // {
       //     AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
       //     return RedirectToAction("Index", "Home");
       // }

        public IActionResult SignOut()
        {
            HttpContext.SignOutAsync(
         CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignOutAsync(
        GoogleDefaults.AuthenticationScheme);
            return Redirect("~/");
            //return View("Index");
        }
    }
} 
