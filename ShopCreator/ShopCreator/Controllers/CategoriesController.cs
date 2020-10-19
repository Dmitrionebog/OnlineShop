using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopCreator.Models;

namespace ShopCreator.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;

        public CategoriesController(DataContext context)
        {
            _context = context;
        }

        // GET: Categories
        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                var Childs = _context.Categories.Where(m => m.ParentCategoryId == null).Include(c => c.ChildCategories).ToList();
                Category CurrentAndChilds = new Category();
                CurrentAndChilds.ChildCategories = Childs;
                return View(CurrentAndChilds);
            }
            else
            {
                //var Childs = await _context.Categories.Include(c => c.ParentCategory).Include(c => c.ChildCategories)
                //.Where(m => m.ParentCategory.Id == id).ToListAsync();
                var Childs = _context.Categories.Where(m => m.ParentCategoryId == id ).Include(c => c.ChildCategories).ToList();
                
                var CurrentAndChilds = _context.Categories.Where(m => m.Id == id).FirstOrDefault();
                CurrentAndChilds.ChildCategories = Childs;

                var path = "";
                var idOfCurrentCategory = id;
                while (idOfCurrentCategory != null)
                {
                    var CurrentCategory = _context.Categories.Where(m => m.Id == idOfCurrentCategory).FirstOrDefault();
                    path =  "/" + CurrentCategory.Name + path;
                    idOfCurrentCategory = CurrentCategory.ParentCategoryId;
                }
                ViewBag.Path = path;
                return View(CurrentAndChilds);
            }


        }

        //public async Task<IActionResult> Index()
        //{

        //    return View(await _context.Categories.ToListAsync());
        //}

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c =>c.ChildCategories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateChild([Bind("Name,ParentCategoryId")] Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(category);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}

        // GET: Categories/Create
        public IActionResult Create(int? id)
        {
            int? ParentCategoryId;
            if (id != null)
            {
                 ParentCategoryId = _context.Categories.Where(cat => cat.Id == id).Select(cat => cat.Id).FirstOrDefault();
            }
            else
            {
                 ParentCategoryId = null;
            }
           
            ViewBag.ParentCategoryId = ParentCategoryId;
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ParentCategoryId")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        //public object GetTopCategories(int? id)
        //{
        //    //var categoriesFromDatabase = _context.Categories.Where(m => m.ParentCategoryId == null).Include(c => c.ChildCategories).Select(cat => new { Id = cat.Id, Name = cat.Name }).ToList();

        //    var categoriesFromDatabase = (id == null)
        //        ? (_context.Categories.Where(m => m.ParentCategoryId == null).Include(c => c.ChildCategories).Select(cat => new { Id = cat.Id, Name = cat.Name, HasChilds = (cat.ChildCategories.Count() != 0) ? true : false }).ToList())
        //        : (_context.Categories.Where(m => m.ParentCategoryId == id).Include(c => c.ChildCategories).Select(cat => new { Id = cat.Id, Name = cat.Name, HasChilds = (cat.ChildCategories.Count() != 0) ? true : false }).ToList());

        //    return categoriesFromDatabase;
        //}

        public async Task<object> GetCategories(int? id)
        {
            //var categoriesFromDatabase = _context.Categories.Where(m => m.ParentCategoryId == null).Include(c => c.ChildCategories).Select(cat => new { Id = cat.Id, Name = cat.Name }).ToList();

            var categoriesFromDatabase = (id == null)
                ? (await _context.Categories.Where(m => m.ParentCategoryId == null).Include(c => c.ChildCategories).Select(cat => new { Id = cat.Id, Name = cat.Name, HasChilds = (cat.ChildCategories.Count() != 0) ? true : false }).ToListAsync())
                : (await _context.Categories.Where(m => m.ParentCategoryId == id).Include(c => c.ChildCategories).Select(cat => new { Id = cat.Id, Name = cat.Name, HasChilds = (cat.ChildCategories.Count() != 0) ? true : false }).ToListAsync());

            return categoriesFromDatabase;
        }
    }
}
