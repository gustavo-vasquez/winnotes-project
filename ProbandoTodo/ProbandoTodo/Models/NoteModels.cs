using Business_Logic_Layer;
using Domain_Layer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProbandoTodo.Models
{
    public class NoteModels
    {
        public class CreateNoteModelView
        {
            public string UserName { get; set; }
            public string PersonalPhrase { get; set; }
            public string AvatarSrc { get; set; }
            public int FoldersCount { get; set; }
            public int NotesCount { get; set; }

            [Required]
            [StringLength(70)]
            public string Title { get; set; }

            [Required]
            [StringLength(100)]
            public string Details { get; set; }
                                                
            public DateTime ExpirationDate { get; set; }

            public bool Starred { get; set; }

            [Required]
            public string FolderSelected { get; set; }
            public IEnumerable<SelectListItem> FoldersBox { get; set; }            

            [Required]
            public string HourSelected { get; set; }
            public IEnumerable<SelectListItem> HourBox { get; set; }

            [Required]
            public string MinuteSelected { get; set; }
            public IEnumerable<SelectListItem> MinuteBox { get; set; }

            [Required]
            public string TimeTableSelected { get; set; }
            public IEnumerable<SelectListItem> TimeTableBox { get; set; }
        }

        public class ClassifiedQueryableNotes
        {
            public List<NoteInformationQueryable> PendingNotes { get; set; }
            public List<NoteInformationQueryable> CompleteNotes { get; set; }

            public ClassifiedQueryableNotes(IQueryable<NoteInformationQueryable> notes)
            {
                this.PendingNotes = new List<NoteInformationQueryable>();
                this.CompleteNotes = new List<NoteInformationQueryable>();

                foreach (var note in notes)
                {
                    if (!Convert.ToBoolean(note.Completed))
                    {
                        this.PendingNotes.Add(note);
                    }
                    else
                    {
                        this.CompleteNotes.Add(note);
                    }
                }
            }
        }

        public class ChangeDatetimeEventModel
        {
            public string CurrentDate { get; set; }
            public string PreviousDate { get; set; }
            public string HourSelected { get; set; }
            public IEnumerable<SelectListItem> HourBox { get; set; }
            public string MinuteSelected { get; set; }
            public IEnumerable<SelectListItem> MinuteBox { get; set; }            
            public string TimeTableSelected { get; set; }
            public IEnumerable<SelectListItem> TimeTableBox { get; set; }
            public string ID_Note { get; set; }
            public string ID_Folder { get; set; }
            public bool Localized { get; set; }

            public ChangeDatetimeEventModel(string currentDate, string previousDate, string idNote, string idFolder, bool localized)
            {
                string[] dts = previousDate.Split(' ');
                string[] ts = dts[1].Split(':');                
                this.CurrentDate = currentDate;
                this.PreviousDate = dts[0];
                this.HourSelected = ts[0];
                this.MinuteSelected = ts[1];
                this.TimeTableSelected = dts[2];
                this.HourBox = new NoteBLL().GenerateHourCombo();
                this.MinuteBox = new NoteBLL().GenerateMinuteCombo();
                this.TimeTableBox = new NoteBLL().GenerateTimeTableCombo();
                this.ID_Note = idNote;
                this.ID_Folder = idFolder;
                this.Localized = localized;
            }

            public ChangeDatetimeEventModel()
            {

            }
        }        
    }
}