using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using YellowTail.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace YellowTail.Controllers
{
    //controller for dashboard and wedding functionality

    public class DashboardController : Controller
    {
        // creates private context and active user
        private Context _context;
        //activeuser grabs user model through session id
        private User ActiveUser
        {
            get{ return _context.users.Where(u => u.userId == HttpContext.Session.GetInt32("id")).FirstOrDefault();}
        }
        //wedding controller has context passed through it
        public DashboardController(Context context)
        {
            _context = context;
        }
        //index returns user to homepage if no active user present
        public IActionResult Index()
        {
            if(ActiveUser == null)
                return RedirectToAction("Index", "Home");
            Dashboard dashData = new Dashboard
            //dashdata creates new dashboard with weddings and active user passed through as properties
            {
                Activities = _context.activities.Include(w => w.Participants).ToList(),
                User = ActiveUser
            };
            

        return View(dashData);
        }
        public IActionResult Create(ViewActivity newActivity)
        {
            //post action for creating a new wedding
            //redirects to home page if no activeUser found
            if(ActiveUser == null)
                return RedirectToAction("Index", "Home");
            
            if(ModelState.IsValid)
            {
                
                Activity activity = new Activity
                {
                    title = newActivity.Title,
                    description = newActivity.Description,
                    datetime = newActivity.DateTime,
                    duration = newActivity.Duration,
                    userId = ActiveUser.userId

                };
                //add wedding created to database and save changes
                //redirect to dashboard
                _context.activities.Add(activity);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            //otherwise return to created page with newwedding passed through
            return View(newActivity);
        }
        //adds rsvp with user id passed through
        //returns to homepage if no user present
        public IActionResult JoinActivity(int id)
        {
            if(ActiveUser == null)
            return RedirectToAction("Index", "Home");
            //creates new rsvp id with wedding id and user id as parameters
            //adds to rsvps and saves changes
            //redirects to dashboard
            Participant participant = new Participant
            {
                userId = ActiveUser.userId,
                activityId = id
            };
            _context.participants.Add(participant);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        // passes through wedding id for removing rsvp
        //redirects to homepage is active user isn't present
        public IActionResult Unjoin(int id)
        {
            //grabs rsvp to be deleted by filtering based on userId and wedding id
            if(ActiveUser == null)
                return RedirectToAction("Index", "Home");
            Participant toDelete = _context.participants.Where(r => r.activityId == id)
                                          .Where(r => r.userId == ActiveUser.userId)
                                          .SingleOrDefault();
            //removes rsvp from table and save changes
            //redirects to dashboard
            _context.participants.Remove(toDelete);
            _context.SaveChanges();
            return RedirectToAction("Index");  

        }
        public IActionResult DeleteActivity(int id)
        {
            //grabs rsvp to be deleted by filtering based on userId and wedding id
            if(ActiveUser == null)
                return RedirectToAction("Index", "Home");
            Activity deleter = _context.activities.Where(r => r.activityId == id)
                                          .SingleOrDefault();
        
            _context.activities.Remove(deleter);
            _context.SaveChanges();
            return RedirectToAction("Index");  

        }
        //route for showing wedding
        //passes wedding id through
        //checks to se if userId is present and returns to  dashboard if not present
        public IActionResult Show(int id)
        {
            if(HttpContext.Session.GetInt32("id") == null){
                return RedirectToAction("Index");
            }
            // assigns attendee variable to the list of guests in the rsvps
            //attaches guests to viewbag
            //returns a view passing through single wedding based on id presented
        var attendees = _context.participants.Where(w => w.activityId == id).Include(w => w.participant).ToList();
            ShowActivity showData = new ShowActivity
            //dashdata creates new dashboard with weddings and active user passed through as properties
            {
                Activity = _context.activities.Where(w => w.activityId == id).SingleOrDefault(),
                User = ActiveUser
            };
  
           return View(showData);
           
            // return View(_context.activities.Where(w => w.activityId == id).SingleOrDefault());
        }
    }
}