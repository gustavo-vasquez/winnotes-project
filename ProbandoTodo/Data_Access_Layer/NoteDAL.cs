using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Data_Access_Layer
{
    public class NoteDAL
    {
        /// <summary>
        /// Devuelve las carpetas del usuario que se van listar en el combo
        /// </summary>
        /// <param name="userID">ID del usuario</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetFoldersToSelectDAL(int userID)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {                    
                    List<Folder> foldersToList = context.Folder.Where(f => f.Person_ID.Equals(userID)).ToList();
                    List<SelectListItem> listOfFolders = new List<SelectListItem>();
                    listOfFolders.Add(new SelectListItem() { Value = String.Empty, Text = "Sin carpeta...", Selected = true });

                    foreach (var f in foldersToList)
                    {
                        listOfFolders.Add(new SelectListItem() { Value = f.Name, Text = f.Name });
                    }                    

                    return listOfFolders;
                }                
            }
            catch
            {
                List<SelectListItem> listOfFolders = new List<SelectListItem>();
                listOfFolders.Add(new SelectListItem() { Value = String.Empty, Text = "Sin carpeta...", Selected = true });
                SelectList listFolderComplete = new SelectList(listOfFolders);

                return listFolderComplete;
            }
        }

        /// <summary>
        /// Devuelve la información del usuario que se mostrará en el panel de la izquierda al crear una nota
        /// </summary>
        /// <param name="userID">ID del usuario</param>
        /// <returns></returns>
        public string[] GetUserBoxInfoDAL(int userID)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {                    
                    Person user = context.Person.Where(p => p.PersonID == userID).First();
                    int foldersCount = context.Folder.Where(f => f.Person_ID == userID).Count();
                    int notesCount = context.Note.Where(n => n.Person_ID == userID).Count();

                    string[] userBoxInfo = null;
                    userBoxInfo = new string[]
                    {
                        user.UserName,
                        user.PersonalPhrase,
                        new UserDAL().GetAvatarImage(user.AvatarImage, user.AvatarMIMEType),
                        foldersCount.ToString(),
                        notesCount.ToString()
                    };

                    return userBoxInfo;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva nota y devuelve true o false
        /// </summary>
        /// <param name="title">Titulo</param>
        /// <param name="details">Detalles</param>
        /// <param name="expirationDate">Fecha de expiración</param>
        /// <param name="starred">Destacado</param>
        /// <param name="folderSelected">Carpeta elegida</param>
        /// <returns></returns>
        public void CreateNoteDAL(int userID, string title, string details, DateTime expirationDate, bool starred, string folderSelected, int hourSelected, int minuteSelected, string timeTableSelected, ref int folderAuxID)
        {
            try
            {
                if (timeTableSelected.Equals("PM"))
                {
                    if (hourSelected < 12)
                        hourSelected += 12;
                    else
                        hourSelected = 0;
                }

                expirationDate = new DateTime(expirationDate.Year, expirationDate.Month, expirationDate.Day, hourSelected, minuteSelected, 0);

                using (var context = new WinNotesDBEntities())
                {                    
                    Note newNote = new Note();
                    newNote.Title = title;
                    newNote.Details = details;
                    newNote.ExpirationDate = expirationDate;
                    newNote.Starred = starred;
                    newNote.Completed = false;
                    newNote.Folder_ID = context.Folder.Where(f => f.Name.Equals(folderSelected)).First().FolderID;
                    newNote.Person_ID = userID;
                    context.Note.Add(newNote);
                    context.SaveChanges();
                    folderAuxID = newNote.Folder_ID;
                }
            }
            catch
            {
                throw;
            }
        }

        public void ForceCompleteTaskDAL(int noteID)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {
                    Note note = new Note();
                    note = context.Note.Where(n => n.NoteID == noteID).First();
                    if (note.Completed != true)
                    {
                        note.Completed = true;
                        context.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }           
        }

        public void StarTaskDAL(int noteID)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {
                    Note note = new Note();
                    note = context.Note.Where(n => n.NoteID == noteID).First();
                    if (note.Completed != true)
                    {
                        if (note.Starred != false)
                            note.Starred = false;
                        else
                            note.Starred = true;

                        context.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }           
        }

        public IQueryable<NoteInformationQueryable> GetDataForNoteList(int userID)
        {
            try
            {
                var context = new WinNotesDBEntities();                
                IQueryable<NoteInformationQueryable> notes = context.Note.Join(context.Folder,
                                                                            n => n.Folder_ID,
                                                                            f => f.FolderID,
                                                                            (n, f) => new { n, f })
                                                                            .Where(parameters => parameters.n.Person_ID == userID)
                                                                            .Select(m => new NoteInformationQueryable
                                                                            {
                                                                                NoteID = m.n.NoteID,
                                                                                Title = m.n.Title,
                                                                                Details = m.n.Details,
                                                                                ExpirationDate = m.n.ExpirationDate,
                                                                                Starred = m.n.Starred,
                                                                                Completed = m.n.Completed,
                                                                                BelongsToFolderID = m.f.FolderID,
                                                                                BelongsToFolderName = m.f.Name
                                                                            });                    
                return notes;
            }
            catch
            {
                throw;
            }
        }

        public void ChangeDatetimeEventDAL(string currentDate, int hour, int minute, string timeTable, int id_note, int userID)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {
                    var note = context.Note.Where(n => n.NoteID.Equals(id_note) && n.Person_ID.Equals(userID)).First();
                    if (note.Completed != true)
                    {
                        List<int> date = currentDate.Split('/').Select(int.Parse).ToList();

                        if (timeTable == "PM" && hour < 12)
                            hour = hour + 12;
                        else if (timeTable == "AM" && hour == 12)
                            hour = 0;

                        DateTime datetimeParsed = new DateTime(date[2], date[1], date[0], hour, minute, 0);
                        note.ExpirationDate = datetimeParsed;
                        context.SaveChanges();
                    }                    
                }                    
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<Note> CheckExpiredEventsDAL(string encryptedUser)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {                    
                    int userID = context.Person.Where(p => p.PersonIDEncrypted == encryptedUser).First().PersonID;
                    List<Note> notes = context.Note.Where(n => n.Person_ID == userID && n.Completed != true).ToList();
                    List<Note> expiredNotes = new List<Note>();

                    foreach (var note in notes)
                    {
                        bool isExpired = (note.ExpirationDate - DateTime.Now).TotalDays <= 0;
                        if (isExpired)
                        {
                            expiredNotes.Add(note);
                            note.Completed = true;
                            context.SaveChanges();
                        }
                    }

                    return expiredNotes;
                }                    
            }
            catch
            {
                throw;
            }
        }
    }
}
