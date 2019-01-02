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
    public class ExaminationsController : Controller
    {
        private readonly HCHWebContext _context;

        public ExaminationsController(HCHWebContext context)
        {
            _context = context;
        }

        // GET: Examinations
        public async Task<IActionResult> Index()
        {
            var hCHWebContext = _context.Examinations.Include(e => e.Patient).Include(e => e.Therapist).Include(e => e.Therapy);
            return View(await hCHWebContext.ToListAsync());
        }

        // GET: Examinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Therapist)
                .Include(e => e.Therapy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examination == null)
            {
                return NotFound();
            }

            return View(examination);
        }

        // GET: Examinations/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["TherapyId"] = new SelectList(_context.Therapies, "Id", "Id");
            return View();
        }

        // POST: Examinations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExaminationDate,TherapistId,PatientId,Anamnesis,TherapyId")] Examination examination)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", examination.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", examination.TherapistId);
            ViewData["TherapyId"] = new SelectList(_context.Therapies, "Id", "Id", examination.TherapyId);
            return View(examination);
        }

        // GET: Examinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations.FindAsync(id);
            if (examination == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", examination.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", examination.TherapistId);
            ViewData["TherapyId"] = new SelectList(_context.Therapies, "Id", "Id", examination.TherapyId);
            return View(examination);
        }

        // POST: Examinations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExaminationDate,TherapistId,PatientId,Anamnesis,TherapyId")] Examination examination)
        {
            if (id != examination.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExaminationExists(examination.Id))
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
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", examination.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", examination.TherapistId);
            ViewData["TherapyId"] = new SelectList(_context.Therapies, "Id", "Id", examination.TherapyId);
            return View(examination);
        }

        // GET: Examinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Therapist)
                .Include(e => e.Therapy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examination == null)
            {
                return NotFound();
            }

            return View(examination);
        }

        // POST: Examinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examination = await _context.Examinations.FindAsync(id);
            _context.Examinations.Remove(examination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExaminationExists(int id)
        {
            return _context.Examinations.Any(e => e.Id == id);
        }
    }
}
