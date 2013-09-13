using RemoteUrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteUrlClient.Controllers
{
    public class rootController : Controller
    {
        //
        // GET: /root/

        public ActionResult Index(string url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                var remoteUrl = new WebPage { Url = url };
                remoteUrl.GetSource();
                remoteUrl.UpdateSource();

                return View(remoteUrl);
            }
            else
            {
                return View(new WebPage());
            }
        }

        public JsonResult Preview(string url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                var remoteUrl = new WebPage { Url = url };
                remoteUrl.GetSource();
                remoteUrl.UpdateSource();

                return Json(remoteUrl);
            }
            else
            {
                return Json(new WebPage());
            }
        }

    }
}
