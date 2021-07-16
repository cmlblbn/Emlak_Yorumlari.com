using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminUserController : Controller
    {
        // GET: AdminUser
        private MyContext db = new MyContext();
        public ActionResult AdminuserControl()
        {

            AdminUserControlViewModel model = new AdminUserControlViewModel();
            model.ClassList = new List<AdminUserControlViewModel>();

            var dataList = db.Users.ToList();
            MyContext secondCursor = new MyContext();
            foreach (var data in dataList)
            {
                AdminUserControlViewModel elementModel = new AdminUserControlViewModel();
                elementModel.user = data;
                elementModel.commentsCount = secondCursor.Comments.Where(x => x.user_id == data.user_id).Count();
                elementModel.placesCount = secondCursor.Places.Where(x => x.user_id == data.user_id).Count();
                model.ClassList.Add(elementModel);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminuserControl(AdminUserControlViewModel model)
        {
            var dataList = new List<User>();
            if (model.SearchText != null)
            {
                if (model.isActivate)
                {
                    var query = from p in db.Users.ToList()
                        where p.username.Contains(model.SearchText) && p.IsActive
                        select p;
                    dataList = query.ToList();
                }
                else
                {
                    var query = from p in db.Users.ToList()
                        where p.username.Contains(model.SearchText)
                        select p;
                    dataList = query.ToList();
                }

            }
            else
            {
                if (model.isActivate)
                {
                    dataList = db.Users.Where(x => x.IsActive).ToList();
                }
                else
                {
                    dataList = db.Users.ToList();
                }
            }
            model.ClassList = new List<AdminUserControlViewModel>();
            MyContext secondCursor = new MyContext();
            foreach (var data in dataList)
            {
                AdminUserControlViewModel elementModel = new AdminUserControlViewModel();
                elementModel.user = data;
                elementModel.commentsCount = secondCursor.Comments.Where(x => x.user_id == data.user_id).Count();
                elementModel.placesCount = secondCursor.Places.Where(x => x.user_id == data.user_id).Count();
                model.ClassList.Add(elementModel);
            }

            return View(model);
        }


        public ActionResult UserActivate(int? id)
        {
            if (id != null)
            {
                User user = new User();
                user = db.Users.Where(x => x.user_id == id).FirstOrDefault();
                if (user != null)
                {
                    user.IsActive = true;
                    db.SaveChanges();
                }

            }
            return RedirectToAction("AdminuserControl");
        }

        public ActionResult UserDeactivate(int? id)
        {
            if (id != null)
            {
                User user = new User();
                user = db.Users.Where(x => x.user_id == id).FirstOrDefault();
                if (user != null)
                {
                    user.IsActive = false;
                    db.SaveChanges();
                }

            }
            return RedirectToAction("AdminuserControl");
        }
    }
}