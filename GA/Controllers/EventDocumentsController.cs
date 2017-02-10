using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GA.Data;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using GA.Models;

namespace GA.Controllers
{
    public class EventDocumentsController : Controller
    {
        private GAEntities db = new GAEntities();

        // GET: EventDocuments
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Return all the event that have not yet started.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEventDocuments([DataSourceRequest] DataSourceRequest request, int eventId)
        {
            var eventDocuments = db.EventDocuments
                .Where(ed => ed.EventId == eventId )
                .Select(ed => new
                {
                    ed.EventDocumentId,
                    ed.DocumentId,
                    ed.Documents.Title,
                    ed.Documents.Filename,
                });
            List<EventDocumentView> documents = new List<EventDocumentView>();
            foreach(var doc in eventDocuments)
            {
                documents.Add(new EventDocumentView()
                {
                    EventDocumentId = doc.EventDocumentId,
                    DocumentId = doc.DocumentId,          
                    Filename = doc.Filename,
                    Title = doc.Title,
                });
            }
            return Json(documents.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: EventDocuments/Create
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult Create(int year)
        {
            ViewBag.Year = year.ToString();

            return View();
        }

        // POST: EventDocuments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Create([Bind(Include = "EventDocumentId,EventId,DocumentId")] EventDocuments eventDocuments)
        {
            if (ModelState.IsValid)
            {
                db.EventDocuments.Add(eventDocuments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventId = new SelectList(db.Event, "EventId", "Title", eventDocuments.EventId);
            ViewBag.DocumentId = new SelectList(db.Documents, "DocumentId", "Title", eventDocuments.DocumentId);
            return View(eventDocuments);
        }


        // GET: EventDocuments/Delete/5
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDocuments eventDocuments = db.EventDocuments.Find(id);
            if (eventDocuments == null)
            {
                return HttpNotFound();
            }
            return View(eventDocuments);
        }

        // POST: EventDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            EventDocuments eventDocuments = db.EventDocuments.Find(id);
            db.EventDocuments.Remove(eventDocuments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public FileResult Download(string Id)
        {
            int documentId = Convert.ToInt32(Id);
            Documents doc = db.Documents.Find(documentId);

            return File(doc.DocumentContent, doc.Type, doc.Filename);
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
