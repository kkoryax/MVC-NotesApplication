using Microsoft.EntityFrameworkCore;
using NoteFeature_App.Data;
using NoteFeature_App.Models.Note;
using NoteFeature_App.Helpers;
using Services.Helpers;
using System.Threading.Tasks;

namespace NoteFeature_App.Repositories
{
    //Service contracts
    public interface INoteRepo
    {
        //Action Method List
        List<NoteModel> GetAllNote();
        List<NoteModel> GetNoteByID(Guid? noteId);
        Task AddNote(NoteModel? note, List<IFormFile>? files = null);
        void UpdateNote(NoteModel? note);
        void DeleteNote(Guid? noteId);
        NotePagination GetListNotePagination(NotePagination pagination, string? currentUserId = null, bool isAdmin = false);

    }

    //service implementation
    public class NoteRepo : INoteRepo
    {
        #region DBSetting
        private readonly ApplicationDBContext _db;
        private readonly IWebHostEnvironment _webHost;
        private readonly IHostEnvironment _hosting;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //DB Constructor
        public NoteRepo(ApplicationDBContext db,
                    IWebHostEnvironment webHost,
                    IHostEnvironment hosting,
                    IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _webHost = webHost;
            _hosting = hosting;
            _httpContextAccessor = httpContextAccessor;
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

            return _db.Notes
                .Include(n => n.CreatedByUser)
                .Include(n => n.UpdatedByUser)
                .Include(n => n.NoteFiles)
                .Where(n => n.NoteId == noteId && n.FlagActive == true)
                .ToList();
        }

        //ADD NOTE
        public async Task AddNote(NoteModel? note, List<IFormFile>? files)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            ValidationHelper.ModelValidation(note);

            if (note.CreatedByUserId.HasValue)
            {
                var user = _db.Users.FirstOrDefault(u => u.UserId == note.CreatedByUserId.Value);
                if (user != null)
                {
                    note.CreatedByUser = user;
                }

                //Add default values
                note.FlagActive = true;

                //check is public now or not via helper
                note.IsPublic = CaculateIsPublicHelper.CalculateIsPublic(note.ActiveFrom, note.ActiveUntil);

                    /* Add Note File data */
                note.NoteFiles = new List<NoteFile>();

                var request = _httpContextAccessor.HttpContext?.Request;
                string savePath = $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
                string uploadPath = _hosting.ContentRootPath;
                string folderName = "Upload";

                string? uploadsFolder = Path.Combine(_webHost.ContentRootPath, "Upload"); 

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Process uploaded files
                if (files != null && files.Count > 0)
                {         
                    //loop create files
                    foreach (var file in files)
                    {
                        if (file.Length > 0)    
                        {
                            string rawFileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string fileName = Guid.NewGuid().ToString();
                            string fileExtension = Path.GetExtension(file.FileName);
                            //get .jpg .png bla bla to save in path
                            string fileNameWithExtension = fileName + fileExtension;
                            string fullSavePath = Path.Combine(savePath, folderName, fileNameWithExtension);

                            string physicalFilePath = Path.Combine(uploadsFolder, fileNameWithExtension);
                            string urlPath = $"{savePath.TrimEnd('/')}/{folderName}/{fileNameWithExtension}";

                            using (FileStream stream = new FileStream(physicalFilePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            note.NoteFiles.Add(new NoteFile
                            {
                                NoteFileId = Guid.NewGuid(),
                                NoteId = note.NoteId,
                                NoteFileName = rawFileName + fileExtension,
                                NoteFilePath = urlPath,
                                NoteFileSize = file.Length,
                                NoteFileType = file.ContentType,
                                UploadedDate = DateTime.Now
                            });
                        }
                    }
                }

                _db.Notes.Add(note);

                await _db.SaveChangesAsync();
            } 
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
            note_find_by_id.UpdatedByUserId = note.UpdatedByUserId;   
            note_find_by_id.ActiveFrom = note.ActiveFrom;
            note_find_by_id.ActiveUntil = note.ActiveUntil;
            note_find_by_id.IsPublic = CaculateIsPublicHelper.CalculateIsPublic(note.ActiveFrom, note.ActiveUntil);

            if (note.UpdatedByUserId.HasValue)
            {
                var user = _db.Users.FirstOrDefault(u => u.UserId == note.UpdatedByUserId.Value);
                if (user != null)
                {
                    note_find_by_id.UpdatedByUser = user; 
                }
            }

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

        public NotePagination GetListNotePagination(NotePagination pagination, string? currentUserId = null, bool isAdmin = false)
        {
            NotePagination Notes = new NotePagination();

            var perPage = pagination.PerPage;
            var skip = pagination.Offset;
            var search = pagination.Search ?? string.Empty;

            var sort = pagination.Sort ?? "CreatedAt desc";

            var fromDate = pagination.FromDate.Date;
            var toDate = pagination.ToDate.HasValue ? pagination.ToDate.Value.Date : DateTime.MaxValue.Date;
            var statusFilter = FilterHelper.ParseStatusFilter(pagination.StatusFilter);

            var query = _db.Notes
                            .Include(n => n.CreatedByUser)
                            .Include(n => n.UpdatedByUser)
                            .Include(n => n.NoteFiles)
                            .AsQueryable();

            query = query.Where(n => n.FlagActive == true);
            
            if (!statusFilter.Contains("Unpublish"))
            {
                query = query.Where(n => n.IsPublic == true);
            }

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

            // Apply Advance Status Filter
            if (statusFilter != null && statusFilter.Any())
            {
                if (statusFilter.Contains("Edit"))
                {
                    query = query.Where(n => n.UpdatedAt.HasValue);
                }
                if (statusFilter.Contains("Nonedit"))
                {
                    query = query.Where(n => !n.UpdatedAt.HasValue);
                }
                if (statusFilter.Contains("File"))
                {
                    query = query.Where(n => n.NoteFiles.Any());
                }
                if (statusFilter.Contains("Nonfile"))
                {
                    query = query.Where(n => !n.NoteFiles.Any());
                }
                if (statusFilter.Contains("Pinned"))
                {
                    query = query.Where(n => n.IsPinned == true);
                }
                if (statusFilter.Contains("Unpinned"))
                {
                    query = query.Where(n => n.IsPinned == false);
                }
                if (statusFilter.Contains("Timelimit"))
                {
                    query = query.Where(n => n.ActiveUntil != null && n.IsPublic == true);
                }
                if (statusFilter.Contains("Nonlimit"))
                {
                    query = query.Where(n => !n.ActiveUntil.HasValue);
                }
                if (statusFilter.Contains("Expiredsoon"))
                {
                    query = query
                            .Where(n => n.ActiveUntil.HasValue && 
                                      n.ActiveUntil != null && 
                                      n.IsPublic == true &&
                                      n.ActiveUntil.Value >= DateTime.Now);
                }
                if (statusFilter.Contains("Unpublish"))
                {
                    if (isAdmin)
                    {
                        query = query.Where(n => n.IsPublic == false && n.ActiveUntil.HasValue && n.ActiveUntil >= DateTime.Now);
                    }
                    else if (!string.IsNullOrEmpty(currentUserId))
                    {
                        var userId = Guid.Parse(currentUserId);
                        query = query.Where(n => n.IsPublic == false && 
                                                n.ActiveUntil.HasValue && 
                                                n.ActiveUntil >= DateTime.Now &&
                                                n.CreatedByUserId == userId);
                    }
                    else
                    {
                        query = query.Where(n => false);
                    }
                }
            }

            //Order queue
            if (statusFilter.Contains("Expiredsoon") && sort == "CreatedAt desc")
            {
                query = query
                        .OrderByDescending(n => n.IsPinned == true)
                        .ThenBy(n => n.ActiveUntil.Value);
            } else if (statusFilter.Contains("Expiredsoon") && sort == "CreatedAt asc")
            {
                query = query
                        .OrderByDescending(n => n.IsPinned == true)
                        .ThenByDescending(n => n.ActiveUntil.Value);
            }
            else if (sort == "CreatedAt desc")
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
