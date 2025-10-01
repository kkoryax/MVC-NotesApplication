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
        public DbSet<NoteFile> NoteFiles { get; set; }

        //relationship config
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure NoteFile relationship
            modelBuilder.Entity<NoteFile>()
                .HasOne(nf => nf.Note)
                .WithMany(n => n.NoteFiles)
                .HasForeignKey(nf => nf.NoteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
