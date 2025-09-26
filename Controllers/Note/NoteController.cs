using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteFeature_App.Helpers;
using NoteFeature_App.Models.Note;
using NoteFeature_App.Repositories;
using System.Security.Claims;

namespace NoteFeature_App.Controllers.Note
{
    public class NoteController : Controller
    {
        private readonly INoteRepo _noteRepo;

        public NoteController(INoteRepo noteRepo)
        {
            _noteRepo = noteRepo;
        }

        [Authorize]
        [Route("home")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("detail/{noteId}")]
        public IActionResult Detail(Guid? noteId)
        {
            if (noteId == null)
            {
                return NotFound();
            }

            var note = _noteRepo.GetNoteByID(noteId).FirstOrDefault();

            ViewBag.CreatedByUserEmail = note.CreatedByUser?.Email;
            ViewBag.UpdatedByUserEmail = note.UpdatedByUser?.Email;
            ViewBag.CreatedByUserId = note.CreatedByUserId;
            ViewBag.UpdatedByUserId = note.UpdatedByUserId;

            return View(note);
        }

        [Authorize]
        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [Route("create")]
        [HttpPost]
        public IActionResult Create(NoteModel note)
        {
            if (!ModelState.IsValid)
            {
                List<NoteModel> notes = _noteRepo.GetAllNote();

                ViewBag.Notes = notes;
                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                return View();
            }

            string? currentUserId = AuthHelper.GetCurrentUserId(HttpContext);
            note.CreatedByUserId = Guid.Parse(currentUserId);

            _noteRepo.AddNote(note);
            return RedirectToAction("Index");
        }

        [Authorize]
        [Route("edit/{noteId}")]
        [HttpGet]
        public IActionResult Edit(Guid? noteId)
        {
            if (noteId == null || noteId == Guid.Empty)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                List<NoteModel> notes = _noteRepo.GetAllNote();

                ViewBag.Notes = notes;
                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                return View();
            }

            NoteModel? note = _noteRepo.GetNoteByID(noteId).FirstOrDefault();

            var isAdmin = User.IsInRole("Admin");
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isOwner = note.CreatedByUserId.ToString() == currentUserId;

            if (!isAdmin && !isOwner)
            {
                return Forbid();
            }

            return View(note);
        }

        [Authorize]
        [Route("edit/{noteId}")]
        [HttpPost]
        public IActionResult Edit (NoteModel note)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var existing = _noteRepo.GetNoteByID(note.NoteId).FirstOrDefault();
            if (existing == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isOwner = existing.CreatedByUserId.ToString() == currentUserId;

            if (!isAdmin && !isOwner) return Forbid();

            _noteRepo.UpdateNote(note);
            return RedirectToAction("Index");

        }

        [Authorize]
        [Route("delete/{noteId}")]
        [HttpPost]
        public IActionResult Delete(Guid? noteId)
        {
            if (noteId == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                List<NoteModel> notes = _noteRepo.GetAllNote();

                ViewBag.Notes = notes;
                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                return View();
            }

            var note = _noteRepo.GetNoteByID(noteId).FirstOrDefault();
            if (note == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isOwner = note.CreatedByUserId.ToString() == currentUserId;

            if (!isAdmin && !isOwner)
            {
                return Forbid();
            }

            _noteRepo.DeleteNote(noteId);

            return RedirectToAction("Index");
        }

        [Authorize]
        [Route("/get-note-list")]
        [HttpGet]
        public JsonResult GetNoteList(NotePagination pagination)
        {
            try
            {
                if (pagination == null) pagination = new NotePagination();
                pagination.Offset = pagination.Page <= 1 ? 0 : pagination.Offset;

                var result = _noteRepo.GetListNotePagination(pagination);

                var notesDto = result.Notes.Select(n => new
                {
                    noteId = n.NoteId,
                    noteTitle = n.NoteTitle,
                    noteContent = n.NoteContent,
                    isPinned = n.IsPinned == true,
                    createdAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    createdByUserId = n.CreatedByUserId,
                    createdByUserEmail = n.CreatedByUser?.Email,
                    updatedAt = n.UpdatedAt.HasValue ? n.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm") : null,
                    updatedByUserId = n.UpdatedByUserId,
                    updatedByUserEmail = n.UpdatedByUser?.Email,
                }).ToList();

                return Json(new
                {
                    success = true,
                    notes = notesDto,
                    total = result.Total
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = InnerException(ex)
                });
            }
        }
        protected string InnerException(Exception ex)
        {
            return (ex.InnerException != null) ? InnerException(ex.InnerException) : ex.Message;
        }
    }
}
