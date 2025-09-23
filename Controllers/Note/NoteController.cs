using Microsoft.AspNetCore.Mvc;
using NoteFeature_App.Models;
using NoteFeature_App.Repositories;
using NoteFeature_App.Models.NotePagination;

namespace NoteFeature_App.Controllers.Note
{
    public class NoteController : Controller
    {
        private readonly INoteRepo _noteRepo;

        public NoteController(INoteRepo noteRepo)
        {
            _noteRepo = noteRepo;
        }

        [Route("/")]
        [Route("home")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("detail/{noteId}")]
        public IActionResult Detail(Guid? noteId)
        {
            if (noteId == null)
            {
                return NotFound();
            }

            var note = _noteRepo.GetNoteByID(noteId).FirstOrDefault();
            return View(note);
        }
        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
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

            _noteRepo.AddNote(note);
            return RedirectToAction("Index");
        }
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

            return View(note);
        }
        [Route("edit/{noteId}")]
        [HttpPost]
        public IActionResult Edit (NoteModel note)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _noteRepo.UpdateNote(note);
            return RedirectToAction("Index");

        }
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

            _noteRepo.DeleteNote(noteId);

            return RedirectToAction("Index");
        }

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
                    updatedAt = n.UpdatedAt.HasValue ? n.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm") : null
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
