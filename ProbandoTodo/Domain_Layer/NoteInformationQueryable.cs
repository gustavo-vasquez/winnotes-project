using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer
{
    public class NoteInformationQueryable
    {
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool? Starred { get; set; }
        public bool? Completed { get; set; }
        public int BelongsToFolderID { get; set; }
        public string BelongsToFolderName { get; set; }
    }
}
