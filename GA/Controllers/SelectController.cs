using GA.Data;
using GA.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GA.Controllers
{
    public class SelectController : Controller
    {
        private GAEntities db = new GAEntities();
        private int _startYear = 2008;
        // GET: Year
        public ActionResult GetYears()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            int startYear = _startYear;
            startYear = db.Association
                .Select(a => a.StartYear)
                .FirstOrDefault();
            for(int i = startYear; i <= DateTime.Now.Year; i++)
            {
                list.Add(new SelectListItem()
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCurrentYear([DataSourceRequest] DataSourceRequest request)
        {
            string currentYear = string.Format("{0}", DateTime.Now.Year);
            return Json(new { year = currentYear });
        }

        public ActionResult GetWinningTypes()
        {
            List<WinningTypes> list = new List<WinningTypes>();
            var types = db.WinningTypes;
            foreach(var type in types)
            {
                list.Add(new WinningTypes()
                {
                    WinningTypeId = type.WinningTypeId,
                    WinningType = type.WinningType,
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}