using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer;
using Domain_Layer;
using System.Web.Mvc;

namespace Business_Logic_Layer
{
    public class FolderBLL
    {
        static private FolderDAL folderDAL = new FolderDAL();

        public IEnumerable<Folder> GetAllFoldersBLL(int userID)
        {
            return folderDAL.GetAllFoldersDAL(userID);
        }

        public void CreateFolderBLL(int id, string name, string details)
        {
            folderDAL.CreateFolderDAL(id, name, details);
        }

        public void EditFolderBLL(int userID, int folderID, string name, string details)
        {
            folderDAL.EditFolderDAL(userID, folderID, name, details);
        }

        public void RemoveFolderBLL(int userID, int folderID)
        {
            folderDAL.RemoveFolderDAL(userID, folderID);
        }

        public Folder GetFolderDataBLL(int folderID)
        {
            return folderDAL.GetFolderDataDAL(folderID);
        }

        public List<Note> GetNotesInFolderBLL(int userID, int folderID)
        {
            return folderDAL.GetNotesInFolderDAL(userID, folderID);
        }

        public List<SelectListItem> GetFoldersOfUserBLL(int userID)
        {
            return folderDAL.GetFoldersOfUserDAL(userID);
        }

        public void ChangeFolderBLL(int noteID, int userID, string folderSelected)
        {
            folderDAL.ChangeFolderDAL(noteID, userID, folderSelected);
        }
    }
}
