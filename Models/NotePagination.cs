using NoteFeature_App.Models;
using System.Drawing;
//Pagination template from P'TAR project

namespace NoteFeature_App.Models.NotePagination
{
    public class NotePagination
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

        // Display
        public List<NoteModel> Notes { get; set; } = new List<NoteModel>(); //GET Note data

    }
}