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
    public class TreatmentsController : Controller
    {
        private readonly HCHWebContext _context;

        public TreatmentsController(HCHWebContext context)
        {
            _context = context;
        }

        // GET: Treatments
        public async Task<IActionResult> Index()
        {
            var hCHWebContext = _context.Treatments.Include(t => t.Profile);
            return View(await hCHWebContext.ToListAsync());
        }

        // GET: Treatments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments
                .Include(t => t.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // GET: Treatments/Create
        public IActionResult Create()
        {
            ViewData["ProfileId"] = new SelectList(_context.Profiles, "Id", "Id");
            return View();
        }

        // POST: Treatments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProfileId,Name,Description,Price")] Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(treatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfileId"] = new SelectList(_context.Profiles, "Id", "Id", treatment.ProfileId);
            return View(treatment);
        }

        // GET: Treatments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }
            ViewData["ProfileId"] = new SelectList(_context.Profiles, "Id", "Id", treatment.ProfileId);
            return View(treatment);
        }

        // POST: Treatments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ProfileId,Name,Description,Price")] Treatment treatment)
        {
            if (id != treatment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentExists(treatment.Id))
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
            ViewData["ProfileId"] = new SelectList(_context.Profiles, "Id", "Id", treatment.ProfileId);
            return View(treatment);
        }

        // GET: Treatments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments
                .Include(t => t.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // POST: Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var treatment = await _context.Treatments.FindAsync(id);
            _context.Treatments.Remove(treatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentExists(string id)
        {
            return _context.Treatments.Any(e => e.Id == id);
        }
    }
}
