using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using verificable.Models;

namespace verificable.Controllers
{
    public class EnajenantesController : Controller
    {
        private readonly BbddverificableContext _context;

        public EnajenantesController(BbddverificableContext context)
        {
            _context = context;
        }

        // GET: Enajenantes
        public async Task<IActionResult> Index()
        {
            var bbddverificableContext = _context.Enajenantes.Include(e => e.NumAtencionNavigation);
            return View(await bbddverificableContext.ToListAsync());
        }

        // GET: Enajenantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Enajenantes == null)
            {
                return NotFound();
            }

            var enajenante = await _context.Enajenantes
                .Include(e => e.NumAtencionNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enajenante == null)
            {
                return NotFound();
            }

            return View(enajenante);
        }

        // GET: Enajenantes/Create
        public IActionResult Create()
        {
            ViewData["NumAtencion"] = new SelectList(_context.Formularios, "NumAtencion", "NumAtencion");
            return View();
        }

        // POST: Enajenantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumAtencion,RunRut,PorcentajeDerecho,NoAcreditado")] Enajenante enajenante)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enajenante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NumAtencion"] = new SelectList(_context.Formularios, "NumAtencion", "NumAtencion", enajenante.NumAtencion);
            return View(enajenante);
        }

        // GET: Enajenantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enajenantes == null)
            {
                return NotFound();
            }

            var enajenante = await _context.Enajenantes.FindAsync(id);
            if (enajenante == null)
            {
                return NotFound();
            }
            ViewData["NumAtencion"] = new SelectList(_context.Formularios, "NumAtencion", "NumAtencion", enajenante.NumAtencion);
            return View(enajenante);
        }

        // POST: Enajenantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumAtencion,RunRut,PorcentajeDerecho,NoAcreditado")] Enajenante enajenante)
        {
            if (id != enajenante.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enajenante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnajenanteExists(enajenante.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NumAtencion"] = new SelectList(_context.Formularios, "NumAtencion", "NumAtencion", enajenante.NumAtencion);
            return View(enajenante);
        }

        // GET: Enajenantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enajenantes == null)
            {
                return NotFound();
            }

            var enajenante = await _context.Enajenantes
                .Include(e => e.NumAtencionNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enajenante == null)
            {
                return NotFound();
            }

            return View(enajenante);
        }

        // POST: Enajenantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enajenantes == null)
            {
                return Problem("Entity set 'BbddverificableContext.Enajenantes'  is null.");
            }
            var enajenante = await _context.Enajenantes.FindAsync(id);
            if (enajenante != null)
            {
                _context.Enajenantes.Remove(enajenante);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnajenanteExists(int id)
        {
          return (_context.Enajenantes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
