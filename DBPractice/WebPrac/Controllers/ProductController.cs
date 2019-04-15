using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class ProductController : Controller
    {
        private ActionResult GetUrlToRedirect()
        {
            if (!SessionManager.IsValidUser)
            {
                TempData["Message"] = "Unauthorized Access";
                return Redirect("~/User/Login");
            }
            return null;
        }
        public ActionResult ShowAll()
        {
            if (SessionManager.IsValidUser == false)
            {
                return Redirect("~/User/Login");
            }

            var products = PMS.BAL.ProductBO.GetAllProducts(true);

            return View(products);
        }

        public ActionResult New()
        {
            var redVal = GetUrlToRedirect();
            if (redVal == null)
            {
                var dto = new ProductDTO();
                redVal =  View(dto);
            }

            return redVal;
        }

        public ActionResult Edit(int id)
        {

            var redVal = GetUrlToRedirect();
            if (redVal == null)
            {
                var prod = PMS.BAL.ProductBO.GetProductById(id);
                redVal= View("New", prod);
            }

            return redVal;
            
        }
        public ActionResult Edit2(int ProductID)
        {
            var prod = PMS.BAL.ProductBO.GetProductById(ProductID);
            return View("New", prod);
        }
        public ActionResult Delete()
        {
            if (!SessionManager.IsValidUser)
            {
               TempData["Message"] = "Unauthorized Access";
                return Redirect("~/User/Login");
            }
            int id = Convert.ToInt32(Request["id"]);
            int res = PMS.BAL.ProductBO.DeleteProduct(id);
            if ( res> 0) {
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(ProductDTO dto)
        {

            if (!SessionManager.IsValidUser)
            {
                    TempData["Message"] = "Unauthorized Access";
                    return Redirect("~/User/Login");
            }


            var uniqueName = "";

            if (Request.Files["Image"] != null)
            {
                var file = Request.Files["Image"];
                if (file.FileName != "")
                {
                    var ext = System.IO.Path.GetExtension(file.FileName);

                    //Generate a unique name using Guid
                    uniqueName = Guid.NewGuid().ToString() + ext;

                    //Get physical path of our folder where we want to save images
                    var rootPath = Server.MapPath("~/UploadedFiles");

                    var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);

                    // Save the uploaded file to "UploadedFiles" folder
                    file.SaveAs(fileSavePath);

                    dto.PictureName = uniqueName;
                }
            }



            if (dto.ProductID > 0)
            {
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = WebPrac.Security.SessionManager.User.UserID;
                PMS.BAL.ProductBO.Save(dto);
                TempData["Msg"] = "Record is Updated!";

                return RedirectToAction("ShowAll");
            }
            else
            {
                
                dto.CreatedOn = DateTime.Now;
                dto.CreatedBy = WebPrac.Security.SessionManager.User.UserID;
                PMS.BAL.ProductBO.Save(dto);

                TempData["Msg"] = "Record is saved!";

                return RedirectToAction("ShowAll");
            }

            
        }
        public ActionResult showProfile()
        {


            PMS.Entities.UserDTO user = (PMS.Entities.UserDTO) Session["userProf"];
            return View(user);
        }
    }
}