 using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopCreator.BL;
using ShopCreator.Models;
using ShopCreator.Models.ViewModels;

namespace ShopCreator.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private readonly DataContext _context;

        public PropertiesController(DataContext context)
        {
            _context = context;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            List<Property> properties = _context.Properties
            .FromSqlRaw("SELECT * FROM dbo.Properties")
            .ToList();
            return View(properties);
            //return View(await _context.Properties.ToListAsync());
            // TODO rewrite to SQL ( don't use _context.Properties )
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            // Пытаюсь сформировать список PropertyTypes для передачи в представление
            // SelectList proptypes = new SelectList(GetPropertyTypes);

            //var Name = new Property();

            ViewBag.PropertyTypes = Enum.GetValues(typeof(ShopCreator.Models.ViewModels.PropertyType)).Cast<ShopCreator.Models.ViewModels.PropertyType>().
                  Select(p => new SelectListItem() { Text = p.ToString(), Value = ((int)p).ToString() }).ToList();
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PropertyType,ValidValues")] PropertyViewModel property)
        {
            if (ModelState.IsValid)
            {
                if (property.ValidValues == null)
                {
                    var commandText = "INSERT Properties (Name,PropertyType) VALUES (@Name,@PropertyType)";
                    var Name = new SqlParameter("@Name", property.Name);
                   var PropertyType = new SqlParameter("@PropertyType", (int)property.PropertyType);
                     _context.Database.ExecuteSqlRaw(commandText, Name, PropertyType);
                }
                else
                {
                    var commandText = "INSERT Properties (Name,PropertyType,ValidValues) VALUES (@Name,@PropertyType,@ValidValues)";
                    var Name = new SqlParameter("@Name", property.Name);
                    var PropertyType = new SqlParameter("@PropertyType", property.PropertyType);
                    var PropertyTypeValue = new SqlParameter("@ValidValues", property.ValidValues);
                    _context.Database.ExecuteSqlCommand(commandText, Name, PropertyType, PropertyTypeValue);
                } 
                //_context.Add(@property);
                //await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(property);
            
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties.FindAsync(id);
            if (@property == null)
            {
                return NotFound();
            }
            return View(@property);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PropertyType,ValidValues")] PropertyViewModel property)
        {
            if (id != property.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dbProperty = await _context.Properties
                  .FirstOrDefaultAsync(pr => pr.Id == property.Id);
                    Converters.ConvertToDBProperty(property, dbProperty);
                    _context.Update(dbProperty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(property.Id))
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
            return View(property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
    }
}
