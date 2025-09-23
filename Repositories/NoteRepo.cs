using NoteFeature_App.Data;
using Services.Helpers;
using NoteFeature_App.Models.Note;

namespace NoteFeature_App.Repositories
{
    //Service contracts
    public interface INoteRepo
    {
        //Action Method List
        List<NoteModel> GetAllNote();
        List<NoteModel> GetNoteByID(Guid? noteId);
        void AddNote(NoteModel? note);
        void UpdateNote(NoteModel? note);
        void DeleteNote(Guid? noteId);
        NotePagination GetListNotePagination(NotePagination pagination);

    }

    //service implementation
    public class NoteRepo : INoteRepo
    {
        #region DBSetting
        private readonly ApplicationDBContext _db;

        //DB Constructor
        public NoteRepo(ApplicationDBContext db)
        {
            _db = db;
        }
        #endregion

        //Implementation of Action Method List
        //GET ALL NOTE
        public List<NoteModel> GetAllNote()
        {
            return _db.Notes.Where(n => n.FlagActive == true).ToList();
        }

        //GET NOTE BY ID
        public List<NoteModel> GetNoteByID(Guid? noteId)
        {
            if (noteId == null)
            {
                return null;
            }

            return _db.Notes.Where(n => n.NoteId == noteId && n.FlagActive == true).ToList();
        }

        //ADD NOTE
        public void AddNote(NoteModel? note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            ValidationHelper.ModelValidation(note);

            //Add default values
            note.NoteId = Guid.NewGuid();
            note.FlagActive = true;

                _db.Notes.Add(note);

            _db.SaveChanges();
        }

        //UPDATE NOTE
        public void UpdateNote(NoteModel? note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            ValidationHelper.ModelValidation(note);

            //GET NOTE BY ID
            NoteModel? note_find_by_id = _db.Notes.FirstOrDefault(n => n.NoteId == note.NoteId);

            if (note_find_by_id == null)
            {
                throw new ArgumentException("ไม่พบข้อมูลที่ต้องการแก้ไข");
            }

            //UPDATE VALUE
            note_find_by_id.NoteTitle = note.NoteTitle;
            note_find_by_id.NoteContent = note.NoteContent;
            note_find_by_id.UpdatedAt = DateTime.Now;

            if (note.IsPinned == true)
            {
                note_find_by_id.IsPinned = true;
            } else
            {
                note_find_by_id.IsPinned = false;
            }

                _db.Notes.Update(note_find_by_id);
            _db.SaveChanges();
        }

        //DELETE NOTE
        public void DeleteNote(Guid? noteId)
        {
            if (noteId == null)
            {
                throw new ArgumentNullException(nameof(noteId));
            }

            //GET NOTE BY ID
            NoteModel? note_find_by_id = _db.Notes.FirstOrDefault(n => n.NoteId == noteId);
            
            if (note_find_by_id == null)
            {
                throw new ArgumentException("ไม่พบข้อมูลที่ต้องการลบ");
            }

            //SOFT DELETE
            note_find_by_id.FlagActive = false;

            _db.Notes.Update(note_find_by_id);
            _db.SaveChanges();
        }

        public NotePagination GetListNotePagination(NotePagination pagination)
        {
            NotePagination Notes = new NotePagination();

            var perPage = pagination.PerPage;
            var skip = pagination.Offset;
            var search = pagination.Search ?? string.Empty;

            var sort = pagination.Sort ?? "CreatedAt desc";

            var fromDate = pagination.FromDate.Date;
            var toDate = pagination.ToDate.HasValue ? pagination.ToDate.Value.Date : DateTime.MaxValue.Date;

            var query = _db.Notes.AsQueryable();

            query = query.Where(n => n.FlagActive == true);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(n => n.NoteTitle.Contains(search) || n.NoteContent.Contains(search));
            }
            if (fromDate != null)
            {
                query = query.Where(n => n.CreatedAt.Date >= fromDate);
            }
            if (toDate != DateTime.MaxValue.Date)
            {
                query = query.Where(n => n.CreatedAt.Date <= toDate);
            }

            // Order query
            if (sort == "CreatedAt desc")
            {
                query = query
                        .OrderByDescending(n => n.IsPinned == true)
                        .ThenByDescending(n => (n.UpdatedAt ?? n.CreatedAt));
            } else
            {
                query = query
                        .OrderByDescending(n => n.IsPinned == true)
                        .ThenBy(n => (n.UpdatedAt ?? n.CreatedAt));
            }

                Notes.Total = query.Count();

            var result = query
                        .Skip(skip)
                        .Take(perPage)
                        .AsEnumerable()
                        .ToList();
            Notes.Notes = result;

            return Notes;
        }
    }
}
