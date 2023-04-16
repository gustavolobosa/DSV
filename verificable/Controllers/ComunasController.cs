using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using verificable.Models;

namespace verificable.Controllers
{
    public class ComunasController : Controller
    {
        private readonly BbddverificableContext _context;

        public ComunasController(BbddverificableContext context)
        {
            _context = context;
        }
        // GET: ComunasController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ComunasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ComunasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComunasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComunasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComunasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComunasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ComunasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
