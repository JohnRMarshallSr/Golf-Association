using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GA.Data;

namespace GA.Controllers
{
    public class AssociationsController : Controller
    {
        private GAEntities db = new GAEntities();

        public ActionResult Get()
        {
            Association association = db.Association.First();

            return Json(association, JsonRequestBehavior.AllowGet);
        }

        // GET: Rules/Details/5
        public ActionResult Mission()
        {
            Association association = db.Association.First();
            return View(association);
        }

        // GET: Associations/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Association association = db.Association.Find(id);
            if (association == null)
            {
                return HttpNotFound();
            }
            return View(association);
        }

        // POST: Associations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "AssociationId,Name,SmallImage,LargeImage,StartYear,Mission")] Association association)
        {
            if (ModelState.IsValid)
            {
                association.Mission = Server.HtmlDecode(association.Mission);
                db.Entry(association).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(association);
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
