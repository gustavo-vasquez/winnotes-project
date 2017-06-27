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
        public IEnumerable<Folder> GetAllFoldersDAL()
        {
            try
            {
                using(var context = new WinNotesDBEntities())
                {
                    IEnumerable<Folder> folders = context.Folder.ToList();
                    return folders;
                }
            }
            catch
            {
                throw new NullReferenceException("No se pudo recuperar el listado de carpetas");
            }
        }

        public void CreateFolderDAL(int id, string name, string details)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {
                    if(!context.Folder.Any(f => f.Name == name))
                    {
                        Folder folder = new Folder();
                        folder.Name = name;
                        folder.Details = details;
                        folder.LastModified = DateTime.Now;
                        folder.Person_ID = id;
                        context.Folder.Add(folder);
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new ArgumentException("Ya existe una carpeta con ese nombre");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void EditFolderDAL(int userID, int folderID, string name, string details)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {
                    var folder = context.Folder.Where(f => f.Person_ID == userID && f.FolderID == folderID).First();
                    folder.Name = name;
                    folder.Details = details;
                    folder.LastModified = DateTime.Now;
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public Folder GetFolderDataDAL(int folderID)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {
                    Folder folder = context.Folder.Where(f => f.FolderID == folderID).First();
                    return folder;
                }                
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
                using (var context = new WinNotesDBEntities())
                {
                    List<Note> notes = context.Note.Where(n => n.Folder_ID == folderID && n.Person_ID == userID).ToList();                    
                    return notes;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SelectListItem> GetFoldersOfUserDAL(int userID)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {                    
                    List<Folder> folders = context.Folder.Where(f => f.Person_ID == userID).ToList();
                    List<SelectListItem> folderComboBox = new List<SelectListItem>();
                    foreach (var folder in folders)                    
                        folderComboBox.Add(new SelectListItem { Value = folder.Name, Text = folder.Name });
                                        
                    return folderComboBox;
                }                    
            }
            catch
            {
                throw new ArgumentException("No se han podido cargar las carpetas disponibles");
            }
        }

        public void ChangeFolderDAL(int noteID, int userID, string folderSelected)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {
                    int folderID = context.Folder.Where(f => f.Name == folderSelected && f.Person_ID == userID).First().FolderID;
                    Note note = context.Note.Where(n => n.NoteID == noteID).First();
                    if (note.Completed != false)
                        throw new ArgumentException("Esta nota ya se completó");

                    note.Folder_ID = folderID;
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
