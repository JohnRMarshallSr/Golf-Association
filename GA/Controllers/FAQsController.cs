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
    public class FAQsController : Controller
    {
        private GAEntities db = new GAEntities();

        // GET: FAQs
        public ActionResult Index()
        {
            return View(db.FAQ.ToList());
        }

        public ActionResult Get([DataSourceRequest] DataSourceRequest request)
        {
            var faqs = db.FAQ.OrderBy(f => f.FAQDate).ToList();
            return Json(faqs.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: FAQs/Create
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: FAQs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create([Bind(Include = "FAQId,Active,FAQDate,Question,Answer")] FAQ fAQ)
        {
            if (ModelState.IsValid)
            {
                db.FAQ.Add(fAQ);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fAQ);
        }

        // GET: FAQs/Edit/5
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FAQ fAQ = db.FAQ.Find(id);
            if (fAQ == null)
            {
                return HttpNotFound();
            }
            return View(fAQ);
        }

        // POST: FAQs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Edit([Bind(Include = "FAQId,Active,FAQDate,Question,Answer")] FAQ fAQ)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fAQ).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fAQ);
        }

        // GET: FAQs/Delete/5
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FAQ fAQ = db.FAQ.Find(id);
            if (fAQ == null)
            {
                return HttpNotFound();
            }
            return View(fAQ);
        }

        // POST: FAQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            FAQ fAQ = db.FAQ.Find(id);
            db.FAQ.Remove(fAQ);
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
