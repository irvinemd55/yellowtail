using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using YellowTail.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace YellowTail.Controllers
{
    //controller for the homepage login and registration
    public class HomeController : Controller
    {
        //context passwed through the homecontroller with _to differentiate
        // User passed through as the potential activeUser
        //created privated for this specific session
        private Context _context;
        private User ActiveUser
       {
           //activeUser call checks to see if the userId on the table matches the id currently available in session
            get{ return _context.users.Where(u => u.userId == HttpContext.Session.GetInt32("id")).FirstOrDefault();}
        }
        private User ActiveUserDetailed
        {
            //query that returns the active user details
            get{ return _context.users
                .Where(u => u.userId == HttpContext.Session.GetInt32("id"))
                .FirstOrDefault();}
        }
        //homecontroller has contexted passed through it
        public HomeController(Context context)
        {
            _context = context;
        }
        //index assings a list of users to the vaible user and returns the index view
        [HttpGet]
        public IActionResult Index()
        {
            var users = _context.users.ToList();
            return View();
        }
        // Post for creating a new user. Mehtod is called register 
        //passes through a newuser parameter
        [HttpPost]
        public IActionResult Register(NewUser newUser)
        {
            //password hasher creates a hasher atrribute for the newuser
            PasswordHasher<NewUser> hasher = new PasswordHasher<NewUser>();
            //if the user email being passed through context is equal to a user email on file then it will return an error
            if(_context.users.Where(u => u.email == newUser.Email).SingleOrDefault() != null)
                ModelState.AddModelError("Username", "Username in use");
            //if the modelstate is valid then a new user model will be created with the newuser fields added
            //password equals the hashed password that is created using the string added and the password hasher method
            if(ModelState.IsValid)
            {
                User User = new User
                {
                    firstname = newUser.FirstName,
                    lastname = newUser.LastName,
                    email = newUser.Email,
                    password = hasher.HashPassword(newUser, newUser.Password),
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now,
                };
            //the user is added to the user model using entity
                User theUser = _context.Add(User).Entity;
                _context.SaveChanges();
            //user id for the user is added to session
            //redirected to Dashboard dashboard
                HttpContext.Session.SetInt32("id", theUser.userId);
                return RedirectToAction("Index", "Dashboard");
            }
            // validation failure sends user back to index page
            return View("Index");
        }
        //httppost request for login
        //loguser passed through from the forms side of the page
        [HttpPost]
        public IActionResult Login(LogUser logUser)
        {
            //new password hasher instance created
            PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
            //userloging in is found by searching the database for a matching email
            User userToLog = _context.users.Where(u => u.email == logUser.LogEmail).SingleOrDefault();
            if(userToLog == null)
            //if no user found then change modelstate errors
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");
            else if(hasher.VerifyHashedPassword(logUser, userToLog.password, logUser.LogPassword) == 0)
            {
                //password verification that checks whether passwords match
                //if no matches then add to modelstate errors
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");
            }
            if(!ModelState.IsValid)
            //if the model state isn't valid then send back to index with errors
                return View("Index");
            //if model state is valid then assign the userId to session and redirect to dashboard
            HttpContext.Session.SetInt32("id", userToLog.userId);
            return RedirectToAction("Index", "Dashboard");
        }
        //logout route clears sessions and sends back to index
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        //show takes in a user id and checks for the id in the table
        //redirects to index if no id found or returns the user information
        public IActionResult Show(int id)
        {
            if(HttpContext.Session.GetInt32("id") == null ||
                HttpContext.Session.GetInt32("id") != id)
                return RedirectToAction("Index");
            return View(this.ActiveUser);
        }
    }
}