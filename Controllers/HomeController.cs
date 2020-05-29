using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using regLog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace regLog.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
    
        
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
    
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            List<User> AllUsers = dbContext.Users.ToList();
            
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
    
            if(ModelState.IsValid)
            {
                
                if (!dbContext.Users.Any(u => u.Email == user.Email))
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserId",user.UserId);
                    return RedirectToAction("Success");

                    
                }
                
                
                ModelState.AddModelError("Email", "Email already in use!");

                return View("Index");
                

            }
            else{
                return View("Index");
            }
            
        } 
        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
        if(ModelState.IsValid)
        {
            
            var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
            
            if(userInDb == null)
            {
                
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }
            
            
            var hasher = new PasswordHasher<LoginUser>();
            
            
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
            
            
                if(result == 0)
                {
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
                }
                else{
                    HttpContext.Session.SetInt32("UserId",userInDb.UserId);
                    return RedirectToAction("Success");
                }

            }
            else{
                return View("Index");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            User userInDb = dbContext.Users.FirstOrDefault( u => u.UserId == HttpContext.Session.GetInt32("UserId") );
            if(userInDb == null)
            {
                return RedirectToAction("Logout");
            }
            return View();
        }

        [HttpGet("logout")]
        public IActionResult Logout()   
        {
        HttpContext.Session.Clear();
        return View("Index");
        }
    }
}
