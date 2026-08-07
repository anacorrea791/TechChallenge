using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechChallenge;
using TechChallenge.Models;

namespace TechChallenge.Controllers
{
     [Authorize] //Proteger as controller AlunosController, usando o atributo [Authorize],configurando autentificação/autorização.
    public class AlunosEquipesController : Controller
    {
        private readonly AppDbContext _context;

        public AlunosEquipesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AlunosEquipes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.AlunosEquipes.Include(a => a.Aluno).Include(a => a.Equipe);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AlunosEquipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alunoEquipe = await _context.AlunosEquipes
                .Include(a => a.Aluno)
                .Include(a => a.Equipe)
                .FirstOrDefaultAsync(m => m.AlunoId == id);
            if (alunoEquipe == null)
            {
                return NotFound();
            }

            return View(alunoEquipe);
        }

        // GET: AlunosEquipes/Create
        public IActionResult Create()
        {
            ViewData["AlunoId"] = new SelectList(_context.Alunos, "Id", "Id");
            ViewData["EquipeId"] = new SelectList(_context.Equipes, "Id", "Nome");
            return View();
        }

        // POST: AlunosEquipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlunoId,EquipeId,DataEntrada")] AlunoEquipe alunoEquipe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alunoEquipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlunoId"] = new SelectList(_context.Alunos, "Id", "Id", alunoEquipe.AlunoId);
            ViewData["EquipeId"] = new SelectList(_context.Equipes, "Id", "Nome", alunoEquipe.EquipeId);
            return View(alunoEquipe);
        }

        // GET: AlunosEquipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alunoEquipe = await _context.AlunosEquipes.FindAsync(id);
            if (alunoEquipe == null)
            {
                return NotFound();
            }
            ViewData["AlunoId"] = new SelectList(_context.Alunos, "Id", "Id", alunoEquipe.AlunoId);
            ViewData["EquipeId"] = new SelectList(_context.Equipes, "Id", "Nome", alunoEquipe.EquipeId);
            return View(alunoEquipe);
        }

        // POST: AlunosEquipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlunoId,EquipeId,DataEntrada")] AlunoEquipe alunoEquipe)
        {
            if (id != alunoEquipe.AlunoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alunoEquipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlunoEquipeExists(alunoEquipe.AlunoId))
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
            ViewData["AlunoId"] = new SelectList(_context.Alunos, "Id", "Id", alunoEquipe.AlunoId);
            ViewData["EquipeId"] = new SelectList(_context.Equipes, "Id", "Nome", alunoEquipe.EquipeId);
            return View(alunoEquipe);
        }

        // GET: AlunosEquipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alunoEquipe = await _context.AlunosEquipes
                .Include(a => a.Aluno)
                .Include(a => a.Equipe)
                .FirstOrDefaultAsync(m => m.AlunoId == id);
            if (alunoEquipe == null)
            {
                return NotFound();
            }

            return View(alunoEquipe);
        }

        // POST: AlunosEquipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alunoEquipe = await _context.AlunosEquipes.FindAsync(id);
            if (alunoEquipe != null)
            {
                _context.AlunosEquipes.Remove(alunoEquipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlunoEquipeExists(int id)
        {
            return _context.AlunosEquipes.Any(e => e.AlunoId == id);
        }
    }
}
