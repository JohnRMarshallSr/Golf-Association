using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GA.Data;
using GA.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Diagnostics;

namespace GA.Controllers
{
    public class MoneyController : Controller
    {
        private GAEntities db = new GAEntities();

        // GET: Money
        public ActionResult Index()
        {
            var memberWinnings = db.MemberWinnings.Include(m => m.Event).Include(m => m.Members).Include(m => m.WinningTypes);
            return View(memberWinnings.ToList());
        }

        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult Get([DataSourceRequest] DataSourceRequest request, int year)
        {
            List<Winnings> winnings = new List<Winnings>();

            var memberWinnings = from mw in db.MemberWinnings
                                 where mw.Event.EventEnd.Year == year
                                 group mw by new { mw.MemberId, mw.WinningTypeId } into grp
                                 select new
                                 {
                                     MemberId = grp.Key.MemberId,
                                     TypeId = grp.Key.WinningTypeId,
                                     Data = grp.Sum(mw => mw.Amount),
                                 };

            var ordered = memberWinnings.OrderBy(x => x.MemberId).ToList();
            foreach(var mw in ordered)
            {
                // See if the current member winning (mw) has an entry in the list
                Winnings member = winnings.Where(w => w.MemberId == mw.MemberId).FirstOrDefault();
                if(member == null)
                {
                    var m = db.Members.Find(mw.MemberId);

                    member = new Winnings();
                    member.MemberId = mw.MemberId;
                    member.Member = new Members()
                    {
                        Firstname = m.Firstname,
                        Lastname = m.Lastname,
                        MemberId = m.MemberId
                    };

                    winnings.Add(member);
                }

                switch(mw.TypeId)
                {
                    case 1: // Tournament
                        member.Tournament = mw.Data;
                        break;
                    case 2:     // Side Games
                        member.SideGame = mw.Data;
                        break;
                    default:
                        break;
                }
            }

            return Json(winnings.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult GetWinnings(int year)
        {
            List<MemberWinnings> list = new List<MemberWinnings>();

            var winnings = db.MemberWinnings
                .Where(mw => mw.Event.EventEnd.Year == year)
                .OrderBy(mw => mw.Members.Lastname)
                .ThenBy(mw => mw.Members.Firstname)
                .ThenBy(mw => mw.Event.EventEnd)
                .ToList();
            foreach(MemberWinnings winning in winnings)
            {
                MemberWinnings mw = new MemberWinnings()
                {
                    Amount = winning.Amount,
                    Event = new Event()
                    {
                        EventId = winning.EventId,
                        Title = winning.Event.Title,
                        EventEnd = winning.Event.EventEnd
                    },
                    MemberId = winning.MemberId,
                    Members = new Members()
                    {
                        MemberId = winning.MemberId,
                        Firstname = winning.Members.Firstname,
                        Lastname = winning.Members.Lastname,
                    },
                    MemberWinningId = winning.MemberWinningId,
                    WinningTypeId = winning.WinningTypeId,
                    WinningTypes = new WinningTypes()
                    {
                        WinningType = winning.WinningTypes.WinningType,
                        WinningTypeId = winning.WinningTypes.WinningTypeId,
                    }
                };
                list.Add(mw);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMember([DataSourceRequest] DataSourceRequest request, int memberId, int year)
        {
            List<MemberWinning> memberWinnings = new List<MemberWinning>();

            var winnings = db.MemberWinnings
                .Where(mw => mw.MemberId == memberId && mw.Event.EventEnd.Year == year)
                .OrderBy(mw => mw.EventId)
                .ToList();

            foreach(MemberWinnings mw in winnings)
            {
                MemberWinning memberWinning = memberWinnings
                    .Where(x => x.EventId == mw.EventId)
                    .FirstOrDefault();
                if(memberWinning == null)
                {
                    memberWinning = new MemberWinning();
                    memberWinning.Event = new Event()
                    {
                        EventEnd = mw.Event.EventEnd,
                        Title = mw.Event.Title,
                    };
                    memberWinning.EventId = mw.EventId;
                    memberWinning.Flight = new Flight()
                    {
                        FlightId = mw.FlightId,
                        Name = mw.Flight.Name,
                    };
                    memberWinning.MemberId = mw.MemberId;

                    memberWinnings.Add(memberWinning);
                }

                switch (mw.WinningTypeId)
                {
                    case 1: // Tournament
                        memberWinning.Tournament += mw.Amount;
                        break;
                    case 2:     // Side Games
                        memberWinning.SideGame += mw.Amount;
                        break;
                    default:
                        break;
                }
            }

            return Json(memberWinnings.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Only returns Tournament Result, no Skins
        /// </summary>
        /// <param name="request"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public ActionResult GetTournament([DataSourceRequest] DataSourceRequest request, int eventId)
        {
            List<MemberWinning> memberWinnings = new List<MemberWinning>();
            int skinFlightId = db.Flight
                .Where(f => f.Name == "Skins")
                .Select(f => f.FlightId)
                .Single();
            var winnings = db.MemberWinnings
                .Where(mw => mw.EventId == eventId && mw.FlightId != skinFlightId)
                .OrderBy(mw => mw.FlightId)
                .ThenBy(mw => mw.Amount)
                .ToList();

            foreach (MemberWinnings mw in winnings)
            {
                MemberWinning memberWinning = memberWinnings
                    .Where(x => x.MemberId == mw.MemberId)
                    .FirstOrDefault();
                if (memberWinning == null)
                {
                    memberWinning = new MemberWinning();
                    Members member = db.Members.Find(mw.MemberId);
                    memberWinning.Member = new Members()
                    {
                        Firstname = member.Firstname,
                        Lastname = member.Lastname,
                        MemberId = member.MemberId,
                    };
                    memberWinning.Flight = new Flight()
                    {
                        FlightId = mw.FlightId,
                        Name = mw.Flight.Name,
                    };
                    memberWinning.EventId = mw.EventId;
                    memberWinning.MemberId = mw.MemberId;

                    memberWinnings.Add(memberWinning);
                }

                switch (mw.WinningTypeId)
                {
                    case 1: // Tournament
                        memberWinning.Tournament += mw.Amount;
                        break;
                    case 2:     // Side Games
                        memberWinning.SideGame += mw.Amount;
                        break;
                    default:
                        break;
                }
            }

            return Json(memberWinnings.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSkins([DataSourceRequest] DataSourceRequest request, int eventId)
        {
            List<MemberWinning> memberWinnings = new List<MemberWinning>();
            int skinFlightId = db.Flight
                .Where(f => f.Name == "Skins")
                .Select(f => f.FlightId)
                .Single();
            var winnings = db.MemberWinnings
                .Where(mw => mw.EventId == eventId && mw.FlightId == skinFlightId)
                .OrderBy(mw => mw.Amount)
                .ToList();

            foreach (MemberWinnings mw in winnings)
            {
                MemberWinning memberWinning = memberWinnings
                    .Where(x => x.MemberId == mw.MemberId)
                    .FirstOrDefault();
                if (memberWinning == null)
                {
                    memberWinning = new MemberWinning();
                    Members member = db.Members.Find(mw.MemberId);
                    memberWinning.Member = new Members()
                    {
                        Firstname = member.Firstname,
                        Lastname = member.Lastname,
                        MemberId = member.MemberId,
                    };
                    memberWinning.Flight = new Flight()
                    {
                        FlightId = mw.FlightId,
                        Name = mw.Flight.Name,
                    };
                    memberWinning.EventId = mw.EventId;
                    memberWinning.MemberId = mw.MemberId;

                    memberWinnings.Add(memberWinning);
                }

                switch (mw.WinningTypeId)
                {
                    case 1: // Tournament
                        memberWinning.Tournament += mw.Amount;
                        break;
                    case 2:     // Side Games
                        memberWinning.SideGame += mw.Amount;
                        break;
                    default:
                        break;
                }
            }

            return Json(memberWinnings.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        // GET: Money/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberWinnings memberWinnings = db.MemberWinnings.Find(id);
            if (memberWinnings == null)
            {
                return HttpNotFound();
            }
            return View(memberWinnings);
        }

        // GET: Money/Create
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create(int year)
        {
            ViewBag.EventId = new SelectList(db.Event.Where(e => e.EventEnd.Year == year), "EventId", "Title");
            ViewBag.MemberId = new SelectList(db.Members, "MemberId", "Lastname");
            ViewBag.WinningTypeId = new SelectList(db.WinningTypes, "WinningTypeId", "WinningType");
            ViewBag.Year = year;
            return View();
        }

        // POST: Money/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create([Bind(Include = "MemberWinningId,DisplayName,WinningTypeId,Amount,EventId,FlightId")] MemberWinnings memberWinnings)
        {
            bool successful = false;
            try
            {
                List<Members> members = db.Members.ToList();
                memberWinnings.MemberId = members
                    .Where(m => m.DisplayName == memberWinnings.DisplayName)
                    .Select(m => m.MemberId)
                    .First();
                if (memberWinnings.WinningTypeId == 2) // Equals 'Side Games'
                {
                    memberWinnings.FlightId = 7;        // Side Games
                }
                if (ModelState.IsValid)
                {
                    db.MemberWinnings.Add(memberWinnings);
                    db.SaveChanges();
                    successful = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("----Error in Members.Create()---------------------------------------------------");
                Debug.WriteLine(ex.Message);
                ViewBag.Message = "Did you select a Member?";
                Debug.WriteLine("--------------------------------------------------------------------------------");
            }

            ViewBag.EventId = new SelectList(db.Event, "EventId", "Title", memberWinnings.EventId);
            ViewBag.MemberId = new SelectList(db.Members, "MemberId", "Lastname", memberWinnings.MemberId);
            ViewBag.WinningTypeId = new SelectList(db.WinningTypes, "WinningTypeId", "WinningType", memberWinnings.WinningTypeId);
            ViewBag.Year = db.Event.Find(memberWinnings.EventId).EventEnd.Year;
            ViewBag.Success = successful;

            if (successful)
            {
                ViewBag.Message = "Event Winning Created";
                ViewBag.Event = memberWinnings.Event.Title;
                ViewBag.Member = db.Members.Find(memberWinnings.MemberId).DisplayName;
                ViewBag.Type = db.WinningTypes.Find(memberWinnings.WinningTypeId).WinningType;
                ViewBag.Amount = memberWinnings.Amount;

                memberWinnings.MemberId = 0;
            }

            return View(memberWinnings);
        }

        // GET: Money/Delete/5
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberWinnings memberWinnings = db.MemberWinnings.Find(id);
            if (memberWinnings == null)
            {
                return HttpNotFound();
            }
            return View(memberWinnings);
        }

        // POST: Money/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            MemberWinnings memberWinnings = db.MemberWinnings.Find(id);
            db.MemberWinnings.Remove(memberWinnings);
            db.SaveChanges();
            return RedirectToAction("Manage");
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
