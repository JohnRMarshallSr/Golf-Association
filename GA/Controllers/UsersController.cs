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
using GA.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace GA.Controllers
{
    public class UsersController : Controller
    {
        private GAEntities db = new GAEntities();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Users
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Roles()
        {
            return View();
        }
        public ActionResult GetUsers([DataSourceRequest] DataSourceRequest request)
        {
            var users = db.AspNetUsers
            .OrderBy(u => u.UserName)
            .ToList();

            List<AspNetUsers> list = new List<AspNetUsers>();
            foreach(AspNetUsers u in users)
            {
                list.Add(new AspNetUsers
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                });
            }

            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Administrator")]
        public ActionResult GetUserList()
        {
            var users = db.AspNetUsers
                .OrderBy(u => u.UserName)
                .ToList();
            List<AspNetUsers> list = new List<AspNetUsers>();
            foreach(var user in users)
            {
                list.Add(new AspNetUsers()
                {
                    Id = user.Id,
                    UserName = user.UserName
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // GET: User/UserProfile
        public ActionResult UserProfile()
        {
            
            AspNetUsers aspNetUsers = db.AspNetUsers
                .Where(u => u.UserName == User.Identity.Name)
                .SingleOrDefault();

            return View(aspNetUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(AspNetUsers userProfile)
        {
            if (ModelState.IsValid)
            {
                AspNetUsers user = db.AspNetUsers.Find(userProfile.Id);
                AspNetUsers profileUser = new AspNetUsers()
                {
                    AccessFailedCount = user.AccessFailedCount,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEndDateUtc = user.LockoutEndDateUtc,
                    PasswordHash = user.PasswordHash,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    SecurityStamp = user.SecurityStamp,
                    TwoFactorEnabled = user.TwoFactorEnabled,

                    Email = userProfile.Email,
                    Id = userProfile.Id,
                    PhoneNumber = userProfile.PhoneNumber,
                    UserName = userProfile.UserName,
                    MemberId = userProfile.MemberId,
                };

                db.Entry(user).State = EntityState.Detached;
                db.Entry(profileUser).State = EntityState.Modified;

                db.SaveChanges();

                if(userProfile.MemberId > 0)
                {
                    return RedirectToAction("View", "Members", new { id = userProfile.MemberId });
                }
                return RedirectToAction("Index", "Home");
            }

            return View(userProfile);
        }

        // GET: User/Roles
        [Authorize(Roles = "Administrator")]
        public ActionResult GetUserRoles(string Id)
        {
            List<UserRoles> list = new List<UserRoles>();
            var roles = db.AspNetRoles.OrderBy(r => r.Name).ToList();

            foreach(AspNetRoles role in roles)
            {
                list.Add(new UserRoles()
                {
                    Checked = false,
                    Role = role.Name,
                    RoleId = role.Id,
                });
            }

            // Get the list of Roles the User currently has
            var user = db.AspNetUsers
                .Include("AspNetUserRoles")
                .Where(u => u.Id == Id)
                .FirstOrDefault();
            foreach(AspNetUserRoles userRole in user.AspNetUserRoles)
            {
                list.Find(u => u.RoleId == userRole.RoleId).Checked = true;
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        // GET: Users/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "Id,Email,PhoneNumber,UserName")] AspNetUsers aspNetUsers)
        {
            if (ModelState.IsValid)
            {
                AspNetUsers user = db.AspNetUsers.Find(aspNetUsers.Id);

                aspNetUsers.AccessFailedCount = user.AccessFailedCount;
                aspNetUsers.EmailConfirmed = user.EmailConfirmed;
                aspNetUsers.LockoutEnabled = user.LockoutEnabled;
                aspNetUsers.LockoutEndDateUtc = user.LockoutEndDateUtc;
                aspNetUsers.PasswordHash = user.PasswordHash;
                aspNetUsers.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                aspNetUsers.SecurityStamp = user.SecurityStamp;
                aspNetUsers.TwoFactorEnabled = user.TwoFactorEnabled;

                db.Entry(user).State = EntityState.Detached;
                db.Entry(aspNetUsers).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(aspNetUsers);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult EditRole(bool isChecked, string userId, string roleId)
        {
            bool successful = false;          
            if(isChecked)
            {
                // Adding Role for User
                AspNetUserRoles newRole = new AspNetUserRoles()
                {
                    RoleId = roleId,
                    UserId = userId,
                    UserRoleId = Guid.NewGuid().ToString(),
                };

                db.AspNetUserRoles.Add(newRole);
                db.SaveChanges();
                successful = true;
            }
            else
            {
                // Remove Role from User
                AspNetUserRoles userRole = db.AspNetUserRoles
                    .Where(ur => ur.UserId == userId && ur.RoleId == roleId)
                    .FirstOrDefault();
                if(userRole != null)
                {
                    db.AspNetUserRoles.Attach(userRole);
                    db.AspNetUserRoles.Remove(userRole);
                    db.SaveChanges();
                    successful = true;
                }
            }

            return Json(new { success = successful }, JsonRequestBehavior.AllowGet);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUsers);
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
