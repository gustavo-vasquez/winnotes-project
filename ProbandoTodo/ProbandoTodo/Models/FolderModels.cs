using Domain_Layer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProbandoTodo.Models
{
    public class FolderModels
    {
        public class FolderListModelView
        {
            public int FolderID { get; set; }
            public string Name { get; set; }
            public string Details { get; set; }
            public DateTime LastModified { get; set; }
        }

        public class CreateFolderModelView
        {
            [Required]
            public string Name { get; set; }

            [Required]
            public string Details { get; set; }
        }

        public class EditFolderModelView
        {
            public int FolderID { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public string Details { get; set; }
        }

        public class ClassifiedNotes
        {
            public List<NoteInformation> PendingNotes { get; set; }
            public List<NoteInformation> CompleteNotes { get; set; }

            public ClassifiedNotes(IEnumerable<Note> notes)
            {
                this.PendingNotes = new List<NoteInformation>();
                this.CompleteNotes = new List<NoteInformation>();

                foreach (var note in notes)
                {
                    if(!Convert.ToBoolean(note.Completed))
                    {
                        this.PendingNotes.Add(new NoteInformation()
                        {
                            NoteID = note.NoteID,
                            Title = note.Title,
                            Details = note.Details,
                            Starred = Convert.ToBoolean(note.Starred),
                            ExpirationDate = note.ExpirationDate,
                            Completed = Convert.ToBoolean(note.Completed)
                        });
                    }
                    else
                    {
                        this.CompleteNotes.Add(new NoteInformation()
                        {
                            NoteID = note.NoteID,
                            Title = note.Title,
                            Details = note.Details,
                            Starred = Convert.ToBoolean(note.Starred),
                            ExpirationDate = note.ExpirationDate,
                            Completed = Convert.ToBoolean(note.Completed)
                        });
                    }
                }
            }
        }

        public class NoteInformation
        {
            public int NoteID { get; set; }
            public string Title { get; set; }
            public string Details { get; set; }
            public DateTime ExpirationDate { get; set; }
            public bool Starred { get; set; }
            public bool Completed { get; set; }
        }

        public class ContentInFolderModelView
        {
            public FolderListModelView FolderData { get; set; }
            public ClassifiedNotes NoteList { get; set; }

            public ContentInFolderModelView(Folder folder, IEnumerable<Note> notes)
            {
                this.FolderData = new FolderListModelView();
                this.FolderData.FolderID = folder.FolderID;
                this.FolderData.Name = folder.Name;
                this.FolderData.Details = folder.Details;
                this.FolderData.LastModified = folder.LastModified;
                this.NoteList = new ClassifiedNotes(notes);
            }
        }

        public class ChangeFolderModelView
        {
            public int FolderID { get; set; }
            public int NoteID { get; set; }
            public string CurrentFolder { get; set; }
            public IEnumerable<SelectListItem> FoldersComboBox { get; set; }
            public string FolderSelected { get; set; }
        }
    }
}