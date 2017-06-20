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

        public IEnumerable<Folder> GetAllFoldersBLL()
        {
            return folderDAL.GetAllFoldersDAL();
        }

        public void CreateFolderBLL(int id, string name, string details)
        {
            folderDAL.CreateFolderDAL(id, name, details);
        }

        public Folder GetFolderDataBLL(int folderID)
        {
            return folderDAL.GetFolderDataDAL(folderID);
        }

        public List<Note> GetNotesInFolderBLL(string userID, string folderID)
        {
            return folderDAL.GetNotesInFolderDAL(Convert.ToInt32(userID), Convert.ToInt32(folderID));
        }

        public List<SelectListItem> GetFoldersOfUserBLL(int userID)
        {
            return folderDAL.GetFoldersOfUserDAL(userID);
        }

        public bool ChangeFolderBLL(string noteID, string userID, string folderSelected)
        {
            return folderDAL.ChangeFolderDAL(Convert.ToInt32(noteID), Convert.ToInt32(userID), folderSelected);
        }
    }
}
