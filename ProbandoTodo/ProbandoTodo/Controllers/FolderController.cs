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
    [WithAccount]
    public class FolderController : Controller
    {
        static private FolderBLL folderBLL = new FolderBLL();

        private int GetSessionID(object user)
        {
            return ((UserLoginData)user).UserID;
        }

        // GET: Folder        
        public ActionResult List()
        {
            return View(FillFoldersThumbnail());
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

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView("_CreateFolder");
        }

        [HttpGet]
        public ActionResult ChangeFolderPartial(int folderID, string folderName, int noteID)
        {
            try
            {                
                ChangeFolderModelView model = new ChangeFolderModelView();
                model.FolderID = folderID;
                model.NoteID = noteID;
                model.CurrentFolder = folderName;
                model.FoldersComboBox = folderBLL.GetFoldersOfUserBLL(GetSessionID(Session["UserLoggedIn"]));
                return PartialView("_ChangeFolder", model);
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ChangeFolder(ChangeFolderModelView model)
        {
            try
            {
                int userID = GetSessionID(Session["UserLoggedIn"]);
                folderBLL.ChangeFolderBLL(model.NoteID, userID, model.FolderSelected);
                string controller = (Request.UrlReferrer.AbsolutePath.Split('/'))[2];

                switch (controller)
                {
                    case "Folder":
                        return PartialView("_NotesInFolder", new ClassifiedNotes(folderBLL.GetNotesInFolderBLL(userID, model.FolderID)));
                    case "Note":
                        return PartialView("~/Views/Note/_ListOfNotes.cshtml", new Models.NoteModels.ClassifiedQueryableNotes(new NoteBLL().GetDataForNoteList(userID)));
                    default: throw new HttpException("Error desconocido. Vuelva a intentarlo.");
                }
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message }, JsonRequestBehavior.DenyGet);
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

        [HttpGet]
        public ActionResult Edit(string folderID, string name, string details)
        {            
            return PartialView("_EditFolder", new EditFolderModelView() { FolderID = Convert.ToInt32(folderID), Name = name, Details = details });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditFolderModelView model)
        {
            if(!ModelState.IsValid)            
                return PartialView("_EditFolder", model);            

            try
            {
                folderBLL.EditFolderBLL(GetSessionID(Session["UserLoggedIn"]), model.FolderID, model.Name, model.Details);
                return Redirect(Request.UrlReferrer.AbsolutePath.ToString());
            }
            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                return Redirect(Request.UrlReferrer.AbsolutePath.ToString());
            }
        }        
        
        public ActionResult NotesList(int folderID)
        {
            try
            {                
                int userID = GetSessionID(Session["UserLoggedIn"]);
                ContentInFolderModelView model = new ContentInFolderModelView(folderBLL.GetFolderDataBLL(folderID),
                                                                                folderBLL.GetNotesInFolderBLL(userID, folderID));
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("List");
            }
        }        
    }
}