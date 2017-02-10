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
    public class RolesController : Controller
    {
        private GAEntities db = new GAEntities();

        // GET: AspNetRoles
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View(db.AspNetRoles.ToList());
        }

        /// <summary>
        /// Return all the Roles for the application
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult GetRoles([DataSourceRequest] DataSourceRequest request)
        {
            var roles = db.AspNetRoles.OrderBy(r => r.Name).ToList();

            List<AspNetRoles> list = new List<AspNetRoles>();

            foreach(var role in roles)
            {
                list.Add(new AspNetRoles()
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }

            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: AspNetRoles/Details/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRoles aspNetRoles = db.AspNetRoles.Find(id);
            if (aspNetRoles == null)
            {
                return HttpNotFound();
            }
            return View(aspNetRoles);
        }

        // GET: AspNetRoles/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create([Bind(Include = "Id,Name")] AspNetRoles aspNetRoles)
        {
            if (ModelState.IsValid)
            {
                aspNetRoles.Id = Guid.NewGuid().ToString();
                db.AspNetRoles.Add(aspNetRoles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetRoles);
        }

        // GET: AspNetRoles/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRoles aspNetRoles = db.AspNetRoles.Find(id);
            if (aspNetRoles == null)
            {
                return HttpNotFound();
            }
            return View(aspNetRoles);
        }

        // POST: AspNetRoles/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, AspNetRoles role)
        {
            if (role != null && ModelState.IsValid)
            {
                var entity = new AspNetRoles()
                {
                    Id = role.Id,
                    Name = role.Name
                };
                db.AspNetRoles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Post: AspNetRoles/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, AspNetRoles role)
        {
            if (role != null)
            {
                db.AspNetRoles.Attach(role);
                db.AspNetRoles.Remove(role);
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
