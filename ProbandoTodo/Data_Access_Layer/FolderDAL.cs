using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Data_Access_Layer
{
    public class FolderDAL
    {
        private WinNotesEntities OpenConnection()
        {
            WinNotesEntities WinNotesContext = new WinNotesEntities();
            return WinNotesContext;
        }

        private void CloseConnection(WinNotesEntities WinNotesContext)
        {
            WinNotesContext.Dispose();
        }

        public IEnumerable<Folder> GetAllFoldersDAL()
        {
            try
            {
                var context = OpenConnection();
                IEnumerable<Folder> folders = context.Folder.ToList();
                return folders;
            }
            catch
            {
                throw new NullReferenceException("No se pudo recuperar el listado de carpetas");
            }
        }

        public bool CreateFolderDAL(int id, string name, string details)
        {
            try
            {                
                Folder folder = new Folder();
                folder.Name = name;
                folder.Details = details;
                folder.LastModified = DateTime.Now;
                folder.Person_ID = id;

                var WinNotesContext = OpenConnection();
                WinNotesContext.Folder.Add(folder);
                WinNotesContext.SaveChanges();
                CloseConnection(WinNotesContext);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Folder GetFolderDataDAL(int folderID)
        {
            try
            {
                var context = OpenConnection();
                Folder folder = context.Folder.Where(f => f.FolderID == folderID).First();
                return folder;
            }
            catch(Exception ex)
            {                
                throw ex;
            }
        }

        public List<Note> GetNotesInFolderDAL(int userID, int folderID)
        {
            try
            {
                var context = OpenConnection();
                List<Note> notes = context.Note.Where(n => n.Folder_ID == folderID && n.Person_ID == userID).ToList();
                CloseConnection(context);
                return notes;
            }            
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListItem> GetFoldersOfUserDAL(int userID)
        {
            try
            {
                var context = OpenConnection();
                List<Folder> folders = context.Folder.Where(f => f.Person_ID == userID).ToList();
                List<SelectListItem> folderComboBox = new List<SelectListItem>();
                foreach (var folder in folders)
                {
                    folderComboBox.Add(new SelectListItem { Value = folder.Name, Text = folder.Name });
                }
                CloseConnection(context);
                return folderComboBox;
            }
            catch
            {
                List<SelectListItem> folderComboBox = new List<SelectListItem>();
                folderComboBox.Add(new SelectListItem { Value = "error", Text = "se ha producido un error" });
                return folderComboBox;
            }
        }

        public bool ChangeFolderDAL(int noteID, int userID, string folderSelected)
        {
            try
            {
                var context = OpenConnection();
                int folderID = context.Folder.Where(f => f.Name == folderSelected && f.Person_ID == userID).First().FolderID;
                Note note = context.Note.Where(n => n.NoteID == noteID).First();
                if(note.Completed != true)
                {
                    note.Folder_ID = folderID;
                    context.SaveChanges();
                }                
                CloseConnection(context);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
