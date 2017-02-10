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
using System.IO;

namespace GA.Controllers
{
    public class DocumentsController : Controller
    {
        private GAEntities db = new GAEntities();

        // GET: Documents
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult Index()
        {
            return View(db.Documents.ToList());
        }

        /// <summary>
        /// Return all the documents
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult GetDocuments([DataSourceRequest] DataSourceRequest request)
        {
            var docs = db.Documents.Select(d => new { d.DocumentId, d.Title, d.Filename, d.Type, d.Extension }).ToList();
            List<Documents> documents = new List<Documents>();
            foreach(var doc in docs)
            {
                documents.Add(new Documents()
                {
                    DocumentId = doc.DocumentId,
                    Title = doc.Title,
                    Filename = doc.Filename,
                    Type = doc.Type,
                    Extension = doc.Extension
                });
            }

            return Json(documents.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetList()
        {
            var docs = db.Documents
                .Select(d => new { d.DocumentId, d.Title, d.Filename, d.Type, d.Extension })
                .OrderByDescending(d => d.Filename)
                .ToList();
            List<Documents> documents = new List<Documents>();
            foreach (var doc in docs)
            {
                documents.Add(new Documents()
                {
                    DocumentId = doc.DocumentId,
                    Title = doc.Title,
                    Filename = doc.Filename,
                    Type = doc.Type,
                    Extension = doc.Extension
                });
            }

            return Json(documents, JsonRequestBehavior.AllowGet);
        }
        // GET: Documents/Create
        [Authorize(Roles = "Administrator, Document Manager, Pro Shop")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult Create([Bind(Include = "DocumentContent,Title")] Documents documents, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                foreach (var file in files)
                {
                    byte[] data;
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        data = memoryStream.ToArray();
                    }
                    documents.Type = file.ContentType;
                    documents.DocumentContent = data;
                    documents.Filename = file.FileName;
                    int index = file.FileName.LastIndexOf('.');
                    if(index > 0)
                    {
                        documents.Extension = file.FileName.Substring(index + 1);
                    }
                    db.Documents.Add(documents);
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(documents);
        }

        // GET: Documents/Edit/5
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documents documents = db.Documents.Find(id);
            if (documents == null)
            {
                return HttpNotFound();
            }
            return View(documents);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, Documents document)
        {
            byte[] content = db.Documents
                .Where(d => d.DocumentId == document.DocumentId)
                .Select(d => d.DocumentContent)
                .SingleOrDefault();
            string error = string.Empty;
            if (document != null)
            {
                if(document.Title == null)
                {
                    ModelState.AddModelError("Error", "Error");
                    error = "Title is required";
                }

                if(document.Filename == null)
                {
                    ModelState.AddModelError("Error", "Error");
                    error = "Filename is required";
                }

                document.DocumentContent = content;
            }

            if(ModelState.IsValid)
            {
                db.Documents.Attach(document);
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                return Json(new DataSourceResult
                {
                    Errors = error
                });
            }
            return RedirectToAction("Index");
        }

        // GET: Documents/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrator, Tournament Manager, Pro Shop")]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, Documents document)
        {
            if (document != null)
            {
                int count = db.EventDocuments.Where(ed => ed.DocumentId == document.DocumentId).Count();
                if (count > 0)
                {
                    return Json(new DataSourceResult
                    {
                        Errors = string.Format("The document [{0}] is associated with a tournament and can not be deleted.",
                        document.Title)
                    });                    
                }
                
                if(ModelState.IsValid)
                {                    
                    db.Documents.Attach(document);
                    db.Documents.Remove(document);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Documents");
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
