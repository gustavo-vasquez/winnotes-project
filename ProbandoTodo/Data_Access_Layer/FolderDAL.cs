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
        public IEnumerable<sp_getUserFolders_Result> GetAllFoldersDAL(int userID)
        {
            try
            {
                using(var context = new WinNotesEntities())
                {
                    //IEnumerable<Folder> folders = context.Folder.Where(f => f.Person_ID == userID).ToList();
                    List<sp_getUserFolders_Result> folders = context.sp_getUserFolders(userID).ToList();
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
                using (var context = new WinNotesEntities())
                {
                    #region FORMA ALTERNATIVA
                    //if(!context.Folder.Any(f => f.Name == name))
                    //{
                    //    Folder folder = new Folder();
                    //    folder.Name = name;
                    //    folder.Details = details;
                    //    folder.LastModified = DateTime.Now;
                    //    folder.Person_ID = id;
                    //    context.Folder.Add(folder);
                    //    context.SaveChanges();
                    //}
                    //else
                    //{
                    //    throw new ArgumentException("Ya existe una carpeta con ese nombre");
                    //}
                    #endregion

                    context.sp_createNewFolder(id, name, details);
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
                using (var context = new WinNotesEntities())
                {
                    #region FORMA ALTERNATIVA
                    //var folder = context.Folder.Where(f => f.Person_ID == userID && f.FolderID == folderID).First();
                    //folder.Name = name;
                    //folder.Details = details;
                    //folder.LastModified = DateTime.Now;
                    //context.SaveChanges();
                    #endregion

                    context.sp_editFolder(userID, folderID, name, details);
                }
            }
            catch
            {
                throw;
            }
        }

        public void RemoveFolderDAL(int userID, int folderID)
        {
            try
            {
                using(var context = new WinNotesEntities())
                {
                    #region FORMA ALTERNATIVA
                    //var notes = context.Note.Where(n => n.Person_ID == userID && n.Folder_ID == folderID);
                    //context.Note.RemoveRange(notes);
                    //var folder = context.Folder.Where(f => f.Person_ID == userID && f.FolderID == folderID).First();
                    //context.Folder.Remove(folder);
                    //context.SaveChanges();                    
                    #endregion

                    context.sp_removeFolder(userID, folderID);
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
                using (var context = new WinNotesEntities())
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
                using (var context = new WinNotesEntities())
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
                using (var context = new WinNotesEntities())
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
                using (var context = new WinNotesEntities())
                {
                    int folderID = context.Folder.Where(f => f.Name == folderSelected && f.Person_ID == userID).First().FolderID;
                    Note note = context.Note.Where(n => n.NoteID == noteID).First();
                    if (note.Completed != true)
                    {
                        note.Folder_ID = folderID;
                        context.SaveChanges();
                    }                    
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
