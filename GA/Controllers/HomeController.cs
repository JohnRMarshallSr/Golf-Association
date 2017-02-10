using GA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GA.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            ViewBag.Title = "Patty";
            //List<News> list = new List<News>();
            //News article = new News()
            //{
            //    Date = new DateTime(2016, 6, 23),
            //    Id = 1,
            //    Text = "This is the first article in a series of many",
            //    Title = "Article 1",
            //};
            //list.Add(article);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}