using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PotholesApp.Models;

namespace PotholesApp.Controllers
{
    public class HolesController : Controller
    {
        private readonly PotholeDetectorAppContext _context;

        public HolesController(PotholeDetectorAppContext context)
        {
            _context = context;
        }

        // GET: Holes
        public async Task<IActionResult> Index()
        {
              return _context.Holes != null ? 
                          View(await _context.Holes.ToListAsync()) :
                          Problem("Entity set 'PotholeDetectorAppContext.Holes'  is null.");
        }

        // GET: Holes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Holes == null)
            {
                return NotFound();
            }

            var hole = await _context.Holes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hole == null)
            {
                return NotFound();
            }

            return View(hole);
        }

        // GET: Holes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Holes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Latitude,Longitude,Location,Img,DetectDate")] Hole hole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hole);
        }

        // GET: Holes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Holes == null)
            {
                return NotFound();
            }

            var hole = await _context.Holes.FindAsync(id);
            if (hole == null)
            {
                return NotFound();
            }
            return View(hole);
        }

        // POST: Holes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Latitude,Longitude,Location,Img,DetectDate")] Hole hole)
        {
            if (id != hole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoleExists(hole.Id))
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
            return View(hole);
        }

        // GET: Holes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Holes == null)
            {
                return NotFound();
            }

            var hole = await _context.Holes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hole == null)
            {
                return NotFound();
            }

            return View(hole);
        }

        // POST: Holes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Holes == null)
            {
                return Problem("Entity set 'PotholeDetectorAppContext.Holes'  is null.");
            }
            var hole = await _context.Holes.FindAsync(id);
            if (hole != null)
            {
                _context.Holes.Remove(hole);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoleExists(int id)
        {
          return (_context.Holes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
