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
    public class RulesController : Controller
    {
        private GAEntities db = new GAEntities();

        // GET: Rules
        public ActionResult Index()
        {
            return View(db.Rules.ToList());
        }

        public ActionResult GetRules([DataSourceRequest] DataSourceRequest request)
        {
            var links = db.Rules
                .ToList();
            return Json(links.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: Rules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rules rules = db.Rules.Find(id);
            if (rules == null)
            {
                return HttpNotFound();
            }
            return View(rules);
        }

        // GET: Rules/Create
        [Authorize(Roles = "Administrator, Pro Shop")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Pro Shop")]
        public ActionResult Create([Bind(Include = "RuleId,Active,RuleDate,Title,Description")] Rules rules)
        {
            if (ModelState.IsValid)
            {
                rules.Description = Server.HtmlDecode(rules.Description);
                db.Rules.Add(rules);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rules);
        }

        // GET: Rules/Edit/5
        [Authorize(Roles = "Administrator, Pro Shop")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rules rules = db.Rules.Find(id);
            if (rules == null)
            {
                return HttpNotFound();
            }
            return View(rules);
        }

        // POST: Rules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Pro Shop")]
        public ActionResult Edit([Bind(Include = "RuleId,Active,RuleDate,Title,Description")] Rules rules)
        {
            if (ModelState.IsValid)
            {
                rules.Description = Server.HtmlDecode(rules.Description);
                db.Entry(rules).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rules);
        }

        // GET: Rules/Delete/5
        [Authorize(Roles = "Administrator, Pro Shop")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rules rules = db.Rules.Find(id);
            if (rules == null)
            {
                return HttpNotFound();
            }
            return View(rules);
        }

        // POST: Rules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Pro Shop")]
        public ActionResult DeleteConfirmed(int id)
        {
            Rules rules = db.Rules.Find(id);
            db.Rules.Remove(rules);
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
