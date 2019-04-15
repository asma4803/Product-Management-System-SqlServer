using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(String login, String password)
        {
            var obj = PMS.BAL.UserBO.ValidateUser(login, password);
            if (obj != null)
            {
                Session["user"] = obj;
                if (obj.IsAdmin == true)
                    return Redirect("~/Home/Admin");
                else
                    return Redirect("~/Home/NormalUser");
            }

            ViewBag.MSG = "Invalid Login/Password";
            ViewBag.Login = login;

            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(UserDTO dto)
        {

            var uniqueName = "";
            if (Request.Files["Image"] != null )
            {
                var file = Request.Files["Image"];
                if (file.FileName != "")
                {
                    var ext = System.IO.Path.GetExtension(file.FileName);
                    uniqueName = Guid.NewGuid().ToString() + ext;
                    var rootPath = Server.MapPath("~/UserPics");
                    var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);
                    file.SaveAs(fileSavePath);

                    dto.PictureName = uniqueName;
                }
            }
            int i = PMS.BAL.UserBO.Save(dto);
            //User Save Logic

            if (dto.UserID == 0)
            {
                string r = Request["remember"];
                if (Request["remember"] == "on" && i > 0)
                {
                    var obj = PMS.BAL.UserBO.ValidateUser(dto.Login, dto.Password);
                    if (obj != null)
                    {
                        Session["user"] = obj;
                        return Redirect("~/Home/NormalUser");
                    }
                }
                return View("Login");
            }
            else
            {
                return Redirect("~/Home/NormalUser");
            }
        }

        public ActionResult EditProfile() {
            if (WebPrac.Security.SessionManager.IsValidUser)
            {
                if (WebPrac.Security.SessionManager.User.IsAdmin == true)
                {
                    TempData["msg"] = "Unauthorized Access";
                    return Redirect("~/Home/Admin");
                }
            }
            else {
                TempData["msg"] = "Need to login first";
                return Redirect("~/User/Login");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            SessionManager.ClearSession();
            return RedirectToAction("Login");
        }


        [HttpGet]
        public ActionResult Login2()
        {
            if (!WebPrac.Security.SessionManager.IsValidUser) {
                TempData["msg"] = "Wrong Access";
                return Redirect("~/User/Login");
            }
            return View();
        }

        [HttpPost]
        public JsonResult ValidateUser(String login, String password)
        {

            Object data = null;

            try
            {
                var url = "";
                var flag = false;

                var obj = PMS.BAL.UserBO.ValidateUser(login, password);
                if (obj != null)
                {
                    flag = true;
                    SessionManager.User = obj;

                    if (obj.IsAdmin == true)
                        url = Url.Content("~/Home/Admin");
                    else
                        url = Url.Content("~/Home/NormalUser");
                }

                data = new
                {
                    valid = flag,
                    urlToRedirect = url
                };
            }
            catch (Exception)
            {
                data = new
                {
                    valid = false,
                    urlToRedirect = ""
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdatePassword() {
            UserDTO user = new UserDTO();
            user.Password = Request["new_password"];
            user.UserID = Convert.ToInt32(Request["ID"]);
            int i = PMS.BAL.UserBO.UpdatePassword(user);
            if (i > 0) {
                TempData["pass_changed"] = "Password changed";
            }
            return View("Login");
        }
        public ActionResult SendEmail() {
            var to = Request["e_eamil"];
            int id = PMS.BAL.UserBO.findUserByEmail(to);
            if (id == 0)
            {
                Response.Write("<script>alert('No such Email exists');</script>");
                return View("Login");
            }
            Random rand = new Random();
            int num = rand.Next(2000, 10000);
            if (PMS.BAL.UserBO.SendEmail(to, "Password Reset", num.ToString()) == true)
            {
                TempData["code"] = num;
                Session["pass_id"] = id;
                return View("CodeScreen");
            }
            Response.Write("<script>alert('Some problem accured');</script>");
            return View("Login");
        }
        public ActionResult CodeScreen() {
            var code = Request["code"];
            if (code == TempData["code"].ToString())
            {
                return View("ChangePassword");
            }
            Response.Write("<script>alert('enter code first')</script>");
            return View("CodeScreen");
        }
        public ActionResult ChangePassword()
        {
            if (!WebPrac.Security.SessionManager.IsValidUser || WebPrac.Security.SessionManager.User.IsAdmin) {
                TempData["msg"] = "You are not Logged in";
                return Redirect("~/User/Login");
            }
            return View();
        }


        //Validate Change Password
        [HttpPost]
        [ActionName("ChangePassword")]
        public ActionResult ChangePassword2()
        {
            var new_pass = Request["NewPassword"];
            int id = Convert.ToInt32(Session["pass_id"]);
            UserDTO user = new UserDTO();
            user.UserID = id;
            user.Password = new_pass;
            if (PMS.BAL.UserBO.UpdatePassword(user)>0)
            {
                Response.Write("<script>alert('Password updated successfully');</script>");
                return View("Login");
            }
            Response.Write("<script>alert('Some problem occured');</script>");
            return View();
        }
        public ActionResult getUser() {
            PMS.Entities.UserDTO user = PMS.BAL.UserBO.GetUserById(Convert.ToInt32(Request["cid"]));

            return Json(user, JsonRequestBehavior.AllowGet);
        }
       

    }
}