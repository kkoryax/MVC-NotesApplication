using Microsoft.EntityFrameworkCore;
using NoteFeature_App.Models;

namespace NoteFeature_App.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<NoteModel> Notes { get; set; }
    }
}
