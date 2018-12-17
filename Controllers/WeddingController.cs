using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WeddingPlanner.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class WeddingController : Controller
    {
        private UserContext dbContext;
        public WeddingController(UserContext context)
        {
            dbContext = context;
        }

        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            List<User> allUsers = dbContext.UsersTable.ToList();
            ViewBag.AllUsers = allUsers;
            return View();
        }
        
        //Registration process
        [HttpPost]
        [Route("/registration")]
        public IActionResult RegistrationProcess(User user)
        {
            List<User> allUsers=dbContext.UsersTable.ToList();
            ViewBag.AllUsers = allUsers;

            if (ModelState.IsValid)
            {
                if(dbContext.UsersTable.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use. use other email!!");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);

                dbContext.Add(user);
                dbContext.SaveChanges();

                //setting user's firstname in session
                HttpContext.Session.SetString("firstName", user.FirstName);
                HttpContext.Session.SetString("lastName", user.LastName);
                HttpContext.Session.SetInt32("UserId", user.UserId);

                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        //Login process
        [HttpPost]
        [Route("/login")]
        public IActionResult LoginProcess(LoginUser userSubmission)
        {
            List<User> allUsers = dbContext.UsersTable.ToList();
            ViewBag.AllUsers = allUsers;

            if(ModelState.IsValid)
            {
                var userInDb = dbContext.UsersTable.SingleOrDefault(u => u.Email == userSubmission.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email!! Maybe you need to register first!!");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);

                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Invalid password. it did not match with database password!!");
                    return View("Index");
                }
                HttpContext.Session.SetString("firstName", userInDb.FirstName);
                HttpContext.Session.SetString("lastName", userInDb.LastName);
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);

                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        //delete process
        [HttpGet]
        [Route("/delete/{deleteId}")]
        public IActionResult DeleteUser(int deleteId)
        {
            User deleteOne = dbContext.UsersTable.SingleOrDefault(r => r.UserId == deleteId);
            dbContext.UsersTable.Remove(deleteOne);
            dbContext.SaveChanges();
            return RedirectToAction("index");
        }

        //logout process
        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        //Registration/Login success process
        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.firstname = HttpContext.Session.GetString("firstName");
            ViewBag.lastname = HttpContext.Session.GetString("lastName");

            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.userId = HttpContext.Session.GetInt32("UserId");

            List<WeddingSchedule> weddingwithguest = dbContext.WeddingTable.Include( w => w.guest).ToList();

            return View(weddingwithguest);
        }

        //render newWedding page
        [HttpGet]
        [Route("/newWeddingPage")]
        public IActionResult NewWeddingPageMethod()
        {
            ViewBag.firstname = HttpContext.Session.GetString("firstName");
            ViewBag.lastname = HttpContext.Session.GetString("lastName");

            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.userid = HttpContext.Session.GetInt32("UserId");

            return View("NewWeddingPage");
        }

        [HttpPost]
        [Route("/createWeddingProcessRoute")]
        public IActionResult CreateWeddingProcessMethod(WeddingSchedule schedule)
        {

            ViewBag.firstname = HttpContext.Session.GetString("firstName");
            ViewBag.lastname = HttpContext.Session.GetString("lastName");

            

            if(ModelState.IsValid)
            {
                if(schedule.Date <= DateTime.Today)
                {
                    ModelState.AddModelError("Date", "Date must be set to the future date!!");
                    return View("NewWeddingPage");
                }
                schedule.UserId = (int)HttpContext.Session.GetInt32("UserId");

                dbContext.Add(schedule);
                dbContext.SaveChanges();
                System.Console.WriteLine("#########################");
                System.Console.WriteLine("#########################");
                System.Console.WriteLine("#########################");
                System.Console.WriteLine("#########################");
                System.Console.WriteLine("#########################");
                System.Console.WriteLine("#########################");
                return RedirectToAction("Dashboard");
            }
            return View("NewWeddingPage");
        }

        [HttpGet]
        [Route("/weddingDetailPage/{showId}")]
        public IActionResult WeddingDetail(int showId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            WeddingSchedule showOne = dbContext.WeddingTable.Include(w => w.guest).ThenInclude(g => g.User).SingleOrDefault(h =>h.WeddingId == showId);
            ViewBag.ShowOne = showOne;

            return View(showOne);
        }

        [HttpGet]
        [Route("/deleteWed/{wedId}")]
        public IActionResult DeleteWedding(int wedId)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            WeddingSchedule CurrentWedding = dbContext.WeddingTable.SingleOrDefault( d => d.WeddingId == wedId);
            dbContext.WeddingTable.Remove(CurrentWedding);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("//rsvp/{wId}")]
        public IActionResult RSVP(int wId)
        {
            //check user in session
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            //get the session id 
            User currentUser = dbContext.UsersTable.SingleOrDefault( c => c.UserId == HttpContext.Session.GetInt32("UserId"));

            WeddingSchedule currWed = dbContext.WeddingTable.Include( w => w.guest).ThenInclude(g => g.User).SingleOrDefault(r => r.WeddingId == wId);
            Guest thisguest = dbContext.Guest.Where(j => j.WeddingId == wId && j.UserId == currentUser.UserId).SingleOrDefault();
            
            if(thisguest != null)
            {
                currentUser.guest.Remove(thisguest);
            }
            else{
                Guest newGuest = new Guest
                {
                    UserId = currentUser.UserId,
                    WeddingId = currWed.WeddingId,
                };
                currWed.guest.Add(newGuest);
            }
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        
    }
}
