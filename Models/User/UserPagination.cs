using System.Drawing;
//Pagination template from P'TAR project

namespace NoteFeature_App.Models.User
{
    public class UserPagination
    {
        public int PerPage { get; set; } = 10;
        public int Page { get; set; } = 1;
        public int Offset { get; set; } = 0;
        public int Total { get; set; } = 0;

        //Parameter for filtering
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string? StatusFilter { get; set; } // <-- ADD Status Filter for sorting in advance filter 

        // Display
        public List<UserModel> Users { get; set; } = new List<UserModel>(); //GET User data

    }
}