using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteFeature.Controllers.Base;
using NoteFeature_App.Helpers;
using NoteFeature_App.Models.DTO;
using NoteFeature_App.Models.Note;
using NoteFeature_App.Repositories;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Claims;

namespace NoteFeature_App.Controllers.Note
{
    public class NoteController : BaseController
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

        [Authorize(Roles = "User,Admin")]
        [Route("/get-add-note-modal")]
        [HttpGet]
        public JsonResult GetAddNoteModal()
        {
            try
            {
                var html = RenderRazorViewtoString(this, "Partial_NoteCreate", null);
                
                return Json(new
                {
                    success = true,
                    html
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
        [Route("note-details/{noteId}")]
        [HttpGet]
        public JsonResult NoteDetails(Guid? noteId)
        {
            if (noteId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Note ID is required."
                });
            }
            var note = _noteRepo.GetNoteByID(noteId).FirstOrDefault();
            if (note == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Note not found."
                });
            }
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            var isUser = User.IsInRole("User");
            var isOwner = note.CreatedByUserId.ToString() == currentUserId; 
            var canEdit = isAdmin || isOwner;

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsUser = isUser;

            var html = RenderRazorViewtoString(this, "Partial_NoteEdit", note);

            return Json(new
            {
                success = true,
                html,
                canEdit
            }); 
        }

        [Authorize]
        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(NoteModel note, List<IFormFile> files)
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

            await _noteRepo.AddNote(note, files);
            return RedirectToAction("Index");
        } //Old action method page

        [Authorize(Roles = "User,Admin")]
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
                return RedirectToAction("Login", "User");
            }

            return View(note);
        }

        [Authorize(Roles = "User,Admin")]
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

            if (!isAdmin && !isOwner)
            {
                return RedirectToAction("Login", "User");
            }

            _noteRepo.UpdateNote(note);
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "User,Admin")]
        [Route("/create-note")]
        [HttpPost]
        public async Task<JsonResult> CreateNote(NoteModel note, List<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(e => e.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, errors = errors });
            }

            string? currentUserId = AuthHelper.GetCurrentUserId(HttpContext);
            note.CreatedByUserId = Guid.Parse(currentUserId);
            note.CreatedAt = DateTime.Now;
            note.NoteId = Guid.NewGuid();

            await _noteRepo.AddNote(note, files);

            return Json(new { success = true });
        } //New method

        [Authorize(Roles = "User,Admin")]
        [Route("/update-note")]
        [HttpPost]
        public JsonResult UpdateNote(NoteModel note)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(e => e.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, errors = errors });
                }

                var existing = _noteRepo.GetNoteByID(note.NoteId).FirstOrDefault();
                if (existing == null)
                {
                    return Json(new { success = false, errors = new[] { "Note not found." } });
                }

                // Check permissions
                var isAdmin = User.IsInRole("Admin");
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isOwner = existing.CreatedByUserId.ToString() == currentUserId;

                if (!isAdmin && !isOwner)
                {
                    return Json(new { success = false, errors = new[] { "You don't have permission to edit this note." } });
                }

                // Update note
                existing.NoteTitle = note.NoteTitle;
                existing.NoteContent = note.NoteContent;
                existing.IsPinned = note.IsPinned;
                existing.UpdatedAt = DateTime.Now;
                existing.UpdatedByUserId = Guid.Parse(currentUserId);

                _noteRepo.UpdateNote(existing);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { InnerException(ex) } });
            }
        }

        [Authorize(Roles = "User,Admin")]
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
                return RedirectToAction("Login", "User");
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

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Admin");
                var isUser = User.IsInRole("User");

                var notesList = result.Notes.ToList();

                ViewBag.CurrentUserId = currentUserId;
                ViewBag.IsAdmin = isAdmin;
                ViewBag.IsUser = isUser;

                var html = RenderRazorViewtoString(this, "Partial_NoteCard", notesList);

                return Json(new
                {
                    success = true,
                    notes = html,
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
