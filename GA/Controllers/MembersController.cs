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
    public class MembersController : Controller
    {
        private GAEntities db = new GAEntities();

        // GET: Members
        public ActionResult Index()
        {
            return View(db.Members.ToList());
        }

        public ActionResult Get([DataSourceRequest] DataSourceRequest request)
        {
            var members = db.Members
                .ToList();
            List<Members> list = new List<Members>();
            foreach (Members user in members)
            {
                list.Add(new Members()
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    MemberId = user.MemberId,
                });
            }
            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetList()
        {
            var members = db.Members.OrderBy(m => m.Lastname).ToList();
            List<Members> list = new List<Members>();
            foreach (Members user in members)
            {
                list.Add(new Members()
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    MemberId = user.MemberId,
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Home(string username)
        {
            string url = "/Home";

            AspNetUsers user = db.AspNetUsers
                .Where(u => u.UserName == username)
                .FirstOrDefault();
            if(user != null && user.MemberId != null)
            {
                url = string.Format("/{0}/{1}/{2}", "Members", "View", user.MemberId);
            }

            return Redirect(url);
        }
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Members member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }

            return View(member);
        }

        // GET: Members/Details/5
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Members members = db.Members.Find(id);
            if (members == null)
            {
                return HttpNotFound();
            }
            return View(members);
        }

        // GET: Members/Create
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create([Bind(Include = "MemberId,Lastname,Firstname,UserId")] Members members)
        {
            if (ModelState.IsValid)
            {
                db.Members.Add(members);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(members);
        }

        // GET: Members/Edit/5
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Members members = db.Members.Find(id);
            if (members == null)
            {
                return HttpNotFound();
            }
            return View(members);
        }

        // POST: Members/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, Members members)
        {
            if (ModelState.IsValid)
            {
                db.Entry(members).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(members);
        }

        // GET: Members/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, Members member)
        {
            if (member != null)
            {
                db.Members.Attach(member);
                db.Members.Remove(member);
                db.SaveChanges();
            }
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
