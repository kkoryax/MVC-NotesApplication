using Microsoft.AspNetCore.Mvc;
using NoteFeature_App.Models;
using NoteFeature_App.Repositories;

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
        [Route("detail")]
        public IActionResult Detail()
        {
            return View();
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
        [Route("delete")]
        public IActionResult Delete()
        {
            return View();
        }
    }
}
