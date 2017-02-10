using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GA.Data;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace GA.Controllers
{
    public class EventsController : Controller
    {
        private GAEntities db = new GAEntities();

        // GET: Events
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Return all the event that have not yet started.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEvents([DataSourceRequest] DataSourceRequest request)
        {
            var events = db.Event
                .Where(e => e.EventStart > DateTime.Today)
                .OrderBy(e => e.EventStart)
                .ToList();
            List<Event> list = new List<Event>();
            foreach (Event e in events)
            {
                list.Add(new Event()
                {
                    Course = e.Course,
                    Description = e.Description,
                    EntryFee = e.EntryFee,
                    EventEnd = e.EventEnd,
                    EventId = e.EventId,
                    EventStart = e.EventStart,
                    SignUpEnd = e.SignUpEnd,
                    SignUpStart = e.SignUpStart,
                    Start = e.Start,
                    Title = e.Title,
                    EventDocuments = null,
                });
            }
            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetList(int year)
        {
            DateTime start = new DateTime(year, 1, 1);
            DateTime end = new DateTime(year, 12, 31);
            var events = db.Event
                .Where(e => e.EventStart >= start && e.EventStart <= end)
                .OrderBy(e => e.EventStart)
                .ToList();
            List<Event> list = new List<Event>();
            foreach (Event e in events)
            {
                list.Add(new Event()
                {
                    Course = e.Course,
                    Description = e.Description,
                    EntryFee = e.EntryFee,
                    EventEnd = e.EventEnd,
                    EventId = e.EventId,
                    EventStart = e.EventStart,
                    SignUpEnd = e.SignUpEnd,
                    SignUpStart = e.SignUpStart,
                    Start = e.Start,
                    Title = e.Title,
                    EventDocuments = null,
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFlights()
        {
            var flights = db.Flight
                .OrderBy(f => f.FlightId)
                .ToList();
            List<Flight> list = new List<Flight>();
            foreach(Flight flight in flights)
            {
                list.Add(new Flight()
                {
                    FlightId = flight.FlightId,
                    Name = flight.Name
                });
            }


            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Return all the events that have completed this year
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetResults(int year)
        {
            DateTime start = new DateTime(year, 1, 1);
            DateTime end = new DateTime(year, 12, 31);
            var events = db.Event.Where(e => e.SignUpEnd <= DateTime.Today
                && e.EventStart >= start && e.EventStart <= end)
                .OrderByDescending(e => e.EventStart);
            List<Event> list = new List<Event>();
            foreach (Event e in events)
            {
                list.Add(new Event()
                {
                    Course = e.Course,
                    Description = e.Description,
                    EntryFee = e.EntryFee,
                    EventEnd = e.EventEnd,
                    EventId = e.EventId,
                    EventStart = e.EventStart,
                    SignUpEnd = e.SignUpEnd,
                    SignUpStart = e.SignUpStart,
                    Start = e.Start,
                    Title = e.Title,
                    EventDocuments = null,
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailTemplate()
        {
            return View();
        }

        // GET: Events/Create
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create()
        {
            DateTime now = DateTime.Today;

            Event evt = new Event()
            {
                EventStart = now.AddDays(14),
                EventEnd = now.AddDays(14),
                SignUpStart = now,
                SignUpEnd = now.AddDays(7),
                Title = string.Empty,
            };

            return View(evt);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create([Bind(Include = "EventId,Title,Description,Course,Start,SignUpStart,SignUpEnd,EventStart,EventEnd,EntryFee")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Event.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult Edit([Bind(Include = "EventId,Title,Description,Course,Start,SignUpStart,SignUpEnd,EventStart,EventEnd,EntryFee")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Event.Find(id);
            db.Event.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
