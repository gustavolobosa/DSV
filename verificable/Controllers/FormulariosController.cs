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
    public class FormulariosController : Controller
    {
        private readonly BbddverificableContext _context;

        public FormulariosController(BbddverificableContext context)
        {
            _context = context;
        }

        // GET: Formularios
        public async Task<IActionResult> Index()
        {
              return _context.Formularios != null ? 
                          View(await _context.Formularios.ToListAsync()) :
                          Problem("Entity set 'BbddverificableContext.Formularios'  is null.");
        }

        // GET: Formularios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Formularios == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formularios
                .FirstOrDefaultAsync(m => m.NumAtencion == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // GET: Formularios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Formularios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Create([Bind("NumAtencion,Cne,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumInscripcion")] Formulario formulario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formulario);
                await _context.SaveChangesAsync();

                var runRut = Request.Form["run_rut"];
                var porcentajeDerecho = Request.Form["porcentaje_derecho"];
                var noAcreditado = Request.Form["no_acreditado"];

                // Crea un nuevo objeto Enajenantes
                var nuevoEnajenante = new Enajenante
                {
                    NumAtencion = formulario.NumAtencion,
                    RunRut = runRut,
                    PorcentajeDerecho = float.Parse(porcentajeDerecho),
                    NoAcreditado = float.Parse(noAcreditado),
                };

                // Agrega el enajenante a la base de datos
                _context.Add(nuevoEnajenante);

                var adqRunRut = Request.Form["adq__run_rut"];
                var adqPorcentajeDerecho = Request.Form["adq_porcentaje_derecho"];
                var adqNoAcreditado = Request.Form["adq_no_acreditado"];

                // Crea un nuevo objeto Enajenantes
                var nuevoAdquiriente = new Adquirente
                {
                    NumAtencion = formulario.NumAtencion,
                    RunRut = adqRunRut,
                    PorcentajeDerecho = float.Parse(adqPorcentajeDerecho),
                    NoAcreditado = float.Parse(adqNoAcreditado),
                };

                // Agrega el enajenante a la base de datos
                _context.Add(nuevoAdquiriente);

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(formulario);
        }

        // GET: Formularios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Formularios == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formularios.FindAsync(id);
            if (formulario == null)
            {
                return NotFound();
            }
            return View(formulario);
        }

        // POST: Formularios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumAtencion,Cne,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumInscripcion")] Formulario formulario)
        {
            if (id != formulario.NumAtencion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(formulario.NumAtencion))
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
            return View(formulario);
        }

        // GET: Formularios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Formularios == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formularios
                .FirstOrDefaultAsync(m => m.NumAtencion == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // POST: Formularios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Formularios == null)
            {
                return Problem("Entity set 'BbddverificableContext.Formularios'  is null.");
            }
            var formulario = await _context.Formularios.FindAsync(id);
            if (formulario != null)
            {
                _context.Formularios.Remove(formulario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormularioExists(int id)
        {
          return (_context.Formularios?.Any(e => e.NumAtencion == id)).GetValueOrDefault();
        }
    }
}
