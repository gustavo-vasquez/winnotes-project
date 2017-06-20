using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business_Logic_Layer;
using static ProbandoTodo.Filters.CustomFilters;
using static ProbandoTodo.Models.FolderModels;
using Domain_Layer;

namespace ProbandoTodo.Controllers
{
    public class FolderController : Controller
    {
        static private FolderBLL folderBLL = new FolderBLL();

        private int GetSessionID(object user)
        {
            return ((UserLoginData)user).UserID;
        }

        // GET: Folder
        [OnlyUser]
        public ActionResult List()
        {
            return View(FillFoldersThumbnail());
        }

        public ActionResult FolderPartial()
        {
            return PartialView("_CreateFolder");
        }

        [HttpGet]
        public ActionResult ChangeFolderPartial(string folderID, string folderName, string noteID)
        {

            ChangeFolderModelView model = new ChangeFolderModelView();
            model.FolderID = Convert.ToInt32(folderID);
            model.NoteID = Convert.ToInt32(noteID);
            model.CurrentFolder = folderName;
            model.FoldersComboBox = folderBLL.GetFoldersOfUserBLL(Convert.ToInt32(((string[])Session["UserLoggedIn"])[0]));
            return PartialView("_ChangeFolder", model);
        }

        [HttpPost]
        public ActionResult ChangeFolder(string folderID, string noteID, string folderSelected)
        {
            string userID = ((string[])Session["UserLoggedIn"])[0];
            bool result = folderBLL.ChangeFolderBLL(noteID, userID, folderSelected);
            if (!result)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "No se ha procesado la operación.";            
                var responseError = new { statusCode = Response.StatusCode, statusText = Response.StatusDescription };
                return Json(responseError, JsonRequestBehavior.DenyGet);
            }

            string controller = (Request.UrlReferrer.AbsolutePath.Split('/'))[2];

            switch (controller)
            {
                case "Folder":
                    return PartialView("_NotesInFolder", new ClassifiedNotes(folderBLL.GetNotesInFolderBLL(userID, folderID)));                    
                case "Note":
                    return PartialView("~/Views/Note/_ListOfNotes.cshtml", new Models.NoteModels.ClassifiedQueryableNotes(new NoteBLL().GetDataForNoteList(userID)));
                default: return Json(Response, JsonRequestBehavior.DenyGet);
            }            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateFolderModelView model)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    int userID = GetSessionID(Session["UserLoggedIn"]);
                    folderBLL.CreateFolderBLL(userID, model.Name, model.Details);
                    //return PartialView("_FoldersThumbnail", FillFoldersThumbnail());
                    return Redirect(Request.UrlReferrer.AbsolutePath.ToString());
                }

                return PartialView("_CreateFolder", model);
            }
            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                return Redirect(Request.UrlReferrer.AbsolutePath.ToString());
            }
        }

        public IEnumerable<FolderListModelView> FillFoldersThumbnail()
        {
            List<FolderListModelView> model = new List<FolderListModelView>();
            var folders = folderBLL.GetAllFoldersBLL();

            foreach (var folder in folders)
            {
                model.Add(
                    new FolderListModelView()
                    {
                        FolderID = folder.FolderID,
                        Name = folder.Name,
                        Details = folder.Details,
                        LastModified = folder.LastModified
                    });
            }

            return model;
        }

        [OnlyUser]
        public ActionResult NotesList(int folderID)
        {
            try
            {
                string userID = ((string[])Session["UserLoggedIn"])[0];                
                ContentInFolderModelView model = new ContentInFolderModelView(folderBLL.GetFolderDataBLL(folderID),
                                                                                folderBLL.GetNotesInFolderBLL(userID, folderID.ToString()));
                return View(model);
            }
            catch
            {
                return RedirectToAction("List");
            }
        }        
    }
}