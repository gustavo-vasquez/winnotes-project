using Business_Logic_Layer;
using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ProbandoTodo.Filters.CustomFilters;
using static ProbandoTodo.Models.FolderModels;
using static ProbandoTodo.Models.NoteModels;

namespace ProbandoTodo.Controllers
{
    public class NoteController : Controller
    {
        static NoteBLL noteBLL = new NoteBLL();

        private int GetSessionID(object user)
        {
            return ((UserLoginData)user).UserID;
        }

        // GET: Note
        public ActionResult Index()
        {
            return View();
        }

        [OnlyUser]
        public ActionResult Create()
        {
            int userID = GetSessionID(Session["UserLoggedIn"]);
            CreateNoteModelView model = new CreateNoteModelView();
            this.PrepareModelToCreateNote(userID, ref model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OnlyUser]
        public ActionResult Create(CreateNoteModelView model)
        {
            try
            {                
                int userID = GetSessionID(Session["UserLoggedIn"]);

                if (!ModelState.IsValid)
                {
                    this.PrepareModelToCreateNote(userID, ref model);
                    return View(model);
                }

                int folderAuxID = 0;
                noteBLL.CreateNoteBLL(
                                    userID,
                                    model.Title,
                                    model.Details,
                                    model.ExpirationDate,
                                    model.Starred,
                                    model.FolderSelected,
                                    model.HourSelected,
                                    model.MinuteSelected,
                                    model.TimeTableSelected,
                                    ref folderAuxID
                                   );

                return RedirectToAction("NotesList", "Folder", new { folderID = folderAuxID });
            }
            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                this.PrepareModelToCreateNote(GetSessionID(Session["UserLoggedIn"]), ref model);
                return View(model);
            }
        }

        private void PrepareModelToCreateNote(int userID, ref CreateNoteModelView model)
        {            
            string[] userInfoBox = noteBLL.GetUserBoxInfoBLL(userID);
            model.FoldersBox = noteBLL.GetFoldersToSelectBLL(userID);
            model.HourBox = noteBLL.GenerateHourCombo();
            model.MinuteBox = noteBLL.GenerateMinuteCombo();
            model.TimeTableBox = noteBLL.GenerateTimeTableCombo();
            model.UserName = userInfoBox[0];
            model.AvatarSrc = userInfoBox[1];
            model.FoldersCount = Convert.ToInt32(userInfoBox[2]);
            model.NotesCount = Convert.ToInt32(userInfoBox[3]);            
        }

        [OnlyUser]
        public ActionResult List()
        {
            try
            {
                string userID = ((string[])Session["UserLoggedIn"])[0];
                return View(new ClassifiedQueryableNotes(noteBLL.GetDataForNoteList(userID)));
            }
            catch(Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [HttpPost]
        public ActionResult ForceCompleteTask(string folderID, string noteID, bool localized)
        {
            noteBLL.ForceCompleteTaskBLL(Convert.ToInt32(noteID));
            string userID = ((string[])Session["UserLoggedIn"])[0];            

            if(localized)
                return PartialView("~/Views/Folder/_NotesInFolder.cshtml", new ClassifiedNotes(new FolderBLL().GetNotesInFolderBLL(userID, folderID)));

            return PartialView("_ListOfNotes", new ClassifiedQueryableNotes(noteBLL.GetDataForNoteList(userID)));
        }

        [HttpPost]
        public ActionResult StarTask(string folderID, string noteID, bool localized)
        {
            noteBLL.StarTaskBLL(Convert.ToInt32(noteID));
            string userID = ((string[])Session["UserLoggedIn"])[0];

            if (localized)
                return PartialView("~/Views/Folder/_NotesInFolder.cshtml", new ClassifiedNotes(new FolderBLL().GetNotesInFolderBLL(userID, folderID)));

            return PartialView("_ListOfNotes", new ClassifiedQueryableNotes(noteBLL.GetDataForNoteList(userID)));
        }

        [HttpGet]
        public ActionResult ChangeDateTimeEventPartial(string currentDate, string previousDate, string idNote, string idFolder, bool localized)
        {            
            return PartialView("_ChangeDateTimeEvent", new ChangeDatetimeEventModel(currentDate, previousDate, idNote, idFolder, localized));
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDatetimeEvent(ChangeDatetimeEventModel model)
        {
            try
            {
                string userID = ((string[])Session["UserLoggedIn"])[0];
                noteBLL.ChangeDatetimeEventBLL(model.CurrentDate, model.HourSelected, model.MinuteSelected, model.TimeTableSelected, model.ID_Note, userID);
                if (model.Localized)
                    return PartialView("~/Views/Folder/_NotesInFolder.cshtml", new ClassifiedNotes(new FolderBLL().GetNotesInFolderBLL(userID, model.ID_Folder)));
                else
                    return PartialView("_ListOfNotes", new ClassifiedQueryableNotes(noteBLL.GetDataForNoteList(userID)));
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "No se ha procesado la operación. " + ex.Message;
                var responseError = new { statusCode = Response.StatusCode, statusText = Response.StatusDescription };
                return Json(responseError, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public ActionResult CheckExpiredEventsPartial(string[] uhick)
        {
            try
            {
                return PartialView("_FireAlarmDialog", noteBLL.CheckExpiredEventsBLL(uhick[0]));
            }
            catch (Exception ex)
            {
                //Response.StatusCode = 500;
                Response.StatusDescription = "No se pudo cargar el resultado. " + ex.Message;
                var responseError = new { statusCode = Response.StatusCode, statusText = Response.StatusDescription };
                return Json(responseError, JsonRequestBehavior.DenyGet);
            }
        }
    }
}