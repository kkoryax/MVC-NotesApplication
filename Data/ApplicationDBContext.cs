using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoteFeature_App.Data.Identity;
using NoteFeature_App.Models.Note;

namespace NoteFeature_App.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<NoteModel> Notes { get; set; }
    }
}
