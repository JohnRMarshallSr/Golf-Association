using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Kendo.Mvc.UI;
using GA.Models;
using Kendo.Mvc.Extensions;
using System.Drawing;
using System.Diagnostics;

namespace GA.Controllers
{
    public class ImageController : Controller
    {
        private const string _imagePath = "/Content/images";

        // GET: Image
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            string sourceDirectory = Server.MapPath(_imagePath);
            if(sourceDirectory.StartsWith("C:"))
            {
                sourceDirectory = Server.MapPath("/GA" + _imagePath);
            }
            IEnumerable<string> files = Directory.EnumerateFiles(sourceDirectory, "*");
            foreach (string file in files)
            {
                int index = file.LastIndexOf("\\");
                string name = file.Substring(index + 1);
                list.Add(new SelectListItem()
                {
                    Text = name,
                    Value = name,
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult GetImages([DataSourceRequest] DataSourceRequest request)
        {
            List<ImageFile> list = new List<ImageFile>();
            string sourceDirectory = Server.MapPath(_imagePath);

            IEnumerable<string> files = Directory.EnumerateFiles(sourceDirectory, "*");
            foreach (string file in files)
            {
                Image img = null;
                try
                {
                    img = new Bitmap(file);
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                int index = file.LastIndexOf("\\");
                string name = file.Substring(index + 1);
                FileInfo info = new FileInfo(file);
                list.Add(new ImageFile()
                {
                    Filename = name,
                    Size = info.Length,
                    Height = img != null ? img.Height : 0,
                    Width = img != null ? img.Width : 0,
                });
            }

            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: Image/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Image/Create
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                foreach(HttpPostedFileBase file in files)
                {
                    string fullPath = string.Format("{0}/{1}", _imagePath, file.FileName);
                    file.SaveAs(Request.MapPath(fullPath));
                    file.InputStream.Close();
                    file.InputStream.Dispose();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Image/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Image/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Image/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string filename)
        {
            ImageFile file = new ImageFile()
            {
                Filename = filename,
            };

            return View(file);
        }

        // POST: Image/Delete/5
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string filename, FormCollection collection)
        {
            try
            {
                var fullPath = string.Format("{0}/{1}", _imagePath, filename);
                var path = Server.MapPath(fullPath);
                if(System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ImageFile file = new ImageFile()
                {
                    Filename = filename,
                };
                ViewBag.Error = e.Message;

                return View(file);
            }
        }
    }
}
