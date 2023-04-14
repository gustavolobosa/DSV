using System;
using System.Collections.Generic;
using System.Globalization;
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
            ViewData["Enajenantes"] = await _context.Enajenantes
                .Where(e => e.NumAtencion == id)
                .ToListAsync();
            ViewData["Adquirientes"] = await _context.Adquirentes
                .Where(e => e.NumAtencion == id)
                .ToListAsync();
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
            //formulario.FechaInscripcion = DateTime.ParseExact(Request.Form["fecha_inscripcion"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (ModelState.IsValid)
            {
                _context.Add(formulario);
                await _context.SaveChangesAsync();

                int numEnajenantes = 0;
                foreach (string key in Request.Form.Keys)
                {
                    if (key.StartsWith("enajenantes") && key.EndsWith("run_rut"))
                    {
                        numEnajenantes++;
                    }
                }

                int numAdquirentes = 0;
                foreach (string key in Request.Form.Keys)
                {
                    if (key.StartsWith("adquirentes") && key.EndsWith("run_rut"))
                    {
                        numAdquirentes++;
                    }
                }

                //Console.WriteLine("num of enajenante: " + numEnajenantes);
                //Console.WriteLine("num of adquriente: " + numAdquirentes);

                for (int i = 0; i < numEnajenantes; i++)
                {
                    string runRut = Request.Form["enajenantes[" + i + "].run_rut"];
                    decimal? porcentajeDerecho = null;
                    if (!string.IsNullOrEmpty(Request.Form["enajenantes[" + i + "].porcentaje_derecho"]))
                    {
                        porcentajeDerecho = decimal.Parse(Request.Form["enajenantes[" + i + "].porcentaje_derecho"]);
                    }
                    bool noAcreditado = false;
                    string checkboxValue = Request.Form["enajenantes[" + i + "].no_acreditado"].ToString().ToLower();
                    if (checkboxValue == "on" || checkboxValue == "true")
                    {
                        noAcreditado = true;
                    }
                    _context.Add(new Enajenante { RunRut = runRut, PorcentajeDerecho = (double?)porcentajeDerecho, NoAcreditado = (bool?)noAcreditado, NumAtencion = formulario.NumAtencion });
                }

                for (int i = 0; i < numAdquirentes; i++)
                {
                    string runRut = Request.Form["adquirentes[" + i + "].run_rut"];
                    decimal? porcentajeDerecho = null;
                    if (!string.IsNullOrEmpty(Request.Form["adquirentes[" + i + "].porcentaje_derecho"]))
                    {
                        porcentajeDerecho = decimal.Parse(Request.Form["adquirentes[" + i + "].porcentaje_derecho"]);
                    }
                    bool noAcreditado = false;
                    string checkboxValue = Request.Form["adquirentes[" + i + "].no_acreditado"].ToString().ToLower();
                    if (checkboxValue == "on" || checkboxValue == "true")
                    {
                        noAcreditado = true;
                    }
                    _context.Add(new Adquirente { RunRut = runRut, PorcentajeDerecho = (double?)porcentajeDerecho, NoAcreditado = (bool?)noAcreditado, NumAtencion = formulario.NumAtencion });
                    if (formulario.Cne == "Regularizacion De Patrimonio")
                    {
                        _context.Add(new Multipropietario {Comuna = formulario.Comuna, Manzana = formulario.Manzana, Predio = formulario.Predio, RunRut = runRut, 
                                                           PorcentajeDerecho = (double?)porcentajeDerecho, Fojas = formulario.Fojas, FechaInscripcion = formulario.FechaInscripcion, 
                                                           NumInscripcion = formulario.NumInscripcion, VigenciaInicial = formulario.FechaInscripcion });
                    }
                }

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
                // Delete associated enajenantes first
                var enajenantes = _context.Enajenantes.Where(e => e.NumAtencion == id);
                foreach (var enajenante in enajenantes)
                {
                    _context.Enajenantes.Remove(enajenante);
                }

                // Delete associated adquirentes first
                var adquirentes = _context.Adquirentes.Where(a => a.NumAtencion == id);
                foreach (var adquirente in adquirentes)
                {
                    _context.Adquirentes.Remove(adquirente);
                }
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
