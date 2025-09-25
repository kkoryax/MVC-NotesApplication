using Microsoft.EntityFrameworkCore;
using NoteFeature_App.Models.Note;
using NoteFeature_App.Models.User;

namespace NoteFeature_App.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<NoteModel> Notes { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
