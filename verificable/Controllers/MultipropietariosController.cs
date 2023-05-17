using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using verificable.Models;
using verificable.ViewModels;

namespace verificable.Controllers
{
    public class MultipropietariosController : Controller
    {
        private readonly BbddverificableContext _context;

        public MultipropietariosController(BbddverificableContext context)
        {
            _context = context;
            
            var multipropietarios = _context.Multipropietarios.OrderBy(obj => obj.VigenciaInicial).ToList();
            cleanFinalDates(multipropietarios);
            foreach (var multipropietario in multipropietarios)
            {
                var comuna = multipropietario.Comuna;
                var manzana = multipropietario.Manzana;
                var predio = multipropietario.Predio;
                var fecha = multipropietario.FechaInscripcion;
                var initialYear = multipropietario.VigenciaInicial;
                var finalYear = initialYear - 1;
                var numInscripcion = multipropietario.NumInscripcion;

                foreach(var comparison in multipropietarios)
                {
                    bool samePlace = comparison.Comuna == comuna && comparison.Manzana == manzana && comparison.Predio == predio;
                    bool minorDate = comparison.VigenciaInicial < initialYear && comparison.VigenciaFinal == null;
                    bool sameDate = comparison.VigenciaInicial == initialYear;
                    bool mayorDate = comparison.VigenciaInicial > initialYear && comparison.VigenciaFinal != null;

                    if (samePlace && minorDate)
                    {
                        comparison.VigenciaFinal = finalYear;
                    }
                    else if (samePlace && sameDate)
                    {
                        if(comparison.NumInscripcion > numInscripcion)
                        {
                            _context.Remove(multipropietario);
                        }
                        else if (comparison.NumInscripcion < numInscripcion)
                        {
                            _context.Remove(comparison);
                        }
                        
                    }
                    else if (samePlace && mayorDate)
                    {
                        multipropietario.VigenciaFinal = comparison.VigenciaInicial - 1;

                    }
                }
            }
            _context.SaveChanges();

        }

        // GET: Multipropietarios
        public async Task<IActionResult> Index(string? comunaInput, string? manzanaInput, string predioInput, string añoInput)
        {
            List<Comuna> comunas = new List<Comuna>();
            List<Multipropietario> multipropietarios = new List<Multipropietario>();
            if (!string.IsNullOrEmpty(comunaInput) && !string.IsNullOrEmpty(manzanaInput) && !string.IsNullOrEmpty(predioInput) && !string.IsNullOrEmpty(añoInput))
            {
                var comuna = Request.Form["comunaInput"];
                var manzana = Request.Form["manzanaInput"];
                var predio = Request.Form["predioInput"];
                var year = int.Parse(Request.Form["añoInput"]);

                comunas = await _context.Comunas.ToListAsync();

                multipropietarios = await _context.Multipropietarios.Where(m => m.Comuna.Contains(comuna)
                    && m.Manzana.Contains(manzana)
                    && m.Predio.Contains(predio)
                    && m.FechaInscripcion.Value.Year <= year)
                    .OrderBy(obj => obj.VigenciaInicial)
                    .ToListAsync();

                var MultipropietarioComunaViewModel = new MultipropietarioComunaViewModel
                {
                    MultipropietariosList = multipropietarios,
                    ComunasList = comunas
                };
                return View(MultipropietarioComunaViewModel);
            }
            else
            {
                comunas = await _context.Comunas.ToListAsync();


                var MultipropietarioComunaViewModel = new MultipropietarioComunaViewModel
                {
                    MultipropietariosList = multipropietarios,
                    ComunasList = comunas
                };
                return View(MultipropietarioComunaViewModel);
            }
        }

        // GET: Multipropietarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Multipropietarios == null)
            {
                return NotFound();
            }

            var multipropietario = await _context.Multipropietarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (multipropietario == null)
            {
                return NotFound();
            }

            return View(multipropietario);
        }

        // GET: Multipropietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Multipropietarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comuna,Manzana,Predio,RunRut,PorcentajeDerecho,Fojas,FechaInscripcion,NumInscripcion,VigenciaInicial,VigenciaFinal")] Multipropietario multipropietario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(multipropietario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(multipropietario);
        }

        // GET: Multipropietarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Multipropietarios == null)
            {
                return NotFound();
            }

            var multipropietario = await _context.Multipropietarios.FindAsync(id);
            if (multipropietario == null)
            {
                return NotFound();
            }
            return View(multipropietario);
        }

        // POST: Multipropietarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comuna,Manzana,Predio,RunRut,PorcentajeDerecho,Fojas,FechaInscripcion,NumInscripcion,VigenciaInicial,VigenciaFinal")] Multipropietario multipropietario)
        {
            if (id != multipropietario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(multipropietario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MultipropietarioExists(multipropietario.Id))
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
            return View(multipropietario);
        }

        // GET: Multipropietarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Multipropietarios == null)
            {
                return NotFound();
            }

            var multipropietario = await _context.Multipropietarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (multipropietario == null)
            {
                return NotFound();
            }

            return View(multipropietario);
        }

        // POST: Multipropietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Multipropietarios == null)
            {
                return Problem("Entity set 'BbddverificableContext.Multipropietarios'  is null.");
            }
            var multipropietario = await _context.Multipropietarios.FindAsync(id);
            if (multipropietario != null)
            {
                _context.Multipropietarios.Remove(multipropietario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MultipropietarioExists(int id)
        {
          return (_context.Multipropietarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private void cleanFinalDates(List<Multipropietario> multipropietarioList)
        {
            foreach (var multipropietario in multipropietarioList)
            {
                multipropietario.VigenciaFinal = null;
            }
        }
    }
}
