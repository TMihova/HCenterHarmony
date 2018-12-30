using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HCH.Data;
using HCH.Models;

namespace HCH.Web.Controllers
{
    public class TherapiesController : Controller
    {
        private readonly HCHWebContext _context;

        public TherapiesController(HCHWebContext context)
        {
            _context = context;
        }

        // GET: Therapies
        public async Task<IActionResult> Index()
        {
            var hCHWebContext = _context.Therapies.Include(t => t.Patient).Include(t => t.Therapist);
            return View(await hCHWebContext.ToListAsync());
        }

        // GET: Therapies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var therapy = await _context.Therapies
                .Include(t => t.Patient)
                .Include(t => t.Therapist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (therapy == null)
            {
                return NotFound();
            }

            return View(therapy);
        }

        // GET: Therapies/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Therapies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,PatientId,TherapistId")] Therapy therapy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(therapy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", therapy.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", therapy.TherapistId);
            return View(therapy);
        }

        // GET: Therapies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var therapy = await _context.Therapies.FindAsync(id);
            if (therapy == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", therapy.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", therapy.TherapistId);
            return View(therapy);
        }

        // POST: Therapies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,StartDate,EndDate,PatientId,TherapistId")] Therapy therapy)
        {
            if (id != therapy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(therapy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TherapyExists(therapy.Id))
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
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", therapy.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", therapy.TherapistId);
            return View(therapy);
        }

        // GET: Therapies/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var therapy = await _context.Therapies
                .Include(t => t.Patient)
                .Include(t => t.Therapist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (therapy == null)
            {
                return NotFound();
            }

            return View(therapy);
        }

        // POST: Therapies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var therapy = await _context.Therapies.FindAsync(id);
            _context.Therapies.Remove(therapy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TherapyExists(string id)
        {
            return _context.Therapies.Any(e => e.Id == id);
        }
    }
}
