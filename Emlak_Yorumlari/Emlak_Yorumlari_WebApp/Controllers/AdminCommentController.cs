using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminCommentController : Controller
    {
        // GET: AdminComment
        public ActionResult ShowComment()
        {
            return View();
        }
    }
}