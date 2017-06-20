using Data_Access_Layer;
using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Business_Logic_Layer
{
    public class NoteBLL
    {
        static NoteDAL noteDAL = new NoteDAL();

        public IEnumerable<SelectListItem> GetFoldersToSelectBLL(int userID)
        {
            return noteDAL.GetFoldersToSelectDAL(userID);
        }

        public string[] GetUserBoxInfoBLL(int userID)
        {
            return noteDAL.GetUserBoxInfoDAL(userID);
        }

        public void CreateNoteBLL(int userID, string title, string details, DateTime expirationDate, bool starred, string folderSelected, string hourSelected, string minuteSelected, string timeTableSelected, ref int folderAuxID)
        {            
            noteDAL.CreateNoteDAL(userID, title, details, expirationDate, starred, folderSelected, Convert.ToInt32(hourSelected), Convert.ToInt32(minuteSelected), timeTableSelected, ref folderAuxID);
        }

        public void ForceCompleteTaskBLL(int noteID)
        {
            noteDAL.ForceCompleteTaskDAL(noteID);
        }

        public void StarTaskBLL(int noteID)
        {
            noteDAL.StarTaskDAL(noteID);
        }

        public void ChangeDatetimeEventBLL(string currentDate, string hour, string minute, string timeTable, string id_note, string userID)
        {
            noteDAL.ChangeDatetimeEventDAL(currentDate, Convert.ToInt32(hour), Convert.ToInt32(minute), timeTable, Convert.ToInt32(id_note), Convert.ToInt32(userID));
        }

        public IQueryable<NoteInformationQueryable> GetDataForNoteList(string userID)
        {
            return noteDAL.GetDataForNoteList(Convert.ToInt32(userID));
        }

        public IEnumerable<SelectListItem> GenerateHourCombo()
        {
            List<SelectListItem> hoursBox = new List<SelectListItem>();            

            for(var i = 0; i <= 12; i++)
            {
                hoursBox.Add(new SelectListItem() { Value = i.ToString("D2"), Text = i.ToString("D2") + " hs." });
            }

            return hoursBox;
        }

        public IEnumerable<SelectListItem> GenerateMinuteCombo()
        {
            List<SelectListItem> minutesBox = new List<SelectListItem>();            

            for (var i = 0; i <= 60; i++)
            {
                minutesBox.Add(new SelectListItem() { Value = i.ToString("D2"), Text = i.ToString("D2") + " min." });
            }

            return minutesBox;
        }

        public IEnumerable<SelectListItem> GenerateTimeTableCombo()
        {
            List<SelectListItem> timeTableBox = new List<SelectListItem>();            
            timeTableBox.Add(new SelectListItem() { Value = "AM", Text = "a.m." });
            timeTableBox.Add(new SelectListItem() { Value = "PM", Text = "p.m." });

            return timeTableBox;
        }

        public List<Note> CheckExpiredEventsBLL(string encryptedUser)
        {
            return noteDAL.CheckExpiredEventsDAL(encryptedUser);
        }
    }
}
