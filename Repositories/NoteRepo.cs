using NoteFeature_App.Data;
using NoteFeature_App.Models;
using Services.Helpers;

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

            if (note.IsPinned == null)
            {
                note.IsPinned = false;
            }
            note.IsPinned = true;

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
            note_find_by_id.IsPinned = note.IsPinned;
            note_find_by_id.UpdatedAt = DateTime.Now;

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
    }
}
